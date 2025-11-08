using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class LirBlock
{
    internal LirBlock(string label)
    {
        Label = label;
    }

    internal string Label { get; }
    internal List<LirInst> Instructions { get; } = new();
}
