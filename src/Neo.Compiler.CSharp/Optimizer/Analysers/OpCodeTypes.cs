// Copyright (C) 2015-2024 The Neo Project.
//
// OpCodeTypes.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using static Neo.VM.OpCode;

namespace Neo.Optimizer
{
    static class OpCodeTypes
    {
        public static readonly HashSet<OpCode> push = new();
        public static readonly HashSet<OpCode> allowedBasicBlockEnds;

        static OpCodeTypes()
        {
            foreach (OpCode op in pushInt)
                push.Add(op);
            foreach (OpCode op in pushBool)
                push.Add(op);
            push.Add(PUSHA);
            push.Add(PUSHNULL);
            foreach (OpCode op in pushData)
                push.Add(op);
            foreach (OpCode op in pushConstInt)
                push.Add(op);
            foreach (OpCode op in pushStackOps)
                push.Add(op);
            foreach (OpCode op in pushNewCompoundType)
                push.Add(op);
            allowedBasicBlockEnds = ((OpCode[])Enum.GetValues(typeof(OpCode)))
                    .Where(i => JumpTarget.SingleJumpInOperand(i) && i != PUSHA || JumpTarget.DoubleJumpInOperand(i)).ToHashSet()
                    .Union(new HashSet<OpCode>() { RET, ABORT, ABORTMSG, THROW, ENDFINALLY
            }).ToHashSet();
        }

        public static readonly HashSet<OpCode> pushInt = new()
        {
            PUSHINT8,
            PUSHINT16,
            PUSHINT32,
            PUSHINT64,
            PUSHINT128,
            PUSHINT256,
        };

        public static readonly HashSet<OpCode> pushBool = new()
        {
            PUSHT, PUSHF,
        };

        public static readonly HashSet<OpCode> pushData = new()
        {
            PUSHDATA1,
            PUSHDATA2,
            PUSHDATA4,
        };

        public static readonly HashSet<OpCode> pushConstInt = new()
        {
            PUSHM1,
            PUSH0,
            PUSH1,
            PUSH2,
            PUSH3,
            PUSH4,
            PUSH5,
            PUSH6,
            PUSH7,
            PUSH8,
            PUSH9,
            PUSH10,
            PUSH11,
            PUSH12,
            PUSH13,
            PUSH14,
            PUSH15,
            PUSH16,
        };

        public static readonly HashSet<OpCode> pushStackOps = new()
        {
            DEPTH,
            DUP,
            OVER,
        };

        public static readonly HashSet<OpCode> pushNewCompoundType = new()
        {
            NEWARRAY0,
            NEWSTRUCT0,
            NEWMAP,
        };

        // BE AWARE that PUSHA is also related to addresses
        public static readonly HashSet<OpCode> tryThrowFinally = new()
        {
            TRY,
            TRY_L,
            THROW,
            ENDTRY,
            ENDTRY_L,
            ENDFINALLY,
        };

        public static readonly HashSet<OpCode> unconditionalJump = new()
        {
            JMP,
            JMP_L,
        };

        public static readonly HashSet<OpCode> callWithJump = new()
        {
            CALL,
            CALL_L,
            CALLA,
        };

        public static readonly HashSet<OpCode> conditionalJump = new()
        {
            JMPIF,
            JMPIFNOT,
            JMPEQ,
            JMPNE,
            JMPGT,
            JMPGE,
            JMPLT,
            JMPLE,
        };

        public static readonly HashSet<OpCode> conditionalJump_L = new()
        {
            JMPIF_L,
            JMPIFNOT_L,
            JMPEQ_L,
            JMPNE_L,
            JMPGT_L,
            JMPGE_L,
            JMPLT_L,
            JMPLE_L,
        };

        public static readonly HashSet<OpCode> loadStaticFields = new()
        {
            LDSFLD,
            LDSFLD0,
            LDSFLD1,
            LDSFLD2,
            LDSFLD3,
            LDSFLD4,
            LDSFLD5,
            LDSFLD6,
        };
        public static readonly HashSet<OpCode> storeStaticFields = new()
        {
            STSFLD,
            STSFLD0,
            STSFLD1,
            STSFLD2,
            STSFLD3,
            STSFLD4,
            STSFLD5,
            STSFLD6,
        };
        public static readonly HashSet<OpCode> loadLocalVariables = new()
        {
            LDLOC,
            LDLOC0,
            LDLOC1,
            LDLOC2,
            LDLOC3,
            LDLOC4,
            LDLOC5,
            LDLOC6,
        };
        public static readonly HashSet<OpCode> storeLocalVariables = new()
        {
            STLOC,
            STLOC0,
            STLOC1,
            STLOC2,
            STLOC3,
            STLOC4,
            STLOC5,
            STLOC6,
        };
        public static readonly HashSet<OpCode> loadArguments = new()
        {
            LDARG,
            LDARG0,
            LDARG1,
            LDARG2,
            LDARG3,
            LDARG4,
            LDARG5,
            LDARG6,
        };
        public static readonly HashSet<OpCode> storeArguments = new()
        {
            STARG,
            STARG0,
            STARG1,
            STARG2,
            STARG3,
            STARG4,
            STARG5,
            STARG6,
        };
    }
}
