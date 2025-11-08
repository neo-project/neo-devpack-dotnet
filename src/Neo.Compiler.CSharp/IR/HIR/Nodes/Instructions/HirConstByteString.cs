using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirConstByteString : HirInst
{
    public HirConstByteString(byte[] value)
        : base(HirType.ByteStringType)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public byte[] Value { get; }
}
