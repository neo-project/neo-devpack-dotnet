namespace Neo.Compiler.HIR;

internal sealed record HirMapType(HirType KeyType, HirType ValueType) : HirType;

