using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VBufferCopy : VNode
{
    internal VBufferCopy(VNode dst, VNode src, VNode dstOffset, VNode srcOffset, VNode length)
        : base(dst.Type)
    {
        Destination = dst;
        Source = src;
        DestinationOffset = dstOffset;
        SourceOffset = srcOffset;
        Length = length;
    }

    internal VNode Destination { get; }
    internal VNode Source { get; }
    internal VNode DestinationOffset { get; }
    internal VNode SourceOffset { get; }
    internal VNode Length { get; }
}
