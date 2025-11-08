using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirPhi : MirInst
{
    private readonly List<(MirBlock Block, MirValue Value)> _inputs = new();

    internal MirPhi(MirType type)
        : base(type)
    {
    }

    internal IReadOnlyList<(MirBlock Block, MirValue Value)> Inputs => _inputs;
    internal bool IsPinned { get; set; }
    internal bool IsLocalPhi { get; set; }
    internal HirLocal? Local { get; set; }

    internal void AddIncoming(MirBlock block, MirValue value)
    {
        _inputs.Add((block ?? throw new ArgumentNullException(nameof(block)), value ?? throw new ArgumentNullException(nameof(value))));
    }

    internal void ReplaceIncoming(MirValue oldValue, MirValue newValue)
    {
        ArgumentNullException.ThrowIfNull(oldValue);
        ArgumentNullException.ThrowIfNull(newValue);
        for (int i = 0; i < _inputs.Count; i++)
        {
            if (ReferenceEquals(_inputs[i].Value, oldValue))
                _inputs[i] = (_inputs[i].Block, newValue);
        }
    }

    internal void RemoveIncoming(MirBlock block)
    {
        ArgumentNullException.ThrowIfNull(block);
        for (int i = _inputs.Count - 1; i >= 0; i--)
        {
            if (ReferenceEquals(_inputs[i].Block, block))
                _inputs.RemoveAt(i);
        }
    }

    internal void ResetInputs()
    {
        _inputs.Clear();
    }
}
