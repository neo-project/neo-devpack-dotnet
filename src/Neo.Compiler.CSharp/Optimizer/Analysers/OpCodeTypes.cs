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
        public static readonly HashSet<OpCode> pushConst = new();
        public static readonly HashSet<OpCode> push;
        public static readonly HashSet<OpCode> longInstructions;
        public static readonly HashSet<OpCode> shortInstructions;
        public static readonly HashSet<OpCode> allowedBasicBlockEnds;

        static OpCodeTypes()
        {
            pushConst = pushConst
                .Union(pushInt)
                .Union(pushBool)
                .Union(pushData)
                .Union(pushConstInt)
                //.Union(pushNewCompoundType)  // array, struct and map are mutable; not considered const
                .ToHashSet();
            pushConst.Add(PUSHA);
            pushConst.Add(PUSHNULL);

            push = pushConst.Union(pushNewCompoundType).Union(pushStackOps).ToHashSet();

            longInstructions = new() { TRY_L, ENDTRY_L, JMP_L, CALL_L, };
            longInstructions = longInstructions.Union(conditionalJump_L).ToHashSet();
            shortInstructions = longInstructions.Select(i => i - 1).ToHashSet();

            allowedBasicBlockEnds = ((OpCode[])Enum.GetValues(typeof(OpCode)))
                    .Where(i => JumpTarget.SingleJumpInOperand(i) && i != PUSHA || JumpTarget.DoubleJumpInOperand(i)).ToHashSet()
                    .Union(new HashSet<OpCode>() { RET, ABORT, ABORTMSG, THROW, ENDFINALLY })
                    .ToHashSet();
        }

        public static OpCode ToLongVersion(OpCode opCode)
        {
            if (longInstructions.Contains(opCode))
                return opCode;
            if (!shortInstructions.Contains(opCode))
                throw new BadScriptException($"No long version for OpCode {opCode}");
            return opCode + 1;
        }

        public static OpCode ToShortVersion(OpCode opCode)
        {
            if (shortInstructions.Contains(opCode))
                return opCode;
            if (!longInstructions.Contains(opCode))
                throw new BadScriptException($"No short version for OpCode {opCode}");
            return opCode - 1;
        }

        public static byte SlotIndex(Instruction i)
        {
            OpCode o = i.OpCode;
            if (slotNonConst.Contains(o))
                return i.TokenU8;
            return SlotIndex(o);
        }

        public static byte SlotIndex(OpCode o)
        {
            if (slotNonConst.Contains(o))
                throw new ArgumentException($"Instruction is needed to get slot index for {o}");
            if (loadArguments.Contains(o))
                return o - LDARG0;
            if (storeArguments.Contains(o))
                return o - STARG0;
            if (loadLocalVariables.Contains(o))
                return o - LDLOC0;
            if (storeLocalVariables.Contains(o))
                return o - STLOC0;
            if (loadStaticFields.Contains(o))
                return o - LDSFLD0;
            if (storeStaticFields.Contains(o))
                return o - STSFLD0;
            throw new ArgumentException($"OpCode {o} cannot access slot");
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
            // CALLT is not included because it generally calls another contract
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
        public static readonly HashSet<OpCode> slotNonConst = new()
        {
            LDARG, LDLOC, LDSFLD,
            STARG, STLOC, STSFLD,
        };
    }
}
