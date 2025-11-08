using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirConstByteString : MirInst
{
    internal MirConstByteString(byte[] value)
        : base(MirType.TByteString)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    internal byte[] Value { get; }
}
