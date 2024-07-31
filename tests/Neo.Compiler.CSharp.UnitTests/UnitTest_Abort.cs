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
    public class UnitTest_Abort : TestBase<Contract_Abort>
    {
        private readonly JObject _debugInfo;
        private readonly bool[] falseTrue = [false, true];

        public UnitTest_Abort() : base(Contract_Abort.Nef, Contract_Abort.Manifest)
        {
            var contract = TestCleanup.EnsureArtifactsUpToDateInternal().Where(u => u.ContractName == "Contract_Abort").First();
            _debugInfo = contract.CreateDebugInformation();
        }

        [TestMethod]
        public void Test_Abort()
        {
            // All the ABORT instruction addresses in "testAbort" method
            List<int> AbortAddresses = DumpNef.OpCodeAddressesInMethod(Contract_Abort.Nef, _debugInfo, "testAbort", OpCode.ABORT);
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAbort());
            Assert.AreEqual(1021260, Engine.FeeConsumed.Value);
            Assert.AreEqual(exception.CurrentContext?.InstructionPointer, AbortAddresses[0]);  // stop at the 1st ABORT
            Assert.AreEqual(exception.CurrentContext?.LocalVariables?[0].GetInteger(), 0);  // v==0
            Assert.AreEqual(exception.State, VMState.FAULT);
        }

        [TestMethod]
        public void Test_AbortMsg()
        {
            // All the ABORTMSG instruction addresses in "testAbortMsg" method
            List<int> AbortAddresses = DumpNef.OpCodeAddressesInMethod(Contract_Abort.Nef, _debugInfo, "testAbortMsg", OpCode.ABORTMSG);
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestAbortMsg());
            Assert.AreEqual(1021500, Engine.FeeConsumed.Value);
            Assert.AreEqual(exception.CurrentContext?.InstructionPointer, AbortAddresses[0]);  // stop at the 1st ABORTMSG
            Assert.AreEqual(exception.CurrentContext?.LocalVariables?[0].GetInteger(), 0);  // v==0
            Assert.AreEqual(exception.State, VMState.FAULT);
        }

        [TestMethod]
        public void Test_AbortInFunction()
        {
            foreach (bool b in falseTrue)
            {
                Assert.ThrowsException<TestException>(() => Contract.TestAbortInFunction(b));
            }
        }

        [TestMethod]
        public void Test_AbortInTry()
        {
            foreach (bool b in falseTrue)
            {
                Assert.ThrowsException<TestException>(() => Contract.TestAbortInTry(b));
            }
        }

        [TestMethod]
        public void Test_AbortInCatch()
        {
            foreach (bool b in falseTrue)
            {
                Assert.ThrowsException<TestException>(() => Contract.TestAbortInCatch(b));
            }
        }

        [TestMethod]
        public void Test_AbortInFinally()
        {
            foreach (bool b in falseTrue)
            {
                Assert.ThrowsException<TestException>(() => Contract.TestAbortInFinally(b));
            }
        }
    }
}
