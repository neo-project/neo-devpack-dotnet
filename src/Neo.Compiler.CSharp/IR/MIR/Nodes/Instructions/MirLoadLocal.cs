using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirLoadLocal : MirInst
{
    internal MirLoadLocal(int slot, MirType type)
        : base(type)
    {
        Slot = slot;
    }

    internal int Slot { get; }
}
