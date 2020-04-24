using Neo.VM;
using System;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.Optimizer
{
    public static class OptimizerExtensions
    {
        public static void UpdateForPush(this NefInstruction ins, BigInteger value)
        {
            // Compile

            using var sb = new ScriptBuilder();
            sb.EmitPush(value);
            var script = sb.ToArray();

            // Update

            ins.SetOpCode((OpCode)script[0]);
            ins.SetData(script.Skip(1).ToArray());
        }

        public static bool IsPush(this NefInstruction ins, out BigInteger value)
        {
            switch (ins.OpCode)
            {
                case OpCode.PUSHM1:
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
                    {
                        value = (int)ins.OpCode - (int)OpCode.PUSH0;
                        return true;
                    }
                case OpCode.PUSHDATA1:
                case OpCode.PUSHDATA2:
                case OpCode.PUSHDATA4:
                case OpCode.PUSHINT8:
                case OpCode.PUSHINT16:
                case OpCode.PUSHINT32:
                case OpCode.PUSHINT64:
                case OpCode.PUSHINT128:
                case OpCode.PUSHINT256:
                    {
                        value = new BigInteger(ins.Data);
                        return true;
                    }
            }

            value = 0;
            return false;
        }
    }
}
