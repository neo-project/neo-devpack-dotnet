// Copyright (C) 2015-2026 The Neo Project.
//
// InstructionTranslator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.VM;
using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.Backend.RiscV;

/// <summary>
/// Maps a single NeoVM <see cref="Instruction"/> to a Rust code string
/// targeting the RISC-V Context API. Returns null for control-flow opcodes
/// (jumps, calls, returns, try/catch) which are handled by
/// <see cref="NeoVmToRustTranslator"/>.
/// </summary>
internal static class InstructionTranslator
{
    /// <summary>
    /// Translates one NeoVM instruction to a Rust statement string.
    /// Returns null when the opcode is a control-flow instruction or NOP.
    /// </summary>
    public static string? Translate(Instruction instr)
    {
        switch (instr.OpCode)
        {
            // ---- Constants ----
            case OpCode.PUSHM1:
                return "ctx.push_int(-1);";
            case OpCode.PUSH0:
            case OpCode.PUSH1:
            case OpCode.PUSH2:
            case OpCode.PUSH3:
            case OpCode.PUSH4:
            case OpCode.PUSH5:
            case OpCode.PUSH6:
            case OpCode.PUSH7:
            case OpCode.PUSH8:
            case OpCode.PUSH9:
            case OpCode.PUSH10:
            case OpCode.PUSH11:
            case OpCode.PUSH12:
            case OpCode.PUSH13:
            case OpCode.PUSH14:
            case OpCode.PUSH15:
            case OpCode.PUSH16:
                return $"ctx.push_int({(int)instr.OpCode - (int)OpCode.PUSH0});";

            case OpCode.PUSHINT8:
                return $"ctx.push_int({(sbyte)instr.Operand![0]});";
            case OpCode.PUSHINT16:
                return $"ctx.push_int({BinaryPrimitives.ReadInt16LittleEndian(instr.Operand!)});";
            case OpCode.PUSHINT32:
                return $"ctx.push_int({BinaryPrimitives.ReadInt32LittleEndian(instr.Operand!)});";
            case OpCode.PUSHINT64:
                return $"ctx.push_int({BinaryPrimitives.ReadInt64LittleEndian(instr.Operand!)});";
            case OpCode.PUSHINT128:
            case OpCode.PUSHINT256:
                return $"ctx.push_bigint(&[{FormatBytes(instr.Operand!)}]);";

            case OpCode.PUSHT:
                return "ctx.push_bool(true);";
            case OpCode.PUSHF:
                return "ctx.push_bool(false);";
            case OpCode.PUSHNULL:
                return "ctx.push_null();";

            case OpCode.PUSHA:
                {
                    // PUSHA operand is a relative offset; NeoVM pushes the absolute address.
                    int relOffset = BinaryPrimitives.ReadInt32LittleEndian(instr.Operand!);
                    int absOffset = instr.Offset + relOffset;
                    return $"ctx.push_int({absOffset});";
                }

            case OpCode.PUSHDATA1:
                return $"ctx.push_bytes(&[{FormatBytes(instr.Operand.AsSpan(1))}]);";
            case OpCode.PUSHDATA2:
                return $"ctx.push_bytes(&[{FormatBytes(instr.Operand.AsSpan(2))}]);";
            case OpCode.PUSHDATA4:
                return $"ctx.push_bytes(&[{FormatBytes(instr.Operand.AsSpan(4))}]);";

            // ---- Control flow (return null -- handled by method-level translator) ----
            case OpCode.NOP:
            case OpCode.JMP:
            case OpCode.JMP_L:
            case OpCode.JMPIF:
            case OpCode.JMPIF_L:
            case OpCode.JMPIFNOT:
            case OpCode.JMPIFNOT_L:
            case OpCode.JMPEQ:
            case OpCode.JMPEQ_L:
            case OpCode.JMPNE:
            case OpCode.JMPNE_L:
            case OpCode.JMPGT:
            case OpCode.JMPGT_L:
            case OpCode.JMPGE:
            case OpCode.JMPGE_L:
            case OpCode.JMPLT:
            case OpCode.JMPLT_L:
            case OpCode.JMPLE:
            case OpCode.JMPLE_L:
            case OpCode.CALL:
            case OpCode.CALL_L:
            case OpCode.CALLA:
            case OpCode.RET:
            case OpCode.TRY:
            case OpCode.TRY_L:
            case OpCode.ENDTRY:
            case OpCode.ENDTRY_L:
            case OpCode.ENDFINALLY:
                return null;

            // ---- Abort / Assert / Throw ----
            case OpCode.ABORT:
                return "ctx.abort();";
            case OpCode.ASSERT:
                return "ctx.assert_top();";
            case OpCode.THROW:
                return "ctx.throw_ex();";
            case OpCode.ABORTMSG:
                return "ctx.abort_msg();";
            case OpCode.ASSERTMSG:
                return "ctx.assert_msg();";

            // ---- Syscall / CallT ----
            case OpCode.SYSCALL:
                return $"ctx.syscall(0x{BinaryPrimitives.ReadUInt32LittleEndian(instr.Operand!):x8});";
            case OpCode.CALLT:
                {
                    ushort token = BinaryPrimitives.ReadUInt16LittleEndian(instr.Operand!);
                    uint calltHash = 0x43540000u | token;
                    return $"ctx.syscall(0x{calltHash:x8});";
                }

            // ---- Stack manipulation ----
            case OpCode.DEPTH:
                return "ctx.depth();";
            case OpCode.DROP:
                return "ctx.drop();";
            case OpCode.NIP:
                return "ctx.nip();";
            case OpCode.XDROP:
                return "ctx.xdrop();";
            case OpCode.CLEAR:
                return "ctx.clear();";
            case OpCode.DUP:
                return "ctx.dup();";
            case OpCode.OVER:
                return "ctx.over();";
            case OpCode.PICK:
                return "ctx.pick();";
            case OpCode.TUCK:
                return "ctx.tuck();";
            case OpCode.SWAP:
                return "ctx.swap();";
            case OpCode.ROT:
                return "ctx.rot();";
            case OpCode.ROLL:
                return "ctx.roll();";
            case OpCode.REVERSE3:
                return "ctx.reverse3();";
            case OpCode.REVERSE4:
                return "ctx.reverse4();";
            case OpCode.REVERSEN:
                return "ctx.reverse_n();";

            // ---- Slots ----
            case OpCode.INITSSLOT:
                return $"ctx.init_sslot({instr.Operand![0]});";
            case OpCode.INITSLOT:
                return $"ctx.init_slot({instr.Operand![0]}, {instr.Operand[1]});";

            // LDSFLD0-6 and LDSFLD
            case OpCode.LDSFLD0:
            case OpCode.LDSFLD1:
            case OpCode.LDSFLD2:
            case OpCode.LDSFLD3:
            case OpCode.LDSFLD4:
            case OpCode.LDSFLD5:
            case OpCode.LDSFLD6:
                return $"ctx.load_static({(int)instr.OpCode - (int)OpCode.LDSFLD0});";
            case OpCode.LDSFLD:
                return $"ctx.load_static({instr.Operand![0]});";

            // STSFLD0-6 and STSFLD
            case OpCode.STSFLD0:
            case OpCode.STSFLD1:
            case OpCode.STSFLD2:
            case OpCode.STSFLD3:
            case OpCode.STSFLD4:
            case OpCode.STSFLD5:
            case OpCode.STSFLD6:
                return $"ctx.store_static({(int)instr.OpCode - (int)OpCode.STSFLD0});";
            case OpCode.STSFLD:
                return $"ctx.store_static({instr.Operand![0]});";

            // LDLOC0-6 and LDLOC
            case OpCode.LDLOC0:
            case OpCode.LDLOC1:
            case OpCode.LDLOC2:
            case OpCode.LDLOC3:
            case OpCode.LDLOC4:
            case OpCode.LDLOC5:
            case OpCode.LDLOC6:
                return $"ctx.load_local({(int)instr.OpCode - (int)OpCode.LDLOC0});";
            case OpCode.LDLOC:
                return $"ctx.load_local({instr.Operand![0]});";

            // STLOC0-6 and STLOC
            case OpCode.STLOC0:
            case OpCode.STLOC1:
            case OpCode.STLOC2:
            case OpCode.STLOC3:
            case OpCode.STLOC4:
            case OpCode.STLOC5:
            case OpCode.STLOC6:
                return $"ctx.store_local({(int)instr.OpCode - (int)OpCode.STLOC0});";
            case OpCode.STLOC:
                return $"ctx.store_local({instr.Operand![0]});";

            // LDARG0-6 and LDARG
            case OpCode.LDARG0:
            case OpCode.LDARG1:
            case OpCode.LDARG2:
            case OpCode.LDARG3:
            case OpCode.LDARG4:
            case OpCode.LDARG5:
            case OpCode.LDARG6:
                return $"ctx.load_arg({(int)instr.OpCode - (int)OpCode.LDARG0});";
            case OpCode.LDARG:
                return $"ctx.load_arg({instr.Operand![0]});";

            // STARG0-6 and STARG
            case OpCode.STARG0:
            case OpCode.STARG1:
            case OpCode.STARG2:
            case OpCode.STARG3:
            case OpCode.STARG4:
            case OpCode.STARG5:
            case OpCode.STARG6:
                return $"ctx.store_arg({(int)instr.OpCode - (int)OpCode.STARG0});";
            case OpCode.STARG:
                return $"ctx.store_arg({instr.Operand![0]});";

            // ---- Splice / String ----
            case OpCode.NEWBUFFER:
                return "ctx.new_buffer();";
            case OpCode.MEMCPY:
                return "ctx.memcpy();";
            case OpCode.CAT:
                return "ctx.cat();";
            case OpCode.SUBSTR:
                return "ctx.substr();";
            case OpCode.LEFT:
                return "ctx.left();";
            case OpCode.RIGHT:
                return "ctx.right();";

            // ---- Bitwise logic ----
            case OpCode.INVERT:
                return "ctx.bitwise_not();";
            case OpCode.AND:
                return "ctx.bitwise_and();";
            case OpCode.OR:
                return "ctx.bitwise_or();";
            case OpCode.XOR:
                return "ctx.bitwise_xor();";
            case OpCode.EQUAL:
                return "ctx.equal();";
            case OpCode.NOTEQUAL:
                return "ctx.not_equal();";

            // ---- Arithmetic ----
            case OpCode.SIGN:
                return "ctx.sign();";
            case OpCode.ABS:
                return "ctx.abs();";
            case OpCode.NEGATE:
                return "ctx.negate();";
            case OpCode.INC:
                return "ctx.inc();";
            case OpCode.DEC:
                return "ctx.dec();";
            case OpCode.ADD:
                return "ctx.add();";
            case OpCode.SUB:
                return "ctx.sub();";
            case OpCode.MUL:
                return "ctx.mul();";
            case OpCode.DIV:
                return "ctx.div();";
            case OpCode.MOD:
                return "ctx.modulo();";
            case OpCode.POW:
                return "ctx.pow();";
            case OpCode.SQRT:
                return "ctx.sqrt();";
            case OpCode.MODMUL:
                return "ctx.mod_mul();";
            case OpCode.MODPOW:
                return "ctx.mod_pow();";
            case OpCode.SHL:
                return "ctx.shl();";
            case OpCode.SHR:
                return "ctx.shr();";
            case OpCode.NOT:
                return "ctx.not();";
            case OpCode.BOOLAND:
                return "ctx.bool_and();";
            case OpCode.BOOLOR:
                return "ctx.bool_or();";
            case OpCode.NZ:
                return "ctx.nz();";
            case OpCode.NUMEQUAL:
                return "ctx.num_equal();";
            case OpCode.NUMNOTEQUAL:
                return "ctx.num_not_equal();";
            case OpCode.LT:
                return "ctx.less_than();";
            case OpCode.LE:
                return "ctx.less_or_equal();";
            case OpCode.GT:
                return "ctx.greater_than();";
            case OpCode.GE:
                return "ctx.greater_or_equal();";
            case OpCode.MIN:
                return "ctx.min();";
            case OpCode.MAX:
                return "ctx.max();";
            case OpCode.WITHIN:
                return "ctx.within();";

            // ---- Compound types ----
            case OpCode.PACKMAP:
                return "ctx.pack_map();";
            case OpCode.PACKSTRUCT:
                return "ctx.pack_struct();";
            case OpCode.PACK:
                return "ctx.pack();";
            case OpCode.UNPACK:
                return "ctx.unpack();";
            case OpCode.NEWARRAY0:
                return "ctx.new_array0();";
            case OpCode.NEWARRAY:
                return "ctx.new_array();";
            case OpCode.NEWARRAY_T:
                return $"ctx.new_array_t(0x{instr.Operand![0]:x2});";
            case OpCode.NEWSTRUCT0:
                return "ctx.new_struct0();";
            case OpCode.NEWSTRUCT:
                return "ctx.new_struct();";
            case OpCode.NEWMAP:
                return "ctx.new_map();";
            case OpCode.SIZE:
                return "ctx.size();";
            case OpCode.HASKEY:
                return "ctx.has_key();";
            case OpCode.KEYS:
                return "ctx.keys();";
            case OpCode.VALUES:
                return "ctx.values();";
            case OpCode.PICKITEM:
                return "ctx.pick_item();";
            case OpCode.APPEND:
                return "ctx.append();";
            case OpCode.SETITEM:
                return "ctx.set_item();";
            case OpCode.REVERSEITEMS:
                return "ctx.reverse_items();";
            case OpCode.REMOVE:
                return "ctx.remove();";
            case OpCode.CLEARITEMS:
                return "ctx.clear_items();";
            case OpCode.POPITEM:
                return "ctx.pop_item();";

            // ---- Types ----
            case OpCode.ISNULL:
                return "ctx.is_null();";
            case OpCode.ISTYPE:
                return $"ctx.is_type(0x{instr.Operand![0]:x2});";
            case OpCode.CONVERT:
                return $"ctx.convert(0x{instr.Operand![0]:x2});";

            default:
                return $"ctx.fault(\"unsupported opcode: {instr.OpCode}\");";
        }
    }

    /// <summary>
    /// Formats a byte span as comma-separated hex literals for Rust byte slice syntax.
    /// Example: "0x01, 0x02, 0xff"
    /// </summary>
    internal static string FormatBytes(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0) return "";
        var sb = new StringBuilder(bytes.Length * 6);
        for (int i = 0; i < bytes.Length; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append($"0x{bytes[i]:x2}");
        }
        return sb.ToString();
    }
}
