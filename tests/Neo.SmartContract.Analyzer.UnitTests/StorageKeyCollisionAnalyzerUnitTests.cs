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
    }
}
