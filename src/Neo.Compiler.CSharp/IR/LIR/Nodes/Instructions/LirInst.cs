using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


/// <summary>
/// Stack-LIR instruction representation after scheduling.
/// </summary>
internal sealed class LirInst
{
    internal LirInst(LirOpcode op) => Op = op;

    internal LirOpcode Op { get; }
    internal byte[]? Immediate { get; init; }
    internal SourceSpan? Span { get; init; }
    internal string? TargetLabel { get; init; }
    internal string? TargetLabel2 { get; init; }
    internal int? PopOverride { get; set; }
    internal int? PushOverride { get; set; }
}
