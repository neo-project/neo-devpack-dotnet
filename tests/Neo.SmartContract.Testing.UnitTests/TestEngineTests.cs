// Copyright (C) 2015-2026 The Neo Project.
//
// TestEngineTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.Extensions;
using Neo.Extensions.VM;
using Neo.SmartContract.Testing.Extensions;
using Neo.SmartContract.Testing.Native;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

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
            foreach (var n in Neo.SmartContract.Native.NativeContract.Contracts)
            {
                var manifest = n.GetContractState(ProtocolSettings.Default, uint.MaxValue).Manifest;
                var source = manifest.GetArtifactsSource(manifest.Name, generateProperties: true);
                var fullPath = Path.GetFullPath($"../../../../../src/Neo.SmartContract.Testing/Native/{manifest.Name}.cs");

                File.WriteAllText(fullPath, source);
            }
        }

        [TestMethod]
        public void TestSkip()
        {
            TestEngine engine = new(true);

            Assert.AreEqual(0L, engine.Native.Ledger.CurrentIndex);
            engine.PersistingBlock.Skip(10, TimeSpan.Zero);
            engine.PersistingBlock.Persist();
            Assert.AreEqual(11L, engine.Native.Ledger.CurrentIndex);
        }

        [TestMethod]
        public void TestNextBlock()
        {
            TestEngine engine = new(true);

            Assert.AreEqual(0L, engine.Native.Ledger.CurrentIndex);
            engine.PersistingBlock.Persist();
            Assert.AreEqual(1L, engine.Native.Ledger.CurrentIndex);
        }

        [TestMethod]
        public void TestOnGetEntryScriptHash()
        {
            TestEngine engine = new(true);

            var builder = new ScriptBuilder();
            builder.EmitSysCall(ApplicationEngine.System_Runtime_GetEntryScriptHash);
            var script = builder.ToArray();

            Assert.AreEqual("0xfa99b1aeedab84a47856358515e7f982341aa767", engine.Execute(script).ConvertTo(typeof(UInt160))!.ToString());

            engine.OnGetEntryScriptHash = (current, expected) => UInt160.Parse("0x0000000000000000000000000000000000000001");
            Assert.AreEqual("0x0000000000000000000000000000000000000001", engine.Execute(script).ConvertTo(typeof(UInt160))!.ToString());
        }

        [TestMethod]
        public void TestOnGetCallingScriptHash()
        {
            TestEngine engine = new(true);

            var builder = new ScriptBuilder();
            builder.EmitSysCall(ApplicationEngine.System_Runtime_GetCallingScriptHash);
            var script = builder.ToArray();

            Assert.AreEqual(StackItem.Null, engine.Execute(script));

            engine.OnGetCallingScriptHash = (current, expected) => UInt160.Parse("0x0000000000000000000000000000000000000001");
            Assert.AreEqual("0x0000000000000000000000000000000000000001", engine.Execute(script).ConvertTo(typeof(UInt160))!.ToString());
        }

        [TestMethod]
        public void TestHashExists()
        {
            TestEngine engine = new(false);

            Assert.ThrowsExactly<KeyNotFoundException>(() => _ = engine.FromHash<Testing.Native.Ledger>(engine.Native.Ledger.Hash, true));

            engine.Native.Initialize(false);

            Assert.IsInstanceOfType<Testing.Native.Ledger>(engine.FromHash<Testing.Native.Ledger>(engine.Native.Ledger.Hash, true));
        }

        [TestMethod]
        public void TestCustomMock()
        {
            // Initialize TestEngine and native smart contracts

            TestEngine engine = new(true);

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

            // Instantiate ledger contract from native hash

            var ledger = engine.FromHash<Testing.Native.Ledger>(engine.Native.Ledger.Hash, true);

            // Ensure that the ledger is accessible

            Assert.IsNotNull(ledger);
            Assert.AreEqual(engine.Native.Ledger.Hash, ledger.Hash);
        }
    }
}
