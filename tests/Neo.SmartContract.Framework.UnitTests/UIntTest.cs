using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class UIntTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_UInt.cs");
        }

        [TestMethod]
        public void TestHexToUInt160()
        {
            _engine.Reset();
            var str = "0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff01";
            var result = _engine.ExecuteTestCaseStandard("testUInt160", str).Pop();
            var wantResult = new byte[] { 0x01, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xa4 };
            Assert.AreEqual(new ByteString(wantResult), result.ConvertTo(StackItemType.ByteString));

            _engine.Reset();
            str = "a400ff00ff00ff00ff00ff00ff00ff00ff00ff01";
            result = _engine.ExecuteTestCaseStandard("testUInt160", str).Pop();
            wantResult = new byte[] { 0x01, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xa4 };
            Assert.AreEqual(new ByteString(wantResult), result.ConvertTo(StackItemType.ByteString));

            _engine.Reset();
            str = "a400ff00ff00ff00ff00ff00ff00ff00ff00ff";
            _engine.ExecuteTestCaseStandard("testUInt160", str);
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            str = "k400ff00ff00ff00ff00ff00ff00ff00ff00ff";
            _engine.ExecuteTestCaseStandard("testUInt160", str);
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void TestHexToUInt256()
        {
            _engine.Reset();
            var str = "0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01";
            var result = _engine.ExecuteTestCaseStandard("testUInt256", str).Pop();
            var wantResult = new byte[] { 0x01, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xa4 };
            Assert.AreEqual(new ByteString(wantResult), result.ConvertTo(StackItemType.ByteString));

            _engine.Reset();
            str = "a400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01";
            result = _engine.ExecuteTestCaseStandard("testUInt256", str).Pop();
            wantResult = new byte[] { 0x01, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xa4 };
            Assert.AreEqual(new ByteString(wantResult), result.ConvertTo(StackItemType.ByteString));

            _engine.Reset();
            str = "a400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff";
            _engine.ExecuteTestCaseStandard("testUInt256", str);
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            str = "k400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff";
            _engine.ExecuteTestCaseStandard("testUInt256", str);
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }
    }
}
