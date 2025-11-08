using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Neo.Compiler.HIR;
using Neo.VM;

namespace Neo.Compiler.LIR.Backend;

/// <summary>
/// Encodes Stack-LIR into NeoVM bytecode, resolving label fixups and producing a source map that links instruction
/// offsets back to original spans.
/// </summary>
internal sealed class NeoEmitter
{
    internal sealed record Fixup(int InstructionOffset, int OperandOffset, string TargetLabel, OpCode Opcode, int OperandSize);

    private static readonly Dictionary<LirOpcode, OpCode> DirectOpcodeMap = new()
    {
        { LirOpcode.PUSH0, OpCode.PUSH0 },
        { LirOpcode.PUSHM1, OpCode.PUSHM1 },
        { LirOpcode.PUSHT, OpCode.PUSHT },
        { LirOpcode.PUSHF, OpCode.PUSHF },
        { LirOpcode.PUSHNULL, OpCode.PUSHNULL },
        { LirOpcode.DROP, OpCode.DROP },
        { LirOpcode.DUP, OpCode.DUP },
        { LirOpcode.OVER, OpCode.OVER },
        { LirOpcode.SWAP, OpCode.SWAP },
        { LirOpcode.ROT, OpCode.ROT },
        { LirOpcode.PICK, OpCode.PICK },
        { LirOpcode.ROLL, OpCode.ROLL },
        { LirOpcode.REVERSEN, OpCode.REVERSEN },
        { LirOpcode.NIP, OpCode.NIP },
        { LirOpcode.TUCK, OpCode.TUCK },
        { LirOpcode.ISNULL, OpCode.ISNULL },
        { LirOpcode.CONVERT, OpCode.CONVERT },
        { LirOpcode.ADD, OpCode.ADD },
        { LirOpcode.SUB, OpCode.SUB },
        { LirOpcode.MUL, OpCode.MUL },
        { LirOpcode.DIV, OpCode.DIV },
        { LirOpcode.MOD, OpCode.MOD },
        { LirOpcode.NEG, OpCode.NEGATE },
        { LirOpcode.ABS, OpCode.ABS },
        { LirOpcode.SIGN, OpCode.SIGN },
        { LirOpcode.INC, OpCode.INC },
        { LirOpcode.DEC, OpCode.DEC },
        { LirOpcode.SQRT, OpCode.SQRT },
        { LirOpcode.AND, OpCode.AND },
        { LirOpcode.OR, OpCode.OR },
        { LirOpcode.XOR, OpCode.XOR },
        { LirOpcode.NOT, OpCode.NOT },
        { LirOpcode.SHL, OpCode.SHL },
        { LirOpcode.SHR, OpCode.SHR },
        { LirOpcode.NUMEQUAL, OpCode.NUMEQUAL },
        { LirOpcode.NUMNOTEQUAL, OpCode.NUMNOTEQUAL },
        { LirOpcode.GT, OpCode.GT },
        { LirOpcode.LT, OpCode.LT },
        { LirOpcode.GTE, OpCode.GE },
        { LirOpcode.LTE, OpCode.LE },
        { LirOpcode.WITHIN, OpCode.WITHIN },
        { LirOpcode.MAX, OpCode.MAX },
        { LirOpcode.MIN, OpCode.MIN },
        { LirOpcode.POW, OpCode.POW },
        { LirOpcode.MODMUL, OpCode.MODMUL },
        { LirOpcode.MODPOW, OpCode.MODPOW },
        { LirOpcode.CAT, OpCode.CAT },
        { LirOpcode.SUBSTR, OpCode.SUBSTR },
        { LirOpcode.LEFT, OpCode.LEFT },
        { LirOpcode.RIGHT, OpCode.RIGHT },
        { LirOpcode.NEWARRAY, OpCode.NEWARRAY },
        { LirOpcode.NEWSTRUCT, OpCode.NEWSTRUCT },
        { LirOpcode.NEWMAP, OpCode.NEWMAP },
        { LirOpcode.NEWBUFFER, OpCode.NEWBUFFER },
        { LirOpcode.MEMCPY, OpCode.MEMCPY },
        { LirOpcode.GETITEM, OpCode.PICKITEM },
        { LirOpcode.SETITEM, OpCode.SETITEM },
        { LirOpcode.APPEND, OpCode.APPEND },
        { LirOpcode.PACK, OpCode.PACK },
        { LirOpcode.PACKSTRUCT, OpCode.PACKSTRUCT },
        { LirOpcode.UNPACK, OpCode.UNPACK },
        { LirOpcode.KEYS, OpCode.KEYS },
        { LirOpcode.VALUES, OpCode.VALUES },
        { LirOpcode.LENGTH, OpCode.SIZE },
        { LirOpcode.HASKEY, OpCode.HASKEY },
        { LirOpcode.REMOVE, OpCode.REMOVE },
        { LirOpcode.LDSFLD, OpCode.LDSFLD },
        { LirOpcode.STSFLD, OpCode.STSFLD },
        { LirOpcode.JMP, OpCode.JMP_L },
        { LirOpcode.JMPIF, OpCode.JMPIF_L },
        { LirOpcode.JMPIFNOT, OpCode.JMPIFNOT_L },
        { LirOpcode.JMPEQ, OpCode.JMPEQ_L },
        { LirOpcode.JMPNE, OpCode.JMPNE_L },
        { LirOpcode.JMPGT, OpCode.JMPGT_L },
        { LirOpcode.JMPGE, OpCode.JMPGE_L },
        { LirOpcode.JMPLT, OpCode.JMPLT_L },
        { LirOpcode.JMPLE, OpCode.JMPLE_L },
        { LirOpcode.CALL, OpCode.CALL_L },
        { LirOpcode.CALLA, OpCode.CALLA },
        { LirOpcode.CALLT, OpCode.CALLT },
        { LirOpcode.TRY_L, OpCode.TRY_L },
        { LirOpcode.ENDTRY_L, OpCode.ENDTRY_L },
        { LirOpcode.ENDFINALLY, OpCode.ENDFINALLY },
        { LirOpcode.RET, OpCode.RET },
        { LirOpcode.ASSERT, OpCode.ASSERT },
        { LirOpcode.ABORT, OpCode.ABORT },
        { LirOpcode.ABORTMSG, OpCode.ABORTMSG }
    };

    internal sealed record EmitResult(byte[] Code, Dictionary<int, SourceMapEntry> SourceMap, int MaxStack);

    internal sealed record IntermediateResult(
        byte[] Code,
        Dictionary<int, SourceMapEntry> SourceMap,
        Dictionary<string, int> LabelOffsets,
        List<Fixup> Fixups,
        int MaxStack);

    internal sealed record SourceMapEntry(string File, int Line, int Column);

    internal IntermediateResult EmitIntermediate(LirFunction function, int assumedMaxStack)
    {
        if (function is null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        var labelOffsets = new Dictionary<string, int>(StringComparer.Ordinal);
        var fixups = new List<Fixup>();
        var sourceMap = new Dictionary<int, SourceMapEntry>();

        foreach (var block in function.Blocks)
        {
            labelOffsets[block.Label] = (int)stream.Position;

            foreach (var instruction in block.Instructions)
            {
                var position = (int)stream.Position;
                EncodeInstruction(writer, instruction, fixups);

                if (instruction.Span is { } span)
                {
                    sourceMap[position] = new SourceMapEntry(span.File, span.StartLine, span.StartColumn);
                }
            }
        }

        return new IntermediateResult(stream.ToArray(), sourceMap, labelOffsets, fixups, assumedMaxStack);
    }

    internal EmitResult Emit(LirFunction function, int assumedMaxStack)
    {
        var intermediate = EmitIntermediate(function, assumedMaxStack);
        var code = (byte[])intermediate.Code.Clone();
        PatchFixups(code, intermediate.LabelOffsets, intermediate.Fixups);
        return new EmitResult(code, intermediate.SourceMap, intermediate.MaxStack);
    }

    private static void EncodeInstruction(BinaryWriter writer, LirInst instruction, List<Fixup> fixups)
    {
        switch (instruction.Op)
        {
            case LirOpcode.PUSHINT:
                EncodePushInt(writer, instruction);
                return;

            case LirOpcode.PUSHDATA1:
            case LirOpcode.PUSHDATA2:
            case LirOpcode.PUSHDATA4:
                EncodePushData(writer, instruction);
                return;

            case LirOpcode.INITSSLOT:
                EncodeInitSslot(writer, instruction);
                return;

            case LirOpcode.INITSLOT:
                EncodeInitSlot(writer, instruction);
                return;

            case LirOpcode.LDARG:
                EncodeLoadStoreSlot(writer, instruction, OpCode.LDARG0, OpCode.LDARG);
                return;

            case LirOpcode.STARG:
                EncodeLoadStoreSlot(writer, instruction, OpCode.STARG0, OpCode.STARG);
                return;

            case LirOpcode.LDLOC:
                EncodeLoadStoreSlot(writer, instruction, OpCode.LDLOC0, OpCode.LDLOC);
                return;

            case LirOpcode.STLOC:
                EncodeLoadStoreSlot(writer, instruction, OpCode.STLOC0, OpCode.STLOC);
                return;

            case LirOpcode.JMP:
                EncodeJump(writer, instruction, fixups, OpCode.JMP_L, operandSize: 4);
                return;

            case LirOpcode.JMPIF:
                EncodeJump(writer, instruction, fixups, OpCode.JMPIF_L, operandSize: 4);
                return;

            case LirOpcode.JMPIFNOT:
                EncodeJump(writer, instruction, fixups, OpCode.JMPIFNOT_L, operandSize: 4);
                return;

            case LirOpcode.JMPEQ:
                EncodeJump(writer, instruction, fixups, OpCode.JMPEQ_L, operandSize: 4);
                return;

            case LirOpcode.JMPNE:
                EncodeJump(writer, instruction, fixups, OpCode.JMPNE_L, operandSize: 4);
                return;

            case LirOpcode.JMPGT:
                EncodeJump(writer, instruction, fixups, OpCode.JMPGT_L, operandSize: 4);
                return;

            case LirOpcode.JMPGE:
                EncodeJump(writer, instruction, fixups, OpCode.JMPGE_L, operandSize: 4);
                return;

            case LirOpcode.JMPLT:
                EncodeJump(writer, instruction, fixups, OpCode.JMPLT_L, operandSize: 4);
                return;

            case LirOpcode.JMPLE:
                EncodeJump(writer, instruction, fixups, OpCode.JMPLE_L, operandSize: 4);
                return;

            case LirOpcode.CALL:
                EncodeJump(writer, instruction, fixups, OpCode.CALL_L, operandSize: 4);
                return;

            case LirOpcode.TRY_L:
                EncodeTry(writer, instruction, fixups);
                return;

            case LirOpcode.ENDTRY_L:
                EncodeJump(writer, instruction, fixups, OpCode.ENDTRY_L, operandSize: 4);
                return;

            case LirOpcode.SYSCALL:
                EncodeSyscall(writer, instruction);
                return;

            case LirOpcode.LDSFLD:
                EncodeStaticSlot(writer, instruction, isStore: false);
                return;

            case LirOpcode.STSFLD:
                EncodeStaticSlot(writer, instruction, isStore: true);
                return;
        }

        if (!DirectOpcodeMap.TryGetValue(instruction.Op, out var opcode))
        {
            throw new NotSupportedException($"No opcode mapping defined for {instruction.Op}.");
        }

        writer.Write((byte)opcode);

        if (instruction.Immediate is { Length: > 0 })
        {
            writer.Write(instruction.Immediate);
        }
    }

    private static void EncodePushInt(BinaryWriter writer, LirInst instruction)
    {
        if (instruction.Immediate is not { Length: > 0 })
        {
            throw new InvalidOperationException("PUSHINT requires an integer payload.");
        }

        var value = new BigInteger(instruction.Immediate);
        WriteInteger(writer, value);
    }

    private static void EncodePushData(BinaryWriter writer, LirInst instruction)
    {
        if (instruction.Immediate is not { } data)
        {
            throw new InvalidOperationException($"{instruction.Op} requires data payload.");
        }

        switch (instruction.Op)
        {
            case LirOpcode.PUSHDATA1:
                if (data.Length > byte.MaxValue)
                    throw new InvalidOperationException("PUSHDATA1 payload exceeds 255 bytes.");
                writer.Write((byte)OpCode.PUSHDATA1);
                writer.Write((byte)data.Length);
                writer.Write(data);
                break;

            case LirOpcode.PUSHDATA2:
                if (data.Length > ushort.MaxValue)
                    throw new InvalidOperationException("PUSHDATA2 payload exceeds 65535 bytes.");
                writer.Write((byte)OpCode.PUSHDATA2);
                writer.Write((ushort)data.Length);
                writer.Write(data);
                break;

            case LirOpcode.PUSHDATA4:
                writer.Write((byte)OpCode.PUSHDATA4);
                writer.Write(data.Length);
                writer.Write(data);
                break;

            default:
                throw new NotSupportedException($"Unsupported push-data opcode {instruction.Op}.");
        }
    }

    private static void EncodeInitSslot(BinaryWriter writer, LirInst instruction)
    {
        if (instruction.Immediate is not { Length: 1 } payload)
            throw new InvalidOperationException("INITSSLOT expects single-byte static slot count.");

        writer.Write((byte)OpCode.INITSSLOT);
        writer.Write(payload[0]);
    }

    private static void EncodeInitSlot(BinaryWriter writer, LirInst instruction)
    {
        if (instruction.Immediate is not { Length: 2 } payload)
            throw new InvalidOperationException("INITSLOT expects argument/local counts encoded as two bytes.");

        writer.Write((byte)OpCode.INITSLOT);
        writer.Write(payload);
    }

    private static void EncodeLoadStoreSlot(BinaryWriter writer, LirInst instruction, OpCode shortBase, OpCode general)
    {
        if (instruction.Immediate is not { Length: 1 } payload)
            throw new InvalidOperationException($"{instruction.Op} expects single-byte slot index.");

        var index = payload[0];
        if (index <= 6)
        {
            writer.Write((byte)((byte)shortBase + index));
        }
        else
        {
            writer.Write((byte)general);
            writer.Write(index);
        }
    }

    private static void EncodeTry(BinaryWriter writer, LirInst instruction, List<Fixup> fixups)
    {
        if (instruction.TargetLabel2 is null)
            throw new InvalidOperationException("TRY_L requires a finally target label.");

        var instructionOffset = (int)writer.BaseStream.Position;
        writer.Write((byte)OpCode.TRY_L);
        var operandOffset = (int)writer.BaseStream.Position;
        writer.Write(0);
        writer.Write(0);

        if (instruction.TargetLabel is not null)
            fixups.Add(new Fixup(instructionOffset, operandOffset, instruction.TargetLabel, OpCode.TRY_L, 4));

        fixups.Add(new Fixup(instructionOffset, operandOffset + 4, instruction.TargetLabel2, OpCode.TRY_L, 4));
    }

    private static void EncodeJump(BinaryWriter writer, LirInst instruction, List<Fixup> fixups, OpCode opcode, int operandSize)
    {
        if (instruction.TargetLabel is null)
        {
            throw new InvalidOperationException($"{instruction.Op} requires a target label.");
        }

        var instructionOffset = (int)writer.BaseStream.Position;
        writer.Write((byte)opcode);
        var operandOffset = (int)writer.BaseStream.Position;
        fixups.Add(new Fixup(instructionOffset, operandOffset, instruction.TargetLabel, opcode, operandSize));
        writer.Write(new byte[operandSize]);
    }

    private static void EncodeSyscall(BinaryWriter writer, LirInst instruction)
    {
        if (instruction.Immediate is not { Length: 4 })
        {
            throw new InvalidOperationException("SYSCALL expects a 4-byte identifier.");
        }

        writer.Write((byte)OpCode.SYSCALL);
        writer.Write(instruction.Immediate);
    }

    private static void EncodeStaticSlot(BinaryWriter writer, LirInst instruction, bool isStore)
    {
        if (instruction.Immediate is not { Length: 1 })
            throw new InvalidOperationException($"{instruction.Op} expects a single-byte slot immediate.");

        var slot = instruction.Immediate[0];
        if (slot <= 6)
        {
            var baseOpcode = isStore ? OpCode.STSFLD0 : OpCode.LDSFLD0;
            var opcode = (OpCode)((byte)baseOpcode + slot);
            writer.Write((byte)opcode);
        }
        else
        {
            writer.Write((byte)(isStore ? OpCode.STSFLD : OpCode.LDSFLD));
            writer.Write(slot);
        }
    }

    private static void WriteInteger(BinaryWriter writer, BigInteger value)
    {
        if (value == -1)
        {
            writer.Write((byte)OpCode.PUSHM1);
            return;
        }

        if (value >= 0 && value <= 16)
        {
            var smallConstOpcode = (OpCode)((byte)OpCode.PUSH0 + (byte)value);
            writer.Write((byte)smallConstOpcode);
            return;
        }

        Span<byte> buffer = stackalloc byte[32];
        if (!value.TryWriteBytes(buffer, out var bytesWritten, isUnsigned: false, isBigEndian: false))
        {
            throw new InvalidOperationException("Failed to encode integer literal.");
        }

        (int width, OpCode pushOpcode) = bytesWritten switch
        {
            <= 1 => (1, OpCode.PUSHINT8),
            <= 2 => (2, OpCode.PUSHINT16),
            <= 4 => (4, OpCode.PUSHINT32),
            <= 8 => (8, OpCode.PUSHINT64),
            <= 16 => (16, OpCode.PUSHINT128),
            <= 32 => (32, OpCode.PUSHINT256),
            _ => throw new InvalidOperationException("Integer literal exceeds 256 bits.")
        };

        writer.Write((byte)pushOpcode);

        for (var i = 0; i < width; i++)
        {
            var b = i < bytesWritten ? buffer[i] : (value.Sign < 0 ? (byte)0xFF : (byte)0x00);
            writer.Write(b);
        }
    }

    private static void PatchFixups(byte[] code, IReadOnlyDictionary<string, int> labelOffsets, List<Fixup> fixups)
    {
        foreach (var fixup in fixups)
        {
            if (!labelOffsets.TryGetValue(fixup.TargetLabel, out var targetOffset))
            {
                throw new InvalidOperationException($"Unknown label '{fixup.TargetLabel}' referenced during emission.");
            }

            var relativeBase = fixup.OperandOffset + fixup.OperandSize;
            var relative = targetOffset - relativeBase;
            var span = code.AsSpan(fixup.OperandOffset, fixup.OperandSize);

            switch (fixup.OperandSize)
            {
                case 1:
                    if (relative is < sbyte.MinValue or > sbyte.MaxValue)
                        throw new InvalidOperationException("Jump offset does not fit in 1 byte.");
                    span[0] = unchecked((byte)(sbyte)relative);
                    break;

                case 4:
                    BinaryPrimitives.WriteInt32LittleEndian(span, relative);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported operand size {fixup.OperandSize} for {fixup.Opcode}.");
            }
        }
    }

    internal static IReadOnlySet<OpCode> GetSupportedOpcodes()
    {
        var supported = new HashSet<OpCode>(DirectOpcodeMap.Values);

        // Arithmetic constant encoding produces specialised opcodes.
        supported.Add(OpCode.PUSHINT8);
        supported.Add(OpCode.PUSHINT16);
        supported.Add(OpCode.PUSHINT32);
        supported.Add(OpCode.PUSHINT64);
        supported.Add(OpCode.PUSHINT128);
        supported.Add(OpCode.PUSHINT256);
        supported.Add(OpCode.PUSHDATA1);
        supported.Add(OpCode.PUSHDATA2);
        supported.Add(OpCode.PUSHDATA4);
        supported.Add(OpCode.INITSSLOT);
        supported.Add(OpCode.INITSLOT);
        supported.Add(OpCode.LDARG);
        supported.Add(OpCode.STARG);
        supported.Add(OpCode.LDLOC);
        supported.Add(OpCode.STLOC);
        supported.Add(OpCode.SYSCALL);

        for (int value = 1; value <= 16; value++)
        {
            var opcode = (OpCode)((byte)OpCode.PUSH0 + value);
            supported.Add(opcode);
        }

        // Stack-slot helpers emitted for small indices.
        for (int slot = 0; slot <= 6; slot++)
        {
            supported.Add((OpCode)((byte)OpCode.LDSFLD0 + slot));
            supported.Add((OpCode)((byte)OpCode.STSFLD0 + slot));
            supported.Add((OpCode)((byte)OpCode.LDARG0 + slot));
            supported.Add((OpCode)((byte)OpCode.STARG0 + slot));
            supported.Add((OpCode)((byte)OpCode.LDLOC0 + slot));
            supported.Add((OpCode)((byte)OpCode.STLOC0 + slot));
        }

        supported.Add(OpCode.LDSFLD);
        supported.Add(OpCode.STSFLD);
        supported.Add(OpCode.LDARG);
        supported.Add(OpCode.STARG);
        supported.Add(OpCode.LDLOC);
        supported.Add(OpCode.STLOC);

        // Control-flow helpers emitted by specialised encoders.
        supported.Add(OpCode.JMP_L);
        supported.Add(OpCode.JMPIF_L);
        supported.Add(OpCode.JMPIFNOT_L);
        supported.Add(OpCode.CALL_L);

        return supported;
    }

    internal static bool SupportsOpcode(OpCode opcode)
        => GetSupportedOpcodes().Contains(opcode);
}
