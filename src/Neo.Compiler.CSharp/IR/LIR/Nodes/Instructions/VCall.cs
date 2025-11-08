using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VCall : VNode
{
    internal VCall(string callee, IReadOnlyList<VNode> arguments, LirType returnType, bool isPure)
        : base(returnType)
    {
        Callee = callee;
        Arguments = arguments;
        IsPure = isPure;
    }

    internal string Callee { get; }
    internal IReadOnlyList<VNode> Arguments { get; }
    internal bool IsPure { get; }
}
