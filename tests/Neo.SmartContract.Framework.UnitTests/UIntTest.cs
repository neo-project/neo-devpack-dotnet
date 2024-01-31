using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class UIntTest
    {
        private TestEngine.TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine.TestEngine();
            _engine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_UInt.cs");
        }

        [TestMethod]
        public void TestStringAdd()
        {
            var result = _engine.ExecuteTestCaseStandard("isZeroUInt256", UInt256.Zero.ToArray());
            Assert.IsTrue(result.Pop().GetBoolean());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("isZeroUInt160", UInt160.Zero.ToArray());
            Assert.IsTrue(result.Pop().GetBoolean());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("isZeroUInt256", UInt256.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01").ToArray());
            Assert.IsFalse(result.Pop().GetBoolean());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("isZeroUInt160", UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4").ToArray());
            Assert.IsFalse(result.Pop().GetBoolean());
        }
    }
}
