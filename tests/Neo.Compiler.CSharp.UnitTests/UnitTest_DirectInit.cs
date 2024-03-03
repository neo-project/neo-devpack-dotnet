using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM.Types;
using System;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_DirectInit
    {

        [TestMethod]
        public void Test_GetUInt160()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_DirectInit.cs");
            var result = testengine.ExecuteTestCaseStandard("testGetUInt160");
            var value = result.Pop().GetSpan();

            Assert.AreEqual(value.ToArray().ToHexString(), "7eee1aabeb67ed1d791d44e4f5fcf3ae9171a871");
        }

        [TestMethod]
        public void Test_GetECPoint()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_DirectInit.cs");
            var result = testengine.ExecuteTestCaseStandard("testGetECPoint");
            var value = result.Pop().GetSpan();
            Assert.AreEqual(value.ToHexString(), "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9");
        }

        [TestMethod]
        public void Test_GetUInt256()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_DirectInit.cs");
            var result = testengine.ExecuteTestCaseStandard("testGetUInt256");
            var value = result.Pop().GetSpan();

            Assert.AreEqual(value.ToArray().ToHexString(), "edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925");
        }

        [TestMethod]
        public void Test_GetString()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_DirectInit.cs");
            var result = testengine.ExecuteTestCaseStandard("testGetString");
            var value = result.Pop().GetString();

            Assert.AreEqual(value, "hello world");
        }
    }
}
