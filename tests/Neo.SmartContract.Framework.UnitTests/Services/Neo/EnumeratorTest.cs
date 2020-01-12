using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
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
        public void TestNext()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testNext", new Array(new StackItem[] { 1, 2, 3 }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(6, item.GetBigInteger());
        }

        [TestMethod]
        public void TestConcat()
        {
            // A and B

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testConcat",
                new Array(new StackItem[] { 1, 2, 3 }),
                new Array(new StackItem[] { 4, 5, 6 })
                );
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(21, item.GetBigInteger());

            // Only A

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcat",
               new Array(new StackItem[] { 1, 2, 3 }),
               new Array()
               );
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(6, item.GetBigInteger());

            // Only B

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcat",
               new Array(),
               new Array(new StackItem[] { 4, 5, 6 })
               );
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(15, item.GetBigInteger());

            // Empty

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testConcat",
               new Array(),
               new Array()
               );
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            //Assert.IsInstanceOfType(item, typeof(ByteArray));
            Assert.AreEqual(0, item.GetByteLength());
        }
    }
}
