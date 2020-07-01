using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class IteratorTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Iterator.cs");
        }

        [TestMethod]
        public void TestNextArray()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testNextArray", new Array(new StackItem[] { 1, 2, 3 }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(6, item.GetInteger());
        }

        [TestMethod]
        public void TestConcatArray()
        {
            var a = new Array(new StackItem[] { 1, 2, 3 });
            var b = new Array(new StackItem[] { 4, 5, 6 });

            // A and B

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testConcatArray", a, b);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(21, item.GetInteger());

            // Only A

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatArray", a, new Array());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(6, item.GetInteger());

            // Only B

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatArray", new Array(), b);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(15, item.GetInteger());

            // Empty

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatArray", new Array(), new Array());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0, item.GetSpan().Length);
        }

        [TestMethod]
        public void TestConcatMap()
        {
            var a = new Map
            {
                [new Integer(1)] = new Integer(2),
                [new Integer(3)] = new Integer(4)
            };

            var b = new Map
            {
                [new Integer(5)] = new Integer(6),
                [new Integer(7)] = new Integer(8)
            };

            // A and B

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testConcatMap", a, b);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(36, item.GetInteger());

            // Only A

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatMap", a, new Array());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(10, item.GetInteger());

            // Only B

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatMap", new Array(), b);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(26, item.GetInteger());

            // Empty

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatMap", new Array(), new Array());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0, item.GetSpan().Length);
        }

        [TestMethod]
        public void TestConcatKeys()
        {
            var a = new Map
            {
                [new Integer(1)] = new Integer(2),
                [new Integer(3)] = new Integer(4)
            };

            var b = new Map
            {
                [new Integer(5)] = new Integer(6),
                [new Integer(7)] = new Integer(8)
            };

            // A and B

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testConcatKeys", a, b);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(16, item.GetInteger());

            // Only A

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatKeys", a, new Array());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(4, item.GetInteger());

            // Only B

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatKeys", new Array(), b);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(12, item.GetInteger());

            // Empty

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatKeys", new Array(), new Array());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0, item.GetSpan().Length);
        }

        [TestMethod]
        public void TestConcatValues()
        {
            var a = new Map
            {
                [new Integer(1)] = new Integer(2),
                [new Integer(3)] = new Integer(4)
            };

            var b = new Map
            {
                [new Integer(5)] = new Integer(6),
                [new Integer(7)] = new Integer(8)
            };

            // A and B

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testConcatValues", a, b);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(20, item.GetInteger());

            // Only A

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatValues", a, new Array());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(6, item.GetInteger());

            // Only B

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatValues", new Array(), b);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(14, item.GetInteger());

            // Empty

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatValues", new Array(), new Array());
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0, item.GetSpan().Length);
        }
    }
}
