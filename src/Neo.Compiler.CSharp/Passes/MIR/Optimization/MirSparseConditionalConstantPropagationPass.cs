using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.MIR.Optimization;

/// <summary>
/// Sparse conditional constant propagation over MIR. Tracks block reachability while evaluating a value lattice
/// (unknown/constant/overdefined) and replaces foldable instructions, tightens branches, and prunes unreachable blocks.
/// </summary>
internal sealed class MirSparseConditionalConstantPropagationPass : IMirPass
{
    private static readonly bool s_trace = string.Equals(
        Environment.GetEnvironmentVariable("NEO_SCCP_TRACE"),
        "1",
        StringComparison.OrdinalIgnoreCase);
    private const int MaxIterations = 10000;

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
        internal ValueState()
        {
            Kind = LatticeKind.Unknown;
        }

        internal LatticeKind Kind { get; set; }
        internal object? Value { get; set; }
    }

    private readonly Dictionary<MirValue, ValueState> _states = new();
    private readonly HashSet<MirBlock> _reachable = new();
    private readonly Queue<MirBlock> _worklist = new();
    private readonly HashSet<MirBlock> _queued = new();
    private MirFunction _function = null!;
    private int _iterations;

    private static void Trace(string message)
    {
        if (s_trace)
            Console.WriteLine($"[SCCP] {message}");
    }

    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        _function = function;
        _states.Clear();
        _reachable.Clear();
        _worklist.Clear();
        _queued.Clear();
        _iterations = 0;

        SeedStates(function);

        Enqueue(function.Entry);
        while (_worklist.Count > 0)
        {
            _iterations++;
            if (_iterations > MaxIterations)
                throw new InvalidOperationException($"SCCP exceeded iteration limit ({MaxIterations}) for function '{function.Name}'. Enable NEO_SCCP_TRACE=1 to trace progress.");
            var block = _worklist.Dequeue();
            _queued.Remove(block);
            _reachable.Add(block);

            var blockChanged = ProcessBlock(block);
            Trace($"Processed block {block.Label}, changed={blockChanged}, worklist={_worklist.Count}");
            foreach (var successor in MirControlFlow.GetSuccessors(block))
            {
                if (blockChanged || !_reachable.Contains(successor))
                    Enqueue(successor);
            }
        }

        return ApplyResults(function);
    }

    private void SeedStates(MirFunction function)
    {
        foreach (var block in function.Blocks)
        {
            foreach (var phi in block.Phis)
            {
                GetState(phi);
            }

            foreach (var inst in block.Instructions)
            {
                var state = GetState(inst);
                switch (inst)
                {
                    case MirConstInt constInt:
                        state.Kind = LatticeKind.Constant;
                        state.Value = constInt.Value;
                        break;
                    case MirConstBool constBool:
                        state.Kind = LatticeKind.Constant;
                        state.Value = constBool.Value;
                        break;
                    case MirConstByteString constBytes:
                        state.Kind = LatticeKind.Constant;
                        state.Value = CopyBytes(constBytes.Value);
                        break;
                    case MirConstBuffer constBuffer:
                        state.Kind = LatticeKind.Constant;
                        state.Value = CopyBytes(constBuffer.Value);
                        break;
                    case MirConstNull:
                        state.Kind = LatticeKind.Constant;
                        state.Value = null;
                        break;
                }
            }
        }
    }

    private void Enqueue(MirBlock block)
    {
        if (block is null)
            return;
        if (_queued.Add(block))
            _worklist.Enqueue(block);
    }

    private bool ProcessBlock(MirBlock block)
    {
        var changed = false;

        void EnqueueIfNeeded(MirBlock? target)
        {
            if (target is null)
                return;
            if (changed || !_reachable.Contains(target))
                Enqueue(target);
        }

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

        // Terminators drive reachability. Only enqueue successors that remain feasible.
        switch (block.Terminator)
        {
            case MirBranch branch:
                EnqueueIfNeeded(branch.Target);
                break;

            case MirCondBranch cond:
                if (TryGetBool(cond.Condition, out var condition))
                {
                    EnqueueIfNeeded(condition ? cond.TrueTarget : cond.FalseTarget);
                }
                else
                {
                    EnqueueIfNeeded(cond.TrueTarget);
                    EnqueueIfNeeded(cond.FalseTarget);
                }
                break;

            case MirSwitch @switch:
                if (TryGetInt(@switch.Key, out var key))
                {
                    var matched = false;
                    foreach (var (caseValue, target) in @switch.Cases)
                    {
                        if (caseValue == key)
                        {
                            EnqueueIfNeeded(target);
                            matched = true;
                            break;
                        }
                    }

                    if (!matched)
                        EnqueueIfNeeded(@switch.DefaultTarget);
                }
                else
                {
                    foreach (var (_, target) in @switch.Cases)
                        EnqueueIfNeeded(target);
                    EnqueueIfNeeded(@switch.DefaultTarget);
                }
                break;

            case MirLeave leave:
                EnqueueIfNeeded(leave.Target);
                break;

            case MirEndFinally endFinally:
                EnqueueIfNeeded(endFinally.Target);
                break;

            default:
                foreach (var successor in MirControlFlow.GetSuccessors(block))
                    EnqueueIfNeeded(successor);
                break;
        }

        return changed;
    }

    private Evaluation EvaluatePhi(MirPhi phi)
    {
        var hasValue = false;
        object? value = null;

        foreach (var (pred, incoming) in phi.Inputs)
        {
            if (!_reachable.Contains(pred))
                continue;

            var state = GetState(incoming);
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

    private Evaluation EvaluateInstruction(MirInst inst)
    {
        switch (inst)
        {
            case MirBinary binary when binary.Effect == MirEffect.None:
                {
                    if (!TryGetInt(binary.Left, out var left) || !TryGetInt(binary.Right, out var right))
                        return Evaluation.Unknown;

                    var result = binary.OpCode switch
                    {
                        MirBinary.Op.Add => left + right,
                        MirBinary.Op.Sub => left - right,
                        MirBinary.Op.Mul => left * right,
                        MirBinary.Op.Div when right != BigInteger.Zero => left / right,
                        MirBinary.Op.Mod when right != BigInteger.Zero => left % right,
                        MirBinary.Op.And => left & right,
                        MirBinary.Op.Or => left | right,
                        MirBinary.Op.Xor => left ^ right,
                        MirBinary.Op.Shl when TryToInt(right, out var shift) => left << shift,
                        MirBinary.Op.Shr when TryToInt(right, out var shift) => left >> shift,
                        _ => (BigInteger?)null
                    };

                    if (result is null)
                        return Evaluation.Overdefined;

                    return Evaluation.Constant(result.Value);
                }

            case MirCompare compare:
                {
                    if (!TryGetInt(compare.Left, out var left) || !TryGetInt(compare.Right, out var right))
                        return Evaluation.Unknown;

                    var result = compare.OpCode switch
                    {
                        MirCompare.Op.Eq => left == right,
                        MirCompare.Op.Ne => left != right,
                        MirCompare.Op.Lt => compare.Unsigned ? left < right : left < right,
                        MirCompare.Op.Le => compare.Unsigned ? left <= right : left <= right,
                        MirCompare.Op.Gt => compare.Unsigned ? left > right : left > right,
                        MirCompare.Op.Ge => compare.Unsigned ? left >= right : left >= right,
                        _ => (bool?)null
                    };

                    if (result is null)
                        return Evaluation.Overdefined;

                    return Evaluation.Constant(result.Value);
                }

            case MirUnary unary when unary.OpCode == MirUnary.Op.Neg:
                {
                    if (!TryGetInt(unary.Operand, out var operand))
                        return Evaluation.Unknown;
                    return Evaluation.Constant(-operand);
                }

            case MirConvert convert when convert.ConversionKind == MirConvert.Kind.ToBool:
                {
                    if (TryGetBool(convert.Value, out var boolValue))
                        return Evaluation.Constant(boolValue);
                    if (TryGetInt(convert.Value, out var intValue))
                        return Evaluation.Constant(intValue != BigInteger.Zero);
                    return Evaluation.Unknown;
                }

            case MirStructPack pack:
                {
                    var foldedFields = new List<MirValue>(pack.Fields.Count);
                    foreach (var field in pack.Fields)
                    {
                        var state = GetState(field);
                        if (state.Kind != LatticeKind.Constant)
                            return Evaluation.Unknown;
                        foldedFields.Add(field);
                    }

                    // Struct packs remain values; no scalar constant result.
                    return Evaluation.Unknown;
                }

            case MirConstInt:
            case MirConstBool:
            case MirConstByteString:
            case MirConstBuffer:
            case MirConstNull:
                {
                    var state = GetState(inst);
                    return state.Kind == LatticeKind.Constant
                        ? Evaluation.Constant(CloneStateValue(state.Value))
                        : Evaluation.Unknown;
                }
        }

        return Evaluation.Unknown;
    }

    private bool MergeState(MirValue value, Evaluation evaluation)
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

    private ValueState GetState(MirValue value)
    {
        if (!_states.TryGetValue(value, out var state))
        {
            state = new ValueState();
            _states[value] = state;
        }

        return state;
    }

    private bool ApplyResults(MirFunction function)
    {
        var changed = false;

        foreach (var block in function.Blocks)
        {
            for (int i = 0; i < block.Phis.Count; i++)
            {
                var phi = block.Phis[i];
                var state = GetState(phi);
                if (state.Kind == LatticeKind.Constant && TryCreateConstant(phi.Type, state.Value, out var constant))
                {
                    constant.Span = phi.Span;
                    block.Instructions.Insert(0, constant);
                    GetState(constant).Kind = LatticeKind.Constant;
                    GetState(constant).Value = CloneStateValue(state.Value);
                    MirValueRewriter.Replace(function, phi, constant);
                    block.RemovePhi(phi);
                    i--;
                    changed = true;
                }
            }
        }

        foreach (var block in function.Blocks)
        {
            var instructions = block.Instructions;
            for (int i = 0; i < instructions.Count; i++)
            {
                var inst = instructions[i];
                var state = GetState(inst);
                if (state.Kind != LatticeKind.Constant)
                    continue;

                if (IsInstructionAlreadyConstant(inst, state.Value))
                    continue;

                if (TryCreateConstant(inst.Type, state.Value, out var constant))
                {
                    constant.Span = inst.Span;
                    instructions[i] = constant;
                    var constantState = GetState(constant);
                    constantState.Kind = LatticeKind.Constant;
                    constantState.Value = CloneStateValue(state.Value);
                    MirValueRewriter.Replace(function, inst, constant);
                    changed = true;
                }
            }
        }

        if (RemoveUnreachableBlocks(function))
            changed = true;

        return changed;
    }

    private bool RemoveUnreachableBlocks(MirFunction function)
    {
        var removed = false;
        var blocks = function.Blocks.ToArray();
        foreach (var block in blocks)
        {
            if (ReferenceEquals(block, function.Entry))
                continue;
            if (_reachable.Contains(block))
                continue;

            function.RemoveBlock(block);
            removed = true;
        }

        return removed;
    }

    private bool TryGetConstant(MirValue value, out object? constant)
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

    private bool TryGetInt(MirValue value, out BigInteger constant)
    {
        if (TryGetConstant(value, out var boxed) && boxed is BigInteger big)
        {
            constant = big;
            return true;
        }

        constant = default;
        return false;
    }

    private bool TryGetBool(MirValue value, out bool constant)
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

        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        if (left is BigInteger lBig && right is BigInteger rBig)
            return lBig == rBig;

        if (left is bool lBool && right is bool rBool)
            return lBool == rBool;

        if (left is byte[] lBytes && right is byte[] rBytes)
            return lBytes.AsSpan().SequenceEqual(rBytes);

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

    private bool TryCreateConstant(MirType type, object? value, out MirInst constant)
    {
        constant = null!;

        switch (type)
        {
            case MirBoolType:
                if (value is bool boolValue)
                {
                    constant = new MirConstBool(boolValue);
                    return true;
                }

                if (value is BigInteger big)
                {
                    constant = new MirConstBool(big != BigInteger.Zero);
                    return true;
                }
                break;

            case MirIntType:
                if (value is BigInteger bigInt)
                {
                    constant = new MirConstInt(bigInt);
                    return true;
                }
                break;

            case MirByteStringType when value is byte[] bytes:
                constant = new MirConstByteString(CopyBytes(bytes));
                return true;

            case MirBufferType when value is byte[] buffer:
                constant = new MirConstBuffer(CopyBytes(buffer));
                return true;

            case MirUnknownType when value is null:
                constant = new MirConstNull();
                return true;
        }

        return false;
    }

    private static bool IsInstructionAlreadyConstant(MirInst inst, object? value)
    {
        return inst switch
        {
            MirConstInt constInt when value is BigInteger bigInt => constInt.Value == bigInt,
            MirConstBool constBool when value is bool boolValue => constBool.Value == boolValue,
            MirConstBool constBool when value is BigInteger bigFromInt => constBool.Value == (bigFromInt != BigInteger.Zero),
            MirConstByteString constBytes when value is byte[] bytes => constBytes.Value.AsSpan().SequenceEqual(bytes),
            MirConstBuffer constBuffer when value is byte[] buffer => constBuffer.Value.AsSpan().SequenceEqual(buffer),
            MirConstNull when value is null => true,
            _ => false
        };
    }
}
