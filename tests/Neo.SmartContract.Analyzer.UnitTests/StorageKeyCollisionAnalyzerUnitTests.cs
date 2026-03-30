// Copyright (C) 2015-2026 The Neo Project.
//
// StorageKeyCollisionAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.StorageKeyCollisionAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class StorageKeyCollisionAnalyzerUnitTests
    {
        private const string StorageStubs = """
            namespace Neo.SmartContract.Framework.Services
            {
                public class StorageContext { }
                public static class Storage
                {
                    public static StorageContext CurrentContext { get; } = new StorageContext();
                }

                public class StorageMap
                {
                    public StorageMap(StorageContext context, byte prefix) { }
                    public StorageMap(StorageContext context, byte[] prefix) { }
                    public StorageMap(StorageContext context, string prefix) { }
                }

                public class LocalStorageMap
                {
                    public LocalStorageMap(byte prefix) { }
                    public LocalStorageMap(byte[] prefix) { }
                    public LocalStorageMap(string prefix) { }
                }
            }
            """;

        [TestMethod]
        public async Task DuplicateBytePrefixFields_ReportDiagnostic()
        {
            var test = StorageStubs + """
                public class ContractA
                {
                    private const byte PrefixBalances = 0x01;
                    private const byte PrefixAllowances = 0x01;

                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Balances =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, PrefixBalances);
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap Allowances =
                        new(PrefixAllowances);
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithSpan(29, 82, 29, 92)
                .WithArguments("01", "Allowances", "Balances");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task DuplicateStringPrefixFields_ReportDiagnostic()
        {
            var test = StorageStubs + """
                public class ContractB
                {
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Owners =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, "owner");
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Admins =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, "owner");
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithSpan(26, 77, 26, 83)
                .WithArguments("6F776E6572", "Admins", "Owners");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task DifferentPrefixes_DoNotReportDiagnostic()
        {
            var test = StorageStubs + """
                public class ContractC
                {
                    private const byte PrefixBalances = 0x01;
                    private const byte PrefixAllowances = 0x02;

                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Balances =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, PrefixBalances);
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap Allowances =
                        new(PrefixAllowances);
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task MethodLocalReuse_DoesNotReportDiagnostic()
        {
            var test = StorageStubs + """
                public class ContractD
                {
                    public void Put()
                    {
                        var balances = new Neo.SmartContract.Framework.Services.StorageMap(
                            Neo.SmartContract.Framework.Services.Storage.CurrentContext, (byte)0x01);
                        var balancesAgain = new Neo.SmartContract.Framework.Services.StorageMap(
                            Neo.SmartContract.Framework.Services.Storage.CurrentContext, (byte)0x01);
                    }
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task DuplicateFactoryCreatedPrefixes_ReportDiagnostic()
        {
            var test = StorageStubs + """
                public class ContractE
                {
                    private const byte PrefixShared = 0x2A;

                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Owners = CreateOwners();
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap Admins = CreateAdmins();

                    private static Neo.SmartContract.Framework.Services.StorageMap CreateOwners()
                    {
                        return new Neo.SmartContract.Framework.Services.StorageMap(
                            Neo.SmartContract.Framework.Services.Storage.CurrentContext,
                            PrefixShared);
                    }

                    private static Neo.SmartContract.Framework.Services.LocalStorageMap CreateAdmins() =>
                        new Neo.SmartContract.Framework.Services.LocalStorageMap(PrefixShared);
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithSpan(27, 82, 27, 88)
                .WithArguments("2A", "Admins", "Owners");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task DifferentFactoryCreatedPrefixes_DoNotReportDiagnostic()
        {
            var test = StorageStubs + """
                public class ContractF
                {
                    private const byte PrefixOwners = 0x2A;
                    private const byte PrefixAdmins = 0x2B;

                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Owners = CreateOwners();
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap Admins = CreateAdmins();

                    private static Neo.SmartContract.Framework.Services.StorageMap CreateOwners() =>
                        new Neo.SmartContract.Framework.Services.StorageMap(
                            Neo.SmartContract.Framework.Services.Storage.CurrentContext,
                            PrefixOwners);

                    private static Neo.SmartContract.Framework.Services.LocalStorageMap CreateAdmins()
                    {
                        return new Neo.SmartContract.Framework.Services.LocalStorageMap(PrefixAdmins);
                    }
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task DuplicateNestedFactoryCreatedPrefixes_ReportDiagnostic()
        {
            var test = StorageStubs + """
                public class ContractG
                {
                    private const byte PrefixShared = 0x3C;

                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Owners = CreateOwners();
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap Admins = CreateAdmins();

                    private static Neo.SmartContract.Framework.Services.StorageMap CreateOwners() => CreateOwnersCore();

                    private static Neo.SmartContract.Framework.Services.StorageMap CreateOwnersCore()
                    {
                        return new Neo.SmartContract.Framework.Services.StorageMap(
                            Neo.SmartContract.Framework.Services.Storage.CurrentContext,
                            PrefixShared);
                    }

                    private static Neo.SmartContract.Framework.Services.LocalStorageMap CreateAdmins()
                    {
                        return CreateAdminsCore();
                    }

                    private static Neo.SmartContract.Framework.Services.LocalStorageMap CreateAdminsCore() =>
                        new Neo.SmartContract.Framework.Services.LocalStorageMap(PrefixShared);
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithSpan(27, 82, 27, 88)
                .WithArguments("3C", "Admins", "Owners");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ParameterizedFactoryCreatedPrefixes_DoNotReportDiagnostic()
        {
            var test = StorageStubs + """
                public class ContractH
                {
                    private const byte PrefixShared = 0x4D;

                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Owners = CreateOwners(PrefixShared);
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap Admins = CreateAdmins(PrefixShared);

                    private static Neo.SmartContract.Framework.Services.StorageMap CreateOwners(byte prefix) =>
                        new Neo.SmartContract.Framework.Services.StorageMap(
                            Neo.SmartContract.Framework.Services.Storage.CurrentContext,
                            prefix);

                    private static Neo.SmartContract.Framework.Services.LocalStorageMap CreateAdmins(byte prefix) =>
                        new Neo.SmartContract.Framework.Services.LocalStorageMap(prefix);
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
