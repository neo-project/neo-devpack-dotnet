using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirStoreLocal : MirInst
{
    internal MirStoreLocal(int slot, MirValue value)
        : base(MirType.TVoid)
    {
        Slot = slot;
        Value = value;
    }

    internal int Slot { get; }
    internal MirValue Value { get; }
}
