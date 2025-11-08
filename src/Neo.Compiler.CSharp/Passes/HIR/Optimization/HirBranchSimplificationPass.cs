using System;

namespace Neo.Compiler.HIR.Optimization;

internal sealed class HirBranchSimplificationPass : IHirPass
{
    public bool Run(HirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var changed = false;

        foreach (var block in function.Blocks)
        {
            switch (block.Terminator)
            {
                case HirConditionalBranch cond when TryEvaluate(cond.Condition, out var value):
                    block.SetTerminator(new HirBranch(value ? cond.TrueBlock : cond.FalseBlock) { Span = cond.Span });
                    changed = true;
                    break;
                case HirSwitch sw when TryGetConstInt(sw.Key, out var constant):
                    var target = FindSwitchTarget(sw, constant);
                    block.SetTerminator(new HirBranch(target) { Span = sw.Span });
                    changed = true;
                    break;
            }
        }

        return changed;
    }

    private static bool TryEvaluate(HirValue value, out bool result)
    {
        switch (value)
        {
            case HirConstBool constBool:
                result = constBool.Value;
                return true;
            case HirConstInt constInt:
                result = constInt.Value != System.Numerics.BigInteger.Zero;
                return true;
            default:
                result = false;
                return false;
        }
    }

    private static bool TryGetConstInt(HirValue value, out System.Numerics.BigInteger constant)
    {
        if (value is HirConstInt constInt)
        {
            constant = constInt.Value;
            return true;
        }

        constant = default;
        return false;
    }

    private static HirBlock FindSwitchTarget(HirSwitch @switch, System.Numerics.BigInteger value)
    {
        foreach (var (caseValue, target) in @switch.Cases)
        {
            if (caseValue == value)
                return target;
        }

        return @switch.DefaultTarget;
    }
}
