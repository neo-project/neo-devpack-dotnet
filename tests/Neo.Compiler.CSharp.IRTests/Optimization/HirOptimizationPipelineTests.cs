using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;
using Neo.Compiler.HIR.Optimization;
using System.Numerics;

namespace Neo.Compiler.CSharp.IRTests.Optimization;

[TestClass]
public sealed class HirOptimizationPipelineTests
{
    private static readonly SourceSpan GeneratedSpan = new("<generated>", 0, 0, 0, 0);

    [TestMethod]
    public void Pipeline_FixesPointerCallMergeWithoutResurfacingBlocks()
    {
        var signature = new HirSignature(Array.Empty<HirType>(), HirType.IntType, Array.Empty<HirAttribute>());
        var function = new HirFunction("Raise", signature);
        var entry = function.Entry;
        var pointerPath = function.AddBlock("pointer_path");
        var fallbackPath = function.AddBlock("fallback_path");
        var merge = function.AddBlock("merge");

        var cond = new HirConstBool(true) { Span = GeneratedSpan };
        entry.Append(cond);
        entry.SetTerminator(new HirConditionalBranch(cond, pointerPath, fallbackPath) { Span = GeneratedSpan });

        var pointerCall = new HirPointerCall(
            pointer: null,
            arguments: Array.Empty<HirValue>(),
            HirType.IntType,
            HirCallSemantics.Effectful,
            isTailCall: true,
            callTableIndex: 0)
        { Span = GeneratedSpan };
        pointerPath.Append(pointerCall);
        pointerPath.SetTerminator(new HirBranch(merge) { Span = GeneratedSpan });

        var fallbackValue = new HirConstInt(new BigInteger(5)) { Span = GeneratedSpan };
        fallbackPath.Append(fallbackValue);
        fallbackPath.SetTerminator(new HirBranch(merge) { Span = GeneratedSpan });

        var phi = new HirPhi(HirType.IntType) { Span = GeneratedSpan };
        phi.AddIncoming(pointerPath, pointerCall);
        phi.AddIncoming(fallbackPath, fallbackValue);
        merge.AppendPhi(phi);
        merge.SetTerminator(new HirReturn(phi) { Span = GeneratedSpan });

        var pipeline = new HirOptimizationPipeline();
        pipeline.Run(function);

        Assert.IsFalse(function.Blocks.Any(b => b.Label == "fallback_path"), "Unreachable fallback block should be pruned.");
        var mergeBlock = function.Blocks.Single(b => b.Label == "merge");
        Assert.AreEqual(0, mergeBlock.Phis.Count, "Pointer-call phi should collapse to a direct return.");
        Assert.AreEqual(0, mergeBlock.Instructions.Count, "Merge block should not gain synthetic constants.");
        var ret = mergeBlock.Terminator as HirReturn;
        Assert.IsNotNull(ret, "Merge block should end in a return.");
        Assert.AreSame(pointerCall, ret!.Value, "Return should forward the pointer-call result directly.");

        // Running the pipeline a second time should be a no-op and must not resurrect removed blocks.
        pipeline.Run(function);
        Assert.IsFalse(function.Blocks.Any(b => b.Label == "fallback_path"), "Fallback block should stay pruned after repeated runs.");
    }
}
