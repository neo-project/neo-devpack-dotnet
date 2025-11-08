namespace Neo.Compiler.HIR;

internal sealed record HirArrayType(HirType ElementType, bool NullableElements = false) : HirType;

