using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;

/// <summary>
/// VReg-LIR node base representing a value in the SSA-based graph prior to stack scheduling.
/// </summary>
internal abstract class VNode
{
    protected VNode(LirType type) => Type = type;

    internal LirType Type { get; }
    internal SourceSpan? Span { get; set; }
}
