using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Record : TestBase2<Contract_Record>
    {
        public UnitTest_Record() : base(Contract_Record.Nef, Contract_Record.Manifest) { }

        [TestMethod]
        public void Test_CreateRecord()
        {
            var name = "klsas";
            var age = 24;
            var result = Contract.Test_CreateRecord(name, age)!;
            Assert.AreEqual(2048820, Engine.FeeConsumed.Value);
            var arr = result as Struct;
            Assert.AreEqual(2, arr!.Count);
            Assert.AreEqual(name, arr[0].GetString());
            Assert.AreEqual(age, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_CreateRecord2()
        {
            var name = "klsas";
            var age = 24;
            var result = Contract.Test_CreateRecord2(name, age)!;
            Assert.AreEqual(2048970, Engine.FeeConsumed.Value);
            var arr = result as Struct;
            Assert.AreEqual(2, arr!.Count);
            Assert.AreEqual(name, arr[0].GetString());
            Assert.AreEqual(age, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_UpdateRecord()
        {
            var name = "klsas";
            var age = 24;
            var result = Contract.Test_UpdateRecord(name, age)! as Struct;
            Assert.AreEqual(2435700, Engine.FeeConsumed.Value);
            Assert.AreEqual(2, result!.Count);
            Assert.AreEqual(name, result[0].GetString());
            Assert.AreEqual(age, result[1].GetInteger());
        }

        [TestMethod]
        public void Test_UpdateRecord2()
        {
            var name = "klsas";
            var age = 2;
            var result = Contract.Test_UpdateRecord2(name, age)!;
            Assert.AreEqual(3006450, Engine.FeeConsumed.Value);
            var arr = result as Struct;
            Assert.AreEqual(2, arr!.Count);
            Assert.AreEqual("0" + name, arr[0].GetString());
            Assert.AreEqual(age + 1, arr[1].GetInteger());
        }

        [TestMethod]
        public void Test_DeconstructRecord()
        {
            var name = "klsas";
            var age = 24;
            var result = Contract.Test_DeconstructRecord(name, age)!;
            Assert.AreEqual(2110620, Engine.FeeConsumed.Value);
            Assert.AreEqual(name, result);
        }
    }
}
