using System;
using System.Collections.Generic;

namespace Neo.Compiler.MIR.Optimization;

/// <summary>
/// Converts <c>MirCompare</c> followed by <c>MirCondBranch</c> into a single <c>MirCompareBranch</c> terminator
/// when the compare result has no other uses. This enables direct emission of NeoVM compare-branch opcodes.
/// </summary>
internal sealed class MirCompareBranchLoweringPass : IMirPass
{
    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var changed = false;

        foreach (var block in function.Blocks)
        {
            if (block.Terminator is not MirCondBranch cond)
                continue;
            if (cond.Condition is not MirCompare compare)
                continue;
            if (compare.Unsigned)
                continue;
            if (block.Instructions.Count == 0 || !ReferenceEquals(block.Instructions[^1], compare))
                continue;
            if (!HasSingleUse(function, compare, cond))
                continue;

            // Remove the compare instruction (now folded into terminator)
            block.Instructions.RemoveAt(block.Instructions.Count - 1);

            block.Terminator = new MirCompareBranch(
                compare.OpCode,
                compare.Left,
                compare.Right,
                cond.TrueTarget,
                cond.FalseTarget,
                compare.Unsigned)
            {
                Span = cond.Span ?? compare.Span
            };

            changed = true;
        }

        return changed;
    }

    private static bool HasSingleUse(MirFunction function, MirCompare compare, MirCondBranch cond)
    {
        foreach (var block in function.Blocks)
        {
            foreach (var phi in block.Phis)
            {
                foreach (var (_, value) in phi.Inputs)
                {
                    if (ReferenceEquals(value, compare))
                        return false;
                }
            }

            foreach (var inst in block.Instructions)
            {
                if (ReferenceEquals(inst, compare))
                    continue;

                foreach (var operand in EnumerateOperands(inst))
                {
                    if (ReferenceEquals(operand, compare))
                        return false;
                }
            }

            if (!ReferenceEquals(block.Terminator, cond))
            {
                foreach (var operand in EnumerateTerminatorOperands(block.Terminator))
                {
                    if (ReferenceEquals(operand, compare))
                        return false;
                }
            }
        }

        return true;
    }

    private static IEnumerable<MirValue> EnumerateOperands(MirInst inst)
    {
        switch (inst)
        {
            case MirUnary unary:
                yield return unary.Operand;
                break;
            case MirConvert convert:
                yield return convert.Value;
                break;
            case MirBinary binary:
                yield return binary.Left;
                yield return binary.Right;
                break;
            case MirCompare compare:
                yield return compare.Left;
                yield return compare.Right;
                break;
            case MirStructPack pack:
                foreach (var field in pack.Fields)
                    yield return field;
                break;
            case MirStructGet get:
                yield return get.Object;
                break;
            case MirStructSet set:
                yield return set.Object;
                yield return set.Value;
                break;
            case MirArrayNew arrayNew:
                yield return arrayNew.Length;
                break;
            case MirArrayLen arrayLen:
                yield return arrayLen.Array;
                break;
            case MirArrayGet arrayGet:
                yield return arrayGet.Array;
                yield return arrayGet.Index;
                break;
            case MirArraySet arraySet:
                yield return arraySet.Array;
                yield return arraySet.Index;
                yield return arraySet.Value;
                break;
            case MirMapNew:
                break;
            case MirMapLen mapLen:
                yield return mapLen.Map;
                break;
            case MirMapGet mapGet:
                yield return mapGet.Map;
                yield return mapGet.Key;
                break;
            case MirMapSet mapSet:
                yield return mapSet.Map;
                yield return mapSet.Key;
                yield return mapSet.Value;
                break;
            case MirMapHas mapHas:
                yield return mapHas.Map;
                yield return mapHas.Key;
                break;
            case MirMapDelete mapDelete:
                yield return mapDelete.Map;
                yield return mapDelete.Key;
                break;
            case MirConcat concat:
                yield return concat.Left;
                yield return concat.Right;
                break;
            case MirSlice slice:
                yield return slice.Value;
                yield return slice.Start;
                yield return slice.Length;
                break;
            case MirBufferNew bufferNew:
                yield return bufferNew.Length;
                break;
            case MirBufferSet bufferSet:
                yield return bufferSet.Buffer;
                yield return bufferSet.Index;
                yield return bufferSet.Byte;
                break;
            case MirBufferCopy bufferCopy:
                yield return bufferCopy.Destination;
                yield return bufferCopy.Source;
                yield return bufferCopy.DestinationOffset;
                yield return bufferCopy.SourceOffset;
                yield return bufferCopy.Length;
                break;
            case MirModMul modMul:
                yield return modMul.Left;
                yield return modMul.Right;
                yield return modMul.Modulus;
                break;
            case MirModPow modPow:
                yield return modPow.Value;
                yield return modPow.Exponent;
                yield return modPow.Modulus;
                break;
            case MirCall call:
                foreach (var arg in call.Arguments)
                    yield return arg;
                break;
            case MirPointerCall pointerCall:
                if (pointerCall.Pointer is not null)
                    yield return pointerCall.Pointer;
                foreach (var arg in pointerCall.Arguments)
                    yield return arg;
                break;
            case MirSyscall syscall:
                foreach (var arg in syscall.Arguments)
                    yield return arg;
                break;
            case MirGuardNull guardNull:
                yield return guardNull.Reference;
                break;
            case MirGuardBounds guardBounds:
                yield return guardBounds.Index;
                yield return guardBounds.Length;
                break;
            case MirCheckedAdd checkedAdd:
                yield return checkedAdd.Left;
                yield return checkedAdd.Right;
                break;
            case MirCheckedSub checkedSub:
                yield return checkedSub.Left;
                yield return checkedSub.Right;
                break;
            case MirCheckedMul checkedMul:
                yield return checkedMul.Left;
                yield return checkedMul.Right;
                break;
        }
    }

    private static IEnumerable<MirValue> EnumerateTerminatorOperands(MirTerminator? terminator)
    {
        switch (terminator)
        {
            case MirReturn ret when ret.Value is not null:
                yield return ret.Value;
                break;
            case MirCondBranch cond:
                yield return cond.Condition;
                break;
            case MirSwitch @switch:
                yield return @switch.Key;
                break;
            case MirCompareBranch cmpBranch:
                yield return cmpBranch.Left;
                yield return cmpBranch.Right;
                break;
        }
    }
}
