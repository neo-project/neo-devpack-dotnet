using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Switch : DebugAndTestBase<Contract_Switch>
    {
        /// <summary>
        /// switch of more than 6 entries require a ComputeStringHash method
        /// </summary>
        [TestMethod]
        public void Test_SwitchLong()
        {
            // Test cases

            for (int x = 0; x <= 20; x++)
            {
                Assert.AreEqual(x + 1, ((VM.Types.Integer)Contract.SwitchLong(x.ToString())!).GetInteger());
            }

            // Test default

            Assert.AreEqual(99, ((VM.Types.Integer)Contract.SwitchLong(21.ToString())!).GetInteger());
        }

        [TestMethod]
        public void Test_SwitchLongLong()
        {
            Assert.AreEqual(2, ((VM.Types.Integer)Contract.SwitchLongLong("a")!).GetInteger());
            AssertGasConsumed(1049490);
            Assert.AreEqual(0, ((VM.Types.Integer)Contract.SwitchLongLong("b")!).GetInteger());
            AssertGasConsumed(1052130);
            Assert.AreEqual(2, ((VM.Types.Integer)Contract.SwitchLongLong("c")!).GetInteger());
            AssertGasConsumed(1050840);
            Assert.AreEqual(-1, ((VM.Types.Integer)Contract.SwitchLongLong("d")!).GetInteger());
            AssertGasConsumed(1053480);
            Assert.AreEqual(1, ((VM.Types.Integer)Contract.SwitchLongLong("e")!).GetInteger());
            AssertGasConsumed(1054830);
            Assert.AreEqual(3, ((VM.Types.Integer)Contract.SwitchLongLong("f")!).GetInteger());
            AssertGasConsumed(1056120);
            Assert.AreEqual(3, ((VM.Types.Integer)Contract.SwitchLongLong("g")!).GetInteger());
            AssertGasConsumed(1057440);
        }

        [TestMethod]
        public void Test_SwitchInteger()
        {
            Assert.AreEqual(2, ((VM.Types.Integer)Contract.SwitchInteger(1)!).GetInteger());
            AssertGasConsumed(1048500);
            Assert.AreEqual(3, ((VM.Types.Integer)Contract.SwitchInteger(2)!).GetInteger());
            AssertGasConsumed(1049610);
            Assert.AreEqual(6, ((VM.Types.Integer)Contract.SwitchInteger(3)!).GetInteger());
            AssertGasConsumed(1050720);
            Assert.AreEqual(0, ((VM.Types.Integer)Contract.SwitchInteger(0)!).GetInteger());
            AssertGasConsumed(1050720);
        }

        [TestMethod]
        public void Test_Switch6()
        {
            // Test cases

            for (int x = 0; x <= 5; x++)
            {
                Assert.AreEqual(x + 1, ((VM.Types.Integer)Contract.Switch6(x.ToString())!).GetInteger());
                Assert.AreEqual(x + 1, ((VM.Types.Integer)Contract.Switch6Inline(x.ToString())!).GetInteger());
            }

            // Test default

            Assert.AreEqual(99, ((VM.Types.Integer)Contract.Switch6(6.ToString())!).GetInteger());
            AssertGasConsumed(1055310);
            Assert.AreEqual(99, ((VM.Types.Integer)Contract.Switch6Inline(6.ToString())!).GetInteger());
            AssertGasConsumed(1055340);
        }
    }
}
