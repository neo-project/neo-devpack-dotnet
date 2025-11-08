using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VPhi : VNode
{
    private readonly List<(VBlock Block, VNode Value)> _inputs = new();

    internal VPhi(LirType type)
        : base(type)
    {
    }

    internal IReadOnlyList<(VBlock Block, VNode Value)> Inputs => _inputs;

    internal void AddIncoming(VBlock block, VNode value)
    {
        _inputs.Add((block, value));
    }
}
