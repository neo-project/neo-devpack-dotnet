using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class SmartContractStorageTests
    {
        [TestMethod]
        public void TestAlterStorage()
        {
            // Defines the prefix used to store the registration price in neo

            byte[] registerPricePrefix = new byte[] { 13 };

            // Engine an contract creation

            TestEngine engine = new(true);

            // Check previous data

            Assert.AreEqual(100000000000, engine.Native.NEO.RegisterPrice);

            // Alter data

            engine.Native.NEO.Storage.Put(registerPricePrefix, BigInteger.MinusOne);

            // Check altered data

            Assert.AreEqual(BigInteger.MinusOne, engine.Native.NEO.RegisterPrice);
        }
    }
}
