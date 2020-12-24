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
