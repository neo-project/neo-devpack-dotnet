using Neo.VM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Neo.Optimizer.OpCodeTypes;
using static Neo.Optimizer.Optimizer;

namespace Neo.Optimizer
{
    static class OptimizedScriptBuilder
    {
        /// <summary>
        /// Build script with instruction dictionary and jump
        /// </summary>
        /// <param name="simplifiedInstructionsToAddress">new Instruction => int address</param>
        /// <param name="jumpSourceToTargets">All jumping instructions source => target</param>
        /// <param name="trySourceToTargets">All try instructions source => target</param>
        /// <param name="oldAddressToInstruction">Used for convenient debugging only</param>
        /// <returns></returns>
        /// <exception cref="BadScriptException"></exception>
        public static Script BuildScriptWithJumpTargets(
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress,
            Dictionary<Instruction, Instruction> jumpSourceToTargets,
            Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
            Dictionary<int, Instruction>? oldAddressToInstruction = null)
        {
            List<byte> simplifiedScript = new();
            foreach (DictionaryEntry item in simplifiedInstructionsToAddress)
            {
                (Instruction i, int a) = ((Instruction)item.Key, (int)item.Value!);
                simplifiedScript.Add((byte)i.OpCode);
                int operandSizeLength = OperandSizePrefixTable[(int)i.OpCode];
                simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(i.Operand.Length)[0..operandSizeLength]).ToList();
                if (jumpSourceToTargets.TryGetValue(i, out Instruction? dst))
                {
                    int delta;
                    if (simplifiedInstructionsToAddress.Contains(dst))  // target instruction not deleted
                        delta = (int)simplifiedInstructionsToAddress[dst]! - a;
                    else if (i.OpCode == OpCode.PUSHA || i.OpCode == OpCode.ENDTRY || i.OpCode == OpCode.ENDTRY_L)
                        delta = 0;  // TODO: decide a good target
                    else
                    {
                        if (oldAddressToInstruction != null)
                            foreach ((int oldAddress, Instruction oldInstruction) in oldAddressToInstruction)
                                if (oldInstruction == i)
                                    throw new BadScriptException($"Target instruction of {i.OpCode} at old address {oldAddress} is deleted");
                        throw new BadScriptException($"Target instruction of {i.OpCode} at new address {a} is deleted");
                    }
                    if (i.OpCode == OpCode.JMP || conditionalJump.Contains(i.OpCode) || i.OpCode == OpCode.CALL || i.OpCode == OpCode.ENDTRY)
                        if (sbyte.MinValue <= delta && delta <= sbyte.MaxValue)
                            simplifiedScript.Add(BitConverter.GetBytes(delta)[0]);
                        else
                            // TODO: build with _L version
                            throw new NotImplementedException($"Need {i.OpCode}_L for delta={delta}");
                    if (i.OpCode == OpCode.PUSHA || i.OpCode == OpCode.JMP_L || conditionalJump_L.Contains(i.OpCode) || i.OpCode == OpCode.CALL_L || i.OpCode == OpCode.ENDTRY_L)
                        simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(delta)).ToList();
                    continue;
                }
                if (trySourceToTargets.TryGetValue(i, out (Instruction dst1, Instruction dst2) dsts))
                {
                    (Instruction dst1, Instruction dst2) = (dsts.dst1, dsts.dst2);
                    (int delta1, int delta2) = ((int)simplifiedInstructionsToAddress[dst1]! - a, (int)simplifiedInstructionsToAddress[dst2]! - a);
                    if (i.OpCode == OpCode.TRY)
                    {
                        simplifiedScript.Add(BitConverter.GetBytes(delta1)[0]);
                        simplifiedScript.Add(BitConverter.GetBytes(delta2)[0]);
                    }
                    if (i.OpCode == OpCode.TRY_L)
                    {
                        simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(delta1)).ToList();
                        simplifiedScript = simplifiedScript.Concat(BitConverter.GetBytes(delta2)).ToList();
                    }
                    continue;
                }
                if (i.Operand.Length != 0)
                    simplifiedScript = simplifiedScript.Concat(i.Operand.ToArray()).ToList();
            }
            Script script = new(simplifiedScript.ToArray());
            return script;
        }

        /// <summary>
        /// Typically used when you delete the oldTarget from script
        /// and the newTarget is the first following instruction undeleted in script
        /// </summary>
        /// <param name="oldTarget">target instruction in the old script</param>
        /// <param name="newTarget">target instruction in the new script</param>
        /// <param name="jumpSourceToTargets">All jumping instructions source => target</param>
        /// <param name="trySourceToTargets">All try instructions source => target</param>
        /// <param name="jumpTargetToSources">All target instructions that are targets of jumps and trys</param>
        public static void RetargetJump(Instruction oldTarget, Instruction newTarget,
            Dictionary<Instruction, Instruction> jumpSourceToTargets,
            Dictionary<Instruction, (Instruction, Instruction)> trySourceToTargets,
            Dictionary<Instruction, HashSet<Instruction>> jumpTargetToSources)
        {
            if (jumpTargetToSources.Remove(oldTarget, out HashSet<Instruction>? sources) && sources.Count > 0)
            {
                foreach (Instruction s in sources)
                {
                    if (jumpSourceToTargets.TryGetValue(s, out Instruction? t0) && t0 == oldTarget)
                        jumpSourceToTargets[s] = newTarget;
                    if (trySourceToTargets.TryGetValue(s, out (Instruction t1, Instruction t2) t))
                    {
                        Instruction newT1 = (t.t1 == oldTarget ? newTarget : t.t1);
                        Instruction newT2 = (t.t2 == oldTarget ? newTarget : t.t2);
                        trySourceToTargets[s] = (newT1, newT2);
                    }
                }
                if (jumpTargetToSources.TryGetValue(newTarget, out HashSet<Instruction>? newTargetSources))
                    sources = newTargetSources.Union(sources).ToHashSet();
                jumpTargetToSources[newTarget] = sources;
            }
        }
    }
}
