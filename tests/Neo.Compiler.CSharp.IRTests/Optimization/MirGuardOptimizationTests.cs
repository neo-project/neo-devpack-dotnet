using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;
using Neo.Compiler.MIR;
using Neo.Compiler.MIR.Optimization;
using System.Numerics;

namespace Neo.Compiler.CSharp.IRTests.Optimization;

[TestClass]
public sealed class MirGuardOptimizationTests
{
    private static readonly HirSignature VoidSignature = new HirSignature(System.Array.Empty<HirType>(), HirType.VoidType, System.Array.Empty<HirAttribute>());

    [TestMethod]
    public void GuardOptimization_RemovesRedundantNullGuards()
    {
        var function = new MirFunction(new HirFunction("GuardOpt", VoidSignature));
        var entry = function.Entry;

        var constant = new MirConstInt(new BigInteger(12)) { Span = null };
        entry.Append(constant);

        var guard1 = new MirGuardNull(constant, MirGuardFail.Abort) { Span = null };
        entry.Append(guard1);
        var guard2 = new MirGuardNull(constant, MirGuardFail.Abort) { Span = null };
        entry.Append(guard2);

        entry.Terminator = new MirReturn(null);

        var pass = new MirGuardOptimizationPass();
        var changed = pass.Run(function);
        Assert.IsTrue(changed, "Guard optimization should remove redundant or unnecessary null guards.");

        var remainingNullGuards = entry.Instructions.OfType<MirGuardNull>().Count();
        Assert.AreEqual(0, remainingNullGuards, "All redundant guards over constant values should be removed.");
    }

    [TestMethod]
    public void DeadCodeElimination_RemovesUnusedArithmetic()
    {
        var function = new MirFunction(new HirFunction("Unused", VoidSignature));
        var entry = function.Entry;

        var left = new MirConstInt(new BigInteger(2));
        var right = new MirConstInt(new BigInteger(3));
        entry.Append(left);
        entry.Append(right);

        var add = new MirBinary(MirBinary.Op.Add, left, right, MirType.TInt);
        entry.Append(add);

        entry.Terminator = new MirReturn(left);

        var pass = new MirDeadCodeEliminationPass();
        var changed = pass.Run(function);
        Assert.IsTrue(changed, "Dead-code elimination should remove unused MIR binary.");
        Assert.IsFalse(entry.Instructions.Contains(add), "Unused addition should be pruned.");
    }
}
