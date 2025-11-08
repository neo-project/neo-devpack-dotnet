using System;
using System.Collections.Generic;

namespace Neo.Compiler.HIR;

/// <summary>
/// Aggregates all HIR functions produced for a compilation unit. Thread safe to support parallel conversions.
/// </summary>
internal sealed class HirModule
{
    private readonly Dictionary<string, HirFunction> _functions = new(StringComparer.Ordinal);
    private readonly object _sync = new();

    public IReadOnlyDictionary<string, HirFunction> Functions => _functions;

    public HirFunction GetOrAddFunction(string name, HirSignature signature)
    {
        lock (_sync)
        {
            if (!_functions.TryGetValue(name, out var function))
            {
                function = new HirFunction(name, signature);
                _functions.Add(name, function);
            }
            else
            {
                function.Reset();
            }

            return function;
        }
    }

    public void Clear()
    {
        lock (_sync)
        {
            _functions.Clear();
        }
    }

    public bool TryGetFunction(string name, out HirFunction function)
    {
        if (name is null)
            throw new ArgumentNullException(nameof(name));

        lock (_sync)
        {
            return _functions.TryGetValue(name, out function!);
        }
    }
}
