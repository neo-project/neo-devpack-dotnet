using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class MapTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Map.cs");
        }

        [TestMethod]
        public void TestByteArray()
        {
            _engine.Reset();
            _engine.ExecuteTestCaseStandard("testByteArray");
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void TestByteArray2()
        {
            _engine.Reset();
            StackItem key = System.Text.Encoding.ASCII.GetBytes("a");
            var result = _engine.ExecuteTestCaseStandard("testByteArray2", key);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            // Except: {"a":"dGVzdHN0cmluZzI="}
            Assert.AreEqual("7b2261223a226447567a64484e30636d6c755a7a493d227d", (item as ByteString).Span.ToHexString());
        }

        [TestMethod]
        public void TestByteArray3()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testByteArray3");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            // Except: {"\u0001\u0001":"IiI="}
            Assert.AreEqual("7b225c75303030315c7530303031223a224969493d227d", (item as ByteString).Span.ToHexString());
        }

        [TestMethod]
        public void TestUnicode()
        {
            _engine.Reset();
            StackItem key = System.Text.Encoding.UTF8.GetBytes("中");
            var result = _engine.ExecuteTestCaseStandard("testUnicode", key);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            // Except: {"\u4E2D":"MTI5ODQwdGVzdDEwMDIyOTM5"}
            Assert.AreEqual("7b225c7534453244223a224d5449354f4451776447567a644445774d4449794f544d35227d", (item as ByteString).Span.ToHexString());
        }

        [TestMethod]
        public void TestUnicodeValue()
        {
            _engine.Reset();
            StackItem value = System.Text.Encoding.UTF8.GetBytes("文");
            var result = _engine.ExecuteTestCaseStandard("testUnicodeValue", value);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            // Except: {"ab":"5paH"}
            Assert.AreEqual("7b226162223a2235706148227d", (item as ByteString).Span.ToHexString());
        }

        [TestMethod]
        public void TestUnicodeKeyValue()
        {
            _engine.Reset();
            StackItem key = System.Text.Encoding.UTF8.GetBytes("中");
            StackItem value = System.Text.Encoding.UTF8.GetBytes("文");
            var result = _engine.ExecuteTestCaseStandard("testUnicodeKeyValue", key, value);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            // Except: {"\u4E2D":"5paH"}
            Assert.AreEqual("7b225c7534453244223a2235706148227d", (item as ByteString).Span.ToHexString());
        }

        [TestMethod]
        public void TestInt()
        {
            _engine.Reset();
            StackItem key = 1;
            var result = _engine.ExecuteTestCaseStandard("testInt", key);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            // Except: {"\u0001":"c3RyaW5n"}
            Assert.AreEqual("7b225c7530303031223a22633352796157356e227d", (item as ByteString).Span.ToHexString());
        }

        [TestMethod]
        public void TestBool()
        {
            _engine.Reset();
            StackItem key = true;
            var result = _engine.ExecuteTestCaseStandard("testBool", key);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            // Except: {"\u0001":"dGVzdGJvb2w="}
            Assert.AreEqual("7b225c7530303031223a226447567a64474a766232773d227d", (item as ByteString).Span.ToHexString());
        }

        [TestMethod]
        public void TestDeserialize()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testDeserialize", "a");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Map));
            var map = item as Map;
            Assert.AreEqual(1, map.Count);
            Assert.IsTrue(map.ContainsKey("a"));
            Assert.AreEqual((ByteString)"dGVzdGRlc2VyaWFsaXpl", map["a"]);
        }
    }
}
