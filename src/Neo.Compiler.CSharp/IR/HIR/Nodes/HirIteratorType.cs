namespace Neo.Compiler.HIR;

internal sealed record HirIteratorType(HirType? ElementType = null) : HirType;

