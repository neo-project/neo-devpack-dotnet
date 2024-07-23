using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class UIntTest : TestBase<Contract_UInt>
    {
        public UIntTest() : base(Contract_UInt.Nef, Contract_UInt.Manifest) { }

        [TestMethod]
        public void TestStringAdd()
        {
            Assert.IsTrue(Contract.IsZeroUInt256(UInt256.Zero));
            Assert.AreEqual(1002108250, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.IsZeroUInt160(UInt160.Zero));
            Assert.AreEqual(1003155820, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.IsZeroUInt256(UInt256.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01")));
            Assert.AreEqual(1004203390, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.IsZeroUInt160(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            Assert.AreEqual(1005250960, Engine.FeeConsumed.Value);
            Assert.AreEqual("Nas9CRigvY94DyqA59HiBZNrgWHRsgrUgt", Contract.ToAddress(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            Assert.AreEqual(1009843630, Engine.FeeConsumed.Value);
        }
    }
}
