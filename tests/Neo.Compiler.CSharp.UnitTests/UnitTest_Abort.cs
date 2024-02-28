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
        bool[] falseTrue = new bool[] { false, true };
        [TestInitialize]
        public void Initialize()
        {
            testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Abort.cs");
        }

        [TestMethod]
        public void Test_Abort()
        {
            string method = "testAbort";
            Assert.AreEqual(testengine.ExecuteTestCaseStandard(method).Count, 0);
            // All the ABORT instruction addresses in "testAbort" method
            List<int> AbortAddresses = DumpNef.OpCodeAddressesInMethod(testengine.Nef, testengine.DebugInfo, method, OpCode.ABORT);
            Assert.AreEqual(testengine.CurrentContext.InstructionPointer, AbortAddresses[0]);  // stop at the 1st ABORT
            Assert.AreEqual(testengine.CurrentContext.LocalVariables[0].GetInteger(), 0);  // v==0
            Assert.AreEqual(testengine.State, VMState.FAULT);
        }

        [TestMethod]
        public void Test_AbortMsg()
        {
            string method = "testAbortMsg";
            Assert.AreEqual(testengine.ExecuteTestCaseStandard(method).Count, 0);
            // All the ABORTMSG instruction addresses in "testAbortMsg" method
            List<int> AbortAddresses = DumpNef.OpCodeAddressesInMethod(testengine.Nef, testengine.DebugInfo, method, OpCode.ABORTMSG);
            Assert.AreEqual(testengine.CurrentContext.InstructionPointer, AbortAddresses[0]);  // stop at the 1st ABORTMSG
            Assert.AreEqual(testengine.CurrentContext.LocalVariables[0].GetInteger(), 0);  // v==0
            Assert.AreEqual(testengine.State, VMState.FAULT);
        }

        [TestMethod]
        public void Test_AbortInFunction()
        {
            foreach (bool b in falseTrue)
            {
                Assert.AreEqual(testengine.ExecuteTestCaseStandard("testAbortInFunction", b).Count, 0);
                Assert.AreEqual(testengine.State, VMState.FAULT);
            }
        }

        [TestMethod]
        public void Test_AbortInTry()
        {
            foreach (bool b in falseTrue)
            {
                Assert.AreEqual(testengine.ExecuteTestCaseStandard("testAbortInTry", b).Count, 0);
                Assert.AreEqual(testengine.State, VMState.FAULT);
            }
        }

        [TestMethod]
        public void Test_AbortInCatch()
        {
            foreach (bool b in falseTrue)
            {
                Assert.AreEqual(testengine.ExecuteTestCaseStandard("testAbortInCatch", b).Count, 0);
                Assert.AreEqual(testengine.State, VMState.FAULT);
            }
        }

        [TestMethod]
        public void Test_AbortInFinally()
        {
            foreach (bool b in falseTrue)
            {
                Assert.AreEqual(testengine.ExecuteTestCaseStandard("testAbortInFinally", b).Count, 0);
                Assert.AreEqual(testengine.State, VMState.FAULT);
            }
        }
    }
}
