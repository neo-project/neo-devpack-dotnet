// Copyright (C) 2015-2025 The Neo Project.
//
// EventRegistrationAnalyzerUnitTest.cs file belongs to the neo project and is free
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

using VerifyCS = Neo.SmartContract.Analyzer.UnitTests.TestHelper;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class EventRegistrationAnalyzerUnitTest
    {
        [TestMethod]
        public async Task EventRegistrationAnalyzer_UnregisteredEvent_ShouldReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;
                                      using Neo.SmartContract.Framework.Services;

                                      public class TestContract : SmartContract
                                      {
                                          public static void Main()
                                          {
                                              // Using an unregistered event
                                              Runtime.Notify(MyEvent, "data");
                                          }
                                      }
                                      """;

            var test = VerifyCS.CreateAnalyzerTest<EventRegistrationAnalyzer>();
            test.TestCode = sourceCode;
            test.ExpectedDiagnostics.Add(new DiagnosticResult(EventRegistrationAnalyzer.DiagnosticIdUnregisteredEvent, DiagnosticSeverity.Error)
                .WithSpan(9, 31, 9, 38)
                .WithArguments("MyEvent"));

            await test.RunAsync();
        }

        [TestMethod]
        public async Task EventRegistrationAnalyzer_RegisteredEvent_ShouldNotReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;
                                      using Neo.SmartContract.Framework.Services;

                                      public class TestContract : SmartContract
                                      {
                                          // Properly registered event
                                          public static event Action<string> MyEvent;

                                          public static void Main()
                                          {
                                              // Using a registered event
                                              Runtime.Notify(MyEvent, "data");
                                          }
                                      }
                                      """;

            var test = VerifyCS.CreateAnalyzerTest<EventRegistrationAnalyzer>();
            test.TestCode = sourceCode;
            // No diagnostics expected

            await test.RunAsync();
        }

        [TestMethod]
        public async Task EventRegistrationAnalyzer_NonStaticEvent_ShouldNotReportDiagnostic()
        {
            const string sourceCode = """
                                      using Neo.SmartContract.Framework;
                                      using Neo.SmartContract.Framework.Services;
                                      using System;

                                      public class TestContract : SmartContract
                                      {
                                          // Non-static event (now allowed)
                                          public delegate void MyEventDelegate(string data);
                                          public MyEventDelegate MyEvent;

                                          public static void Main()
                                          {
                                              // Using the event
                                              Runtime.Notify(MyEvent, "data");
                                          }
                                      }
                                      """;

            var test = VerifyCS.CreateAnalyzerTest<EventRegistrationAnalyzer>();
            test.TestCode = sourceCode;
            // Only expect the unregistered event diagnostic, not the non-static warning
            test.ExpectedDiagnostics.Add(new DiagnosticResult(EventRegistrationAnalyzer.DiagnosticIdUnregisteredEvent, DiagnosticSeverity.Error)
                .WithSpan(13, 31, 13, 38)
                .WithArguments("MyEvent"));

            await test.RunAsync();
        }

        [TestMethod]
        public async Task EventRegistrationAnalyzer_FixUnregisteredEvent()
        {
            const string originalCode = """
                                        using Neo.SmartContract.Framework;
                                        using Neo.SmartContract.Framework.Services;

                                        public class TestContract : SmartContract
                                        {
                                            public static void Main()
                                            {
                                                // Using an unregistered event
                                                Runtime.Notify(MyEvent, "data");
                                            }
                                        }
                                        """;

            const string fixedCode = """
                                     using Neo.SmartContract.Framework;
                                     using Neo.SmartContract.Framework.Services;

                                     public class TestContract : SmartContract
                                     {
                                         public delegate void MyEvent(string name);

                                         public static void Main()
                                         {
                                             // Using an unregistered event
                                             Runtime.Notify(MyEvent, "data");
                                         }
                                     }
                                     """;

            var test = VerifyCS.CreateCodeFixTest<EventRegistrationAnalyzer, EventRegistrationCodeFixProvider>();
            test.TestCode = originalCode;
            test.FixedCode = fixedCode;
            test.ExpectedDiagnostics.Add(new DiagnosticResult(EventRegistrationAnalyzer.DiagnosticIdUnregisteredEvent, DiagnosticSeverity.Error)
                .WithSpan(9, 31, 9, 38)
                .WithArguments("MyEvent"));

            await test.RunAsync();
        }


    }
}
