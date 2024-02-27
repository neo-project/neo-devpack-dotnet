using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using Neo.Optimizer;
using System.Collections.Generic;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Assert
    {
        TestEngine testengine;
        [TestInitialize]
        public void Initialize()
        {
            testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Assert.cs");
        }

        public void AssertsInFalse()
        {
            List<int> assertAddresses = DumpNef.OpCodeAddressesInMethod(testengine.Nef, testengine.DebugInfo, "testAssertFalse", OpCode.ASSERT);
            Assert.AreEqual(testengine.CurrentContext.InstructionPointer, assertAddresses[1]);
            Assert.AreEqual(testengine.CurrentContext.LocalVariables[0].GetInteger(), 1);
            Assert.AreEqual(testengine.State, VMState.FAULT);
        }

        [TestMethod]
        public void Test_AssertFalse()
        {
            Assert.AreEqual(testengine.ExecuteTestCaseStandard("testAssertFalse").Count, 0);
            AssertsInFalse();
        }

        [TestMethod]
        public void Test_AssertInFunction()
        {
            Assert.AreEqual(testengine.ExecuteTestCaseStandard("testAssertInFunction").Count, 0);
            AssertsInFalse();
            Assert.AreEqual(testengine.InvocationStack.ToArray()[1].LocalVariables[0].GetInteger(), 0);
        }

        [TestMethod]
        public void Test_AssertInTry()
        {
            Assert.AreEqual(testengine.ExecuteTestCaseStandard("testAssertInTry").Count, 0);
            AssertsInFalse();
            Assert.AreEqual(testengine.InvocationStack.ToArray()[1].LocalVariables[0].GetInteger(), 0);
        }

        [TestMethod]
        public void Test_AssertInCatch()
        {
            Assert.AreEqual(testengine.ExecuteTestCaseStandard("testAssertInCatch").Count, 0);
            AssertsInFalse();
            Assert.AreEqual(testengine.InvocationStack.ToArray()[1].LocalVariables[0].GetInteger(), 1);
        }

        [TestMethod]
        public void Test_AssertInFinally()
        {
            Assert.AreEqual(testengine.ExecuteTestCaseStandard("testAssertInFinally").Count, 0);
            AssertsInFalse();
            Assert.AreEqual(testengine.InvocationStack.ToArray()[1].LocalVariables[0].GetInteger(), 1);
        }
    }
}
