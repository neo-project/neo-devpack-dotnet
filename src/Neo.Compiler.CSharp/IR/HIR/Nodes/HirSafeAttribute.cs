namespace Neo.Compiler.HIR;

internal sealed record HirSafeAttribute(bool Value = true) : HirAttribute;

