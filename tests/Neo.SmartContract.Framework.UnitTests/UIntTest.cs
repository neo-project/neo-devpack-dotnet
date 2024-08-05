using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class UIntTest : TestBase2<Contract_UInt>
    {
        [TestMethod]
        public void TestStringAdd()
        {
            Assert.IsTrue(Contract.IsZeroUInt256(UInt256.Zero));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.IsZeroUInt160(UInt160.Zero));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.IsZeroUInt256(UInt256.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01")));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.IsZeroUInt160(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            Assert.AreEqual(1047510, Engine.FeeConsumed.Value);
            Assert.AreEqual("Nas9CRigvY94DyqA59HiBZNrgWHRsgrUgt", Contract.ToAddress(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            Assert.AreEqual(4592490, Engine.FeeConsumed.Value);
        }
    }
}
