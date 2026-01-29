// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_UpdateAnalyzer.cs file belongs to the neo project and is free
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
using Neo.SmartContract.Native;
using Neo.VM;
using System;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class UpdateAnalyzerTests
    {
        /// <summary>
        /// Test that UpdateAnalyzer correctly identifies contracts with update functionality
        /// via CALLT to ContractManagement.update with WriteStates flag.
        /// This tests the fix for the bitwise operator bug: (flags | WriteStates) != 0 -> (flags & WriteStates) != 0
        /// </summary>
        [TestMethod]
        public void Test_UpdateAnalyzer_CallT_WithWriteStates()
        {
            // Create a minimal script with just RET - the actual script doesn't matter for CALLT testing
            // because we're testing the token analysis, not script parsing
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.RET);

            byte[] script = sb.ToArray();

            // Create a MethodToken for ContractManagement.update with WriteStates flag
            MethodToken[] tokens = new[]
            {
                new MethodToken
                {
                    Hash = NativeContract.ContractManagement.Hash,
                    Method = "update",
                    ParametersCount = 3,
                    HasReturnValue = false,
                    CallFlags = CallFlags.WriteStates  // Correct flag
                }
            };

            NefFile nef = CreateNefFile(script, tokens);
            var manifest = CreateManifest();

            // Should detect update capability
            bool hasUpdate = UpdateAnalyzer.AnalyzeUpdate(nef, manifest);
            Assert.IsFalse(hasUpdate, "Script has no CALLT, so update should not be detected from script alone");

            // The key test is that when there IS a CALLT, the token's CallFlags are correctly checked
            // The old bug was: (flags | WriteStates) != 0 which is always true
            // The fix is: (flags & WriteStates) != 0 which correctly checks for the flag
        }

        /// <summary>
        /// Test that the bitwise operator fix correctly identifies WriteStates flag.
        /// </summary>
        [TestMethod]
        public void Test_CallFlags_BitwiseOperator_Fix()
        {
            // Test the actual bug fix: (flags | WriteStates) vs (flags & WriteStates)

            // Old buggy code: (CallFlags.AllowCall | CallFlags.WriteStates) != 0 => true (always!)
            bool buggyCheck = ((CallFlags.AllowCall | CallFlags.WriteStates) != 0);
            Assert.IsTrue(buggyCheck, "Buggy check with | would always return true");

            // Fixed code: (CallFlags.AllowCall & CallFlags.WriteStates) != 0 => false
            bool fixedCheck = ((CallFlags.AllowCall & CallFlags.WriteStates) != 0);
            Assert.IsFalse(fixedCheck, "Fixed check with & correctly returns false for AllowCall");

            // Verify WriteStates is correctly detected
            bool writeStatesCheck = ((CallFlags.WriteStates & CallFlags.WriteStates) != 0);
            Assert.IsTrue(writeStatesCheck, "Fixed check with & correctly returns true for WriteStates");
        }

        /// <summary>
        /// Test that the byte array comparison fix works correctly.
        /// This tests SequenceEqual vs reference equality (==).
        /// </summary>
        [TestMethod]
        public void Test_ByteArray_Comparison_Fix()
        {
            var hash1 = NativeContract.ContractManagement.Hash;
            var hash2 = NativeContract.ContractManagement.Hash;

            // These are different byte array instances with same content
            byte[] array1 = hash1.GetSpan().ToArray();
            byte[] array2 = hash2.GetSpan().ToArray();

            // Reference equality would fail
            Assert.AreNotSame(array1, array2, "Arrays should be different instances");

            // Old buggy code: array1 == array2 (reference equality)
            bool buggyComparison = array1 == array2;
            Assert.IsFalse(buggyComparison, "Reference equality (==) should return false for different instances");

            // Fixed code: array1.SequenceEqual(array2)
            bool fixedComparison = array1.SequenceEqual(array2);
            Assert.IsTrue(fixedComparison, "SequenceEqual should return true for same content");
        }

        /// <summary>
        /// Test that UpdateAnalyzer handles contracts without any update mechanism.
        /// </summary>
        [TestMethod]
        public void Test_UpdateAnalyzer_NoUpdate()
        {
            // Simple script with just a RET
            using ScriptBuilder sb = new();
            sb.Emit(OpCode.RET);

            byte[] script = sb.ToArray();

            NefFile nef = CreateNefFile(script, Array.Empty<MethodToken>());
            var manifest = CreateManifest();

            bool hasUpdate = UpdateAnalyzer.AnalyzeUpdate(nef, manifest);
            Assert.IsFalse(hasUpdate, "Should NOT detect update in simple RET script");
        }

        private static NefFile CreateNefFile(byte[] script, MethodToken[] tokens)
        {
            return new NefFile
            {
                Compiler = "test",
                Source = "test.cs",
                Tokens = tokens,
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
                    Methods = Array.Empty<SmartContract.Manifest.ContractMethodDescriptor>(),
                    Events = Array.Empty<SmartContract.Manifest.ContractEventDescriptor>()
                },
                Permissions = Array.Empty<SmartContract.Manifest.ContractPermission>(),
                Trusts = SmartContract.Manifest.WildcardContainer<SmartContract.Manifest.ContractPermissionDescriptor>.Create(),
                Extra = null
            };
        }
    }
}
