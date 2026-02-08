// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_AnalyzerCompat.cs file belongs to the neo project and is free
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
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class AnalyzerCompatibilityTests
    {
        [TestMethod]
        public void Test_OldAnalyzerNames_DelegateToNew()
        {
            byte[] script = new byte[] { (byte)OpCode.RET };
            var nef = CreateNefFile(script);
            var manifest = CreateManifest();

            bool newUpdate = UpdateAnalyzer.AnalyzeUpdate(nef, manifest);
            bool oldUpdate = UpdateAnalzyer.AnalyzeUpdate(nef, manifest);
            Assert.AreEqual(newUpdate, oldUpdate);

            var newWrite = WriteInTryAnalyzer.AnalyzeWriteInTry(nef, manifest);
            var oldWrite = WriteInTryAnalzyer.AnalyzeWriteInTry(nef, manifest);
            Assert.AreEqual(newWrite.Vulnerabilities.Count, oldWrite.Vulnerabilities.Count);
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

        private static ContractManifest CreateManifest()
        {
            return new ContractManifest
            {
                Name = "TestContract",
                Groups = Array.Empty<ContractGroup>(),
                SupportedStandards = Array.Empty<string>(),
                Abi = new ContractAbi
                {
                    Methods = Array.Empty<ContractMethodDescriptor>(),
                    Events = Array.Empty<ContractEventDescriptor>()
                },
                Permissions = Array.Empty<ContractPermission>(),
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
                Extra = null
            };
        }
    }
}
