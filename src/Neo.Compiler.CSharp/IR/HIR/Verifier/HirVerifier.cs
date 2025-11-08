using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirVerifier
{
    private readonly List<string> _errors = new();
    private HirFunction? _function;

    public IReadOnlyList<string> Verify(HirFunction function)
    {
        _function = function ?? throw new ArgumentNullException(nameof(function));
        _errors.Clear();

        CheckBlocks(function);
        CheckTerminators(function);
        CheckInstructions(function);
        // Dominance/SSA validation will be added when MethodConvert produces SSA.

        return _errors.ToArray();
    }

    private void CheckBlocks(HirFunction function)
    {
        if (function.Blocks.Count == 0)
            _errors.Add($"Function '{function.Name}' contains no blocks.");

        foreach (var block in function.Blocks)
        {
            if (block.Terminator is null)
            {
                _errors.Add($"Block '{block.Label}' is missing a terminator.");
            }
        }
    }

    private void CheckTerminators(HirFunction function)
    {
        var tryScopes = new HashSet<HirTryFinallyScope>(function.Blocks.SelectMany(b => b.Instructions).OfType<HirTryFinallyScope>());

        foreach (var block in function.Blocks)
        {
            switch (block.Terminator)
            {
                case HirBranch branch:
                    RequireBlock(branch.Target, block, "branch");
                    break;
                case HirConditionalBranch cond:
                    RequireValue(cond.Condition, block, "conditional branch");
                    RequireBlock(cond.TrueBlock, block, "conditional branch (true)");
                    RequireBlock(cond.FalseBlock, block, "conditional branch (false)");
                    break;
                case HirSwitch @switch:
                    RequireValue(@switch.Key, block, "switch");
                    foreach (var (_, target) in @switch.Cases)
                    {
                        RequireBlock(target, block, "switch case");
                    }
                    RequireBlock(@switch.DefaultTarget, block, "switch default");
                    break;
                case HirLeave leave:
                    if (!tryScopes.Contains(leave.Scope))
                        _errors.Add($"Block '{block.Label}' references unknown try scope in HirLeave.");
                    RequireBlock(leave.Target, block, nameof(HirLeave));
                    break;
                case HirEndFinally endFinally:
                    if (!tryScopes.Contains(endFinally.Scope))
                        _errors.Add($"Block '{block.Label}' references unknown try scope in HirEndFinally.");
                    RequireBlock(endFinally.Target, block, nameof(HirEndFinally));
                    break;
                case HirReturn ret:
                    if (ret.Value is null && !function.Signature.ReturnType.Equals(HirType.VoidType))
                        _errors.Add($"Function '{function.Name}' returns non-void but block '{block.Label}' returns null.");
                    if (ret.Value is not null && function.Signature.ReturnType.Equals(HirType.VoidType))
                        _errors.Add($"Function '{function.Name}' returns void but block '{block.Label}' returns a value.");
                    break;
                case HirThrow:
                case HirAbort:
                case HirUnreachable:
                    break;
                case HirAbortMessage abortMessage:
                    RequireValue(abortMessage.Message, block, nameof(HirAbortMessage));
                    break;
                case null:
                    break;
                default:
                    _errors.Add($"Block '{block.Label}' contains unsupported terminator '{block.Terminator.GetType().Name}'.");
                    break;
            }
        }
    }

    private void CheckInstructions(HirFunction function)
    {
        foreach (var block in function.Blocks)
        {
            foreach (var instruction in block.Instructions)
            {
                switch (instruction)
                {
                    case HirPhi phi:
                        ValidatePhi(phi, block);
                        break;
                    case HirBinaryInst binary:
                        RequireValue(binary.Left, block, instruction.GetType().Name);
                        RequireValue(binary.Right, block, instruction.GetType().Name);
                        break;
                    case HirCompare cmp:
                        RequireValue(cmp.Left, block, nameof(HirCompare));
                        RequireValue(cmp.Right, block, nameof(HirCompare));
                        break;
                    case HirConvert conv:
                        RequireValue(conv.Value, block, nameof(HirConvert));
                        break;
                    case HirStructGet get:
                        RequireValue(get.Object, block, nameof(HirStructGet));
                        break;
                    case HirStructSet set:
                        RequireValue(set.Object, block, nameof(HirStructSet));
                        RequireValue(set.Value, block, nameof(HirStructSet));
                        break;
                    case HirArrayLen len:
                        RequireValue(len.Array, block, nameof(HirArrayLen));
                        break;
                    case HirArrayGet aget:
                        RequireValue(aget.Array, block, nameof(HirArrayGet));
                        RequireValue(aget.Index, block, nameof(HirArrayGet));
                        break;
                    case HirArraySet aset:
                        RequireValue(aset.Array, block, nameof(HirArraySet));
                        RequireValue(aset.Index, block, nameof(HirArraySet));
                        RequireValue(aset.Value, block, nameof(HirArraySet));
                        break;
                    case HirMapGet mget:
                        RequireValue(mget.Map, block, nameof(HirMapGet));
                        RequireValue(mget.Key, block, nameof(HirMapGet));
                        break;
                    case HirMapSet mset:
                        RequireValue(mset.Map, block, nameof(HirMapSet));
                        RequireValue(mset.Key, block, nameof(HirMapSet));
                        RequireValue(mset.Value, block, nameof(HirMapSet));
                        break;
                    case HirMapLen mapLen:
                        RequireValue(mapLen.Map, block, nameof(HirMapLen));
                        break;
                    case HirMapHas mapHas:
                        RequireValue(mapHas.Map, block, nameof(HirMapHas));
                        RequireValue(mapHas.Key, block, nameof(HirMapHas));
                        break;
                    case HirMapDelete mapDelete:
                        RequireValue(mapDelete.Map, block, nameof(HirMapDelete));
                        RequireValue(mapDelete.Key, block, nameof(HirMapDelete));
                        break;
                    case HirNullCheck check:
                        RequireValue(check.Reference, block, nameof(HirNullCheck));
                        break;
                    case HirBoundsCheck bcheck:
                        RequireValue(bcheck.Index, block, nameof(HirBoundsCheck));
                        RequireValue(bcheck.Length, block, nameof(HirBoundsCheck));
                        break;
                    case HirCheckedBinary cbin:
                        RequireValue(cbin.Left, block, nameof(HirCheckedBinary));
                        RequireValue(cbin.Right, block, nameof(HirCheckedBinary));
                        break;
                    case HirCall call:
                        foreach (var arg in call.Arguments)
                            RequireValue(arg, block, nameof(HirCall));
                        break;
                    case HirTryFinallyScope tryScope:
                        RequireBlock(tryScope.TryBlock, block, nameof(HirTryFinallyScope));
                        RequireBlock(tryScope.FinallyBlock, block, nameof(HirTryFinallyScope));
                        RequireBlock(tryScope.MergeBlock, block, nameof(HirTryFinallyScope));
                        foreach (var handler in tryScope.CatchHandlers)
                            RequireBlock(handler.Block, block, nameof(HirTryFinallyScope));
                        break;
                    case HirFinallyScope finallyScope:
                        if (finallyScope.Parent is null)
                            _errors.Add($"Finally scope marker in block '{block.Label}' is missing a parent try scope reference.");
                        break;
                    case HirCatchScope catchScope:
                        if (catchScope.Parent is null)
                            _errors.Add($"Catch scope marker in block '{block.Label}' is missing a parent try scope reference.");
                        else if (!catchScope.Parent.CatchHandlers.Contains(catchScope.Clause))
                            _errors.Add($"Catch scope in block '{block.Label}' references clause not registered with its parent try scope.");
                        break;
                    case HirPointerCall pointerCall:
                        if (pointerCall.Pointer is null && pointerCall.CallTableIndex is null)
                            _errors.Add($"Pointer call in block '{block.Label}' must provide either a pointer operand or a call-table index.");
                        if (pointerCall.Pointer is not null)
                            RequireValue(pointerCall.Pointer, block, nameof(HirPointerCall));
                        foreach (var arg in pointerCall.Arguments)
                            RequireValue(arg, block, nameof(HirPointerCall));
                        break;
                    case HirIntrinsicCall intrinsic:
                        foreach (var arg in intrinsic.Arguments)
                            RequireValue(arg, block, nameof(HirIntrinsicCall));
                        break;
                    case HirNewObject obj:
                        foreach (var arg in obj.Arguments)
                            RequireValue(arg, block, nameof(HirNewObject));
                        break;
                    case HirConcat concat:
                        RequireValue(concat.Left, block, nameof(HirConcat));
                        RequireValue(concat.Right, block, nameof(HirConcat));
                        break;
                    case HirSlice slice:
                        RequireValue(slice.Value, block, nameof(HirSlice));
                        RequireValue(slice.Start, block, nameof(HirSlice));
                        RequireValue(slice.Length, block, nameof(HirSlice));
                        break;
                    case HirBufferNew bufferNew:
                        RequireValue(bufferNew.Length, block, nameof(HirBufferNew));
                        break;
                    case HirBufferSet bufferSet:
                        RequireValue(bufferSet.Buffer, block, nameof(HirBufferSet));
                        RequireValue(bufferSet.Index, block, nameof(HirBufferSet));
                        RequireValue(bufferSet.Byte, block, nameof(HirBufferSet));
                        break;
                    case HirBufferCopy bufferCopy:
                        RequireValue(bufferCopy.Destination, block, nameof(HirBufferCopy));
                        RequireValue(bufferCopy.Source, block, nameof(HirBufferCopy));
                        RequireValue(bufferCopy.DestinationOffset, block, nameof(HirBufferCopy));
                        RequireValue(bufferCopy.SourceOffset, block, nameof(HirBufferCopy));
                        RequireValue(bufferCopy.Length, block, nameof(HirBufferCopy));
                        break;
                }
            }
        }
    }

    private void RequireBlock(HirBlock target, HirBlock source, string context)
    {
        if (!_function!.Blocks.Contains(target))
        {
            _errors.Add($"Block '{source.Label}' references unknown target in {context}.");
        }
    }

    private void RequireValue(HirValue value, HirBlock block, string context)
    {
        if (value is null)
        {
            _errors.Add($"Block '{block.Label}' contains null operand in {context}.");
        }
    }

    private void ValidatePhi(HirPhi phi, HirBlock block)
    {
        if (phi.Inputs.Count == 0)
        {
            _errors.Add($"Phi node in block '{block.Label}' has no incoming values.");
            return;
        }

        foreach (var incoming in phi.Inputs)
        {
            if (!_function!.Blocks.Contains(incoming.Block))
                _errors.Add($"Phi node in block '{block.Label}' has incoming edge from unknown block '{incoming.Block.Label}'.");
            RequireValue(incoming.Value, block, "Phi");
        }
    }
}
