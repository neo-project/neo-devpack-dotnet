using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VSlice : VNode
{
    internal VSlice(VNode value, VNode start, VNode length, bool bufferSlice)
        : base(bufferSlice ? LirType.TBuffer : LirType.TByteString)
    {
        Value = value;
        Start = start;
        Length = length;
        IsBufferSlice = bufferSlice;
    }

    internal VNode Value { get; }
    internal VNode Start { get; }
    internal VNode Length { get; }
    internal bool IsBufferSlice { get; }
}
