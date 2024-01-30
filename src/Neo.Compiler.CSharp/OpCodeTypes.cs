using Neo.VM;
using System.Collections.Generic;
using static Neo.VM.OpCode;

namespace Neo.Optimizer
{
    public static class OpCodeTypes
    {
        // BE AWARE that PUSHA is also related to addresses
        public static HashSet<OpCode> tryThrowFinally = new HashSet<OpCode>
        {
            TRY,
            TRY_L,
            THROW,
            ENDTRY,
            ENDTRY_L,
            ENDFINALLY,
        };
        public static HashSet<OpCode> unconditionalJump = new HashSet<OpCode>
        {
            JMP,
            JMP_L,
        };
        public static HashSet<OpCode> callWithJump = new HashSet<OpCode>
        {
            CALL,
            CALL_L,
            CALLA,
        };
        public static HashSet<OpCode> conditionalJump = new HashSet<OpCode>
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
        public static HashSet<OpCode> conditionalJump_L = new HashSet<OpCode>
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
