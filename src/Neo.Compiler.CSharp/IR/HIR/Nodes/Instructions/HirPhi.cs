using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed partial class HirPhi : HirInst
{
    public bool IsLocalPhi { get; set; }
    public HirLocal? Local { get; set; }

    public HirPhi(HirType type)
        : base(type)
    {
    }

    private readonly List<Incoming> _inputs = new();

    public IReadOnlyList<Incoming> Inputs => _inputs;

    public void AddIncoming(HirBlock block, HirValue value)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(value);
        _inputs.Add(new Incoming(block, value));
    }

    public void ReplaceIncoming(HirValue oldValue, HirValue newValue)
    {
        ArgumentNullException.ThrowIfNull(oldValue);
        ArgumentNullException.ThrowIfNull(newValue);
        for (int i = 0; i < _inputs.Count; i++)
        {
            var incoming = _inputs[i];
            if (ReferenceEquals(incoming.Value, oldValue))
                _inputs[i] = incoming with { Value = newValue };
        }
    }

    public void RemoveIncoming(HirBlock block)
    {
        ArgumentNullException.ThrowIfNull(block);
        for (int i = _inputs.Count - 1; i >= 0; i--)
        {
            if (ReferenceEquals(_inputs[i].Block, block))
                _inputs.RemoveAt(i);
        }
    }
}
