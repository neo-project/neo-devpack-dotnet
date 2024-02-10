using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class NativeArtifactsTests
    {
        [TestMethod]
        public void TestInitialize()
        {
            var engine = new TestEngine();

            Assert.AreEqual(0, engine.Storage.Store.Seek(System.Array.Empty<byte>(), Persistence.SeekDirection.Forward).Count());

            engine.Native.Initialize(true);

            // Ensure that the main address contains the totalSupply

            var addr = Contract.GetBFTAddress(engine.ProtocolSettings.StandbyValidators);

            Assert.AreEqual(100_000_000, engine.Native.NEO.totalSupply());
            Assert.AreEqual(engine.Native.NEO.totalSupply(), engine.Native.NEO.balanceOf(addr));
        }
    }
}
