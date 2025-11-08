using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.MIR.Optimization;

internal static class MirMemoryTokenRetargeter
{
    internal static void Retokenize(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        if (function.Entry.Instructions.FirstOrDefault() is not MirTokenSeed seed)
            return;

        var predecessors = new Dictionary<MirBlock, HashSet<MirBlock>>(ReferenceEqualityComparer.Instance);
        foreach (var block in function.Blocks)
            predecessors[block] = new HashSet<MirBlock>(ReferenceEqualityComparer.Instance);

        foreach (var block in function.Blocks)
        {
            foreach (var successor in MirControlFlow.GetSuccessors(block))
            {
                if (predecessors.TryGetValue(successor, out var set))
                    set.Add(block);
            }
        }

        var incoming = new Dictionary<MirBlock, Dictionary<MirBlock, MirValue>>(ReferenceEqualityComparer.Instance);
        foreach (var block in function.Blocks)
            incoming[block] = new Dictionary<MirBlock, MirValue>(ReferenceEqualityComparer.Instance);
        incoming[function.Entry][function.Entry] = function.EntryToken;

        var queue = new Queue<MirBlock>();
        var inQueue = new HashSet<MirBlock>(ReferenceEqualityComparer.Instance) { function.Entry };
        queue.Enqueue(function.Entry);

        while (queue.Count > 0)
        {
            var block = queue.Dequeue();
            inQueue.Remove(block);

            var incomingMap = incoming[block];
            if (!ReferenceEquals(block, function.Entry))
            {
                if (predecessors.TryGetValue(block, out var preds) && preds.Count > 0)
                {
                    foreach (var predecessor in incomingMap.Keys.ToArray())
                    {
                        if (!preds.Contains(predecessor))
                            incomingMap.Remove(predecessor);
                    }
                }
                else
                {
                    incomingMap.Clear();
                }
            }

            var tokenPhi = block.Phis.FirstOrDefault(p => p.Type is MirTokenType);
            tokenPhi?.ResetInputs();

            MirValue currentToken;

            if (ReferenceEquals(block, function.Entry))
            {
                currentToken = seed.Token;
                tokenPhi?.AddIncoming(block, currentToken);
            }
            else if (incomingMap.Count == 0)
            {
                if (tokenPhi is not null)
                    block.RemovePhi(tokenPhi);
                currentToken = function.EntryToken;
            }
            else if (incomingMap.Count == 1)
            {
                var (pred, token) = incomingMap.First();
                if (tokenPhi is not null)
                {
                    tokenPhi.AddIncoming(pred, token);
                    currentToken = tokenPhi;
                }
                else
                {
                    currentToken = token;
                }
            }
            else
            {
                if (tokenPhi is null)
                {
                    tokenPhi = new MirPhi(MirType.TToken);
                    block.Phis.Insert(0, tokenPhi);
                }

                foreach (var (pred, token) in incomingMap)
                    tokenPhi.AddIncoming(pred, token);

                currentToken = tokenPhi;
            }

            foreach (var inst in block.Instructions)
            {
                if (inst is MirTokenSeed tokenSeed)
                {
                    currentToken = tokenSeed.Token;
                    continue;
                }

                if (RequiresMemoryToken(inst))
                {
                    if (!inst.ConsumesMemoryToken || !ReferenceEquals(inst.TokenInput, currentToken))
                    {
                        inst.ResetMemoryTokenState();
                        var newToken = inst.AttachMemoryToken(currentToken);
                        currentToken = newToken;
                    }
                    else
                    {
                        currentToken = inst.TokenOutput!;
                    }
                }
                else if (inst.ConsumesMemoryToken || inst.ProducesMemoryToken)
                {
                    inst.ResetMemoryTokenState();
                }
            }

            foreach (var successor in MirControlFlow.GetSuccessors(block))
            {
                if (!incoming.TryGetValue(successor, out var map))
                    continue;

                if (!map.TryGetValue(block, out var previous) || !ReferenceEquals(previous, currentToken))
                {
                    map[block] = currentToken;
                    if (inQueue.Add(successor))
                        queue.Enqueue(successor);
                }
            }
        }

        foreach (var block in function.Blocks)
        {
            var tokenPhi = block.Phis.FirstOrDefault(p => p.Type is MirTokenType);
            if (tokenPhi is null)
                continue;

            if (!predecessors.TryGetValue(block, out var preds))
                preds = null;

            var inputs = tokenPhi.Inputs.ToArray();
            foreach (var (pred, _) in inputs)
            {
                if (preds is null || !preds.Contains(pred))
                    tokenPhi.RemoveIncoming(pred);
            }

            if (tokenPhi.Inputs.Count == 0)
                block.RemovePhi(tokenPhi);
        }
    }

    private static bool RequiresMemoryToken(MirInst inst)
    {
        if (inst is null)
            return false;
        if (inst is MirTokenSeed)
            return false;
        return (inst.Effect & MirEffect.Memory) != MirEffect.None;
    }
}
