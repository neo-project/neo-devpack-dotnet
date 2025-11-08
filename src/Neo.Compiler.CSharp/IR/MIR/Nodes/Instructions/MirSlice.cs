using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirSlice : MirInst
{
    internal MirSlice(MirValue value, MirValue start, MirValue length, bool bufferSlice)
        : base(bufferSlice ? MirType.TBuffer : MirType.TByteString)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Start = start ?? throw new ArgumentNullException(nameof(start));
        Length = length ?? throw new ArgumentNullException(nameof(length));
        IsBufferSlice = bufferSlice;
    }

    internal MirValue Value { get; }
    internal MirValue Start { get; }
    internal MirValue Length { get; }
    internal bool IsBufferSlice { get; }
}
