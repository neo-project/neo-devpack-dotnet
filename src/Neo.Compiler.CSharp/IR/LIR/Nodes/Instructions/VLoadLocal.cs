using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;

internal sealed class VLoadLocal : VNode
{
    internal VLoadLocal(int slot, LirType type)
        : base(type)
    {
        Slot = slot;
    }

    internal int Slot { get; }
}
