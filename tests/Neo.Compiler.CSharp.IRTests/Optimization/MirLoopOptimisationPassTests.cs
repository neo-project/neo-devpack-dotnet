using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.Analysis;
using Neo.Compiler.HIR;
using Neo.Compiler.MIR;
using Neo.Compiler.MIR.Optimization;

namespace Neo.Compiler.CSharp.IRTests.Optimization;

[TestClass]
public sealed class MirLoopOptimisationPassTests
{
    private static readonly HirSignature VoidSignature = new HirSignature(System.Array.Empty<HirType>(), HirType.VoidType, System.Array.Empty<HirAttribute>());

    [TestMethod]
    public void LoopOptimisation_DoesNotRecreateCanonicalPreheader()
    {
        var function = new MirFunction(new HirFunction("CanonicalLoop", VoidSignature));
        var entry = function.Entry;
        var preheader = function.CreateBlock("loop.preheader");
        var header = function.CreateBlock("loop");
        var body = function.CreateBlock("loop.body");
        var exit = function.CreateBlock("exit");

        entry.Terminator = new MirBranch(preheader);

        var zero = new MirConstInt(BigInteger.Zero);
        preheader.Append(zero);
        preheader.Terminator = new MirBranch(header);

        var phi = new MirPhi(MirType.TInt);
        phi.AddIncoming(preheader, zero);
        header.AppendPhi(phi);

        var limit = new MirConstInt(new BigInteger(4));
        header.Append(limit);
        var compare = new MirCompare(MirCompare.Op.Lt, phi, limit);
        header.Append(compare);
        header.Terminator = new MirCondBranch(compare, body, exit);

        var one = new MirConstInt(BigInteger.One);
        body.Append(one);
        var increment = new MirBinary(MirBinary.Op.Add, phi, one, MirType.TInt);
        body.Append(increment);
        body.Terminator = new MirBranch(header);
        phi.AddIncoming(body, increment);

        exit.Terminator = new MirReturn(null);

        Assert.AreEqual(1, header.Phis.Count, "Expected loop header to contain a single phi node.");
        Assert.AreEqual(2, phi.Inputs.Count, "Phi should accept preheader and loop backedge inputs.");

        var loops = LoopAnalysis.FindNaturalLoops(function.Blocks, function.Entry, static block => MirControlFlow.GetSuccessors(block));
        var loopInfo = loops.Single(info => ReferenceEquals(info.Header, header));
        var predecessors = MirDominatorAnalysis.BuildPredecessors(function);
        Assert.IsTrue(predecessors.TryGetValue(header, out var headerPreds), "Header should have predecessors recorded.");
        Assert.IsTrue(headerPreds.Contains(preheader) && headerPreds.Contains(body),
            $"Unexpected header predecessors: {string.Join(", ", headerPreds.Select(pred => pred.Label))}");
        Assert.IsTrue(preheader.Terminator is MirBranch { Target: var target } && ReferenceEquals(target, header),
            "Canonical preheader must be a simple branch into the loop header.");

        var blockCountBefore = function.Blocks.Count;
        var preheaderCountBefore = function.Blocks.Count(block => block.Label.StartsWith("loop.preheader"));

        var pass = new MirLoopOptimisationPass();
        pass.Run(function);

        var blockLabelsAfter = function.Blocks.Select(block => block.Label).ToArray();
        var preheaderCountAfter = function.Blocks.Count(block => block.Label.StartsWith("loop.preheader"));
        Assert.AreEqual(preheaderCountBefore, preheaderCountAfter, $"Loop normalisation should not introduce additional preheaders when already canonical. Blocks: {string.Join(", ", blockLabelsAfter)}");
        Assert.AreEqual(blockCountBefore, function.Blocks.Count, $"Canonical loop should maintain block count. Blocks: {string.Join(", ", blockLabelsAfter)}");
        Assert.IsTrue(entry.Terminator is MirBranch { Target: var entryTarget } && ReferenceEquals(entryTarget, preheader),
            "Entry should continue branching to the original preheader.");
        Assert.IsTrue(preheader.Terminator is MirBranch { Target: var preheaderTarget } && ReferenceEquals(preheaderTarget, header),
            "Preheader should continue branching to the loop header.");
        var phiInputs = phi.Inputs.Select(input => input.Block).ToArray();
        CollectionAssert.AreEquivalent(new[] { preheader, body }, phiInputs, "Phi should retain preheader and latch inputs.");
    }

    [TestMethod]
    public void LoopOptimisation_CreatesPreheaderForMultipleOutsidePredecessors()
    {
        var function = new MirFunction(new HirFunction("LoopWithMultipleEntries", VoidSignature));
        var entry = function.Entry;
        var header = function.CreateBlock("loop");
        var body = function.CreateBlock("loop.body");
        var exit = function.CreateBlock("exit");
        var spill = function.CreateBlock("spill");

        var zero = new MirConstInt(BigInteger.Zero);
        entry.Append(zero);

        var cond = new MirConstBool(true);
        entry.Append(cond);

        var phi = new MirPhi(MirType.TInt);
        phi.AddIncoming(entry, zero);
        phi.AddIncoming(spill, zero);
        header.AppendPhi(phi);

        entry.Terminator = new MirCondBranch(cond, header, spill);
        spill.Terminator = new MirBranch(header);

        var limit = new MirConstInt(new BigInteger(6));
        header.Append(limit);
        var compare = new MirCompare(MirCompare.Op.Lt, phi, limit);
        header.Append(compare);
        header.Terminator = new MirCondBranch(compare, body, exit);

        var one = new MirConstInt(BigInteger.One);
        body.Append(one);
        var increment = new MirBinary(MirBinary.Op.Add, phi, one, MirType.TInt);
        body.Append(increment);
        body.Terminator = new MirBranch(header);
        phi.AddIncoming(body, increment);

        exit.Terminator = new MirReturn(null);

        Assert.AreEqual(1, header.Phis.Count, "Expected loop header to contain a single phi node.");
        Assert.AreEqual(3, phi.Inputs.Count, "Phi should accept all outside predecessors and the loop backedge.");

        var predecessors = MirDominatorAnalysis.BuildPredecessors(function);
        Assert.IsTrue(predecessors.TryGetValue(header, out var headerPreds), "Header should have predecessors recorded.");
        CollectionAssert.AreEquivalent(new[] { entry.Label, spill.Label, body.Label }, headerPreds.Select(pred => pred.Label).ToArray(),
            "Header should be reached from both external blocks and the latch before normalisation.");

        var pass = new MirLoopOptimisationPass();
        var changed = pass.Run(function);
        Assert.IsTrue(changed, $"Loop with multiple external predecessors should be normalised. Blocks: {string.Join(", ", function.Blocks.Select(b => b.Label))}");

        var preheaders = function.Blocks.Where(block => block.Label.StartsWith("loop.preheader")).ToArray();
        Assert.AreEqual(1, preheaders.Length, $"Normalisation should synthesise a single preheader. Blocks: {string.Join(", ", function.Blocks.Select(b => b.Label))}");
        var preheader = preheaders[0];

        Assert.IsTrue(preheader.Terminator is MirBranch { Target: var branchTarget } && ReferenceEquals(branchTarget, header),
            "New preheader must branch into the loop header.");

        Assert.IsTrue(entry.Terminator is MirCondBranch { TrueTarget: var entryTrue, FalseTarget: var entryFalse }
            && ReferenceEquals(entryTrue, preheader)
            && ReferenceEquals(entryFalse, spill),
            "Entry should now target the preheader on the loop-taking edge.");

        Assert.IsTrue(spill.Terminator is MirBranch { Target: var spillTarget } && ReferenceEquals(spillTarget, preheader),
            "All external predecessors should be rewired to the shared preheader.");

        var phiIncomingBlocks = phi.Inputs.Select(input => input.Block).ToArray();
        CollectionAssert.AreEquivalent(new[] { preheader, body }, phiIncomingBlocks, "Phi should now receive values from preheader and latch only.");
    }

}
