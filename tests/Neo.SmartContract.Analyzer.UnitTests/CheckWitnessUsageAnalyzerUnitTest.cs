// Copyright (C) 2015-2025 The Neo Project.
//
// CheckWitnessUsageAnalyzerUnitTest.cs file belongs to the neo project and is free
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
    public class CheckWitnessUsageAnalyzerUnitTest
    {
        [TestMethod]
        public async Task CheckWitnessUsageAnalyzer_UnverifiedResult_ShouldReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;
                                      using Neo.SmartContract.Framework.Services;

                                      public class TestContract : SmartContract
                                      {
                                          public static void Main(UInt160 owner)
                                          {
                                              // Unverified CheckWitness result
                                              Runtime.CheckWitness(owner);
                                          }
                                      }
                                      """;

            var test = VerifyCS.CreateAnalyzerTest<CheckWitnessUsageAnalyzer>();
            test.TestCode = sourceCode;

            // Add all expected diagnostics
            // Compiler errors
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));

            // CS1705 errors
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));

            // Specific errors with spans
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(4, 14, 4, 26).WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(6, 19, 6, 23).WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(6, 29, 6, 36).WithArguments("System.Object"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0103").WithSpan(9, 9, 9, 16).WithArguments("Runtime"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 9, 9, 16).WithArguments("System.Object"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 17, 9, 29).WithArguments("System.Object"));

            // Our analyzer diagnostic
            test.ExpectedDiagnostics.Add(new DiagnosticResult(CheckWitnessUsageAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
                .WithSpan(9, 15, 9, 41)
                .WithArguments("Use Assert(Runtime.CheckWitness(...)) or if (Runtime.CheckWitness(...))"));

            await test.RunAsync();
        }

        [TestMethod]
        public async Task CheckWitnessUsageAnalyzer_VerifiedWithAssert_ShouldNotReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;
                                      using Neo.SmartContract.Framework.Services;

                                      public class TestContract : SmartContract
                                      {
                                          public static void Main(UInt160 owner)
                                          {
                                              // Verified with Assert
                                              Assert(Runtime.CheckWitness(owner));
                                          }
                                      }
                                      """;

            var test = VerifyCS.CreateAnalyzerTest<CheckWitnessUsageAnalyzer>();
            test.TestCode = sourceCode;

            // Add all expected diagnostics
            // Compiler errors
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));

            // CS1705 errors
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));

            // Specific errors with spans
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(4, 14, 4, 26).WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(6, 19, 6, 23).WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(6, 29, 6, 36).WithArguments("System.Object"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0103").WithSpan(9, 9, 9, 15).WithArguments("Assert"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0103").WithSpan(9, 16, 9, 23).WithArguments("Runtime"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 22, 9, 29).WithArguments("System.Object"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 30, 9, 33).WithArguments("System.Object"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 34, 9, 39).WithArguments("System.String"));

            await test.RunAsync();
        }

        [TestMethod]
        public async Task CheckWitnessUsageAnalyzer_VerifiedWithIfCondition_ShouldNotReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;
                                      using Neo.SmartContract.Framework.Services;

                                      public class TestContract : SmartContract
                                      {
                                          public static void Main(UInt160 owner)
                                          {
                                              // Verified with if condition
                                              if (Runtime.CheckWitness(owner))
                                              {
                                                  // Authorized code
                                              }
                                          }
                                      }
                                      """;

            var test = VerifyCS.CreateAnalyzerTest<CheckWitnessUsageAnalyzer>();
            test.TestCode = sourceCode;

            // Add all expected diagnostics
            // Compiler errors
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));

            // CS1705 errors
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));

            // Specific errors with spans
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(4, 14, 4, 26).WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(6, 19, 6, 23).WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(6, 29, 6, 36).WithArguments("System.Object"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0103").WithSpan(9, 13, 9, 20).WithArguments("Runtime"));

            await test.RunAsync();
        }

        [TestMethod]
        public async Task CheckWitnessUsageAnalyzer_FixWithAssert()
        {
            const string originalCode = """
                                        using Neo.SmartContract.Framework;
                                        using Neo.SmartContract.Framework.Services;

                                        public class TestContract : SmartContract
                                        {
                                            public static void Main(UInt160 owner)
                                            {
                                                // Unverified CheckWitness result
                                                Runtime.CheckWitness(owner);
                                            }
                                        }
                                        """;

            const string fixedCode = """
                                     using Neo.SmartContract.Framework;
                                     using Neo.SmartContract.Framework.Services;

                                     public class TestContract : SmartContract
                                     {
                                         public static void Main(UInt160 owner)
                                         {
                                             // Unverified CheckWitness result
                                             Assert(Runtime.CheckWitness(owner));
                                         }
                                     }
                                     """;

            var test = VerifyCS.CreateCodeFixTest<CheckWitnessUsageAnalyzer, CheckWitnessUsageCodeFixProvider>();
            test.TestCode = originalCode;
            test.FixedCode = fixedCode;

            // Add all expected diagnostics for the test state
            // Compiler errors
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));

            // CS1705 errors
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));

            // Specific errors with spans
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(4, 14, 4, 26).WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(6, 19, 6, 23).WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(6, 29, 6, 36).WithArguments("System.Object"));

            // Our analyzer diagnostic
            test.ExpectedDiagnostics.Add(new DiagnosticResult(CheckWitnessUsageAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
                .WithSpan(9, 15, 9, 41)
                .WithArguments("Use Assert(Runtime.CheckWitness(...)) or if (Runtime.CheckWitness(...))"));

            // Clear fixed state diagnostics
            test.FixedState.ExpectedDiagnostics.Clear();

            // Add all expected diagnostics for the fixed state
            // Compiler errors
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));

            // CS1705 errors
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));

            // Specific errors with spans
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(4, 14, 4, 26).WithArguments("System.Void"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(6, 19, 6, 23).WithArguments("System.Void"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(6, 29, 6, 36).WithArguments("System.Object"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0103").WithSpan(9, 9, 9, 15).WithArguments("Assert"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0103").WithSpan(9, 16, 9, 23).WithArguments("Runtime"));

            await test.RunAsync();
        }

        [TestMethod]
        public async Task CheckWitnessUsageAnalyzer_FixWithIfCondition()
        {
            const string originalCode = """
                                        using Neo.SmartContract.Framework;
                                        using Neo.SmartContract.Framework.Services;

                                        public class TestContract : SmartContract
                                        {
                                            public static void Main(UInt160 owner)
                                            {
                                                // Unverified CheckWitness result
                                                Runtime.CheckWitness(owner);
                                            }
                                        }
                                        """;

            const string fixedCode = """
                                     using Neo.SmartContract.Framework;
                                     using Neo.SmartContract.Framework.Services;

                                     public class TestContract : SmartContract
                                     {
                                         public static void Main(UInt160 owner)
                                         {
                                             // Unverified CheckWitness result
                                             if (Runtime.CheckWitness(owner))
                                             {
                                                 // Add your code here
                                             }
                                             else
                                             {
                                                 throw new Exception("No authorization.");
                                             }
                                         }
                                     }
                                     """;

            var test = VerifyCS.CreateCodeFixTest<CheckWitnessUsageAnalyzer, CheckWitnessUsageCodeFixProvider>();
            test.TestCode = originalCode;
            test.FixedCode = fixedCode;
            test.CodeActionIndex = 1; // Use the second code fix (if condition)

            // Add all expected diagnostics for the test state
            // Compiler errors
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));

            // CS1705 errors
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));

            // Specific errors with spans
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(4, 14, 4, 26).WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(6, 19, 6, 23).WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(6, 29, 6, 36).WithArguments("System.Object"));

            // Our analyzer diagnostic
            test.ExpectedDiagnostics.Add(new DiagnosticResult(CheckWitnessUsageAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
                .WithSpan(9, 15, 9, 41)
                .WithArguments("Use Assert(Runtime.CheckWitness(...)) or if (Runtime.CheckWitness(...))"));

            // Clear fixed state diagnostics
            test.FixedState.ExpectedDiagnostics.Clear();

            // Add all expected diagnostics for the fixed state
            // Compiler errors
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));

            // CS1705 errors
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS1705")
                .WithArguments("Neo.SmartContract.Framework", "Neo.SmartContract.Framework, Version=3.7.4.0, Culture=neutral, PublicKeyToken=null",
                              "System.Runtime, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Runtime",
                              "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));

            // Specific errors with spans
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(4, 14, 4, 26).WithArguments("System.Void"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(6, 19, 6, 23).WithArguments("System.Void"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(6, 29, 6, 36).WithArguments("System.Object"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0103").WithSpan(9, 13, 9, 20).WithArguments("Runtime"));
            test.FixedState.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0103").WithSpan(15, 18, 15, 26).WithArguments("Exception"));

            // Note: We're testing the "Use in if condition" code fix here by specifying CodeActionIndex = 1
            // This selects the second code fix provider action

            await test.RunAsync();
        }
    }
}
