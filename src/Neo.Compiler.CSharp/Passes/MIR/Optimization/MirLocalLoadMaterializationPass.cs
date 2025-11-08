using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.MIR.Optimization;

/// <summary>
/// Materializes <see cref="MirLoadLocal"/> nodes within each block so SSA values never cross block boundaries on the stack.
/// </summary>
internal sealed class MirLocalLoadMaterializationPass : IMirPass
{
    private static readonly bool s_trace =
        string.Equals(Environment.GetEnvironmentVariable("NEO_IR_TRACE_LOCAL_LOADS"), "1", StringComparison.OrdinalIgnoreCase);

    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var blockCaches = new Dictionary<MirBlock, Dictionary<int, MirLoadLocal>>(ReferenceEqualityComparer.Instance);
        var pendingLoads = new Dictionary<MirBlock, List<MirLoadLocal>>(ReferenceEqualityComparer.Instance);
        var changed = false;

        foreach (var block in function.Blocks)
        {
            if (ProcessPhis(block, blockCaches, pendingLoads))
                changed = true;
            if (ProcessInstructions(function, block, blockCaches, pendingLoads))
                changed = true;
            if (ProcessTerminator(block, blockCaches, pendingLoads))
                changed = true;

            if (pendingLoads.TryGetValue(block, out var additions))
            {
                for (int i = additions.Count - 1; i >= 0; i--)
                    block.Instructions.Insert(0, additions[i]);
            }

            if (s_trace && changed)
            {
                Console.WriteLine($"[BLOCK] {block.Label}");
                foreach (var inst in block.Instructions)
                    Console.WriteLine($"    INST {inst.GetType().Name}");
            }
        }

        return changed;
    }

    private static bool ProcessPhis(
        MirBlock block,
        Dictionary<MirBlock, Dictionary<int, MirLoadLocal>> caches,
        Dictionary<MirBlock, List<MirLoadLocal>> pending)
    {
        var changed = false;
        foreach (var phi in block.Phis)
        {
            var inputs = phi.Inputs.ToArray();
            foreach (var entry in inputs)
            {
                var pred = entry.Block;
                var value = entry.Value;
                if (value is not MirLoadLocal load || pred is null)
                    continue;

                var replacement = GetOrInsertLoad(pred, load.Slot, load.Type, caches, pending);
                if (ReferenceEquals(replacement, load))
                    continue;

                Trace($"[PHI] Block {block.Label} using load slot {load.Slot} from {pred.Label}->#{replacement.GetHashCode():X}");
                phi.ReplaceIncoming(load, replacement);
                changed = true;
            }
        }

        return changed;
    }

    private static bool ProcessInstructions(
        MirFunction function,
        MirBlock block,
        Dictionary<MirBlock, Dictionary<int, MirLoadLocal>> caches,
        Dictionary<MirBlock, List<MirLoadLocal>> pending)
    {
        var changed = false;
        var instructions = block.Instructions;
        for (int i = 0; i < instructions.Count; i++)
        {
            var inst = instructions[i];
            var replacement = inst;
            var mutated = false;

            foreach (var operand in MirAnalysis.GetOperands(inst))
            {
                if (operand is not MirLoadLocal load)
                    continue;

                var materialized = GetOrInsertLoad(block, load.Slot, load.Type, caches, pending);
                if (ReferenceEquals(materialized, load))
                    continue;

                Trace($"[INST] Block {block.Label} materialized slot {load.Slot} before {inst.GetType().Name}");
                replacement = MirValueRewriter.ReplaceInInstruction(replacement, load, materialized);
                mutated = true;
            }

            if (!mutated)
                continue;

            instructions[i] = replacement;
            MirValueRewriter.Replace(function, inst, replacement);
            changed = true;
        }

        return changed;
    }

    private static bool ProcessTerminator(
        MirBlock block,
        Dictionary<MirBlock, Dictionary<int, MirLoadLocal>> caches,
        Dictionary<MirBlock, List<MirLoadLocal>> pending)
    {
        if (block.Terminator is not { } terminator)
            return false;

        var replacement = terminator;
        var mutated = false;

        foreach (var operand in MirAnalysis.GetOperands(terminator))
        {
            if (operand is not MirLoadLocal load)
                continue;

            var materialized = GetOrInsertLoad(block, load.Slot, load.Type, caches, pending);
            if (ReferenceEquals(materialized, load))
                continue;

            Trace($"[TERM] Block {block.Label} materialized slot {load.Slot} before {terminator.GetType().Name}");
            replacement = MirValueRewriter.ReplaceInTerminator(replacement, load, materialized);
            mutated = true;
        }

        if (!mutated)
            return false;

        block.Terminator = replacement;
        return true;
    }

    private static MirLoadLocal GetOrInsertLoad(
        MirBlock block,
        int slot,
        MirType type,
        Dictionary<MirBlock, Dictionary<int, MirLoadLocal>> caches,
        Dictionary<MirBlock, List<MirLoadLocal>> pending)
    {
        if (!caches.TryGetValue(block, out var loads))
        {
            loads = new Dictionary<int, MirLoadLocal>();
            caches[block] = loads;
        }

        if (loads.TryGetValue(slot, out var existing))
            return existing;

        var clone = new MirLoadLocal(slot, type);
        if (!pending.TryGetValue(block, out var list))
        {
            list = new List<MirLoadLocal>();
            pending[block] = list;
        }
        list.Add(clone);
        loads[slot] = clone;
        Trace($"[CLONE] Block {block.Label} scheduled load slot {slot}");
        return clone;
    }

    private static void Trace(string message)
    {
        if (s_trace)
            Console.WriteLine(message);
    }
}
