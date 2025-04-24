using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class EventRegistrationAnalyzerTests
    {
        [TestMethod]
        public async Task NonStaticEventDelegateIsAllowed()
        {
            // Arrange
            var source = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace TestContract
{
    public class Contract : SmartContract.Framework.SmartContract
    {
        // Non-static event delegate
        public delegate void Transfer(string from, string to, int amount);

        public static void Main()
        {
            // Trigger the event
            OnTransfer(""sender"", ""receiver"", 100);
        }

        public static void OnTransfer(string from, string to, int amount)
        {
            Transfer(from, to, amount);
        }
    }
}";

            // Act
            var compilation = await TestHelper.CreateCompilationAsync(source);
            var analyzer = new EventRegistrationAnalyzer();
            var diagnostics = await compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer)).GetAnalyzerDiagnosticsAsync();

            // Assert
            // There should be no diagnostics for the non-static event delegate
            Assert.AreEqual(0, diagnostics.Count(d => d.Id == EventRegistrationAnalyzer.DiagnosticIdImproperEventDefinition));
        }

        [TestMethod]
        public async Task NonStaticEventFieldIsAllowed()
        {
            // Arrange
            var source = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace TestContract
{
    public delegate void Transfer(string from, string to, int amount);

    public class Contract : SmartContract.Framework.SmartContract
    {
        // Non-static event field
        public Transfer TransferEvent;

        public static void Main()
        {
            // Trigger the event
            OnTransfer(""sender"", ""receiver"", 100);
        }

        public static void OnTransfer(string from, string to, int amount)
        {
            TransferEvent(from, to, amount);
        }
    }
}";

            // Act
            var compilation = await TestHelper.CreateCompilationAsync(source);
            var analyzer = new EventRegistrationAnalyzer();
            var diagnostics = await compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer)).GetAnalyzerDiagnosticsAsync();

            // Assert
            // There should be no diagnostics for the non-static event field
            Assert.AreEqual(0, diagnostics.Count(d => d.Id == EventRegistrationAnalyzer.DiagnosticIdImproperEventDefinition));
        }
    }
}
