using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VStaticStore : VNode
{
    internal VStaticStore(byte slot, VNode value, LirType valueType)
        : base(LirType.TVoid)
    {
        Slot = slot;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
    }

    internal byte Slot { get; }
    internal VNode Value { get; }
    internal LirType ValueType { get; }
}
