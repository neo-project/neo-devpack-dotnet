using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class UIntTest : DebugAndTestBase<Contract_UInt>
    {
        [TestMethod]
        public void TestStringAdd()
        {
            Assert.IsTrue(Contract.IsZeroUInt256(UInt256.Zero));
            AssertGasConsumed(1047480);
            Assert.IsFalse(Contract.IsValidAndNotZeroUInt256(UInt256.Zero));
            AssertGasConsumed(1065390);
            Assert.IsTrue(Contract.IsZeroUInt160(UInt160.Zero));
            AssertGasConsumed(1047480);
            Assert.IsFalse(Contract.IsValidAndNotZeroUInt160(UInt160.Zero));
            AssertGasConsumed(1065390);
            Assert.IsFalse(Contract.IsZeroUInt256(UInt256.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01")));
            AssertGasConsumed(1047480);
            Assert.IsTrue(Contract.IsValidAndNotZeroUInt256(UInt256.Parse("0xa400ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff00ff01")));
            AssertGasConsumed(1065390);
            Assert.IsFalse(Contract.IsZeroUInt160(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            AssertGasConsumed(1047480);
            Assert.IsTrue(Contract.IsValidAndNotZeroUInt160(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            AssertGasConsumed(1065390);
            Assert.AreEqual("Nas9CRigvY94DyqA59HiBZNrgWHRsgrUgt", Contract.ToAddress(UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            AssertGasConsumed(4592370);
        }
    }
}
