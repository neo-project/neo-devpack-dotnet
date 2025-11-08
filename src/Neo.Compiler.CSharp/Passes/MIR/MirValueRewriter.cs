using System;
using System.Collections.Generic;

namespace Neo.Compiler.MIR.Optimization;

internal static class MirValueRewriter
{
    public static void Replace(MirFunction function, MirValue oldValue, MirValue newValue)
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

            var instructions = block.Instructions;
            for (int i = 0; i < instructions.Count; i++)
            {
                var inst = instructions[i];
                var replacement = ReplaceInInstruction(inst, oldValue, newValue);
                if (!ReferenceEquals(replacement, inst))
                {
                    PreserveMemoryTokens(inst, replacement);
                    replacement.Span = inst.Span;
                    instructions[i] = replacement;
                }
            }

            if (block.Terminator is { } terminator)
            {
                var replacement = ReplaceInTerminator(terminator, oldValue, newValue);
                if (!ReferenceEquals(replacement, terminator))
                {
                    replacement.Span = terminator.Span;
                    block.Terminator = replacement;
                }
            }
        }
    }

    private static void PreserveMemoryTokens(MirInst original, MirInst replacement)
    {
        if (original is null || replacement is null)
            return;

        replacement.CopyMemoryTokenStateFrom(original);
    }

    private static MirInst ReplaceInInstruction(MirInst instruction, MirValue oldValue, MirValue newValue)
    {
        switch (instruction)
        {
            case MirBinary binary:
                {
                    var left = ReferenceEquals(binary.Left, oldValue) ? newValue : binary.Left;
                    var right = ReferenceEquals(binary.Right, oldValue) ? newValue : binary.Right;
                    if (ReferenceEquals(left, binary.Left) && ReferenceEquals(right, binary.Right))
                        return instruction;
                    return new MirBinary(binary.OpCode, left, right, binary.Type) { Span = instruction.Span };
                }

            case MirCompare compare:
                {
                    var left = ReferenceEquals(compare.Left, oldValue) ? newValue : compare.Left;
                    var right = ReferenceEquals(compare.Right, oldValue) ? newValue : compare.Right;
                    if (ReferenceEquals(left, compare.Left) && ReferenceEquals(right, compare.Right))
                        return instruction;
                    return new MirCompare(compare.OpCode, left, right, compare.Unsigned) { Span = instruction.Span };
                }

            case MirUnary unary:
                if (!ReferenceEquals(unary.Operand, oldValue))
                    return instruction;
                return new MirUnary(unary.OpCode, newValue, unary.Type) { Span = instruction.Span };

            case MirConvert convert:
                if (!ReferenceEquals(convert.Value, oldValue))
                    return instruction;
                return new MirConvert(convert.ConversionKind, newValue, convert.Type) { Span = instruction.Span };

            case MirStructGet structGet:
                if (!ReferenceEquals(structGet.Object, oldValue))
                    return instruction;
                return new MirStructGet(newValue, structGet.Index, structGet.Type) { Span = instruction.Span };

            case MirStructSet structSet:
                {
                    var obj = ReferenceEquals(structSet.Object, oldValue) ? newValue : structSet.Object;
                    var val = ReferenceEquals(structSet.Value, oldValue) ? newValue : structSet.Value;
                    if (ReferenceEquals(obj, structSet.Object) && ReferenceEquals(val, structSet.Value))
                        return instruction;
                    return new MirStructSet(obj, structSet.Index, val, (MirStructType)structSet.Type) { Span = instruction.Span };
                }

            case MirStructPack structPack:
                if (!ReplaceOperands(structPack.Fields, oldValue, newValue, out var newFields))
                    return instruction;
                return new MirStructPack(newFields, (MirStructType)structPack.Type) { Span = instruction.Span };

            case MirArrayGet arrayGet:
                {
                    var array = ReferenceEquals(arrayGet.Array, oldValue) ? newValue : arrayGet.Array;
                    var index = ReferenceEquals(arrayGet.Index, oldValue) ? newValue : arrayGet.Index;
                    if (ReferenceEquals(array, arrayGet.Array) && ReferenceEquals(index, arrayGet.Index))
                        return instruction;
                    return new MirArrayGet(array, index, arrayGet.Type) { Span = instruction.Span };
                }

            case MirArraySet arraySet:
            {
                var array = ReferenceEquals(arraySet.Array, oldValue) ? newValue : arraySet.Array;
                var index = ReferenceEquals(arraySet.Index, oldValue) ? newValue : arraySet.Index;
                var value = ReferenceEquals(arraySet.Value, oldValue) ? newValue : arraySet.Value;
                if (!ReferenceEquals(array, arraySet.Array) || !ReferenceEquals(index, arraySet.Index) || !ReferenceEquals(value, arraySet.Value))
                    arraySet.Update(array, index, value);
                return instruction;
            }

            case MirCall call:
                {
                    if (!ReplaceOperands(call.Arguments, oldValue, newValue, out var callArgs))
                        return instruction;
                    return new MirCall(call.Callee, callArgs, call.Type, call.IsPure, call.IsTailCall) { Span = instruction.Span };
                }

            case MirPointerCall pointerCall:
                {
                    var pointer = ReferenceEquals(pointerCall.Pointer, oldValue) ? newValue : pointerCall.Pointer;
                    var pointerChanged = !ReferenceEquals(pointer, pointerCall.Pointer);

                    if (!ReplaceOperands(pointerCall.Arguments, oldValue, newValue, out var pointerArgs))
                    {
                        if (!pointerChanged)
                            return instruction;
                        return new MirPointerCall(pointer, pointerCall.Arguments, pointerCall.Type, pointerCall.IsPure, pointerCall.IsTailCall, pointerCall.CallTableIndex)
                        { Span = instruction.Span };
                    }

                    return new MirPointerCall(pointer, pointerArgs, pointerCall.Type, pointerCall.IsPure, pointerCall.IsTailCall, pointerCall.CallTableIndex)
                    { Span = instruction.Span };
                }

            case MirSyscall syscall:
                if (!ReplaceOperands(syscall.Arguments, oldValue, newValue, out var syscallArgs))
                    return instruction;
                return new MirSyscall(syscall.Category, syscall.Name, syscallArgs, syscall.Type, syscall.EffectOverride, syscall.GasHint) { Span = instruction.Span };

            case MirSlice slice:
                {
                    var value = ReferenceEquals(slice.Value, oldValue) ? newValue : slice.Value;
                    var start = ReferenceEquals(slice.Start, oldValue) ? newValue : slice.Start;
                    var length = ReferenceEquals(slice.Length, oldValue) ? newValue : slice.Length;
                    if (ReferenceEquals(value, slice.Value) && ReferenceEquals(start, slice.Start) && ReferenceEquals(length, slice.Length))
                        return instruction;
                    return new MirSlice(value, start, length, slice.IsBufferSlice) { Span = instruction.Span };
                }

            case MirConcat concat:
                {
                    var left = ReferenceEquals(concat.Left, oldValue) ? newValue : concat.Left;
                    var right = ReferenceEquals(concat.Right, oldValue) ? newValue : concat.Right;
                    if (ReferenceEquals(left, concat.Left) && ReferenceEquals(right, concat.Right))
                        return instruction;
                    return new MirConcat(left, right) { Span = instruction.Span };
                }

            case MirBufferNew bufferNew:
                if (!ReferenceEquals(bufferNew.Length, oldValue))
                    return instruction;
                return new MirBufferNew(newValue) { Span = instruction.Span };

            case MirBufferSet bufferSet:
                {
                    var buffer = ReferenceEquals(bufferSet.Buffer, oldValue) ? newValue : bufferSet.Buffer;
                    var index = ReferenceEquals(bufferSet.Index, oldValue) ? newValue : bufferSet.Index;
                    var byteVal = ReferenceEquals(bufferSet.Byte, oldValue) ? newValue : bufferSet.Byte;
                    if (ReferenceEquals(buffer, bufferSet.Buffer) && ReferenceEquals(index, bufferSet.Index) && ReferenceEquals(byteVal, bufferSet.Byte))
                        return instruction;
                    return new MirBufferSet(buffer, index, byteVal) { Span = instruction.Span };
                }

            case MirBufferCopy bufferCopy:
                {
                    var dest = ReferenceEquals(bufferCopy.Destination, oldValue) ? newValue : bufferCopy.Destination;
                    var src = ReferenceEquals(bufferCopy.Source, oldValue) ? newValue : bufferCopy.Source;
                    var destOffset = ReferenceEquals(bufferCopy.DestinationOffset, oldValue) ? newValue : bufferCopy.DestinationOffset;
                    var srcOffset = ReferenceEquals(bufferCopy.SourceOffset, oldValue) ? newValue : bufferCopy.SourceOffset;
                    var length = ReferenceEquals(bufferCopy.Length, oldValue) ? newValue : bufferCopy.Length;
                    if (ReferenceEquals(dest, bufferCopy.Destination) && ReferenceEquals(src, bufferCopy.Source) && ReferenceEquals(destOffset, bufferCopy.DestinationOffset) && ReferenceEquals(srcOffset, bufferCopy.SourceOffset) && ReferenceEquals(length, bufferCopy.Length))
                        return instruction;
                    return new MirBufferCopy(dest, src, destOffset, srcOffset, length) { Span = instruction.Span };
                }

            case MirStaticFieldStore staticStore:
                {
                    var value = ReferenceEquals(staticStore.Value, oldValue) ? newValue : staticStore.Value;
                    if (ReferenceEquals(value, staticStore.Value))
                        return instruction;
                    return new MirStaticFieldStore(staticStore.Slot, value, staticStore.FieldType, staticStore.FieldName) { Span = instruction.Span };
                }
        }

        return instruction;
    }

    private static MirTerminator ReplaceInTerminator(MirTerminator terminator, MirValue oldValue, MirValue newValue)
    {
        switch (terminator)
        {
            case MirCondBranch cond when ReferenceEquals(cond.Condition, oldValue):
                return new MirCondBranch(newValue, cond.TrueTarget, cond.FalseTarget)
                { Span = terminator.Span };

            case MirSwitch @switch when ReferenceEquals(@switch.Key, oldValue):
                return new MirSwitch(newValue, @switch.Cases, @switch.DefaultTarget)
                { Span = terminator.Span };

            case MirReturn ret when ReferenceEquals(ret.Value, oldValue):
                return new MirReturn(newValue)
                { Span = terminator.Span };

            case MirAbortMsg abortMsg when ReferenceEquals(abortMsg.Message, oldValue):
                return new MirAbortMsg(newValue)
                { Span = terminator.Span };
        }

        return terminator;
    }

    private static bool ReplaceOperands(IReadOnlyList<MirValue> operands, MirValue oldValue, MirValue newValue, out MirValue[] replaced)
    {
        replaced = new MirValue[operands.Count];
        var changed = false;

        for (int i = 0; i < operands.Count; i++)
        {
            var operand = operands[i];
            if (ReferenceEquals(operand, oldValue))
            {
                replaced[i] = newValue;
                changed = true;
            }
            else
            {
                replaced[i] = operand;
            }
        }

        return changed;
    }
}
