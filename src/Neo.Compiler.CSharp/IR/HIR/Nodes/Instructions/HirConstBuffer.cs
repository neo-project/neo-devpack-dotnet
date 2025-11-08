using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirConstBuffer : HirInst
{
    public HirConstBuffer(byte[] value)
        : base(HirType.BufferType)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public byte[] Value { get; }
}
