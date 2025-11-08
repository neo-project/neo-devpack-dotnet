using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.LIR;
using Neo.Compiler.LIR.Backend;
using System.Numerics;

namespace Neo.Compiler.CSharp.IRTests.Scheduling;

[TestClass]
public sealed class StackSchedulerPointerCallTests
{
    [TestMethod]
    public void TailCall_WithPointerOperand_EmitsCalltAndPopsPointer()
    {
        var function = new VFunction("TailPointerCall");
        var entry = new VBlock("entry");
        function.Blocks.Add(entry);

        var pointer = new VConstInt(new BigInteger(0xCAFE));
        entry.Nodes.Add(pointer);

        var argument = new VConstInt(new BigInteger(7));
        entry.Nodes.Add(argument);

        var pointerCall = new VPointerCall(pointer, new[] { argument }, LirType.TInt, isPure: false, isTailCall: true, callTableIndex: null);
        entry.Nodes.Add(pointerCall);
        entry.Terminator = new VRet(pointerCall);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        var instructions = result.Function.Blocks.Single(b => b.Label == "entry").Instructions;
        var callInst = instructions.Single(i => i.Op == LirOpcode.CALLT);

        Assert.AreEqual(pointerCall.Arguments.Count + 1, callInst.PopOverride, "CALLT should pop the pointer operand and the arguments.");
        Assert.AreEqual(pointerCall.Type is LirVoidType ? 0 : 1, callInst.PushOverride, "CALLT should push the return value for non-void calls.");
        Assert.IsNull(callInst.Immediate, "CALLT should not encode a call-table index when lowering a pointer operand.");
    }

    [TestMethod]
    public void TailCall_WithCallTableIndex_EmitsImmediateAndKeepsArgumentCount()
    {
        var function = new VFunction("TailCallTableCall");
        var entry = new VBlock("entry");
        function.Blocks.Add(entry);

        var argument = new VConstInt(new BigInteger(42));
        entry.Nodes.Add(argument);

        var pointerCall = new VPointerCall(pointer: null, new[] { argument }, LirType.TInt, isPure: false, isTailCall: true, callTableIndex: 5);
        entry.Nodes.Add(pointerCall);
        entry.Terminator = new VRet(pointerCall);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        var instructions = result.Function.Blocks.Single(b => b.Label == "entry").Instructions;
        var callInst = instructions.Single(i => i.Op == LirOpcode.CALLT);

        Assert.AreEqual(pointerCall.Arguments.Count, callInst.PopOverride, "CALLT should only pop the arguments when invoked via call-table index.");
        Assert.IsNotNull(callInst.Immediate, "CALLT should encode the call-table index as an immediate operand.");
        Assert.AreEqual(1, callInst.Immediate!.Length, "CALLT should emit exactly one immediate byte for the call-table index.");
        Assert.AreEqual(pointerCall.CallTableIndex, callInst.Immediate[0], "CALLT immediate should match the pointer call's call-table index.");
    }
}
