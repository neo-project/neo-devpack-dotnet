using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class ListTest : TestBase<Contract_List>
    {
        public ListTest() : base(Contract_List.Nef, Contract_List.Manifest) { }

        [TestMethod]
        public void TestCount()
        {
            Assert.AreEqual(4, Contract.TestCount(4));
            Assert.AreEqual(2036100, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestAdd()
        {
            var item = Contract.TestAdd(4);
            var json = ParseJson(item);

            Assert.IsTrue(json is JArray);
            var jarray = (JArray)json;
            for (int i = 0; i < 4; i++)
            {
                Assert.IsTrue(jarray[i] is JNumber);
                Assert.AreEqual(i, jarray[i]!.AsNumber());
            }
        }

        [TestMethod]
        public void TestRemoveAt()
        {
            var item = Contract.TestRemoveAt(5, 2);
            Assert.AreEqual(3389940, Engine.FeeConsumed.Value);
            var json = ParseJson(item);

            Assert.IsTrue(json is JArray);
            var jarray = (JArray)json;
            for (int i = 0; i < 4; i++)
            {
                Assert.IsTrue(jarray[i] is JNumber);
                Assert.AreEqual(i < 2 ? i : i + 1, jarray[i]!.AsNumber());
            }
        }

        [TestMethod]
        public void TestClear()
        {
            var item = Contract.TestClear(4);
            Assert.AreEqual(3142470, Engine.FeeConsumed.Value);
            var json = ParseJson(item);

            Assert.IsTrue(json is JArray);
            var jarray = (JArray)json;
            Assert.AreEqual(0, jarray.Count);
        }

        [TestMethod]
        public void TestArrayConvert()
        {
            var array = Contract.TestArrayConvert(4)!;
            Assert.AreEqual(2035980, Engine.FeeConsumed.Value);
            Assert.AreEqual(4, array.Count);
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(new BigInteger(i), array[i]);
            }
        }

        static JToken ParseJson(string? json)
        {
            return JToken.Parse(json!)!;
        }
    }
}
