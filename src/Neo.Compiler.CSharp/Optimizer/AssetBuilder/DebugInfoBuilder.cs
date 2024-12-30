using Neo.Json;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Neo.Optimizer
{
    static class DebugInfoBuilder
    {
        private static readonly Regex RangeRegex = new(@"(\d+)\-(\d+)", RegexOptions.Compiled);
        private static readonly Regex SequencePointRegex = new(@"(\d+)(\[\d+\]\d+\:\d+\-\d+\:\d+)", RegexOptions.Compiled);

        /// <summary>
        /// Modify debug info to fit simplified instructions
        /// </summary>
        /// <param name="debugInfo">Debug information</param>
        /// <param name="simplifiedInstructionsToAddress">new Instruction => int address</param>
        /// <param name="oldAddressToInstruction">old int address => Instruction</param>
        /// <param name="oldSequencePointAddressToNew">old int address => new int address</param>
        /// <returns></returns>
        public static JObject? ModifyDebugInfo(JObject? debugInfo,
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress,
            Dictionary<int, Instruction> oldAddressToInstruction,
            Dictionary<int, int>? oldSequencePointAddressToNew = null)
        {
            if (debugInfo == null) return null;
            //Dictionary<int, (int docId, int startLine, int startCol, int endLine, int endCol)> newAddrToSequencePoint = new();
            HashSet<JToken> methodsToRemove = new();
            foreach (JToken? method in (JArray)debugInfo["methods"]!)
            {
                GroupCollection rangeGroups = RangeRegex.Match(method!["range"]!.AsString()).Groups;
                (int oldMethodStart, int oldMethodEnd) = (int.Parse(rangeGroups[1].ToString()), int.Parse(rangeGroups[2].ToString()));
                int methodStart;
                if (simplifiedInstructionsToAddress.Contains(oldAddressToInstruction[oldMethodStart]))
                    methodStart = (int)simplifiedInstructionsToAddress[oldAddressToInstruction[oldMethodStart]]!;
                else if (oldSequencePointAddressToNew?.TryGetValue(oldMethodStart, out methodStart) != true)
                {
                    methodsToRemove.Add(method);
                    continue;
                }
                // The instruction at the end of the method may have been deleted.
                // We need to find the last instruction that is not deleted.
                if (oldSequencePointAddressToNew?.TryGetValue(oldMethodEnd, out var methodEnd) != true)
                {
                    //int methodEnd = (int)simplifiedInstructionsToAddress[oldAddressToInstruction[oldMethodEnd]]!;
                    int oldMethodEndNotDeleted = oldAddressToInstruction.Where(kv =>
                    kv.Key >= oldMethodStart && kv.Key <= oldMethodEnd &&
                    simplifiedInstructionsToAddress.Contains(kv.Value)
                    ).Max(kv => kv.Key);
                    methodEnd = (int)simplifiedInstructionsToAddress[oldAddressToInstruction[oldMethodEndNotDeleted]]!;
                }
                method["range"] = $"{methodStart}-{methodEnd}";

                int previousSequencePoint = methodStart;
                JArray newSequencePoints = [];
                foreach (JToken? sequencePoint in (JArray)method!["sequence-points"]!)
                {
                    GroupCollection sequencePointGroups = SequencePointRegex.Match(sequencePoint!.AsString()).Groups;
                    int startInstructionAddress = int.Parse(sequencePointGroups[1].ToString());
                    Instruction oldInstruction = oldAddressToInstruction[startInstructionAddress];
                    if (simplifiedInstructionsToAddress.Contains(oldInstruction))
                    {
                        startInstructionAddress = (int)simplifiedInstructionsToAddress[oldInstruction]!;
                        newSequencePoints.Add(new JString($"{startInstructionAddress}{sequencePointGroups[2]}"));
                        previousSequencePoint = startInstructionAddress;
                    }
                    else if (oldSequencePointAddressToNew != null
                        && oldSequencePointAddressToNew.TryGetValue(startInstructionAddress, out int newStartInstructionAddress))
                    {
                        newSequencePoints.Add(new JString($"{newStartInstructionAddress}{sequencePointGroups[2]}"));
                        previousSequencePoint = newStartInstructionAddress;
                    }
                    else
                        newSequencePoints.Add(new JString($"{previousSequencePoint}{sequencePointGroups[2]}"));
                }
                method["sequence-points"] = newSequencePoints;

                if (method["sequence-points-v2"] is JObject sequencePointsV2)
                {
                    JObject newSequencePointsV2 = new();
                    previousSequencePoint = methodStart;
                    foreach ((string addr, JToken? content) in sequencePointsV2.Properties)
                    {
                        int startInstructionAddress = int.Parse(addr);
                        Instruction oldInstruction = oldAddressToInstruction[startInstructionAddress];
                        string writeAddr;
                        if (simplifiedInstructionsToAddress.Contains(oldInstruction))
                        {
                            startInstructionAddress = (int)simplifiedInstructionsToAddress[oldInstruction]!;
                            writeAddr = startInstructionAddress.ToString();
                            previousSequencePoint = startInstructionAddress;
                        }
                        else if (oldSequencePointAddressToNew != null
                            && oldSequencePointAddressToNew.TryGetValue(startInstructionAddress, out int newStartInstructionAddress))
                        {
                            writeAddr = newStartInstructionAddress.ToString();
                            previousSequencePoint = newStartInstructionAddress;
                            if (content is JArray oldContentArray)
                                foreach (JToken? i in oldContentArray)
                                    ((JObject)i!)["optimization"] = Neo.Compiler.CompilationOptions.OptimizationType.Experimental.ToString().ToLowerInvariant();
                            else if (content is JObject oldContentObject)
                                content["optimization"] = Neo.Compiler.CompilationOptions.OptimizationType.Experimental.ToString().ToLowerInvariant();
                        }
                        else
                            // previousSequencePoint unchanged
                            writeAddr = previousSequencePoint.ToString();
                        switch (newSequencePointsV2[writeAddr])
                        // TODO: compress newSequencePointsV2 array to non-duplicating array
                        {
                            case null:
                                newSequencePointsV2[writeAddr] = content;
                                break;
                            case JObject existingObject:
                                if (content is JArray contentArr)
                                    newSequencePointsV2[writeAddr] = new JArray() { existingObject }.Concat(contentArr).ToArray();
                                else if (content is JObject contentObj)
                                    newSequencePointsV2[writeAddr] = new JArray() { existingObject, content };
                                break;
                            case JArray existingArray:
                                if (content is JArray contentArray)
                                    newSequencePointsV2[writeAddr] = existingArray.Concat(contentArray).ToArray();
                                else if (content is JObject contentObject)
                                    existingArray.Add(contentObject);
                                break;
                            default:
                                throw new BadScriptException($"Invalid sequence-points-v2 in debug info: key {addr}, value {content}");
                        }
                    }
                    method["sequence-points-v2"] = newSequencePointsV2;
                }
                if (method["abi"] is JObject abi && abi["offset"] != null)
                {
                    int offset = int.Parse(abi["offset"]!.ToString());
                    if (simplifiedInstructionsToAddress.Contains(oldAddressToInstruction[offset]))
                        offset = (int)simplifiedInstructionsToAddress[oldAddressToInstruction[offset]]!;
                    else
                        oldSequencePointAddressToNew?.TryGetValue(offset, out offset);
                    abi["offset"] = offset;
                }
            }
            JArray methods = (JArray)debugInfo["methods"]!;
            foreach (JToken method in methodsToRemove)
                methods.Remove(method);
            return debugInfo;
        }
    }
}
