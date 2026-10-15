// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Coverage.Formats;
using Neo.VM;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Neo.SmartContract.Testing.UnitTests.Coverage;

[TestClass]
public class CoberturaSourceTests
{
    [TestMethod]
    public void BranchesBetweenSequencePointsBelongOnlyToTheirSourceLine()
    {
        var coverage = CreateCoverage();
        var points = new[] { Point(0, 0, 10), Point(4, 0, 11) };
        NeoDebugInfo.Method method = new("0", "Contract", "run", (0, 7), [], points);
        CollectionAssert.AreEqual(new[] { 1 }, CoberturaFormat.GetBranchInstructions(coverage, method, points[0]).Select(b => b.address).ToArray());
        CollectionAssert.AreEqual(new[] { 5 }, CoberturaFormat.GetBranchInstructions(coverage, method, points[1]).Select(b => b.address).ToArray());
    }

    [TestMethod]
    public void BranchAtNextSequencePointIsNotAttributedTwice()
    {
        var coverage = CreateCoverage();
        var points = new[] { Point(0, 0, 10), Point(1, 0, 11) };
        NeoDebugInfo.Method method = new("0", "Contract", "run", (0, 7), [], points);
        Assert.AreEqual(0, CoberturaFormat.GetBranchInstructions(coverage, method, points[0]).Count());
        Assert.AreEqual(0, CoberturaFormat.GetLineLastAddress(coverage.Lines.ToArray(), method, 0));
        CollectionAssert.AreEqual(new[] { 1, 5 }, CoberturaFormat.GetBranchInstructions(coverage, method, points[1]).Select(b => b.address).ToArray());
    }

    [TestMethod]
    [DataRow(0, "0% (0/2)", "0%")]
    [DataRow(1, "50% (1/2)", "50%")]
    [DataRow(2, "100% (2/2)", "100%")]
    public void ReportCountsActualBranchPaths(int paths, string expected, string condition)
    {
        var coverage = CreateCoverage();
        for (var i = 0; i < paths; i++) coverage.Hit(1, new Script(coverage.ContractState!.Script).GetInstruction(1), 1, i == 0);
        NeoDebugInfo.Method method = new("0", "Contract", "run", (0, 3), [], [Point(0, 0, 10)]);
        var report = Report(coverage, new(UInt160.Zero, "", ["Contract.cs"], [method]));
        Assert.AreEqual("2", (string?)report.Root!.Attribute("branches-valid"));
        Assert.AreEqual(paths.ToString(CultureInfo.InvariantCulture), (string?)report.Root.Attribute("branches-covered"));
        var line = report.Descendants("class").Single().Element("lines")!.Element("line")!;
        Assert.AreEqual("true", (string?)line.Attribute("branch"));
        Assert.AreEqual(expected, (string?)line.Attribute("condition-coverage"));
        Assert.AreEqual(condition, (string?)line.Descendants("condition").Single().Attribute("coverage"));
    }

    [TestMethod]
    public void GroupsMethodsByClassAndFiltersEachDocument()
    {
        var coverage = CreateCoverage();
        NeoDebugInfo.Method first = new("0", "Contract", "first", (0, 3), [], [Point(0, 0, 10), Point(3, 1, 30)]);
        NeoDebugInfo.Method second = new("1", "Contract", "second", (4, 7), [], [Point(4, 0, 20)]);
        var report = Report(coverage, new(UInt160.Zero, "", ["Contract.cs", "Other.cs"], [first, second]));
        var classes = report.Descendants("class").ToArray();
        Assert.AreEqual(2, classes.Length);
        var main = classes.Single(c => (string?)c.Attribute("filename") == "Contract.cs");
        Assert.AreEqual(2, main.Element("methods")!.Elements("method").Count());
        CollectionAssert.AreEqual(new[] { "10", "20" }, main.Element("lines")!.Elements("line").Select(l => (string)l.Attribute("number")!).ToArray());
        var other = classes.Single(c => (string?)c.Attribute("filename") == "Other.cs");
        CollectionAssert.AreEqual(new[] { "30" }, other.Descendants("line").Select(l => (string)l.Attribute("number")!).Distinct().ToArray());
    }

    [TestMethod]
    public void DuplicateSourceLinesAndUnmappedPointsDoNotInflateTotals()
    {
        var coverage = CreateCoverage();
        coverage.Hit(1, new Script(coverage.ContractState!.Script).GetInstruction(1), 1, true);
        NeoDebugInfo.Method method = new("0", "Contract", "run", (0, 7), [],
            [Point(0, 0, 10), Point(1, 0, 10), Point(4, 0, 20), Point(7, -1, 30)]);
        var report = Report(coverage, new(UInt160.Zero, "", ["Contract.cs"], [method]));
        Assert.AreEqual("2", (string?)report.Root!.Attribute("lines-valid"));
        Assert.AreEqual("1", (string?)report.Root.Attribute("lines-covered"));
        Assert.AreEqual(2, report.Descendants("class").Single().Element("lines")!.Elements("line").Count());
    }

    [TestMethod]
    public void NumericAttributesAreCultureIndependent()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            var coverage = CreateCoverage();
            coverage.Hit(0, new Script(coverage.ContractState!.Script).GetInstruction(0), 1, null);
            NeoDebugInfo.Method method = new("0", "Contract", "run", (0, 7), [], [Point(0, 0, 10), Point(4, 0, 20)]);
            var report = Report(coverage, new(UInt160.Zero, "", ["Contract.cs"], [method]));
            foreach (var element in report.Descendants().Where(e => e.Attribute("line-rate") is not null))
                Assert.AreEqual("0.5000", (string?)element.Attribute("line-rate"));
        }
        finally { CultureInfo.CurrentCulture = previous; }
    }

    private static NeoDebugInfo.SequencePoint Point(int address, int document, int line) => new(address, document, (line, 1), (line, 5));

    private static CoveredContract CreateCoverage()
    {
        var state = new ContractState
        {
            Hash = UInt160.Zero,
            Nef = new NefFile { Compiler = "test", Source = "", Script = new byte[] { (byte)OpCode.NOP, (byte)OpCode.JMPIF, 2, (byte)OpCode.RET, (byte)OpCode.NOP, (byte)OpCode.JMPIF, 2, (byte)OpCode.RET }, Tokens = [] },
            Manifest = new ContractManifest { Name = "Contract", Groups = [], SupportedStandards = [], Permissions = [], Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(), Abi = new ContractAbi { Methods = [], Events = [] } }
        };
        return new CoveredContract(MethodDetectionMechanism.NextMethod, UInt160.Zero, state);
    }

    private static XDocument Report(CoveredContract coverage, NeoDebugInfo info)
    {
        using var stream = new MemoryStream();
        new CoberturaFormat((coverage, info)).WriteReport((_, write) => write(stream));
        stream.Position = 0;
        return XDocument.Load(stream);
    }
}
