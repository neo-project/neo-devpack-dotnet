using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VStructPack : VNode
{
    internal VStructPack(IReadOnlyList<VNode> fields, LirStructType type)
        : base(type)
    {
        Fields = fields;
    }

    internal IReadOnlyList<VNode> Fields { get; }
}
