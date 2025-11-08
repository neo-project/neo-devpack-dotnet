using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR.Backend;

/// <summary>
/// Lowers VReg-LIR into Stack-LIR while maintaining stack discipline. The scheduler keeps a precise model of the
/// evaluation stack, inserts the minimal shuffles needed to prepare operands, and accounts for residual uses so values
/// survive until their final consumer.
/// </summary>
internal sealed partial class StackScheduler
{
    private static readonly bool s_traceStack =
        string.Equals(Environment.GetEnvironmentVariable("NEO_IR_TRACE_STACK"), "1", StringComparison.OrdinalIgnoreCase);

    private void EmitModMul(LirBlock block, VModMul node)
    {
        PrepareOperands(block, node.Span, (node.Left, false), (node.Right, false), (node.Modulus, false));
        EmitInstruction(block, new LirInst(LirOpcode.MODMUL) { Span = node.Span });
        TagResult(node);
        DropIfDead(block, node);
    }

    private void EmitModPow(LirBlock block, VModPow node)
    {
        PrepareOperands(block, node.Span, (node.Value, false), (node.Exponent, false), (node.Modulus, false));
        EmitInstruction(block, new LirInst(LirOpcode.MODPOW) { Span = node.Span });
        TagResult(node);
        DropIfDead(block, node);
    }

    private readonly List<StackSlot> _stack = new();
    private readonly Dictionary<VNode, int> _useCounts = new();
    private readonly Dictionary<VNode, int> _remainingUses = new();
    private readonly Dictionary<VNode, int> _reservationScratch = new();
    private readonly Dictionary<VBlock, List<VPhi>> _blockPhis = new();
    private readonly Dictionary<(VBlock From, VBlock To), List<VNode>> _edgePhiTransfers = new();
    private readonly Dictionary<VBlock, List<VNode>> _blockEntryStacks = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<VBlock, HashSet<VBlock>> _predecessors = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<VTry, TryScopeInfo> _tryScopeInfo = new();
    private readonly HashSet<VNode> _emittedNodes = new();
    private readonly Dictionary<VNode, int> _sethiNumbers = new();
    private readonly HashSet<VNode> _sethiVisiting = new();

    private int _maxStack;
    private VBlock? _currentBlock;

    internal Result Lower(VFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var lowered = new LirFunction(function.Name);
        lowered.EntryParameterCount = function.ParameterCount;
        _stack.Clear();
        _useCounts.Clear();
        _remainingUses.Clear();
        _blockPhis.Clear();
        _edgePhiTransfers.Clear();
        _tryScopeInfo.Clear();
        _emittedNodes.Clear();
        _maxStack = 0;
        _blockEntryStacks.Clear();
        _predecessors.Clear();

        var entryBlock = function.Blocks.FirstOrDefault() ?? throw new InvalidOperationException("VFunction contains no blocks.");

        AnalyzePhiTransfers(function);
        ComputeUseCounts(function);
        ComputeSethiNumbers(function);
        BuildPredecessors(function);

        var scheduled = new HashSet<VBlock>(ReferenceEqualityComparer.Instance);
        var queue = new Queue<VBlock>(function.Blocks);
        var guard = function.Blocks.Count * 4;
        while (queue.Count > 0)
        {
            if (--guard < 0)
                throw new InvalidOperationException("Failed to determine stack entry state for all blocks during scheduling.");

            var vBlock = queue.Dequeue();
            if (scheduled.Contains(vBlock))
                continue;

            if (!CanScheduleBlock(vBlock, entryBlock, scheduled))
            {
                queue.Enqueue(vBlock);
                continue;
            }

            _currentBlock = vBlock;
            var lirBlock = new LirBlock(vBlock.Label);
            lowered.Blocks.Add(lirBlock);

            _stack.Clear();
            SeedEntryStack(vBlock);

            foreach (var node in vBlock.Nodes)
                ScheduleNode(node, lirBlock);

            ScheduleTerminator(vBlock, lirBlock);

            _stack.Clear();
            _currentBlock = null;
            scheduled.Add(vBlock);
        }
        _blockEntryStacks.Clear();

        StackEffectAnnotator.Annotate(lowered);
        return new Result(lowered, _maxStack);
    }

    private void BuildPredecessors(VFunction function)
    {
        foreach (var block in function.Blocks)
            _predecessors[block] = new HashSet<VBlock>(ReferenceEqualityComparer.Instance);

        foreach (var block in function.Blocks)
        {
            foreach (var successor in EnumerateSuccessors(block))
            {
                if (_predecessors.TryGetValue(successor, out var set))
                    set.Add(block);
            }
        }
    }

    private static IEnumerable<VBlock> EnumerateSuccessors(VBlock block)
    {
        if (block.Terminator is null)
            yield break;

        switch (block.Terminator)
        {
            case VJmp jmp:
                yield return jmp.Target;
                break;
            case VJmpIf jmpIf:
                yield return jmpIf.TrueTarget;
                yield return jmpIf.FalseTarget;
                break;
            case VCompareBranch cmp:
                yield return cmp.TrueTarget;
                yield return cmp.FalseTarget;
                break;
            case VSwitch vSwitch:
                foreach (var (_, target) in vSwitch.Cases)
                    yield return target;
                yield return vSwitch.DefaultTarget;
                break;
            case VLeave leave:
                yield return leave.Target;
                break;
            case VEndFinally endFinally:
                yield return endFinally.Target;
                break;
        }
    }

    private bool CanScheduleBlock(VBlock block, VBlock entry, HashSet<VBlock> scheduled)
    {
        if (ReferenceEquals(block, entry))
            return true;

        if (_blockPhis.TryGetValue(block, out var phis) && phis.Count > 0)
            return true;

        if (_blockEntryStacks.ContainsKey(block))
            return true;

        if (_predecessors.TryGetValue(block, out var preds))
        {
            if (preds.Count == 0)
                return true;

            var allScheduled = true;
            foreach (var pred in preds)
            {
                if (!scheduled.Contains(pred))
                {
                    allScheduled = false;
                    break;
                }
            }

            if (allScheduled)
                return true;
        }

        return false;
    }

    private void AnalyzePhiTransfers(VFunction function)
    {
        foreach (var block in function.Blocks)
        {
            List<VPhi>? phisForBlock = null;

            foreach (var node in block.Nodes)
            {
                if (node is not VPhi phi)
                    continue;

                phisForBlock ??= new List<VPhi>();
                phisForBlock.Add(phi);

                foreach (var (pred, value) in phi.Inputs)
                {
                    var key = (pred, block);
                    if (!_edgePhiTransfers.TryGetValue(key, out var list))
                    {
                        list = new List<VNode>();
                        _edgePhiTransfers[key] = list;
                    }

                    list.Add(value);
                }
            }

            if (phisForBlock is not null)
                _blockPhis[block] = phisForBlock;
        }
    }

    #region Use counting

    private void ComputeUseCounts(VFunction function)
    {
        foreach (var block in function.Blocks)
        {
            foreach (var node in block.Nodes)
                CountUses(node);

            foreach (var phi in block.Nodes)
            {
                if (phi is VPhi vPhi)
                {
                    foreach (var (_, value) in vPhi.Inputs)
                        AddUse(value);
                }
            }

            switch (block.Terminator)
            {
                case VRet ret when ret.Value is not null:
                    AddUse(ret.Value);
                    break;

                case VJmpIf jmpIf:
                    AddUse(jmpIf.Condition);
                    break;

                case VSwitch vSwitch:
                    AddUse(vSwitch.Key);
                    break;

                case VCompareBranch cmpBranch:
                    AddUse(cmpBranch.Left);
                    AddUse(cmpBranch.Right);
                    break;

                case VAbortMsg abortMsg:
                    AddUse(abortMsg.Message);
                    break;
            }
        }

        foreach (var (node, uses) in _useCounts)
            _remainingUses[node] = uses;
    }

    private void CountUses(VNode node)
    {
        switch (node)
        {
            case VUnary unary:
                AddUse(unary.Operand);
                break;

            case VBinary binary:
                AddUse(binary.Left);
                AddUse(binary.Right);
                break;

            case VCompare compare:
                AddUse(compare.Left);
                AddUse(compare.Right);
                break;

            case VConvert convert:
                AddUse(convert.Value);
                break;

            case VPhi phi:
                // Inputs counted after blocks are wired.
                break;

            case VStructPack pack:
                foreach (var field in pack.Fields)
                    AddUse(field);
                break;

            case VStructGet get:
                AddUse(get.Object);
                break;

            case VStructSet set:
                AddUse(set.Object);
                AddUse(set.Value);
                break;

            case VArrayNew arrayNew:
                AddUse(arrayNew.Length);
                break;

            case VArrayLen arrayLen:
                AddUse(arrayLen.Array);
                break;

            case VArrayGet arrayGet:
                AddUse(arrayGet.Array);
                AddUse(arrayGet.Index);
                break;

            case VArraySet arraySet:
                AddUse(arraySet.Array);
                AddUse(arraySet.Index);
                AddUse(arraySet.Value);
                break;

            case VMapGet mapGet:
                AddUse(mapGet.Map);
                AddUse(mapGet.Key);
                break;

            case VMapSet mapSet:
                AddUse(mapSet.Map);
                AddUse(mapSet.Key);
                AddUse(mapSet.Value);
                break;

            case VMapDelete mapDelete:
                AddUse(mapDelete.Map);
                AddUse(mapDelete.Key);
                break;

            case VMapHas mapHas:
                AddUse(mapHas.Map);
                AddUse(mapHas.Key);
                break;

            case VMapLen mapLen:
                AddUse(mapLen.Map);
                break;

            case VConcat concat:
                AddUse(concat.Left);
                AddUse(concat.Right);
                break;

            case VSlice slice:
                AddUse(slice.Value);
                AddUse(slice.Start);
                AddUse(slice.Length);
                break;

            case VBufferNew bufferNew:
                AddUse(bufferNew.Length);
                break;

            case VBufferSet bufferSet:
                AddUse(bufferSet.Buffer);
                AddUse(bufferSet.Index);
                AddUse(bufferSet.Value);
                break;

            case VBufferCopy bufferCopy:
                AddUse(bufferCopy.Destination);
                AddUse(bufferCopy.Source);
                AddUse(bufferCopy.DestinationOffset);
                AddUse(bufferCopy.SourceOffset);
                AddUse(bufferCopy.Length);
                break;

            case VStaticStore staticStore:
                AddUse(staticStore.Value);
                break;

            case VLoadLocal:
                break;

            case VStoreLocal storeLocal:
                AddUse(storeLocal.Value);
                break;

            case VSyscall syscall:
                foreach (var arg in syscall.Arguments)
                    AddUse(arg);
                break;

            case VCall call:
                foreach (var arg in call.Arguments)
                    AddUse(arg);
                break;

            case VPointerCall pointerCall:
                if (pointerCall.Pointer is not null)
                    AddUse(pointerCall.Pointer);
                foreach (var arg in pointerCall.Arguments)
                    AddUse(arg);
                break;

            case VTry:
            case VFinally:
                break;

            case VGetItem getItem:
                AddUse(getItem.Object);
                AddUse(getItem.KeyOrIndex);
                break;

            case VSetItem setItem:
                AddUse(setItem.Object);
                AddUse(setItem.KeyOrIndex);
                AddUse(setItem.Value);
                break;

            case VGuardNull guardNull:
                AddUse(guardNull.Reference);
                break;

            case VGuardBounds guardBounds:
                AddUse(guardBounds.Index);
                AddUse(guardBounds.Length);
                break;

            case VCheckedArithmetic checkedArithmetic:
                AddUse(checkedArithmetic.Left);
                AddUse(checkedArithmetic.Right);
                break;

            case VModMul modMul:
                AddUse(modMul.Left);
                AddUse(modMul.Right);
                AddUse(modMul.Modulus);
                break;

            case VModPow modPow:
                AddUse(modPow.Value);
                AddUse(modPow.Exponent);
                AddUse(modPow.Modulus);
                break;
        }
    }

    private void AddUse(VNode? node)
    {
        if (node is null)
            return;

        if (_useCounts.TryGetValue(node, out var count))
            _useCounts[node] = count + 1;
        else
            _useCounts[node] = 1;
    }

    private int GetRemainingUses(VNode node)
    {
        return _remainingUses.TryGetValue(node, out var count) ? count : 0;
    }

    private void ConsumeNode(VNode node)
    {
        if (_remainingUses.TryGetValue(node, out var count) && count > 0)
            _remainingUses[node] = count - 1;
    }

    private void ReserveTemporaryUse(VNode node)
    {
        if (_remainingUses.TryGetValue(node, out var count))
            _remainingUses[node] = count + 1;
    }

    #endregion

    #region Sethi-Ullman analysis

    private void ComputeSethiNumbers(VFunction function)
    {
        _sethiNumbers.Clear();
        _sethiVisiting.Clear();

        foreach (var block in function.Blocks)
        {
            foreach (var node in block.Nodes)
                GetSethiNumber(node);

            switch (block.Terminator)
            {
                case VRet ret when ret.Value is not null:
                    GetSethiNumber(ret.Value);
                    break;
                case VJmpIf jmpIf:
                    GetSethiNumber(jmpIf.Condition);
                    break;
                case VSwitch vSwitch:
                    GetSethiNumber(vSwitch.Key);
                    break;
                case VCompareBranch cmpBranch:
                    GetSethiNumber(cmpBranch.Left);
                    GetSethiNumber(cmpBranch.Right);
                    break;
            }
        }
    }

    private int GetSethiNumber(VNode node)
    {
        if (_sethiNumbers.TryGetValue(node, out var number))
            return number;

        if (!_sethiVisiting.Add(node))
            return 1;

        int result;

        if (node is VPhi phi)
        {
            var max = 1;
            foreach (var (_, value) in phi.Inputs)
                max = Math.Max(max, GetSethiNumber(value));
            result = max;
        }
        else
        {
            var operands = EnumerateOperands(node);
            if (operands.Count == 0)
            {
                result = 1;
            }
            else if (operands.Count == 1)
            {
                result = Math.Max(1, GetSethiNumber(operands[0]));
            }
            else if (operands.Count == 2)
            {
                var left = GetSethiNumber(operands[0]);
                var right = GetSethiNumber(operands[1]);
                result = left == right ? left + 1 : Math.Max(left, right);
            }
            else
            {
                var costs = new List<int>(operands.Count);
                foreach (var operand in operands)
                    costs.Add(GetSethiNumber(operand));
                costs.Sort();

                var max = costs[^1];
                var remaining = costs.Count - 1;
                for (int i = remaining - 1; i >= 0; i--)
                    max = Math.Max(max, costs[i] + (remaining - i));
                result = max;
            }
        }

        _sethiVisiting.Remove(node);
        _sethiNumbers[node] = result;
        return result;
    }

    private static List<VNode> EnumerateOperands(VNode node)
    {
        var operands = new List<VNode>();

        switch (node)
        {
            case VUnary unary:
                operands.Add(unary.Operand);
                break;

            case VBinary binary:
                operands.Add(binary.Left);
                operands.Add(binary.Right);
                break;

            case VCompare compare:
                operands.Add(compare.Left);
                operands.Add(compare.Right);
                break;

            case VConvert convert:
                operands.Add(convert.Value);
                break;

            case VStructPack structPack:
                operands.AddRange(structPack.Fields);
                break;

            case VStructGet structGet:
                operands.Add(structGet.Object);
                break;

            case VStructSet structSet:
                operands.Add(structSet.Object);
                operands.Add(structSet.Value);
                break;

            case VArrayNew arrayNew:
                operands.Add(arrayNew.Length);
                break;

            case VArrayGet arrayGet:
                operands.Add(arrayGet.Array);
                operands.Add(arrayGet.Index);
                break;

            case VArraySet arraySet:
                operands.Add(arraySet.Array);
                operands.Add(arraySet.Index);
                operands.Add(arraySet.Value);
                break;

            case VMapGet mapGet:
                operands.Add(mapGet.Map);
                operands.Add(mapGet.Key);
                break;

            case VMapSet mapSet:
                operands.Add(mapSet.Map);
                operands.Add(mapSet.Key);
                operands.Add(mapSet.Value);
                break;

            case VMapDelete mapDelete:
                operands.Add(mapDelete.Map);
                operands.Add(mapDelete.Key);
                break;

            case VMapHas mapHas:
                operands.Add(mapHas.Map);
                operands.Add(mapHas.Key);
                break;

            case VMapLen mapLen:
                operands.Add(mapLen.Map);
                break;

            case VConcat concat:
                operands.Add(concat.Left);
                operands.Add(concat.Right);
                break;

            case VSlice slice:
                operands.Add(slice.Value);
                operands.Add(slice.Start);
                operands.Add(slice.Length);
                break;

            case VBufferNew bufferNew:
                operands.Add(bufferNew.Length);
                break;

            case VBufferSet bufferSet:
                operands.Add(bufferSet.Buffer);
                operands.Add(bufferSet.Index);
                operands.Add(bufferSet.Value);
                break;

            case VBufferCopy bufferCopy:
                operands.Add(bufferCopy.Destination);
                operands.Add(bufferCopy.Source);
                operands.Add(bufferCopy.DestinationOffset);
                operands.Add(bufferCopy.SourceOffset);
                operands.Add(bufferCopy.Length);
                break;

            case VStaticStore staticStore:
                operands.Add(staticStore.Value);
                break;

            case VLoadLocal:
                break;

            case VStoreLocal storeLocal:
                operands.Add(storeLocal.Value);
                break;

            case VSyscall syscall:
                operands.AddRange(syscall.Arguments);
                break;

            case VCall call:
                operands.AddRange(call.Arguments);
                break;

            case VPointerCall pointerCall:
                if (pointerCall.Pointer is not null)
                    operands.Add(pointerCall.Pointer);
                operands.AddRange(pointerCall.Arguments);
                break;

            case VGetItem getItem:
                operands.Add(getItem.Object);
                operands.Add(getItem.KeyOrIndex);
                break;

            case VSetItem setItem:
                operands.Add(setItem.Object);
                operands.Add(setItem.KeyOrIndex);
                operands.Add(setItem.Value);
                break;

            case VGuardNull guardNull:
                operands.Add(guardNull.Reference);
                break;

            case VGuardBounds guardBounds:
                operands.Add(guardBounds.Index);
                operands.Add(guardBounds.Length);
                break;

            case VCheckedArithmetic checkedArithmetic:
                operands.Add(checkedArithmetic.Left);
                operands.Add(checkedArithmetic.Right);
                break;

            case VModMul modMul:
                operands.Add(modMul.Left);
                operands.Add(modMul.Right);
                operands.Add(modMul.Modulus);
                break;

            case VModPow modPow:
                operands.Add(modPow.Value);
                operands.Add(modPow.Exponent);
                operands.Add(modPow.Modulus);
                break;
        }

        return operands;
    }

    private int GetSethiNumberOrDefault(VNode node)
    {
        return _sethiNumbers.TryGetValue(node, out var value) ? value : 1;
    }

    private static bool IsCommutative(VBinaryOp op) => op switch
    {
        VBinaryOp.Add => true,
        VBinaryOp.Mul => true,
        VBinaryOp.And => true,
        VBinaryOp.Or => true,
        VBinaryOp.Xor => true,
        VBinaryOp.Max => true,
        VBinaryOp.Min => true,
        _ => false
    };

    private static bool IsCommutative(VCompare compare) => compare.Op is VCompareOp.Eq or VCompareOp.Ne;

    private static bool IsCommutative(VCheckedOp op) => op is VCheckedOp.Add or VCheckedOp.Mul;

    #endregion

    #region Scheduling

    private void ScheduleNode(VNode node, LirBlock block)
    {
        if (_emittedNodes.Contains(node))
            return;

        switch (node)
        {
            case VConstInt constInt:
                PushInt(block, constInt.Value, constInt.Span, constInt, markImmediate: false);
                DropIfDead(block, constInt);
                break;

            case VConstBool constBool:
                PushBool(block, constBool.Value, constBool.Span, constBool);
                DropIfDead(block, constBool);
                break;

            case VConstByteString constBytes:
                PushByteString(block, constBytes.Value, constBytes.Span, constBytes);
                DropIfDead(block, constBytes);
                break;

            case VConstBuffer constBuffer:
                PushBuffer(block, constBuffer);
                DropIfDead(block, constBuffer);
                break;

            case VConstNull constNull:
                PushNull(block, constNull.Span, constNull);
                DropIfDead(block, constNull);
                break;

            case VParam param:
                PushPlaceholder(param);
                DropIfDead(block, param);
                break;

            case VPhi phi:
                HandlePhi(block, phi);
                break;

            case VUnary unary:
                EmitUnary(block, unary);
                break;

            case VBinary binary:
                EmitBinary(block, binary);
                break;

            case VCompare compare:
                EmitCompare(block, compare);
                break;

            case VConvert convert:
                EmitConvert(block, convert);
                break;

            case VStructPack structPack:
                EmitStructPack(block, structPack);
                break;

            case VStructGet structGet:
                EmitStructGet(block, structGet);
                break;

            case VStructSet structSet:
                EmitStructSet(block, structSet);
                break;

            case VArrayNew arrayNew:
                EmitArrayNew(block, arrayNew);
                break;

            case VArrayLen arrayLen:
                EmitArrayLen(block, arrayLen);
                break;

            case VArrayGet arrayGet:
                EmitArrayGet(block, arrayGet);
                break;

            case VArraySet arraySet:
                EmitArraySet(block, arraySet);
                break;

            case VMapNew mapNew:
                EmitMapNew(block, mapNew);
                break;

            case VMapGet mapGet:
                EmitMapGet(block, mapGet);
                break;

            case VMapSet mapSet:
                EmitMapSet(block, mapSet);
                break;

            case VMapDelete mapDelete:
                EmitMapDelete(block, mapDelete);
                break;

            case VMapHas mapHas:
                EmitMapHas(block, mapHas);
                break;

            case VMapLen mapLen:
                EmitMapLen(block, mapLen);
                break;

            case VConcat concat:
                EmitConcat(block, concat);
                break;

            case VSlice slice:
                EmitSlice(block, slice);
                break;

            case VBufferNew bufferNew:
                EmitBufferNew(block, bufferNew);
                break;

            case VBufferSet bufferSet:
                EmitBufferSet(block, bufferSet);
                break;

            case VBufferCopy bufferCopy:
                EmitBufferCopy(block, bufferCopy);
                break;

            case VModMul modMul:
                EmitModMul(block, modMul);
                break;

            case VModPow modPow:
                EmitModPow(block, modPow);
                break;

            case VStaticLoad staticLoad:
                EmitStaticLoad(block, staticLoad);
                break;

            case VStaticStore staticStore:
                EmitStaticStore(block, staticStore);
                break;

            case VLoadLocal loadLocal:
                EmitLoadLocal(block, loadLocal);
                break;

            case VStoreLocal storeLocal:
                EmitStoreLocal(block, storeLocal);
                break;

            case VSyscall syscall:
                EmitSyscall(block, syscall);
                break;

            case VCall call:
                EmitCall(block, call);
                break;

            case VPointerCall pointerCall:
                EmitPointerCall(block, pointerCall);
                break;

            case VTry vTry:
                EmitTry(block, vTry);
                break;

            case VFinally vFinally:
                if (!_tryScopeInfo.ContainsKey(vFinally.Scope))
                    throw new NotSupportedException("Encountered VFinally without associated try scope.");
                break;

            case VCatch vCatch:
                if (!_tryScopeInfo.ContainsKey(vCatch.Scope))
                    throw new NotSupportedException("Encountered VCatch without associated try scope.");
                break;

            case VGetItem getItem:
                EmitGetItem(block, getItem);
                break;

            case VSetItem setItem:
                EmitSetItem(block, setItem);
                break;

            case VGuardNull guardNull:
                EmitGuardNull(block, guardNull);
                break;

            case VGuardBounds guardBounds:
                EmitGuardBounds(block, guardBounds);
                break;

            case VCheckedArithmetic checkedArithmetic:
                EmitCheckedArithmetic(block, checkedArithmetic);
                break;

            default:
                throw new NotSupportedException($"Stack scheduler does not yet support node '{node.GetType().Name}'.");
        }

        _emittedNodes.Add(node);
    }

    private void ScheduleTerminator(VBlock source, LirBlock target)
    {
        switch (source.Terminator)
        {
            case VRet ret:
                EmitReturn(target, ret);
                break;

            case VJmp jmp:
                AlignSuccessorStack(source, jmp.Target, target, source.Terminator?.Span);
                EmitJump(target, LirOpcode.JMP, jmp.Target.Label, source.Terminator?.Span);
                _stack.Clear();
                break;

            case VJmpIf jmpIf:
                EmitJumpIf(source, target, jmpIf);
                break;

            case VCompareBranch cmpBranch:
                EmitCompareBranch(source, target, cmpBranch);
                break;

            case VAbort:
                EmitInstruction(target, new LirInst(LirOpcode.ABORT) { Span = source.Terminator?.Span });
                break;

            case VAbortMsg abortMsg:
                {
                    var reservations = BeginReservations();
                    PrepareOperand(target, abortMsg.Message, source.Terminator?.Span, forceDuplicate: false, reservations);
                    EmitInstruction(target, new LirInst(LirOpcode.ABORTMSG) { Span = source.Terminator?.Span });
                    break;
                }

            case VSwitch vSwitch:
                EmitSwitch(source, target, vSwitch);
                break;

            case VUnreachable:
                EmitInstruction(target, new LirInst(LirOpcode.ABORT) { Span = source.Terminator?.Span });
                break;

            case VLeave leave:
                EmitEndTry(target, leave);
                _stack.Clear();
                break;

            case VEndFinally endFinally:
                EmitEndFinally(target, endFinally);
                _stack.Clear();
                break;

            case null:
                throw new InvalidOperationException($"Block '{source.Label}' is missing a terminator.");

            default:
                throw new NotSupportedException($"Unsupported terminator '{source.Terminator.GetType().Name}'.");
        }
    }

    #endregion

    #region Node emitters

    private void EmitUnary(LirBlock block, VUnary unary)
    {
        var opcode = unary.Op switch
        {
            VUnaryOp.Negate => LirOpcode.NEG,
            VUnaryOp.Not => LirOpcode.NOT,
            VUnaryOp.Abs => LirOpcode.ABS,
            VUnaryOp.Sign => LirOpcode.SIGN,
            VUnaryOp.Inc => LirOpcode.INC,
            VUnaryOp.Dec => LirOpcode.DEC,
            VUnaryOp.Sqrt => LirOpcode.SQRT,
            _ => throw new NotSupportedException($"Unary op '{unary.Op}' is not supported.")
        };

        PrepareSingleOperand(block, unary.Operand, unary.Span);
        EmitInstruction(block, new LirInst(opcode) { Span = unary.Span });
        TagResult(unary);
        DropIfDead(block, unary);
    }

    private void EmitBinary(LirBlock block, VBinary binary)
    {
        var opcode = binary.Op switch
        {
            VBinaryOp.Add => LirOpcode.ADD,
            VBinaryOp.Sub => LirOpcode.SUB,
            VBinaryOp.Mul => LirOpcode.MUL,
            VBinaryOp.Div => LirOpcode.DIV,
            VBinaryOp.Mod => LirOpcode.MOD,
            VBinaryOp.And => LirOpcode.AND,
            VBinaryOp.Or => LirOpcode.OR,
            VBinaryOp.Xor => LirOpcode.XOR,
            VBinaryOp.Shl => LirOpcode.SHL,
            VBinaryOp.Shr => LirOpcode.SHR,
            VBinaryOp.Cat => LirOpcode.CAT,
            VBinaryOp.Max => LirOpcode.MAX,
            VBinaryOp.Min => LirOpcode.MIN,
            VBinaryOp.Pow => LirOpcode.POW,
            _ => throw new NotSupportedException($"Binary op '{binary.Op}' is not supported.")
        };

        var left = binary.Left;
        var right = binary.Right;
        if (IsCommutative(binary.Op) && GetSethiNumberOrDefault(left) < GetSethiNumberOrDefault(right))
            (left, right) = (right, left);

        PrepareOperands(block, binary.Span, (left, false), (right, false));
        EmitInstruction(block, new LirInst(opcode) { Span = binary.Span });
        TagResult(binary);
        DropIfDead(block, binary);
    }

    private void EmitCheckedArithmetic(LirBlock block, VCheckedArithmetic node)
    {
        var opcode = node.Op switch
        {
            VCheckedOp.Add => LirOpcode.ADD,
            VCheckedOp.Sub => LirOpcode.SUB,
            VCheckedOp.Mul => LirOpcode.MUL,
            _ => throw new NotSupportedException($"Checked op '{node.Op}' is not supported.")
        };

        var left = node.Left;
        var right = node.Right;
        if (IsCommutative(node.Op) && GetSethiNumberOrDefault(left) < GetSethiNumberOrDefault(right))
            (left, right) = (right, left);

        PrepareOperands(block, node.Span, (left, false), (right, false));
        EmitInstruction(block, new LirInst(opcode) { Span = node.Span });
        TagResult(node);
        if (!TryGetIntBounds(node.Type, out var minInclusive, out var maxExclusive))
        {
            DropIfDead(block, node);
            return;
        }

        ReserveTemporaryUse(node);
        EmitDup(block, node.Span);
        PushInt(block, minInclusive, node.Span, producedNode: null, markImmediate: true);
        PushInt(block, maxExclusive, node.Span, producedNode: null, markImmediate: true);
        EmitInstruction(block, new LirInst(LirOpcode.WITHIN) { Span = node.Span });

        if (node.FailKind == VGuardFailKind.Branch)
        {
            if (node.FailTarget is null)
                throw new InvalidOperationException("Checked arithmetic with branch failure requires a fail target.");

            var currentBlock = _currentBlock ?? throw new InvalidOperationException("Checked arithmetic lowering requires active block context.");
            AlignSuccessorStack(currentBlock, node.FailTarget, block, node.Span, reservedTop: 1);
            EmitJump(block, LirOpcode.JMPIFNOT, node.FailTarget.Label, node.Span);
        }
        else
        {
            EmitInstruction(block, new LirInst(LirOpcode.NOT) { Span = node.Span });
            EmitInstruction(block, new LirInst(LirOpcode.ASSERT) { Span = node.Span });
        }

        DropIfDead(block, node);
    }

    private void EmitCompare(LirBlock block, VCompare compare)
    {
        var opcode = compare.Op switch
        {
            VCompareOp.Eq => LirOpcode.NUMEQUAL,
            VCompareOp.Ne => LirOpcode.NUMNOTEQUAL,
            VCompareOp.Lt => LirOpcode.LT,
            VCompareOp.Le => LirOpcode.LTE,
            VCompareOp.Gt => LirOpcode.GT,
            VCompareOp.Ge => LirOpcode.GTE,
            _ => throw new NotSupportedException($"Compare op '{compare.Op}' is not supported.")
        };

        var left = compare.Left;
        var right = compare.Right;
        if (IsCommutative(compare))
        {
            var leftScore = GetSethiNumberOrDefault(left);
            var rightScore = GetSethiNumberOrDefault(right);
            if (rightScore > leftScore)
                (left, right) = (right, left);
        }

        PrepareOperands(block, compare.Span, (left, false), (right, false));
        EmitInstruction(block, new LirInst(opcode) { Span = compare.Span });
        TagResult(compare);
        DropIfDead(block, compare);
    }

    private void EmitConvert(LirBlock block, VConvert convert)
    {
        PrepareSingleOperand(block, convert.Value, convert.Span);

        switch (convert.Op)
        {
            case VConvertOp.ToBool:
                ConsumeNode(convert.Value);
                PushImmediateInt(block, convert.Span, 0);
                EmitInstruction(block, new LirInst(LirOpcode.NUMNOTEQUAL) { Span = convert.Span });
                TagResult(convert);
                DropIfDead(block, convert);
                break;

            case VConvertOp.SignExtend:
            case VConvertOp.ZeroExtend:
            case VConvertOp.Narrow:
                ConsumeNode(convert.Value);
                TagResult(convert);
                DropIfDead(block, convert);
                break;

            case VConvertOp.ToByteString:
                {
                    ConsumeNode(convert.Value);
                    var inst = new LirInst(LirOpcode.CONVERT)
                    {
                        Span = convert.Span,
                        Immediate = new[] { (byte)Neo.VM.Types.StackItemType.ByteString }
                    };
                    EmitInstruction(block, inst);
                    TagResult(convert);
                    DropIfDead(block, convert);
                    break;
                }

            case VConvertOp.ToBuffer:
                {
                    ConsumeNode(convert.Value);
                    var inst = new LirInst(LirOpcode.CONVERT)
                    {
                        Span = convert.Span,
                        Immediate = new[] { (byte)Neo.VM.Types.StackItemType.Buffer }
                    };
                    EmitInstruction(block, inst);
                    TagResult(convert);
                    DropIfDead(block, convert);
                    break;
                }

            default:
                throw new NotSupportedException($"Convert op '{convert.Op}' is not supported.");
        }
    }

    private void EmitStructPack(LirBlock block, VStructPack pack)
    {
        var reservations = BeginReservations();

        foreach (var field in pack.Fields)
            PrepareOperand(block, field, pack.Span, forceDuplicate: false, reservations);

        PushImmediateInt(block, pack.Span, pack.Fields.Count);

        var inst = new LirInst(LirOpcode.PACKSTRUCT)
        {
            Span = pack.Span,
            PopOverride = pack.Fields.Count + 1,
            PushOverride = 1
        };

        EmitInstruction(block, inst);
        TagResult(pack);
        DropIfDead(block, pack);
    }

    private void EmitStructGet(LirBlock block, VStructGet get)
    {
        PrepareSingleOperand(block, get.Object, get.Span);
        PushImmediateInt(block, get.Span, get.Index);
        EmitInstruction(block, new LirInst(LirOpcode.GETITEM) { Span = get.Span });
        TagResult(get);
        DropIfDead(block, get);
    }

    private void EmitStructSet(LirBlock block, VStructSet set)
    {
        var reservations = BeginReservations();
        PrepareOperand(block, set.Object, set.Span, forceDuplicate: true, reservations);
        PushImmediateInt(block, set.Span, set.Index);
        PrepareOperand(block, set.Value, set.Span, forceDuplicate: false, reservations);
        EmitInstruction(block, new LirInst(LirOpcode.SETITEM)
        {
            Span = set.Span,
            PopOverride = 3,
            PushOverride = 0
        });

        TagResult(set);
        DropIfDead(block, set);
    }

    private void EmitArrayNew(LirBlock block, VArrayNew arrayNew)
    {
        PrepareSingleOperand(block, arrayNew.Length, arrayNew.Span);
        EmitInstruction(block, new LirInst(LirOpcode.NEWARRAY) { Span = arrayNew.Span });
        TagResult(arrayNew);
        DropIfDead(block, arrayNew);
    }

    private void EmitArrayLen(LirBlock block, VArrayLen arrayLen)
    {
        PrepareSingleOperand(block, arrayLen.Array, arrayLen.Span);
        EmitInstruction(block, new LirInst(LirOpcode.LENGTH) { Span = arrayLen.Span });
        TagResult(arrayLen);
        DropIfDead(block, arrayLen);
    }

    private void EmitArrayGet(LirBlock block, VArrayGet arrayGet)
    {
        PrepareOperands(block, arrayGet.Span, (arrayGet.Array, false), (arrayGet.Index, false));
        EmitInstruction(block, new LirInst(LirOpcode.GETITEM) { Span = arrayGet.Span });
        TagResult(arrayGet);
        DropIfDead(block, arrayGet);
    }

    private void EmitArraySet(LirBlock block, VArraySet arraySet)
    {
        var reservations = BeginReservations();
        PrepareOperand(block, arraySet.Array, arraySet.Span, forceDuplicate: false, reservations);
        EmitDup(block, arraySet.Span);
        PrepareOperand(block, arraySet.Index, arraySet.Span, forceDuplicate: false, reservations);
        PrepareOperand(block, arraySet.Value, arraySet.Span, forceDuplicate: false, reservations);

        EmitInstruction(block, new LirInst(LirOpcode.SETITEM)
        {
            Span = arraySet.Span,
            PopOverride = 3,
            PushOverride = 0
        });
    }

    private void EmitMapNew(LirBlock block, VMapNew mapNew)
    {
        EmitInstruction(block, new LirInst(LirOpcode.NEWMAP) { Span = mapNew.Span });
        TagResult(mapNew);
        DropIfDead(block, mapNew);
    }

    private void EmitMapGet(LirBlock block, VMapGet mapGet)
    {
        PrepareOperands(block, mapGet.Span, (mapGet.Map, false), (mapGet.Key, false));
        EmitInstruction(block, new LirInst(LirOpcode.GETITEM) { Span = mapGet.Span });
        TagResult(mapGet);
        DropIfDead(block, mapGet);
    }

    private void EmitMapSet(LirBlock block, VMapSet mapSet)
    {
        var reservations = BeginReservations();
        PrepareOperand(block, mapSet.Map, mapSet.Span, forceDuplicate: true, reservations);
        PrepareOperand(block, mapSet.Key, mapSet.Span, forceDuplicate: false, reservations);
        PrepareOperand(block, mapSet.Value, mapSet.Span, forceDuplicate: false, reservations);

        EmitInstruction(block, new LirInst(LirOpcode.SETITEM)
        {
            Span = mapSet.Span,
            PopOverride = 3,
            PushOverride = 0
        });
    }

    private void EmitMapDelete(LirBlock block, VMapDelete mapDelete)
    {
        var reservations = BeginReservations();
        PrepareOperand(block, mapDelete.Map, mapDelete.Span, forceDuplicate: true, reservations);
        PrepareOperand(block, mapDelete.Key, mapDelete.Span, forceDuplicate: false, reservations);

        EmitInstruction(block, new LirInst(LirOpcode.REMOVE) { Span = mapDelete.Span });
    }

    private void EmitMapHas(LirBlock block, VMapHas mapHas)
    {
        PrepareOperands(block, mapHas.Span, (mapHas.Map, false), (mapHas.Key, false));
        EmitInstruction(block, new LirInst(LirOpcode.HASKEY) { Span = mapHas.Span });
        TagResult(mapHas);
        DropIfDead(block, mapHas);
    }

    private void EmitMapLen(LirBlock block, VMapLen mapLen)
    {
        PrepareSingleOperand(block, mapLen.Map, mapLen.Span);
        EmitInstruction(block, new LirInst(LirOpcode.LENGTH) { Span = mapLen.Span });
        TagResult(mapLen);
        DropIfDead(block, mapLen);
    }

    private void EmitConcat(LirBlock block, VConcat concat)
    {
        PrepareOperands(block, concat.Span, (concat.Left, false), (concat.Right, false));
        EmitInstruction(block, new LirInst(LirOpcode.CAT) { Span = concat.Span });
        TagResult(concat);
        DropIfDead(block, concat);
    }

    private void EmitSlice(LirBlock block, VSlice slice)
    {
        PrepareOperands(block, slice.Span, (slice.Value, false), (slice.Start, false), (slice.Length, false));
        EmitInstruction(block, new LirInst(LirOpcode.SUBSTR) { Span = slice.Span });
        TagResult(slice);
        DropIfDead(block, slice);
    }

    private void EmitBufferNew(LirBlock block, VBufferNew bufferNew)
    {
        PrepareSingleOperand(block, bufferNew.Length, bufferNew.Span);
        EmitInstruction(block, new LirInst(LirOpcode.NEWBUFFER) { Span = bufferNew.Span });
        TagResult(bufferNew);
        DropIfDead(block, bufferNew);
    }

    private void EmitBufferSet(LirBlock block, VBufferSet bufferSet)
    {
        var reservations = BeginReservations();
        PrepareOperand(block, bufferSet.Buffer, bufferSet.Span, forceDuplicate: true, reservations);
        PrepareOperand(block, bufferSet.Index, bufferSet.Span, forceDuplicate: false, reservations);
        PrepareOperand(block, bufferSet.Value, bufferSet.Span, forceDuplicate: false, reservations);

        EmitInstruction(block, new LirInst(LirOpcode.SETITEM)
        {
            Span = bufferSet.Span,
            PopOverride = 3,
            PushOverride = 0
        });

        TagResult(bufferSet);
        DropIfDead(block, bufferSet);
    }

    private void EmitBufferCopy(LirBlock block, VBufferCopy bufferCopy)
    {
        var reservations = BeginReservations();
        PrepareOperand(block, bufferCopy.Destination, bufferCopy.Span, forceDuplicate: true, reservations);
        PrepareOperand(block, bufferCopy.DestinationOffset, bufferCopy.Span, forceDuplicate: false, reservations);
        PrepareOperand(block, bufferCopy.Source, bufferCopy.Span, forceDuplicate: false, reservations);
        PrepareOperand(block, bufferCopy.SourceOffset, bufferCopy.Span, forceDuplicate: false, reservations);
        PrepareOperand(block, bufferCopy.Length, bufferCopy.Span, forceDuplicate: false, reservations);

        EmitInstruction(block, new LirInst(LirOpcode.MEMCPY)
        {
            Span = bufferCopy.Span,
            PopOverride = 5,
            PushOverride = 0
        });

        if (_stack.Count == 0)
            throw new InvalidOperationException("Expected destination buffer to remain on stack after MEMCPY.");

        TagResult(bufferCopy);
        DropIfDead(block, bufferCopy);
    }

    private void EmitStaticLoad(LirBlock block, VStaticLoad load)
    {
        var inst = new LirInst(LirOpcode.LDSFLD)
        {
            Span = load.Span,
            Immediate = new[] { load.Slot }
        };

        EmitInstruction(block, inst);
        TagResult(load);
        DropIfDead(block, load);
    }

    private void EmitStaticStore(LirBlock block, VStaticStore store)
    {
        var reservations = BeginReservations();
        PrepareOperand(block, store.Value, store.Span, forceDuplicate: false, reservations);

        var inst = new LirInst(LirOpcode.STSFLD)
        {
            Span = store.Span,
            Immediate = new[] { store.Slot }
        };

        EmitInstruction(block, inst);
    }

    private void EmitLoadLocal(LirBlock block, VLoadLocal load)
    {
        var inst = new LirInst(LirOpcode.LDLOC)
        {
            Span = load.Span,
            Immediate = new[] { unchecked((byte)load.Slot) }
        };

        EmitInstruction(block, inst);
        TagResult(load);
        DropIfDead(block, load);
    }

    private void EmitStoreLocal(LirBlock block, VStoreLocal store)
    {
        var reservations = BeginReservations();
        PrepareOperand(block, store.Value, store.Span, forceDuplicate: false, reservations);

        var inst = new LirInst(LirOpcode.STLOC)
        {
            Span = store.Span,
            Immediate = new[] { unchecked((byte)store.Slot) }
        };

        EmitInstruction(block, inst);
    }

    private void EmitSyscall(LirBlock block, VSyscall syscall)
    {
        var reservations = BeginReservations();
        foreach (var arg in syscall.Arguments)
            PrepareOperand(block, arg, syscall.Span, forceDuplicate: false, reservations);

        var inst = new LirInst(LirOpcode.SYSCALL)
        {
            Span = syscall.Span,
            Immediate = BitConverter.GetBytes(syscall.SysId),
            PopOverride = syscall.Arguments.Count,
            PushOverride = syscall.Type is LirVoidType ? 0 : 1
        };

        EmitInstruction(block, inst);

        if (syscall.Type is not LirVoidType)
        {
            TagResult(syscall);
            DropIfDead(block, syscall);
        }
    }

    private void EmitCall(LirBlock block, VCall call)
    {
        var reservations = BeginReservations();
        foreach (var arg in call.Arguments)
            PrepareOperand(block, arg, call.Span, forceDuplicate: false, reservations);

        var inst = new LirInst(LirOpcode.CALL)
        {
            Span = call.Span,
            TargetLabel = call.Callee,
            PopOverride = call.Arguments.Count,
            PushOverride = call.Type is LirVoidType ? 0 : 1
        };

        EmitInstruction(block, inst);

        if (call.Type is not LirVoidType)
        {
            TagResult(call);
            DropIfDead(block, call);
        }
    }

    private void EmitPointerCall(LirBlock block, VPointerCall call)
    {
        var reservations = BeginReservations();
        foreach (var arg in call.Arguments)
            PrepareOperand(block, arg, call.Span, forceDuplicate: false, reservations);

        if (call.Pointer is not null)
            PrepareOperand(block, call.Pointer, call.Span, forceDuplicate: false, reservations);

        var opcode = call.IsTailCall ? LirOpcode.CALLT : LirOpcode.CALLA;

        var immediate = call.IsTailCall && call.CallTableIndex.HasValue
            ? new[] { call.CallTableIndex.Value }
            : null;

        var popCount = call.Arguments.Count + (call.Pointer is null ? 0 : 1);

        var inst = new LirInst(opcode)
        {
            Span = call.Span,
            PopOverride = popCount,
            PushOverride = call.Type is LirVoidType ? 0 : 1,
            Immediate = immediate
        };

        EmitInstruction(block, inst);

        if (call.Type is not LirVoidType)
        {
            TagResult(call);
            DropIfDead(block, call);
        }
    }

    private void EmitTry(LirBlock block, VTry scope)
    {
        if (!_tryScopeInfo.ContainsKey(scope))
            _tryScopeInfo[scope] = new TryScopeInfo(scope.TryBlock, scope.FinallyBlock, scope.MergeBlock, scope.CatchBlocks);

        var inst = new LirInst(LirOpcode.TRY_L)
        {
            Span = scope.Span,
            TargetLabel = scope.CatchBlocks.Count > 0 ? scope.CatchBlocks[0].Label : null,
            TargetLabel2 = scope.FinallyBlock.Label
        };

        EmitInstruction(block, inst);
    }

    private void EmitEndTry(LirBlock block, VLeave leave)
    {
        if (!_tryScopeInfo.ContainsKey(leave.Scope))
            throw new NotSupportedException("Encountered VLeave without registered try scope.");

        if (_currentBlock is null)
            throw new InvalidOperationException("TRY emission requires current block context.");

        AlignSuccessorStack(_currentBlock, leave.Target, block, leave.Span);

        var inst = new LirInst(LirOpcode.ENDTRY_L)
        {
            Span = leave.Span,
            TargetLabel = leave.Target.Label
        };

        EmitInstruction(block, inst);
    }

    private void EmitEndFinally(LirBlock block, VEndFinally endFinally)
    {
        if (!_tryScopeInfo.ContainsKey(endFinally.Scope))
            throw new NotSupportedException("Encountered VEndFinally without registered try scope.");

        if (_currentBlock is null)
            throw new InvalidOperationException("ENDFINALLY emission requires current block context.");

        AlignSuccessorStack(_currentBlock, endFinally.Target, block, endFinally.Span);

        var inst = new LirInst(LirOpcode.ENDFINALLY)
        {
            Span = endFinally.Span
        };

        EmitInstruction(block, inst);
    }

    private void EmitGetItem(LirBlock block, VGetItem getItem)
    {
        PrepareOperands(block, getItem.Span, (getItem.Object, false), (getItem.KeyOrIndex, false));
        EmitInstruction(block, new LirInst(LirOpcode.GETITEM) { Span = getItem.Span });
        TagResult(getItem);
        DropIfDead(block, getItem);
    }

    private void EmitSetItem(LirBlock block, VSetItem setItem)
    {
        var reservations = BeginReservations();
        PrepareOperand(block, setItem.Object, setItem.Span, forceDuplicate: true, reservations);
        PrepareOperand(block, setItem.KeyOrIndex, setItem.Span, forceDuplicate: false, reservations);
        PrepareOperand(block, setItem.Value, setItem.Span, forceDuplicate: false, reservations);

        EmitInstruction(block, new LirInst(LirOpcode.SETITEM)
        {
            Span = setItem.Span,
            PopOverride = 3,
            PushOverride = 0
        });

        TagResult(setItem);
        DropIfDead(block, setItem);
    }

    private void EmitGuardNull(LirBlock block, VGuardNull guard)
    {
        var reservations = BeginReservations();
        PrepareOperand(block, guard.Reference, guard.Span, forceDuplicate: true, reservations);

        EmitInstruction(block, new LirInst(LirOpcode.ISNULL) { Span = guard.Span });

        if (guard.FailKind == VGuardFailKind.Branch && guard.FailTarget is not null)
        {
            var currentBlock = _currentBlock ?? throw new InvalidOperationException("Guard lowering requires active block context.");
            AlignSuccessorStack(currentBlock, guard.FailTarget, block, guard.Span, reservedTop: 1);
            EmitJump(block, LirOpcode.JMPIF, guard.FailTarget.Label, guard.Span);
        }
        else
        {
            EmitInstruction(block, new LirInst(LirOpcode.NOT) { Span = guard.Span });
            EmitInstruction(block, new LirInst(LirOpcode.ASSERT) { Span = guard.Span });
        }
    }

    private void EmitGuardBounds(LirBlock block, VGuardBounds guard)
    {
        var reservations = BeginReservations();

        PrepareOperand(block, guard.Index, guard.Span, forceDuplicate: true, reservations);
        PushInt(block, BigInteger.Zero, guard.Span, producedNode: null, markImmediate: true);
        PrepareOperand(block, guard.Length, guard.Span, forceDuplicate: true, reservations);

        EmitInstruction(block, new LirInst(LirOpcode.WITHIN) { Span = guard.Span });

        if (guard.FailKind == VGuardFailKind.Branch && guard.FailTarget is not null)
        {
            var currentBlock = _currentBlock ?? throw new InvalidOperationException("Guard lowering requires active block context.");
            AlignSuccessorStack(currentBlock, guard.FailTarget, block, guard.Span, reservedTop: 1);
            EmitJump(block, LirOpcode.JMPIFNOT, guard.FailTarget.Label, guard.Span);
        }
        else
        {
            EmitInstruction(block, new LirInst(LirOpcode.ASSERT) { Span = guard.Span });
        }
    }

    #endregion

    #region Terminators

    private void EmitReturn(LirBlock block, VRet ret)
    {
        if (ret.Value is not null)
        {
            PrepareSingleOperand(block, ret.Value, ret.Span);
            ConsumeNode(ret.Value);
        }

        EmitInstruction(block, new LirInst(LirOpcode.RET) { Span = ret.Span });
        _stack.Clear();
    }

    private void EmitJumpIf(VBlock currentBlock, LirBlock block, VJmpIf jmpIf)
    {
        PrepareSingleOperand(block, jmpIf.Condition, jmpIf.Span);

        AlignConditionalSuccessors(currentBlock, block, jmpIf.TrueTarget, jmpIf.FalseTarget, jmpIf.Span, reservedTop: 1);

        EmitJump(block, LirOpcode.JMPIF, jmpIf.TrueTarget.Label, jmpIf.Span);
        EmitJump(block, LirOpcode.JMP, jmpIf.FalseTarget.Label, jmpIf.Span);
        _stack.Clear();
    }

    private void EmitCompareBranch(VBlock currentBlock, LirBlock block, VCompareBranch branch)
    {
        if (branch.Unsigned)
            throw new NotSupportedException("Unsigned compare branches are not yet supported in stack scheduler.");

        PrepareOperands(block, branch.Span, (branch.Left, false), (branch.Right, false));

        AlignConditionalSuccessors(currentBlock, block, branch.TrueTarget, branch.FalseTarget, branch.Span, reservedTop: 2);

        var opcode = branch.Op switch
        {
            VCompareOp.Eq => LirOpcode.JMPEQ,
            VCompareOp.Ne => LirOpcode.JMPNE,
            VCompareOp.Gt => LirOpcode.JMPGT,
            VCompareOp.Ge => LirOpcode.JMPGE,
            VCompareOp.Lt => LirOpcode.JMPLT,
            VCompareOp.Le => LirOpcode.JMPLE,
            _ => throw new NotSupportedException($"Compare op '{branch.Op}' is not supported.")
        };

        EmitJump(block, opcode, branch.TrueTarget.Label, branch.Span);
        EmitJump(block, LirOpcode.JMP, branch.FalseTarget.Label, branch.Span);
        _stack.Clear();
    }

    private void EmitSwitch(VBlock source, LirBlock block, VSwitch vSwitch)
    {
        PrepareSingleOperand(block, vSwitch.Key, vSwitch.Span);

        var cases = vSwitch.Cases;
        for (int i = 0; i < cases.Count; i++)
        {
            var (caseValue, caseTarget) = cases[i];
            var isLastCase = i == cases.Count - 1;

            if (!isLastCase)
                EmitDup(block, vSwitch.Span);

            PushInt(block, caseValue, vSwitch.Span, producedNode: null, markImmediate: true);
            EmitInstruction(block, new LirInst(LirOpcode.NUMEQUAL) { Span = vSwitch.Span });

            AlignSuccessorStack(source, caseTarget, block, vSwitch.Span, reservedTop: 1);
            EmitJump(block, LirOpcode.JMPIF, caseTarget.Label, vSwitch.Span);
        }

        if (cases.Count == 0)
            EmitDrop(block, vSwitch.Span);

        AlignSuccessorStack(source, vSwitch.DefaultTarget, block, vSwitch.Span);
        EmitJump(block, LirOpcode.JMP, vSwitch.DefaultTarget.Label, vSwitch.Span);
        _stack.Clear();
    }

    private void EmitJump(LirBlock block, LirOpcode opcode, string targetLabel, SourceSpan? span)
    {
        if (opcode is not LirOpcode.JMP and not LirOpcode.JMPIF and not LirOpcode.JMPIFNOT
            and not LirOpcode.JMPEQ and not LirOpcode.JMPNE
            and not LirOpcode.JMPGT and not LirOpcode.JMPGE
            and not LirOpcode.JMPLT and not LirOpcode.JMPLE)
            throw new ArgumentOutOfRangeException(nameof(opcode));

        var inst = new LirInst(opcode)
        {
            Span = span,
            TargetLabel = targetLabel
        };

        EmitInstruction(block, inst);
    }

    #endregion

    #region Operand preparation

    private Dictionary<VNode, int> BeginReservations()
    {
        _reservationScratch.Clear();
        return _reservationScratch;
    }

    private void PrepareSingleOperand(LirBlock block, VNode operand, SourceSpan? span)
    {
        var reservations = BeginReservations();
        PrepareOperand(block, operand, span, forceDuplicate: false, reservations);
    }

    private void PrepareOperands(LirBlock block, SourceSpan? span, params (VNode Node, bool ForceDuplicate)[] operands)
    {
        var reservations = BeginReservations();
        foreach (var (node, force) in operands)
            PrepareOperand(block, node, span, force, reservations);
    }

    private void PrepareOperand(LirBlock block, VNode operand, SourceSpan? span, bool forceDuplicate, Dictionary<VNode, int> reservations)
    {
        var index = FindSlotIndex(operand);
        if (index < 0)
        {
            if (operand is VParam param)
            {
                var ldArg = new LirInst(LirOpcode.LDARG)
                {
                    Span = span,
                    Immediate = new[] { unchecked((byte)param.Index) }
                };

                EmitInstruction(block, ldArg);
                TagResult(param);
                DropIfDead(block, param);
                index = _stack.Count - 1;
            }
            else
            {
                if (!_emittedNodes.Contains(operand))
                {
                    ScheduleNode(operand, block);
                }
                else if (operand is VConvert or VStaticLoad)
                {
                    ReserveTemporaryUse(operand);
                    _emittedNodes.Remove(operand);
                    ScheduleNode(operand, block);
                }
                else if (operand is VConstInt or VConstBool or VConstByteString or VConstBuffer)
                {
                    ReserveTemporaryUse(operand);
                    _emittedNodes.Remove(operand);
                    ScheduleNode(operand, block);
                }

                index = FindSlotIndex(operand);

                if (index < 0)
                {
                    if (operand is VConvert conv)
                    {
                        var uses = GetRemainingUses(conv);
                        var totalUses = _useCounts.TryGetValue(conv, out var total) ? total : 0;
                        throw new InvalidOperationException($"Operand '{operand.GetType().Name}' ({conv.Op}) could not be materialised on stack. RemainingUses={uses}, TotalUses={totalUses}");
                    }

                    if (operand is VStaticLoad staticLoad)
                    {
                        var uses = GetRemainingUses(staticLoad);
                        var totalUses = _useCounts.TryGetValue(staticLoad, out var total) ? total : 0;
                        throw new InvalidOperationException($"Operand '{operand.GetType().Name}' could not be materialised on stack. RemainingUses={uses}, TotalUses={totalUses}");
                    }

                    throw new InvalidOperationException($"Operand '{operand.GetType().Name}' could not be materialised on stack.");
                }
            }
        }

        var depth = _stack.Count - 1 - index;
        var remaining = GetRemainingUses(operand);
        var reserved = reservations.TryGetValue(operand, out var res) ? res : 0;
        var shouldDuplicate = forceDuplicate || remaining - reserved > 1;

        if (shouldDuplicate)
            DuplicateSlot(block, depth, span);
        else
            MoveSlotToTop(block, depth, span);

        var slot = _stack[^1];
        slot.Reserved = true;
        reservations[operand] = reserved + 1;
    }

    private int FindSlotIndex(VNode node)
    {
        for (var i = _stack.Count - 1; i >= 0; i--)
        {
            var slot = _stack[i];
            if (!slot.Reserved && ReferenceEquals(slot.Value, node))
                return i;
        }

        return -1;
    }

    private void DuplicateSlot(LirBlock block, int depth, SourceSpan? span)
    {
        if (depth < 0)
            throw new ArgumentOutOfRangeException(nameof(depth));

        if (depth == 0)
        {
            EmitDup(block, span);
        }
        else if (depth == 1)
        {
            EmitOver(block, span);
        }
        else
        {
            PushImmediateInt(block, span, depth);
            EmitPick(block, span, depth);
        }
    }

    private void MoveSlotToTop(LirBlock block, int depth, SourceSpan? span)
    {
        if (depth <= 0)
            return;

        if (depth == 1)
        {
            EmitSwap(block, span);
            return;
        }

        if (depth == 2)
        {
            EmitRot(block, span);
            return;
        }

        PushImmediateInt(block, span, depth);
        EmitRoll(block, span, depth);
    }

    private void AlignValues(LirBlock block, IReadOnlyList<VNode> values, SourceSpan? span, int reservedTop)
    {
        if (values.Count == 0)
            return;

        var extracted = new List<StackSlot>(values.Count);

        for (int i = values.Count - 1; i >= 0; i--)
        {
            var value = values[i];
            var index = FindSlotIndex(value);
            if (index < 0)
            {
                StackSlot? replicaSource = null;
                for (int j = extracted.Count - 1; j >= 0; j--)
                {
                    if (ReferenceEquals(extracted[j].Value, value))
                    {
                        replicaSource = extracted[j];
                        break;
                    }
                }

                if (replicaSource is not null)
                {
                    extracted.Add(replicaSource.CloneReplica());
                    continue;
                }

                throw new InvalidOperationException($"Value '{value.GetType().Name}' required for successor edge is not present on stack.");
            }

            extracted.Add(_stack[index]);
            _stack.RemoveAt(index);
        }

        var insertIndex = Math.Max(0, _stack.Count - reservedTop);
        for (int i = extracted.Count - 1; i >= 0; i--)
            _stack.Insert(insertIndex, extracted[i]);
    }

    private IReadOnlyList<VNode> GetTransferValues(VBlock from, VBlock to, int reservedTop)
    {
        if (_edgePhiTransfers.TryGetValue((from, to), out var values) && values.Count > 0)
            return values;

        return CaptureLiveStackValues(reservedTop);
    }

    private IReadOnlyList<VNode> CaptureLiveStackValues(int reservedTop)
    {
        var transferableCount = Math.Max(0, _stack.Count - reservedTop);
        if (transferableCount == 0)
            return Array.Empty<VNode>();

        var list = new List<VNode>(transferableCount);
        for (int i = 0; i < transferableCount; i++)
        {
            var slot = _stack[i];
            if (slot.Value is null)
                continue;
            if (slot.Value is VParam)
                continue;
            if (GetRemainingUses(slot.Value) == 0)
                continue;
            list.Add(slot.Value);
        }

        return list.Count == 0 ? Array.Empty<VNode>() : list;
    }

    private void RecordEntryStack(VBlock current, VBlock successor, IReadOnlyList<VNode> values)
    {
        if (!_blockEntryStacks.TryGetValue(successor, out var existing))
        {
            _blockEntryStacks[successor] = new List<VNode>(values);
            if (s_traceStack)
                Console.WriteLine($"[STACK] Recording entry stack for {successor.Label} from {current.Label}: [{FormatStack(values)}]");
            return;
        }

        if (existing.Count != values.Count)
        {
            if (s_traceStack)
            {
                Console.WriteLine($"[STACK] Shape mismatch for {successor.Label}: existing=[{FormatStack(existing)}] incoming=[{FormatStack(values)}] (from {current.Label})");
            }
            throw new InvalidOperationException($"Stack shape mismatch for block '{successor.Label}'.");
        }

        for (int i = 0; i < existing.Count; i++)
        {
            if (!ReferenceEquals(existing[i], values[i]))
            {
                if (s_traceStack)
                {
                    Console.WriteLine($"[STACK] Value mismatch for {successor.Label} @ {i}: existing={DescribeVNode(existing[i])} incoming={DescribeVNode(values[i])} (from {current.Label})");
                }
                throw new InvalidOperationException($"Stack shape mismatch for block '{successor.Label}'.");
            }
        }
    }

    private static string FormatStack(IReadOnlyList<VNode> nodes)
        => string.Join(", ", nodes.Select(DescribeVNode));

    private static string DescribeVNode(VNode? node)
    {
        if (node is null)
            return "<null>";
        return node switch
        {
            VLoadLocal load => $"LoadLocal(slot={load.Slot})",
            VStoreLocal store => $"StoreLocal(slot={store.Slot})",
            VConstInt constInt => $"ConstInt({constInt.Value})",
            VConstBool constBool => $"ConstBool({constBool.Value})",
            VCall call => $"Call({call.Callee})",
            VBinary binary => $"Binary({binary.Op})",
            _ => node.GetType().Name
        };
    }

    private void AlignSuccessorStack(VBlock current, VBlock successor, LirBlock block, SourceSpan? span, int reservedTop = 0)
    {
        var values = GetTransferValues(current, successor, reservedTop);
        RecordEntryStack(current, successor, values);
        if (values.Count > 0)
            AlignValues(block, values, span, reservedTop);
    }

    private void AlignConditionalSuccessors(VBlock current, LirBlock block, VBlock trueTarget, VBlock falseTarget, SourceSpan? span, int reservedTop)
    {
        var trueValues = GetTransferValues(current, trueTarget, reservedTop);
        var falseValues = GetTransferValues(current, falseTarget, reservedTop);

        if (trueValues.Count != falseValues.Count)
            throw new NotSupportedException("Conditional branch requires matching stack shapes for both successors.");

        for (int i = 0; i < trueValues.Count; i++)
        {
            if (!ReferenceEquals(trueValues[i], falseValues[i]))
                throw new NotSupportedException("Conditional branch requires identical phi transfer order for both successors.");
        }

        RecordEntryStack(current, trueTarget, trueValues);
        RecordEntryStack(current, falseTarget, falseValues);

        if (trueValues.Count > 0)
            AlignValues(block, trueValues, span, reservedTop);
    }

    #endregion

    #region Stack helpers

    private void PushPlaceholder(VNode node)
    {
        _stack.Add(new StackSlot(node, isImmediate: false));
        UpdateMaxStack();
    }

    private void SeedEntryStack(VBlock block)
    {
        var seeded = false;
        if (_blockPhis.TryGetValue(block, out var phis) && phis.Count > 0)
        {
            for (int i = phis.Count - 1; i >= 0; i--)
            {
                if (GetRemainingUses(phis[i]) == 0)
                    continue;

                _stack.Add(new StackSlot(phis[i], isImmediate: false));
                UpdateMaxStack();
            }

            seeded = true;
        }

        if (seeded)
            return;

        if (_blockEntryStacks.TryGetValue(block, out var values) && values.Count > 0)
        {
            for (int i = values.Count - 1; i >= 0; i--)
            {
                var value = values[i];
                if (GetRemainingUses(value) == 0)
                    continue;
                _stack.Add(new StackSlot(value, isImmediate: false));
                UpdateMaxStack();
            }
        }
    }

    private void PushInt(LirBlock block, BigInteger value, SourceSpan? span, VNode? producedNode, bool markImmediate)
    {
        var inst = new LirInst(LirOpcode.PUSHINT)
        {
            Span = span,
            Immediate = value.ToByteArray()
        };

        EmitInstruction(block, inst);

        var slot = _stack[^1];
        slot.Value = producedNode;
        slot.IsImmediate = markImmediate;
        slot.IsReplica = false;

        if (producedNode is not null)
            TagResult(producedNode);
    }

    private void PushBool(LirBlock block, bool value, SourceSpan? span, VNode producedNode)
    {
        var opcode = value ? LirOpcode.PUSHT : LirOpcode.PUSHF;
        EmitInstruction(block, new LirInst(opcode) { Span = span });
        TagResult(producedNode);
    }

    private void PushNull(LirBlock block, SourceSpan? span, VNode producedNode)
    {
        EmitInstruction(block, new LirInst(LirOpcode.PUSHNULL) { Span = span });
        TagResult(producedNode);
    }

    private void PushByteString(LirBlock block, byte[] data, SourceSpan? span, VNode producedNode)
    {
        LirOpcode opcode;
        if (data.Length <= byte.MaxValue)
            opcode = LirOpcode.PUSHDATA1;
        else if (data.Length <= ushort.MaxValue)
            opcode = LirOpcode.PUSHDATA2;
        else
            opcode = LirOpcode.PUSHDATA4;

        var inst = new LirInst(opcode)
        {
            Span = span,
            Immediate = data
        };

        EmitInstruction(block, inst);
        TagResult(producedNode);
    }

    private void PushBuffer(LirBlock block, VConstBuffer buffer)
    {
        LirOpcode opcode;
        if (buffer.Value.Length <= byte.MaxValue)
            opcode = LirOpcode.PUSHDATA1;
        else if (buffer.Value.Length <= ushort.MaxValue)
            opcode = LirOpcode.PUSHDATA2;
        else
            opcode = LirOpcode.PUSHDATA4;

        var pushInst = new LirInst(opcode)
        {
            Span = buffer.Span,
            Immediate = buffer.Value
        };

        EmitInstruction(block, pushInst);

        var convertInst = new LirInst(LirOpcode.CONVERT)
        {
            Span = buffer.Span,
            Immediate = new[] { (byte)Neo.VM.Types.StackItemType.Buffer }
        };

        EmitInstruction(block, convertInst);
        TagResult(buffer);
    }

    private void PushImmediateInt(LirBlock block, SourceSpan? span, int value)
    {
        if (value == 0)
        {
            EmitInstruction(block, new LirInst(LirOpcode.PUSH0) { Span = span });
        }
        else if (value == -1)
        {
            EmitInstruction(block, new LirInst(LirOpcode.PUSHM1) { Span = span });
        }
        else
        {
            PushInt(block, new BigInteger(value), span, producedNode: null, markImmediate: true);
            var slot = _stack[^1];
            slot.IsImmediate = true;
            return;
        }

        var pushedSlot = _stack[^1];
        pushedSlot.Value = null;
        pushedSlot.IsImmediate = true;
    }

    private void EmitDup(LirBlock block, SourceSpan? span)
    {
        if (_stack.Count == 0)
            throw new InvalidOperationException("Cannot DUP on empty stack.");

        block.Instructions.Add(new LirInst(LirOpcode.DUP) { Span = span });

        var original = _stack[^1];
        var clone = original.CloneReplica();
        _stack.Add(clone);
        UpdateMaxStack();
        ClearReservations();
    }

    private void EmitOver(LirBlock block, SourceSpan? span)
    {
        if (_stack.Count < 2)
            throw new InvalidOperationException("Cannot OVER with fewer than two values.");

        block.Instructions.Add(new LirInst(LirOpcode.OVER) { Span = span });

        var source = _stack[^2];
        var clone = source.CloneReplica();
        _stack.Add(clone);
        UpdateMaxStack();
        ClearReservations();
    }

    private void EmitSwap(LirBlock block, SourceSpan? span)
    {
        if (_stack.Count < 2)
            throw new InvalidOperationException("Cannot SWAP with fewer than two values.");

        block.Instructions.Add(new LirInst(LirOpcode.SWAP) { Span = span });

        var top = _stack[^1];
        _stack[^1] = _stack[^2];
        _stack[^2] = top;
        ClearReservations();
    }

    private void EmitRot(LirBlock block, SourceSpan? span)
    {
        if (_stack.Count < 3)
            throw new InvalidOperationException("Cannot ROT with fewer than three values.");

        block.Instructions.Add(new LirInst(LirOpcode.ROT) { Span = span });

        var third = _stack[^3];
        var second = _stack[^2];
        var first = _stack[^1];
        _stack[^3] = second;
        _stack[^2] = first;
        _stack[^1] = third;
        ClearReservations();
    }

    private void EmitPick(LirBlock block, SourceSpan? span, int depth)
    {
        block.Instructions.Add(new LirInst(LirOpcode.PICK) { Span = span });

        if (_stack.Count == 0 || !_stack[^1].IsImmediate)
            throw new InvalidOperationException("PICK expects depth immediate on stack.");

        _stack.RemoveAt(_stack.Count - 1); // remove depth immediate

        var index = _stack.Count - 1 - depth;
        if (index < 0)
            throw new InvalidOperationException("PICK depth exceeds stack height.");

        var source = _stack[index];
        var clone = source.CloneReplica();
        _stack.Add(clone);
        UpdateMaxStack();
        ClearReservations();
    }

    private void EmitRoll(LirBlock block, SourceSpan? span, int depth)
    {
        block.Instructions.Add(new LirInst(LirOpcode.ROLL) { Span = span });

        if (_stack.Count == 0 || !_stack[^1].IsImmediate)
            throw new InvalidOperationException("ROLL expects depth immediate on stack.");

        _stack.RemoveAt(_stack.Count - 1); // remove depth immediate

        var index = _stack.Count - 1 - depth;
        if (index < 0)
            throw new InvalidOperationException("ROLL depth exceeds stack height.");

        var slot = _stack[index];
        _stack.RemoveAt(index);
        _stack.Add(slot);
        ClearReservations();
    }

    private int EmitInstruction(LirBlock block, LirInst inst)
    {
        block.Instructions.Add(inst);

        var info = LirOpcodeTable.Get(inst.Op);
        var pop = inst.PopOverride ?? info.Pop ?? 0;
        var push = inst.PushOverride ?? info.Push ?? 0;

        for (var i = 0; i < pop; i++)
        {
            if (_stack.Count == 0)
                throw new InvalidOperationException($"Stack underflow while emitting '{inst.Op}'.");

            var slot = _stack[^1];
            _stack.RemoveAt(_stack.Count - 1);

            if (slot.Value is not null)
                ConsumeNode(slot.Value);
        }

        for (var i = 0; i < push; i++)
            _stack.Add(new StackSlot(null, isImmediate: false));

        UpdateMaxStack();
        ClearReservations();
        return push;
    }

    private void EmitDrop(LirBlock block, SourceSpan? span)
    {
        if (_stack.Count == 0)
            return;

        EmitInstruction(block, new LirInst(LirOpcode.DROP) { Span = span });
    }

    private void TagResult(VNode node)
    {
        if (_stack.Count == 0)
            throw new InvalidOperationException("Expected produced value on top of stack.");

        var slot = _stack[^1];
        slot.Value = node;
        slot.IsImmediate = false;
        slot.IsReplica = false;
    }

    private void DropIfDead(LirBlock block, VNode node)
    {
        if (GetRemainingUses(node) == 0)
            EmitDrop(block, node.Span);
    }

    private void UpdateMaxStack()
    {
        if (_stack.Count > _maxStack)
            _maxStack = _stack.Count;
    }

    private void ClearReservations()
    {
        foreach (var slot in _stack)
            slot.Reserved = false;
    }

    private void HandlePhi(LirBlock block, VPhi phi)
    {
        if (GetRemainingUses(phi) == 0)
            return;
    }

    private static bool TryGetIntBounds(LirType type, out BigInteger minInclusive, out BigInteger maxExclusive)
    {
        if (type is LirIntType intType && intType.WidthHintBits is int width && width > 0 && width <= 256)
        {
            if (intType.IsSigned)
            {
                var magnitude = BigInteger.One << (width - 1);
                minInclusive = -magnitude;
                maxExclusive = magnitude;
            }
            else
            {
                minInclusive = BigInteger.Zero;
                maxExclusive = BigInteger.One << width;
            }

            return true;
        }

        minInclusive = default;
        maxExclusive = default;
        return false;
    }

    #endregion
}
