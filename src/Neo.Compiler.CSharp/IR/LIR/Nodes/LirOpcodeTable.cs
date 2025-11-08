using System;
using System.Collections.Generic;

namespace Neo.Compiler.LIR;

internal static class LirOpcodeTable
{
    private static readonly Dictionary<LirOpcode, LirOpcodeInfo> Table = new()
    {
        { LirOpcode.PUSH0, new LirOpcodeInfo(LirOpcode.PUSH0, 0, 1) },
        { LirOpcode.PUSHM1, new LirOpcodeInfo(LirOpcode.PUSHM1, 0, 1) },
        { LirOpcode.PUSHT, new LirOpcodeInfo(LirOpcode.PUSHT, 0, 1) },
        { LirOpcode.PUSHF, new LirOpcodeInfo(LirOpcode.PUSHF, 0, 1) },
        { LirOpcode.PUSHNULL, new LirOpcodeInfo(LirOpcode.PUSHNULL, 0, 1) },
        { LirOpcode.PUSHINT, new LirOpcodeInfo(LirOpcode.PUSHINT, 0, 1, HasImmediate: true) },
        { LirOpcode.PUSHDATA1, new LirOpcodeInfo(LirOpcode.PUSHDATA1, 0, 1, HasImmediate: true) },
        { LirOpcode.PUSHDATA2, new LirOpcodeInfo(LirOpcode.PUSHDATA2, 0, 1, HasImmediate: true) },
        { LirOpcode.PUSHDATA4, new LirOpcodeInfo(LirOpcode.PUSHDATA4, 0, 1, HasImmediate: true) },

        { LirOpcode.DROP, new LirOpcodeInfo(LirOpcode.DROP, 1, 0) },
        { LirOpcode.DUP, new LirOpcodeInfo(LirOpcode.DUP, 1, 2) },
        { LirOpcode.OVER, new LirOpcodeInfo(LirOpcode.OVER, 2, 3) },
        { LirOpcode.SWAP, new LirOpcodeInfo(LirOpcode.SWAP, 2, 2) },
        { LirOpcode.ROT, new LirOpcodeInfo(LirOpcode.ROT, 3, 3) },
        { LirOpcode.PICK, new LirOpcodeInfo(LirOpcode.PICK, 1, 1) },
        { LirOpcode.ROLL, new LirOpcodeInfo(LirOpcode.ROLL, 1, 0) },
        { LirOpcode.REVERSEN, new LirOpcodeInfo(LirOpcode.REVERSEN, 1, 0) },
        { LirOpcode.NIP, new LirOpcodeInfo(LirOpcode.NIP, 2, 1) },
        { LirOpcode.TUCK, new LirOpcodeInfo(LirOpcode.TUCK, 2, 3) },
        { LirOpcode.ISNULL, new LirOpcodeInfo(LirOpcode.ISNULL, 1, 1) },
        { LirOpcode.CONVERT, new LirOpcodeInfo(LirOpcode.CONVERT, 1, 1, HasImmediate: true, ImmediateSizeBytes: 1) },
        { LirOpcode.INITSSLOT, new LirOpcodeInfo(LirOpcode.INITSSLOT, 0, 0, HasImmediate: true, ImmediateSizeBytes: 1) },
        { LirOpcode.INITSLOT, new LirOpcodeInfo(LirOpcode.INITSLOT, 0, 0, HasImmediate: true, ImmediateSizeBytes: 2) },

        { LirOpcode.ADD, new LirOpcodeInfo(LirOpcode.ADD, 2, 1) },
        { LirOpcode.SUB, new LirOpcodeInfo(LirOpcode.SUB, 2, 1) },
        { LirOpcode.MUL, new LirOpcodeInfo(LirOpcode.MUL, 2, 1) },
        { LirOpcode.DIV, new LirOpcodeInfo(LirOpcode.DIV, 2, 1) },
        { LirOpcode.MOD, new LirOpcodeInfo(LirOpcode.MOD, 2, 1) },
        { LirOpcode.NEG, new LirOpcodeInfo(LirOpcode.NEG, 1, 1) },
        { LirOpcode.ABS, new LirOpcodeInfo(LirOpcode.ABS, 1, 1) },
        { LirOpcode.SIGN, new LirOpcodeInfo(LirOpcode.SIGN, 1, 1) },
        { LirOpcode.INC, new LirOpcodeInfo(LirOpcode.INC, 1, 1) },
        { LirOpcode.DEC, new LirOpcodeInfo(LirOpcode.DEC, 1, 1) },
        { LirOpcode.SQRT, new LirOpcodeInfo(LirOpcode.SQRT, 1, 1) },
        { LirOpcode.AND, new LirOpcodeInfo(LirOpcode.AND, 2, 1) },
        { LirOpcode.OR, new LirOpcodeInfo(LirOpcode.OR, 2, 1) },
        { LirOpcode.XOR, new LirOpcodeInfo(LirOpcode.XOR, 2, 1) },
        { LirOpcode.NOT, new LirOpcodeInfo(LirOpcode.NOT, 1, 1) },
        { LirOpcode.SHL, new LirOpcodeInfo(LirOpcode.SHL, 2, 1) },
        { LirOpcode.SHR, new LirOpcodeInfo(LirOpcode.SHR, 2, 1) },
        { LirOpcode.NUMEQUAL, new LirOpcodeInfo(LirOpcode.NUMEQUAL, 2, 1) },
        { LirOpcode.NUMNOTEQUAL, new LirOpcodeInfo(LirOpcode.NUMNOTEQUAL, 2, 1) },
        { LirOpcode.GT, new LirOpcodeInfo(LirOpcode.GT, 2, 1) },
        { LirOpcode.LT, new LirOpcodeInfo(LirOpcode.LT, 2, 1) },
        { LirOpcode.GTE, new LirOpcodeInfo(LirOpcode.GTE, 2, 1) },
        { LirOpcode.LTE, new LirOpcodeInfo(LirOpcode.LTE, 2, 1) },
        { LirOpcode.WITHIN, new LirOpcodeInfo(LirOpcode.WITHIN, 3, 1) },
        { LirOpcode.MAX, new LirOpcodeInfo(LirOpcode.MAX, 2, 1) },
        { LirOpcode.MIN, new LirOpcodeInfo(LirOpcode.MIN, 2, 1) },
        { LirOpcode.POW, new LirOpcodeInfo(LirOpcode.POW, 2, 1) },
        { LirOpcode.MODMUL, new LirOpcodeInfo(LirOpcode.MODMUL, 3, 1) },
        { LirOpcode.MODPOW, new LirOpcodeInfo(LirOpcode.MODPOW, 3, 1) },

        { LirOpcode.CAT, new LirOpcodeInfo(LirOpcode.CAT, 2, 1) },
        { LirOpcode.SUBSTR, new LirOpcodeInfo(LirOpcode.SUBSTR, 3, 1) },
        { LirOpcode.LEFT, new LirOpcodeInfo(LirOpcode.LEFT, 2, 1) },
        { LirOpcode.RIGHT, new LirOpcodeInfo(LirOpcode.RIGHT, 2, 1) },

        { LirOpcode.NEWARRAY, new LirOpcodeInfo(LirOpcode.NEWARRAY, 1, 1) },
        { LirOpcode.NEWSTRUCT, new LirOpcodeInfo(LirOpcode.NEWSTRUCT, 1, 1) },
        { LirOpcode.NEWMAP, new LirOpcodeInfo(LirOpcode.NEWMAP, 0, 1) },
        { LirOpcode.GETITEM, new LirOpcodeInfo(LirOpcode.GETITEM, 2, 1) },
        { LirOpcode.SETITEM, new LirOpcodeInfo(LirOpcode.SETITEM, 3, 0) },
        { LirOpcode.REMOVE, new LirOpcodeInfo(LirOpcode.REMOVE, 2, 0) },
        { LirOpcode.APPEND, new LirOpcodeInfo(LirOpcode.APPEND, 2, 0) },
        { LirOpcode.PACK, new LirOpcodeInfo(LirOpcode.PACK, null, 1) },
        { LirOpcode.PACKSTRUCT, new LirOpcodeInfo(LirOpcode.PACKSTRUCT, null, 1) },
        { LirOpcode.UNPACK, new LirOpcodeInfo(LirOpcode.UNPACK, 1, null) },
        { LirOpcode.KEYS, new LirOpcodeInfo(LirOpcode.KEYS, 1, 1) },
        { LirOpcode.VALUES, new LirOpcodeInfo(LirOpcode.VALUES, 1, 1) },
        { LirOpcode.LENGTH, new LirOpcodeInfo(LirOpcode.LENGTH, 1, 1) },
        { LirOpcode.HASKEY, new LirOpcodeInfo(LirOpcode.HASKEY, 2, 1) },
        { LirOpcode.NEWBUFFER, new LirOpcodeInfo(LirOpcode.NEWBUFFER, 1, 1) },
        { LirOpcode.MEMCPY, new LirOpcodeInfo(LirOpcode.MEMCPY, 5, 0) },

        { LirOpcode.LDSFLD, new LirOpcodeInfo(LirOpcode.LDSFLD, 0, 1, HasImmediate: true, ImmediateSizeBytes: 1) },
        { LirOpcode.STSFLD, new LirOpcodeInfo(LirOpcode.STSFLD, 1, 0, HasImmediate: true, ImmediateSizeBytes: 1) },
        { LirOpcode.LDARG, new LirOpcodeInfo(LirOpcode.LDARG, 0, 1, HasImmediate: true, ImmediateSizeBytes: 1) },
        { LirOpcode.STARG, new LirOpcodeInfo(LirOpcode.STARG, 1, 0, HasImmediate: true, ImmediateSizeBytes: 1) },
        { LirOpcode.LDLOC, new LirOpcodeInfo(LirOpcode.LDLOC, 0, 1, HasImmediate: true, ImmediateSizeBytes: 1) },
        { LirOpcode.STLOC, new LirOpcodeInfo(LirOpcode.STLOC, 1, 0, HasImmediate: true, ImmediateSizeBytes: 1) },

        { LirOpcode.JMP, new LirOpcodeInfo(LirOpcode.JMP, 0, 0, HasImmediate: true, ImmediateSizeBytes: 4) },
        { LirOpcode.JMPIF, new LirOpcodeInfo(LirOpcode.JMPIF, 1, 0, HasImmediate: true, ImmediateSizeBytes: 4) },
        { LirOpcode.JMPIFNOT, new LirOpcodeInfo(LirOpcode.JMPIFNOT, 1, 0, HasImmediate: true, ImmediateSizeBytes: 4) },
        { LirOpcode.JMPEQ, new LirOpcodeInfo(LirOpcode.JMPEQ, 2, 0, HasImmediate: true, ImmediateSizeBytes: 4) },
        { LirOpcode.JMPNE, new LirOpcodeInfo(LirOpcode.JMPNE, 2, 0, HasImmediate: true, ImmediateSizeBytes: 4) },
        { LirOpcode.JMPGT, new LirOpcodeInfo(LirOpcode.JMPGT, 2, 0, HasImmediate: true, ImmediateSizeBytes: 4) },
        { LirOpcode.JMPGE, new LirOpcodeInfo(LirOpcode.JMPGE, 2, 0, HasImmediate: true, ImmediateSizeBytes: 4) },
        { LirOpcode.JMPLT, new LirOpcodeInfo(LirOpcode.JMPLT, 2, 0, HasImmediate: true, ImmediateSizeBytes: 4) },
        { LirOpcode.JMPLE, new LirOpcodeInfo(LirOpcode.JMPLE, 2, 0, HasImmediate: true, ImmediateSizeBytes: 4) },
        { LirOpcode.CALL, new LirOpcodeInfo(LirOpcode.CALL, 0, 0, HasImmediate: true, ImmediateSizeBytes: 4) },
        { LirOpcode.CALLA, new LirOpcodeInfo(LirOpcode.CALLA, 1, 0) },
        { LirOpcode.CALLT, new LirOpcodeInfo(LirOpcode.CALLT, 0, 0, HasImmediate: true, ImmediateSizeBytes: 1) },
        { LirOpcode.TRY_L, new LirOpcodeInfo(LirOpcode.TRY_L, 0, 0, HasImmediate: true, ImmediateSizeBytes: 8) },
        { LirOpcode.ENDTRY_L, new LirOpcodeInfo(LirOpcode.ENDTRY_L, 0, 0, HasImmediate: true, ImmediateSizeBytes: 4) },
        { LirOpcode.ENDFINALLY, new LirOpcodeInfo(LirOpcode.ENDFINALLY, 0, 0) },
        { LirOpcode.RET, new LirOpcodeInfo(LirOpcode.RET, 0, 0) },
        { LirOpcode.ASSERT, new LirOpcodeInfo(LirOpcode.ASSERT, 1, 0) },

        { LirOpcode.SYSCALL, new LirOpcodeInfo(LirOpcode.SYSCALL, 0, 1, HasImmediate: true, ImmediateSizeBytes: 4) },

        { LirOpcode.ABORT, new LirOpcodeInfo(LirOpcode.ABORT, 0, 0) },
        { LirOpcode.ABORTMSG, new LirOpcodeInfo(LirOpcode.ABORTMSG, 1, 0) }
    };

    internal static LirOpcodeInfo Get(LirOpcode opcode)
    {
        if (!Table.TryGetValue(opcode, out var info))
            throw new ArgumentOutOfRangeException(nameof(opcode), opcode, null);

        return info;
    }
}
