namespace Neo.Compiler.HIR;

internal sealed class HirReturn : HirTerminator
{
    public HirReturn(HirValue? value)
    {
        Value = value;
    }

    public HirValue? Value { get; }
}

