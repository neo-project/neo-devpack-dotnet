namespace Neo.Compiler.HIR;

internal sealed class HirThrow : HirTerminator
{
    public HirThrow(HirValue? exception)
    {
        Exception = exception;
    }

    public HirValue? Exception { get; }
}

