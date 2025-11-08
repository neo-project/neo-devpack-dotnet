using System;
using System.Collections.Generic;

namespace Neo.Compiler.HIR;

/// <summary>
/// Base class for all HIR nodes (instructions, blocks, functions). Nodes can carry attributes and optional source spans.
/// </summary>
internal abstract class HirNode
{
    public IReadOnlyList<HirAttribute> Attributes { get; init; } = Array.Empty<HirAttribute>();
    public SourceSpan? Span { get; set; }
}
