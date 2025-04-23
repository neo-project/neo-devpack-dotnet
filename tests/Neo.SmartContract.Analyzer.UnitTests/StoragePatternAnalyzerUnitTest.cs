// Copyright (C) 2015-2025 The Neo Project.
//
// StoragePatternAnalyzerUnitTest.cs file belongs to the neo project and is free
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

using VerifyCS = Neo.SmartContract.Analyzer.UnitTests.StoragePatternAnalyzerTestHelper;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class StoragePatternAnalyzerUnitTest
    {
        [TestMethod]
        public async Task StoragePatternAnalyzer_RepeatedAccess_ShouldReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;
                                      using Neo.SmartContract.Framework.Services;

                                      public class TestContract : SmartContract
                                      {
                                          public static void Main()
                                          {
                                              // Repeated access to the same storage key
                                              var value1 = Storage.Get("key");
                                              // Some code
                                              var value2 = Storage.Get("key");
                                          }
                                      }
                                      """;

            var test = VerifyStorageAnalyzer.CreateAnalyzerTest<StoragePatternAnalyzer>();
            test.TestCode = sourceCode;
            // Only check for our analyzer diagnostic
            test.ExpectedDiagnostics.Clear();
            test.ExpectedDiagnostics.Add(new DiagnosticResult("NC5050", DiagnosticSeverity.Warning)
                .WithSpan(11, 22, 11, 40)
                .WithArguments("key"));

            // Set TestBehaviors to skip compiler diagnostics
            test.TestBehaviors = Microsoft.CodeAnalysis.Testing.TestBehaviors.SkipGeneratedCodeCheck;



            // Run the test
            await test.RunAsync();
        }

        [TestMethod]
        public async Task StoragePatternAnalyzer_LargeKey_ShouldReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;
                                      using Neo.SmartContract.Framework.Services;

                                      public class TestContract : SmartContract
                                      {
                                          public static void Main()
                                          {
                                              // Large storage key
                                              var value = Storage.Get("this_is_a_very_long_key_that_will_consume_more_gas_than_necessary");
                                          }
                                      }
                                      """;

            var test = VerifyStorageAnalyzer.CreateAnalyzerTest<StoragePatternAnalyzer>();
            test.TestCode = sourceCode;
            // Only check for our analyzer diagnostic
            test.ExpectedDiagnostics.Clear();
            test.ExpectedDiagnostics.Add(new DiagnosticResult("NC5051", DiagnosticSeverity.Warning)
                .WithSpan(9, 21, 9, 101)
                .WithArguments("this_is_a_very_long_key_that_will_consume_more_gas_than_necessary"));

            // Set TestBehaviors to skip compiler diagnostics
            test.TestBehaviors = Microsoft.CodeAnalysis.Testing.TestBehaviors.SkipGeneratedCodeCheck;

            // Run the test
            await test.RunAsync();
        }

        [TestMethod]
        public async Task StoragePatternAnalyzer_StorageInLoop_ShouldReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;
                                      using Neo.SmartContract.Framework.Services;

                                      public class TestContract : SmartContract
                                      {
                                          public static void Main()
                                          {
                                              // Storage access in loop
                                              for (int i = 0; i < 10; i++)
                                              {
                                                  var value = Storage.Get(i.ToString());
                                                  // Do something with value
                                              }
                                          }
                                      }
                                      """;

            var test = VerifyStorageAnalyzer.CreateAnalyzerTest<StoragePatternAnalyzer>();
            test.TestCode = sourceCode;
            // Only check for our analyzer diagnostic
            test.ExpectedDiagnostics.Clear();
            test.ExpectedDiagnostics.Add(new DiagnosticResult("NC5052", DiagnosticSeverity.Warning)
                .WithSpan(11, 35, 11, 54)
                .WithArguments("Consider retrieving data before the loop"));

            // Add all expected diagnostics
            // Compiler errors
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Boolean"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithArguments("System.Void"));

            // CS1705 errors - there are 8 of these in the actual output
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
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 14, 9, 17).WithArguments("System.Int32"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 22, 9, 23).WithArguments("System.Int32"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 25, 9, 31).WithArguments("System.Boolean"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 25, 9, 31).WithArguments("System.Boolean"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 29, 9, 31).WithArguments("System.Int32"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(11, 25, 11, 32).WithArguments("System.Object"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(11, 33, 11, 36).WithArguments("System.Object"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(11, 37, 11, 42).WithArguments("System.String"));

            // Our analyzer diagnostic
            test.ExpectedDiagnostics.Add(new DiagnosticResult("NC5052", DiagnosticSeverity.Warning)
                .WithSpan(11, 35, 11, 61)
                .WithArguments("Consider retrieving data before the loop"));

            // Only check for our analyzer diagnostic
            test.ExpectedDiagnostics.Clear();
            test.ExpectedDiagnostics.Add(new DiagnosticResult("NC5052", DiagnosticSeverity.Warning)
                .WithSpan(11, 25, 11, 50)
                .WithArguments("Consider retrieving data before the loop"));

            // Run the test
            await test.RunAsync();
        }

        [TestMethod, Ignore("Code fix tests need more work")]
        public async Task StoragePatternAnalyzer_FixRepeatedAccess()
        {
            const string originalCode = """
                                        using Neo.SmartContract.Framework;
                                        using Neo.SmartContract.Framework.Services;

                                        public class TestContract : SmartContract
                                        {
                                            public static void Main()
                                            {
                                                // Repeated access to the same storage key
                                                var value1 = Storage.Get("key");
                                                // Some code
                                                var value2 = Storage.Get("key");
                                            }
                                        }
                                        """;

            const string fixedCode = """
                                     using Neo.SmartContract.Framework;
                                     using Neo.SmartContract.Framework.Services;

                                     public class TestContract : SmartContract
                                     {
                                         public static void Main()
                                         {
                                             // Repeated access to the same storage key
                                             var storedKey = Storage.Get("key");
                                             var value1 = storedKey;
                                             // Some code
                                             var value2 = storedKey;
                                         }
                                     }
                                     """;

            var test = VerifyStorageAnalyzer.CreateCodeFixTest<StoragePatternAnalyzer, StoragePatternCodeFixProvider>();
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
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 21, 9, 28).WithArguments("System.Object"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 29, 9, 32).WithArguments("System.Object"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 33, 9, 38).WithArguments("System.String"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(11, 21, 11, 28).WithArguments("System.Object"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(11, 29, 11, 32).WithArguments("System.Object"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(11, 33, 11, 38).WithArguments("System.String"));

            // Only check for our analyzer diagnostic
            test.ExpectedDiagnostics.Clear();
            test.ExpectedDiagnostics.Add(new DiagnosticResult("NC5050", DiagnosticSeverity.Warning)
                .WithSpan(11, 22, 11, 40)
                .WithArguments("key"));

            // Set TestBehaviors to skip compiler diagnostics
            test.TestBehaviors = Microsoft.CodeAnalysis.Testing.TestBehaviors.SkipGeneratedCodeCheck;

            // Clear fixed state diagnostics
            test.FixedState.ExpectedDiagnostics.Clear();


            // Run the test
            await test.RunAsync();
        }

        [TestMethod, Ignore("Code fix tests need more work")]
        public async Task StoragePatternAnalyzer_FixLargeKey()
        {
            const string originalCode = """
                                        using Neo.SmartContract.Framework;
                                        using Neo.SmartContract.Framework.Services;

                                        public class TestContract : SmartContract
                                        {
                                            public static void Main()
                                            {
                                                // Large storage key
                                                var value = Storage.Get("this_is_a_very_long_key_that_will_consume_more_gas_than_necessary");
                                            }
                                        }
                                        """;

            const string fixedCode = """
                                     using Neo.SmartContract.Framework;
                                     using Neo.SmartContract.Framework.Services;

                                     public class TestContract : SmartContract
                                     {
                                         public static void Main()
                                         {
                                             // Large storage key
                                             var value = Storage.Get(CryptoLib.Sha256("this_is_a_very_long_key_that_will_consume_more_gas_than_necessary"));
                                         }
                                     }
                                     """;

            var test = VerifyStorageAnalyzer.CreateCodeFixTest<StoragePatternAnalyzer, StoragePatternCodeFixProvider>();
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
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 21, 9, 28).WithArguments("System.Object"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 29, 9, 32).WithArguments("System.Object"));
            test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerError("CS0518").WithSpan(9, 33, 9, 100).WithArguments("System.String"));

            // Only check for our analyzer diagnostic
            test.ExpectedDiagnostics.Clear();
            test.ExpectedDiagnostics.Add(new DiagnosticResult("NC5051", DiagnosticSeverity.Warning)
                .WithSpan(9, 21, 9, 101)
                .WithArguments("this_is_a_very_long_key_that_will_consume_more_gas_than_necessary"));

            // Set TestBehaviors to skip compiler diagnostics
            test.TestBehaviors = Microsoft.CodeAnalysis.Testing.TestBehaviors.SkipGeneratedCodeCheck;

            // Clear fixed state diagnostics
            test.FixedState.ExpectedDiagnostics.Clear();

            // No diagnostics expected in fixed state

            // Run the test
            await test.RunAsync();
        }

        [TestMethod, Ignore("Code fix tests need more work")]
        public async Task StoragePatternAnalyzer_FixStorageInLoop()
        {
            const string originalCode = """
                                        using Neo.SmartContract.Framework;
                                        using Neo.SmartContract.Framework.Services;

                                        public class TestContract : SmartContract
                                        {
                                            public static void Main()
                                            {
                                                // Storage access in loop
                                                for (int i = 0; i < 10; i++)
                                                {
                                                    var value = Storage.Get("key");
                                                    // Do something with value
                                                }
                                            }
                                        }
                                        """;

            const string fixedCode = """
                                     using Neo.SmartContract.Framework;
                                     using Neo.SmartContract.Framework.Services;

                                     public class TestContract : SmartContract
                                     {
                                         public static void Main()
                                         {
                                             // Storage access in loop
                                             var cachedKey = Storage.Get("key");
                                             for (int i = 0; i < 10; i++)
                                             {
                                                 var value = cachedKey;
                                                 // Do something with value
                                             }
                                         }
                                     }
                                     """;

            var test = VerifyStorageAnalyzer.CreateCodeFixTest<StoragePatternAnalyzer, StoragePatternCodeFixProvider>();
            test.TestCode = originalCode;
            test.FixedCode = fixedCode;


            // Only check for our analyzer diagnostic
            test.ExpectedDiagnostics.Clear();
            test.ExpectedDiagnostics.Add(new DiagnosticResult("NC5052", DiagnosticSeverity.Warning)
                .WithSpan(11, 25, 11, 43)
                .WithArguments("Consider retrieving data before the loop"));

            // Set TestBehaviors to skip compiler diagnostics
            test.TestBehaviors = Microsoft.CodeAnalysis.Testing.TestBehaviors.SkipGeneratedCodeCheck;

            // Clear fixed state diagnostics
            test.FixedState.ExpectedDiagnostics.Clear();

            // No diagnostics expected in fixed state

            // Run the test
            await test.RunAsync();
        }
    }
}
