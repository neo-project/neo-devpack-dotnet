using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.SmartContract.TestEngine;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ConcatByteStringAddAssign
    {
        private TestEngine testengine;

        [TestInitialize]
        public void Init()
        {
            testengine = new TestEngine();
            testengine.AddEntryScript<Contract_ConcatByteStringAddAssign>();
        }

        [TestMethod]
        public void Test_ByteStringAdd()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("byteStringAddAssign", "a", "b", "c");

            Assert.AreEqual(1, result.Count);

            var r1 = result.Pop<ByteString>();
            Assert.AreEqual(r1.GetString(), "abc");
        }
    }
}
