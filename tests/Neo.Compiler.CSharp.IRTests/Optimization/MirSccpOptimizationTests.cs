using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MIR;
using System.Numerics;

namespace Neo.Compiler.CSharp.IRTests.Optimization;

[TestClass]
public sealed class MirSccpOptimizationTests
{
    [TestMethod]
    public void PropagatesConstantsThroughBranches()
    {
        var hirSignature = new Neo.Compiler.HIR.HirSignature(System.Array.Empty<Neo.Compiler.HIR.HirType>(), Neo.Compiler.HIR.HirType.VoidType, System.Array.Empty<Neo.Compiler.HIR.HirAttribute>());
        var function = new MirFunction(new Neo.Compiler.HIR.HirFunction("ConstBranch", hirSignature));
        var entry = function.Entry;
        var trueBlock = function.CreateBlock("true");
        var falseBlock = function.CreateBlock("false");
        var mergeBlock = function.CreateBlock("merge");

        var span = new Neo.Compiler.HIR.SourceSpan("<generated>", 0, 0, 0, 0);
        var cond = new MirConstBool(true) { Span = span };
        entry.Append(cond);
        entry.Terminator = new MirCondBranch(cond, trueBlock, falseBlock);

        var trueConst = new MirConstInt(new BigInteger(42)) { Span = span };
        trueBlock.Append(trueConst);
        trueBlock.Terminator = new MirBranch(mergeBlock);

        var falseConst = new MirConstInt(new BigInteger(42)) { Span = span };
        falseBlock.Append(falseConst);
        falseBlock.Terminator = new MirBranch(mergeBlock);

        var phi = new MirPhi(MirType.TInt) { Span = span };
        mergeBlock.AppendPhi(phi);
        mergeBlock.Terminator = new MirReturn(phi);
        phi.AddIncoming(trueBlock, trueConst);
        phi.AddIncoming(falseBlock, falseConst);

        var pass = new Neo.Compiler.MIR.Optimization.MirSparseConditionalConstantPropagationPass();
        pass.Run(function);

        if (mergeBlock.Phis.Count == 0)
        {
            var constant = mergeBlock.Instructions.OfType<MirConstInt>().SingleOrDefault();
            Assert.IsNotNull(constant, "Expected constant instruction replacing phi.");
            Assert.AreEqual(new BigInteger(42), constant.Value);
        }
        else
        {
            var optimizedPhi = mergeBlock.Phis.Single();
            Assert.IsTrue(optimizedPhi.Inputs.All(i => i.Value is MirConstInt c && c.Value == 42),
                "Phi should be replaced by propagated constant 42.");
        }
    }

    [TestMethod]
    public void DivergentBranch_KeepsPhiOverdefined()
    {
        var hirSignature = new Neo.Compiler.HIR.HirSignature(new[] { Neo.Compiler.HIR.HirType.BoolType }, Neo.Compiler.HIR.HirType.VoidType, System.Array.Empty<Neo.Compiler.HIR.HirAttribute>());
        var function = new MirFunction(new Neo.Compiler.HIR.HirFunction("DivergentBranch", hirSignature));
        var entry = function.Entry;
        var trueBlock = function.CreateBlock("true");
        var falseBlock = function.CreateBlock("false");
        var mergeBlock = function.CreateBlock("merge");

        var span = new Neo.Compiler.HIR.SourceSpan("<generated>", 0, 0, 0, 0);
        var cond = new MirArg(0, MirType.TBool) { Span = span };
        entry.Append(cond);
        entry.Terminator = new MirCondBranch(cond, trueBlock, falseBlock);

        var trueConst = new MirConstInt(new BigInteger(21)) { Span = span };
        trueBlock.Append(trueConst);
        trueBlock.Terminator = new MirBranch(mergeBlock);

        var falseConst = new MirConstInt(new BigInteger(84)) { Span = span };
        falseBlock.Append(falseConst);
        falseBlock.Terminator = new MirBranch(mergeBlock);

        var phi = new MirPhi(MirType.TInt) { Span = span };
        mergeBlock.AppendPhi(phi);
        mergeBlock.Terminator = new MirReturn(phi);
        phi.AddIncoming(trueBlock, trueConst);
        phi.AddIncoming(falseBlock, falseConst);

        var pass = new Neo.Compiler.MIR.Optimization.MirSparseConditionalConstantPropagationPass();
        pass.Run(function);

        Assert.AreEqual(1, mergeBlock.Phis.Count, "Phi should not be removed when inputs carry different constants.");
        var verificationPhi = mergeBlock.Phis.Single();
        Assert.IsTrue(verificationPhi.Inputs.Any(i => i.Value == trueConst), "Phi should still reference the true-block constant.");
        Assert.IsTrue(verificationPhi.Inputs.Any(i => i.Value == falseConst), "Phi should still reference the false-block constant.");
        Assert.IsFalse(mergeBlock.Instructions.OfType<MirConstInt>().Any(), "SCCP should not materialize a constant in place of an overdefined phi.");
    }

    [TestMethod]
    public void Switch_WithConstantKey_PrunesUnreachableCases()
    {
        var hirSignature = new Neo.Compiler.HIR.HirSignature(System.Array.Empty<Neo.Compiler.HIR.HirType>(), Neo.Compiler.HIR.HirType.IntType, System.Array.Empty<Neo.Compiler.HIR.HirAttribute>());
        var function = new MirFunction(new Neo.Compiler.HIR.HirFunction("SwitchConstant", hirSignature));
        var entry = function.Entry;
        var caseOne = function.CreateBlock("case_one");
        var caseTwo = function.CreateBlock("case_two");
        var defaultBlock = function.CreateBlock("default");

        var span = new Neo.Compiler.HIR.SourceSpan("<generated>", 0, 0, 0, 0);
        var key = new MirConstInt(new BigInteger(2)) { Span = span };
        entry.Append(key);
        entry.Terminator = new MirSwitch(
            key,
            new[]
            {
                (new BigInteger(1), caseOne),
                (new BigInteger(2), caseTwo)
            },
            defaultBlock)
        { Span = span };

        var caseOneValue = new MirConstInt(new BigInteger(100)) { Span = span };
        caseOne.Append(caseOneValue);
        caseOne.Terminator = new MirReturn(caseOneValue) { Span = span };

        var caseTwoValue = new MirConstInt(new BigInteger(200)) { Span = span };
        caseTwo.Append(caseTwoValue);
        caseTwo.Terminator = new MirReturn(caseTwoValue) { Span = span };

        var defaultValue = new MirConstInt(new BigInteger(300)) { Span = span };
        defaultBlock.Append(defaultValue);
        defaultBlock.Terminator = new MirReturn(defaultValue) { Span = span };

        var pass = new Neo.Compiler.MIR.Optimization.MirSparseConditionalConstantPropagationPass();
        pass.Run(function);

        CollectionAssert.DoesNotContain(function.Blocks, caseOne, "Unreachable case block should be removed.");
        CollectionAssert.DoesNotContain(function.Blocks, defaultBlock, "Unreachable default block should be removed.");

        var survivingCases = function.Blocks.Single(b => b.Label == "case_two");
        Assert.AreSame(caseTwo, survivingCases, "Matching case block should remain after SCCP.");
        Assert.IsInstanceOfType(entry.Terminator, typeof(MirSwitch), "Entry terminator should remain a switch for now.");
    }
}
