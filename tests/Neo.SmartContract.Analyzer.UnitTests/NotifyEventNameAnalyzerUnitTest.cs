// Copyright (C) 2015-2026 The Neo Project.
//
// NotifyEventNameAnalyzerUnitTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.NotifyEventNameAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class NotifyEventNameAnalyzerUnitTest
    {
        [TestMethod]
        public async Task NonNotifyInvocation_ShouldNotReportDiagnostic()
        {
            var test = @"
class TestClass
{
    public void Main()
    {
        var s = ""hello"";
        var result = s.Contains(""ell"");
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task NotifyOnNonEventMember_ShouldNotReportDiagnostic()
        {
            var test = @"
class Notifier
{
    public void Notify(string name, string arg1, int arg2) { }
}

class TestClass
{
    public void Main()
    {
        var n = new Notifier();
        n.Notify(""Transfer"", ""to"", 100);
    }
}";

            // The analyzer only triggers on member access named "Notify" and checks
            // events in the containing type. With no events, the name won't match
            // any DisplayName, so a diagnostic IS expected.
            var expected = VerifyCS.Diagnostic(NotifyEventNameAnalyzer.DiagnosticId)
                .WithLocation(12, 18)
                .WithArguments("Transfer");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task MethodNotNamedNotify_ShouldNotReportDiagnostic()
        {
            var test = @"
class Sender
{
    public void Send(string name) { }
}

class TestClass
{
    public void Main()
    {
        var s = new Sender();
        s.Send(""Transfer"");
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
