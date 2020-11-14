using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.Compiler.Optimizer;
using Neo.IO.Json;
using Neo.VM;
using System;
using System.Buffers.Binary;
using System.Numerics;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_NefOptimizer
    {
        [TestMethod]
        public void Test_OptimizerNopEntryPoint()
        {
            var testengine = new TestEngine();
            var build = testengine.Build("./TestClasses/Contract_OptimizationTest.cs", false, true);
            var methods = (build.finalABI["methods"] as JArray);

            Assert.AreEqual(methods[0]["name"].AsString(), "_initialize");
            Assert.AreEqual(methods[0]["offset"].AsString(), "0");
            Assert.AreEqual(methods[1]["name"].AsString(), "dummyMethod");
            Assert.AreEqual(methods[1]["offset"].AsString(), "26");
            Assert.AreEqual(methods[2]["name"].AsString(), "verify");
            Assert.AreEqual(methods[2]["offset"].AsString(), "33");
        }

        [TestMethod]
        public void Test_Optimize_RemoveNOPS()
        {
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(VM.OpCode.NOP);
            scriptBefore.Emit(VM.OpCode.NOP);

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(),
                OptimizeParserType.DELETE_DEAD_CODE |
                OptimizeParserType.USE_SHORT_ADDRESS
                );

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
                scriptBefore.Emit(VM.OpCode.PUSH0);
                scriptBefore.Emit(VM.OpCode.EQUAL);

                var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_USELESS_EQUAL | OptimizeParserType.DELETE_NOP);

                scriptAfter.Emit(VM.OpCode.PUSH1);

                CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
            }
        }

        [TestMethod]
        public void Test_Optimize_StaticMath_ADD()
        {
            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.ADD);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.Emit(VM.OpCode.PUSH2);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSHM1);
                scriptBefore.Emit(VM.OpCode.PUSH11);
                scriptBefore.Emit(VM.OpCode.ADD);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.Emit(VM.OpCode.PUSH10);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }
        }

        [TestMethod]
        public void Test_Optimize_StaticMath_INC()
        {
            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.INC);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(2);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.EmitPush(int.MaxValue);
                scriptBefore.Emit(VM.OpCode.INC);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(new BigInteger(int.MaxValue) + 1);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }
        }

        [TestMethod]
        public void Test_Optimize_StaticMath_ISNULL()
        {
            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.ISNULL);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.Emit(VM.OpCode.PUSH0);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSHNULL);
                scriptBefore.Emit(VM.OpCode.ISNULL);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.Emit(VM.OpCode.PUSH1);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }
        }

        [TestMethod]
        public void Test_Optimize_Static_DROP()
        {
            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH0);
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.DROP);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.Emit(VM.OpCode.PUSH0);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.PUSHNULL);
                scriptBefore.Emit(VM.OpCode.DROP);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.Emit(VM.OpCode.PUSH1);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }
        }

        [TestMethod]
        public void Test_Optimize_StaticMath_DEC()
        {
            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH0);
                scriptBefore.Emit(VM.OpCode.DEC);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(-1);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.EmitPush(short.MaxValue + 1);
                scriptBefore.Emit(VM.OpCode.DEC);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(short.MaxValue);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }
        }

        [TestMethod]
        public void Test_Optimize_StaticMath_NEGATE()
        {
            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH0);
                scriptBefore.Emit(VM.OpCode.NEGATE);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(0);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.EmitPush(1);
                scriptBefore.Emit(VM.OpCode.NEGATE);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(-1);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.EmitPush(-1);
                scriptBefore.Emit(VM.OpCode.NEGATE);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(1);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }
        }

        [TestMethod]
        public void Test_Optimize_StaticMath_SIGN()
        {
            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH0);
                scriptBefore.Emit(VM.OpCode.SIGN);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(0);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.EmitPush(1);
                scriptBefore.Emit(VM.OpCode.SIGN);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(1);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.EmitPush(-1);
                scriptBefore.Emit(VM.OpCode.SIGN);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(-1);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }
        }

        [TestMethod]
        public void Test_Optimize_StaticMath_SUB()
        {
            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH10);
                scriptBefore.Emit(VM.OpCode.PUSH11);
                scriptBefore.Emit(VM.OpCode.SUB);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(-1);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH11);
                scriptBefore.Emit(VM.OpCode.PUSH10);
                scriptBefore.Emit(VM.OpCode.SUB);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(1);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.EmitPush(ushort.MaxValue);
                scriptBefore.EmitPush(short.MaxValue);
                scriptBefore.Emit(VM.OpCode.SUB);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(ushort.MaxValue - short.MaxValue);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }
        }

        [TestMethod]
        public void Test_Optimize_StaticMath_DIV()
        {
            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH10);
                scriptBefore.Emit(VM.OpCode.PUSH11);
                scriptBefore.Emit(VM.OpCode.DIV);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(10 / 11);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH11);
                scriptBefore.Emit(VM.OpCode.PUSH10);
                scriptBefore.Emit(VM.OpCode.DIV);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(11 / 10);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.EmitPush(ushort.MaxValue);
                scriptBefore.EmitPush(short.MaxValue);
                scriptBefore.Emit(VM.OpCode.DIV);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(BigInteger.Divide(new BigInteger(ushort.MaxValue), new BigInteger(short.MaxValue)));

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }
        }

        [TestMethod]
        public void Test_Optimize_StaticMath_MOD()
        {
            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH10);
                scriptBefore.Emit(VM.OpCode.PUSH11);
                scriptBefore.Emit(VM.OpCode.MOD);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(10 % 11);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH10);
                scriptBefore.Emit(VM.OpCode.PUSH4);
                scriptBefore.Emit(VM.OpCode.MOD);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(10 / 4);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.EmitPush(ushort.MaxValue);
                scriptBefore.EmitPush(short.MaxValue);
                scriptBefore.Emit(VM.OpCode.MOD);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(new BigInteger(ushort.MaxValue) % new BigInteger(short.MaxValue));

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }
        }

        [TestMethod]
        public void Test_Optimize_StaticMath_ABS()
        {
            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH0);
                scriptBefore.Emit(VM.OpCode.ABS);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(0);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.EmitPush(1);
                scriptBefore.Emit(VM.OpCode.ABS);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(1);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.EmitPush(-1);
                scriptBefore.Emit(VM.OpCode.ABS);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(1);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }
        }

        [TestMethod]
        public void Test_Optimize_StaticMath_MUL()
        {
            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.PUSH2);
                scriptBefore.Emit(VM.OpCode.MUL);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.Emit(VM.OpCode.PUSH2);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.EmitPush(int.MaxValue);
                scriptBefore.EmitPush(int.MaxValue);
                scriptBefore.Emit(VM.OpCode.MUL);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.EmitPush(BigInteger.Multiply(new BigInteger(int.MaxValue), new BigInteger(int.MaxValue)));

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }
        }

        [TestMethod]
        public void Test_Optimize_ConstExecution_ROT()
        {
            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.PUSH2);
                scriptBefore.Emit(VM.OpCode.PUSH3);
                scriptBefore.Emit(VM.OpCode.ROT);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.Emit(VM.OpCode.PUSH2);
                    scriptAfter.Emit(VM.OpCode.PUSH3);
                    scriptAfter.Emit(VM.OpCode.PUSH1);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.PUSH2);
                scriptBefore.Emit(VM.OpCode.PUSHNULL);
                scriptBefore.Emit(VM.OpCode.ROT);

                using (var scriptAfter = new ScriptBuilder())
                {
                    scriptAfter.Emit(VM.OpCode.PUSH2);
                    scriptAfter.Emit(VM.OpCode.PUSHNULL);
                    scriptAfter.Emit(VM.OpCode.PUSH1);

                    var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                    CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
                }
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.PUSH5);
                scriptBefore.Emit(VM.OpCode.PUSH4);
                scriptBefore.EmitJump(VM.OpCode.JMP, 3);
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.PUSH2);
                scriptBefore.Emit(VM.OpCode.PUSH3);
                scriptBefore.Emit(VM.OpCode.ROT);

                var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_CONST_EXECUTION);
                CollectionAssert.AreEqual(scriptBefore.ToArray(), optimized);
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

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_USELESS_EQUAL);

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_BoolNotEqualTrue()
        {
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(VM.OpCode.PUSH1);
            scriptBefore.Emit(VM.OpCode.PUSH1);
            scriptBefore.Emit(VM.OpCode.NOTEQUAL);
            scriptBefore.Emit(VM.OpCode.NOP);

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.Emit(VM.OpCode.PUSH0);
            scriptAfter.Emit(VM.OpCode.NOP);

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_USELESS_EQUAL);

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        [TestMethod]
        public void Test_OptimizeSkip_Recalculate_BoolEqualTrue()
        {
            //jmp will cause skip this equal optimize
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(VM.OpCode.JMP, new byte[2] { 0x04, 0x00 });
            scriptBefore.Emit(VM.OpCode.PUSH1);
            scriptBefore.Emit(VM.OpCode.PUSH1);
            scriptBefore.Emit(VM.OpCode.EQUAL);
            scriptBefore.Emit(VM.OpCode.NOP);

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.Emit(VM.OpCode.PUSH1);
            scriptAfter.Emit(VM.OpCode.NOP);

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_USELESS_EQUAL);

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

                var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_USELESS_EQUAL);

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

                var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_USELESS_EQUAL);

                scriptAfter.Emit(VM.OpCode.NOP);
                scriptAfter.Emit(VM.OpCode.PUSH0);

                CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
            }
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_BoolNotEqualFalse()
        {
            using (var scriptBefore = new ScriptBuilder())
            using (var scriptAfter = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.NOP);
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.PUSH0);
                scriptBefore.Emit(VM.OpCode.NOTEQUAL);

                var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_USELESS_EQUAL);

                scriptAfter.Emit(VM.OpCode.NOP);
                scriptAfter.Emit(VM.OpCode.PUSH1);

                CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
            }

            using (var scriptBefore = new ScriptBuilder())
            using (var scriptAfter = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.NOP);
                scriptBefore.Emit(VM.OpCode.PUSH0);
                scriptBefore.Emit(VM.OpCode.PUSH1);
                scriptBefore.Emit(VM.OpCode.NOTEQUAL);

                var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_USELESS_EQUAL);

                scriptAfter.Emit(VM.OpCode.NOP);
                scriptAfter.Emit(VM.OpCode.PUSH1);

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

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(),
                OptimizeParserType.DELETE_DEAD_CODE |
                OptimizeParserType.USE_SHORT_ADDRESS
                );

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

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(),
                OptimizeParserType.DELETE_DEAD_CODE |
                OptimizeParserType.USE_SHORT_ADDRESS
                );

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

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(),
                OptimizeParserType.DELETE_DEAD_CODE |
                OptimizeParserType.USE_SHORT_ADDRESS
                );

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

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(),
                OptimizeParserType.DELETE_DEAD_CODE |
                OptimizeParserType.USE_SHORT_ADDRESS
                );

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        [TestMethod]
        public void Test_Optimize_JMP_LNext()
        {
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(VM.OpCode.JMP_L, ToJumpLArg(5));       // ───┐
            scriptBefore.Emit(VM.OpCode.PUSH1);                      // <──┘

            // useshortaddress before deleteuselessjmp
            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.USE_SHORT_ADDRESS | OptimizeParserType.DELETE_USELESS_JMP);

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.Emit(VM.OpCode.PUSH1);
            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);

            // deleteuselessjmp before useshortaddress
            optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_USELESS_JMP | OptimizeParserType.USE_SHORT_ADDRESS);
            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);

            // use deleteuselessjmp only
            optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_USELESS_JMP);
            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        [TestMethod]
        public void Test_Optimize_JMP_Next()
        {
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(VM.OpCode.JMP, ToJumpArg(2));        // ───┐
            scriptBefore.Emit(VM.OpCode.PUSH1);                    // <──┘

            var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_USELESS_JMP);

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.Emit(VM.OpCode.PUSH1);

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        [TestMethod]
        public void Test_Optimize_Delete_Dead_Code()
        {
            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.NOP);
                scriptBefore.Emit(VM.OpCode.JMP, ToJumpArg(8));        //  ─────┐
                scriptBefore.Emit(VM.OpCode.PUSH1);                    //       |
                scriptBefore.Emit(VM.OpCode.PUSH2);                    //       |
                scriptBefore.Emit(VM.OpCode.PUSH3);                    //   ┌<┐ |
                scriptBefore.Emit(VM.OpCode.JMP, ToJumpArg(6));        //   x | |
                scriptBefore.Emit(VM.OpCode.PUSH4);                    //   | | |
                scriptBefore.Emit(VM.OpCode.JMP, ToJumpArg(-4));       //   | x<┘
                scriptBefore.Emit(VM.OpCode.NOP);                      //   |
                scriptBefore.Emit(VM.OpCode.PUSH5);                    // x<┘
                scriptBefore.Emit(VM.OpCode.NOP);
                scriptBefore.Emit(VM.OpCode.PUSH6);
                scriptBefore.Emit(VM.OpCode.RET);
                scriptBefore.Emit(VM.OpCode.PUSH7);

                var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_DEAD_CODE);

                using var scriptAfter = new ScriptBuilder();
                scriptAfter.Emit(VM.OpCode.JMP, ToJumpArg(5));
                scriptAfter.Emit(VM.OpCode.PUSH3);
                scriptAfter.Emit(VM.OpCode.JMP, ToJumpArg(4));
                scriptAfter.Emit(VM.OpCode.JMP, ToJumpArg(-3));
                scriptAfter.Emit(VM.OpCode.PUSH5);
                scriptAfter.Emit(VM.OpCode.PUSH6);
                scriptAfter.Emit(VM.OpCode.RET);

                CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
            }

            using (var scriptBefore = new ScriptBuilder())
            {
                scriptBefore.Emit(VM.OpCode.NOP);
                scriptBefore.Emit(VM.OpCode.JMP_L, ToJumpLArg(10));       //  ───┐
                scriptBefore.Emit(VM.OpCode.PUSH1);                       //     |
                scriptBefore.Emit(VM.OpCode.PUSH2);                       //     |
                scriptBefore.Emit(VM.OpCode.PUSH3);                       // x<┐ |
                scriptBefore.Emit(VM.OpCode.RET);                         //   | |
                scriptBefore.Emit(VM.OpCode.PUSH4);                       //   | |
                scriptBefore.Emit(VM.OpCode.JMP_L, ToJumpLArg(-3));       //   x<┘
                scriptBefore.Emit(VM.OpCode.PUSH5);

                var optimized = NefOptimizeTool.Optimize(scriptBefore.ToArray(), Array.Empty<int>(), OptimizeParserType.DELETE_DEAD_CODE);

                using var scriptAfter = new ScriptBuilder();
                scriptAfter.Emit(VM.OpCode.JMP_L, ToJumpLArg(7));
                scriptAfter.Emit(VM.OpCode.PUSH3);
                scriptAfter.Emit(VM.OpCode.RET);
                scriptAfter.Emit(VM.OpCode.JMP_L, ToJumpLArg(-2));

                CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
            }
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
