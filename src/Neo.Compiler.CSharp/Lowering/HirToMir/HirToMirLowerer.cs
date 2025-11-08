using System;
using System.Collections.Generic;
using System.Linq;
using Neo.Compiler.HIR;
using Neo.Compiler.HIR.Optimization;
using Neo.Compiler.MIR;
using Neo.Compiler.MIR.Optimization;

namespace Neo.Compiler.MiddleEnd.Lowering;

internal sealed class HirToMirLowerer
{
    internal MirFunction Lower(HirFunction hirFunction, MirModule module)
    {
        if (hirFunction is null)
            throw new ArgumentNullException(nameof(hirFunction));
        if (module is null)
            throw new ArgumentNullException(nameof(module));

        var mirFunction = module.GetOrAddFunction(hirFunction);
        mirFunction.Reset(hirFunction);
        var entry = mirFunction.Entry;

        var blockMap = new Dictionary<HirBlock, MirBlock>
        {
            [hirFunction.Entry] = entry
        };

        foreach (var block in hirFunction.Blocks.Where(b => b != hirFunction.Entry))
        {
            var mirBlock = mirFunction.CreateBlock(block.Label);
            blockMap[block] = mirBlock;
        }

        var valueMap = new Dictionary<HirValue, MirValue>();
        var argumentValues = new Dictionary<HirArgument, MirValue>();
        var tokenIncoming = new Dictionary<HirBlock, List<(HirBlock Pred, MirValue Token)>>();
        var tryScopeMap = new Dictionary<HirTryFinallyScope, MirTry>();
        var catchHandlerMap = new Dictionary<HirCatchClause, MirCatchHandler>();
        var localIncoming = new Dictionary<HirBlock, List<(HirBlock Pred, Dictionary<HirLocal, MirValue> State)>>();

        foreach (var block in hirFunction.Blocks)
            tokenIncoming[block] = new List<(HirBlock, MirValue)>();

        foreach (var block in hirFunction.Blocks)
            localIncoming[block] = new List<(HirBlock, Dictionary<HirLocal, MirValue>)>();

        tokenIncoming[hirFunction.Entry].Add((hirFunction.Entry, mirFunction.EntryToken));

        var hirPredecessors = new Dictionary<HirBlock, HashSet<HirBlock>>(ReferenceEqualityComparer.Instance);
        foreach (var block in hirFunction.Blocks)
            hirPredecessors[block] = new HashSet<HirBlock>(ReferenceEqualityComparer.Instance);

        foreach (var block in hirFunction.Blocks)
        {
            foreach (var successor in GetSuccessors(block.Terminator))
            {
                if (hirPredecessors.TryGetValue(successor, out var preds))
                    preds.Add(block);
            }
        }

        var orderedBlocks = ComputeReversePostOrder(hirFunction);

        foreach (var block in orderedBlocks)
        {
            var mirBlock = blockMap[block];
            var incomingLocalStates = localIncoming[block];
            var localValues = BuildEntryLocalState(block, mirBlock, incomingLocalStates, blockMap);
            incomingLocalStates.Clear();

            var incomingTokens = tokenIncoming[block];
            MirValue currentToken;

            if (incomingTokens.Count == 0)
            {
                currentToken = mirFunction.EntryToken;
            }
            else
            {
                currentToken = incomingTokens[0].Token;
            }

            foreach (var phi in block.Phis)
            {
                var mirPhi = new MirPhi(MirTypeMapper.FromHirType(phi.Type))
                {
                    Span = phi.Span
                };
                mirBlock.AppendPhi(mirPhi);
                valueMap[phi] = mirPhi;
            }

            foreach (var inst in block.Instructions)
            {
                if (valueMap.ContainsKey(inst))
                    continue;

                var (generatedInst, resultValue, updatedToken) = LowerInstruction(
                    inst,
                    valueMap,
                    argumentValues,
                    localValues,
                    mirFunction,
                    currentToken,
                    blockMap,
                    tryScopeMap,
                    catchHandlerMap,
                    mirBlock);

                if (generatedInst is not null)
                {
                    generatedInst.Span = inst.Span;
                    mirBlock.Append(generatedInst);
                }

                if (resultValue is not null && inst.Type is not HirVoidType and not HirNullType)
                    valueMap[inst] = resultValue;

                if (updatedToken is not null)
                    currentToken = updatedToken;
            }

            mirBlock.Terminator = LowerTerminator(block.Terminator, blockMap, valueMap, argumentValues, localValues, mirFunction, tryScopeMap, mirBlock);

            foreach (var successor in GetSuccessors(block.Terminator))
            {
                var list = tokenIncoming[successor];
                var exists = list.Any(entry => ReferenceEquals(entry.Pred, block) && ReferenceEquals(entry.Token, currentToken));
                if (!exists)
                    list.Add((block, currentToken));

                var stateClone = CloneLocalState(localValues);
                localIncoming[successor].Add((block, stateClone));
            }

            if (block.Terminator is HirLeave leaveTerm && tryScopeMap.TryGetValue(leaveTerm.Scope, out _))
            {
                var finallyBlock = leaveTerm.Scope.FinallyBlock;
                if (finallyBlock is not null)
                {
                    var stateClone = CloneLocalState(localValues);
                    localIncoming[finallyBlock].Add((block, stateClone));
                }
            }

        }

        foreach (var block in hirFunction.Blocks)
        {
            var mirBlock = blockMap[block];
            foreach (var phi in block.Phis)
            {
                var mirPhi = (MirPhi)valueMap[phi];
                mirPhi.ResetInputs();
                foreach (var incoming in phi.Inputs)
                {
                    var incomingPred = incoming.Block;
                    if (hirPredecessors.TryGetValue(block, out var preds))
                    {
                        if (preds.Count == 0)
                        {
                            if (!ReferenceEquals(incomingPred, block))
                                continue;
                        }
                        else if (!preds.Contains(incomingPred))
                        {
                            continue;
                        }
                    }

                    var incomingBlock = blockMap[incomingPred];
                    var incomingValue = GetValue(incoming.Value, valueMap, argumentValues, null, mirFunction, incomingBlock);
                    mirPhi.AddIncoming(incomingBlock, incomingValue);
                }
            }
        }

        RebuildTokenPhis(tokenIncoming, blockMap, mirFunction.Entry, orderedBlocks);
        MirPhiUtilities.PruneNonPredecessorInputs(mirFunction);

        return mirFunction;
    }

    private static void RebuildTokenPhis(
        Dictionary<HirBlock, List<(HirBlock Pred, MirValue Token)>> tokenIncoming,
        Dictionary<HirBlock, MirBlock> blockMap,
        MirBlock entryBlock,
        IReadOnlyList<HirBlock> orderedBlocks)
    {
        foreach (var hirBlock in orderedBlocks)
        {
            if (!tokenIncoming.TryGetValue(hirBlock, out var entries))
                continue;

            var mirBlock = blockMap[hirBlock];
            var existing = mirBlock.Phis.FirstOrDefault(phi => phi.Type is MirTokenType);

            var uniqueTokens = new List<MirValue>();
            foreach (var (_, token) in entries)
            {
                if (token is null)
                    continue;

                var seen = false;
                foreach (var recorded in uniqueTokens)
                {
                    if (ReferenceEquals(recorded, token))
                    {
                        seen = true;
                        break;
                    }
                }

                if (!seen)
                    uniqueTokens.Add(token);
            }

            var hasMultipleIncoming = entries
                .Select(entry => entry.Pred)
                .Where(pred => pred is not null)
                .Distinct()
                .Count() > 1;

            var requiresPhi = !ReferenceEquals(mirBlock, entryBlock) && (uniqueTokens.Count > 1 || hasMultipleIncoming);

            if (!requiresPhi)
            {
                if (existing is not null)
                    mirBlock.RemovePhi(existing);
                continue;
            }

            var phiInputs = new List<(MirBlock Pred, MirValue Token)>();
            foreach (var token in uniqueTokens)
            {
                HirBlock? chosenPred = null;

                for (var index = entries.Count - 1; index >= 0; index--)
                {
                    var (pred, incomingToken) = entries[index];
                    if (pred is null || !ReferenceEquals(incomingToken, token))
                        continue;

                    chosenPred = pred;
                    break;
                }

                if (chosenPred is null)
                {
                    for (var index = 0; index < entries.Count; index++)
                    {
                        var (pred, _) = entries[index];
                        if (pred is not null)
                        {
                            chosenPred = pred;
                            break;
                        }
                    }
                }

                if (chosenPred is null)
                    continue;

                phiInputs.Add((blockMap[chosenPred], token));
            }

            if (phiInputs.Count == 0)
            {
                if (existing is not null)
                    mirBlock.RemovePhi(existing);
                continue;
            }

            if (existing is null)
            {
                existing = new MirPhi(MirType.TToken);
                mirBlock.Phis.Insert(0, existing);
            }
            else
            {
                existing.ResetInputs();
            }

            foreach (var (predBlock, token) in phiInputs)
                existing.AddIncoming(predBlock, token);

            foreach (var successor in HirControlFlow.GetSuccessors(hirBlock))
            {
                if (!tokenIncoming.TryGetValue(successor, out var successorEntries))
                    continue;

                for (var i = 0; i < successorEntries.Count; i++)
                {
                    if (!ReferenceEquals(successorEntries[i].Pred, hirBlock))
                        continue;

                    successorEntries[i] = (hirBlock, existing);
                }
            }
        }
    }

    private static List<HirBlock> ComputeReversePostOrder(HirFunction function)
    {
        var visited = new HashSet<HirBlock>();
        var order = new List<HirBlock>();

        void Visit(HirBlock block)
        {
            if (!visited.Add(block))
                return;

            foreach (var successor in HirControlFlow.GetSuccessors(block))
                Visit(successor);

            order.Add(block);
        }

        Visit(function.Entry);

        foreach (var block in function.Blocks)
        {
            if (!visited.Contains(block))
                Visit(block);
        }

        order.Reverse();
        return order;
    }

    private static Dictionary<HirLocal, MirValue> BuildEntryLocalState(
        HirBlock block,
        MirBlock mirBlock,
        List<(HirBlock Pred, Dictionary<HirLocal, MirValue> State)> incomingStates,
        Dictionary<HirBlock, MirBlock> blockMap)
    {
        var localValues = new Dictionary<HirLocal, MirValue>();
        if (incomingStates.Count == 0)
            return localValues;

        var grouped = new Dictionary<HirLocal, List<(HirBlock Pred, MirValue Value)>>();
        foreach (var (pred, state) in incomingStates)
        {
            foreach (var (local, value) in state)
            {
                if (!grouped.TryGetValue(local, out var list))
                {
                    list = new List<(HirBlock, MirValue)>();
                    grouped[local] = list;
                }

                list.Add((pred, value));
            }
        }

        foreach (var (local, entries) in grouped)
        {
            MirValue merged;
            var requiresPhi = entries.Count > 1;
            if (!requiresPhi && entries.Count == 1)
            {
                var (pred, _) = entries[0];
                requiresPhi = !ReferenceEquals(pred, block);
            }

            if (!requiresPhi)
            {
                merged = entries[0].Value;
            }
            else
            {
                var first = entries[0].Value;
                var phi = new MirPhi(first.Type)
                {
                    IsPinned = true
                };

                foreach (var (pred, value) in entries)
                    phi.AddIncoming(blockMap[pred], value);

                mirBlock.AppendPhi(phi);
                merged = phi;
            }

            localValues[local] = merged;
        }

        return localValues;
    }

    private static Dictionary<HirLocal, MirValue> CloneLocalState(Dictionary<HirLocal, MirValue> state)
    {
        var clone = new Dictionary<HirLocal, MirValue>(state.Count);
        foreach (var (local, value) in state)
            clone[local] = value;
        return clone;
    }

    private (MirInst? GeneratedInst, MirValue? ResultValue, MirValue? TokenDelta) LowerInstruction(
        HirInst inst,
        Dictionary<HirValue, MirValue> valueMap,
        Dictionary<HirArgument, MirValue> argumentValues,
        Dictionary<HirLocal, MirValue> localValues,
        MirFunction function,
        MirValue currentToken,
        Dictionary<HirBlock, MirBlock> blockMap,
        Dictionary<HirTryFinallyScope, MirTry> tryScopeMap,
        Dictionary<HirCatchClause, MirCatchHandler> catchHandlerMap,
        MirBlock currentMirBlock)
    {
        MirValue Resolve(HirValue value) => GetValue(value, valueMap, argumentValues, localValues, function, currentMirBlock);

        switch (inst)
        {
            case HirConstInt constInt:
                {
                    var hint = MirTypeMapper.FromHirType(constInt.Type) as MirIntType;
                    var node = new MirConstInt(constInt.Value, hint);
                    return (node, node, null);
                }

            case HirConstBool constBool:
                {
                    var node = new MirConstBool(constBool.Value);
                    return (node, node, null);
                }

            case HirConstByteString constBytes:
                {
                    var node = new MirConstByteString(constBytes.Value);
                    return (node, node, null);
                }

            case HirConstBuffer constBuffer:
                {
                    var node = new MirConstBuffer(constBuffer.Value);
                    return (node, node, null);
                }

            case HirConstNull:
                {
                    var node = new MirConstNull();
                    return (node, node, null);
                }

            case HirLoadArgument loadArg:
                return (null, GetArgumentValue(loadArg.Argument, function, argumentValues, valueMap), null);

            case HirStoreArgument storeArg:
                return StoreArgument(storeArg, valueMap, argumentValues, localValues, function, currentMirBlock);

            case HirLoadLocal loadLocal:
                if (valueMap.TryGetValue(loadLocal.Local, out var existingLocal))
                    return (null, existingLocal, null);
                return (null, GetLocalValue(loadLocal.Local, localValues, valueMap), null);

            case HirStoreLocal storeLocal:
                return StoreLocal(storeLocal, valueMap, argumentValues, localValues, function, currentMirBlock);

            case HirAdd add:
                {
                    var node = new MirBinary(MirBinary.Op.Add, Resolve(add.Left), Resolve(add.Right), MirTypeMapper.FromHirType(add.Type));
                    return (node, node, null);
                }

            case HirSub sub:
                {
                    var node = new MirBinary(MirBinary.Op.Sub, Resolve(sub.Left), Resolve(sub.Right), MirTypeMapper.FromHirType(sub.Type));
                    return (node, node, null);
                }

            case HirMul mul:
                {
                    var node = new MirBinary(MirBinary.Op.Mul, Resolve(mul.Left), Resolve(mul.Right), MirTypeMapper.FromHirType(mul.Type));
                    return (node, node, null);
                }

            case HirDiv div:
                {
                    var node = new MirBinary(MirBinary.Op.Div, Resolve(div.Left), Resolve(div.Right), MirTypeMapper.FromHirType(div.Type));
                    return (node, node, null);
                }

            case HirMod mod:
                {
                    var node = new MirBinary(MirBinary.Op.Mod, Resolve(mod.Left), Resolve(mod.Right), MirTypeMapper.FromHirType(mod.Type));
                    return (node, node, null);
                }

            case HirBitAnd bitAnd:
                {
                    var node = new MirBinary(MirBinary.Op.And, Resolve(bitAnd.Left), Resolve(bitAnd.Right), MirTypeMapper.FromHirType(bitAnd.Type));
                    return (node, node, null);
                }

            case HirBitOr bitOr:
                {
                    var node = new MirBinary(MirBinary.Op.Or, Resolve(bitOr.Left), Resolve(bitOr.Right), MirTypeMapper.FromHirType(bitOr.Type));
                    return (node, node, null);
                }

            case HirBitXor bitXor:
                {
                    var node = new MirBinary(MirBinary.Op.Xor, Resolve(bitXor.Left), Resolve(bitXor.Right), MirTypeMapper.FromHirType(bitXor.Type));
                    return (node, node, null);
                }

            case HirShl shl:
                {
                    var node = new MirBinary(MirBinary.Op.Shl, Resolve(shl.Left), Resolve(shl.Right), MirTypeMapper.FromHirType(shl.Type));
                    return (node, node, null);
                }

            case HirShr shr:
                {
                    var node = new MirBinary(MirBinary.Op.Shr, Resolve(shr.Left), Resolve(shr.Right), MirTypeMapper.FromHirType(shr.Type));
                    return (node, node, null);
                }

            case HirNeg neg:
                {
                    var node = new MirUnary(MirUnary.Op.Neg, Resolve(neg.Operand), MirTypeMapper.FromHirType(neg.Type));
                    return (node, node, null);
                }

            case HirNot not:
                {
                    var node = new MirUnary(MirUnary.Op.Not, Resolve(not.Operand), MirType.TBool);
                    return (node, node, null);
                }

            case HirCompare cmp:
                {
                    var node = new MirCompare(ToMirCompareOp(cmp.Kind), Resolve(cmp.Left), Resolve(cmp.Right), cmp.Unsigned);
                    return (node, node, null);
                }

            case HirCall call:
                {
                    var arguments = call.Arguments.Select(Resolve).ToArray();
                    var node = new MirCall(call.Callee, arguments, MirTypeMapper.FromHirType(call.Type), call.Semantics == HirCallSemantics.Pure);
                    if (call.Semantics != HirCallSemantics.Pure)
                    {
                        var token = node.AttachMemoryToken(currentToken);
                        return (node, node, token);
                    }

                    return (node, node, null);
                }

            case HirPointerCall pointerCall:
                {
                    var pointer = pointerCall.Pointer is null
                        ? null
                        : Resolve(pointerCall.Pointer);
                    var arguments = pointerCall.Arguments.Select(Resolve).ToArray();
                    var node = new MirPointerCall(pointer, arguments, MirTypeMapper.FromHirType(pointerCall.Type), pointerCall.Semantics == HirCallSemantics.Pure, pointerCall.IsTailCall, pointerCall.CallTableIndex);
                    if (pointerCall.Semantics != HirCallSemantics.Pure)
                    {
                        var token = node.AttachMemoryToken(currentToken);
                        return (node, node, token);
                    }

                    return (node, node, null);
                }

            case HirTryFinallyScope tryScope:
                {
                    var catchHandlers = tryScope.CatchHandlers
                        .Select(handler => new MirCatchHandler(blockMap[handler.Block]))
                        .ToArray();
                    foreach (var (clause, handler) in tryScope.CatchHandlers.Zip(catchHandlers))
                        catchHandlerMap[clause] = handler;

                    var mirTry = new MirTry(
                        blockMap[tryScope.TryBlock],
                        blockMap[tryScope.FinallyBlock],
                        blockMap[tryScope.MergeBlock],
                        catchHandlers);
                    tryScopeMap[tryScope] = mirTry;
                    return (mirTry, null, null);
                }

            case HirCatchScope catchScope:
                {
                    if (!tryScopeMap.TryGetValue(catchScope.Parent, out var parentTry))
                        throw new NotSupportedException("Catch scope encountered without matching try scope.");
                    if (!catchHandlerMap.TryGetValue(catchScope.Clause, out var handler))
                        throw new NotSupportedException("Catch scope encountered without registered handler.");
                    var mirCatch = new MirCatch(parentTry, handler);
                    return (mirCatch, null, null);
                }

            case HirFinallyScope finallyScope:
                {
                    if (finallyScope.Parent is null || !tryScopeMap.TryGetValue(finallyScope.Parent, out var parentTry))
                        throw new NotSupportedException("Finally scope encountered without matching try scope.");
                    var mirFinally = new MirFinally(parentTry);
                    return (mirFinally, null, null);
                }

            case HirIntrinsicCall intrinsic:
                {
                    var node = LowerIntrinsicCall(intrinsic, valueMap, argumentValues, localValues, function, currentMirBlock);
                    MirValue? token = null;
                    if (intrinsic.Metadata.RequiresMemoryToken)
                    {
                        token = node.AttachMemoryToken(currentToken);
                    }
                    return (node, node, token);
                }

            case HirNewStruct newStruct:
                {
                    var fields = newStruct.Fields.Select(Resolve).ToArray();
                    var structType = (MirStructType)MirTypeMapper.FromHirType(newStruct.Type);
                    var node = new MirStructPack(fields, structType);
                    return (node, node, null);
                }

            case HirNewObject newObject:
                {
                    var structType = MirTypeMapper.FromHirType(newObject.Type) as MirStructType
                        ?? throw new NotSupportedException("Object creation lowering currently supports struct types only.");
                    var fields = newObject.Arguments.Select(Resolve).ToArray();
                    var node = new MirStructPack(fields, structType);
                    return (node, node, null);
                }

            case HirStructGet structGet:
                {
                    var obj = Resolve(structGet.Object);
                    var node = new MirStructGet(obj, structGet.Index, MirTypeMapper.FromHirType(structGet.Type));
                    return (node, node, null);
                }

            case HirStructSet structSet:
                {
                    var obj = Resolve(structSet.Object);
                    var val = Resolve(structSet.Value);
                    var structType = (MirStructType)MirTypeMapper.FromHirType(structSet.Type);
                    var node = new MirStructSet(obj, structSet.Index, val, structType);
                    var token = node.AttachMemoryToken(currentToken);
                    return (node, node, token);
                }

            case HirLoadStaticField loadStatic:
                {
                    var fieldType = MirTypeMapper.FromHirType(loadStatic.Type);
                    var node = new MirStaticFieldLoad(loadStatic.Slot, fieldType, loadStatic.FieldName);
                    var token = node.AttachMemoryToken(currentToken);
                    return (node, node, token);
                }

            case HirStoreStaticField storeStatic:
                {
                    var value = Resolve(storeStatic.Value);
                    var fieldType = MirTypeMapper.FromHirType(storeStatic.FieldType);
                    var node = new MirStaticFieldStore(storeStatic.Slot, value, fieldType, storeStatic.FieldName);
                    var token = node.AttachMemoryToken(currentToken);
                    return (node, value, token);
                }

            case HirArrayNew arrayNew:
                {
                    var length = Resolve(arrayNew.Length);
                    var elementType = MirTypeMapper.FromHirType(((HirArrayType)arrayNew.Type).ElementType);
                    var node = new MirArrayNew(length, elementType);
                    return (node, node, null);
                }

            case HirArrayLen arrayLen:
                {
                    var arr = Resolve(arrayLen.Array);
                    var node = new MirArrayLen(arr);
                    return (node, node, null);
                }

            case HirArrayGet arrayGet:
                {
                    var arr = Resolve(arrayGet.Array);
                    var idx = Resolve(arrayGet.Index);
                    var node = new MirArrayGet(arr, idx, MirTypeMapper.FromHirType(arrayGet.Type));
                    return (node, node, null);
                }

            case HirArraySet arraySet:
                {
                    var arr = Resolve(arraySet.Array);
                    var idx = Resolve(arraySet.Index);
                    var val = Resolve(arraySet.Value);
                    var node = new MirArraySet(arr, idx, val);
                    var token = node.AttachMemoryToken(currentToken);
                    return (node, node, token);
                }

            case HirMapNew mapNew:
                {
                    var node = new MirMapNew(MirTypeMapper.FromHirType(mapNew.KeyType), MirTypeMapper.FromHirType(mapNew.ValueType));
                    return (node, node, null);
                }

            case HirMapGet mapGet:
                {
                    var map = Resolve(mapGet.Map);
                    var key = Resolve(mapGet.Key);
                    var node = new MirMapGet(map, key, MirTypeMapper.FromHirType(mapGet.Type));
                    return (node, node, null);
                }

            case HirMapSet mapSet:
                {
                    var map = Resolve(mapSet.Map);
                    var key = Resolve(mapSet.Key);
                    var val = Resolve(mapSet.Value);
                    var node = new MirMapSet(map, key, val);
                    var token = node.AttachMemoryToken(currentToken);
                    return (node, node, token);
                }

            case HirMapLen mapLen:
                {
                    var map = Resolve(mapLen.Map);
                    var node = new MirMapLen(map);
                    return (node, node, null);
                }

            case HirMapHas mapHas:
                {
                    var map = Resolve(mapHas.Map);
                    var key = Resolve(mapHas.Key);
                    var node = new MirMapHas(map, key);
                    return (node, node, null);
                }

            case HirMapDelete mapDelete:
                {
                    var map = Resolve(mapDelete.Map);
                    var key = Resolve(mapDelete.Key);
                    var node = new MirMapDelete(map, key);
                    var token = node.AttachMemoryToken(currentToken);
                    return (node, node, token);
                }

            case HirConvert convert:
                {
                    var node = new MirConvert(ToMirConvertKind(convert.Kind), Resolve(convert.Value), MirTypeMapper.FromHirType(convert.Type));
                    return (node, node, null);
                }

            case HirNullCheck nullCheck when nullCheck.Policy == HirFailPolicy.Assume:
                return (null, null, null);

            case HirNullCheck nullCheck:
                {
                    var node = new MirGuardNull(Resolve(nullCheck.Reference), ToGuardFail(nullCheck.Policy));
                    return (node, null, null);
                }

            case HirBoundsCheck boundsCheck when boundsCheck.Policy == HirFailPolicy.Assume:
                return (null, null, null);

            case HirBoundsCheck boundsCheck:
                {
                    var node = new MirGuardBounds(Resolve(boundsCheck.Index), Resolve(boundsCheck.Length), ToGuardFail(boundsCheck.Policy));
                    return (node, null, null);
                }

            case HirCheckedBinary checkedBinary when checkedBinary.Policy == HirFailPolicy.Assume:
                {
                    var op = ToMirBinaryOp(checkedBinary.Operation);
                    var left = Resolve(checkedBinary.Left);
                    var right = Resolve(checkedBinary.Right);
                    var node = new MirBinary(op, left, right, MirTypeMapper.FromHirType(checkedBinary.Type));
                    return (node, node, null);
                }

            case HirCheckedBinary checkedBinary:
                {
                    var left = Resolve(checkedBinary.Left);
                    var right = Resolve(checkedBinary.Right);
                    MirInst node = checkedBinary.Operation switch
                    {
                        HirCheckedOp.Add => new MirCheckedAdd(left, right, MirTypeMapper.FromHirType(checkedBinary.Type), ToGuardFail(checkedBinary.Policy)),
                        HirCheckedOp.Sub => new MirCheckedSub(left, right, MirTypeMapper.FromHirType(checkedBinary.Type), ToGuardFail(checkedBinary.Policy)),
                        HirCheckedOp.Mul => new MirCheckedMul(left, right, MirTypeMapper.FromHirType(checkedBinary.Type), ToGuardFail(checkedBinary.Policy)),
                        _ => throw new NotSupportedException($"Checked operation '{checkedBinary.Operation}' is not supported.")
                    };
                    return (node, node, null);
                }

            case HirConcat concat:
                {
                    var left = Resolve(concat.Left);
                    var right = Resolve(concat.Right);
                    var node = new MirConcat(left, right);
                    return (node, node, null);
                }

            case HirSlice slice:
                {
                    var value = Resolve(slice.Value);
                    var start = Resolve(slice.Start);
                    var length = Resolve(slice.Length);
                    var node = new MirSlice(value, start, length, slice.IsBufferSlice);
                    return (node, node, null);
                }

            case HirBufferNew bufferNew:
                {
                    var length = Resolve(bufferNew.Length);
                    var node = new MirBufferNew(length);
                    var token = node.AttachMemoryToken(currentToken);
                    return (node, node, token);
                }

            case HirBufferSet bufferSet:
                {
                    var buffer = Resolve(bufferSet.Buffer);
                    var index = Resolve(bufferSet.Index);
                    var @byte = Resolve(bufferSet.Byte);
                    var node = new MirBufferSet(buffer, index, @byte);
                    var token = node.AttachMemoryToken(currentToken);
                    return (node, node, token);
                }

            case HirBufferCopy bufferCopy:
                {
                    var destination = Resolve(bufferCopy.Destination);
                    var source = Resolve(bufferCopy.Source);
                    var destinationOffset = Resolve(bufferCopy.DestinationOffset);
                    var sourceOffset = Resolve(bufferCopy.SourceOffset);
                    var length = Resolve(bufferCopy.Length);
                    var node = new MirBufferCopy(destination, source, destinationOffset, sourceOffset, length);
                    var token = node.AttachMemoryToken(currentToken);
                    return (node, node, token);
                }

            default:
                throw new NotSupportedException($"HIR instruction '{inst.GetType().Name}' is not yet supported in MIR lowering.");
        }
    }

    private static MirCompare.Op ToMirCompareOp(HirCmpKind kind) => kind switch
    {
        HirCmpKind.Eq => MirCompare.Op.Eq,
        HirCmpKind.Ne => MirCompare.Op.Ne,
        HirCmpKind.Lt => MirCompare.Op.Lt,
        HirCmpKind.Le => MirCompare.Op.Le,
        HirCmpKind.Gt => MirCompare.Op.Gt,
        HirCmpKind.Ge => MirCompare.Op.Ge,
        _ => throw new NotSupportedException($"Comparison kind {kind} not supported")
    };

    private static MirConvert.Kind ToMirConvertKind(HirConvKind kind) => kind switch
    {
        HirConvKind.SignExtend => MirConvert.Kind.SignExtend,
        HirConvKind.ZeroExtend => MirConvert.Kind.ZeroExtend,
        HirConvKind.Narrow => MirConvert.Kind.Narrow,
        HirConvKind.ToBool => MirConvert.Kind.ToBool,
        HirConvKind.ToByteString => MirConvert.Kind.ToByteString,
        HirConvKind.ToBuffer => MirConvert.Kind.ToBuffer,
        _ => throw new NotSupportedException($"Conversion kind {kind} not supported.")
    };

    private static MirBinary.Op ToMirBinaryOp(HirCheckedOp op) => op switch
    {
        HirCheckedOp.Add => MirBinary.Op.Add,
        HirCheckedOp.Sub => MirBinary.Op.Sub,
        HirCheckedOp.Mul => MirBinary.Op.Mul,
        _ => throw new NotSupportedException($"Checked operation {op} not supported")
    };

    private static MirGuardFail ToGuardFail(HirFailPolicy policy) => policy switch
    {
        HirFailPolicy.Abort => MirGuardFail.Abort,
        HirFailPolicy.PathSplit => MirGuardFail.Branch,
        HirFailPolicy.Assume => MirGuardFail.Abort,
        _ => MirGuardFail.Abort
    };

    private static IEnumerable<HirBlock> GetSuccessors(HirTerminator? terminator) => terminator switch
    {
        HirBranch br => new[] { br.Target },
        HirConditionalBranch cond => new[] { cond.TrueBlock, cond.FalseBlock },
        HirSwitch sw => sw.Cases.Select(c => c.Target).Append(sw.DefaultTarget),
        HirLeave leave => new[] { leave.Target },
        HirEndFinally endFinally => new[] { endFinally.Target },
        _ => Array.Empty<HirBlock>()
    };

    private static MirSyscall LowerIntrinsicCall(
        HirIntrinsicCall intrinsic,
        Dictionary<HirValue, MirValue> valueMap,
        Dictionary<HirArgument, MirValue> argumentValues,
        Dictionary<HirLocal, MirValue> localValues,
        MirFunction function,
        MirBlock currentBlock)
    {
        var args = intrinsic.Arguments.Select(a => GetValue(a, valueMap, argumentValues, localValues, function, currentBlock)).ToArray();
        var effect = (MirEffect)intrinsic.Metadata.Effect;
        return new MirSyscall(
            intrinsic.Metadata.Category,
            intrinsic.Metadata.Name,
            args,
            MirTypeMapper.FromHirType(intrinsic.Metadata.ReturnType),
            effect,
            intrinsic.Metadata.GasCostHint.HasValue ? (ulong?)intrinsic.Metadata.GasCostHint.Value : null);
    }

    private static MirTerminator LowerThrow(
        HirThrow throwTerm,
        Dictionary<HirValue, MirValue> valueMap,
        Dictionary<HirArgument, MirValue> argumentValues,
        Dictionary<HirLocal, MirValue> localValues,
        MirFunction function,
        MirBlock currentBlock)
    {
        if (throwTerm.Exception is null)
            return new MirAbort();

        var message = GetValue(throwTerm.Exception, valueMap, argumentValues, localValues, function, currentBlock);
        return message.Type is MirByteStringType
            ? new MirAbortMsg(message)
            : new MirAbort();
    }

    private static MirTerminator? LowerTerminator(
        HirTerminator? terminator,
        Dictionary<HirBlock, MirBlock> blockMap,
        Dictionary<HirValue, MirValue> valueMap,
        Dictionary<HirArgument, MirValue> argumentValues,
        Dictionary<HirLocal, MirValue> localValues,
        MirFunction function,
        Dictionary<HirTryFinallyScope, MirTry> tryScopeMap,
        MirBlock currentBlock)
    {
        return terminator switch
        {
            null => null,
            HirReturn ret => new MirReturn(ret.Value is null ? null : GetValue(ret.Value, valueMap, argumentValues, localValues, function, currentBlock)),
            HirBranch br => new MirBranch(blockMap[br.Target]),
            HirConditionalBranch cond => new MirCondBranch(GetValue(cond.Condition, valueMap, argumentValues, localValues, function, currentBlock), blockMap[cond.TrueBlock], blockMap[cond.FalseBlock]),
            HirSwitch @switch => new MirSwitch(GetValue(@switch.Key, valueMap, argumentValues, localValues, function, currentBlock), @switch.Cases.Select(c => (c.Case, blockMap[c.Target])).ToArray(), blockMap[@switch.DefaultTarget]),
            HirAbort => new MirAbort(),
            HirAbortMessage abortMessage => new MirAbortMsg(GetValue(abortMessage.Message, valueMap, argumentValues, localValues, function, currentBlock)),
            HirThrow throwTerm => LowerThrow(throwTerm, valueMap, argumentValues, localValues, function, currentBlock),
            HirLeave leave => tryScopeMap.TryGetValue(leave.Scope, out var mirScope)
                ? new MirLeave(mirScope, blockMap[leave.Target])
                : throw new NotSupportedException("Encountered HirLeave without matching MirTry scope."),
            HirEndFinally endFinally => tryScopeMap.TryGetValue(endFinally.Scope, out var mirTry)
                ? new MirEndFinally(mirTry, blockMap[endFinally.Target])
                : throw new NotSupportedException("Encountered HirEndFinally without matching MirTry scope."),
            HirUnreachable => new MirUnreachable(),
            _ => throw new NotSupportedException($"Terminator '{terminator.GetType().Name}' is not yet supported in MIR lowering.")
        };
    }

    private static MirValue GetValue(
        HirValue value,
        Dictionary<HirValue, MirValue> valueMap,
        Dictionary<HirArgument, MirValue>? argumentValues,
        Dictionary<HirLocal, MirValue>? localValues,
        MirFunction? function,
        MirBlock? currentBlock = null)
    {
        if (valueMap.TryGetValue(value, out var existing))
            return existing;

        switch (value)
        {
            case HirArgument argument when argumentValues is not null && function is not null:
                return GetArgumentValue(argument, function, argumentValues, valueMap);
            case HirLoadArgument loadArg when argumentValues is not null && function is not null:
                return GetArgumentValue(loadArg.Argument, function, argumentValues, valueMap);
            case HirLocal local when localValues is not null:
                if (localValues.TryGetValue(local, out var localValue))
                    return localValue;
                break;
            case HirLoadLocal loadLocal when localValues is not null:
                return GetLocalValue(loadLocal.Local, localValues, valueMap);
            case HirConstInt constInt:
                {
                    var hint = MirTypeMapper.FromHirType(constInt.Type) as MirIntType;
                    var node = new MirConstInt(constInt.Value, hint) { Span = constInt.Span };
                    valueMap[value] = node;
                    AppendInstruction(currentBlock, node);
                    return node;
                }
            case HirConstBool constBool:
                {
                    var node = new MirConstBool(constBool.Value) { Span = constBool.Span };
                    valueMap[value] = node;
                    AppendInstruction(currentBlock, node);
                    return node;
                }
            case HirConstByteString constBytes:
                {
                    var node = new MirConstByteString(constBytes.Value) { Span = constBytes.Span };
                    valueMap[value] = node;
                    AppendInstruction(currentBlock, node);
                    return node;
                }
            case HirConstBuffer constBuffer:
                {
                    var node = new MirConstBuffer(constBuffer.Value) { Span = constBuffer.Span };
                    valueMap[value] = node;
                    AppendInstruction(currentBlock, node);
                    return node;
                }
            case HirConstNull constNull:
                {
                    var node = new MirConstNull() { Span = constNull.Span };
                    valueMap[value] = node;
                    AppendInstruction(currentBlock, node);
                    return node;
                }
            case HirBinaryInst binary:
            {
                if (!TryMapBinary(binary, valueMap, argumentValues, localValues, function, currentBlock, out var binResult))
                    break;
                binResult.Span = binary.Span;
                valueMap[value] = binResult;
                return binResult;
            }
            case HirNeg neg:
                {
                    var operand = GetValue(neg.Operand, valueMap, argumentValues, localValues, function, currentBlock);
                    var node = new MirUnary(MirUnary.Op.Neg, operand, MirTypeMapper.FromHirType(neg.Type)) { Span = neg.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirNot not:
                {
                    var operand = GetValue(not.Operand, valueMap, argumentValues, localValues, function, currentBlock);
                    var node = new MirUnary(MirUnary.Op.Not, operand, MirType.TBool) { Span = not.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirCompare compare:
                {
                    var left = GetValue(compare.Left, valueMap, argumentValues, localValues, function, currentBlock);
                    var right = GetValue(compare.Right, valueMap, argumentValues, localValues, function, currentBlock);
                    var node = new MirCompare(ToMirCompareOp(compare.Kind), left, right, compare.Unsigned) { Span = compare.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirCall call when call.Semantics == HirCallSemantics.Pure && function is not null && argumentValues is not null:
                {
                    var args = call.Arguments.Select(a => GetValue(a, valueMap, argumentValues, localValues, function, currentBlock)).ToArray();
                    var node = new MirCall(call.Callee, args, MirTypeMapper.FromHirType(call.Type), isPure: true) { Span = call.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirPointerCall pointerCall when pointerCall.Semantics == HirCallSemantics.Pure && function is not null && argumentValues is not null:
                {
                    var pointer = pointerCall.Pointer is null
                        ? null
                        : GetValue(pointerCall.Pointer, valueMap, argumentValues, localValues, function, currentBlock);
                    var args = pointerCall.Arguments.Select(a => GetValue(a, valueMap, argumentValues, localValues, function, currentBlock)).ToArray();
                    var node = new MirPointerCall(pointer, args, MirTypeMapper.FromHirType(pointerCall.Type), isPure: true, pointerCall.IsTailCall, pointerCall.CallTableIndex)
                    {
                        Span = pointerCall.Span
                    };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirIntrinsicCall intrinsic when !intrinsic.Metadata.RequiresMemoryToken && function is not null && argumentValues is not null && localValues is not null && currentBlock is not null:
                {
                    var node = LowerIntrinsicCall(intrinsic, valueMap, argumentValues, localValues, function, currentBlock);
                    node.Span = intrinsic.Span;
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirNewStruct newStruct:
                {
                    var fields = newStruct.Fields.Select(f => GetValue(f, valueMap, argumentValues, localValues, function, currentBlock)).ToArray();
                    var node = new MirStructPack(fields, (MirStructType)MirTypeMapper.FromHirType(newStruct.Type)) { Span = newStruct.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirNewObject newObject:
                {
                    var structType = MirTypeMapper.FromHirType(newObject.Type) as MirStructType
                        ?? throw new NotSupportedException("Object creation lowering currently supports struct types only.");
                    var fields = newObject.Arguments.Select(arg => GetValue(arg, valueMap, argumentValues, localValues, function, currentBlock)).ToArray();
                    var node = new MirStructPack(fields, structType) { Span = newObject.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirStructGet structGet:
                {
                    var obj = GetValue(structGet.Object, valueMap, argumentValues, localValues, function, currentBlock);
                    var node = new MirStructGet(obj, structGet.Index, MirTypeMapper.FromHirType(structGet.Type)) { Span = structGet.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirArrayLen arrayLen:
                {
                    var array = GetValue(arrayLen.Array, valueMap, argumentValues, localValues, function, currentBlock);
                    var node = new MirArrayLen(array) { Span = arrayLen.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirArrayGet arrayGet:
                {
                    var array = GetValue(arrayGet.Array, valueMap, argumentValues, localValues, function, currentBlock);
                    var index = GetValue(arrayGet.Index, valueMap, argumentValues, localValues, function, currentBlock);
                    var node = new MirArrayGet(array, index, MirTypeMapper.FromHirType(arrayGet.Type)) { Span = arrayGet.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirArrayNew arrayNew:
                {
                    var length = GetValue(arrayNew.Length, valueMap, argumentValues, localValues, function, currentBlock);
                    var elementType = MirTypeMapper.FromHirType(arrayNew.ElementType);
                    var node = new MirArrayNew(length, elementType) { Span = arrayNew.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirMapNew mapNew:
                {
                    var node = new MirMapNew(MirTypeMapper.FromHirType(mapNew.KeyType), MirTypeMapper.FromHirType(mapNew.ValueType)) { Span = mapNew.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirMapGet mapGet:
                {
                    var map = GetValue(mapGet.Map, valueMap, argumentValues, localValues, function, currentBlock);
                    var key = GetValue(mapGet.Key, valueMap, argumentValues, localValues, function, currentBlock);
                    var node = new MirMapGet(map, key, MirTypeMapper.FromHirType(mapGet.Type)) { Span = mapGet.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirMapLen mapLen:
                {
                    var map = GetValue(mapLen.Map, valueMap, argumentValues, localValues, function, currentBlock);
                    var node = new MirMapLen(map) { Span = mapLen.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirMapHas mapHas:
                {
                    var map = GetValue(mapHas.Map, valueMap, argumentValues, localValues, function, currentBlock);
                    var key = GetValue(mapHas.Key, valueMap, argumentValues, localValues, function, currentBlock);
                    var node = new MirMapHas(map, key) { Span = mapHas.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirConvert convert:
                {
                    var operand = GetValue(convert.Value, valueMap, argumentValues, localValues, function, currentBlock);
                    var node = new MirConvert(ToMirConvertKind(convert.Kind), operand, MirTypeMapper.FromHirType(convert.Type)) { Span = convert.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirCheckedBinary checkedBinary when checkedBinary.Policy == HirFailPolicy.Assume:
                {
                    var left = GetValue(checkedBinary.Left, valueMap, argumentValues, localValues, function, currentBlock);
                    var right = GetValue(checkedBinary.Right, valueMap, argumentValues, localValues, function, currentBlock);
                    var node = new MirBinary(ToMirBinaryOp(checkedBinary.Operation), left, right, MirTypeMapper.FromHirType(checkedBinary.Type))
                    {
                        Span = checkedBinary.Span
                    };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirCheckedBinary checkedBinary:
                {
                    var left = GetValue(checkedBinary.Left, valueMap, argumentValues, localValues, function, currentBlock);
                    var right = GetValue(checkedBinary.Right, valueMap, argumentValues, localValues, function, currentBlock);
                    MirInst node = checkedBinary.Operation switch
                    {
                        HirCheckedOp.Add => new MirCheckedAdd(left, right, MirTypeMapper.FromHirType(checkedBinary.Type), ToGuardFail(checkedBinary.Policy)),
                        HirCheckedOp.Sub => new MirCheckedSub(left, right, MirTypeMapper.FromHirType(checkedBinary.Type), ToGuardFail(checkedBinary.Policy)),
                        HirCheckedOp.Mul => new MirCheckedMul(left, right, MirTypeMapper.FromHirType(checkedBinary.Type), ToGuardFail(checkedBinary.Policy)),
                        _ => throw new NotSupportedException($"Checked operation '{checkedBinary.Operation}' is not supported.")
                    };
                    node.Span = checkedBinary.Span;
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirConcat concat:
                {
                    var left = GetValue(concat.Left, valueMap, argumentValues, localValues, function, currentBlock);
                    var right = GetValue(concat.Right, valueMap, argumentValues, localValues, function, currentBlock);
                    var node = new MirConcat(left, right) { Span = concat.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
            case HirSlice slice:
                {
                    var valueOperand = GetValue(slice.Value, valueMap, argumentValues, localValues, function, currentBlock);
                    var start = GetValue(slice.Start, valueMap, argumentValues, localValues, function, currentBlock);
                    var length = GetValue(slice.Length, valueMap, argumentValues, localValues, function, currentBlock);
                    var node = new MirSlice(valueOperand, start, length, slice.IsBufferSlice) { Span = slice.Span };
                    AppendInstruction(currentBlock, node);
                    valueMap[value] = node;
                    return node;
                }
        }

        throw new NotSupportedException($"HIR value '{value.GetType().Name}' was not lowered before use.");
    }

    private static void AppendInstruction(MirBlock? block, MirInst inst)
    {
        if (block is null)
            return;
        block.Append(inst);
    }

    private static bool TryMapBinary(
        HirBinaryInst binary,
        Dictionary<HirValue, MirValue> valueMap,
        Dictionary<HirArgument, MirValue>? argumentValues,
        Dictionary<HirLocal, MirValue>? localValues,
        MirFunction? function,
        MirBlock? currentBlock,
        out MirBinary result)
    {
        result = null!;
        MirBinary.Op opCode = binary switch
        {
            HirAdd => MirBinary.Op.Add,
            HirSub => MirBinary.Op.Sub,
            HirMul => MirBinary.Op.Mul,
            HirDiv => MirBinary.Op.Div,
            HirMod => MirBinary.Op.Mod,
            HirBitAnd => MirBinary.Op.And,
            HirBitOr => MirBinary.Op.Or,
            HirBitXor => MirBinary.Op.Xor,
            HirShl => MirBinary.Op.Shl,
            HirShr => MirBinary.Op.Shr,
            _ => (MirBinary.Op)(-1)
        };

        if ((int)opCode < 0)
            return false;

        var left = GetValue(binary.Left, valueMap, argumentValues, localValues, function, currentBlock);
        var right = GetValue(binary.Right, valueMap, argumentValues, localValues, function, currentBlock);
        result = new MirBinary(opCode, left, right, MirTypeMapper.FromHirType(binary.Type))
        {
            Span = binary.Span
        };
        AppendInstruction(currentBlock, result);
        return true;
    }

    private static MirValue GetArgumentValue(
        HirArgument argument,
        MirFunction function,
        Dictionary<HirArgument, MirValue> argumentValues,
        Dictionary<HirValue, MirValue> valueMap)
    {
        if (argumentValues.TryGetValue(argument, out var existing))
            return existing;

        var mirArg = new MirArg(argument.Index, MirTypeMapper.FromHirType(argument.Type));
        function.Entry.Instructions.Insert(function.Entry.Instructions.Count, mirArg);
        argumentValues[argument] = mirArg;
        valueMap[argument] = mirArg;
        return mirArg;
    }

    private static (MirInst? GeneratedInst, MirValue? ResultValue, MirValue? TokenDelta) StoreArgument(
        HirStoreArgument storeArg,
        Dictionary<HirValue, MirValue> valueMap,
        Dictionary<HirArgument, MirValue> argumentValues,
        Dictionary<HirLocal, MirValue> localValues,
        MirFunction function,
        MirBlock currentBlock)
    {
        var value = GetValue(storeArg.Value, valueMap, argumentValues, localValues, function, currentBlock);
        argumentValues[storeArg.Argument] = value;
        valueMap[storeArg.Argument] = value;
        return (null, null, null);
    }

    private static MirValue GetLocalValue(
        HirLocal local,
        Dictionary<HirLocal, MirValue> localValues,
        Dictionary<HirValue, MirValue> valueMap)
    {
        if (localValues.TryGetValue(local, out var value))
            return value;

        if (valueMap.TryGetValue(local, out var cached))
            return cached;

        throw new InvalidOperationException($"Local '{local.Name}' was read before assignment during HIR→MIR lowering.");
    }

    private static (MirInst? GeneratedInst, MirValue? ResultValue, MirValue? TokenDelta) StoreLocal(
        HirStoreLocal storeLocal,
        Dictionary<HirValue, MirValue> valueMap,
        Dictionary<HirArgument, MirValue> argumentValues,
        Dictionary<HirLocal, MirValue> localValues,
        MirFunction function,
        MirBlock currentBlock)
    {
        MirValue value;
        if (storeLocal.Value is HirLoadLocal loadValue)
        {
            value = GetLocalValue(loadValue.Local, localValues, valueMap);
        }
        else
        {
            value = GetValue(storeLocal.Value, valueMap, argumentValues, localValues, function, currentBlock);
        }
        localValues[storeLocal.Local] = value;
        var isScopeSlot = storeLocal.Local.Name.StartsWith("try_state_", StringComparison.Ordinal);
        if (isScopeSlot)
            valueMap.Remove(storeLocal.Local);
        else
            valueMap[storeLocal.Local] = value;

        return (null, null, null);
    }
}
