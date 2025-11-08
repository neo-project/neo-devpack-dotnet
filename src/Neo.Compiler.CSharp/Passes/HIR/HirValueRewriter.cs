using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.HIR.Optimization;

internal static class HirValueRewriter
{
    internal static void Replace(HirFunction function, HirValue oldValue, HirValue newValue)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));
        if (oldValue is null)
            throw new ArgumentNullException(nameof(oldValue));
        if (newValue is null)
            throw new ArgumentNullException(nameof(newValue));
        if (ReferenceEquals(oldValue, newValue))
            return;

        foreach (var block in function.Blocks)
        {
            foreach (var phi in block.Phis)
                phi.ReplaceIncoming(oldValue, newValue);

            for (int i = 0; i < block.Instructions.Count; i++)
            {
                var instruction = block.Instructions[i];
                if (TryReplaceInInstruction(function, block, instruction, oldValue, newValue, out var replacement))
                {
                    replacement.Span = instruction.Span;
                    block.ReplaceInstruction(i, replacement);
                }
            }

            if (block.Terminator is { } terminator)
            {
                var replacementTerminator = ReplaceInTerminator(terminator, oldValue, newValue);
                if (!ReferenceEquals(replacementTerminator, terminator))
                {
                    replacementTerminator.Span = terminator.Span;
                    block.SetTerminator(replacementTerminator);
                }
            }
        }
    }

    private static bool TryReplaceInInstruction(
        HirFunction function,
        HirBlock block,
        HirInst instruction,
        HirValue oldValue,
        HirValue newValue,
        out HirInst replacement)
    {
        replacement = instruction;
        switch (instruction)
        {
            case HirBinaryInst binary:
                if (ReferenceEquals(binary.Left, oldValue))
                    binary.Left = newValue;
                if (ReferenceEquals(binary.Right, oldValue))
                    binary.Right = newValue;
                return false;

            case HirCheckedBinary checkedBinary:
                if (ReferenceEquals(checkedBinary.Left, oldValue))
                    checkedBinary.Left = newValue;
                if (ReferenceEquals(checkedBinary.Right, oldValue))
                    checkedBinary.Right = newValue;
                return false;

            case HirNeg neg:
                if (ReferenceEquals(neg.Operand, oldValue))
                    neg.Operand = newValue;
                return false;

            case HirNot not:
                if (ReferenceEquals(not.Operand, oldValue))
                    not.Operand = newValue;
                return false;

            case HirConvert convert:
                if (ReferenceEquals(convert.Value, oldValue))
                    convert.Value = newValue;
                return false;

            case HirStructGet structGet:
                if (ReferenceEquals(structGet.Object, oldValue))
                    structGet.Object = newValue;
                return false;

            case HirStructSet structSet:
                if (ReferenceEquals(structSet.Object, oldValue))
                    structSet.Object = newValue;
                if (ReferenceEquals(structSet.Value, oldValue))
                    structSet.Value = newValue;
                return false;

            case HirArrayLen arrayLen:
                if (ReferenceEquals(arrayLen.Array, oldValue))
                    arrayLen.Array = newValue;
                return false;

            case HirArrayGet arrayGet:
                if (ReferenceEquals(arrayGet.Array, oldValue))
                    arrayGet.Array = newValue;
                if (ReferenceEquals(arrayGet.Index, oldValue))
                    arrayGet.Index = newValue;
                return false;

            case HirArraySet arraySet:
                if (ReferenceEquals(arraySet.Array, oldValue))
                    arraySet.Array = newValue;
                if (ReferenceEquals(arraySet.Index, oldValue))
                    arraySet.Index = newValue;
                if (ReferenceEquals(arraySet.Value, oldValue))
                    arraySet.Value = newValue;
                return false;

            case HirMapGet mapGet:
                if (ReferenceEquals(mapGet.Map, oldValue))
                    mapGet.Map = newValue;
                if (ReferenceEquals(mapGet.Key, oldValue))
                    mapGet.Key = newValue;
                return false;

            case HirMapSet mapSet:
                if (ReferenceEquals(mapSet.Map, oldValue))
                    mapSet.Map = newValue;
                if (ReferenceEquals(mapSet.Key, oldValue))
                    mapSet.Key = newValue;
                if (ReferenceEquals(mapSet.Value, oldValue))
                    mapSet.Value = newValue;
                return false;

            case HirMapDelete mapDelete:
                if (ReferenceEquals(mapDelete.Map, oldValue))
                    mapDelete.Map = newValue;
                if (ReferenceEquals(mapDelete.Key, oldValue))
                    mapDelete.Key = newValue;
                return false;

            case HirMapHas mapHas:
                if (ReferenceEquals(mapHas.Map, oldValue))
                    mapHas.Map = newValue;
                if (ReferenceEquals(mapHas.Key, oldValue))
                    mapHas.Key = newValue;
                return false;

            case HirMapLen mapLen:
                if (ReferenceEquals(mapLen.Map, oldValue))
                    mapLen.Map = newValue;
                return false;

            case HirNullCheck nullCheck:
                if (ReferenceEquals(nullCheck.Reference, oldValue))
                    nullCheck.Reference = newValue;
                return false;

            case HirBoundsCheck boundsCheck:
                if (ReferenceEquals(boundsCheck.Index, oldValue))
                    boundsCheck.Index = newValue;
                if (ReferenceEquals(boundsCheck.Length, oldValue))
                    boundsCheck.Length = newValue;
                return false;

            case HirBufferNew bufferNew:
                if (ReferenceEquals(bufferNew.Length, oldValue))
                    bufferNew.Length = newValue;
                return false;

            case HirBufferSet bufferSet:
                if (ReferenceEquals(bufferSet.Buffer, oldValue))
                    bufferSet.Buffer = newValue;
                if (ReferenceEquals(bufferSet.Index, oldValue))
                    bufferSet.Index = newValue;
                if (ReferenceEquals(bufferSet.Byte, oldValue))
                    bufferSet.Byte = newValue;
                return false;

            case HirBufferCopy bufferCopy:
                if (ReferenceEquals(bufferCopy.Destination, oldValue))
                    bufferCopy.Destination = newValue;
                if (ReferenceEquals(bufferCopy.Source, oldValue))
                    bufferCopy.Source = newValue;
                if (ReferenceEquals(bufferCopy.DestinationOffset, oldValue))
                    bufferCopy.DestinationOffset = newValue;
                if (ReferenceEquals(bufferCopy.SourceOffset, oldValue))
                    bufferCopy.SourceOffset = newValue;
                if (ReferenceEquals(bufferCopy.Length, oldValue))
                    bufferCopy.Length = newValue;
                return false;

            case HirConcat concat:
                if (ReferenceEquals(concat.Left, oldValue))
                    concat.Left = newValue;
                if (ReferenceEquals(concat.Right, oldValue))
                    concat.Right = newValue;
                return false;

            case HirSlice slice:
                if (ReferenceEquals(slice.Value, oldValue))
                    slice.Value = newValue;
                if (ReferenceEquals(slice.Start, oldValue))
                    slice.Start = newValue;
                if (ReferenceEquals(slice.Length, oldValue))
                    slice.Length = newValue;
                return false;

            case HirCall call:
                {
                    var arguments = ReplaceImmutableList(call.Arguments, oldValue, newValue, out var changed);
                    if (!changed)
                        return false;

                    replacement = new HirCall(call.Callee, arguments, call.Type, call.IsStatic, call.Semantics)
                    {
                        Span = call.Span
                    };
                    return true;
                }

            case HirIntrinsicCall intrinsic:
                {
                    var arguments = ReplaceImmutableList(intrinsic.Arguments, oldValue, newValue, out var changed);
                    if (!changed)
                        return false;

                    replacement = new HirIntrinsicCall(intrinsic.Metadata.Category, intrinsic.Metadata.Name, arguments, intrinsic.Metadata)
                    {
                        Span = intrinsic.Span
                    };
                    return true;
                }

            case HirNewObject newObject:
                {
                    var arguments = ReplaceImmutableList(newObject.Arguments, oldValue, newValue, out var changed);
                    if (!changed)
                        return false;

                    replacement = new HirNewObject(newObject.TypeName, arguments, (HirStructType)newObject.Type)
                    {
                        Span = newObject.Span
                    };
                    return true;
                }

            default:
                return false;
        }
    }

    private static HirTerminator ReplaceInTerminator(HirTerminator terminator, HirValue oldValue, HirValue newValue)
    {
        switch (terminator)
        {
            case HirReturn ret when ReferenceEquals(ret.Value, oldValue):
                return new HirReturn(newValue) { Span = ret.Span };
            case HirThrow when terminator is HirThrow { Exception: { } exception } throwTerm && ReferenceEquals(exception, oldValue):
                return new HirThrow(newValue) { Span = throwTerm.Span };
            case HirConditionalBranch cond when ReferenceEquals(cond.Condition, oldValue):
                return new HirConditionalBranch(newValue, cond.TrueBlock, cond.FalseBlock) { Span = cond.Span };
            case HirSwitch @switch when ReferenceEquals(@switch.Key, oldValue):
                return new HirSwitch(newValue, @switch.Cases, @switch.DefaultTarget) { Span = @switch.Span };
            default:
                return terminator;
        }
    }

    private static IReadOnlyList<HirValue> ReplaceImmutableList(IReadOnlyList<HirValue> values, HirValue oldValue, HirValue newValue, out bool changed)
    {
        changed = false;
        var result = new HirValue[values.Count];
        for (int i = 0; i < values.Count; i++)
        {
            var element = values[i];
            if (ReferenceEquals(element, oldValue))
            {
                result[i] = newValue;
                changed = true;
            }
            else
            {
                result[i] = element;
            }
        }

        return changed ? result : values;
    }
}
