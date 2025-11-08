using System;
using System.Collections.Generic;

namespace Neo.Compiler.MIR.Optimization;

internal static class MirControlFlow
{
    internal static IEnumerable<MirBlock> GetSuccessors(MirBlock block)
    {
        if (block is null)
            yield break;

        foreach (var inst in block.Instructions)
        {
            if (inst is MirTry tryInst)
            {
                yield return tryInst.TryBlock;
                yield return tryInst.FinallyBlock;
                foreach (var handler in tryInst.CatchHandlers)
                    yield return handler.Block;
            }
        }

        switch (block.Terminator)
        {
            case MirBranch branch:
                yield return branch.Target;
                break;
            case MirCondBranch cond:
                yield return cond.TrueTarget;
                yield return cond.FalseTarget;
                break;
            case MirCompareBranch cmp:
                yield return cmp.TrueTarget;
                yield return cmp.FalseTarget;
                break;
            case MirSwitch @switch:
                foreach (var (_, target) in @switch.Cases)
                    yield return target;
                yield return @switch.DefaultTarget;
                break;
            case MirLeave leave:
                yield return leave.Target;
                break;
            case MirEndFinally endFinally:
                yield return endFinally.Target;
                break;
        }
    }
}
