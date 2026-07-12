// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_MissingCheckWitness.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.SecurityAnalyzer;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Native;
using Neo.SmartContract.Testing;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class MissingCheckWitnessTests : DebugAndTestBase<Contract_MissingCheckWitness>
    {
        [TestMethod]
        public void Test_MissingCheckWitness()
        {
            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(NefFile, Manifest, null);
            // UnsafeUpdate writes storage without CheckWitness - should be flagged
            Assert.IsTrue(result.vulnerableMethodNames.Contains("unsafeUpdate"));
            // UnsafeLocalUpdate writes local storage without CheckWitness - should be flagged
            Assert.IsTrue(result.vulnerableMethodNames.Contains("unsafeLocalUpdate"));
            // SafeUpdate has CheckWitness - should NOT be flagged
            Assert.IsFalse(result.vulnerableMethodNames.Contains("safeUpdate"));
            // SafeUpdateViaHelper delegates CheckWitness to helper - should NOT be flagged
            Assert.IsFalse(result.vulnerableMethodNames.Contains("safeUpdateViaHelper"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_WarningInfo()
        {
            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(NefFile, Manifest, null);
            string warning = result.GetWarningInfo(print: false);
            Assert.IsTrue(warning.Contains("[SECURITY]"));
            Assert.IsTrue(warning.Contains("unsafeUpdate"));
            Assert.IsTrue(warning.Contains("unsafeLocalUpdate"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_DoesNotSkip_UnderscorePrefixedPublicAbiMethods()
        {
            const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""_admin_transfer"")]
    public static void AdminTransfer()
    {
        Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
    }
}";

            var context = TestHelper.CompileSingleContract(source);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                context.CreateExecutable(),
                context.CreateManifest(),
                null);

            Assert.IsTrue(result.vulnerableMethodNames.Contains("_admin_transfer"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_Skips_Deploy_And_Initialize_Callbacks()
        {
            const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public class Contract : SmartContract
{
    public static void _deploy(object data, bool update)
    {
        Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
    }

    public static void _initialize()
    {
        Storage.Put(Storage.CurrentContext, new byte[] { 0x02 }, 1);
    }
}";

            var context = TestHelper.CompileSingleContract(source);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                context.CreateExecutable(),
                context.CreateManifest(),
                null);

            Assert.IsFalse(result.vulnerableMethodNames.Contains("_deploy"));
            Assert.IsFalse(result.vulnerableMethodNames.Contains("_initialize"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_DoesNotFlag_DominatingWitness()
        {
            const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public class Contract : SmartContract
{
    public static void GuardedUpdate(UInt160 owner)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(owner));
        Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
    }
}";

            var result = AnalyzeSource(source);

            Assert.IsFalse(result.vulnerableMethodNames.Contains("guardedUpdate"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_Flags_WhenBranchBypassesWitness()
        {
            const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

public class Contract : SmartContract
{
    public static void BranchBypass(UInt160 owner, bool checkWitness)
    {
        if (checkWitness)
            ExecutionEngine.Assert(Runtime.CheckWitness(owner));

        Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
    }

    public static void LifecycleBranchBypass(UInt160 owner, bool checkWitness)
    {
        if (checkWitness)
            ExecutionEngine.Assert(Runtime.CheckWitness(owner));

        ContractManagement.Destroy();
    }
}";

            var result = AnalyzeSource(source);

            Assert.IsTrue(result.vulnerableMethodNames.Contains("branchBypass"));
            Assert.IsTrue(result.unauthenticatedLifecycleMethodNames.Contains("lifecycleBranchBypass"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_RequiresHelperWitnessToDominateWrite()
        {
            const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public class Contract : SmartContract
{
    public static void GuardedViaHelper(UInt160 owner)
    {
        EnsureWitness(owner);
        Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
    }

    public static void HelperBranchBypass(UInt160 owner, bool verifyOnly)
    {
        if (verifyOnly)
        {
            EnsureWitness(owner);
            return;
        }

        Storage.Put(Storage.CurrentContext, new byte[] { 0x02 }, 1);
    }

    private static void EnsureWitness(UInt160 owner)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(owner));
    }
}";

            var result = AnalyzeSource(source);

            Assert.IsFalse(result.vulnerableMethodNames.Contains("guardedViaHelper"));
            Assert.IsTrue(result.vulnerableMethodNames.Contains("helperBranchBypass"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_DistinguishesRecursiveAndGuaranteedHelpers()
        {
            const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public class Contract : SmartContract
{
    public static void RecursiveHelperUpdate(UInt160 owner, bool recurse, bool checkWitness)
    {
        MaybeCheckWitness(owner, recurse, checkWitness);
        Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
    }

    public static void GuaranteedHelperUpdate(UInt160 owner)
    {
        EnsureWitness(owner);
        Storage.Put(Storage.CurrentContext, new byte[] { 0x02 }, 1);
    }

    private static void MaybeCheckWitness(UInt160 owner, bool recurse, bool checkWitness)
    {
        if (recurse)
            MaybeCheckWitness(owner, false, checkWitness);

        if (!checkWitness)
            return;

        EnsureWitness(owner);
    }

    private static void EnsureWitness(UInt160 owner)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(owner));
    }
}";

            var result = AnalyzeSource(source);

            Assert.IsTrue(result.vulnerableMethodNames.Contains("recursiveHelperUpdate"));
            Assert.IsFalse(result.vulnerableMethodNames.Contains("guaranteedHelperUpdate"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_DoesNotTreatNonReturningHelperAsWitness()
        {
            const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System;

public class Contract : SmartContract
{
    public static void CatchWrite()
    {
        try
        {
            AlwaysThrow();
        }
        catch
        {
            Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
        }
    }

    private static void AlwaysThrow()
    {
        throw new Exception();
    }
}";

            var result = AnalyzeSource(source);

            Assert.IsTrue(result.vulnerableMethodNames.Contains("catchWrite"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_Flags_DynamicCall_WithoutWitness()
        {
            const int helperOffset = 8;
            byte[] script =
            [
                (byte)OpCode.PUSHA, .. BitConverter.GetBytes(helperOffset),
                (byte)OpCode.CALLA,
                (byte)OpCode.RET,
                (byte)OpCode.RET,
                (byte)OpCode.SYSCALL, .. BitConverter.GetBytes(ApplicationEngine.System_Storage_Put.Hash),
                (byte)OpCode.RET
            ];

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                CreateNefFile(script),
                CreateManifest(
                    Method("dynamicWrite", 0),
                    Method("_deploy", 7)),
                null);

            Assert.IsTrue(result.vulnerableMethodNames.Contains("dynamicWrite"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_Flags_ContractManagementUpdate_ViaCallT_WithoutWitness()
        {
            byte[] script =
            [
                (byte)OpCode.CALLT, 0x00, 0x00,
                (byte)OpCode.RET
            ];

            MethodToken[] tokens =
            [
                LifecycleToken("update", CallFlags.All)
            ];

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                CreateNefFile(script, tokens),
                CreateManifest(Method("unsafeUpgrade", 0)),
                null);

            Assert.IsTrue(result.unauthenticatedLifecycleMethodNames.Contains("unsafeUpgrade"));
            // It does not write storage, so it must not appear in the storage-write list.
            Assert.IsFalse(result.vulnerableMethodNames.Contains("unsafeUpgrade"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_Flags_ContractManagementDestroy_ViaCallT_WithoutWitness()
        {
            byte[] script =
            [
                (byte)OpCode.CALLT, 0x00, 0x00,
                (byte)OpCode.RET
            ];

            MethodToken[] tokens =
            [
                LifecycleToken("destroy", CallFlags.All)
            ];

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                CreateNefFile(script, tokens),
                CreateManifest(Method("unsafeDestroy", 0)),
                null);

            Assert.IsTrue(result.unauthenticatedLifecycleMethodNames.Contains("unsafeDestroy"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_DoesNotFlag_ContractManagementUpdate_WhenWitnessPresent()
        {
            // SYSCALL CheckWitness, then CALLT update, then RET.
            var script = new List<byte>();
            script.Add((byte)OpCode.SYSCALL);
            script.AddRange(BitConverter.GetBytes(ApplicationEngine.System_Runtime_CheckWitness.Hash));
            script.Add((byte)OpCode.CALLT);
            script.Add(0x00);
            script.Add(0x00);
            script.Add((byte)OpCode.RET);

            MethodToken[] tokens =
            [
                LifecycleToken("update", CallFlags.All)
            ];

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                CreateNefFile(script.ToArray(), tokens),
                CreateManifest(Method("guardedUpgrade", 0)),
                null);

            Assert.IsFalse(result.unauthenticatedLifecycleMethodNames.Contains("guardedUpgrade"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_DoesNotFlag_NonWriteCapableContractManagementToken()
        {
            byte[] script =
            [
                (byte)OpCode.CALLT, 0x00, 0x00,
                (byte)OpCode.RET
            ];

            // Read-only call flags must not be reported as a lifecycle mutation.
            MethodToken[] tokens =
            [
                LifecycleToken("update", CallFlags.ReadOnly)
            ];

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                CreateNefFile(script, tokens),
                CreateManifest(Method("readOnlyish", 0)),
                null);

            Assert.IsFalse(result.unauthenticatedLifecycleMethodNames.Contains("readOnlyish"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_DoesNotFlag_InvalidLifecycleTokenIndex()
        {
            byte[] script =
            [
                (byte)OpCode.CALLT, 0x01, 0x00,
                (byte)OpCode.RET
            ];

            MethodToken[] tokens =
            [
                LifecycleToken("update", CallFlags.All)
            ];

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                CreateNefFile(script, tokens),
                CreateManifest(Method("badToken", 0)),
                null);

            Assert.IsFalse(result.unauthenticatedLifecycleMethodNames.Contains("badToken"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_DoesNotFlag_NonLifecycleContractManagementToken()
        {
            byte[] script =
            [
                (byte)OpCode.CALLT, 0x00, 0x00,
                (byte)OpCode.RET
            ];

            MethodToken[] tokens =
            [
                LifecycleToken("getContract", CallFlags.All)
            ];

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                CreateNefFile(script, tokens),
                CreateManifest(Method("readContract", 0)),
                null);

            Assert.IsFalse(result.unauthenticatedLifecycleMethodNames.Contains("readContract"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_Flags_ContractManagementUpdate_ViaSyscallPattern()
        {
            byte[] updateBytes = Encoding.UTF8.GetBytes("update");
            byte[] hashBytes = NativeContract.ContractManagement.Hash.GetSpan().ToArray();

            var script = new List<byte>();
            script.Add((byte)OpCode.PUSHDATA1);
            script.Add((byte)updateBytes.Length);
            script.AddRange(updateBytes);
            script.Add((byte)OpCode.PUSHDATA1);
            script.Add((byte)hashBytes.Length);
            script.AddRange(hashBytes);
            script.Add((byte)OpCode.SYSCALL);
            script.AddRange(BitConverter.GetBytes(ApplicationEngine.System_Contract_Call.Hash));
            script.Add((byte)OpCode.RET);

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                CreateNefFile(script.ToArray(), Array.Empty<MethodToken>()),
                CreateManifest(Method("unsafeUpgrade", 0)),
                null);

            Assert.IsTrue(result.unauthenticatedLifecycleMethodNames.Contains("unsafeUpgrade"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_DoesNotFlag_ShortContractCallSyscallPattern()
        {
            byte[] script =
            [
                (byte)OpCode.SYSCALL,
                .. BitConverter.GetBytes(ApplicationEngine.System_Contract_Call.Hash),
                (byte)OpCode.RET
            ];

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                CreateNefFile(script, Array.Empty<MethodToken>()),
                CreateManifest(Method("tooShort", 0)),
                null);

            Assert.IsFalse(result.unauthenticatedLifecycleMethodNames.Contains("tooShort"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_DoesNotFlag_NonLifecycleContractCallSyscallPattern()
        {
            byte[] methodBytes = Encoding.UTF8.GetBytes("balanceOf");
            byte[] hashBytes = NativeContract.ContractManagement.Hash.GetSpan().ToArray();

            var script = new List<byte>();
            script.Add((byte)OpCode.PUSHDATA1);
            script.Add((byte)methodBytes.Length);
            script.AddRange(methodBytes);
            script.Add((byte)OpCode.PUSHDATA1);
            script.Add((byte)hashBytes.Length);
            script.AddRange(hashBytes);
            script.Add((byte)OpCode.SYSCALL);
            script.AddRange(BitConverter.GetBytes(ApplicationEngine.System_Contract_Call.Hash));
            script.Add((byte)OpCode.RET);

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                CreateNefFile(script.ToArray(), Array.Empty<MethodToken>()),
                CreateManifest(Method("readCall", 0)),
                null);

            Assert.IsFalse(result.unauthenticatedLifecycleMethodNames.Contains("readCall"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_DoesNotFlag_WrongLifecycleSyscallHash()
        {
            byte[] updateBytes = Encoding.UTF8.GetBytes("update");
            byte[] hashBytes = new byte[20];

            var script = new List<byte>();
            script.Add((byte)OpCode.PUSHDATA1);
            script.Add((byte)updateBytes.Length);
            script.AddRange(updateBytes);
            script.Add((byte)OpCode.PUSHDATA1);
            script.Add((byte)hashBytes.Length);
            script.AddRange(hashBytes);
            script.Add((byte)OpCode.SYSCALL);
            script.AddRange(BitConverter.GetBytes(ApplicationEngine.System_Contract_Call.Hash));
            script.Add((byte)OpCode.RET);

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                CreateNefFile(script.ToArray(), Array.Empty<MethodToken>()),
                CreateManifest(Method("wrongHash", 0)),
                null);

            Assert.IsFalse(result.unauthenticatedLifecycleMethodNames.Contains("wrongHash"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_WarningInfo_IncludesLifecycleSection()
        {
            byte[] script =
            [
                (byte)OpCode.CALLT, 0x00, 0x00,
                (byte)OpCode.RET
            ];

            MethodToken[] tokens =
            [
                LifecycleToken("update", CallFlags.All)
            ];

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                CreateNefFile(script, tokens),
                CreateManifest(Method("unsafeUpgrade", 0)),
                null);

            string warning = result.GetWarningInfo(print: false);
            Assert.IsTrue(warning.Contains("[SECURITY]"));
            Assert.IsTrue(warning.Contains("ContractManagement.Update/Destroy"));
            Assert.IsTrue(warning.Contains("unsafeUpgrade"));
        }

        private static MethodToken LifecycleToken(string method, CallFlags flags)
        {
            return new MethodToken
            {
                Hash = NativeContract.ContractManagement.Hash,
                Method = method,
                ParametersCount = (ushort)(method == "destroy" ? 0 : 3),
                HasReturnValue = false,
                CallFlags = flags
            };
        }

        private static NefFile CreateNefFile(byte[] script)
            => CreateNefFile(script, Array.Empty<MethodToken>());

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

        private static ContractManifest CreateManifest(params ContractMethodDescriptor[] methods)
        {
            return new ContractManifest
            {
                Name = "TestContract",
                Groups = Array.Empty<ContractGroup>(),
                SupportedStandards = Array.Empty<string>(),
                Abi = new ContractAbi
                {
                    Methods = methods,
                    Events = Array.Empty<ContractEventDescriptor>()
                },
                Permissions = Array.Empty<ContractPermission>(),
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
                Extra = null
            };
        }

        private static ContractMethodDescriptor Method(string name, int offset)
        {
            return new ContractMethodDescriptor
            {
                Name = name,
                Offset = offset,
                Parameters = Array.Empty<ContractParameterDefinition>(),
                ReturnType = ContractParameterType.Void,
                Safe = false
            };
        }

        private static MissingCheckWitnessAnalyzer.MissingCheckWitnessVulnerability AnalyzeSource(string source)
        {
            var context = TestHelper.CompileSingleContract(source);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

            return MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                context.CreateExecutable(),
                context.CreateManifest(),
                null);
        }
    }
}
