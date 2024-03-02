using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM.Types;
using System.Reflection;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class MapTest : TestBase<Contract_Map>
    {
        public MapTest() : base(Contract_Map.Nef, Contract_Map.Manifest) { }

        [TestMethod]
        public void TestCount()
        {
            Assert.AreEqual(4, Contract.TestCount(4));
        }

        [TestMethod]
        public void TestByteArray()
        {
            var key = System.Text.Encoding.ASCII.GetBytes("a");
            // Except: {"a":"teststring2"}
            Assert.AreEqual("7b2261223a2274657374737472696e6732227d", (Contract.TestByteArray(key) as ByteString).GetSpan().ToHexString());
        }

        [TestMethod]
        public void TestClear()
        {
            var key = System.Text.Encoding.ASCII.GetBytes("a");
            // Except: {}
            Assert.AreEqual("7b7d", (Contract.TestClear(key) as ByteString).GetSpan().ToHexString());
        }

        [TestMethod]
        public void TestByteArray2()
        {
            // Except: {"\u0001\u0001":"\u0022\u0022"}
            Assert.AreEqual("{\"\\u0001\\u0001\":\"\\u0022\\u0022\"}", Contract.TestByteArray2());
        }

        [TestMethod]
        public void TestUnicode()
        {
            // Except: {"\u4E2D":"129840test10022939"}
            Assert.AreEqual("{\"\\u4E2D\":\"129840test10022939\"}", Contract.TestUnicode("中"));
        }

        [TestMethod]
        public void TestUnicodeValue()
        {
            // Except: {"ab":"\u6587"}
            Assert.AreEqual("{\"ab\":\"\\u6587\"}", Contract.TestUnicodeValue("文"));
        }

        [TestMethod]
        public void TestUnicodeKeyValue()
        {
            // Except: {"\u4E2D":"\u6587"}
            Assert.AreEqual("{\"\\u4E2D\":\"\\u6587\"}", Contract.TestUnicodeKeyValue("中", "文"));
        }

        [TestMethod]
        public void TestInt()
        {
            // Int cannot be used as the key for serializing Map
            Assert.ThrowsException<TargetInvocationException>(() => Contract.TestInt(1));
        }

        [TestMethod]
        public void TestBool()
        {
            // Bool cannot be used as the key for serializing Map
            Assert.ThrowsException<TargetInvocationException>(() => Contract.TestBool(true));
        }

        [TestMethod]
        public void TestDeserialize()
        {
            var item = Contract.TestDeserialize("a");

            Assert.IsInstanceOfType(item, typeof(Map));
            var map = item as Map;
            Assert.AreEqual(1, map!.Count);
            Assert.IsTrue(map.ContainsKey("a"));
            Assert.AreEqual((ByteString)"testdeserialize", map["a"]);
        }

        [TestMethod]
        public void TestUInt160KeyDeserialize()
        {
            var item = Contract.Testuint160Key();

            Assert.IsInstanceOfType(item, typeof(Map));
            var map = item as Map;
            Assert.AreEqual(1, map!.Count);
            Assert.IsTrue(map.ContainsKey(new byte[20]));
            Assert.AreEqual(1, map[new byte[20]]);
        }
    }
}
