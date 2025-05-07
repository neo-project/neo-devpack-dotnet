// Copyright (C) 2015-2025 The Neo Project.
//
// ContractAttributeAnalyzerUnitTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using VerifyCS = Neo.SmartContract.Analyzer.UnitTests.TestHelper;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class ContractAttributeAnalyzerUnitTest
    {
        [TestMethod]
        public async Task ContractAttributeAnalyzer_MissingAttributes_ShouldReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;

                                      public class TestContract : SmartContract
                                      {
                                          public static void Main()
                                          {
                                              // Some code
                                          }
                                      }
                                      """;

            var test = VerifyCS.CreateAnalyzerTest<ContractAttributeAnalyzer>();
            test.TestCode = sourceCode;
            test.ExpectedDiagnostics.Add(new DiagnosticResult(ContractAttributeAnalyzer.DiagnosticIdMissingAttributes, DiagnosticSeverity.Warning)
                .WithSpan(3, 14, 3, 26)
                .WithArguments("DisplayName, ContractDescription, ContractVersion, ContractPermission"));

            await test.RunAsync();
        }

        [TestMethod]
        public async Task ContractAttributeAnalyzer_AllRequiredAttributes_ShouldNotReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;
                                      using Neo.SmartContract.Framework.Attributes;

                                      [DisplayName("Test Contract")]
                                      [ContractDescription("This is a test contract")]
                                      [ContractVersion("1.0.0")]
                                      [ContractPermission(Permission.Any, Method.Any)]
                                      public class TestContract : SmartContract
                                      {
                                          public static void Main()
                                          {
                                              // Some code
                                          }
                                      }
                                      """;

            var test = VerifyCS.CreateAnalyzerTest<ContractAttributeAnalyzer>();
            test.TestCode = sourceCode;
            // No diagnostics expected

            await test.RunAsync();
        }

        [TestMethod]
        public async Task ContractAttributeAnalyzer_InvalidAttribute_ShouldReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;
                                      using Neo.SmartContract.Framework.Attributes;

                                      [DisplayName]
                                      public class TestContract : SmartContract
                                      {
                                          public static void Main()
                                          {
                                              // Some code
                                          }
                                      }
                                      """;

            var test = VerifyCS.CreateAnalyzerTest<ContractAttributeAnalyzer>();
            test.TestCode = sourceCode;
            test.ExpectedDiagnostics.Add(new DiagnosticResult(ContractAttributeAnalyzer.DiagnosticIdInvalidAttribute, DiagnosticSeverity.Warning)
                .WithSpan(4, 2, 4, 14)
                .WithArguments("DisplayName attribute requires a string parameter"));

            await test.RunAsync();
        }

        [TestMethod]
        public async Task ContractAttributeAnalyzer_FixMissingAttributes()
        {
            const string originalCode = """
                                        using Neo.SmartContract.Framework;

                                        public class TestContract : SmartContract
                                        {
                                            public static void Main()
                                            {
                                                // Some code
                                            }
                                        }
                                        """;

            const string fixedCode = """
                                     using Neo.SmartContract.Framework;

                                     [DisplayName("TestContract")]
                                     [ContractDescription("Description of TestContract")]
                                     [ContractVersion("1.0.0")]
                                     [ContractPermission(Permission.Any, Method.Any)]
                                     public class TestContract : SmartContract
                                     {
                                         public static void Main()
                                         {
                                             // Some code
                                         }
                                     }
                                     """;

            var test = VerifyCS.CreateCodeFixTest<ContractAttributeAnalyzer, ContractAttributeCodeFixProvider>();
            test.TestCode = originalCode;
            test.FixedCode = fixedCode;
            test.ExpectedDiagnostics.Add(new DiagnosticResult(ContractAttributeAnalyzer.DiagnosticIdMissingAttributes, DiagnosticSeverity.Warning)
                .WithSpan(3, 14, 3, 26)
                .WithArguments("DisplayName, ContractDescription, ContractVersion, ContractPermission"));

            await test.RunAsync();
        }

        [TestMethod]
        public async Task ContractAttributeAnalyzer_FixInvalidAttribute()
        {
            const string originalCode = """
                                        using Neo.SmartContract.Framework;
                                        using Neo.SmartContract.Framework.Attributes;

                                        [DisplayName]
                                        public class TestContract : SmartContract
                                        {
                                            public static void Main()
                                            {
                                                // Some code
                                            }
                                        }
                                        """;

            const string fixedCode = """
                                     using Neo.SmartContract.Framework;
                                     using Neo.SmartContract.Framework.Attributes;

                                     [DisplayName("MyContract")]
                                     public class TestContract : SmartContract
                                     {
                                         public static void Main()
                                         {
                                             // Some code
                                         }
                                     }
                                     """;

            var test = VerifyCS.CreateCodeFixTest<ContractAttributeAnalyzer, ContractAttributeCodeFixProvider>();
            test.TestCode = originalCode;
            test.FixedCode = fixedCode;
            test.ExpectedDiagnostics.Add(new DiagnosticResult(ContractAttributeAnalyzer.DiagnosticIdInvalidAttribute, DiagnosticSeverity.Warning)
                .WithSpan(4, 2, 4, 14)
                .WithArguments("DisplayName attribute requires a string parameter"));

            await test.RunAsync();
        }
    }
}
