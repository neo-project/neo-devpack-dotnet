using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirSlice : HirInst
{
    public HirSlice(HirValue value, HirValue start, HirValue length, bool isBufferSlice)
        : base(isBufferSlice ? HirType.BufferType : HirType.ByteStringType)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Start = start ?? throw new ArgumentNullException(nameof(start));
        Length = length ?? throw new ArgumentNullException(nameof(length));
        IsBufferSlice = isBufferSlice;
    }

    public HirValue Value { get; set; }
    public HirValue Start { get; set; }
    public HirValue Length { get; set; }
    public bool IsBufferSlice { get; }
}
