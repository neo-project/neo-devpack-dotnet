using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM.Types;
using System.Reflection;
using Neo.Extensions;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class MapTest : DebugAndTestBase<Contract_Map>
    {
        [TestMethod]
        public void TestCount()
        {
            Assert.AreEqual(4, Contract.TestCount(4));
            Assert.AreEqual(2036820, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestByteArray()
        {
            var key = System.Text.Encoding.ASCII.GetBytes("a");
            // Except: {"a":"teststring2"}
            Assert.AreEqual("7b2261223a2274657374737472696e6732227d", (Contract.TestByteArray(key) as ByteString)!.GetSpan().ToHexString());
            Assert.AreEqual(2645550, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestClear()
        {
            var key = System.Text.Encoding.ASCII.GetBytes("a");
            // Except: {}
            Assert.AreEqual("7b7d", (Contract.TestClear(key) as ByteString)!.GetSpan().ToHexString());
            Assert.AreEqual(2646090, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestByteArray2()
        {
            // Except: {"\u0001\u0001":"\u0022\u0022"}
            Assert.AreEqual("{\"\\u0001\\u0001\":\"\\u0022\\u0022\"}", Contract.TestByteArray2());
            Assert.AreEqual(3936330, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestUnicode()
        {
            // Except: {"\u4E2D":"129840test10022939"}
            Assert.AreEqual("{\"\\u4E2D\":\"129840test10022939\"}", Contract.TestUnicode("中"));
            Assert.AreEqual(2399790, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestUnicodeValue()
        {
            // Except: {"ab":"\u6587"}
            Assert.AreEqual("{\"ab\":\"\\u6587\"}", Contract.TestUnicodeValue("文"));
            Assert.AreEqual(2399790, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestUnicodeKeyValue()
        {
            // Except: {"\u4E2D":"\u6587"}
            Assert.AreEqual("{\"\\u4E2D\":\"\\u6587\"}", Contract.TestUnicodeKeyValue("中", "文"));
            Assert.AreEqual(2399850, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestInt()
        {
            // Int cannot be used as the key for serializing Map
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestInt(1));
            Assert.AreEqual(2399580, Engine.FeeConsumed.Value);
            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);
        }

        [TestMethod]
        public void TestBool()
        {
            // Bool cannot be used as the key for serializing Map
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestBool(true));
            Assert.AreEqual(2399580, Engine.FeeConsumed.Value);
            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);
        }

        [TestMethod]
        public void TestDeserialize()
        {
            var item = Contract.TestDeserialize("a");
            Assert.AreEqual(3874500, Engine.FeeConsumed.Value);

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
            Assert.AreEqual(3813360, Engine.FeeConsumed.Value);

            Assert.IsInstanceOfType(item, typeof(Map));
            var map = item as Map;
            Assert.AreEqual(1, map!.Count);
            Assert.IsTrue(map.ContainsKey(new byte[20]));
            Assert.AreEqual(1, map[new byte[20]]);
        }
    }
}
