namespace Neo.Compiler.MIR;

internal sealed record MirMapType(MirType KeyType, MirType ValueType) : MirType;

