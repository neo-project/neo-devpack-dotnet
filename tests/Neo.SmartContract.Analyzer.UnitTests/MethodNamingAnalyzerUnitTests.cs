// Copyright (C) 2015-2026 The Neo Project.
//
// MethodNamingAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Neo.SmartContract.Analyzer.SmartContractMethodNamingAnalyzer>;

namespace Neo.SmartContract.Analyzer.Test
{
    [TestClass]
    public class MethodNamingAnalyzerUnitTests
    {
        [TestMethod]
        public async Task MethodsWithSameNameAndParamCount_ShouldReportDiagnostic()
        {
            var test = """
                       using System.Numerics;
                       public class TestContract
                       {
                           public void Transfer(byte[] from, byte[] to, BigInteger amount) { }
                           public void Transfer(byte[] to, BigInteger amount) { }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task MethodsWithSameNameButDifferentParamCount_ShouldNotReportDiagnostic()
        {
            var test = """
                       using System.Numerics;
                       public class TestContract
                       {
                           public void Transfer(byte[] from, byte[] to, BigInteger amount) { }
                           public void Transfer(byte[] to) { }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task MethodsWithDifferentNames_ShouldNotReportDiagnostic()
        {
            var test = """
                       using System.Numerics;
                       public class TestContract
                       {
                           public void Transfer(byte[] from, byte[] to, BigInteger amount) { }
                           public void Withdraw(byte[] to, BigInteger amount) { }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
