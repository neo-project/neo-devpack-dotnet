using Neo.VM;
using System.Collections.Generic;
using static Neo.VM.OpCode;

namespace Neo.Optimizer
{
    public static class OpCodeTypes
    {
        public static readonly HashSet<OpCode> push = new();

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
            foreach (OpCode op in pushConst)
                push.Add(op);
            foreach (OpCode op in pushStackOps)
                push.Add(op);
            foreach (OpCode op in pushNewCompoundType)
                push.Add(op);
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

        public static readonly HashSet<OpCode> pushConst = new()
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
    }
}
