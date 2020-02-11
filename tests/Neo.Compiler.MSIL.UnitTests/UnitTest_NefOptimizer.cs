using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            var optimized = new NefOptimizer(scriptBefore.ToArray()).Optimize();

            using var scriptAfter = new ScriptBuilder();

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_Positive_JMP()
        {
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.EmitJump(VM.OpCode.JMP, 4);    // ─┐
            scriptBefore.Emit(VM.OpCode.NOP);           //  │
            scriptBefore.Emit(VM.OpCode.NOP);           //  │
            scriptBefore.Emit(VM.OpCode.RET);           // <┘

            var optimized = new NefOptimizer(scriptBefore.ToArray()).Optimize();

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.EmitJump(VM.OpCode.JMP, 2);     // ─┐
            scriptAfter.Emit(VM.OpCode.RET);            // <┘

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_Positive_JMP_L()
        {
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(VM.OpCode.JMP_L, ToJumpLArg(7));  // ─┐
            scriptBefore.Emit(VM.OpCode.NOP);                   //  │
            scriptBefore.Emit(VM.OpCode.NOP);                   //  │
            scriptBefore.Emit(VM.OpCode.RET);                   // <┘

            var optimized = new NefOptimizer(scriptBefore.ToArray()).Optimize();

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.EmitJump(VM.OpCode.JMP, 2);     // ─┐
            scriptAfter.Emit(VM.OpCode.RET);            // <┘

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_Negative_JMP()
        {
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.EmitJump(VM.OpCode.JMP, 6);    // ───┐
            scriptBefore.Emit(VM.OpCode.PUSH1);         // <┐ │
            scriptBefore.Emit(VM.OpCode.RET);           //  │ │
            scriptBefore.Emit(VM.OpCode.NOP);           //  │ │
            scriptBefore.Emit(VM.OpCode.NOP);           //  │ │
            scriptBefore.EmitJump(VM.OpCode.JMP, -4);   //  x<┘

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.EmitJump(VM.OpCode.JMP, 4);     // ───┐
            scriptAfter.Emit(VM.OpCode.PUSH1);          // <┐ │
            scriptAfter.Emit(VM.OpCode.RET);            //  │ │
            scriptAfter.EmitJump(VM.OpCode.JMP, -2);    //  x<┘

            var optimized = new NefOptimizer(scriptBefore.ToArray()).Optimize();

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        [TestMethod]
        public void Test_Optimize_Recalculate_Negative_JMP_L()
        {
            using var scriptBefore = new ScriptBuilder();
            scriptBefore.Emit(VM.OpCode.JMP_L, ToJumpLArg(9));      // ───┐
            scriptBefore.Emit(VM.OpCode.PUSH1);                     // <┐ │
            scriptBefore.Emit(VM.OpCode.RET);                       //  │ │
            scriptBefore.Emit(VM.OpCode.NOP);                       //  │ │
            scriptBefore.Emit(VM.OpCode.NOP);                       //  │ │
            scriptBefore.Emit(VM.OpCode.JMP_L, ToJumpLArg(-4));     //  x<┘

            using var scriptAfter = new ScriptBuilder();
            scriptAfter.EmitJump(VM.OpCode.JMP, 4);     // ───┐
            scriptAfter.Emit(VM.OpCode.PUSH1);          // <┐ │
            scriptAfter.Emit(VM.OpCode.RET);            //  │ │
            scriptAfter.EmitJump(VM.OpCode.JMP, -2);    //  x<┘

            var optimized = new NefOptimizer(scriptBefore.ToArray()).Optimize();

            CollectionAssert.AreEqual(scriptAfter.ToArray(), optimized);
        }

        private byte[] ToJumpLArg(int value)
        {
            var ret = new byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(ret, value);
            return ret;
        }
    }
}
