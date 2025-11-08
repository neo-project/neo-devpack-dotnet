using System;

namespace Neo.Compiler.MIR;

internal readonly struct MirCostSummary
{
    internal MirCostSummary(int instructionCount, int effectfulInstructionCount, ulong estimatedGas)
    {
        InstructionCount = instructionCount;
        EffectfulInstructionCount = effectfulInstructionCount;
        EstimatedGas = estimatedGas;
    }

    internal int InstructionCount { get; }
    internal int EffectfulInstructionCount { get; }
    internal ulong EstimatedGas { get; }

    internal static MirCostSummary Empty { get; } = new MirCostSummary(0, 0, 0);

    public override bool Equals(object? obj)
    {
        return obj is MirCostSummary other
            && InstructionCount == other.InstructionCount
            && EffectfulInstructionCount == other.EffectfulInstructionCount
            && EstimatedGas == other.EstimatedGas;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(InstructionCount, EffectfulInstructionCount, EstimatedGas);
    }
}
