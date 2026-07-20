using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class NeoAnalyzerSuiteTests
{
    [TestMethod]
    public void SuiteContainsEveryConcreteAnalyzerExactlyOnce()
    {
        var expected = typeof(NeoAnalyzerSuite).Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && typeof(DiagnosticAnalyzer).IsAssignableFrom(type))
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToArray();
        var actual = NeoAnalyzerSuite.Create()
            .Select(analyzer => analyzer.GetType())
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToArray();

        CollectionAssert.AreEqual(expected, actual);
        Assert.AreEqual(actual.Length, actual.Distinct().Count());
    }
}
