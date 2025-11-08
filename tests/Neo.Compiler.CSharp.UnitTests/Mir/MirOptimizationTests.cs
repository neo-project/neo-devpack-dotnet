using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;
using Neo.Compiler.MIR;
using Neo.Compiler.MIR.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.Mir;

[TestClass]
public sealed class MirOptimizationTests
{
    private static MirFunction CreateFunction(string name, HirType returnType)
    {
        var hirSignature = new HirSignature(System.Array.Empty<HirType>(), returnType, System.Array.Empty<HirAttribute>());
        var hirFunction = new HirFunction(name, hirSignature);
        return new MirFunction(hirFunction);
    }

    [TestMethod]
    public void ConstantFolder_FoldsBinary()
    {
        var mirFunction = CreateFunction("fold", HirType.IntType);
        var block = mirFunction.Entry;

        var two = new MirConstInt(new BigInteger(2));
        var three = new MirConstInt(new BigInteger(3));
        block.Append(two);
        block.Append(three);
        var add = new MirBinary(MirBinary.Op.Add, two, three, MirType.TInt);
        block.Append(add);
        block.Terminator = new MirReturn(add);

        var pipeline = new MirOptimizationPipeline();
        pipeline.Run(mirFunction);

        Assert.AreEqual(2, block.Instructions.Count, string.Join(",", block.Instructions.Select(i => i.GetType().Name)));
        Assert.IsInstanceOfType(block.Instructions.Last(), typeof(MirConstInt));
        var folded = (MirConstInt)block.Instructions.Last();
        Assert.AreEqual(new BigInteger(5), folded.Value);
    }

    [TestMethod]
    public void DeadCodeElimination_RemovesUnusedPureInst()
    {
        var mirFunction = CreateFunction("dce", HirType.VoidType);
        var block = mirFunction.Entry;

        block.Append(new MirConstInt(BigInteger.Zero));
        block.Append(new MirConstInt(BigInteger.One));
        block.Terminator = new MirReturn(null);

        var pipeline = new MirOptimizationPipeline();
        pipeline.Run(mirFunction);

        Assert.AreEqual(1, block.Instructions.Count, string.Join(",", block.Instructions.Select(i => i.GetType().Name)));
    }

    [TestMethod]
    public void PhiSimplification_RemovesUniformPhi()
    {
        var mirFunction = CreateFunction("phi", HirType.IntType);
        var entry = mirFunction.Entry;
        var trueBlock = mirFunction.CreateBlock("true");
        var falseBlock = mirFunction.CreateBlock("false");
        var mergeBlock = mirFunction.CreateBlock("merge");

        var value = new MirConstInt(new BigInteger(7));
        entry.Append(value);
        entry.Terminator = new MirCondBranch(new MirConstBool(true), trueBlock, falseBlock);
        trueBlock.Terminator = new MirBranch(mergeBlock);
        falseBlock.Terminator = new MirBranch(mergeBlock);

        var phi = new MirPhi(MirType.TInt);
        phi.AddIncoming(trueBlock, value);
        phi.AddIncoming(falseBlock, value);
        mergeBlock.AppendPhi(phi);
        mergeBlock.Terminator = new MirReturn(phi);

        var pipeline = new MirOptimizationPipeline();
        pipeline.Run(mirFunction);

        Assert.AreEqual(0, mergeBlock.Phis.Count);
        Assert.IsInstanceOfType(((MirReturn)mergeBlock.Terminator!).Value, typeof(MirConstInt));
    }

    [TestMethod]
    public void BranchSimplification_RemovesConstantCondition()
    {
        var mirFunction = CreateFunction("branch", HirType.VoidType);
        var entry = mirFunction.Entry;
        var trueBlock = mirFunction.CreateBlock("true");
        var falseBlock = mirFunction.CreateBlock("false");

        entry.Terminator = new MirCondBranch(new MirConstBool(false), trueBlock, falseBlock);
        trueBlock.Terminator = new MirReturn(null);
        falseBlock.Terminator = new MirReturn(null);

        var pipeline = new MirOptimizationPipeline();
        pipeline.Run(mirFunction);

        Assert.IsInstanceOfType(entry.Terminator, typeof(MirBranch));
        Assert.AreSame(falseBlock, ((MirBranch)entry.Terminator!).Target);
    }

    [TestMethod]
    public void UnreachableBlockElimination_RemovesDeadBlock()
    {
        var mirFunction = CreateFunction("unreach", HirType.VoidType);
        var entry = mirFunction.Entry;
        entry.Terminator = new MirReturn(null);

        var dead = mirFunction.CreateBlock("dead");
        dead.Terminator = new MirReturn(null);

        var pipeline = new MirOptimizationPipeline();
        pipeline.Run(mirFunction);

        CollectionAssert.DoesNotContain(mirFunction.Blocks, dead);
    }

    [TestMethod]
    public void ContainerVersioning_BarriarsOnStorageWrite()
    {
        var mirFunction = CreateFunction("storage_barrier", HirType.IntType);
        var block = mirFunction.Entry;

        var length = new MirConstInt(new BigInteger(2));
        block.Append(length);

        var array = new MirArrayNew(length, MirType.TInt);
        block.Append(array);

        var len1 = new MirArrayLen(array);
        block.Append(len1);

        var context = new MirConstByteString(new byte[] { 0x00 });
        block.Append(context);
        var key = new MirConstByteString(new byte[] { 0x01 });
        block.Append(key);
        var value = new MirConstByteString(new byte[] { 0x02 });
        block.Append(value);

        var storageBarrier = new MirSyscall(
            "Storage",
            "Put",
            new MirValue[] { context, key, value },
            MirType.TVoid,
            MirEffect.StorageWrite);
        var currentToken = storageBarrier.AttachMemoryToken(mirFunction.EntryToken);
        block.Append(storageBarrier);

        var len2 = new MirArrayLen(array);
        block.Append(len2);

        var sum = new MirBinary(MirBinary.Op.Add, len1, len2, MirType.TInt);
        block.Append(sum);
        block.Terminator = new MirReturn(sum);

        var pipeline = new MirOptimizationPipeline();
        pipeline.Run(mirFunction);

        var finalLens = block.Instructions.OfType<MirArrayLen>().ToList();
        Assert.AreEqual(2, finalLens.Count, "Storage write should act as barrier for array length caching.");
    }

    [TestMethod]
    public void LoopSimplify_InsertsPreheader()
    {
        var mirFunction = CreateFunction("loop", HirType.VoidType);
        var entry = mirFunction.Entry;
        var header = mirFunction.CreateBlock("header");
        var body = mirFunction.CreateBlock("body");
        var exit = mirFunction.CreateBlock("exit");

        var zero = new MirConstInt(BigInteger.Zero);
        entry.Append(zero);
        entry.Terminator = new MirBranch(header);

        var phi = new MirPhi(MirType.TInt);
        phi.AddIncoming(entry, zero);
        header.AppendPhi(phi);

        var one = new MirConstInt(BigInteger.One);
        header.Append(one);
        var compare = new MirCompare(MirCompare.Op.Lt, phi, one, unsigned: false);
        header.Append(compare);
        header.Terminator = new MirCondBranch(compare, body, exit);

        var increment = new MirBinary(MirBinary.Op.Add, phi, one, MirType.TInt);
        body.Append(increment);
        body.Terminator = new MirBranch(header);
        phi.AddIncoming(body, increment);

        exit.Terminator = new MirReturn(null);

        var pipeline = new MirOptimizationPipeline();
        pipeline.Run(mirFunction);

        var preheader = mirFunction.Blocks.FirstOrDefault(b => b.Label.StartsWith("header.preheader", StringComparison.Ordinal));
        Assert.IsNotNull(preheader, "Loop simplify should introduce a preheader block.");
        Assert.IsInstanceOfType(preheader!.Terminator, typeof(MirBranch));
        Assert.AreSame(header, ((MirBranch)preheader.Terminator!).Target);

        Assert.IsInstanceOfType(entry.Terminator, typeof(MirBranch));
        Assert.AreSame(preheader, ((MirBranch)entry.Terminator!).Target);

        Assert.IsTrue(phi.Inputs.Any(input => ReferenceEquals(input.Block, preheader) && ReferenceEquals(input.Value, zero)));
    }

    [TestMethod]
    public void CompareBranchLowering_FusesCompareAndBranch()
    {
        var hirSignature = new HirSignature(
            new[] { HirType.IntType, HirType.IntType },
            HirType.VoidType,
            Array.Empty<HirAttribute>());
        var hirFunction = new HirFunction("cmp_branch", hirSignature);
        var mirFunction = new MirFunction(hirFunction);

        var entry = mirFunction.Entry;
        var trueBlock = mirFunction.CreateBlock("true");
        var falseBlock = mirFunction.CreateBlock("false");

        var lhs = new MirArg(0, MirType.TInt);
        var rhs = new MirArg(1, MirType.TInt);
        entry.Append(lhs);
        entry.Append(rhs);

        var compare = new MirCompare(MirCompare.Op.Gt, lhs, rhs, unsigned: false);
        entry.Append(compare);
        entry.Terminator = new MirCondBranch(compare, trueBlock, falseBlock);

        trueBlock.Terminator = new MirReturn(null);
        falseBlock.Terminator = new MirReturn(null);

        var pass = new MirCompareBranchLoweringPass();
        var changed = pass.Run(mirFunction);

        Assert.IsTrue(changed, "Pass should fold compare/branch pattern.");

        CollectionAssert.DoesNotContain(entry.Instructions, compare);
        Assert.IsInstanceOfType(entry.Terminator, typeof(MirCompareBranch));

        var cmpBranch = (MirCompareBranch)entry.Terminator!;
        Assert.AreEqual(MirCompare.Op.Gt, cmpBranch.Operation);
        Assert.AreSame(lhs, cmpBranch.Left);
        Assert.AreSame(rhs, cmpBranch.Right);
        Assert.AreSame(trueBlock, cmpBranch.TrueTarget);
        Assert.AreSame(falseBlock, cmpBranch.FalseTarget);
        Assert.IsFalse(cmpBranch.Unsigned);
    }
}
