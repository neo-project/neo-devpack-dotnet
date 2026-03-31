// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Reentrancy.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.SecurityAnalyzer;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Testing;
using Neo.VM;
using System;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class ReentrancyTests : DebugAndTestBase<Contract_Reentrancy>
    {
        [TestMethod]
        public void Test_HasReentrancy()
        {
            ReEntrancyAnalyzer.ReEntrancyVulnerabilityPair v =
                ReEntrancyAnalyzer.AnalyzeSingleContractReEntrancy(NefFile, Manifest);
            Assert.AreEqual(v.vulnerabilityPairs.Count, 3);
            foreach (BasicBlock b in v.vulnerabilityPairs.Keys)
                // basic blocks calling contract
                Assert.IsTrue(b.startAddr < NefFile.Size * 0.66);
            v.GetWarningInfo(print: false);
        }

        [TestMethod]
        public void Test_ReentrancyWithEnhancedDiagnostics()
        {
            // Test enhanced diagnostic messages without debug info (fallback behavior)
            ReEntrancyAnalyzer.ReEntrancyVulnerabilityPair v =
                ReEntrancyAnalyzer.AnalyzeSingleContractReEntrancy(NefFile, Manifest, null);
            Assert.AreEqual(v.vulnerabilityPairs.Count, 3);

            // Test that warning message contains enhanced diagnostic information
            string warningInfo = v.GetWarningInfo(print: false);

            // Verify enhanced diagnostic format
            Assert.IsTrue(warningInfo.Contains("[SECURITY] Potential Re-entrancy vulnerability detected"));
            Assert.IsTrue(warningInfo.Contains("External contract calls:"));
            Assert.IsTrue(warningInfo.Contains("Storage writes that occur after external calls:"));
            Assert.IsTrue(warningInfo.Contains("Recommendation:"));
            Assert.IsTrue(warningInfo.Contains("allowing potential re-entrancy attacks"));
            Assert.IsTrue(warningInfo.Contains("reentrancy guards"));

            // Message should be more detailed than just addresses
            Assert.IsTrue(warningInfo.Length > 300, "Enhanced diagnostic message should be more detailed than simple address listing");
        }

        [TestMethod]
        public void Test_ReentrancyAnalyzer_Treats_LocalStorageWrites_As_StateWrites()
        {
            byte[] script =
            [
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Contract_Call.Hash),
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Storage_Local_Put.Hash),
                (byte)OpCode.RET
            ];

            var nef = CreateNefFile(script);
            var manifest = CreateManifest();

            var result = ReEntrancyAnalyzer.AnalyzeSingleContractReEntrancy(nef, manifest);
            Assert.AreEqual(1, result.vulnerabilityPairs.Count, "Local storage writes after external calls should be tracked as reentrancy-relevant writes.");
        }

        [TestMethod]
        public void Test_ReentrancyAnalyzer_Treats_LocalStorageDelete_As_StateWrite()
        {
            byte[] script =
            [
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Contract_Call.Hash),
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Storage_Local_Delete.Hash),
                (byte)OpCode.RET
            ];

            var result = ReEntrancyAnalyzer.AnalyzeSingleContractReEntrancy(CreateNefFile(script), CreateManifest(), null);
            Assert.AreEqual(1, result.vulnerabilityPairs.Count);
        }

        [TestMethod]
        public void Test_ReentrancyAnalyzer_DoesNotWarn_On_LocalStoragePut_Without_ExternalCall()
        {
            byte[] script =
            [
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Storage_Local_Put.Hash),
                (byte)OpCode.RET
            ];

            var result = ReEntrancyAnalyzer.AnalyzeSingleContractReEntrancy(CreateNefFile(script), CreateManifest(), null);
            Assert.AreEqual(0, result.vulnerabilityPairs.Count);
        }

        [TestMethod]
        public void Test_ReentrancyAnalyzer_DoesNotWarn_On_LocalStorageDelete_Without_ExternalCall()
        {
            byte[] script =
            [
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Storage_Local_Delete.Hash),
                (byte)OpCode.RET
            ];

            var result = ReEntrancyAnalyzer.AnalyzeSingleContractReEntrancy(CreateNefFile(script), CreateManifest(), null);
            Assert.AreEqual(0, result.vulnerabilityPairs.Count);
        }

        [TestMethod]
        public void Test_ReentrancyAnalyzer_DoesNotTreat_LocalStorageGet_As_StateWrite()
        {
            byte[] script =
            [
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Contract_Call.Hash),
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Storage_Local_Get.Hash),
                (byte)OpCode.RET
            ];

            var result = ReEntrancyAnalyzer.AnalyzeSingleContractReEntrancy(CreateNefFile(script), CreateManifest(), null);
            Assert.AreEqual(0, result.vulnerabilityPairs.Count);
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
