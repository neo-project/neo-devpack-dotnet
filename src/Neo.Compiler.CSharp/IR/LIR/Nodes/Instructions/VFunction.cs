using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VFunction
{
    internal VFunction(string name)
    {
        Name = name;
    }

    internal string Name { get; }
    internal int ParameterCount { get; set; }
    internal List<VBlock> Blocks { get; } = new();
}
