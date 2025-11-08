using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirConstBuffer : MirInst
{
    internal MirConstBuffer(byte[] value)
        : base(MirType.TBuffer)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    internal byte[] Value { get; }
}
