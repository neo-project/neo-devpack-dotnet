using System;
using System.Collections.Generic;
using Neo.Compiler.HIR;

namespace Neo.Compiler.HIR.Optimization;

internal static class HirAnalysis
{
    public static HirUsageGraph Analyze(HirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var useCounts = new Dictionary<HirValue, int>();
        var instructionBlocks = new Dictionary<HirInst, HirBlock>();
        var phiBlocks = new Dictionary<HirPhi, HirBlock>();

        foreach (var block in function.Blocks)
        {
            foreach (var phi in block.Phis)
            {
                phiBlocks[phi] = block;
                EnsureEntry(useCounts, phi);
                foreach (var incoming in phi.Inputs)
                    Increment(useCounts, incoming.Value);
            }

            foreach (var inst in block.Instructions)
            {
                instructionBlocks[inst] = block;
                EnsureEntry(useCounts, inst);
                foreach (var operand in GetOperands(inst))
                    Increment(useCounts, operand);
            }

            if (block.Terminator is { } terminator)
            {
                foreach (var operand in GetOperands(terminator))
                    Increment(useCounts, operand);
            }
        }

        return new HirUsageGraph(useCounts, instructionBlocks, phiBlocks);
    }

    public static IEnumerable<HirValue> GetOperands(HirInst inst)
    {
        switch (inst)
        {
            case HirBinaryInst binary:
                yield return binary.Left;
                yield return binary.Right;
                yield break;

            case HirCheckedBinary checkedBinary:
                yield return checkedBinary.Left;
                yield return checkedBinary.Right;
                yield break;

            case HirNeg neg:
                yield return neg.Operand;
                yield break;

            case HirNot not:
                yield return not.Operand;
                yield break;

            case HirConvert convert:
                yield return convert.Value;
                yield break;

            case HirStructGet structGet:
                yield return structGet.Object;
                yield break;

            case HirStructSet structSet:
                yield return structSet.Object;
                yield return structSet.Value;
                yield break;

            case HirNewStruct newStruct:
                foreach (var field in newStruct.Fields)
                    yield return field;
                yield break;

            case HirArrayLen arrayLen:
                yield return arrayLen.Array;
                yield break;

            case HirArrayGet arrayGet:
                yield return arrayGet.Array;
                yield return arrayGet.Index;
                yield break;

            case HirArraySet arraySet:
                yield return arraySet.Array;
                yield return arraySet.Index;
                yield return arraySet.Value;
                yield break;

            case HirMapGet mapGet:
                yield return mapGet.Map;
                yield return mapGet.Key;
                yield break;

            case HirMapSet mapSet:
                yield return mapSet.Map;
                yield return mapSet.Key;
                yield return mapSet.Value;
                yield break;

            case HirMapDelete mapDelete:
                yield return mapDelete.Map;
                yield return mapDelete.Key;
                yield break;

            case HirMapHas mapHas:
                yield return mapHas.Map;
                yield return mapHas.Key;
                yield break;

            case HirMapLen mapLen:
                yield return mapLen.Map;
                yield break;

            case HirNullCheck nullCheck:
                yield return nullCheck.Reference;
                yield break;

            case HirBoundsCheck boundsCheck:
                yield return boundsCheck.Index;
                yield return boundsCheck.Length;
                yield break;

            case HirBufferNew bufferNew:
                yield return bufferNew.Length;
                yield break;

            case HirBufferSet bufferSet:
                yield return bufferSet.Buffer;
                yield return bufferSet.Index;
                yield return bufferSet.Byte;
                yield break;

            case HirBufferCopy bufferCopy:
                yield return bufferCopy.Destination;
                yield return bufferCopy.Source;
                yield return bufferCopy.DestinationOffset;
                yield return bufferCopy.SourceOffset;
                yield return bufferCopy.Length;
                yield break;

            case HirConcat concat:
                yield return concat.Left;
                yield return concat.Right;
                yield break;

            case HirSlice slice:
                yield return slice.Value;
                yield return slice.Start;
                yield return slice.Length;
                yield break;

            case HirCall call:
                foreach (var arg in call.Arguments)
                    yield return arg;
                yield break;

            case HirIntrinsicCall intrinsic:
                foreach (var arg in intrinsic.Arguments)
                    yield return arg;
                yield break;

            case HirNewObject newObject:
                foreach (var arg in newObject.Arguments)
                    yield return arg;
                yield break;

            case HirStoreLocal storeLocal:
                yield return storeLocal.Value;
                yield break;

            case HirStoreArgument storeArgument:
                yield return storeArgument.Value;
                yield break;

            case HirStoreStaticField storeStatic:
                yield return storeStatic.Value;
                yield break;

            default:
                yield break;
        }
    }

    public static IEnumerable<HirValue> GetOperands(HirTerminator terminator)
    {
        switch (terminator)
        {
            case HirConditionalBranch cond:
                yield return cond.Condition;
                yield break;
            case HirSwitch @switch:
                yield return @switch.Key;
                yield break;
            case HirReturn ret when ret.Value is { } value:
                yield return value;
                yield break;
            case HirThrow throwTerm when throwTerm.Exception is { } ex:
                yield return ex;
                yield break;
            default:
                yield break;
        }
    }

    private static void EnsureEntry(Dictionary<HirValue, int> useCounts, HirValue value)
    {
        if (!useCounts.ContainsKey(value))
            useCounts[value] = 0;
    }

    private static void Increment(Dictionary<HirValue, int> useCounts, HirValue value)
    {
        if (value is null)
            return;

        if (useCounts.TryGetValue(value, out var count))
            useCounts[value] = count + 1;
        else
            useCounts[value] = 1;
    }
}
