using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR.Optimization;

/// <summary>
/// Sparse conditional constant propagation for HIR. Evaluates a three-point lattice (unknown/constant/overdefined)
/// while tracking reachability so that constant folds, branch tightening, and unreachable block pruning can be applied.
/// </summary>
internal sealed class HirSparseConditionalConstantPropagationPass : IHirPass
{
    private enum LatticeKind
    {
        Unknown,
        Constant,
        Overdefined
    }

    private readonly struct Evaluation
    {
        internal Evaluation(LatticeKind kind, object? value)
        {
            Kind = kind;
            Value = value;
        }

        internal LatticeKind Kind { get; }
        internal object? Value { get; }

        internal static Evaluation Unknown => new(LatticeKind.Unknown, null);
        internal static Evaluation Overdefined => new(LatticeKind.Overdefined, null);
        internal static Evaluation Constant(object? value) => new(LatticeKind.Constant, value);
    }

    private sealed class ValueState
    {
        internal LatticeKind Kind { get; set; } = LatticeKind.Unknown;
        internal object? Value { get; set; }
    }

    private readonly Dictionary<HirValue, ValueState> _states = new();
    private readonly HashSet<HirBlock> _reachable = new();
    private readonly Queue<HirBlock> _worklist = new();
    private readonly HashSet<HirBlock> _queued = new();
    private Dictionary<HirBlock, HashSet<HirBlock>> _predecessors = null!;
    private HirFunction _function = null!;

    public bool Run(HirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        _function = function;
        _states.Clear();
        _reachable.Clear();
        _worklist.Clear();
        _queued.Clear();
        _predecessors = HirControlFlow.BuildPredecessors(function);

        SeedStates(function);

        Enqueue(function.Entry);
        while (_worklist.Count > 0)
        {
            var block = _worklist.Dequeue();
            _queued.Remove(block);
            _reachable.Add(block);

            if (ProcessBlock(block))
            {
                foreach (var successor in HirControlFlow.GetSuccessors(block))
                    Enqueue(successor);
            }
        }

        return ApplyResults(function);
    }

    private void SeedStates(HirFunction function)
    {
        foreach (var block in function.Blocks)
        {
            foreach (var phi in block.Phis)
                GetState(phi);

            foreach (var inst in block.Instructions)
            {
                var state = GetState(inst);
                switch (inst)
                {
                    case HirConstInt constInt:
                        state.Kind = LatticeKind.Constant;
                        state.Value = constInt.Value;
                        break;
                    case HirConstBool constBool:
                        state.Kind = LatticeKind.Constant;
                        state.Value = constBool.Value;
                        break;
                    case HirConstByteString constBytes:
                        state.Kind = LatticeKind.Constant;
                        state.Value = CopyBytes(constBytes.Value);
                        break;
                    case HirConstBuffer constBuffer:
                        state.Kind = LatticeKind.Constant;
                        state.Value = CopyBytes(constBuffer.Value);
                        break;
                }
            }
        }
    }

    private void Enqueue(HirBlock? block)
    {
        if (block is null)
            return;
        if (_queued.Add(block))
            _worklist.Enqueue(block);
    }

    private bool ProcessBlock(HirBlock block)
    {
        var changed = false;

        foreach (var phi in block.Phis)
        {
            var evaluation = EvaluatePhi(phi);
            changed |= MergeState(phi, evaluation);
        }

        foreach (var inst in block.Instructions)
        {
            var evaluation = EvaluateInstruction(inst);
            changed |= MergeState(inst, evaluation);
        }

        switch (block.Terminator)
        {
            case HirBranch branch:
                Enqueue(branch.Target);
                break;
            case HirConditionalBranch cond:
                if (TryGetBool(cond.Condition, out var condition))
                {
                    Enqueue(condition ? cond.TrueBlock : cond.FalseBlock);
                }
                else
                {
                    Enqueue(cond.TrueBlock);
                    Enqueue(cond.FalseBlock);
                }
                break;
            case HirSwitch @switch:
                if (TryGetInt(@switch.Key, out var key))
                {
                    var matched = false;
                    foreach (var (caseValue, target) in @switch.Cases)
                    {
                        if (caseValue == key)
                        {
                            Enqueue(target);
                            matched = true;
                            break;
                        }
                    }

                    if (!matched)
                        Enqueue(@switch.DefaultTarget);
                }
                else
                {
                    foreach (var (_, target) in @switch.Cases)
                        Enqueue(target);
                    Enqueue(@switch.DefaultTarget);
                }
                break;
        }

        return changed;
    }

    private Evaluation EvaluatePhi(HirPhi phi)
    {
        var hasValue = false;
        object? value = null;

        foreach (var incoming in phi.Inputs)
        {
            if (!_reachable.Contains(incoming.Block))
                continue;

            var state = GetState(incoming.Value);
            switch (state.Kind)
            {
                case LatticeKind.Unknown:
                    return Evaluation.Unknown;
                case LatticeKind.Overdefined:
                    return Evaluation.Overdefined;
                case LatticeKind.Constant:
                    if (!hasValue)
                    {
                        value = CloneStateValue(state.Value);
                        hasValue = true;
                    }
                    else if (!ConstantsEqual(value, state.Value))
                    {
                        return Evaluation.Overdefined;
                    }
                    break;
            }
        }

        if (!hasValue)
            return Evaluation.Unknown;

        return Evaluation.Constant(value);
    }

    private Evaluation EvaluateInstruction(HirInst inst)
    {
        if (inst.Effect != HirEffect.None || inst.ProducesMemoryToken || inst.ConsumesMemoryToken)
            return Evaluation.Overdefined;

        switch (inst)
        {
            case HirBinaryInst binary:
                return EvaluateBinary(binary);
            case HirNeg neg:
                if (TryGetInt(neg.Operand, out var intOperand))
                    return Evaluation.Constant(-intOperand);
                break;
            case HirNot not:
                if (TryGetBool(not.Operand, out var boolOperand))
                    return Evaluation.Constant(!boolOperand);
                break;
            case HirCompare compare:
                {
                    if (!TryGetInt(compare.Left, out var left) || !TryGetInt(compare.Right, out var right))
                        break;
                    var result = compare.Kind switch
                    {
                        HirCmpKind.Eq => left == right,
                        HirCmpKind.Ne => left != right,
                        HirCmpKind.Lt => compare.Unsigned ? left < right : left < right,
                        HirCmpKind.Le => compare.Unsigned ? left <= right : left <= right,
                        HirCmpKind.Gt => compare.Unsigned ? left > right : left > right,
                        HirCmpKind.Ge => compare.Unsigned ? left >= right : left >= right,
                        _ => (bool?)null
                    };
                    if (result.HasValue)
                        return Evaluation.Constant(result.Value);
                }
                break;
            case HirConvert convert:
                if (convert.Kind == HirConvKind.ToBool)
                {
                    if (TryGetBool(convert.Value, out var boolValue))
                        return Evaluation.Constant(boolValue);
                    if (TryGetInt(convert.Value, out var intValue))
                        return Evaluation.Constant(intValue != BigInteger.Zero);
                }
                break;
            case HirConcat concat:
                {
                    if (TryGetByteString(concat.Left, out var left) && TryGetByteString(concat.Right, out var right))
                    {
                        var buffer = new byte[left.Length + right.Length];
                        Array.Copy(left, buffer, left.Length);
                        Array.Copy(right, 0, buffer, left.Length, right.Length);
                        return Evaluation.Constant(buffer);
                    }
                }
                break;
            case HirSlice slice:
                {
                    if (TryGetByteString(slice.Value, out var bytes)
                        && TryGetInt(slice.Start, out var start)
                        && TryGetInt(slice.Length, out var length))
                    {
                        if (start >= BigInteger.Zero && start <= int.MaxValue && length >= BigInteger.Zero && length <= int.MaxValue)
                        {
                            var startIndex = (int)start;
                            var sliceLength = (int)length;
                            if (startIndex >= 0 && sliceLength >= 0 && startIndex + sliceLength <= bytes.Length)
                            {
                                var buffer = new byte[sliceLength];
                                Array.Copy(bytes, startIndex, buffer, 0, sliceLength);
                                return Evaluation.Constant(buffer);
                            }
                        }
                    }
                }
                break;
            case HirCheckedBinary checkedBinary:
                {
                    var evaluation = EvaluateCheckedBinary(checkedBinary);
                    if (evaluation.Kind == LatticeKind.Constant)
                        return evaluation;
                }
                break;
        }

        return Evaluation.Unknown;
    }

    private Evaluation EvaluateBinary(HirBinaryInst inst)
    {
        if (!TryGetInt(inst.Left, out var left) || !TryGetInt(inst.Right, out var right))
            return Evaluation.Unknown;

        BigInteger? result = inst switch
        {
            HirAdd => left + right,
            HirSub => left - right,
            HirMul => left * right,
            HirDiv when right != BigInteger.Zero => left / right,
            HirMod when right != BigInteger.Zero => left % right,
            HirBitAnd => left & right,
            HirBitOr => left | right,
            HirBitXor => left ^ right,
            HirShl when TryToInt(right, out var shift) => left << shift,
            HirShr when TryToInt(right, out var shift) => left >> shift,
            _ => null
        };

        if (result is null)
            return Evaluation.Overdefined;

        return Evaluation.Constant(result.Value);
    }

    private Evaluation EvaluateCheckedBinary(HirCheckedBinary inst)
    {
        if (!TryGetInt(inst.Left, out var left) || !TryGetInt(inst.Right, out var right))
            return Evaluation.Unknown;

        BigInteger? result = inst.Operation switch
        {
            HirCheckedOp.Add => left + right,
            HirCheckedOp.Sub => left - right,
            HirCheckedOp.Mul => left * right,
            _ => null
        };

        if (result is null)
            return Evaluation.Overdefined;

        return Evaluation.Constant(result.Value);
    }

    private bool MergeState(HirValue value, Evaluation evaluation)
    {
        var state = GetState(value);
        switch (evaluation.Kind)
        {
            case LatticeKind.Unknown:
                return false;

            case LatticeKind.Overdefined:
                if (state.Kind == LatticeKind.Overdefined)
                    return false;
                state.Kind = LatticeKind.Overdefined;
                state.Value = null;
                return true;

            case LatticeKind.Constant:
                if (state.Kind == LatticeKind.Constant)
                {
                    if (ConstantsEqual(state.Value, evaluation.Value))
                        return false;
                    state.Kind = LatticeKind.Overdefined;
                    state.Value = null;
                    return true;
                }

                if (state.Kind == LatticeKind.Overdefined)
                    return false;

                state.Kind = LatticeKind.Constant;
                state.Value = CloneStateValue(evaluation.Value);
                return true;

            default:
                return false;
        }
    }

    private ValueState GetState(HirValue value)
    {
        if (!_states.TryGetValue(value, out var state))
        {
            state = new ValueState();
            _states[value] = state;
        }

        return state;
    }

    private bool ApplyResults(HirFunction function)
    {
        var changed = false;

        foreach (var block in function.Blocks)
        {
            switch (block.Terminator)
            {
                case HirConditionalBranch cond when TryGetBool(cond.Condition, out var condition):
                    {
                        var branchTarget = condition ? cond.TrueBlock : cond.FalseBlock;
                        block.SetTerminator(new HirBranch(branchTarget) { Span = cond.Span });
                        changed = true;
                        break;
                    }
                case HirSwitch @switch when TryGetInt(@switch.Key, out var key):
                    {
                        HirBlock? caseTarget = null;
                        foreach (var (caseValue, candidate) in @switch.Cases)
                        {
                            if (caseValue == key)
                            {
                                caseTarget = candidate;
                                break;
                            }
                        }

                        block.SetTerminator(new HirBranch(caseTarget ?? @switch.DefaultTarget) { Span = @switch.Span });
                        changed = true;
                        break;
                    }
            }
        }

        RemoveUnreachableBlocks(function);
        PrunePhiIncomingEdges(function);

        return changed;
    }

    private void RemoveUnreachableBlocks(HirFunction function)
    {
        var blocks = new List<HirBlock>(function.Blocks);
        foreach (var block in blocks)
        {
            if (ReferenceEquals(block, function.Entry))
                continue;
            if (_reachable.Contains(block))
                continue;

            function.RemoveBlock(block);

            foreach (var other in function.Blocks)
            {
                foreach (var phi in other.Phis)
                    phi.RemoveIncoming(block);
            }
        }
    }

    private void PrunePhiIncomingEdges(HirFunction function)
    {
        foreach (var block in function.Blocks)
        {
            foreach (var phi in block.Phis)
            {
                var blocksToRemove = new HashSet<HirBlock>();
                foreach (var incoming in phi.Inputs)
                {
                    if (!_reachable.Contains(incoming.Block))
                        blocksToRemove.Add(incoming.Block);
                }

                foreach (var blockToRemove in blocksToRemove)
                    phi.RemoveIncoming(blockToRemove);
            }
        }
    }

    private bool TryGetConstant(HirValue value, out object? constant)
    {
        var state = GetState(value);
        if (state.Kind == LatticeKind.Constant)
        {
            constant = state.Value;
            return true;
        }

        constant = null;
        return false;
    }

    private bool TryGetInt(HirValue value, out BigInteger constant)
    {
        if (TryGetConstant(value, out var boxed) && boxed is BigInteger big)
        {
            constant = big;
            return true;
        }

        constant = default;
        return false;
    }

    private bool TryGetBool(HirValue value, out bool constant)
    {
        if (TryGetConstant(value, out var boxed))
        {
            switch (boxed)
            {
                case bool b:
                    constant = b;
                    return true;
                case BigInteger big:
                    constant = big != BigInteger.Zero;
                    return true;
            }
        }

        constant = default;
        return false;
    }

    private bool TryGetByteString(HirValue value, out byte[] data)
    {
        if (TryGetConstant(value, out var boxed) && boxed is byte[] bytes)
        {
            data = bytes;
            return true;
        }

        data = Array.Empty<byte>();
        return false;
    }

    private static bool TryToInt(BigInteger value, out int result)
    {
        if (value >= int.MinValue && value <= int.MaxValue)
        {
            result = (int)value;
            return true;
        }

        result = default;
        return false;
    }

    private static bool ConstantsEqual(object? left, object? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        if (left is BigInteger lBig && right is BigInteger rBig)
            return lBig == rBig;

        if (left is bool lBool && right is bool rBool)
            return lBool == rBool;

        if (left is byte[] lBytes && right is byte[] rBytes)
        {
            if (lBytes.Length != rBytes.Length)
                return false;

            for (int i = 0; i < lBytes.Length; i++)
            {
                if (lBytes[i] != rBytes[i])
                    return false;
            }

            return true;
        }

        return left.Equals(right);
    }

    private static object? CloneStateValue(object? value)
    {
        if (value is byte[] bytes)
            return CopyBytes(bytes);
        return value;
    }

    private static byte[] CopyBytes(byte[] value)
    {
        var copy = new byte[value.Length];
        Array.Copy(value, copy, value.Length);
        return copy;
    }

    private bool TryCreateConstant(HirType type, object? value, out HirInst constant)
    {
        constant = null!;
        switch (type)
        {
            case HirBoolType:
                if (value is bool boolValue)
                {
                    constant = new HirConstBool(boolValue);
                    return true;
                }
                if (value is BigInteger bigBool)
                {
                    constant = new HirConstBool(bigBool != BigInteger.Zero);
                    return true;
                }
                break;
            case HirIntType intType when value is BigInteger bigInt:
                constant = new HirConstInt(bigInt, intType);
                return true;
            case HirIntType when value is BigInteger bigIntDefault:
                constant = new HirConstInt(bigIntDefault);
                return true;
            case HirByteStringType when value is byte[] bytes:
                constant = new HirConstByteString(CopyBytes(bytes));
                return true;
            case HirBufferType when value is byte[] buffer:
                constant = new HirConstBuffer(CopyBytes(buffer));
                return true;
        }

        return false;
    }

    private static bool IsInstructionAlreadyConstant(HirInst inst, object? value)
    {
        return inst switch
        {
            HirConstInt constInt when value is BigInteger bigInt => constInt.Value == bigInt,
            HirConstBool constBool when value is bool boolValue => constBool.Value == boolValue,
            HirConstBool constBool when value is BigInteger bigInt => constBool.Value == (bigInt != BigInteger.Zero),
            HirConstByteString constBytes when value is byte[] bytes => ByteArraysEqual(constBytes.Value, bytes),
            HirConstBuffer constBuffer when value is byte[] buffer => ByteArraysEqual(constBuffer.Value, buffer),
            _ => false
        };
    }

    private static bool ByteArraysEqual(byte[] left, byte[] right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left.Length != right.Length)
            return false;
        for (int i = 0; i < left.Length; i++)
        {
            if (left[i] != right[i])
                return false;
        }

        return true;
    }

}
