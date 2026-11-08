// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Coverage.Formats;
using Neo.VM;
using System;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;

namespace Neo.SmartContract.Testing.UnitTests.Coverage;

[TestClass]
public class CoverletBranchTests
{
    [TestMethod]
    [DataRow(0, 0)]
    [DataRow(3, 0)]
    [DataRow(3, 2)]
    public void ReportsBothPathsWithExecutionCounts(int trueHits, int falseHits)
    {
        var coverage = CreateCoverage();
        Hit(coverage, 1, trueHits, falseHits);
        var branches = Branches(Report(coverage, Info([Point(1, 0, 10)], 3)));
        Assert.HasCount(2, branches);
        CollectionAssert.AreEqual(new[] { 0, 1 }, branches.Select(b => (int)b!["Path"]!).ToArray());
        CollectionAssert.AreEqual(new[] { falseHits, trueHits }, branches.Select(b => (int)b!["Hits"]!).ToArray());
        Assert.AreEqual(trueHits > 0 ? (falseHits > 0 ? 2 : 1) : 0, branches.Count(b => (int)b!["Hits"]! > 0));
    }

    [TestMethod]
    public void NonBranchingSourceLinesKeepTheirHitCounts()
    {
        var coverage = CreateCoverage([(byte)OpCode.NOP, (byte)OpCode.RET]);
        var instruction = new Script(coverage.ContractState!.Script).GetInstruction(0);
        for (int i = 0; i < 3; i++) coverage.Hit(0, instruction, 1, null);
        var report = Report(coverage, Info([Point(0, 0, 10)], 1));
        Assert.HasCount(0, Branches(report));
        Assert.AreEqual(3, (int)report.AsObject().First().Value!["Contract.cs"]!["Contract"]!["run"]!["Lines"]!["10"]!);
    }

    [TestMethod]
    public void BranchesInsideUnsortedSourceIntervalsAreAssignedOnce()
    {
        var report = Report(CreateCoverage(), Info([Point(4, 0, 11), Point(0, 0, 10)]));
        var branches = Branches(report);
        Assert.HasCount(4, branches);
        CollectionAssert.AreEqual(new[] { 1, 1, 5, 5 }, branches.Select(b => (int)b!["Offset"]!).ToArray());
        CollectionAssert.AreEqual(new[] { 10, 10, 11, 11 }, branches.Select(b => (int)b!["Line"]!).ToArray());
        CollectionAssert.AreEqual(new[] { 0, 1, 2, 3 }, branches.Select(b => (int)b!["Ordinal"]!).ToArray());
    }

    [TestMethod]
    public void NextPointAndMethodEndBoundBranchAttribution()
    {
        var branches = Branches(Report(CreateCoverage(), Info([Point(0, 0, 10), Point(1, 0, 11)], 3)));
        Assert.HasCount(2, branches);
        Assert.IsTrue(branches.All(b => (int)b!["Offset"]! == 1 && (int)b["Line"]! == 11));
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(1)]
    public void UnmappedPointsBoundThePreviousSourceInterval(int document)
    {
        var branches = Branches(Report(CreateCoverage(), Info([Point(0, 0, 10), Point(4, document, 11)])));
        Assert.HasCount(2, branches);
        Assert.IsTrue(branches.All(b => (int)b!["Offset"]! == 1));
    }

    [TestMethod]
    [DataRow(OpCode.JMPIF)]
    [DataRow(OpCode.JMPIFNOT)]
    [DataRow(OpCode.JMPEQ)]
    [DataRow(OpCode.JMPNE)]
    [DataRow(OpCode.JMPGT)]
    [DataRow(OpCode.JMPGE)]
    [DataRow(OpCode.JMPLT)]
    [DataRow(OpCode.JMPLE)]
    [DataRow(OpCode.JMPIF_L)]
    [DataRow(OpCode.JMPIFNOT_L)]
    [DataRow(OpCode.JMPEQ_L)]
    [DataRow(OpCode.JMPNE_L)]
    [DataRow(OpCode.JMPGT_L)]
    [DataRow(OpCode.JMPGE_L)]
    [DataRow(OpCode.JMPLT_L)]
    [DataRow(OpCode.JMPLE_L)]
    public void BranchOffsetsIdentifyPredicatePaths(OpCode opcode)
    {
        bool longJump = ((byte)opcode & 1) != 0;
        byte[] script = longJump
            ? [(byte)opcode, 6, 0, 0, 0, (byte)OpCode.NOP, (byte)OpCode.RET]
            : [(byte)opcode, 3, (byte)OpCode.NOP, (byte)OpCode.RET];
        var branches = Branches(Report(CreateCoverage(script), Info([Point(0, 0, 10)], script.Length - 1)));
        Assert.HasCount(2, branches);
        bool inverted = opcode is OpCode.JMPIFNOT or OpCode.JMPIFNOT_L or OpCode.JMPNE or OpCode.JMPNE_L;
        var fallThrough = longJump ? 5 : 2;
        var target = script.Length - 1;
        CollectionAssert.AreEqual(new[] { inverted ? target : fallThrough, inverted ? fallThrough : target },
            branches.Select(b => (int)b!["EndOffset"]!).ToArray());
    }

    [TestMethod]
    public void MergingAddsHitsWithoutCollapsingBranchesOnTheSameLine()
    {
        var first = CreateCoverage();
        var second = CreateCoverage();
        Hit(first, 1, 3, 0);
        Hit(second, 1, 0, 2);
        Hit(second, 5, 4, 0);
        var info = Info([Point(0, 0, 10)]);
        var path = Path.GetTempFileName();
        try
        {
            new CoverletJsonFormat((first, info)).Write(path, false);
            new CoverletJsonFormat((second, info)).Write(path, true);
            var branches = Branches(JsonNode.Parse(File.ReadAllText(path))!);
            Assert.HasCount(4, branches);
            CollectionAssert.AreEqual(new[] { 2, 3, 0, 4 }, branches.Select(b => (int)b!["Hits"]!).ToArray());
        }
        finally { File.Delete(path); }
    }

    private static NeoDebugInfo.SequencePoint Point(int address, int document, int line) => new(address, document, (line, 1), (line, 5));
    private static NeoDebugInfo Info(NeoDebugInfo.SequencePoint[] points, int end = 7) =>
        new(UInt160.Zero, "", ["Contract.cs"], [new("0", "Contract", "run", (0, end), [], points)]);

    private static void Hit(CoveredContract coverage, int address, int trueHits, int falseHits)
    {
        var instruction = new Script(coverage.ContractState!.Script).GetInstruction(address);
        for (int i = 0; i < trueHits; i++) coverage.Hit(address, instruction, 1, true);
        for (int i = 0; i < falseHits; i++) coverage.Hit(address, instruction, 1, false);
    }

    private static CoveredContract CreateCoverage(byte[]? script = null)
    {
        var state = new ContractState
        {
            Hash = UInt160.Zero,
            Nef = new NefFile { Compiler = "test", Source = "", Script = script ?? [(byte)OpCode.NOP, (byte)OpCode.JMPIF, 2, (byte)OpCode.RET, (byte)OpCode.NOP, (byte)OpCode.JMPIF, 2, (byte)OpCode.RET], Tokens = [] },
            Manifest = new ContractManifest { Name = "Contract", Groups = [], SupportedStandards = [], Permissions = [], Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(), Abi = new ContractAbi { Methods = [], Events = [] } }
        };
        return new CoveredContract(MethodDetectionMechanism.NextMethod, UInt160.Zero, state);
    }

    private static JsonNode Report(CoveredContract coverage, NeoDebugInfo info) => JsonNode.Parse(new CoverletJsonFormat((coverage, info)).Dump())!;
    private static JsonArray Branches(JsonNode report) => report.AsObject().First().Value!["Contract.cs"]!["Contract"]!["run"]!["Branches"]!.AsArray();
}
