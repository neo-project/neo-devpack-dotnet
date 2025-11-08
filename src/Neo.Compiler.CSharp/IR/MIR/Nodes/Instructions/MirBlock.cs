using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirBlock
{
    internal MirBlock(string label)
    {
        Label = label ?? throw new ArgumentNullException(nameof(label));
    }

    internal string Label { get; }
    internal List<MirPhi> Phis { get; } = new();
    internal List<MirInst> Instructions { get; } = new();
    internal MirTerminator? Terminator { get; set; }

    internal void AppendPhi(MirPhi phi)
    {
        Phis.Add(phi ?? throw new ArgumentNullException(nameof(phi)));
    }

    internal void Append(MirInst inst)
    {
        Instructions.Add(inst ?? throw new ArgumentNullException(nameof(inst)));
    }

    internal void ReplaceInstruction(int index, MirInst inst)
    {
        if (inst is null)
            throw new ArgumentNullException(nameof(inst));
        if ((uint)index >= Instructions.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        Instructions[index] = inst;
    }

    internal void RemoveInstructionAt(int index)
    {
        if ((uint)index >= Instructions.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        Instructions.RemoveAt(index);
    }

    internal void RemoveInstruction(MirInst inst)
    {
        if (inst is null)
            throw new ArgumentNullException(nameof(inst));
        Instructions.Remove(inst);
    }

    internal void RemovePhi(MirPhi phi)
    {
        if (phi is null)
            throw new ArgumentNullException(nameof(phi));
        Phis.Remove(phi);
    }
}
