using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.SmartContract.Testing.Extensions;
using Neo.SmartContract.Testing.Native;
using Neo.VM;
using System.Collections.Generic;
using System.IO;
using NativeContract = Neo.SmartContract.Native.NativeContract;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class TestEngineTests
    {
        public abstract class MyUndeployedContract : SmartContract
        {
            public abstract int myReturnMethod();
            protected MyUndeployedContract(SmartContractInitialize initialize) : base(initialize) { }
        }

        //[TestMethod]
        public void GenerateNativeArtifacts()
        {
            foreach (var n in NativeContract.Contracts)
            {
                var manifest = n.Manifest;
                var source = manifest.GetArtifactsSource(manifest.Name, generateProperties: true);
                var fullPath = Path.GetFullPath($"../../../../../src/Neo.SmartContract.Testing/Native/{manifest.Name}.cs");

                File.WriteAllText(fullPath, source);
            }
        }

        [TestMethod]
        public void TestHashExists()
        {
            TestEngine engine = new(false);

            Assert.ThrowsException<KeyNotFoundException>(() => engine.FromHash<NeoToken>(engine.Native.NEO.Hash, true));

            engine.Native.Initialize(false);

            Assert.IsInstanceOfType<NeoToken>(engine.FromHash<NeoToken>(engine.Native.NEO.Hash, true));
        }

        [TestMethod]
        public void TestCustomMock()
        {
            // Initialize TestEngine and native smart contracts

            TestEngine engine = new(true);

            // Get neo token smart contract and mock balanceOf to always return 123

            var neo = engine.FromHash<NeoToken>(engine.Native.NEO.Hash,
                mock => mock.Setup(o => o.BalanceOf(It.IsAny<UInt160>())).Returns(123),
                false);

            // Test direct call

            Assert.AreEqual(123, neo.BalanceOf(engine.ValidatorsAddress));

            // Test vm call

            using (ScriptBuilder script = new())
            {
                script.EmitDynamicCall(neo.Hash, nameof(neo.BalanceOf), engine.ValidatorsAddress);

                Assert.AreEqual(123, engine.Execute(script.ToArray()).GetInteger());
            }

            // Test mock on undeployed contract

            var undeployed = engine.FromHash<MyUndeployedContract>(UInt160.Zero,
                mock => mock.Setup(o => o.myReturnMethod()).Returns(1234),
                false);

            using (ScriptBuilder script = new())
            {
                script.EmitDynamicCall(UInt160.Zero, nameof(undeployed.myReturnMethod));

                Assert.AreEqual(1234, engine.Execute(script.ToArray()).GetInteger());
            }
        }

        [TestMethod]
        public void TestNativeContracts()
        {
            TestEngine engine = new(false);

            Assert.AreEqual(engine.Native.ContractManagement.Hash, NativeContract.ContractManagement.Hash);
            Assert.AreEqual(engine.Native.StdLib.Hash, NativeContract.StdLib.Hash);
            Assert.AreEqual(engine.Native.CryptoLib.Hash, NativeContract.CryptoLib.Hash);
            Assert.AreEqual(engine.Native.GAS.Hash, NativeContract.GAS.Hash);
            Assert.AreEqual(engine.Native.NEO.Hash, NativeContract.NEO.Hash);
            Assert.AreEqual(engine.Native.Oracle.Hash, NativeContract.Oracle.Hash);
            Assert.AreEqual(engine.Native.Policy.Hash, NativeContract.Policy.Hash);
            Assert.AreEqual(engine.Native.RoleManagement.Hash, NativeContract.RoleManagement.Hash);
        }

        [TestMethod]
        public void FromHashWithoutCheckTest()
        {
            UInt160 hash = UInt160.Parse("0x1230000000000000000000000000000000000000");
            TestEngine engine = new(false);

            var contract = engine.FromHash<ContractManagement>(hash, false);

            Assert.AreEqual(contract.Hash, hash);
        }

        [TestMethod]
        public void FromHashTest()
        {
            // Create the engine initializing the native contracts

            var engine = new TestEngine(true);

            // Instantiate neo contract from native hash, (not necessary if we use engine.Native.NEO)

            var neo = engine.FromHash<NeoToken>(engine.Native.NEO.Hash, true);

            // Ensure that the main address contains the totalSupply

            Assert.AreEqual(100_000_000, neo.TotalSupply);
            Assert.AreEqual(neo.TotalSupply, neo.BalanceOf(engine.ValidatorsAddress));
        }
    }
}
