using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM.Types;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Record
    {
        private TestEngine testEngine;

        [TestInitialize]
        public void Init()
        {
            testEngine = new TestEngine();
            testEngine.AddNoOptimizeEntryScript(Utils.Extensions.TestContractRoot + "Contract_Record.cs");
        }

        [TestMethod]
        public void Test_CreateRecord()
        {
            testEngine.Reset();
            var name = "klsas";
            var age = 24;
            var result = testEngine.ExecuteTestCaseStandard("test_CreateRecord", name, age);
            var arr = result.Pop<Struct>();
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual(name, arr[0].GetString());
            Assert.AreEqual(age, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_CreateRecord2()
        {
            testEngine.Reset();
            var name = "klsas";
            var age = 24;
            var result = testEngine.ExecuteTestCaseStandard("test_CreateRecord2", name, age);
            var arr = result.Pop<Struct>();
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual(name, arr[0].GetString());
            Assert.AreEqual(age, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_UpdateRecord()
        {
            testEngine.Reset();
            var name = "klsas";
            var age = 24;
            var result = testEngine.ExecuteTestCaseStandard("test_UpdateRecord", name, age);
            var arr = result.Pop<Struct>();
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual(name, arr[0].GetString());
            Assert.AreEqual(age, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_UpdateRecord2()
        {
            testEngine.Reset();
            var name = "klsas";
            var age = 2;
            var result = testEngine.ExecuteTestCaseStandard("test_UpdateRecord2", name, age);
            var arr = result.Pop<Struct>();
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual("0" + name, arr[0].GetString());
            Assert.AreEqual(age + 1, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_DeconstructRecord()
        {
            testEngine.Reset();
            var name = "klsas";
            var age = 24;
            var result = testEngine.ExecuteTestCaseStandard("test_DeconstructRecord", name, age);
            var arr = result.Pop().GetString();
            Assert.AreEqual(name, arr);
        }
    }
}
