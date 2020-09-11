using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.SmartContract.Enumerators;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class EnumeratorTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Enumerator.cs");
        }

        [TestMethod]
        public void TestNextIntArray()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testNextIntArray", new Array(new StackItem[] { 1, 2, 3 }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(6, item.GetInteger());
        }

        [TestMethod]
        public void TestNextByteArray()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testNextByteArray", new byte[] { 1, 2, 3 });
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(6, item.GetInteger());
        }

        [TestMethod]
        public void TestConcatIntArray()
        {
            // A and B

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testConcatIntArray",
                new Array(new StackItem[] { 1, 2, 3 }),
                new Array(new StackItem[] { 4, 5, 6 })
                );
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(21, item.GetInteger());

            // Only A

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatIntArray",
               new Array(new StackItem[] { 1, 2, 3 }),
               new Array()
               );
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(6, item.GetInteger());

            // Only B

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatIntArray",
               new Array(),
               new Array(new StackItem[] { 4, 5, 6 })
               );
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(15, item.GetInteger());

            // Empty

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatIntArray",
               new Array(),
               new Array()
               );
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0, item.GetSpan().Length);
        }

        [TestMethod]
        public void TestConcatByteArray()
        {
            // A and B

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testConcatByteArray",
                new byte[] { 1, 2, 3 },
                new byte[] { 4, 5, 6 }
                );
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(21, item.GetInteger());

            // Only A

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatByteArray",
               new byte[] { 1, 2, 3 },
               new byte[] { }
               );
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(6, item.GetInteger());

            // Only B

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatByteArray",
               new byte[] { },
               new byte[] { 4, 5, 6 }
               );
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(15, item.GetInteger());

            // Empty

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcatByteArray",
               new byte[] { },
               new byte[] { }
               );
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0, item.GetSpan().Length);
        }

        [TestMethod]
        public void TestIntEnumerator()
        {
            _engine.Reset();
            var enumerator = ((InteropInterface)_engine.ExecuteTestCaseStandard("testIntEnumerator").Pop()).GetInterface<IEnumerator>();

            enumerator.Next();
            var v1 = enumerator.Value();
            Assert.AreEqual(4, v1.GetInteger());

            enumerator.Next();
            var v2 = enumerator.Value();
            Assert.AreEqual(6, v2.GetInteger());

            enumerator.Next();
            var v3 = enumerator.Value();
            Assert.AreEqual(8, v3.GetInteger());
        }
    }
}
