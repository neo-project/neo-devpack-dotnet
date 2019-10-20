using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_Syscalls
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Syscalls.cs");
        }

        [TestMethod]
        public void Test_InvocationCounter()
        {
            var result = _engine.ExecuteTestCaseStandard("GetInvocationCounter");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x01, item.GetBigInteger());
        }

        [TestMethod]
        public void Test_GetNotificationsCount()
        {
            var result = _engine.ExecuteTestCaseStandard("GetNotificationsCount", new ByteArray(UInt160.Parse("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF").ToArray()));
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x01, item.GetBigInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("GetNotificationsCount", new ByteArray(new byte[0]));
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x02, item.GetBigInteger());
        }

        [TestMethod]
        public void Test_GetNotifications()
        {
            var result = _engine.ExecuteTestCaseStandard("GetNotifications", new ByteArray(UInt160.Parse("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF").ToArray()));
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x02, item.GetBigInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("GetNotifications", new ByteArray(new byte[0]));
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0x03, item.GetBigInteger());
        }
    }
}
