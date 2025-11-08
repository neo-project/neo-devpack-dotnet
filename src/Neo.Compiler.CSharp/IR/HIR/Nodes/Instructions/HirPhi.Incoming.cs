namespace Neo.Compiler.HIR;

internal sealed partial class HirPhi
{
    public readonly record struct Incoming(HirBlock Block, HirValue Value);
}

