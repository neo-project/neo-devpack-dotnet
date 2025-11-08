using System;
using Neo.Compiler.MIR;
using Neo.Compiler.MIR.Optimization;

namespace Neo.Compiler.CSharp.MirHarness;

internal sealed class PassTracer : IMirPass
{
    private readonly IMirPass _inner;
    private readonly string _name;
    private readonly int _sequence;

    internal PassTracer(IMirPass inner, int sequence)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _name = inner.GetType().Name;
        _sequence = sequence;
    }

    public bool Run(MirFunction function)
    {
        Console.WriteLine($"[TRACE] Enter {_sequence}:{_name}");
        var result = _inner.Run(function);
        Console.WriteLine($"[TRACE] Leave {_sequence}:{_name} changed={result}");
        return result;
    }
}
