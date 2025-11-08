using System;
using System.Numerics;

namespace Neo.Compiler.MIR.Optimization;

internal sealed class MirConstantFolderPass : IMirPass
{
    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var changed = false;

        foreach (var block in function.Blocks)
        {
            for (int i = 0; i < block.Instructions.Count; i++)
            {
                var inst = block.Instructions[i];
                if (!TryFold(inst, out var replacement))
                    continue;

                replacement.Span = inst.Span;
                block.ReplaceInstruction(i, replacement);
                MirValueRewriter.Replace(function, inst, replacement);
                changed = true;
            }
        }

        return changed;
    }

    private static bool TryFold(MirInst inst, out MirInst replacement)
    {
        switch (inst)
        {
            case MirBinary binary when binary.Effect == MirEffect.None && TryGetInt(binary.Left, out var l) && TryGetInt(binary.Right, out var r):
                {
                    var folded = FoldBinary(binary, l, r);
                    if (folded is not null)
                    {
                        replacement = folded;
                        return true;
                    }
                    break;
                }

            case MirCompare compare when TryGetInt(compare.Left, out var l) && TryGetInt(compare.Right, out var r):
                replacement = new MirConstBool(EvaluateCompare(compare.OpCode, l, r));
                return true;

            case MirUnary unary when unary.OpCode == MirUnary.Op.Neg && TryGetInt(unary.Operand, out var operand):
                replacement = new MirConstInt(-operand);
                return true;

            case MirConvert convert when convert.ConversionKind == MirConvert.Kind.ToBool && TryFoldToBool(convert.Value, out var boolValue):
                replacement = new MirConstBool(boolValue);
                return true;
        }

        replacement = inst;
        return false;
    }

    private static MirInst? FoldBinary(MirBinary binary, BigInteger left, BigInteger right)
    {
        return binary.OpCode switch
        {
            MirBinary.Op.Add => new MirConstInt(left + right),
            MirBinary.Op.Sub => new MirConstInt(left - right),
            MirBinary.Op.Mul => new MirConstInt(left * right),
            MirBinary.Op.Div when right != BigInteger.Zero => new MirConstInt(left / right),
            MirBinary.Op.Mod when right != BigInteger.Zero => new MirConstInt(left % right),
            MirBinary.Op.And => new MirConstInt(left & right),
            MirBinary.Op.Or => new MirConstInt(left | right),
            MirBinary.Op.Xor => new MirConstInt(left ^ right),
            MirBinary.Op.Shl when TryGetShiftAmount(right, out var shift) => new MirConstInt(left << shift),
            MirBinary.Op.Shr when TryGetShiftAmount(right, out var shift) => new MirConstInt(left >> shift),
            _ => null
        };
    }

    private static bool TryGetInt(MirValue value, out BigInteger constant)
    {
        if (value is MirConstInt constInt)
        {
            constant = constInt.Value;
            return true;
        }

        constant = default;
        return false;
    }

    private static bool TryFoldToBool(MirValue value, out bool result)
    {
        switch (value)
        {
            case MirConstBool constBool:
                result = constBool.Value;
                return true;
            case MirConstInt constInt:
                result = constInt.Value != BigInteger.Zero;
                return true;
            default:
                result = false;
                return false;
        }
    }

    private static bool TryGetShiftAmount(BigInteger value, out int shift)
    {
        if (value >= int.MinValue && value <= int.MaxValue)
        {
            shift = (int)value;
            return true;
        }

        shift = default;
        return false;
    }

    private static bool EvaluateCompare(MirCompare.Op op, BigInteger left, BigInteger right) => op switch
    {
        MirCompare.Op.Eq => left == right,
        MirCompare.Op.Ne => left != right,
        MirCompare.Op.Lt => left < right,
        MirCompare.Op.Le => left <= right,
        MirCompare.Op.Gt => left > right,
        MirCompare.Op.Ge => left >= right,
        _ => false
    };
}
