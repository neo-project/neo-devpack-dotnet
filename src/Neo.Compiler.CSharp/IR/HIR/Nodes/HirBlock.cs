using System;
using System.Collections.Generic;

namespace Neo.Compiler.HIR;

internal sealed class HirBlock : HirNode
{
    private readonly List<HirPhi> _phis = new();
    private readonly List<HirInst> _instructions = new();

    public HirBlock(string label)
    {
        Label = label ?? throw new ArgumentNullException(nameof(label));
    }

    public string Label { get; }
    public IReadOnlyList<HirPhi> Phis => _phis;
    public IReadOnlyList<HirInst> Instructions => _instructions;
    public HirTerminator? Terminator { get; private set; }

    public void AppendPhi(HirPhi phi)
    {
        if (phi is null)
            throw new ArgumentNullException(nameof(phi));
        _phis.Add(phi);
    }

    public void Append(HirInst instruction)
    {
        if (instruction is null)
            throw new ArgumentNullException(nameof(instruction));
        _instructions.Add(instruction);
    }

    public void Insert(int index, HirInst instruction)
    {
        if (instruction is null)
            throw new ArgumentNullException(nameof(instruction));
        if ((uint)index > _instructions.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        _instructions.Insert(index, instruction);
    }

    public void Remove(HirInst instruction)
    {
        if (instruction is null)
            throw new ArgumentNullException(nameof(instruction));
        _instructions.Remove(instruction);
    }

    public void ReplaceInstruction(int index, HirInst instruction)
    {
        if (instruction is null)
            throw new ArgumentNullException(nameof(instruction));
        if ((uint)index >= _instructions.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        _instructions[index] = instruction;
    }

    public void RemoveInstructionAt(int index)
    {
        if ((uint)index >= _instructions.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        _instructions.RemoveAt(index);
    }

    public void RemovePhi(HirPhi phi)
    {
        if (phi is null)
            throw new ArgumentNullException(nameof(phi));
        _phis.Remove(phi);
    }

    public void SetTerminator(HirTerminator terminator)
    {
        Terminator = terminator ?? throw new ArgumentNullException(nameof(terminator));
    }

    public void Reset()
    {
        _phis.Clear();
        _instructions.Clear();
        Terminator = null;
    }
}
