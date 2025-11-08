using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;

internal sealed class VStoreLocal : VNode
{
    internal VStoreLocal(int slot, VNode value)
        : base(new LirVoidType())
    {
        Slot = slot;
        Value = value;
    }

    internal int Slot { get; }
    internal VNode Value { get; }
}
