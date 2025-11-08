using System;
using System.Collections.Generic;

namespace Neo.Compiler.HIR;

internal sealed class HirFunction : HirNode
{
    private readonly List<HirBlock> _blocks = new();

    public HirFunction(string name, HirSignature signature)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Signature = signature ?? throw new ArgumentNullException(nameof(signature));
        var entry = new HirBlock("entry");
        _blocks.Add(entry);
        Entry = entry;
    }

    public string Name { get; }
    public HirSignature Signature { get; }
    public HirBlock Entry { get; }
    public IReadOnlyList<HirBlock> Blocks => _blocks;

    public HirBlock AddBlock(string label)
    {
        var block = new HirBlock(label);
        _blocks.Add(block);
        return block;
    }

    public void RemoveBlock(HirBlock block)
    {
        if (block is null)
            throw new ArgumentNullException(nameof(block));
        if (ReferenceEquals(block, Entry))
            throw new InvalidOperationException("Cannot remove entry block.");
        _blocks.Remove(block);
    }

    public void Reset()
    {
        foreach (var block in _blocks)
            block.Reset();

        if (_blocks.Count > 1)
            _blocks.RemoveRange(1, _blocks.Count - 1);
    }
}
