using System;
using System.Collections.Generic;

namespace Neo.Compiler.LIR;

internal sealed partial class LirVerifier
{
    internal sealed record Result(bool Ok, IReadOnlyList<string> Errors)
    {
        internal static Result Success() => new(true, Array.Empty<string>());
        internal static Result Failure(List<string> errors) => new(false, errors);
    }
}

