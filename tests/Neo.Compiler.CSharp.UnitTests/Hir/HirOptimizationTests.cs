using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;
using Neo.Compiler.HIR.Optimization;
using System;
using System.Numerics;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.Hir;

[TestClass]
public sealed class HirOptimizationTests
{
    [TestMethod]
    public void GuardPrune_Removes_Assume_Guards()
    {
        var signature = new HirSignature(System.Array.Empty<HirType>(), HirType.VoidType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("test", signature);
        var entry = function.Entry;

        var value = new HirConstInt(System.Numerics.BigInteger.One);
        entry.Append(value);
        entry.Append(new HirNullCheck(value, HirFailPolicy.Assume));
        entry.SetTerminator(new HirReturn(null));

        var pipeline = new HirOptimizationPipeline();
        pipeline.Run(function);

        Assert.AreEqual(0, entry.Instructions.Count, "Assume guards and their unused operands should be pruned.");
    }

    [TestMethod]
    public void ConstantFolder_Folds_Integer_Addition()
    {
        var signature = new HirSignature(System.Array.Empty<HirType>(), HirType.IntType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("test", signature);
        var entry = function.Entry;

        var left = new HirConstInt(new BigInteger(2));
        var right = new HirConstInt(new BigInteger(3));
        entry.Append(left);
        entry.Append(right);
        var add = new HirAdd(left, right, HirType.IntType);
        entry.Append(add);
        entry.SetTerminator(new HirReturn(add));

        var pipeline = new HirOptimizationPipeline();
        pipeline.Run(function);

        Assert.AreEqual(1, entry.Instructions.Count, "Folded constant should remain for the return value.");
        Assert.IsInstanceOfType(entry.Instructions[0], typeof(HirConstInt));
        var folded = (HirConstInt)entry.Instructions[0];
        Assert.AreEqual(new BigInteger(5), folded.Value);
    }

    [TestMethod]
    public void Dce_Removes_Unused_Pure_Values()
    {
        var signature = new HirSignature(System.Array.Empty<HirType>(), HirType.VoidType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("test", signature);
        var entry = function.Entry;

        entry.Append(new HirConstInt(new BigInteger(0)));
        entry.Append(new HirConstInt(new BigInteger(1)));
        entry.SetTerminator(new HirReturn(null));

        var pipeline = new HirOptimizationPipeline();
        pipeline.Run(function);

        Assert.AreEqual(0, entry.Instructions.Count, "Unused pure instructions should be removed by DCE.");
    }
    [TestMethod]
    public void PhiSimplification_ReplacesUniformPhi()
    {
        var signature = new HirSignature(System.Array.Empty<HirType>(), HirType.IntType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("phi", signature);
        var entry = function.Entry;
        var trueBlock = function.AddBlock("true");
        var falseBlock = function.AddBlock("false");
        var mergeBlock = function.AddBlock("merge");

        var condition = new HirConstBool(true);
        entry.Append(condition);
        var value = new HirConstInt(new BigInteger(42));
        entry.Append(value);
        entry.SetTerminator(new HirConditionalBranch(condition, trueBlock, falseBlock));

        trueBlock.SetTerminator(new HirBranch(mergeBlock));
        falseBlock.SetTerminator(new HirBranch(mergeBlock));

        var phi = new HirPhi(HirType.IntType);
        phi.AddIncoming(trueBlock, value);
        phi.AddIncoming(falseBlock, value);
        mergeBlock.AppendPhi(phi);
        mergeBlock.SetTerminator(new HirReturn(phi));

        var pipeline = new HirOptimizationPipeline();
        pipeline.Run(function);

        Assert.AreEqual(0, mergeBlock.Phis.Count, "Uniform phi should be eliminated.");
        var terminator = mergeBlock.Terminator as HirReturn;
        Assert.IsNotNull(terminator);
        Assert.IsInstanceOfType(terminator!.Value, typeof(HirConstInt));
    }

    [TestMethod]
    public void BranchSimplification_ReplacesConstantConditional()
    {
        var signature = new HirSignature(System.Array.Empty<HirType>(), HirType.VoidType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("branch", signature);
        var entry = function.Entry;
        var trueBlock = function.AddBlock("true");
        var falseBlock = function.AddBlock("false");

        var condition = new HirConstBool(false);
        entry.Append(condition);
        entry.SetTerminator(new HirConditionalBranch(condition, trueBlock, falseBlock));
        trueBlock.SetTerminator(new HirReturn(null));
        falseBlock.SetTerminator(new HirReturn(null));

        var pipeline = new HirOptimizationPipeline();
        pipeline.Run(function);

        Assert.IsInstanceOfType(entry.Terminator, typeof(HirBranch));
        var branch = (HirBranch)entry.Terminator!;
        Assert.AreSame(falseBlock, branch.Target);
    }

    [TestMethod]
    public void UnreachableBlockElimination_RemovesDeadBlocks()
    {
        var signature = new HirSignature(System.Array.Empty<HirType>(), HirType.VoidType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("unreachable", signature);
        var entry = function.Entry;
        entry.SetTerminator(new HirReturn(null));

        var dead = function.AddBlock("dead");
        dead.SetTerminator(new HirReturn(null));

        var pipeline = new HirOptimizationPipeline();
        pipeline.Run(function);

        Assert.IsFalse(function.Blocks.Contains(dead), "Unreachable block should be removed.");
    }

    [TestMethod]
    public void Dce_Preserves_Effectful_Instruction()
    {
        var signature = new HirSignature(Array.Empty<HirType>(), HirType.VoidType, Array.Empty<HirAttribute>());
        var function = new HirFunction("effectful", signature);
        var entry = function.Entry;

        var value = new HirConstInt(new BigInteger(7));
        entry.Append(value);
        entry.Append(new HirStoreStaticField(0, value, HirType.IntType, "Counter"));
        entry.Append(new HirConstInt(new BigInteger(42))); // unused pure constant
        entry.SetTerminator(new HirReturn(null));

        var pipeline = new HirOptimizationPipeline();
        pipeline.Run(function);

        Assert.AreEqual(2, entry.Instructions.Count, "Effectful store should remain while unused pure constant is removed.");
        Assert.IsInstanceOfType(entry.Instructions.Last(), typeof(HirStoreStaticField), "Last instruction before return should be static store.");
    }
}
