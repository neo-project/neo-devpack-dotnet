namespace Neo.Compiler.HIR;

internal sealed record HirIntType(int? WidthHintBits = null, bool IsSigned = true) : HirType;

