// Copyright (C) 2015-2026 The Neo Project.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Text.Json;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
[DoNotParallelize]
public class UnitTest_Sarif
{
    [TestMethod]
    public void CommandLineWritesAnalyzerAndCompilerDiagnostics()
    {
        var directory = Path.Combine(Path.GetTempPath(), "neo-sarif-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var source = Path.Combine(directory, "Contract.cs");
            var report = Path.Combine(directory, "diagnostics.sarif");
            File.WriteAllText(source, "#warning Test warning\nusing Neo.SmartContract.Framework; public class Contract : SmartContract { public static double Read() => missing; }");
            Assert.AreEqual(1, Program.Main([source, "--sarif", report, "--diagnostics"]));
            Assert.IsTrue(File.Exists(report), "The compiler must write the requested SARIF report when compilation fails.");
            using var json = JsonDocument.Parse(File.ReadAllText(report));
            Assert.AreEqual("2.1.0", json.RootElement.GetProperty("version").GetString());
            var results = json.RootElement.GetProperty("runs")[0].GetProperty("results").EnumerateArray().ToArray();
            Assert.IsTrue(results.Any(result => result.GetProperty("ruleId").GetString() == "NC4004"));
            Assert.IsTrue(results.Any(result => result.GetProperty("ruleId").GetString() == "CS0103"));
            Assert.IsTrue(results.Any(result => result.GetProperty("ruleId").GetString() == "CS1030"));
            foreach (var result in results)
            {
                var isWarning = result.GetProperty("ruleId").GetString() == "CS1030";
                Assert.AreEqual(isWarning ? "warning" : "error", result.GetProperty("level").GetString());
                var location = result.GetProperty("locations")[0].GetProperty("physicalLocation");
                Assert.AreEqual(new Uri(source).AbsoluteUri, location.GetProperty("artifactLocation").GetProperty("uri").GetString());
                Assert.AreEqual(isWarning ? 1 : 2, location.GetProperty("region").GetProperty("startLine").GetInt32());
                Assert.IsTrue(location.GetProperty("region").GetProperty("startColumn").GetInt32() > 0);
            }
        }
        finally { Directory.Delete(directory, recursive: true); }
    }
    [TestMethod]
    public void SerializationSortsDeduplicatesAndPreservesDiagnosticLevels()
    {
        var tree = CSharpSyntaxTree.ParseText("first\nsecond", path: Path.Combine(Path.GetTempPath(), "sarif source.cs"));
        var warning = Diagnostic.Create(new DiagnosticDescriptor("NC9992", "Warning", "Value {0}", "Test", DiagnosticSeverity.Warning, true), Location.Create(tree, new TextSpan(6, 6)), "\"quoted\"");
        var error = Diagnostic.Create(new DiagnosticDescriptor("NC9991", "Error", "Error", "Test", DiagnosticSeverity.Error, true, helpLinkUri: "https://example.org/rule"), Location.Create(tree, new TextSpan(0, 5)));
        var note = Diagnostic.Create(new DiagnosticDescriptor("NC9993", "Note", "Note", "Test", DiagnosticSeverity.Info, true), Location.None);
        var culture = CultureInfo.CurrentCulture;
        byte[] first;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
            first = SarifDiagnosticWriter.Serialize([warning, error, warning, note]);
        }
        finally { CultureInfo.CurrentCulture = culture; }
        CollectionAssert.AreEqual(first, SarifDiagnosticWriter.Serialize([note, error, warning]));
        using var json = JsonDocument.Parse(first);
        var run = json.RootElement.GetProperty("runs")[0];
        Assert.AreEqual("utf16CodeUnits", run.GetProperty("columnKind").GetString());
        var results = run.GetProperty("results").EnumerateArray().ToArray();
        Assert.AreEqual(3, results.Length);
        var information = results.Single(result => result.GetProperty("ruleId").GetString() == "NC9993");
        Assert.AreEqual("note", information.GetProperty("level").GetString());
        Assert.IsFalse(information.TryGetProperty("locations", out _));
        var actualWarning = results.Single(result => result.GetProperty("ruleId").GetString() == "NC9992");
        Assert.AreEqual("warning", actualWarning.GetProperty("level").GetString());
        Assert.AreEqual("Value \"quoted\"", actualWarning.GetProperty("message").GetProperty("text").GetString());
        var region = actualWarning.GetProperty("locations")[0].GetProperty("physicalLocation").GetProperty("region");
        Assert.AreEqual(2, region.GetProperty("startLine").GetInt32());
        Assert.AreEqual(1, region.GetProperty("startColumn").GetInt32());
        Assert.AreEqual(7, region.GetProperty("endColumn").GetInt32());
        Assert.AreEqual("https://example.org/rule", run.GetProperty("tool").GetProperty("driver").GetProperty("rules")[0].GetProperty("helpUri").GetString());
    }

    [TestMethod]
    public void SerializationHonorsLineMappingsAndZeroLengthLocations()
    {
        var path = Path.Combine(Path.GetTempPath(), "contract source.cs");
        var tree = CSharpSyntaxTree.ParseText("#line 42 \"mapped source.cs\"\nclass Contract {}", path: path);
        var diagnostic = Diagnostic.Create(new DiagnosticDescriptor("NC9990", "Test", "Test", "Test", DiagnosticSeverity.Error, true), Location.Create(tree, new TextSpan(tree.GetText().ToString().IndexOf("class", StringComparison.Ordinal), 0)));
        using var json = JsonDocument.Parse(SarifDiagnosticWriter.Serialize([diagnostic]));
        var location = json.RootElement.GetProperty("runs")[0].GetProperty("results")[0].GetProperty("locations")[0].GetProperty("physicalLocation");
        Assert.AreEqual(new Uri(Path.Combine(Path.GetTempPath(), "mapped source.cs")).AbsoluteUri, location.GetProperty("artifactLocation").GetProperty("uri").GetString());
        var region = location.GetProperty("region");
        Assert.AreEqual(42, region.GetProperty("startLine").GetInt32());
        Assert.IsFalse(region.TryGetProperty("endColumn", out _));
    }

    [TestMethod]
    [DataRow("Contract.cs")]
    [DataRow("src/Contract.cs")]
    [DataRow("src/nested/Contract.cs")]
    public void SerializationResolvesUnmappedRelativePathsFromWorkingDirectory(string sourcePath)
    {
        var tree = CSharpSyntaxTree.ParseText("class Contract {}", path: sourcePath);
        var diagnostic = Diagnostic.Create(new DiagnosticDescriptor("NC9990", "Test", "Test", "Test", DiagnosticSeverity.Error, true), Location.Create(tree, new TextSpan(0, 5)));
        using var json = JsonDocument.Parse(SarifDiagnosticWriter.Serialize([diagnostic]));
        var location = json.RootElement.GetProperty("runs")[0].GetProperty("results")[0].GetProperty("locations")[0].GetProperty("physicalLocation");
        Assert.AreEqual(new Uri(Path.GetFullPath(sourcePath)).AbsoluteUri, location.GetProperty("artifactLocation").GetProperty("uri").GetString());
    }

    [TestMethod]
    [DataRow("mapped.cs")]
    [DataRow("src/Contract.cs")]
    public void SerializationResolvesMappedPathsFromRelativeSourceDirectory(string mappedPath)
    {
        var sourcePath = Path.Combine("src", "Contract.cs");
        var tree = CSharpSyntaxTree.ParseText($"#line 42 \"{mappedPath}\"\nclass Contract {{}}", path: sourcePath);
        var diagnostic = Diagnostic.Create(new DiagnosticDescriptor("NC9990", "Test", "Test", "Test", DiagnosticSeverity.Error, true), Location.Create(tree, new TextSpan(tree.GetText().ToString().IndexOf("class", StringComparison.Ordinal), 5)));
        using var json = JsonDocument.Parse(SarifDiagnosticWriter.Serialize([diagnostic]));
        var location = json.RootElement.GetProperty("runs")[0].GetProperty("results")[0].GetProperty("locations")[0].GetProperty("physicalLocation");
        Assert.AreEqual(new Uri(Path.GetFullPath(Path.Combine("src", mappedPath))).AbsoluteUri, location.GetProperty("artifactLocation").GetProperty("uri").GetString());
        Assert.AreEqual(42, location.GetProperty("region").GetProperty("startLine").GetInt32());
    }

    [TestMethod]
    public void SuccessfulCompilationWritesAnEmptyReportAndReadbackIsIndependent()
    {
        var directory = Path.Combine(Path.GetTempPath(), "neo-sarif-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var source = Path.Combine(directory, "Contract.cs");
            var report = Path.Combine(directory, "diagnostics.sarif");
            File.WriteAllText(source, "using Neo.SmartContract.Framework; public class Contract : SmartContract { public static int Read() => 1; }");
            Assert.AreEqual(0, Program.Main([source, "--sarif", report, "--debug", "None"]));
            using var json = JsonDocument.Parse(File.ReadAllText(report));
            Assert.AreEqual(0, json.RootElement.GetProperty("runs")[0].GetProperty("results").GetArrayLength());
            Assert.IsTrue(File.Exists(Path.Combine(directory, "bin", "sc", "Contract.nef")));
            // Reusing Main must replace the previous report rather than append results.
            File.WriteAllText(source, "using Neo.SmartContract.Framework; public class Contract : SmartContract { public static int Read() => missing; }");
            Assert.AreEqual(1, Program.Main([source, "--sarif", report]));
            using var failed = JsonDocument.Parse(File.ReadAllText(report));
            Assert.AreEqual(1, failed.RootElement.GetProperty("runs")[0].GetProperty("results").GetArrayLength());
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    [TestMethod]
    public void ReportWriteFailuresReturnNonzero()
    {
        var directory = Path.Combine(Path.GetTempPath(), "neo-sarif-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var source = Path.Combine(directory, "Contract.cs");
            File.WriteAllText(source, "using Neo.SmartContract.Framework; public class Contract : SmartContract { public static int Read() => 1; }");
            Assert.AreEqual(1, Program.Main([source, "--sarif", directory, "--debug", "None"]));
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

}
