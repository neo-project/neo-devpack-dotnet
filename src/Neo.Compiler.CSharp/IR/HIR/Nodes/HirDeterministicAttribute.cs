namespace Neo.Compiler.HIR;

internal sealed record HirDeterministicAttribute(bool Value = true) : HirAttribute;

