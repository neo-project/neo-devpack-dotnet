using System;
using System.Collections.Generic;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirModule
{
    private readonly Dictionary<string, MirFunction> _functions = new(StringComparer.Ordinal);
    private readonly object _sync = new();

    internal IReadOnlyDictionary<string, MirFunction> Functions => _functions;

    internal MirFunction GetOrAddFunction(HirFunction hirFunction)
    {
        if (hirFunction is null)
            throw new ArgumentNullException(nameof(hirFunction));

        lock (_sync)
        {
            if (!_functions.TryGetValue(hirFunction.Name, out var function))
            {
                function = new MirFunction(hirFunction);
                _functions.Add(hirFunction.Name, function);
            }
            else
            {
                function.Reset(hirFunction);
            }

            return function;
        }
    }
}
