using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;

internal sealed class VCatch : VNode
{
    internal VCatch(VTry scope)
        : base(LirType.TVoid)
    {
        Scope = scope;
    }

    internal VTry Scope { get; }
}
