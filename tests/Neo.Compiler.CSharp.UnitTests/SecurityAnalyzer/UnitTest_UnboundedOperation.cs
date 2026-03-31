// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_UnboundedOperation.cs file belongs to the neo project and is free
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
    public class UnboundedOperationTests : DebugAndTestBase<Contract_UnboundedOperation>
    {
        [TestMethod]
        public void Test_UnboundedOperation()
        {
            var result = UnboundedOperationAnalyzer.AnalyzeUnboundedOperations(NefFile, Manifest, null);
            // The for loop in Sum currently compiles into a single backward jump at address 113.
            Assert.AreEqual(1, result.backwardJumpAddresses.Count);
            Assert.AreEqual(113, result.backwardJumpAddresses[0]);
        }

        [TestMethod]
        public void Test_UnboundedOperation_WarningInfo()
        {
            var result = UnboundedOperationAnalyzer.AnalyzeUnboundedOperations(NefFile, Manifest, null);
            string warning = result.GetWarningInfo(print: false);
            Assert.IsTrue(warning.Contains("[SECURITY]"));
            Assert.IsTrue(warning.Contains("Backward jumps"));
        }

        [TestMethod]
        public void Test_UnboundedOperation_DetectsDirectRecursiveCall()
        {
            byte[] script =
            [
                (byte)OpCode.CALL_L, 0x00, 0x00, 0x00, 0x00,
                (byte)OpCode.RET
            ];

            var result = UnboundedOperationAnalyzer.AnalyzeUnboundedOperations(CreateNefFile(script), CreateManifest(), null);
            Assert.AreEqual(1, result.recursiveCallAddresses.Count);
            Assert.AreEqual(0, result.recursiveCallAddresses[0]);
        }

        [TestMethod]
        public void Test_UnboundedOperationVulnerability_BackwardJumpOnlyCtor_RemainsCompatible()
        {
            var result = new UnboundedOperationAnalyzer.UnboundedOperationVulnerability(new[] { 7 }, null);
            Assert.AreEqual(1, result.backwardJumpAddresses.Count);
            Assert.AreEqual(0, result.recursiveCallAddresses.Count);
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
