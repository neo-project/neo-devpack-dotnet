using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM.Types;
using System.Linq;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Record
    {
        [TestMethod]
        public void Test_CreateRecord()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Record.cs");

            var name = "klsas";
            var age = 24;
            var result = testengine.ExecuteTestCaseStandard("test_CreateRecord", name, age);
            var arr = result.Pop<Struct>();
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual(name, arr[0].GetString());
            Assert.AreEqual(age, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_CreateRecord2()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Record.cs");

            var name = "klsas";
            var age = 24;
            var result = testengine.ExecuteTestCaseStandard("test_CreateRecord2", name, age);
            var arr = result.Pop<Struct>();
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual(name, arr[0].GetString());
            Assert.AreEqual(age, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_UpdateRecord()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Record.cs");

            var name = "klsas";
            var age = 24;
            var result = testengine.ExecuteTestCaseStandard("test_UpdateRecord", name, age);
            var arr = result.Pop<Struct>();
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual(name, arr[0].GetString());
            Assert.AreEqual(age, arr[1].GetInteger());
        }


        [TestMethod]
        public void Test_UpdateRecord2()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Record.cs");

            var name = "klsas";
            var age = 2;
            var result = testengine.ExecuteTestCaseStandard("test_UpdateRecord2", name, age);
            var arr = result.Pop<Struct>();
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual("0" + name, arr[0].GetString());
            Assert.AreEqual(age + 1, arr[1].GetInteger());
        }


        [TestMethod]
        public void Test_DeconstructRecord()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Record.cs");

            var name = "klsas";
            var age = 24;
            var result = testengine.ExecuteTestCaseStandard("test_DeconstructRecord", name, age);
            var arr = result.Pop().GetString();
            Assert.AreEqual(name, arr);
        }
    }
}
