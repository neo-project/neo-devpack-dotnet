using Neo.Compiler.LIR.Backend;

namespace Neo.Compiler.LIR;

internal sealed partial class LirModule
{
    internal sealed record LirCompilation(VFunction ValueFunction, LirFunction StackFunction, NeoEmitter.EmitResult EmitResult);
}

