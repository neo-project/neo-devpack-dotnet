using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal abstract class VTerminator
{
    internal SourceSpan? Span { get; init; }
}
