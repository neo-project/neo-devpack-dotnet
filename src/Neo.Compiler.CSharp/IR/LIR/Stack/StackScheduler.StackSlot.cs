using Neo.Compiler.LIR;

namespace Neo.Compiler.LIR.Backend;

internal sealed partial class StackScheduler
{
    private sealed class StackSlot
    {
        internal StackSlot(VNode? value, bool isImmediate, bool isReplica = false)
        {
            Value = value;
            IsImmediate = isImmediate;
            IsReplica = isReplica;
        }

        internal VNode? Value { get; set; }
        internal bool IsImmediate { get; set; }
        internal bool IsReplica { get; set; }
        internal bool Reserved { get; set; }

        internal StackSlot CloneReplica()
        {
            return new StackSlot(Value, IsImmediate, isReplica: true);
        }
    }
}

