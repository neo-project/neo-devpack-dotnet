using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.MIR.Optimization;

/// <summary>
/// Performs sparse global value numbering across MIR, folding redundant pure expressions that are dominated by an
/// equivalent definition.
/// </summary>
internal sealed class MirGlobalValueNumberingPass : IMirPass
{
    private readonly struct ExpressionKey : IEquatable<ExpressionKey>
    {
        private readonly ExpressionKind _kind;
        private readonly MirType? _type;
        private readonly int _opCode;
        private readonly bool _flag;
        private readonly int _operand0;
        private readonly int _operand1;
        private readonly int _operand2;
        private readonly int _operandCount;

        private ExpressionKey(ExpressionKind kind, MirType? type, int opCode, bool flag, int operand0, int operand1, int operand2, int operandCount)
        {
            _kind = kind;
            _type = type;
            _opCode = opCode;
            _flag = flag;
            _operand0 = operand0;
            _operand1 = operand1;
            _operand2 = operand2;
            _operandCount = operandCount;
        }

        internal static ExpressionKey Binary(MirBinary binary, Func<MirValue, int> getId)
        {
            var left = getId(binary.Left);
            var right = getId(binary.Right);
            if (IsCommutative(binary.OpCode) && right < left)
                (left, right) = (right, left);

            return new ExpressionKey(ExpressionKind.Binary, binary.Type, (int)binary.OpCode, false, left, right, 0, 2);
        }

        internal static ExpressionKey Unary(MirUnary unary, Func<MirValue, int> getId)
        {
            var operand = getId(unary.Operand);
            return new ExpressionKey(ExpressionKind.Unary, unary.Type, (int)unary.OpCode, false, operand, 0, 0, 1);
        }

        internal static ExpressionKey Compare(MirCompare compare, Func<MirValue, int> getId)
        {
            var left = getId(compare.Left);
            var right = getId(compare.Right);
            if (IsCommutative(compare.OpCode) && right < left)
                (left, right) = (right, left);

            return new ExpressionKey(ExpressionKind.Compare, compare.Type, (int)compare.OpCode, compare.Unsigned, left, right, 0, 2);
        }

        internal static ExpressionKey Convert(MirConvert convert, Func<MirValue, int> getId)
        {
            var operand = getId(convert.Value);
            return new ExpressionKey(ExpressionKind.Convert, convert.Type, (int)convert.ConversionKind, false, operand, 0, 0, 1);
        }

        internal static ExpressionKey ArrayLen(MirArrayLen arrayLen, Func<MirValue, int> getId)
        {
            return new ExpressionKey(ExpressionKind.ArrayLen, arrayLen.Type, 0, false, getId(arrayLen.Array), 0, 0, 1);
        }

        internal static ExpressionKey MapLen(MirMapLen mapLen, Func<MirValue, int> getId)
        {
            return new ExpressionKey(ExpressionKind.MapLen, mapLen.Type, 0, false, getId(mapLen.Map), 0, 0, 1);
        }

        internal static ExpressionKey MapHas(MirMapHas mapHas, Func<MirValue, int> getId)
        {
            var map = getId(mapHas.Map);
            var key = getId(mapHas.Key);
            return new ExpressionKey(ExpressionKind.MapHas, mapHas.Type, 0, false, map, key, 0, 2);
        }

        internal static ExpressionKey Concat(MirConcat concat, Func<MirValue, int> getId)
        {
            var left = getId(concat.Left);
            var right = getId(concat.Right);
            return new ExpressionKey(ExpressionKind.Concat, concat.Type, 0, false, left, right, 0, 2);
        }

        internal static ExpressionKey Slice(MirSlice slice, Func<MirValue, int> getId)
        {
            var value = getId(slice.Value);
            var start = getId(slice.Start);
            var length = getId(slice.Length);
            return new ExpressionKey(ExpressionKind.Slice, slice.Type, slice.IsBufferSlice ? 1 : 0, false, value, start, length, 3);
        }

        public bool Equals(ExpressionKey other)
        {
            return _kind == other._kind
                && Equals(_type, other._type)
                && _opCode == other._opCode
                && _flag == other._flag
                && _operandCount == other._operandCount
                && _operand0 == other._operand0
                && _operand1 == other._operand1
                && _operand2 == other._operand2;
        }

        public override bool Equals(object? obj) => obj is ExpressionKey other && Equals(other);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add((int)_kind);
            hash.Add(_type);
            hash.Add(_opCode);
            hash.Add(_flag);
            hash.Add(_operandCount);
            hash.Add(_operand0);
            hash.Add(_operand1);
            hash.Add(_operand2);
            return hash.ToHashCode();
        }
    }

    private enum ExpressionKind
    {
        Binary,
        Unary,
        Compare,
        Convert,
        ArrayLen,
        MapLen,
        MapHas,
        Concat,
        Slice
    }

    private readonly Dictionary<MirValue, int> _valueIds = new();
    private int _nextValueId;

    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var usage = MirAnalysis.Analyze(function);
        var dominators = MirDominatorAnalysis.Compute(function);

        _valueIds.Clear();
        _nextValueId = 0;

        foreach (var block in function.Blocks)
        {
            foreach (var phi in block.Phis)
                GetValueId(phi);
            foreach (var inst in block.Instructions)
                GetValueId(inst);
        }

        var table = new Dictionary<ExpressionKey, MirValue>();
        var changed = false;

        foreach (var block in function.Blocks)
        {
            var instructions = block.Instructions;
            for (int i = 0; i < instructions.Count; i++)
            {
                var inst = instructions[i];
                if (!IsEligible(inst))
                    continue;

                var key = CreateKey(inst);
                if (table.TryGetValue(key, out var existing))
                {
                    var defBlock = GetDefiningBlock(existing, usage);
                    if (defBlock is null || MirDominatorAnalysis.Dominates(defBlock, block, dominators))
                    {
                        instructions.RemoveAt(i);
                        MirValueRewriter.Replace(function, inst, existing);
                        i--;
                        changed = true;
                        continue;
                    }
                }

                table[key] = inst;
            }
        }

        return changed;
    }

    private ExpressionKey CreateKey(MirInst inst) => inst switch
    {
        MirBinary binary => ExpressionKey.Binary(binary, GetValueId),
        MirUnary unary => ExpressionKey.Unary(unary, GetValueId),
        MirCompare compare => ExpressionKey.Compare(compare, GetValueId),
        MirConvert convert => ExpressionKey.Convert(convert, GetValueId),
        MirArrayLen arrayLen => ExpressionKey.ArrayLen(arrayLen, GetValueId),
        MirMapLen mapLen => ExpressionKey.MapLen(mapLen, GetValueId),
        MirMapHas mapHas => ExpressionKey.MapHas(mapHas, GetValueId),
        MirConcat concat => ExpressionKey.Concat(concat, GetValueId),
        MirSlice slice => ExpressionKey.Slice(slice, GetValueId),
        _ => throw new NotSupportedException($"Unsupported instruction for GVN: {inst.GetType().Name}")
    };

    private int GetValueId(MirValue value)
    {
        if (!_valueIds.TryGetValue(value, out var id))
        {
            id = _nextValueId++;
            _valueIds[value] = id;
        }

        return id;
    }

    private static bool IsEligible(MirInst inst)
    {
        if (inst is null)
            return false;

        if (inst.Effect != MirEffect.None)
            return false;

        if (inst.ProducesMemoryToken || inst.ConsumesMemoryToken)
            return false;

        return inst switch
        {
            MirBinary => true,
            MirUnary => true,
            MirCompare => true,
            MirConvert => true,
            MirArrayLen => true,
            MirMapLen => true,
            MirMapHas => true,
            MirConcat => true,
            MirSlice => true,
            _ => false
        };
    }

    private static bool IsCommutative(MirBinary.Op op) => op switch
    {
        MirBinary.Op.Add => true,
        MirBinary.Op.Mul => true,
        MirBinary.Op.And => true,
        MirBinary.Op.Or => true,
        MirBinary.Op.Xor => true,
        _ => false
    };

    private static bool IsCommutative(MirCompare.Op op) => op switch
    {
        MirCompare.Op.Eq => true,
        MirCompare.Op.Ne => true,
        _ => false
    };

    private static MirBlock? GetDefiningBlock(MirValue value, MirUsageGraph usage)
    {
        if (value is MirPhi phi && usage.PhiBlocks.TryGetValue(phi, out var phiBlock))
            return phiBlock;

        if (value is MirInst inst && usage.InstructionBlocks.TryGetValue(inst, out var block))
            return block;

        return null;
    }
}
