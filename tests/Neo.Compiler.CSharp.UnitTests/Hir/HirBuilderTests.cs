using System;
using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;

namespace Neo.Compiler.CSharp.UnitTests.Hir;

[TestClass]
public sealed class HirBuilderTests
{
    private static HirBuilder CreateBuilder(string name = "test", HirType? returnType = null)
    {
        var signature = new HirSignature(Array.Empty<HirType>(), returnType ?? HirType.VoidType, Array.Empty<HirAttribute>());
        var function = new HirFunction(name, signature);
        return new HirBuilder(function);
    }

    [TestMethod]
    public void Builder_AppendsPhiAndTracksSequencePoints()
    {
        var builder = CreateBuilder(returnType: HirType.IntType);
        var entry = builder.CurrentBlock;
        var body = builder.CreateBlock("body");

        builder.AppendTerminator(new HirBranch(body));
        builder.SetCurrentBlock(body);

        var span = new SourceSpan("contract.cs", 10, 5, 10, 20);
        builder.MarkLocation(span);

        var incomingConst = new HirConstInt(BigInteger.Zero);
        entry.Append(incomingConst);
        var phi = new HirPhi(HirType.IntType);
        phi.AddIncoming(entry, incomingConst);
        builder.AppendPhi(phi);

        Assert.AreEqual(span, phi.Span, "Sequence point should flow to appended phi nodes.");
        Assert.AreEqual(body, builder.CurrentBlock, "Builder should continue appending to the current block.");
        Assert.AreEqual(1, body.Phis.Count, "Phi must be added to current block.");

        builder.MarkLocation(span);
        var value = builder.Append(new HirConstInt(BigInteger.One));
        Assert.AreEqual(span, value.Span, "Sequence point should apply to appended instruction once.");

        var next = builder.Append(new HirConstInt(new BigInteger(2)));
        Assert.IsNull(next.Span, "Pending sequence point should clear after first append.");

        Assert.AreEqual(2, body.Instructions.Count, "Two constants expected in body block.");
    }

    [TestMethod]
    public void Builder_MemoryTokenFlowsAndResetsOnTerminator()
    {
        var builder = CreateBuilder();
        var entry = builder.CurrentBlock;
        var work = builder.CreateBlock("work");

        builder.AppendTerminator(new HirBranch(work));
        builder.SetCurrentBlock(work);

        var value = builder.Append(new HirConstInt(BigInteger.One));
        Assert.IsNull(builder.CurrentMemoryToken, "Pure instructions should not allocate memory token.");

        var store = builder.Append(new HirStoreStaticField(0, value, HirType.IntType, "StaticField"));
        Assert.AreSame(store, builder.CurrentMemoryToken, "Effectful instruction should become active memory token.");

        builder.AppendTerminator(new HirReturn(value));
        Assert.IsNull(builder.CurrentMemoryToken, "Memory token must reset after terminator.");

        Assert.IsInstanceOfType(work.Terminator, typeof(HirReturn), "Return terminator expected on work block.");
        Assert.IsInstanceOfType(entry.Terminator, typeof(HirBranch), "Entry block should branch to work block.");
    }

    [TestMethod]
    public void Builder_SetCurrentBlockAllowsMultipleRegions()
    {
        var builder = CreateBuilder();
        var first = builder.CreateBlock("first");
        var second = builder.CreateBlock("second");

        builder.AppendTerminator(new HirBranch(first));
        builder.SetCurrentBlock(first);
        var value = builder.Append(new HirConstBool(true));
        builder.AppendTerminator(new HirReturn(value));

        builder.SetCurrentBlock(second);
        var phi = builder.AppendPhi(new HirPhi(HirType.BoolType));
        phi.AddIncoming(first, value);
        builder.AppendTerminator(new HirBranch(first));

        Assert.AreSame(second, builder.CurrentBlock, "SetCurrentBlock should update the active block.");
        Assert.IsTrue(second.Phis.Contains(phi), "Phi should be added to selected block.");
        Assert.AreEqual(1, first.Instructions.Count, "First block should contain boolean constant.");
        Assert.AreEqual(0, builder.Function.Entry.Instructions.Count, "Entry block should only contain branch terminator.");
    }
}
