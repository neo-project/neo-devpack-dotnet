using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;
using Neo.Compiler.HIR.Optimization;
using System.Numerics;

namespace Neo.Compiler.CSharp.IRTests.Optimization;

[TestClass]
public sealed class HirCopyPropagationTests
{
    private static readonly SourceSpan GeneratedSpan = new("<generated>", 0, 0, 0, 0);

    [TestMethod]
    public void CopyPropagation_ReplacesLocalLoads()
    {
        var signature = new HirSignature(System.Array.Empty<HirType>(), HirType.IntType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("Copy", signature);
        var entry = function.Entry;

        var local = new HirLocal("temp", HirType.IntType);
        var constant = new HirConstInt(new BigInteger(5)) { Span = GeneratedSpan };
        entry.Append(constant);
        var store = new HirStoreLocal(local, constant) { Span = GeneratedSpan };
        entry.Append(store);
        var load = new HirLoadLocal(local) { Span = GeneratedSpan };
        entry.Append(load);
        entry.SetTerminator(new HirReturn(load) { Span = GeneratedSpan });

        var pass = new HirCopyPropagationPass();
        var changed = pass.Run(function);
        Assert.IsTrue(changed, "Copy propagation should fold load-local into original value.");
        Assert.IsFalse(entry.Instructions.Contains(load), "LoadLocal should be removed after propagation.");
        Assert.AreSame(constant, ((HirReturn)entry.Terminator!).Value, "Return should reference propagated constant.");
    }

    [TestMethod]
    public void BranchSimplification_CollapsesConstantCondition()
    {
        var signature = new HirSignature(System.Array.Empty<HirType>(), HirType.IntType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("Branch", signature);
        var entry = function.Entry;
        var trueBlock = function.AddBlock("true");
        var falseBlock = function.AddBlock("false");

        var condition = new HirConstBool(true) { Span = GeneratedSpan };
        entry.Append(condition);
        entry.SetTerminator(new HirConditionalBranch(condition, trueBlock, falseBlock) { Span = GeneratedSpan });

        var value = new HirConstInt(new BigInteger(10)) { Span = GeneratedSpan };
        trueBlock.Append(value);
        trueBlock.SetTerminator(new HirReturn(value) { Span = GeneratedSpan });
        falseBlock.SetTerminator(new HirReturn(new HirConstInt(BigInteger.Zero)) { Span = GeneratedSpan });

        var pass = new HirBranchSimplificationPass();
        var changed = pass.Run(function);
        Assert.IsTrue(changed, "Branch simplification should collapse constant branch.");
        Assert.IsInstanceOfType(entry.Terminator, typeof(HirBranch));
        Assert.AreSame(trueBlock, ((HirBranch)entry.Terminator!).Target, "Entry should jump directly to true block.");
    }

    [TestMethod]
    public void GuardPrune_DropsAssumeBounds()
    {
        var signature = new HirSignature(System.Array.Empty<HirType>(), HirType.VoidType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("GuardPrune", signature);
        var entry = function.Entry;

        var index = new HirConstInt(BigInteger.Zero) { Span = GeneratedSpan };
        entry.Append(index);
        var length = new HirConstInt(new BigInteger(3)) { Span = GeneratedSpan };
        entry.Append(length);
        var guard = new HirBoundsCheck(index, length, HirFailPolicy.Assume) { Span = GeneratedSpan };
        entry.Append(guard);
        entry.SetTerminator(new HirReturn(null) { Span = GeneratedSpan });

        var pass = new HirGuardPrunePass();
        var changed = pass.Run(function);
        Assert.IsTrue(changed, "Guard prune should remove assume-policy checks.");
        Assert.IsFalse(entry.Instructions.OfType<HirBoundsCheck>().Any(), "Assume guard should be pruned.");
    }

    [TestMethod]
    public void UnreachableBlockElimination_RemovesDetachedBlock()
    {
        var signature = new HirSignature(System.Array.Empty<HirType>(), HirType.VoidType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("Unreachable", signature);
        var entry = function.Entry;
        var dead = function.AddBlock("dead");

        entry.SetTerminator(new HirReturn(null) { Span = GeneratedSpan });
        dead.SetTerminator(new HirReturn(null) { Span = GeneratedSpan });

        var pass = new HirUnreachableBlockEliminationPass();
        var changed = pass.Run(function);
        Assert.IsTrue(changed, "Unreachable block elimination should remove detached block.");
        Assert.IsFalse(function.Blocks.Any(b => b.Label == "dead"), "Dead block should be removed.");
    }
}
