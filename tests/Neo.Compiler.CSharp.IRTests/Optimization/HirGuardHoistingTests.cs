using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;
using Neo.Compiler.HIR.Optimization;
using System.Numerics;

namespace Neo.Compiler.CSharp.IRTests.Optimization;

[TestClass]
public sealed class HirGuardHoistingTests
{
    private static readonly SourceSpan GeneratedSpan = new("<generated>", 0, 0, 0, 0);

    [TestMethod]
    public void GuardHoisting_RemovesDuplicateNullChecks()
    {
        var signature = new HirSignature(new[] { HirType.ByteStringType }, HirType.ByteStringType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("NullGuards", signature);
        var entry = function.Entry;

        var argument = new HirArgument("value", HirType.ByteStringType, 0);
        var load = new HirLoadArgument(argument) { Span = GeneratedSpan };
        entry.Append(load);

        var guard1 = new HirNullCheck(load, HirFailPolicy.Abort) { Span = GeneratedSpan };
        entry.Append(guard1);
        var guard2 = new HirNullCheck(load, HirFailPolicy.Abort) { Span = GeneratedSpan };
        entry.Append(guard2);

        entry.SetTerminator(new HirReturn(load) { Span = GeneratedSpan });

        var pass = new HirGuardHoistingPass();
        var changed = pass.Run(function);
        Assert.IsTrue(changed, "Guard hoisting should remove redundant null checks.");

        var remainingGuards = entry.Instructions.OfType<HirNullCheck>().Count();
        Assert.AreEqual(1, remainingGuards, "Only a single guard should remain after hoisting.");
    }

    [TestMethod]
    public void DeadCodeElimination_RemovesUnusedArithmetic()
    {
        var signature = new HirSignature(System.Array.Empty<HirType>(), HirType.IntType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("DeadCode", signature);
        var entry = function.Entry;

        var c1 = new HirConstInt(new BigInteger(2)) { Span = GeneratedSpan };
        entry.Append(c1);
        var c2 = new HirConstInt(new BigInteger(3)) { Span = GeneratedSpan };
        entry.Append(c2);
        var sum = new HirAdd(c1, c2, HirType.IntType) { Span = GeneratedSpan };
        entry.Append(sum);
        entry.SetTerminator(new HirReturn(c1) { Span = GeneratedSpan });

        var pass = new HirDeadCodeEliminationPass();
        var changed = pass.Run(function);
        Assert.IsTrue(changed, "Dead-code elimination should remove unused arithmetic.");
        Assert.IsFalse(entry.Instructions.Contains(sum), "Unused add should be removed.");
    }
}
