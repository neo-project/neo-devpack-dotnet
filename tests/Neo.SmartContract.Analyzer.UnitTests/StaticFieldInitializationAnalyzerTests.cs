// Copyright (C) 2015-2026 The Neo Project.
//
// StaticFieldInitializationAnalyzerTests.cs file belongs to the neo project and is free
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
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<Neo.SmartContract.Analyzer.StaticFieldInitializationAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class StaticFieldInitializationAnalyzerUnitTest
    {
        private const string FrameworkProjectName = "Neo.SmartContract.Framework";

        private const string FrameworkTypes = """
            namespace Neo.SmartContract.Framework
            {
                public abstract class UInt160
                {
                    public static extern implicit operator UInt160(string value);
                }

                public abstract class UInt256
                {
                    public static extern implicit operator UInt256(string value);
                }

                public abstract class ECPoint
                {
                    public static extern implicit operator ECPoint(string value);
                }
            }
            """;

        [DataTestMethod]
        [DataRow("UInt160", "null")]
        [DataRow("UInt160", "default")]
        [DataRow("UInt160", "default(UInt160)")]
        [DataRow("UInt256", "null")]
        [DataRow("UInt256", "default")]
        [DataRow("UInt256", "default(UInt256)")]
        [DataRow("ECPoint", "null")]
        [DataRow("ECPoint", "default")]
        [DataRow("ECPoint", "default(ECPoint)")]
        public async Task NullOrDefaultInitialization_NoDiagnostic(string typeName, string initializer)
        {
            var code = CreateFrameworkTypeCode(typeName, $"unset = {initializer}");

            await VerifyAnalyzerAsync(code);
        }

        [TestMethod]
        public async Task NullAndInvalidStringInSameDeclaration_ReportsOnlyInvalidString()
        {
            var code = CreateFrameworkTypeCode(
                "UInt160",
                """unset = null, {|#0:invalid = "invalid"|}, uninitialized = default""");

            var expectedDiagnostic = Verifier.Diagnostic(StaticFieldInitializationAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("UInt160 must be initialized with a 40-character hex string or a 34-character string starting with 'N'.");

            await VerifyAnalyzerAsync(code, expectedDiagnostic);
        }

        [TestMethod]
        public async Task ValidUInt256Initialization_NoDiagnostic()
        {
            var code = CreateFrameworkTypeCode(
                "UInt256",
                "validUInt256 = \"edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925\"");

            await VerifyAnalyzerAsync(code);
        }

        [TestMethod]
        public async Task InvalidUInt256Initialization_ReportsDiagnostic()
        {
            var code = CreateFrameworkTypeCode("UInt256", """{|#0:invalidUInt256 = "invalid"|}""");

            var expectedDiagnostic = Verifier.Diagnostic("NC4023")
                .WithLocation(0).WithArguments("UInt256 must be initialized with a 64-character hex string.");

            await VerifyAnalyzerAsync(code, expectedDiagnostic);
        }

        [TestMethod]
        public async Task ValidUInt160HexInitialization_NoDiagnostic()
        {
            var code = CreateFrameworkTypeCode(
                "UInt160",
                "validUInt160 = \"1a4fe29ad7db232a493e5b990fb1da7af0c7b989\"");

            await VerifyAnalyzerAsync(code);
        }

        [TestMethod]
        public async Task ValidUInt160AddressInitialization_NoDiagnostic()
        {
            var code = CreateFrameworkTypeCode(
                "UInt160",
                "validUInt160 = \"NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq\"");

            await VerifyAnalyzerAsync(code);
        }

        [TestMethod]
        public async Task InvalidUInt160Initialization_ReportsDiagnostic()
        {
            var code = CreateFrameworkTypeCode("UInt160", """{|#0:invalidUInt160 = "invalid"|}""");

            var expectedDiagnostic = Verifier.Diagnostic("NC4023")
                .WithLocation(0).WithArguments("UInt160 must be initialized with a 40-character hex string or a 34-character string starting with 'N'.");

            await VerifyAnalyzerAsync(code, expectedDiagnostic);
        }

        [TestMethod]
        public async Task ValidECPointInitialization_NoDiagnostic()
        {
            var code = CreateFrameworkTypeCode(
                "ECPoint",
                "validECPoint = \"024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9\"");

            await VerifyAnalyzerAsync(code);
        }

        [TestMethod]
        public async Task InvalidECPointInitialization_ReportsDiagnostic()
        {
            var code = CreateFrameworkTypeCode("ECPoint", """{|#0:invalidECPoint = "invalid"|}""");

            var expectedDiagnostic = Verifier.Diagnostic("NC4023")
                .WithLocation(0).WithArguments("ECPoint must be initialized with a 66-character hex string.");

            await VerifyAnalyzerAsync(code, expectedDiagnostic);
        }

        [TestMethod]
        public async Task InvalidFullyQualifiedUInt160Initialization_ReportsDiagnostic()
        {
            const string code = """
                                public class Test
                                {
                                    private static readonly Neo.SmartContract.Framework.UInt160 {|#0:invalidUInt160 = "invalid"|};
                                }
                                """;

            var expectedDiagnostic = Verifier.Diagnostic(StaticFieldInitializationAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("UInt160 must be initialized with a 40-character hex string or a 34-character string starting with 'N'.");

            await VerifyAnalyzerAsync(code, expectedDiagnostic);
        }

        [TestMethod]
        public async Task InvalidAliasedUInt256Initialization_ReportsDiagnostic()
        {
            const string code = """
                                using ContractHash = Neo.SmartContract.Framework.UInt256;

                                public class Test
                                {
                                    private static readonly ContractHash {|#0:invalidUInt256 = "invalid"|};
                                }
                                """;

            var expectedDiagnostic = Verifier.Diagnostic(StaticFieldInitializationAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("UInt256 must be initialized with a 64-character hex string.");

            await VerifyAnalyzerAsync(code, expectedDiagnostic);
        }

        [TestMethod]
        public async Task NestedFrameworkNamespaceUInt160_NoDiagnostic()
        {
            const string code = """
                                namespace Neo.SmartContract.Framework
                                {
                                    public class Container
                                    {
                                        public abstract class UInt160
                                        {
                                            public static extern implicit operator UInt160(string value);
                                        }
                                    }
                                }

                                public class Test
                                {
                                    private static readonly Neo.SmartContract.Framework.Container.UInt160 nestedUInt160 = "invalid";
                                }
                                """;

            await VerifyAnalyzerAsync(code);
        }

        [TestMethod]
        public async Task SourceDefinedFrameworkUInt160_NoDiagnostic()
        {
            const string code = """
                                #pragma warning disable CS0436

                                namespace Neo.SmartContract.Framework
                                {
                                    public abstract class UInt160
                                    {
                                        public static extern implicit operator UInt160(string value);
                                    }
                                }

                                public class Test
                                {
                                    private static readonly Neo.SmartContract.Framework.UInt160 sourceDefinedUInt160 = "invalid";
                                }
                                """;

            await VerifyAnalyzerAsync(code);
        }

        [TestMethod]
        public async Task UnrelatedUInt160Type_NoDiagnostic()
        {
            const string code = """
                                namespace Example
                                {
                                    public abstract class UInt160
                                    {
                                        public static extern implicit operator UInt160(string value);
                                    }
                                }

                                namespace Consumer
                                {
                                    using Example;

                                    public class Test
                                    {
                                        private static readonly UInt160 unrelatedUInt160 = "invalid";
                                    }
                                }
                                """;

            await VerifyAnalyzerAsync(code);
        }

        private static string CreateFrameworkTypeCode(string typeName, string declaration) => $$"""
            using Neo.SmartContract.Framework;

            public class Test
            {
                private static readonly {{typeName}} {{declaration}};
            }
            """;

        private static Task VerifyAnalyzerAsync(string code, params DiagnosticResult[] expectedDiagnostics)
        {
            var test = new CSharpAnalyzerTest<StaticFieldInitializationAnalyzer, DefaultVerifier>
            {
                TestCode = code
            };
            test.TestState.AdditionalProjects[FrameworkProjectName].Sources.Add(FrameworkTypes);
            test.TestState.AdditionalProjectReferences.Add(FrameworkProjectName);
            test.ExpectedDiagnostics.AddRange(expectedDiagnostics);
            return test.RunAsync();
        }
    }
}
