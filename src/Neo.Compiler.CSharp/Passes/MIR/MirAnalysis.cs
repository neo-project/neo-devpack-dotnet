using System;
using System.Collections.Generic;

namespace Neo.Compiler.MIR.Optimization;

internal static class MirAnalysis
{
    internal static MirUsageGraph Analyze(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var useCounts = new Dictionary<MirValue, int>();
        var instBlocks = new Dictionary<MirInst, MirBlock>();
        var phiBlocks = new Dictionary<MirPhi, MirBlock>();

        foreach (var block in function.Blocks)
        {
            foreach (var phi in block.Phis)
            {
                phiBlocks[phi] = block;
                EnsureEntry(useCounts, phi);
                foreach (var (pred, value) in phi.Inputs)
                {
                    EnsureEntry(useCounts, value);
                    Increment(useCounts, value);
                }
            }

            foreach (var inst in block.Instructions)
            {
                instBlocks[inst] = block;
                EnsureEntry(useCounts, inst);
                foreach (var operand in GetOperands(inst))
                {
                    EnsureEntry(useCounts, operand);
                    Increment(useCounts, operand);
                }
            }

            if (block.Terminator is { } terminator)
            {
                foreach (var operand in GetOperands(terminator))
                {
                    EnsureEntry(useCounts, operand);
                    Increment(useCounts, operand);
                }
            }
        }

        return new MirUsageGraph(useCounts, instBlocks, phiBlocks);
    }

    internal static IEnumerable<MirValue> GetOperands(MirInst inst)
    {
        switch (inst)
        {
            case MirBinary binary:
                yield return binary.Left;
                yield return binary.Right;
                yield break;

            case MirUnary unary:
                yield return unary.Operand;
                yield break;

            case MirCompare compare:
                yield return compare.Left;
                yield return compare.Right;
                yield break;

            case MirConvert convert:
                yield return convert.Value;
                yield break;

            case MirStructPack pack:
                foreach (var field in pack.Fields)
                    yield return field;
                yield break;

            case MirStructGet structGet:
                yield return structGet.Object;
                yield break;

            case MirStructSet structSet:
                yield return structSet.Object;
                yield return structSet.Value;
                yield break;

            case MirArrayNew arrayNew:
                yield return arrayNew.Length;
                yield break;

            case MirArrayGet arrayGet:
                yield return arrayGet.Array;
                yield return arrayGet.Index;
                yield break;

            case MirArraySet arraySet:
                yield return arraySet.Array;
                yield return arraySet.Index;
                yield return arraySet.Value;
                yield break;

            case MirMapGet mapGet:
                yield return mapGet.Map;
                yield return mapGet.Key;
                yield break;

            case MirMapSet mapSet:
                yield return mapSet.Map;
                yield return mapSet.Key;
                yield return mapSet.Value;
                yield break;

            case MirMapDelete mapDelete:
                yield return mapDelete.Map;
                yield return mapDelete.Key;
                yield break;

            case MirMapHas mapHas:
                yield return mapHas.Map;
                yield return mapHas.Key;
                yield break;

            case MirMapLen mapLen:
                yield return mapLen.Map;
                yield break;

            case MirGuardNull guardNull:
                yield return guardNull.Reference;
                yield break;

            case MirGuardBounds guardBounds:
                yield return guardBounds.Index;
                yield return guardBounds.Length;
                yield break;

            case MirCheckedAdd checkedAdd:
                yield return checkedAdd.Left;
                yield return checkedAdd.Right;
                yield break;

            case MirCheckedSub checkedSub:
                yield return checkedSub.Left;
                yield return checkedSub.Right;
                yield break;

            case MirCheckedMul checkedMul:
                yield return checkedMul.Left;
                yield return checkedMul.Right;
                yield break;

            case MirCall call:
                foreach (var arg in call.Arguments)
                    yield return arg;
                yield break;

            case MirPointerCall pointerCall:
                if (pointerCall.Pointer is not null)
                    yield return pointerCall.Pointer;
                foreach (var arg in pointerCall.Arguments)
                    yield return arg;
                yield break;

            case MirSyscall syscall:
                foreach (var arg in syscall.Arguments)
                    yield return arg;
                yield break;

            case MirSlice slice:
                yield return slice.Value;
                yield return slice.Start;
                yield return slice.Length;
                yield break;

            case MirConcat concat:
                yield return concat.Left;
                yield return concat.Right;
                yield break;

            case MirBufferNew bufferNew:
                yield return bufferNew.Length;
                yield break;

            case MirBufferSet bufferSet:
                yield return bufferSet.Buffer;
                yield return bufferSet.Index;
                yield return bufferSet.Byte;
                yield break;

            case MirBufferCopy bufferCopy:
                yield return bufferCopy.Destination;
                yield return bufferCopy.Source;
                yield return bufferCopy.DestinationOffset;
                yield return bufferCopy.SourceOffset;
                yield return bufferCopy.Length;
                yield break;

            case MirStaticFieldStore staticStore:
                yield return staticStore.Value;
                yield break;

            case MirStoreLocal storeLocal:
                yield return storeLocal.Value;
                yield break;
        }
    }

    internal static IEnumerable<MirValue> GetOperands(MirTerminator terminator)
    {
        switch (terminator)
        {
            case MirCondBranch cond:
                yield return cond.Condition;
                yield break;
            case MirCompareBranch cmp:
                yield return cmp.Left;
                yield return cmp.Right;
                yield break;
            case MirSwitch @switch:
                yield return @switch.Key;
                yield break;
            case MirReturn ret when ret.Value is { } value:
                yield return value;
                yield break;
            case MirAbortMsg abortMsg:
                yield return abortMsg.Message;
                yield break;
        }
    }

    private static void EnsureEntry(Dictionary<MirValue, int> useCounts, MirValue value)
    {
        if (!useCounts.ContainsKey(value))
            useCounts[value] = 0;
    }

    private static void Increment(Dictionary<MirValue, int> useCounts, MirValue value)
    {
        if (value is null)
            return;
        if (useCounts.TryGetValue(value, out var count))
            useCounts[value] = count + 1;
        else
            useCounts[value] = 1;
    }
}
