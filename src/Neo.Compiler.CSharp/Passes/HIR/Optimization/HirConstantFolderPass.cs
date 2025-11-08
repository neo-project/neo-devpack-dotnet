using System;
using System.Numerics;

namespace Neo.Compiler.HIR.Optimization;

internal sealed class HirConstantFolderPass : IHirPass
{
    public bool Run(HirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var changed = false;

        foreach (var block in function.Blocks)
        {
            for (int i = 0; i < block.Instructions.Count; i++)
            {
                var instruction = block.Instructions[i];
                var replacement = TryFold(instruction);
                if (replacement is null)
                    continue;

                replacement.Span = instruction.Span;
                block.ReplaceInstruction(i, replacement);
                HirValueRewriter.Replace(function, instruction, replacement);
                changed = true;
            }
        }

        return changed;
    }

    private static HirInst? TryFold(HirInst instruction)
    {
        switch (instruction)
        {
            case HirAdd add when TryGetConstInt(add.Left, out var l) && TryGetConstInt(add.Right, out var r):
                return new HirConstInt(l + r);

            case HirSub sub when TryGetConstInt(sub.Left, out var l) && TryGetConstInt(sub.Right, out var r):
                return new HirConstInt(l - r);

            case HirMul mul when TryGetConstInt(mul.Left, out var l) && TryGetConstInt(mul.Right, out var r):
                return new HirConstInt(l * r);

            case HirDiv div when TryGetConstInt(div.Left, out var l) && TryGetConstInt(div.Right, out var r) && r != BigInteger.Zero:
                return new HirConstInt(l / r);

            case HirMod mod when TryGetConstInt(mod.Left, out var l) && TryGetConstInt(mod.Right, out var r) && r != BigInteger.Zero:
                return new HirConstInt(l % r);

            case HirBitAnd bitAnd when TryGetConstInt(bitAnd.Left, out var l) && TryGetConstInt(bitAnd.Right, out var r):
                return new HirConstInt(l & r);

            case HirBitOr bitOr when TryGetConstInt(bitOr.Left, out var l) && TryGetConstInt(bitOr.Right, out var r):
                return new HirConstInt(l | r);

            case HirBitXor bitXor when TryGetConstInt(bitXor.Left, out var l) && TryGetConstInt(bitXor.Right, out var r):
                return new HirConstInt(l ^ r);

            case HirShl shl when TryGetConstInt(shl.Left, out var l) && TryGetShiftAmount(shl.Right, out var shift) && shift >= 0:
                return new HirConstInt(l << shift);

            case HirShr shr when TryGetConstInt(shr.Left, out var l) && TryGetShiftAmount(shr.Right, out var shift) && shift >= 0:
                return new HirConstInt(l >> shift);

            case HirCompare cmp when !cmp.Unsigned && TryGetConstInt(cmp.Left, out var l) && TryGetConstInt(cmp.Right, out var r):
                return new HirConstBool(EvaluateCompare(cmp.Kind, l, r));

            case HirNot not when TryGetConstBool(not.Operand, out var operandBool):
                return new HirConstBool(!operandBool);

            case HirConvert convert when convert.Kind == HirConvKind.ToBool && TryFoldToBool(convert.Value, out var boolValue):
                return new HirConstBool(boolValue);

            case HirConcat concat when TryGetConstBytes(concat.Left, out var leftBytes, out var isBufferLeft) &&
                                       TryGetConstBytes(concat.Right, out var rightBytes, out var isBufferRight) &&
                                       !isBufferLeft && !isBufferRight:
                var combined = new byte[leftBytes.Length + rightBytes.Length];
                Buffer.BlockCopy(leftBytes, 0, combined, 0, leftBytes.Length);
                Buffer.BlockCopy(rightBytes, 0, combined, leftBytes.Length, rightBytes.Length);
                return new HirConstByteString(combined);
        }

        return null;
    }

    private static bool TryGetConstInt(HirValue value, out BigInteger constant)
    {
        if (value is HirConstInt constInt)
        {
            constant = constInt.Value;
            return true;
        }

        constant = default;
        return false;
    }

    private static bool TryGetConstBool(HirValue value, out bool constant)
    {
        if (value is HirConstBool constBool)
        {
            constant = constBool.Value;
            return true;
        }

        constant = default;
        return false;
    }

    private static bool TryGetConstBytes(HirValue value, out byte[] bytes, out bool isBuffer)
    {
        switch (value)
        {
            case HirConstByteString byteString:
                bytes = byteString.Value;
                isBuffer = false;
                return true;
            case HirConstBuffer buffer:
                bytes = buffer.Value;
                isBuffer = true;
                return true;
            default:
                bytes = Array.Empty<byte>();
                isBuffer = false;
                return false;
        }
    }

    private static bool TryGetShiftAmount(HirValue value, out int amount)
    {
        if (TryGetConstInt(value, out var shift) && shift >= int.MinValue && shift <= int.MaxValue)
        {
            amount = (int)shift;
            return true;
        }

        amount = default;
        return false;
    }

    private static bool TryFoldToBool(HirValue value, out bool result)
    {
        if (TryGetConstBool(value, out var boolValue))
        {
            result = boolValue;
            return true;
        }

        if (TryGetConstInt(value, out var intValue))
        {
            result = intValue != BigInteger.Zero;
            return true;
        }

        result = default;
        return false;
    }

    private static bool EvaluateCompare(HirCmpKind kind, BigInteger left, BigInteger right) => kind switch
    {
        HirCmpKind.Eq => left == right,
        HirCmpKind.Ne => left != right,
        HirCmpKind.Lt => left < right,
        HirCmpKind.Le => left <= right,
        HirCmpKind.Gt => left > right,
        HirCmpKind.Ge => left >= right,
        _ => false
    };
}
