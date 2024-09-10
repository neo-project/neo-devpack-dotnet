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
        /// <param name="debugInfo"></param>
        /// <param name="simplifiedInstructionsToAddress">new Instruction => int address</param>
        /// <param name="oldAddressToInstruction"></param>
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
                if (!simplifiedInstructionsToAddress.Contains(oldAddressToInstruction[oldMethodStart]))
                {
                    methodsToRemove.Add(method);
                    continue;
                }
                int methodStart = (int)simplifiedInstructionsToAddress[oldAddressToInstruction[oldMethodStart]]!;
                // The instruction at the end of the method may have been deleted.
                // We need to find the last instruction that is not deleted.
                //int methodEnd = (int)simplifiedInstructionsToAddress[oldAddressToInstruction[oldMethodEnd]]!;
                int oldMethodEndNotDeleted = oldAddressToInstruction.Where(kv =>
                kv.Key >= oldMethodStart && kv.Key <= oldMethodEnd &&
                simplifiedInstructionsToAddress.Contains(kv.Value)
                ).Max(kv => kv.Key);
                int methodEnd = (int)simplifiedInstructionsToAddress[oldAddressToInstruction[oldMethodEndNotDeleted]]!;
                method["range"] = $"{methodStart}-{methodEnd}";

                int previousSequencePoint = methodStart;
                JArray newSequencePoints = new();
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
                        previousSequencePoint = startInstructionAddress;
                    }
                    else
                        newSequencePoints.Add(new JString($"{previousSequencePoint}{sequencePointGroups[2]}"));
                }
                method["sequence-points"] = newSequencePoints;
            }
            JArray methods = (JArray)debugInfo["methods"]!;
            foreach (JToken method in methodsToRemove)
                methods.Remove(method);
            return debugInfo;
        }
    }
}
