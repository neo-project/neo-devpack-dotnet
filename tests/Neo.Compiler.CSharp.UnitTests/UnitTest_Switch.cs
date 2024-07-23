using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Switch : TestBase<Contract_Switch>
    {
        public UnitTest_Switch() : base(Contract_Switch.Nef, Contract_Switch.Manifest) { }

        /// <summary>
        /// switch of more than 6 entries require a ComputeStringHash method
        /// </summary>
        [TestMethod]
        public void Test_SwitchLong()
        {
            // Test cases

            for (int x = 0; x <= 20; x++)
            {
                Assert.AreEqual(x + 1, ((VM.Types.Integer)Contract.SwitchLong(x.ToString())).GetInteger());
            }

            // Test default

            Assert.AreEqual(99, ((VM.Types.Integer)Contract.SwitchLong(21.ToString())).GetInteger());
        }

        [TestMethod]
        public void Test_SwitchLongLong()
        {
            Assert.AreEqual(2, ((VM.Types.Integer)Contract.SwitchLongLong("a")).GetInteger());
            Assert.AreEqual(1002125350, Engine.FeeConsumed.Value);
            Assert.AreEqual(0, ((VM.Types.Integer)Contract.SwitchLongLong("b")).GetInteger());
            Assert.AreEqual(1003177540, Engine.FeeConsumed.Value);
            Assert.AreEqual(2, ((VM.Types.Integer)Contract.SwitchLongLong("c")).GetInteger());
            Assert.AreEqual(1004228560, Engine.FeeConsumed.Value);
            Assert.AreEqual(-1, ((VM.Types.Integer)Contract.SwitchLongLong("d")).GetInteger());
            Assert.AreEqual(1005282220, Engine.FeeConsumed.Value);
            Assert.AreEqual(1, ((VM.Types.Integer)Contract.SwitchLongLong("e")).GetInteger());
            Assert.AreEqual(1006337230, Engine.FeeConsumed.Value);
            Assert.AreEqual(3, ((VM.Types.Integer)Contract.SwitchLongLong("f")).GetInteger());
            Assert.AreEqual(1007393530, Engine.FeeConsumed.Value);
            Assert.AreEqual(3, ((VM.Types.Integer)Contract.SwitchLongLong("g")).GetInteger());
            Assert.AreEqual(1008451150, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_SwitchInteger()
        {
            Assert.AreEqual(2, ((VM.Types.Integer)Contract.SwitchInteger(1)).GetInteger());
            Assert.AreEqual(1002124480, Engine.FeeConsumed.Value);
            Assert.AreEqual(3, ((VM.Types.Integer)Contract.SwitchInteger(2)).GetInteger());
            Assert.AreEqual(1003174270, Engine.FeeConsumed.Value);
            Assert.AreEqual(6, ((VM.Types.Integer)Contract.SwitchInteger(3)).GetInteger());
            Assert.AreEqual(1004225170, Engine.FeeConsumed.Value);
            Assert.AreEqual(0, ((VM.Types.Integer)Contract.SwitchInteger(0)).GetInteger());
            Assert.AreEqual(1005276130, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Switch6()
        {
            // Test cases

            for (int x = 0; x <= 5; x++)
            {
                Assert.AreEqual(x + 1, ((VM.Types.Integer)Contract.Switch6(x.ToString())).GetInteger());
                Assert.AreEqual(x + 1, ((VM.Types.Integer)Contract.Switch6Inline(x.ToString())).GetInteger());
            }

            // Test default

            Assert.AreEqual(99, ((VM.Types.Integer)Contract.Switch6(6.ToString())).GetInteger());
            Assert.AreEqual(1014755650, Engine.FeeConsumed.Value);
            Assert.AreEqual(99, ((VM.Types.Integer)Contract.Switch6Inline(6.ToString())).GetInteger());
            Assert.AreEqual(1015811110, Engine.FeeConsumed.Value);
        }
    }
}
