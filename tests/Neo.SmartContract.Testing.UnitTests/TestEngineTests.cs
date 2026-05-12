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
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing.Extensions;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.Native;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
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

        public abstract class TestingStandardsContract : SmartContract, IContractInfo
        {
            public static NefFile Nef => throw new NotSupportedException();
            public static Neo.SmartContract.Manifest.ContractManifest Manifest => throw new NotSupportedException();

            protected TestingStandardsContract(SmartContractInitialize initialize) : base(initialize) { }
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
        public void CreateRuntimeLogWatcherTracksLogs()
        {
            TestEngine engine = new(true);

            var watcher = engine.CreateRuntimeLogWatcher();

            var firstSender = ExecuteRuntimeLog(engine, "first");
            var secondSender = ExecuteRuntimeLog(engine, "second");

            Assert.AreEqual(2, watcher.Count);
            Assert.AreEqual(2, watcher.Logs.Count);
            CollectionAssert.AreEqual(new[] { "first", "second" }, watcher.LogMessages.ToArray());
            Assert.AreEqual(firstSender, watcher.Logs[0].Sender);
            Assert.AreEqual("first", watcher.Logs[0].Message);
            Assert.AreEqual(secondSender, watcher.Logs[1].Sender);
            Assert.AreEqual("second", watcher.Logs[1].Message);

            watcher.Reset();

            Assert.AreEqual(0, watcher.Count);
            Assert.AreEqual(0, watcher.Logs.Count);
            Assert.AreEqual(0, watcher.LogMessages.Count);

            watcher.Dispose();
            ExecuteRuntimeLog(engine, "ignored");

            Assert.AreEqual(0, watcher.Count);
            Assert.AreEqual(0, watcher.Logs.Count);
        }

        [TestMethod]
        public void BuiltInTestSignersUseStableAccounts()
        {
            Assert.AreEqual(UInt160.Parse("0x0102030405060708090a0b0c0d0e0f1011121314"), TestEngine.AliceAccount);
            Assert.AreEqual(UInt160.Parse("0x1112131415161718191a1b1c1d1e1f2021222324"), TestEngine.BobAccount);
            Assert.AreEqual(UInt160.Parse("0x2122232425262728292a2b2c2d2e2f3031323334"), TestEngine.CharlieAccount);

            Assert.AreEqual(TestEngine.AliceAccount, TestEngine.Alice.Account);
            Assert.AreEqual(TestEngine.BobAccount, TestEngine.Bob.Account);
            Assert.AreEqual(TestEngine.CharlieAccount, TestEngine.Charlie.Account);
            Assert.AreEqual(WitnessScope.CalledByEntry, TestEngine.Alice.Scopes);
            Assert.AreEqual(WitnessScope.CalledByEntry, TestEngine.Bob.Scopes);
            Assert.AreEqual(WitnessScope.CalledByEntry, TestEngine.Charlie.Scopes);
        }

        [TestMethod]
        public void BuiltInTestSignersReturnFreshSignerInstances()
        {
            var alice = TestEngine.Alice;
            alice.Account = UInt160.Zero;
            alice.Scopes = WitnessScope.Global;

            Assert.AreEqual(TestEngine.AliceAccount, TestEngine.Alice.Account);
            Assert.AreEqual(WitnessScope.CalledByEntry, TestEngine.Alice.Scopes);
        }

        [TestMethod]
        public void CreateSignerUsesRequestedAccountAndScope()
        {
            var signer = TestEngine.CreateSigner(TestEngine.BobAccount, WitnessScope.Global);

            Assert.AreEqual(TestEngine.BobAccount, signer.Account);
            Assert.AreEqual(WitnessScope.Global, signer.Scopes);
        }

        [TestMethod]
        public void GetNewSignerCreatesAdHocAccountWithRequestedScope()
        {
            var signer = TestEngine.GetNewSigner(WitnessScope.Global);

            Assert.AreNotEqual(UInt160.Zero, signer.Account);
            Assert.AreEqual(WitnessScope.Global, signer.Scopes);
        }

        [TestMethod]
        public void TestingStandardsUseBuiltInTestSigners()
        {
            Assert.AreEqual(TestEngine.AliceAccount, TestBase<TestingStandardsContract>.Alice.Account);
            Assert.AreEqual(TestEngine.BobAccount, TestBase<TestingStandardsContract>.Bob.Account);
            Assert.AreEqual(TestEngine.CharlieAccount, TestBase<TestingStandardsContract>.Charlie.Account);
        }

        private static UInt160 ExecuteRuntimeLog(TestEngine engine, string message)
        {
            var builder = new ScriptBuilder();
            builder.EmitPush(message);
            builder.EmitSysCall(ApplicationEngine.System_Runtime_Log);
            var script = builder.ToArray();
            engine.Execute(script);

            return script.ToScriptHash();
        }

        [TestMethod]
        public void TestHashExists()
        {
            TestEngine engine = new(false);

            Assert.ThrowsExactly<KeyNotFoundException>(() => _ = engine.FromHash<NEO>(engine.Native.NEO.Hash, true));

            engine.Native.Initialize(false);

            Assert.IsInstanceOfType<NEO>(engine.FromHash<NEO>(engine.Native.NEO.Hash, true));
        }

        [TestMethod]
        public void TestCustomMock()
        {
            // Initialize TestEngine and native smart contracts

            TestEngine engine = new(true);

            // Get neo token smart contract and mock balanceOf to always return 123

            var neo = engine.FromHash<NEO>(engine.Native.NEO.Hash,
                mock => mock.Setup(o => o.BalanceOf(It.IsAny<UInt160>())).Returns(new BigInteger(123)),
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

            var neo = engine.FromHash<NEO>(engine.Native.NEO.Hash, true);

            // Ensure that the main address contains the totalSupply

            Assert.AreEqual(100_000_000, neo.TotalSupply);
            Assert.AreEqual(neo.TotalSupply, neo.BalanceOf(engine.ValidatorsAddress));
        }

        [TestMethod]
        public void TestJumpTableSelectionUsesExpectedHandlersByHardfork()
        {
            var preEchidna = CaptureJumpTable(CreateJumpTableProtocolSettings(includeEchidna: false, includeGorgon: false));
            Assert.AreEqual("VulnerableSubStr", preEchidna[OpCode.SUBSTR].Method.Name);
            Assert.AreEqual("HasKey_Before543", preEchidna[OpCode.HASKEY].Method.Name);

            var notGorgon = CaptureJumpTable(CreateJumpTableProtocolSettings(includeEchidna: true, includeGorgon: false));
            Assert.AreEqual("SubStr", notGorgon[OpCode.SUBSTR].Method.Name);
            Assert.AreEqual("HasKey_Before543", notGorgon[OpCode.HASKEY].Method.Name);

            var gorgon = CaptureJumpTable(CreateJumpTableProtocolSettings(includeEchidna: true, includeGorgon: true));
            Assert.AreEqual("SubStr", gorgon[OpCode.SUBSTR].Method.Name);
            Assert.AreEqual("HasKey", gorgon[OpCode.HASKEY].Method.Name);
        }

        [TestMethod]
        public void TestSubStrOverflowFaultChangesAfterEchidna()
        {
            var script = CreateSubStrScript(1, int.MaxValue);

            var preEchidnaFault = ExecuteFault(CreateJumpTableProtocolSettings(includeEchidna: false, includeGorgon: false), script);
            var echidnaFault = ExecuteFault(CreateJumpTableProtocolSettings(includeEchidna: true, includeGorgon: false), script);
            var gorgonFault = ExecuteFault(CreateJumpTableProtocolSettings(includeEchidna: true, includeGorgon: true), script);

            StringAssert.Contains(preEchidnaFault, "Array dimensions exceeded supported range");
            StringAssert.Contains(echidnaFault, "Arithmetic operation resulted in an overflow");
            Assert.AreEqual(echidnaFault, gorgonFault);
        }

        [TestMethod]
        public void TestHasKeyNegativeIndexFaultChangesAfterGorgon()
        {
            var script = CreateHasKeyScript(-1);

            var preGorgonFault = ExecuteFault(CreateJumpTableProtocolSettings(includeEchidna: true, includeGorgon: false), script);
            var gorgonFault = ExecuteFault(CreateJumpTableProtocolSettings(includeEchidna: true, includeGorgon: true), script);

            StringAssert.Contains(preGorgonFault, "negative index -1");
            StringAssert.Contains(gorgonFault, "index -1 is invalid");
            Assert.IsFalse(gorgonFault.Contains("negative index", StringComparison.Ordinal));
        }

        private static JumpTable CaptureJumpTable(ProtocolSettings settings)
        {
            var engine = new TestEngine(settings, true);
            using ScriptBuilder scriptBuilder = new();
            scriptBuilder.Emit(OpCode.RET);

            JumpTable? jumpTable = null;
            engine.Execute((Script)scriptBuilder.ToArray(), beforeExecute: e => jumpTable = e.JumpTable);

            Assert.IsNotNull(jumpTable);
            return jumpTable;
        }

        private static Script CreateSubStrScript(int start, int count)
        {
            using ScriptBuilder scriptBuilder = new();
            scriptBuilder.EmitPush("abc");
            scriptBuilder.EmitPush(start);
            scriptBuilder.EmitPush(count);
            scriptBuilder.Emit(OpCode.SUBSTR);
            scriptBuilder.Emit(OpCode.RET);
            return (Script)scriptBuilder.ToArray();
        }

        private static Script CreateHasKeyScript(int key)
        {
            using ScriptBuilder scriptBuilder = new();
            scriptBuilder.EmitPush("abc");
            scriptBuilder.EmitPush(key);
            scriptBuilder.Emit(OpCode.HASKEY);
            scriptBuilder.Emit(OpCode.RET);
            return (Script)scriptBuilder.ToArray();
        }

        private static string ExecuteFault(ProtocolSettings settings, Script script)
        {
            var engine = new TestEngine(settings, true);
            var exception = Assert.ThrowsExactly<TestException>(() => engine.Execute(script));
            return exception.Message;
        }

        private static ProtocolSettings CreateJumpTableProtocolSettings(bool includeEchidna, bool includeGorgon)
        {
            return TestEngine.Default with
            {
                Hardforks = TestEngine.Default.Hardforks.ToImmutableDictionary(
                    p => p.Key,
                    p => p.Key switch
                    {
                        Hardfork.HF_Echidna => includeEchidna ? 0u : uint.MaxValue,
                        Hardfork.HF_Faun => includeEchidna ? 0u : uint.MaxValue,
                        Hardfork.HF_Gorgon => includeGorgon ? 0u : uint.MaxValue,
                        _ => 0u
                    })
            };
        }
    }
}
