using System;
using System.Collections.Generic;
using System.Linq;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirFunction
{
    internal MirFunction(HirFunction source)
    {
        Reset(source);
    }

    internal string Name { get; private set; } = string.Empty;
    internal HirFunction Source { get; private set; } = null!;
    internal MirBlock Entry { get; private set; } = null!;
    internal MirTokenSeed EntryTokenSeed { get; private set; } = null!;
    internal MirMemoryTokenValue EntryToken { get; private set; } = null!;
    internal List<MirBlock> Blocks { get; } = new();
    internal bool AggressiveInlineHint { get; private set; }
    internal MirCostSummary CostSummary { get; private set; } = MirCostSummary.Empty;

    internal MirBlock CreateBlock(string label)
    {
        var block = new MirBlock(label);
        Blocks.Add(block);
        return block;
    }

    internal void RemoveBlock(MirBlock block)
    {
        if (block is null)
            throw new ArgumentNullException(nameof(block));
        if (ReferenceEquals(block, Entry))
            throw new InvalidOperationException("Cannot remove entry block.");
        Blocks.Remove(block);
    }

    internal void Reset(HirFunction source)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Name = source.Name;
        Blocks.Clear();
        var entryLabel = source.Entry.Label;
        if (!string.IsNullOrEmpty(Name))
            entryLabel = Name;
        Entry = new MirBlock(entryLabel);
        EntryTokenSeed = new MirTokenSeed();
        Entry.Append(EntryTokenSeed);
        EntryToken = EntryTokenSeed.Token;
        Blocks.Add(Entry);
        AggressiveInlineHint = source.Signature.Attributes.OfType<HirInlineAttribute>().Any(attr => attr.Aggressive);
        CostSummary = MirCostSummary.Empty;
    }

    internal void SetCostSummary(MirCostSummary summary) => CostSummary = summary;
}
