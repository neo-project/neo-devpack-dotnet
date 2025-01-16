// Copyright (C) 2015-2024 The Neo Project.
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
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Neo.SmartContract.Analyzer.BigIntegerUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.Test
{

    [TestClass]
    public class BigIntegerUsageAnalyzerUnitTests
    {
        [TestMethod]
        public async Task SupportedBigIntegerMethod_ShouldNotReportDiagnostic()
        {
            var test = """

                       using System.Numerics;

                       class TestClass
                       {
                           public void TestMethod()
                           {
                               BigInteger x = 42;
                               BigInteger y = 24;
                               BigInteger z = BigInteger.Add(x, y);
                           }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
