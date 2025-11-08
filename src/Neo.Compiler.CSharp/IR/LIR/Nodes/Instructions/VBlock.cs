using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VBlock
{
    internal VBlock(string label)
    {
        Label = label;
    }

    internal string Label { get; }
    internal List<VNode> Nodes { get; } = new();
    internal VTerminator? Terminator { get; set; }
}
