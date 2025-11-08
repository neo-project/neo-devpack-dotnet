using System;
using System.Numerics;

namespace Neo.Compiler.MIR.Optimization;

internal sealed class MirBranchSimplificationPass : IMirPass
{
    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var changed = false;

        foreach (var block in function.Blocks)
        {
            switch (block.Terminator)
            {
                case MirCondBranch cond when TryEvaluate(cond.Condition, out var value):
                    block.Terminator = new MirBranch(value ? cond.TrueTarget : cond.FalseTarget)
                    { Span = cond.Span };
                    changed = true;
                    break;
                case MirSwitch @switch when TryGetInt(@switch.Key, out var key):
                    block.Terminator = new MirBranch(FindTarget(@switch, key))
                    { Span = @switch.Span };
                    changed = true;
                    break;
            }
        }

        return changed;
    }

    private static bool TryEvaluate(MirValue value, out bool result)
    {
        switch (value)
        {
            case MirConstBool constBool:
                result = constBool.Value;
                return true;
            case MirConstInt constInt:
                result = constInt.Value != BigInteger.Zero;
                return true;
            default:
                result = false;
                return false;
        }
    }

    private static bool TryGetInt(MirValue value, out BigInteger constant)
    {
        if (value is MirConstInt constInt)
        {
            constant = constInt.Value;
            return true;
        }

        constant = default;
        return false;
    }

    private static MirBlock FindTarget(MirSwitch @switch, BigInteger key)
    {
        foreach (var (caseValue, target) in @switch.Cases)
        {
            if (caseValue == key)
                return target;
        }
        return @switch.DefaultTarget;
    }
}
