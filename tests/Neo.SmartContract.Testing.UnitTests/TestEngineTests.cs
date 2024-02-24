using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM;
using Neo.VM.Types;
using System.Collections.Generic;
using System.IO;
using Neo.Persistence;
using Neo.SmartContract.Native;
using ContractManagement = Neo.SmartContract.Testing.Native.ContractManagement;
using NeoToken = Neo.SmartContract.Testing.Native.NeoToken;

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
        public void GenerateNativeArtifacts(DataCache snapshot)
        {
            foreach (var n in NativeContract.Contracts)
            {
                var manifest = NativeContract.ContractManagement.GetContract(snapshot, n.Hash).Manifest;
                var source = manifest.GetArtifactsSource(manifest.Name, generateProperties: true);
                var fullPath = Path.GetFullPath($"../../../../../src/Neo.SmartContract.Testing/Native/{manifest.Name}.cs");

                File.WriteAllText(fullPath, source);
            }
        }

        [TestMethod]
        public void TestOnGetEntryScriptHash()
        {
            TestEngine engine = new(true);

            var builder = new ScriptBuilder();
            builder.EmitSysCall(ApplicationEngine.System_Runtime_GetEntryScriptHash);
            var script = builder.ToArray();

            Assert.AreEqual("0xfa99b1aeedab84a47856358515e7f982341aa767", engine.Execute(script).ConvertTo(typeof(UInt160)).ToString());

            engine.OnGetEntryScriptHash = current => UInt160.Parse("0x0000000000000000000000000000000000000001");
            Assert.AreEqual("0x0000000000000000000000000000000000000001", engine.Execute(script).ConvertTo(typeof(UInt160)).ToString());
        }

        [TestMethod]
        public void TestOnGetCallingScriptHash()
        {
            TestEngine engine = new(true);

            var builder = new ScriptBuilder();
            builder.EmitSysCall(ApplicationEngine.System_Runtime_GetCallingScriptHash);
            var script = builder.ToArray();

            Assert.AreEqual(StackItem.Null, engine.Execute(script));

            engine.OnGetCallingScriptHash = current => UInt160.Parse("0x0000000000000000000000000000000000000001");
            Assert.AreEqual("0x0000000000000000000000000000000000000001", engine.Execute(script).ConvertTo(typeof(UInt160)).ToString());
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
                script.EmitDynamicCall(neo.Hash, "balanceOf", engine.ValidatorsAddress);

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

            Assert.AreEqual(engine.Native.ContractManagement.Hash, Neo.SmartContract.Native.NativeContract.ContractManagement.Hash);
            Assert.AreEqual(engine.Native.StdLib.Hash, Neo.SmartContract.Native.NativeContract.StdLib.Hash);
            Assert.AreEqual(engine.Native.CryptoLib.Hash, Neo.SmartContract.Native.NativeContract.CryptoLib.Hash);
            Assert.AreEqual(engine.Native.GAS.Hash, Neo.SmartContract.Native.NativeContract.GAS.Hash);
            Assert.AreEqual(engine.Native.NEO.Hash, Neo.SmartContract.Native.NativeContract.NEO.Hash);
            Assert.AreEqual(engine.Native.Oracle.Hash, Neo.SmartContract.Native.NativeContract.Oracle.Hash);
            Assert.AreEqual(engine.Native.Policy.Hash, Neo.SmartContract.Native.NativeContract.Policy.Hash);
            Assert.AreEqual(engine.Native.RoleManagement.Hash, Neo.SmartContract.Native.NativeContract.RoleManagement.Hash);
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
