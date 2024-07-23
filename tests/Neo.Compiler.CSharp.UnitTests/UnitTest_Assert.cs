using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Assert : TestBase<Contract_Assert>
    {
        private readonly JObject _debugInfo;

        public UnitTest_Assert() : base(Contract_Assert.Nef, Contract_Assert.Manifest)
        {
            var contract = TestCleanup.EnsureArtifactsUpToDateInternal().Where(u => u.ContractName == "Contract_Assert").First();
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
            Assert.AreEqual(1002097390, Engine.FeeConsumed.Value);
            AssertsInFalse(exception);
        }

        [TestMethod]
        public void Test_AssertInFunction()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAssertInFunction());
            Assert.AreEqual(1002114820, Engine.FeeConsumed.Value);
            AssertsInFalse(exception);
            Assert.AreEqual(exception.InvocationStack?.ToArray()?[1]?.LocalVariables?[0].GetInteger(), 0);  // v==0
        }

        [TestMethod]
        public void Test_AssertInTry()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAssertInTry());
            Assert.AreEqual(1002114940, Engine.FeeConsumed.Value);
            AssertsInFalse(exception);
            Assert.AreEqual(exception.InvocationStack?.ToArray()?[1]?.LocalVariables?[0].GetInteger(), 0);  // v==0
        }

        [TestMethod]
        public void Test_AssertInCatch()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAssertInCatch());
            Assert.AreEqual(1002130810, Engine.FeeConsumed.Value);
            AssertsInFalse(exception);
            Assert.AreEqual(exception.InvocationStack?.ToArray()?[1]?.LocalVariables?[0].GetInteger(), 1);  // v==1
        }

        [TestMethod]
        public void Test_AssertInFinally()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAssertInFinally());
            Assert.AreEqual(1002115270, Engine.FeeConsumed.Value);
            AssertsInFalse(exception);
            Assert.AreEqual(exception.InvocationStack?.ToArray()?[1]?.LocalVariables?[0].GetInteger(), 1);  // v==1
        }
    }
}
