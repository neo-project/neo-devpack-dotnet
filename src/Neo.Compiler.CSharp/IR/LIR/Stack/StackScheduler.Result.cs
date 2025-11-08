namespace Neo.Compiler.LIR.Backend;

internal sealed partial class StackScheduler
{
    internal sealed record Result(LirFunction Function, int MaxStack);
}

