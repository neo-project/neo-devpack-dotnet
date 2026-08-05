// Copyright (C) 2015-2026 The Neo Project.
//
// BigIntegerCreationAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.CodeFixVerifier<
    Neo.SmartContract.Analyzer.BigIntegerCreationAnalyzer,
    Neo.SmartContract.Analyzer.BigIntegerCreationCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.Test
{
    [TestClass]
    public class BigIntegerCreationAnalyzerUnitTests
    {
        [TestMethod]
        public async Task BigIntegerCreationWithInt_ShouldReportDiagnostic()
        {
            var test = @"
using System.Numerics;

class TestClass
{
    public void TestMethod()
    {
        BigInteger x = new BigInteger(42);
    }
}";

            var expectedDiagnostic = VerifyCS.Diagnostic(BigIntegerCreationAnalyzer.DiagnosticId)
                .WithLocation(8, 24)
                .WithMessage("BigInteger constructor is not supported. Only BigInteger(byte[]) is supported; use BigInteger.Zero for zero or an implicit conversion for integral values.");

            await VerifyCS.VerifyAnalyzerAsync(test, expectedDiagnostic);
        }

        [TestMethod]
        public async Task BigIntegerCreationWithInt_ShouldNotReportDiagnostic()
        {
            var test = """

                       using System.Numerics;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               BigInteger x = 42;
                           }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task BigIntegerCreationWithInt_ShouldReplaceWithExplicitConversion()
        {
            var test = """

                       using System.Numerics;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               BigInteger x = new BigInteger(42);
                           }
                       }
                       """;

            var fixtest = """

                          using System.Numerics;

                          class TestClass
                          {
                              public void TestMethod()
                              {
                                  BigInteger x = (BigInteger)(42);
                              }
                          }
                          """;

            var expectedDiagnostic = VerifyCS.Diagnostic(BigIntegerCreationAnalyzer.DiagnosticId)
                .WithLocation(8, 24)
                .WithMessage("BigInteger constructor is not supported. Only BigInteger(byte[]) is supported; use BigInteger.Zero for zero or an implicit conversion for integral values.");

            await VerifyCS.VerifyCodeFixAsync(test, expectedDiagnostic, fixtest);
        }

        [TestMethod]
        public async Task BigIntegerCreationFix_ShouldPreserveOverloadSelection()
        {
            var test = """
                       using System.Numerics;

                       class TestClass
                       {
                           private static string Pick(int value) => "int";
                           private static string Pick(BigInteger value) => "big-integer";

                           public string TestMethod() => Pick({|#0:new BigInteger(42)|});
                       }
                       """;

            var fixedSource = """
                              using System.Numerics;

                              class TestClass
                              {
                                  private static string Pick(int value) => "int";
                                  private static string Pick(BigInteger value) => "big-integer";

                                  public string TestMethod() => Pick((BigInteger)(42));
                              }
                              """;

            var expected = VerifyCS.Diagnostic(BigIntegerCreationAnalyzer.DiagnosticId)
                .WithLocation(0);

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixedSource);
        }

        [TestMethod]
        public async Task BigIntegerCreationFix_ShouldPreserveVarType()
        {
            var test = """
                       using System.Numerics;

                       class TestClass
                       {
                           private static int GetValue() => 41;

                           public void TestMethod()
                           {
                               var value = {|#0:new BigInteger(GetValue() + 1)|};
                           }
                       }
                       """;

            var fixedSource = """
                              using System.Numerics;

                              class TestClass
                              {
                                  private static int GetValue() => 41;

                                  public void TestMethod()
                                  {
                                      var value = (BigInteger)(GetValue() + 1);
                                  }
                              }
                              """;

            var expected = VerifyCS.Diagnostic(BigIntegerCreationAnalyzer.DiagnosticId)
                .WithLocation(0);

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixedSource);
        }

        [TestMethod]
        public async Task BigIntegerCreationFix_ShouldPreserveArgumentComments()
        {
            var test = """
                       using System.Numerics;

                       class TestClass
                       {
                           public BigInteger TestMethod(int value)
                           {
                               return {|#0:new BigInteger(/* keep */ value /* keep */)|};
                           }
                       }
                       """;

            var fixedSource = """
                              using System.Numerics;

                              class TestClass
                              {
                                  public BigInteger TestMethod(int value)
                                  {
                                      return (BigInteger)(/* keep */ value /* keep */);
                                  }
                              }
                              """;

            var expected = VerifyCS.Diagnostic(BigIntegerCreationAnalyzer.DiagnosticId)
                .WithLocation(0);

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixedSource);
        }

        [DataTestMethod]
        [DataRow("uint")]
        [DataRow("long")]
        [DataRow("ulong")]
        public async Task UnsupportedIntegralConstructors_ShouldReportDiagnostic(string parameterType)
        {
            var test = $$"""
                         using System.Numerics;

                         class TestClass
                         {
                             BigInteger Create({{parameterType}} value) => {|#0:new BigInteger(value)|};
                         }
                         """;

            var expected = VerifyCS.Diagnostic(BigIntegerCreationAnalyzer.DiagnosticId)
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ParameterlessConstructor_ShouldReportDiagnostic()
        {
            var test = """
                       using System.Numerics;

                       class TestClass
                       {
                           BigInteger Create() => {|#0:new BigInteger()|};
                       }
                       """;

            var expected = VerifyCS.Diagnostic(BigIntegerCreationAnalyzer.DiagnosticId)
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ByteArrayConstructorWithFlags_ShouldReportDiagnostic()
        {
            var test = """
                       using System.Numerics;

                       class TestClass
                       {
                           BigInteger Create(byte[] value) => {|#0:new BigInteger(value, true, true)|};
                       }
                       """;

            var expected = VerifyCS.Diagnostic(BigIntegerCreationAnalyzer.DiagnosticId)
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ByteArrayConstructor_ShouldNotReportDiagnostic()
        {
            var test = """
                       using System.Numerics;

                       class TestClass
                       {
                           BigInteger Create(byte[] value) => new BigInteger(value);
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
