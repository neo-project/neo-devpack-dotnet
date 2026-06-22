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
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.NotifyEventNameAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class NotifyEventNameAnalyzerUnitTest
    {
        // Minimal stand-ins for the framework Runtime.Notify and a DisplayName attribute, so the
        // analyzer can bind Notify to Neo.SmartContract.Framework.Services.Runtime. Appended after
        // the test contract so the contract's using directives stay first.
        private const string FrameworkStubs = @"
namespace Neo.SmartContract.Framework.Services
{
    public static class Runtime
    {
        public static void Notify(string eventName, params object[] state) { }
    }
}

namespace System.ComponentModel
{
    public sealed class DisplayNameAttribute : System.Attribute
    {
        public DisplayNameAttribute(string displayName) { }
    }
}
";

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
        public async Task NotifyOnUnrelatedType_ShouldNotReportDiagnostic()
        {
            // A method merely named "Notify" on a user type is not the framework Runtime.Notify
            // and must not be flagged.
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
}" + FrameworkStubs;

            await VerifyCS.VerifyAnalyzerAsync(test);
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

        [TestMethod]
        public async Task Notify_WithMatchingDisplayName_ShouldNotReportDiagnostic()
        {
            var test = @"
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

class Contract
{
    [DisplayName(""Transfer"")]
    public static event System.Action<int> OnTransfer;

    public void Main() => Runtime.Notify(""Transfer"", 1);
}" + FrameworkStubs;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Notify_WithEventFieldNameFallback_ShouldNotReportDiagnostic()
        {
            // The event has no [DisplayName], so its name is the field name. The old analyzer
            // only matched events carrying [DisplayName] and wrongly flagged this case.
            var test = @"
using Neo.SmartContract.Framework.Services;

class Contract
{
    public static event System.Action<int> Transfer;

    public void Main() => Runtime.Notify(""Transfer"", 1);
}" + FrameworkStubs;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Notify_WithInheritedEvent_ShouldNotReportDiagnostic()
        {
            // The event is declared in a base class; the old analyzer only inspected the
            // containing type and wrongly flagged this case.
            var test = @"
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

class TokenBase
{
    [DisplayName(""Transfer"")]
    public static event System.Action<int> OnTransfer;
}

class Contract : TokenBase
{
    public void Main() => Runtime.Notify(""Transfer"", 1);
}" + FrameworkStubs;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Notify_WithUnknownEventName_ShouldReportDiagnostic()
        {
            var test = @"
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

class Contract
{
    [DisplayName(""Transfer"")]
    public static event System.Action<int> OnTransfer;

    public void Main() => Runtime.Notify({|#0:""Tranfser""|}, 1);
}" + FrameworkStubs;

            var expected = VerifyCS.Diagnostic(NotifyEventNameAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("Tranfser");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task Notify_WithCustomDisplayNameAttribute_ShouldReportDiagnostic()
        {
            var test = @"
using Neo.SmartContract.Framework.Services;
using CustomAttributes;

namespace CustomAttributes
{
    public sealed class DisplayNameAttribute : System.Attribute
    {
        public DisplayNameAttribute(string displayName) { }
    }
}

class Contract
{
    [DisplayName(""Transfer"")]
    public static event System.Action<int> OnTransfer;

    public void Main() => Runtime.Notify({|#0:""Transfer""|}, 1);
}" + FrameworkStubs;

            var expected = VerifyCS.Diagnostic(NotifyEventNameAnalyzer.DiagnosticId)
                .WithLocation(0)
                .WithArguments("Transfer");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
