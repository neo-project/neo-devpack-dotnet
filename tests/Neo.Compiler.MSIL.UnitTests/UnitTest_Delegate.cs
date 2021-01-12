using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Extensions;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO;
using Neo.Ledger;
using Neo.Persistence;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_Delegate
    {
        private TestEngine testengine;
        private readonly TestSnapshot snapshot = new TestSnapshot();

        [TestInitialize]
        public void Init()
        {
            snapshot.DeployNativeContracts();

            testengine = new TestEngine(snapshot: snapshot);
            testengine.AddEntryScript("./TestClasses/Contract_Delegate.cs");
        }

        [TestMethod]
        public void TestFunc()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("sumFunc", 2, 3).Pop();
            Assert.AreEqual(5, result.GetInteger());
        }

        [TestMethod]
        public void TestDelegateCall()
        {
            var token = UInt160.Parse("0x4961bf0ab79370b23dc45cde29f568d0e0fa6e93"); // NEO token

            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testDynamicCall", token.ToArray(), "symbol");
            Assert.AreEqual(VMState.HALT, testengine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("NEO", item.GetString());
        }
    }
}
