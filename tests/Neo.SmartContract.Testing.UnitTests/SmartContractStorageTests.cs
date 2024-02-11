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

            const byte Prefix_RegisterPrice = 13;

            // Engine an contract creation

            TestEngine engine = new(true);

            var neo = engine.FromHash<NeoToken>(engine.Native.NEO.Hash, false);

            // Check previous data

            Assert.AreEqual(100000000000, neo.getRegisterPrice());

            // Alter data

            neo.Storage.Put(new byte[] { Prefix_RegisterPrice }, BigInteger.MinusOne.ToByteArray());

            // Check altered data

            Assert.AreEqual(BigInteger.MinusOne, neo.getRegisterPrice());
        }
    }
}
