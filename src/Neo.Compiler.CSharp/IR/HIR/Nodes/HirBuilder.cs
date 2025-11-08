using System;
using System.Collections.Generic;

namespace Neo.Compiler.HIR;

/// <summary>
/// Thin helper for constructing HIR functions. The builder manages current block, location tracking, and optional
/// memory token threading.
/// </summary>
internal sealed class HirBuilder
{
    private readonly HirFunction _function;
    private HirBlock _currentBlock;
    private SourceSpan? _pendingSpan;
    private HirValue? _currentToken;

    public HirBuilder(HirFunction function)
    {
        _function = function ?? throw new ArgumentNullException(nameof(function));
        _currentBlock = function.Entry;
    }

    public HirFunction Function => _function;
    public HirBlock CurrentBlock => _currentBlock;

    public void SetCurrentBlock(HirBlock block)
    {
        _currentBlock = block ?? throw new ArgumentNullException(nameof(block));
    }

    public void MarkLocation(SourceSpan? span)
    {
        _pendingSpan = span;
    }

    public HirPhi AppendPhi(HirPhi phi)
    {
        ArgumentNullException.ThrowIfNull(phi);
        ApplyPendingMetadata(phi);
        _currentBlock.AppendPhi(phi);
        return phi;
    }

    public T Append<T>(T instruction) where T : HirInst
    {
        ArgumentNullException.ThrowIfNull(instruction);
        ApplyPendingMetadata(instruction);

        if (instruction.ConsumesMemoryToken)
        {
            if (_currentToken is null)
                _currentToken = HirMemoryToken.Instance;
        }

        _currentBlock.Append(instruction);

        if (instruction.ProducesMemoryToken)
        {
            _currentToken = instruction;
        }

        return instruction;
    }

    public T AppendTerminator<T>(T terminator) where T : HirTerminator
    {
        ArgumentNullException.ThrowIfNull(terminator);
        ApplyPendingMetadata(terminator);
        _currentBlock.SetTerminator(terminator);
        _currentToken = null;
        return terminator;
    }

    public HirBlock CreateBlock(string label)
        => _function.AddBlock(label);

    public HirValue? CurrentMemoryToken => _currentToken;

    private void ApplyPendingMetadata(HirNode node)
    {
        if (_pendingSpan is not null)
        {
            node.Span = _pendingSpan;
            _pendingSpan = null;
        }
    }
}
