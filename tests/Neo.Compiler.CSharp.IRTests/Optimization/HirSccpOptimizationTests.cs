using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;
using Neo.Compiler.HIR.Optimization;
using System.Numerics;

namespace Neo.Compiler.CSharp.IRTests.Optimization;

[TestClass]
public sealed class HirSccpOptimizationTests
{
    private static readonly SourceSpan GeneratedSpan = new("<generated>", 0, 0, 0, 0);

    [TestMethod]
    public void ConstantBranches_RemainStableAcrossRuns()
    {
        var signature = new HirSignature(new[] { HirType.BoolType }, HirType.IntType, Array.Empty<HirAttribute>());
        var function = new HirFunction("ConstBranch", signature);
        var entry = function.Entry;
        var trueBlock = function.AddBlock("true");
        var falseBlock = function.AddBlock("false");
        var mergeBlock = function.AddBlock("merge");

        var argument = new HirArgument("cond", HirType.BoolType, 0);
        var loadCond = new HirLoadArgument(argument) { Span = GeneratedSpan };
        entry.Append(loadCond);
        entry.SetTerminator(new HirConditionalBranch(loadCond, trueBlock, falseBlock) { Span = GeneratedSpan });

        var trueConst = new HirConstInt(new BigInteger(42)) { Span = GeneratedSpan };
        trueBlock.Append(trueConst);
        trueBlock.SetTerminator(new HirBranch(mergeBlock) { Span = GeneratedSpan });

        var falseConst = new HirConstInt(new BigInteger(42)) { Span = GeneratedSpan };
        falseBlock.Append(falseConst);
        falseBlock.SetTerminator(new HirBranch(mergeBlock) { Span = GeneratedSpan });

        var phi = new HirPhi(HirType.IntType) { Span = GeneratedSpan };
        phi.AddIncoming(trueBlock, trueConst);
        phi.AddIncoming(falseBlock, falseConst);
        mergeBlock.AppendPhi(phi);
        mergeBlock.SetTerminator(new HirReturn(phi) { Span = GeneratedSpan });

        var pass = new HirSparseConditionalConstantPropagationPass();
        var instructionCounts = new int[4];
        var phiCounts = new int[4];

        for (int i = 0; i < instructionCounts.Length; i++)
        {
            pass.Run(function);
            instructionCounts[i] = mergeBlock.Instructions.Count;
            phiCounts[i] = mergeBlock.Phis.Count;
        }

        Assert.IsTrue(instructionCounts.All(c => c == instructionCounts[0]), "SCCP should not grow instruction lists across runs.");
        Assert.IsTrue(phiCounts.All(c => c == phiCounts[0]), "Phi count should remain stable across repeated SCCP runs.");
    }

    [TestMethod]
    public void EffectfulPointerCall_RemainsStableAcrossRuns()
    {
        var signature = new HirSignature(Array.Empty<HirType>(), HirType.IntType, Array.Empty<HirAttribute>());
        var function = new HirFunction("PointerCall", signature);
        var entry = function.Entry;
        var trueBlock = function.AddBlock("true");
        var falseBlock = function.AddBlock("false");
        var mergeBlock = function.AddBlock("merge");

        var cond = new HirConstBool(true) { Span = GeneratedSpan };
        entry.Append(cond);
        entry.SetTerminator(new HirConditionalBranch(cond, trueBlock, falseBlock) { Span = GeneratedSpan });

        var pointerCall = new HirPointerCall(
            pointer: null,
            arguments: Array.Empty<HirValue>(),
            HirType.IntType,
            HirCallSemantics.Effectful,
            isTailCall: true,
            callTableIndex: 0) { Span = GeneratedSpan };
        trueBlock.Append(pointerCall);
        trueBlock.SetTerminator(new HirBranch(mergeBlock) { Span = GeneratedSpan });

        var fallbackValue = new HirConstInt(new BigInteger(7)) { Span = GeneratedSpan };
        falseBlock.Append(fallbackValue);
        falseBlock.SetTerminator(new HirBranch(mergeBlock) { Span = GeneratedSpan });

        var phi = new HirPhi(HirType.IntType) { Span = GeneratedSpan };
        phi.AddIncoming(trueBlock, pointerCall);
        phi.AddIncoming(falseBlock, fallbackValue);
        mergeBlock.AppendPhi(phi);
        mergeBlock.SetTerminator(new HirReturn(phi) { Span = GeneratedSpan });

        var pass = new HirSparseConditionalConstantPropagationPass();
        var instructionCounts = new int[4];
        var phiCounts = new int[4];

        for (int i = 0; i < instructionCounts.Length; i++)
        {
            pass.Run(function);
            instructionCounts[i] = mergeBlock.Instructions.Count;
            phiCounts[i] = mergeBlock.Phis.Count;
        }

        Assert.IsTrue(instructionCounts.All(c => c == instructionCounts[0]), "SCCP should not continually insert new instructions for pointer-call merges.");
        Assert.IsTrue(phiCounts.All(c => c == phiCounts[0]), "Phi population should remain stable for pointer-call scenarios.");
    }
}
