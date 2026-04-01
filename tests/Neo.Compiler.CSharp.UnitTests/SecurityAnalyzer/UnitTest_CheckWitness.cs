// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_CheckWitness.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.SecurityAnalyzer;
using Neo.SmartContract;
using Neo.SmartContract.Testing;
using Neo.VM;
using System;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class CheckWitnessTests : DebugAndTestBase<Contract_CheckWitness>
    {
        [TestMethod]
        public void Test_CheckWitness()
        {
            var result = CheckWitnessAnalyzer.AnalyzeCheckWitness(NefFile, Manifest, null);
            Assert.AreEqual(result.droppedCheckWitnessResults.Count, 1);
        }

        [TestMethod]
        public void Test_CheckWitness_DetectsDropAfterNop()
        {
            byte[] script =
            [
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Runtime_CheckWitness.Hash),
                (byte)OpCode.NOP,
                (byte)OpCode.DROP,
                (byte)OpCode.RET
            ];

            var result = CheckWitnessAnalyzer.AnalyzeCheckWitness(CreateNefFile(script), CreateManifest(), null);
            Assert.AreEqual(1, result.droppedCheckWitnessResults.Count);
        }

        [TestMethod]
        public void Test_CheckWitness_DetectsDropAfterSimpleLocalRoundTrip()
        {
            byte[] script =
            [
                (byte)OpCode.INITSLOT, 0x01, 0x00,
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Runtime_CheckWitness.Hash),
                (byte)OpCode.STLOC0,
                (byte)OpCode.LDLOC0,
                (byte)OpCode.DROP,
                (byte)OpCode.RET
            ];

            var result = CheckWitnessAnalyzer.AnalyzeCheckWitness(CreateNefFile(script), CreateManifest(), null);
            Assert.AreEqual(1, result.droppedCheckWitnessResults.Count);
        }

        [TestMethod]
        public void Test_CheckWitness_DetectsDropAfterGenericLocalRoundTrip()
        {
            byte[] script =
            [
                (byte)OpCode.INITSLOT, 0x08, 0x00,
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Runtime_CheckWitness.Hash),
                (byte)OpCode.STLOC, 0x07,
                (byte)OpCode.NOP,
                (byte)OpCode.LDLOC, 0x07,
                (byte)OpCode.NOP,
                (byte)OpCode.DROP,
                (byte)OpCode.RET
            ];

            var result = CheckWitnessAnalyzer.AnalyzeCheckWitness(CreateNefFile(script), CreateManifest(), null);
            Assert.AreEqual(1, result.droppedCheckWitnessResults.Count);
        }

        [TestMethod]
        public void Test_CheckWitness_IgnoresTrailingNopWithoutConsumer()
        {
            byte[] script =
            [
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Runtime_CheckWitness.Hash),
                (byte)OpCode.NOP
            ];

            var result = CheckWitnessAnalyzer.AnalyzeCheckWitness(CreateNefFile(script), CreateManifest(), null);
            Assert.AreEqual(0, result.droppedCheckWitnessResults.Count);
        }

        [TestMethod]
        public void Test_CheckWitness_IgnoresNonDroppedStoredResult()
        {
            byte[] script =
            [
                (byte)OpCode.INITSLOT, 0x01, 0x00,
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Runtime_CheckWitness.Hash),
                (byte)OpCode.STLOC0,
                (byte)OpCode.LDLOC0,
                (byte)OpCode.RET
            ];

            var result = CheckWitnessAnalyzer.AnalyzeCheckWitness(CreateNefFile(script), CreateManifest(), null);
            Assert.AreEqual(0, result.droppedCheckWitnessResults.Count);
        }

        [TestMethod]
        public void Test_CheckWitness_IgnoresStoreWithoutReload()
        {
            byte[] script =
            [
                (byte)OpCode.INITSLOT, 0x01, 0x00,
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Runtime_CheckWitness.Hash),
                (byte)OpCode.STLOC0,
                (byte)OpCode.RET
            ];

            var result = CheckWitnessAnalyzer.AnalyzeCheckWitness(CreateNefFile(script), CreateManifest(), null);
            Assert.AreEqual(0, result.droppedCheckWitnessResults.Count);
        }

        [TestMethod]
        public void Test_CheckWitness_IgnoresMismatchedLocalRoundTrip()
        {
            byte[] script =
            [
                (byte)OpCode.INITSLOT, 0x02, 0x00,
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Runtime_CheckWitness.Hash),
                (byte)OpCode.STLOC0,
                (byte)OpCode.LDLOC1,
                (byte)OpCode.DROP,
                (byte)OpCode.RET
            ];

            var result = CheckWitnessAnalyzer.AnalyzeCheckWitness(CreateNefFile(script), CreateManifest(), null);
            Assert.AreEqual(0, result.droppedCheckWitnessResults.Count);
        }

        [TestMethod]
        public void Test_CheckWitness_IgnoresNonLocalConsumer()
        {
            byte[] script =
            [
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Runtime_CheckWitness.Hash),
                (byte)OpCode.NZ,
                (byte)OpCode.RET
            ];

            var result = CheckWitnessAnalyzer.AnalyzeCheckWitness(CreateNefFile(script), CreateManifest(), null);
            Assert.AreEqual(0, result.droppedCheckWitnessResults.Count);
        }

        private static NefFile CreateNefFile(byte[] script)
        {
            return new NefFile
            {
                Compiler = "test",
                Source = "test.cs",
                Tokens = Array.Empty<MethodToken>(),
                Script = script
            };
        }

        private static SmartContract.Manifest.ContractManifest CreateManifest()
        {
            return new SmartContract.Manifest.ContractManifest
            {
                Name = "TestContract",
                Groups = Array.Empty<SmartContract.Manifest.ContractGroup>(),
                SupportedStandards = Array.Empty<string>(),
                Abi = new SmartContract.Manifest.ContractAbi
                {
                    Methods =
                    [
                        new SmartContract.Manifest.ContractMethodDescriptor
                        {
                            Name = "main",
                            Offset = 0,
                            Parameters = Array.Empty<SmartContract.Manifest.ContractParameterDefinition>(),
                            ReturnType = ContractParameterType.Void,
                            Safe = false
                        }
                    ],
                    Events = Array.Empty<SmartContract.Manifest.ContractEventDescriptor>()
                },
                Permissions = Array.Empty<SmartContract.Manifest.ContractPermission>(),
                Trusts = SmartContract.Manifest.WildcardContainer<SmartContract.Manifest.ContractPermissionDescriptor>.Create(),
                Extra = null
            };
        }
    }
}
