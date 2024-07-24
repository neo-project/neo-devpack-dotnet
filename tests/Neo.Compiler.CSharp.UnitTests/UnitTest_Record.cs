using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Record : TestBase<Contract_Record>
    {
        public UnitTest_Record() : base(Contract_Record.Nef, Contract_Record.Manifest) { }

        [TestMethod]
        public void Test_CreateRecord()
        {
            var name = "klsas";
            var age = 24;
            var result = Contract.Test_CreateRecord(name, age);
            Assert.AreEqual(2048880, Engine.FeeConsumed.Value);
            var arr = result as Struct;
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual(name, arr[0].GetString());
            Assert.AreEqual(age, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_CreateRecord2()
        {
            var name = "klsas";
            var age = 24;
            var result = Contract.Test_CreateRecord2(name, age);
            Assert.AreEqual(2049030, Engine.FeeConsumed.Value);
            var arr = result as Struct;
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual(name, arr[0].GetString());
            Assert.AreEqual(age, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_UpdateRecord()
        {
            var name = "klsas";
            var age = 24;
            var result = Contract.Test_UpdateRecord(name, age);
            Assert.AreEqual(2435760, Engine.FeeConsumed.Value);
            var arr = result as Struct;
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual(name, arr[0].GetString());
            Assert.AreEqual(age, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_UpdateRecord2()
        {
            var name = "klsas";
            var age = 2;
            var result = Contract.Test_UpdateRecord2(name, age);
            Assert.AreEqual(3006510, Engine.FeeConsumed.Value);
            var arr = result as Struct;
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual("0" + name, arr[0].GetString());
            Assert.AreEqual(age + 1, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_DeconstructRecord()
        {
            var name = "klsas";
            var age = 24;
            var result = Contract.Test_DeconstructRecord(name, age);
            Assert.AreEqual(2110680, Engine.FeeConsumed.Value);
            Assert.AreEqual(name, result);
        }
    }
}
