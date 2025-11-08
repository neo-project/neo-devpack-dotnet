using System;
using System.Collections.Generic;
using Neo.Compiler.LIR.Backend;

namespace Neo.Compiler.LIR;

/// <summary>
/// Aggregates the intermediate artifacts produced during MIR→LIR lowering (value graph, scheduled stack program, and
/// emitted NeoVM bytecode).
/// </summary>
internal sealed partial class LirModule
{
    private readonly Dictionary<string, LirCompilation> _compilations = new(StringComparer.Ordinal);
    private readonly List<string> _order = new();
    private readonly object _sync = new();

    internal IReadOnlyDictionary<string, LirCompilation> Compilations => _compilations;

    internal void Store(string functionName, LirCompilation compilation)
    {
        if (functionName is null)
            throw new ArgumentNullException(nameof(functionName));
        if (compilation is null)
            throw new ArgumentNullException(nameof(compilation));

        lock (_sync)
        {
            if (!_compilations.ContainsKey(functionName))
                _order.Add(functionName);
            _compilations[functionName] = compilation;
        }
    }

    internal bool TryGetCompilation(string functionName, out LirCompilation compilation)
    {
        if (functionName is null)
            throw new ArgumentNullException(nameof(functionName));

        lock (_sync)
        {
            return _compilations.TryGetValue(functionName, out compilation!);
        }
    }

    internal IEnumerable<(string Name, LirCompilation Compilation)> Enumerate()
    {
        lock (_sync)
        {
            foreach (var name in _order)
            {
                if (_compilations.TryGetValue(name, out var compilation))
                    yield return (name, compilation);
            }
        }
    }
}
