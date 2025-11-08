using System;

namespace Neo.Compiler.HIR;

internal sealed class HirCatchScope : HirInst
{
    public HirCatchScope(HirTryFinallyScope parent, HirCatchClause clause)
        : base(HirType.VoidType)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        Clause = clause ?? throw new ArgumentNullException(nameof(clause));
    }

    public HirTryFinallyScope Parent { get; }
    public HirCatchClause Clause { get; }
}
