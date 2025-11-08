namespace Neo.Compiler.HIR;

internal readonly record struct HirField(string Name, HirType Type, bool IsNullable);

