// Copyright (C) 2015-2026 The Neo Project.
//
// BigIntegerUsageAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<Neo.SmartContract.Analyzer.BigIntegerUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.Test
{

    [TestClass]
    public class BigIntegerUsageAnalyzerUnitTests
    {
        [TestMethod]
        public async Task SupportedBigIntegerMethod_ShouldNotReportDiagnostic()
        {
            var test = """

                       using System;
                       using System.Numerics;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               BigInteger x = 42;
                               BigInteger y = 24;
                               _ = BigInteger.Pow(x, 2);
                               _ = BigInteger.ModPow(x, y, 17);
                               _ = BigInteger.Add(x, y);
                               _ = BigInteger.Subtract(x, y);
                               _ = BigInteger.Negate(x);
                               _ = BigInteger.Multiply(x, y);
                               _ = BigInteger.Divide(x, y);
                               _ = BigInteger.Remainder(x, y);
                               _ = BigInteger.Compare(x, y);
                               _ = BigInteger.GreatestCommonDivisor(x, y);
                               _ = BigInteger.Abs(x);
                               _ = BigInteger.Max(x, y);
                               _ = BigInteger.Min(x, y);
                               _ = BigInteger.Parse("42");
                               _ = BigInteger.TryParse("42", out var parsed);
                               _ = x.ToByteArray();
                               _ = x.ToString();
                               _ = x.Equals(y);
                               _ = x.Equals(1L);
                               _ = x.Equals(1UL);
                               _ = x.Equals((object)y);
                               Func<BigInteger, BigInteger, BigInteger> add = BigInteger.Add;
                           }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task UnsupportedBigIntegerOverloads_ShouldReportOneDiagnosticPerUsage()
        {
            var test = """
                       using System;
                       using System.Numerics;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               BigInteger value = 42;
                               _ = {|#0:BigInteger.Log10(value)|};
                               _ = {|#1:value.ToByteArray(true, true)|};
                               _ = {|#2:value.ToString("X")|};
                               Func<BigInteger, double> log = {|#3:BigInteger.Log10|};
                           }
                       }
                       """;

            var expected = new[]
            {
                VerifyCS.Diagnostic(BigIntegerUsageAnalyzer.DiagnosticId).WithLocation(0).WithArguments("Log10"),
                VerifyCS.Diagnostic(BigIntegerUsageAnalyzer.DiagnosticId).WithLocation(1).WithArguments("ToByteArray"),
                VerifyCS.Diagnostic(BigIntegerUsageAnalyzer.DiagnosticId).WithLocation(2).WithArguments("ToString"),
                VerifyCS.Diagnostic(BigIntegerUsageAnalyzer.DiagnosticId).WithLocation(3).WithArguments("Log10")
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
