using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM;
using System.Collections.Generic;
using System.IO;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class TestEngineTests
    {
        public abstract class MyUndeployedContract : SmartContract
        {
            public abstract int myReturnMethod();
            protected MyUndeployedContract(TestEngine testEngine, UInt160 hash) : base(testEngine, hash) { }
        }

        //[TestMethod]
        public void GenerateNativeArtifacts()
        {
            foreach (var n in Native.NativeContract.Contracts)
            {
                var manifest = n.Manifest;
                var source = manifest.Abi.GetArtifactsSource(manifest.Name);
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
            TestEngine engine = new(true);

            var neo = engine.FromHash<NeoToken>(engine.Native.NEO.Hash,
                mock => mock.Setup(o => o.balanceOf(It.IsAny<UInt160>())).Returns(123),
                false);

            // Test direct call

            Assert.AreEqual(123, neo.balanceOf(engine.BFTAddress));

            // Test vm call

            using (ScriptBuilder script = new())
            {
                script.EmitDynamicCall(neo.Hash, nameof(neo.balanceOf), engine.BFTAddress);

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
            TestEngine engine = new(false);

            var contract = engine.FromHash<ContractManagement>(hash, false);

            Assert.AreEqual(contract.Hash, hash);
        }
    }
}
