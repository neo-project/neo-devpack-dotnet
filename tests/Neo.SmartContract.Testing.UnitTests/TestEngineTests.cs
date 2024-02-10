using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class TestEngineTests
    {
        //[TestMethod]
        public void GenerateNativeArtifacts()
        {
            foreach (var n in Native.NativeContract.Contracts)
            {
                var manifest = n.Manifest;
                var source = Artifacts.CreateSourceFromManifest(manifest.Name, manifest.Abi).Replace("\r\n", "\n").Trim();
                var fullPath = Path.GetFullPath($"../../../../../src/Neo.SmartContract.Testing/Native/{manifest.Name}.cs");

                File.WriteAllText(fullPath, source);
            }
        }

        [TestMethod]
        public void TestNativeContracts()
        {
            TestEngine engine = new();

            Assert.AreEqual(engine.Native.ContractManagement.Hash, Native.NativeContract.ContractManagement.Hash);
            Assert.AreEqual(engine.Native.StdLib.Hash, Native.NativeContract.StdLib.Hash);
            Assert.AreEqual(engine.Native.CryptoLib.Hash, Native.NativeContract.CryptoLib.Hash);
            Assert.AreEqual(engine.Native.GAS.Hash, Native.NativeContract.GAS.Hash);
            Assert.AreEqual(engine.Native.NEO.Hash, Native.NativeContract.NEO.Hash);
            Assert.AreEqual(engine.Native.Oracle.Hash, Native.NativeContract.Oracle.Hash);
            Assert.AreEqual(engine.Native.Policy.Hash, Native.NativeContract.Policy.Hash);
            Assert.AreEqual(engine.Native.RoleManagement.Hash, Native.NativeContract.RoleManagement.Hash);
        }

        [TestMethod]
        public void FromHashTest()
        {
            UInt160 hash = UInt160.Parse("0x1230000000000000000000000000000000000000");
            TestEngine engine = new();

            var contract = engine.FromHash<Contract1>(hash);

            Assert.AreEqual(contract.Hash, hash);
        }

        [TestMethod]
        public void TestLog()
        {
            UInt160 hash = UInt160.Parse("0x1230000000000000000000000000000000000000");
            TestEngine engine = new();

            var contractLog = false;
            var contract = engine.FromHash<Contract1>(hash);
            contract.OnLog += (msg) =>
            {
                contractLog = true;
            };

            var neoLog = false;
            engine.Native.NEO.OnLog += (msg) =>
                {
                    neoLog = true;
                };

            engine.Log(contract.Hash, "test");

            Assert.IsTrue(contractLog);
            Assert.IsFalse(neoLog);
        }
    }
}
