using System;

namespace Neo.Compiler.HIR;

internal sealed class HirConditionalBranch : HirTerminator
{
    public HirConditionalBranch(HirValue condition, HirBlock trueBlock, HirBlock falseBlock)
    {
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        TrueBlock = trueBlock ?? throw new ArgumentNullException(nameof(trueBlock));
        FalseBlock = falseBlock ?? throw new ArgumentNullException(nameof(falseBlock));
    }

    public HirValue Condition { get; }
    public HirBlock TrueBlock { get; }
    public HirBlock FalseBlock { get; }
}

