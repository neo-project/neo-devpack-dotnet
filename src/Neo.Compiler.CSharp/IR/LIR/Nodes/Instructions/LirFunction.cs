using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class LirFunction
{
    internal LirFunction(string name)
    {
        Name = name;
    }

    internal string Name { get; }
    internal int EntryParameterCount { get; set; }
    internal List<LirBlock> Blocks { get; } = new();
}
