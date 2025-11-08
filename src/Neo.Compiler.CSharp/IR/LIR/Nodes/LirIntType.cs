namespace Neo.Compiler.LIR;

internal sealed record LirIntType(int? WidthHintBits = null, bool IsSigned = true) : LirType;

