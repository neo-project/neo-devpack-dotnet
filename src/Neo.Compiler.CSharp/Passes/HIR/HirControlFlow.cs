using System;
using System.Collections.Generic;
using Neo.Compiler.Analysis;

namespace Neo.Compiler.HIR.Optimization;

internal static class HirControlFlow
{
    public static Dictionary<HirBlock, HashSet<HirBlock>> BuildPredecessors(HirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        return ControlFlowAnalysis.BuildPredecessors(function.Blocks, GetSuccessors);
    }

    public static IReadOnlyDictionary<HirBlock, HashSet<HirBlock>> ComputeDominators(HirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        return ControlFlowAnalysis.ComputeDominators(function.Blocks, function.Entry, GetSuccessors);
    }

    public static IReadOnlyList<LoopAnalysis.LoopInfo<HirBlock>> FindNaturalLoops(HirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        return LoopAnalysis.FindNaturalLoops(function.Blocks, function.Entry, GetSuccessors);
    }

    public static IEnumerable<HirBlock> GetSuccessors(HirBlock block)
    {
        if (block is null)
            yield break;

        foreach (var instruction in block.Instructions)
        {
            if (instruction is HirTryFinallyScope tryScope)
            {
                yield return tryScope.TryBlock;
                yield return tryScope.FinallyBlock;
                foreach (var handler in tryScope.CatchHandlers)
                    yield return handler.Block;
            }
        }

        switch (block.Terminator)
        {
            case HirBranch branch:
                yield return branch.Target;
                break;
            case HirConditionalBranch cond:
                yield return cond.TrueBlock;
                yield return cond.FalseBlock;
                break;
            case HirSwitch @switch:
                foreach (var (_, target) in @switch.Cases)
                    yield return target;
                yield return @switch.DefaultTarget;
                break;
            case HirLeave leave:
                yield return leave.Target;
                break;
            case HirEndFinally endFinally:
                yield return endFinally.Target;
                break;
        }
    }
}
