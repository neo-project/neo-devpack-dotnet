using System;
using System.Collections.Generic;

namespace Neo.Compiler.MIR.Optimization;

/// <summary>
/// Performs a lightweight scalar replacement of aggregates by dissolving struct packs that feed only field extractions.
/// </summary>
internal sealed class MirScalarReplacementPass : IMirPass
{
    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var usage = MirAnalysis.Analyze(function);
        var packs = new List<MirStructPack>();

        foreach (var block in function.Blocks)
        {
            foreach (var inst in block.Instructions)
            {
                if (inst is MirStructPack pack)
                    packs.Add(pack);
            }
        }

        var changed = false;
        foreach (var pack in packs)
        {
            if (usage.InstructionBlocks.TryGetValue(pack, out var block) && TryDissolvePack(function, pack, block, usage))
                changed = true;
        }

        return changed;
    }

    private static bool TryDissolvePack(MirFunction function, MirStructPack pack, MirBlock packBlock, MirUsageGraph usage)
    {
        var fieldUses = new List<MirStructGet>();
        if (!CollectStructPackUses(function, pack, fieldUses))
            return false;

        foreach (var get in fieldUses)
        {
            var fieldValue = pack.Fields[get.Index];
            MirValueRewriter.Replace(function, get, fieldValue);

            if (usage.InstructionBlocks.TryGetValue(get, out var block))
                block.RemoveInstruction(get);
        }

        packBlock.RemoveInstruction(pack);
        return true;
    }

    private static bool CollectStructPackUses(MirFunction function, MirStructPack pack, List<MirStructGet> uses)
    {
        foreach (var block in function.Blocks)
        {
            foreach (var phi in block.Phis)
            {
                foreach (var (_, value) in phi.Inputs)
                {
                    if (ReferenceEquals(value, pack))
                        return false;
                }
            }

            foreach (var inst in block.Instructions)
            {
                if (ReferenceEquals(inst, pack))
                    continue;

                switch (inst)
                {
                    case MirStructGet structGet when ReferenceEquals(structGet.Object, pack):
                        uses.Add(structGet);
                        break;

                    case MirStructSet structSet when ReferenceEquals(structSet.Object, pack) || ReferenceEquals(structSet.Value, pack):
                        return false;

                    default:
                        foreach (var operand in MirAnalysis.GetOperands(inst))
                        {
                            if (ReferenceEquals(operand, pack))
                                return false;
                        }
                        break;
                }
            }

            switch (block.Terminator)
            {
                case MirReturn ret when ReferenceEquals(ret.Value, pack):
                    return false;
                case MirCondBranch cond when ReferenceEquals(cond.Condition, pack):
                    return false;
                case MirSwitch @switch when ReferenceEquals(@switch.Key, pack):
                    return false;
            }
        }

        return uses.Count > 0;
    }
}
