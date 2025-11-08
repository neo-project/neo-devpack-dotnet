namespace Neo.Compiler.HIR;

internal sealed record HirNoReentrantAttribute(byte Prefix, string Key) : HirAttribute;

