using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Assert : DebugAndTestBase<Contract_Assert>
    {
        private readonly JObject _debugInfo;

        public UnitTest_Assert()
        {
            var contract = TestCleanup.EnsureArtifactUpToDateInternal(nameof(Contract_Assert));
            _debugInfo = contract.CreateDebugInformation();
        }

        public void AssertsInFalse(TestException exception)
        {
            // All the ASSERT opcode addresses in method testAssertFalse
            List<int> assertAddresses = DumpNef.OpCodeAddressesInMethod(Contract_Assert.Nef, _debugInfo, "testAssertFalse", OpCode.ASSERT);
            Assert.AreEqual(exception.CurrentContext?.InstructionPointer, assertAddresses[1]);  // stops at the 2nd ASSERT
            Assert.AreEqual(exception.CurrentContext?.LocalVariables?[0].GetInteger(), 1);  // v==1
            Assert.AreEqual(exception.State, VMState.FAULT);
        }

        [TestMethod]
        public void Test_AssertFalse()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAssertFalse());
            AssertGasConsumed(986250);
            AssertsInFalse(exception);
        }

        [TestMethod]
        public void Test_AssertInFunction()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAssertInFunction());
            AssertGasConsumed(1003620);
            AssertsInFalse(exception);
            Assert.AreEqual(exception.InvocationStack?.ToArray()?[1]?.LocalVariables?[0].GetInteger(), 0);  // v==0
        }

        [TestMethod]
        public void Test_AssertInTry()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAssertInTry());
            AssertGasConsumed(1003740);
            AssertsInFalse(exception);
            Assert.AreEqual(exception.InvocationStack?.ToArray()?[1]?.LocalVariables?[0].GetInteger(), 0);  // v==0
        }

        [TestMethod]
        public void Test_AssertInCatch()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAssertInCatch());
            AssertGasConsumed(1019490);
            AssertsInFalse(exception);
            Assert.AreEqual(exception.InvocationStack?.ToArray()?[1]?.LocalVariables?[0].GetInteger(), 1);  // v==1
        }

        [TestMethod]
        public void Test_AssertInFinally()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAssertInFinally());
            AssertGasConsumed(1003950);
            AssertsInFalse(exception);
            Assert.AreEqual(exception.InvocationStack?.ToArray()?[1]?.LocalVariables?[0].GetInteger(), 1);  // v==1
        }
    }
}
