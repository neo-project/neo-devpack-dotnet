using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM.Types;
using System;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_StaticClass
    {
        private TestEngine testengine;

        [TestInitialize]
        public void Init()
        {
            testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_StaticClass.cs");
        }

        [TestMethod]
        public void Test_StaticClass()
        {
            var result = testengine.ExecuteTestCaseStandard("testStaticClass");

            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result.Pop(), 2);
        }
    }
}
