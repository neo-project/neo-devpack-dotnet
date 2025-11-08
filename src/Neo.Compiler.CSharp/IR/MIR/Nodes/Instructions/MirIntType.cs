namespace Neo.Compiler.MIR;

internal sealed record MirIntType(int? WidthHintBits = null, bool IsSigned = true) : MirType;

