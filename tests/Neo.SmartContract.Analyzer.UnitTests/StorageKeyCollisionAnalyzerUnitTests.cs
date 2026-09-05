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
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
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

        private const string BaseStubs = """
            namespace Neo.SmartContract.Framework
            {
                public abstract class Ownable { }
                public abstract class Pausable { }
                public abstract class TokenContract { }
                public abstract class Nep17Token : TokenContract { }
                public abstract class Nep11Token<TState> : TokenContract { }
            }
            """;

        private const string ExtendedBaseStubs = """
            namespace Neo.SmartContract.Framework
            {
                public abstract class Ownable2Step { }
                public abstract class AccessControl { }
                public abstract class PausableOwnable : Ownable { }
                public abstract class RoyaltyNep11Token<TState> : Nep11Token<TState> { }
            }
            """;

        [DataTestMethod]
        [DataRow(true, "Storage.CurrentContext, (byte)0x2A")]
        [DataRow(false, "Storage.CurrentContext, (byte)0x2A")]
        [DataRow(true, "context: Storage.CurrentContext, prefix: (byte)0x2A")]
        [DataRow(false, "context: Storage.CurrentContext, prefix: (byte)0x2A")]
        [DataRow(true, "prefix: (byte)0x2A, context: Storage.CurrentContext")]
        [DataRow(false, "prefix: (byte)0x2A, context: Storage.CurrentContext")]
        [DataRow(true, "context: Storage.CurrentContext, (byte)0x2A")]
        [DataRow(false, "context: Storage.CurrentContext, (byte)0x2A")]
        [DataRow(true, "Storage.CurrentContext, prefix: (byte)0x2A")]
        [DataRow(false, "Storage.CurrentContext, prefix: (byte)0x2A")]
        public async Task ConstructorArgumentOrder_DuplicatePrefix_ReportsDiagnostic(bool explicitType, string arguments)
        {
            var creationType = explicitType ? "StorageMap" : "";
            var test = "using Neo.SmartContract.Framework.Services;\n" + StorageStubs + $$"""
                public class Contract
                {
                    private static readonly StorageMap Owners = new(Storage.CurrentContext, (byte)0x2A);
                    private static readonly StorageMap {|#0:Admins|} = new {{creationType}}({{arguments}});
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("2A", "Admins", "Owners");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task ReorderedNamedArguments_DifferentPrefixes_NoDiagnostic(bool explicitType)
        {
            var creationType = explicitType ? "StorageMap" : "";
            var test = "using Neo.SmartContract.Framework.Services;\n" + StorageStubs + $$"""
                public class Contract
                {
                    private static readonly StorageMap Owners = new(Storage.CurrentContext, (byte)0x2A);
                    private static readonly StorageMap Admins =
                        new {{creationType}}(prefix: (byte)0x2B, context: Storage.CurrentContext);
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task NamedFactoryAndLocalStoragePrefixes_ReportDiagnostic()
        {
            var test = "using Neo.SmartContract.Framework.Services;\n" + StorageStubs + """
                public class Contract
                {
                    private static readonly StorageMap Owners = CreateOwners();
                    private static readonly LocalStorageMap {|#0:Admins|} = new(prefix: (byte)0x2A);

                    private static StorageMap CreateOwners() =>
                        new StorageMap(prefix: (byte)0x2A, context: Storage.CurrentContext);
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("2A", "Admins", "Owners");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task InheritedOwnablePrefix_ReportsDiagnostic()
        {
            var test = StorageStubs + BaseStubs + """
                public class TokenA : Neo.SmartContract.Framework.Ownable
                {
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Balances =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, (byte)0xFF);
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithSpan(31, 77, 31, 85)
                .WithArguments("FF", "Balances", "the reserved prefix of base class Ownable");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task InheritedPausablePrefix_ReportsDiagnostic()
        {
            var test = StorageStubs + BaseStubs + """
                public class TokenB : Neo.SmartContract.Framework.Pausable
                {
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap Flags =
                        new((byte)0xFE);
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithSpan(31, 82, 31, 87)
                .WithArguments("FE", "Flags", "the reserved prefix of base class Pausable");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task InheritedNep17BalancePrefix_ReportsDiagnostic()
        {
            var test = StorageStubs + BaseStubs + """
                public class TokenE : Neo.SmartContract.Framework.Nep17Token
                {
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap Balances =
                        new((byte)0x01);
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithSpan(31, 82, 31, 90)
                .WithArguments("01", "Balances", "the reserved prefix of base class Nep17Token");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task InheritedNep11TokenPrefix_ReportsDiagnostic()
        {
            var test = StorageStubs + BaseStubs + """
                public class TokenF : Neo.SmartContract.Framework.Nep11Token<object>
                {
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Tokens =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, (byte)0x03);
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithSpan(31, 77, 31, 83)
                .WithArguments("03", "Tokens", "the reserved prefix of base class Nep11Token");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task InheritedOwnable2StepPrefix_ReportsDiagnostic()
        {
            var test = StorageStubs + BaseStubs + ExtendedBaseStubs + """
                public class TokenG : Neo.SmartContract.Framework.Ownable2Step
                {
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap PendingOwners =
                        new((byte)0xFC);
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithSpan(37, 82, 37, 95)
                .WithArguments("FC", "PendingOwners", "the reserved prefix of base class Ownable2Step");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task InheritedOwnable2StepInitializedPrefix_ReportsDiagnostic()
        {
            var test = StorageStubs + BaseStubs + ExtendedBaseStubs + """
                public class TokenG : Neo.SmartContract.Framework.Ownable2Step
                {
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap OwnerInitialized =
                        new((byte)0xFB);
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithSpan(37, 82, 37, 98)
                .WithArguments("FB", "OwnerInitialized", "the reserved prefix of base class Ownable2Step");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task InheritedAccessControlPrefix_ReportsDiagnostic()
        {
            var test = StorageStubs + BaseStubs + ExtendedBaseStubs + """
                public class TokenH : Neo.SmartContract.Framework.AccessControl
                {
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Roles =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, (byte)0xFB);
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithSpan(37, 77, 37, 82)
                .WithArguments("FB", "Roles", "the reserved prefix of base class AccessControl");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task InheritedPausableOwnablePrefix_ReportsDiagnostic()
        {
            var test = StorageStubs + BaseStubs + ExtendedBaseStubs + """
                public class TokenI : Neo.SmartContract.Framework.PausableOwnable
                {
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap Paused =
                        new((byte)0xFE);
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithSpan(37, 82, 37, 88)
                .WithArguments("FE", "Paused", "the reserved prefix of base class PausableOwnable");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task InheritedRoyaltyNep11TokenPrefix_ReportsDiagnostic()
        {
            var test = StorageStubs + BaseStubs + ExtendedBaseStubs + """
                public class TokenJ : Neo.SmartContract.Framework.RoyaltyNep11Token<object>
                {
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap TokenRoyalties =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, (byte)0x06);
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithSpan(37, 77, 37, 91)
                .WithArguments("06", "TokenRoyalties", "the reserved prefix of base class RoyaltyNep11Token");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task NonInheritedReservedPrefix_DoesNotReportDiagnostic()
        {
            // A contract that does not inherit Ownable is free to use 0xFF for its own data.
            var test = StorageStubs + BaseStubs + """
                public class TokenC
                {
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Balances =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, (byte)0xFF);
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task InheritedOwnable_DifferentPrefix_DoesNotReportDiagnostic()
        {
            var test = StorageStubs + BaseStubs + """
                public class TokenD : Neo.SmartContract.Framework.Ownable
                {
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Balances =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, (byte)0x01);
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

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

        [TestMethod]
        public async Task DuplicatePrefixesAcrossPartialDeclarations_ReportDiagnostic()
        {
            var test = StorageStubs + """
                public partial class ContractI
                {
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Owners =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, (byte)0x2A);
                }

                public partial class ContractI
                {
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap {|#0:Admins|} =
                        new((byte)0x2A);
                }
                """;

            var expected = VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("2A", "Admins", "Owners");

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task DuplicatePrefixesAcrossDifferentTypes_DoNotReportDiagnostic()
        {
            var test = StorageStubs + """
                public class ContractJ
                {
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Owners =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, (byte)0x2A);
                }

                public class ContractK
                {
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap Admins =
                        new((byte)0x2A);
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task DuplicatePrefixesAcrossPartialFiles_ReportDiagnostic()
        {
            var test = new CSharpAnalyzerTest<StorageKeyCollisionAnalyzer, DefaultVerifier>();
            test.TestState.Sources.Add(("StorageStubs.cs", StorageStubs));
            test.TestState.Sources.Add(("Contract.Part1.cs", """
                public partial class ContractL
                {
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Owners =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, (byte)0x3B);
                }
                """));
            test.TestState.Sources.Add(("Contract.Part2.cs", """
                public partial class ContractL
                {
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap {|#0:Admins|} =
                        new((byte)0x3B);
                }
                """));
            test.ExpectedDiagnostics.Add(
                VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                    .WithLocation(0)
                    .WithArguments("3B", "Admins", "Owners"));

            await test.RunAsync();
        }

        [TestMethod]
        public async Task DuplicateReadonlyByteArrayPrefixesAcrossPartialFiles_ReportDiagnostic()
        {
            var test = new CSharpAnalyzerTest<StorageKeyCollisionAnalyzer, DefaultVerifier>();
            test.TestState.Sources.Add(("StorageStubs.cs", StorageStubs));
            test.TestState.Sources.Add(("Contract.Prefixes.cs", """
                public partial class ContractM
                {
                    private static readonly byte[] SharedPrefix = new byte[] { 0x4C, 0x01 };
                }
                """));
            test.TestState.Sources.Add(("Contract.Storage.cs", """
                public partial class ContractM
                {
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Owners =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, SharedPrefix);
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap {|#0:Admins|} =
                        new(SharedPrefix);
                }
                """));
            test.ExpectedDiagnostics.Add(
                VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                    .WithLocation(0)
                    .WithArguments("4C01", "Admins", "Owners"));

            await test.RunAsync();
        }

        [TestMethod]
        public async Task DifferentReadonlyByteArrayPrefixesAcrossPartialFiles_DoNotReportDiagnostic()
        {
            var test = new CSharpAnalyzerTest<StorageKeyCollisionAnalyzer, DefaultVerifier>();
            test.TestState.Sources.Add(("StorageStubs.cs", StorageStubs));
            test.TestState.Sources.Add(("Contract.Prefixes.cs", """
                public partial class ContractN
                {
                    private static readonly byte[] OwnersPrefix = new byte[] { 0x4D, 0x01 };
                    private static readonly byte[] AdminsPrefix = new byte[] { 0x4D, 0x02 };
                }
                """));
            test.TestState.Sources.Add(("Contract.Storage.cs", """
                public partial class ContractN
                {
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Owners =
                        new(Neo.SmartContract.Framework.Services.Storage.CurrentContext, OwnersPrefix);
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap Admins =
                        new(AdminsPrefix);
                }
                """));

            await test.RunAsync();
        }

        [TestMethod]
        public async Task DuplicateFactoryCreatedPrefixesAcrossPartialFiles_ReportDiagnostic()
        {
            var test = new CSharpAnalyzerTest<StorageKeyCollisionAnalyzer, DefaultVerifier>();
            test.TestState.Sources.Add(("StorageStubs.cs", StorageStubs));
            test.TestState.Sources.Add(("Contract.Factories.cs", """
                public partial class ContractO
                {
                    private static Neo.SmartContract.Framework.Services.StorageMap CreateOwners() =>
                        new Neo.SmartContract.Framework.Services.StorageMap(
                            Neo.SmartContract.Framework.Services.Storage.CurrentContext,
                            (byte)0x5E);

                    private static Neo.SmartContract.Framework.Services.LocalStorageMap CreateAdmins()
                    {
                        return new Neo.SmartContract.Framework.Services.LocalStorageMap((byte)0x5E);
                    }
                }
                """));
            test.TestState.Sources.Add(("Contract.Storage.cs", """
                public partial class ContractO
                {
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Owners = CreateOwners();
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap {|#0:Admins|} = CreateAdmins();
                }
                """));
            test.ExpectedDiagnostics.Add(
                VerifyCS.Diagnostic(StorageKeyCollisionAnalyzer.DiagnosticId)
                    .WithLocation(0)
                    .WithArguments("5E", "Admins", "Owners"));

            await test.RunAsync();
        }

        [TestMethod]
        public async Task DifferentFactoryCreatedPrefixesAcrossPartialFiles_DoNotReportDiagnostic()
        {
            var test = new CSharpAnalyzerTest<StorageKeyCollisionAnalyzer, DefaultVerifier>();
            test.TestState.Sources.Add(("StorageStubs.cs", StorageStubs));
            test.TestState.Sources.Add(("Contract.Factories.cs", """
                public partial class ContractP
                {
                    private static Neo.SmartContract.Framework.Services.StorageMap CreateOwners() =>
                        new Neo.SmartContract.Framework.Services.StorageMap(
                            Neo.SmartContract.Framework.Services.Storage.CurrentContext,
                            (byte)0x5F);

                    private static Neo.SmartContract.Framework.Services.LocalStorageMap CreateAdmins()
                    {
                        return new Neo.SmartContract.Framework.Services.LocalStorageMap((byte)0x60);
                    }
                }
                """));
            test.TestState.Sources.Add(("Contract.Storage.cs", """
                public partial class ContractP
                {
                    private static readonly Neo.SmartContract.Framework.Services.StorageMap Owners = CreateOwners();
                    private static readonly Neo.SmartContract.Framework.Services.LocalStorageMap Admins = CreateAdmins();
                }
                """));

            await test.RunAsync();
        }
    }
}
