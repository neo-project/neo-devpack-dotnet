using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using Neo.Optimizer;
using System.Collections.Generic;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Abort
    {
        TestEngine testengine;
        [TestInitialize]
        public void Initialize()
        {
            testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Abort.cs");
        }

        [TestMethod]
        public void Test_Abort()
        {
            Assert.AreEqual(testengine.ExecuteTestCaseStandard("testAbort").Count, 0);
            List<int> AbortAddresses = DumpNef.OpCodeAddressesInMethod(testengine.Nef, testengine.DebugInfo, "testAbort", OpCode.ABORT);
            Assert.AreEqual(testengine.CurrentContext.InstructionPointer, AbortAddresses[0]);
            Assert.AreEqual(testengine.CurrentContext.LocalVariables[0].GetInteger(), 0);
            Assert.AreEqual(testengine.State, VMState.FAULT);
        }

        [TestMethod]
        public void Test_AbortInFunction()
        {
            Assert.AreEqual(testengine.ExecuteTestCaseStandard("testAbortInFunction").Count, 0);
            Assert.AreEqual(testengine.State, VMState.FAULT);
        }

        [TestMethod]
        public void Test_AbortInTry()
        {
            Assert.AreEqual(testengine.ExecuteTestCaseStandard("testAbortInTry").Count, 0);
            Assert.AreEqual(testengine.State, VMState.FAULT);
        }

        [TestMethod]
        public void Test_AbortInCatch()
        {
            Assert.AreEqual(testengine.ExecuteTestCaseStandard("testAbortInCatch").Count, 0);
            Assert.AreEqual(testengine.State, VMState.FAULT);
        }

        [TestMethod]
        public void Test_AbortInFinally()
        {
            Assert.AreEqual(testengine.ExecuteTestCaseStandard("testAbortInFinally").Count, 0);
            Assert.AreEqual(testengine.State, VMState.FAULT);
        }
    }
}
