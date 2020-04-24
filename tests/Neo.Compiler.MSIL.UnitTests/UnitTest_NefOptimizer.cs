using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.Optimizer;
using Neo.VM;
using System.Buffers.Binary;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_NefOptimizer
    {
        [TestMethod]
        public void Test_Optimize_RemoveNOPS()
        {
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(VM.OpCode.NOP);
            scriptBefore.Emit(VM.OpCode.NOP);

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray());

            using var scriptAfter = new ScriptBuilder();

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_JMP_L()
        {
            Test_Optimize_Recalculate_Negative_JMPX_L(VM.OpCode.JMP_L);
            Test_Optimize_Recalculate_Positive_JMPX_L(VM.OpCode.JMP_L);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_CALL_L()
        {
            Test_Optimize_Recalculate_Negative_JMPX_L(VM.OpCode.CALL_L);
            Test_Optimize_Recalculate_Positive_JMPX_L(VM.OpCode.CALL_L);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_JMPEQ_L()
        {
            Test_Optimize_Recalculate_Negative_JMPX_L(VM.OpCode.JMPEQ_L);
            Test_Optimize_Recalculate_Positive_JMPX_L(VM.OpCode.JMPEQ_L);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_JMPGE_L()
        {
            Test_Optimize_Recalculate_Negative_JMPX_L(VM.OpCode.JMPGE_L);
            Test_Optimize_Recalculate_Positive_JMPX_L(VM.OpCode.JMPGE_L);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_JMPGT_L()
        {
            Test_Optimize_Recalculate_Negative_JMPX_L(VM.OpCode.JMPGT_L);
            Test_Optimize_Recalculate_Positive_JMPX_L(VM.OpCode.JMPGT_L);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_JMPIFNOT_L()
        {
            Test_Optimize_Recalculate_Negative_JMPX_L(VM.OpCode.JMPIFNOT_L);
            Test_Optimize_Recalculate_Positive_JMPX_L(VM.OpCode.JMPIFNOT_L);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_JMPIF_L()
        {
            Test_Optimize_Recalculate_Negative_JMPX_L(VM.OpCode.JMPIF_L);
            Test_Optimize_Recalculate_Positive_JMPX_L(VM.OpCode.JMPIF_L);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_JMPLE_L()
        {
            Test_Optimize_Recalculate_Negative_JMPX_L(VM.OpCode.JMPLE_L);
            Test_Optimize_Recalculate_Positive_JMPX_L(VM.OpCode.JMPLE_L);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_JMPLT_L()
        {
            Test_Optimize_Recalculate_Negative_JMPX_L(VM.OpCode.JMPLT_L);
            Test_Optimize_Recalculate_Positive_JMPX_L(VM.OpCode.JMPLT_L);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_JMPNE_L()
        {
            Test_Optimize_Recalculate_Negative_JMPX_L(VM.OpCode.JMPNE_L);
            Test_Optimize_Recalculate_Positive_JMPX_L(VM.OpCode.JMPNE_L);
        }

        [TestMethod]
        public void Test_CombinedRules()
        {
            using (var scriptBefore = new ScriptBuilder())
            using (var scriptAfter = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.NOP);
                scriptBefore.Emit(VM.OpCode.PUSH0);
                scriptBefore.Emit(VM.OpCode.NOP);
                scriptBefore.Emit(VM.OpCode.EQUAL);

                var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), OptimizeParserType.DELETE_USELESS_EQUAL, OptimizeParserType.DELETE_NOP);

                scriptAfter.Emit(VM.OpCode.PUSH0);

                CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
            }
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_BoolEqualTrue()
        {
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(VM.OpCode.PUSH1);
            scriptBefore.Emit(VM.OpCode.PUSH1);
            scriptBefore.Emit(VM.OpCode.EQUAL);
            scriptBefore.Emit(VM.OpCode.NOP);

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.Emit(VM.OpCode.PUSH1);
            scriptAfter.Emit(VM.OpCode.NOP);

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), OptimizeParserType.DELETE_USELESS_EQUAL);

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }
        [TestMethod]
        public void Test_OptimizeSkip_Recalculate_BoolEqualTrue()
        {
            //jmp will cause skip this equal optimize
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(VM.OpCode.JMP, new byte[2]);
            scriptBefore.Emit(VM.OpCode.PUSH1);
            scriptBefore.Emit(VM.OpCode.PUSH1);
            scriptBefore.Emit(VM.OpCode.EQUAL);
            scriptBefore.Emit(VM.OpCode.NOP);

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.Emit(VM.OpCode.PUSH1);
            scriptAfter.Emit(VM.OpCode.NOP);

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), OptimizeParserType.DELETE_USELESS_EQUAL);

            CollectionAssert.AreNotEqual(scriptAfter.ToArray(), optimized);
            CollectionAssert.AreEqual(scriptBefore.ToArray(), optimized);
        }
        [TestMethod]
        public void Test_Optimize_Recalculate_BoolEqualFalse()
        {
            using (var scriptBefore = new ScriptBuilder())
            using (var scriptAfter = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.NOP);
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.PUSH0);
                scriptBefore.Emit(VM.OpCode.EQUAL);

                var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), OptimizeParserType.DELETE_USELESS_EQUAL);

                scriptAfter.Emit(VM.OpCode.NOP);
                scriptAfter.Emit(VM.OpCode.PUSH0);

                CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
            }

            using (var scriptBefore = new ScriptBuilder())
            using (var scriptAfter = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.NOP);
                scriptBefore.Emit(VM.OpCode.PUSH0);
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.EQUAL);

                var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), OptimizeParserType.DELETE_USELESS_EQUAL);

                scriptAfter.Emit(VM.OpCode.NOP);
                scriptAfter.Emit(VM.OpCode.PUSH0);

                CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
            }
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_Positive_PUSHA()
        {
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(VM.OpCode.PUSHA, ToJumpLArg(7));  // ─┐
            scriptBefore.Emit(VM.OpCode.NOP);                   //  │
            scriptBefore.Emit(VM.OpCode.NOP);                   //  │
            scriptBefore.Emit(VM.OpCode.RET);                   // <┘

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray());

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.Emit(VM.OpCode.PUSHA, ToJumpLArg(5));   // ─┐
            scriptAfter.Emit(VM.OpCode.RET);                    // <┘

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_Negative_PUSHA()
        {
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(VM.OpCode.PUSHA, ToJumpLArg(9));      // ───┐
            scriptBefore.Emit(VM.OpCode.PUSH1);                     // <┐ │
            scriptBefore.Emit(VM.OpCode.RET);                       //  │ │
            scriptBefore.Emit(VM.OpCode.NOP);                       //  │ │
            scriptBefore.Emit(VM.OpCode.NOP);                       //  │ │
            scriptBefore.Emit(VM.OpCode.PUSHA, ToJumpLArg(-4));     //  x<┘

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.Emit(VM.OpCode.PUSHA, ToJumpLArg(7));       // ───┐
            scriptAfter.Emit(VM.OpCode.PUSH1);                      // <┐ │
            scriptAfter.Emit(VM.OpCode.RET);                        //  │ │
            scriptAfter.Emit(VM.OpCode.PUSHA, ToJumpLArg(-2));      //  x<┘

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray());

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        private void Test_Optimize_Recalculate_Positive_JMPX_L(VM.OpCode biGJumpOpCode)
        {
            var smallJumpOpCode = (VM.OpCode)(biGJumpOpCode - 1);

            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(biGJumpOpCode, ToJumpLArg(7));    // ─┐
            scriptBefore.Emit(VM.OpCode.NOP);                   //  │
            scriptBefore.Emit(VM.OpCode.NOP);                   //  │
            scriptBefore.Emit(VM.OpCode.RET);                   // <┘

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray());

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.Emit(smallJumpOpCode, ToJumpArg(2));    // ─┐
            scriptAfter.Emit(VM.OpCode.RET);                    // <┘

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        private void Test_Optimize_Recalculate_Negative_JMPX_L(VM.OpCode biGJumpOpCode)
        {
            var smallJumpOpCode = (VM.OpCode)(biGJumpOpCode - 1);

            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(biGJumpOpCode, ToJumpLArg(9));        // ───┐
            scriptBefore.Emit(VM.OpCode.PUSH1);                     // <┐ │
            scriptBefore.Emit(VM.OpCode.RET);                       //  │ │
            scriptBefore.Emit(VM.OpCode.NOP);                       //  │ │
            scriptBefore.Emit(VM.OpCode.NOP);                       //  │ │
            scriptBefore.Emit(biGJumpOpCode, ToJumpLArg(-4));       //  x<┘

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.Emit(smallJumpOpCode, ToJumpArg(4));        // ───┐
            scriptAfter.Emit(VM.OpCode.PUSH1);                      // <┐ │
            scriptAfter.Emit(VM.OpCode.RET);                        //  │ │
            scriptAfter.Emit(smallJumpOpCode, ToJumpArg(-2));       //  x<┘

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray());

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        [TestMethod]
        public void Test_Optimize_JMP_LNext()
        {
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(VM.OpCode.JMP_L, ToJumpLArg(5));       // ───┐
            scriptBefore.Emit(VM.OpCode.PUSH1);                      // <──┘

            // useshortaddress before deleteuselessjmp
            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), new OptimizeParserType[] { OptimizeParserType.USE_SHORT_ADDRESS, OptimizeParserType.DELETE_USELESS_JMP });

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.Emit(VM.OpCode.PUSH1);
            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);

            // deleteuselessjmp before useshortaddress
            optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), new OptimizeParserType[] { OptimizeParserType.DELETE_USELESS_JMP, OptimizeParserType.USE_SHORT_ADDRESS });
            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);

            // use deleteuselessjmp only
            optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), new OptimizeParserType[] { OptimizeParserType.DELETE_USELESS_JMP });
            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        [TestMethod]
        public void Test_Optimize_JMP_Next()
        {
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(VM.OpCode.JMP, ToJumpArg(2));        // ───┐
            scriptBefore.Emit(VM.OpCode.PUSH1);                    // <──┘

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), new OptimizeParserType[] { OptimizeParserType.DELETE_USELESS_JMP });

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.Emit(VM.OpCode.PUSH1);

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        private byte[] ToJumpLArg(int value)
        {
            var ret = new byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(ret, value);
            return ret;
        }

        private byte[] ToJumpArg(int value)
        {
            return new byte[1] { (byte)value };
        }
    }
}
