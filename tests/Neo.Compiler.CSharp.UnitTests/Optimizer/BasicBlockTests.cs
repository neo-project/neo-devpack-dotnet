using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Optimizer;
using Neo.SmartContract.Testing;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class BasicBlockTests
    {
        [TestMethod]
        public void Test_BasicBlockIfReturn()
        {
            //public static BigInteger Factorial(BigInteger a)
            //{
            //    ExecutionEngine.Assert(a >= 0, "Minus number not supported");
            //    if (a >= 2) return a * Factorial(a - 1);
            //    return 1;
            //}
            ContractInBasicBlocks contract = new(Contract_Recursion.Nef, Contract_Recursion.Manifest);
            List<BasicBlock> blocks = contract.sortedBasicBlocks;
            Assert.AreEqual(blocks[0].nextBlock, blocks[1]);
            Assert.AreEqual(blocks[0].jumpTargetBlocks.Count, 1);
            Assert.AreEqual(blocks[0].jumpTargetBlocks.First(), blocks[2]);
            Assert.AreEqual(blocks[0].instructions.Last().OpCode, VM.OpCode.JMPIF);
            Assert.AreEqual(blocks[1].instructions.Last().OpCode, VM.OpCode.ABORTMSG);
            Assert.AreEqual(blocks[1].nextBlock, null);
            Assert.AreEqual(blocks[4].instructions.Last().OpCode, VM.OpCode.RET);
        }
    }
}
