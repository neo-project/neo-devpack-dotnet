using Neo.VM;
using System.Collections.Generic;
using static Neo.VM.OpCode;

namespace Neo.Optimizer
{
    public static class OpCodeTypes
    {
        public static readonly HashSet<OpCode> pushInt = new HashSet<OpCode>
        {
            PUSHINT8,
            PUSHINT16,
            PUSHINT32,
            PUSHINT64,
            PUSHINT128,
            PUSHINT256,
        };
        public static readonly HashSet<OpCode> pushBool = new HashSet<OpCode>
        {
            PUSHT, PUSHF,
        };
        public static readonly HashSet<OpCode> pushData = new HashSet<OpCode>
        {
            PUSHDATA1,
            PUSHDATA2,
            PUSHDATA4,
        };
        public static readonly HashSet<OpCode> pushConst = new HashSet<OpCode>
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
        public static readonly HashSet<OpCode> pushStackOps = new HashSet<OpCode>
        {
            DEPTH,
            DUP,
            OVER,
        };
        public static readonly HashSet<OpCode> pushNewCompoundType = new HashSet<OpCode>
        {
            NEWARRAY0,
            NEWSTRUCT0,
            NEWMAP,
        };
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

        // BE AWARE that PUSHA is also related to addresses
        public static readonly HashSet<OpCode> tryThrowFinally = new HashSet<OpCode>
        {
            TRY,
            TRY_L,
            THROW,
            ENDTRY,
            ENDTRY_L,
            ENDFINALLY,
        };
        public static readonly HashSet<OpCode> unconditionalJump = new HashSet<OpCode>
        {
            JMP,
            JMP_L,
        };
        public static readonly HashSet<OpCode> callWithJump = new HashSet<OpCode>
        {
            CALL,
            CALL_L,
            CALLA,
        };
        public static readonly HashSet<OpCode> conditionalJump = new HashSet<OpCode>
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
        public static readonly HashSet<OpCode> conditionalJump_L = new HashSet<OpCode>
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
