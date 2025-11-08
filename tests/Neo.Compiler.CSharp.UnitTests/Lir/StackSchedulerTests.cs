using System;
using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Hir;
using Neo.Compiler.LIR;
using Neo.Compiler.LIR.Backend;
using Neo.Compiler.MIR;
using Neo.Compiler.MiddleEnd.Lowering;

namespace Neo.Compiler.CSharp.UnitTests.Lir;

[TestClass]
public sealed class StackSchedulerTests
{
    [TestMethod]
    public void Scheduler_Emits_Convert_And_Memcpy()
    {
        var function = new VFunction("buffer_copy");
        var block = new VBlock("entry");
        function.Blocks.Add(block);

        var constDestLen = new VConstInt(new BigInteger(4));
        block.Nodes.Add(constDestLen);

        var destBuffer = new VBufferNew(constDestLen);
        block.Nodes.Add(destBuffer);

        var srcBuffer = new VConstBuffer(new byte[] { 0x01, 0x02, 0x03, 0x04 });
        block.Nodes.Add(srcBuffer);

        var destOffset = new VConstInt(BigInteger.Zero);
        block.Nodes.Add(destOffset);

        var srcOffset = new VConstInt(BigInteger.One);
        block.Nodes.Add(srcOffset);

        var length = new VConstInt(new BigInteger(2));
        block.Nodes.Add(length);

        var bufferCopy = new VBufferCopy(destBuffer, srcBuffer, destOffset, srcOffset, length);
        block.Nodes.Add(bufferCopy);

        var convertToByteString = new VConvert(VConvertOp.ToByteString, bufferCopy, LirType.TByteString);
        block.Nodes.Add(convertToByteString);

        block.Terminator = new VRet(convertToByteString);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        Assert.AreEqual(1, result.Function.Blocks.Count, "Scheduler should produce a single block.");
        var instructions = result.Function.Blocks[0].Instructions;

        Assert.IsTrue(instructions.Any(i => i.Op == LirOpcode.MEMCPY), "MEMCPY should be emitted for VBufferCopy.");
        Assert.IsTrue(instructions.Any(i => i.Op == LirOpcode.CONVERT), "CONVERT should be emitted for VConvert ToByteString.");

        var memcpyIndex = instructions.FindIndex(i => i.Op == LirOpcode.MEMCPY);
        var convertAfterMemcopyIndex = memcpyIndex >= 0
            ? instructions.FindIndex(memcpyIndex + 1, instructions.Count - (memcpyIndex + 1), i => i.Op == LirOpcode.CONVERT)
            : -1;
        Assert.IsTrue(memcpyIndex >= 0 && convertAfterMemcopyIndex >= 0, "A CONVERT should appear after MEMCPY in the instruction stream.");
    }

    [TestMethod]
    public void Scheduler_Converts_Buffer_Literal_Before_Mutation()
    {
        var function = new VFunction("buffer_literal");
        var block = new VBlock("entry");
        function.Blocks.Add(block);

        var literal = new VConstBuffer(new byte[] { 0x01, 0x02 });
        block.Nodes.Add(literal);

        var index = new VConstInt(BigInteger.Zero);
        block.Nodes.Add(index);

        var value = new VConstInt(new BigInteger(0xFF));
        block.Nodes.Add(value);

        var bufferSet = new VBufferSet(literal, index, value);
        block.Nodes.Add(bufferSet);

        block.Terminator = new VRet(bufferSet);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        var instructions = result.Function.Blocks[0].Instructions;
        var pushDataIndex = instructions.FindIndex(i => i.Op is LirOpcode.PUSHDATA1 or LirOpcode.PUSHDATA2 or LirOpcode.PUSHDATA4);
        Assert.IsTrue(pushDataIndex >= 0, "Buffer literal should be emitted as PUSHDATA.");

        int convertIndex = -1;
        if (pushDataIndex >= 0 && pushDataIndex + 1 < instructions.Count)
        {
            convertIndex = instructions.FindIndex(
                pushDataIndex + 1,
                instructions.Count - (pushDataIndex + 1),
                i => i.Op == LirOpcode.CONVERT &&
                     i.Immediate is { Length: 1 } payload &&
                     payload[0] == (byte)Neo.VM.Types.StackItemType.Buffer);
        }

        Assert.IsTrue(convertIndex >= 0, "Buffer literal must be converted to a Buffer stack item.");

        var setItemIndex = instructions.FindIndex(i => i.Op == LirOpcode.SETITEM);
        Assert.IsTrue(setItemIndex >= 0 && convertIndex < setItemIndex, "CONVERT Buffer should occur before SETITEM consumes the literal.");
    }

    [TestMethod]
    public void Scheduler_Emits_CheckedArithmetic_Guard()
    {
        var function = new VFunction("checked_add");
        var entry = new VBlock("entry");
        var fail = new VBlock("fail");
        function.Blocks.Add(entry);
        function.Blocks.Add(fail);

        var left = new VConstInt(new BigInteger(1));
        var right = new VConstInt(new BigInteger(2));
        entry.Nodes.Add(left);
        entry.Nodes.Add(right);

        var checkedAdd = new VCheckedArithmetic(VCheckedOp.Add, left, right, new LirIntType(32, IsSigned: true), VGuardFailKind.Branch, fail);
        entry.Nodes.Add(checkedAdd);
        entry.Terminator = new VRet(checkedAdd);

        fail.Terminator = new VAbort();

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        var entryInstructions = result.Function.Blocks.Single(b => b.Label == "entry").Instructions;

        var addIndex = entryInstructions.FindIndex(i => i.Op == LirOpcode.ADD);
        Assert.IsTrue(addIndex >= 0, "ADD should be emitted for the arithmetic portion of VCheckedArithmetic.");

        var withinIndex = entryInstructions.FindIndex(i => i.Op == LirOpcode.WITHIN);
        Assert.IsTrue(withinIndex > addIndex, "WITHIN should follow the arithmetic operation to enforce range checking.");

        var branchIndex = entryInstructions.FindIndex(i => i.Op == LirOpcode.JMPIFNOT);
        Assert.IsTrue(branchIndex > withinIndex, "JMPIFNOT should branch to the fail block when the range check fails.");

        var branchInst = entryInstructions[branchIndex];
        Assert.AreEqual("fail", branchInst.TargetLabel, "Checked arithmetic failure should branch to the designated fail block.");

        var failInstructions = result.Function.Blocks.Single(b => b.Label == "fail").Instructions;
        Assert.IsTrue(failInstructions.Any(i => i.Op == LirOpcode.ABORT), "Fail block should abort after overflow.");
    }

    [TestMethod]
    public void Scheduler_Emits_GuardNull_Abort_Path()
    {
        var function = new VFunction("guard_abort");
        var block = new VBlock("entry");
        function.Blocks.Add(block);

        var reference = new VConstByteString(new byte[] { 0x00 });
        block.Nodes.Add(reference);

        var guard = new VGuardNull(reference, VGuardFailKind.Abort, failTarget: null);
        block.Nodes.Add(guard);

        block.Terminator = new VRet(null);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        var instructions = result.Function.Blocks[0].Instructions;

        var isNullIndex = instructions.FindIndex(i => i.Op == LirOpcode.ISNULL);
        Assert.IsTrue(isNullIndex >= 0, "Guard should start with ISNULL.");

        var notIndex = instructions.FindIndex(i => i.Op == LirOpcode.NOT);
        Assert.IsTrue(notIndex > isNullIndex, "NOT should follow ISNULL for aborting guards.");

        var assertIndex = instructions.FindIndex(i => i.Op == LirOpcode.ASSERT);
        Assert.IsTrue(assertIndex > notIndex, "ASSERT should follow NOT in aborting guard path.");
    }

    [TestMethod]
    public void Scheduler_Orders_Call_Arguments_Correctly()
    {
        var function = new VFunction("call_args");
        var entry = new VBlock("entry");
        var callee = new VBlock("callee");
        function.Blocks.Add(entry);
        function.Blocks.Add(callee);

        var arg0 = new VConstInt(new BigInteger(42));
        var arg1 = new VConstBool(true);
        entry.Nodes.Add(arg0);
        entry.Nodes.Add(arg1);

        var call = new VCall("callee", new VNode[] { arg0, arg1 }, LirType.TInt, isPure: false);
        entry.Nodes.Add(call);
        entry.Terminator = new VRet(call);

        callee.Terminator = new VRet(null);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        var entryInstructions = result.Function.Blocks.Find(b => b.Label == "entry")!.Instructions;

        var intIndex = entryInstructions.FindIndex(i => i.Op == LirOpcode.PUSHINT);
        var boolIndex = entryInstructions.FindIndex(i => i.Op == LirOpcode.PUSHT);
        var callIndex = entryInstructions.FindIndex(i => i.Op == LirOpcode.CALL);

        Assert.IsTrue(intIndex >= 0, "Integer argument should be materialised via PUSHINT.");
        Assert.IsTrue(boolIndex >= 0, "Boolean argument should be materialised via PUSHT.");
        Assert.IsTrue(callIndex >= 0, "CALL instruction should be emitted.");
        Assert.IsTrue(intIndex < boolIndex, "Scheduler must preserve argument order (arg0 before arg1).");
        Assert.IsTrue(boolIndex < callIndex, "All arguments must be prepared before CALL.");
    }

    [TestMethod]
    public void Scheduler_Emits_Numeric_Opcodes()
    {
        var function = new VFunction("numeric");
        var block = new VBlock("entry");
        function.Blocks.Add(block);

        var negFour = new VConstInt(new BigInteger(-4));
        block.Nodes.Add(negFour);

        var abs = new VUnary(VUnaryOp.Abs, negFour, LirType.TInt);
        block.Nodes.Add(abs);

        var sign = new VUnary(VUnaryOp.Sign, abs, LirType.TInt);
        block.Nodes.Add(sign);

        var inc = new VUnary(VUnaryOp.Inc, sign, LirType.TInt);
        block.Nodes.Add(inc);

        var dec = new VUnary(VUnaryOp.Dec, inc, LirType.TInt);
        block.Nodes.Add(dec);

        var sqrt = new VUnary(VUnaryOp.Sqrt, dec, LirType.TInt);
        block.Nodes.Add(sqrt);

        var seven = new VConstInt(new BigInteger(7));
        block.Nodes.Add(seven);
        var max = new VBinary(VBinaryOp.Max, sqrt, seven, LirType.TInt);
        block.Nodes.Add(max);

        var two = new VConstInt(new BigInteger(2));
        block.Nodes.Add(two);
        var min = new VBinary(VBinaryOp.Min, max, two, LirType.TInt);
        block.Nodes.Add(min);

        var four = new VConstInt(new BigInteger(4));
        block.Nodes.Add(four);
        var pow = new VBinary(VBinaryOp.Pow, min, four, LirType.TInt);
        block.Nodes.Add(pow);

        var modulus = new VConstInt(new BigInteger(5));
        block.Nodes.Add(modulus);
        var modMul = new VModMul(pow, two, modulus, LirType.TInt);
        block.Nodes.Add(modMul);

        var three = new VConstInt(new BigInteger(3));
        block.Nodes.Add(three);
        var modPow = new VModPow(modMul, three, modulus, LirType.TInt);
        block.Nodes.Add(modPow);

        block.Terminator = new VRet(modPow);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        var instructions = result.Function.Blocks.Single().Instructions;

        int AbsIndex = instructions.FindIndex(i => i.Op == LirOpcode.ABS);
        int SignIndex = instructions.FindIndex(i => i.Op == LirOpcode.SIGN);
        int IncIndex = instructions.FindIndex(i => i.Op == LirOpcode.INC);
        int DecIndex = instructions.FindIndex(i => i.Op == LirOpcode.DEC);
        int SqrtIndex = instructions.FindIndex(i => i.Op == LirOpcode.SQRT);
        int MaxIndex = instructions.FindIndex(i => i.Op == LirOpcode.MAX);
        int MinIndex = instructions.FindIndex(i => i.Op == LirOpcode.MIN);
        int PowIndex = instructions.FindIndex(i => i.Op == LirOpcode.POW);
        int ModMulIndex = instructions.FindIndex(i => i.Op == LirOpcode.MODMUL);
        int ModPowIndex = instructions.FindIndex(i => i.Op == LirOpcode.MODPOW);

        Assert.IsTrue(AbsIndex >= 0, "ABS should be emitted.");
        Assert.IsTrue(SignIndex > AbsIndex, "SIGN should follow ABS.");
        Assert.IsTrue(IncIndex > SignIndex, "INC should follow SIGN.");
        Assert.IsTrue(DecIndex > IncIndex, "DEC should follow INC.");
        Assert.IsTrue(SqrtIndex > DecIndex, "SQRT should follow DEC.");
        Assert.IsTrue(MaxIndex > SqrtIndex, "MAX should follow SQRT.");
        Assert.IsTrue(MinIndex > MaxIndex, "MIN should follow MAX.");
        Assert.IsTrue(PowIndex > MinIndex, "POW should follow MIN.");
        Assert.IsTrue(ModMulIndex > PowIndex, "MODMUL should follow POW.");
        Assert.IsTrue(ModPowIndex > ModMulIndex, "MODPOW should follow MODMUL.");
    }

    [TestMethod]
    public void Scheduler_Emits_CompareBranch_Opcodes()
    {
        var function = new VFunction("cmp_branch");
        var entry = new VBlock("entry");
        var trueBlock = new VBlock("true");
        var falseBlock = new VBlock("false");
        function.Blocks.Add(entry);
        function.Blocks.Add(trueBlock);
        function.Blocks.Add(falseBlock);

        var left = new VConstInt(new BigInteger(5));
        var right = new VConstInt(new BigInteger(3));
        entry.Nodes.Add(left);
        entry.Nodes.Add(right);

        entry.Terminator = new VCompareBranch(VCompareOp.Gt, unsigned: false, left, right, trueBlock, falseBlock);
        trueBlock.Terminator = new VRet(null);
        falseBlock.Terminator = new VRet(null);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        var entryInstructions = result.Function.Blocks.Single(b => b.Label == "entry").Instructions;

        var cmpIndex = entryInstructions.FindIndex(i => i.Op == LirOpcode.JMPGT);
        Assert.IsTrue(cmpIndex >= 0, "Compare branch should emit JMPGT for greater-than condition.");

        var falseJumpIndex = entryInstructions.FindIndex(i => i.Op == LirOpcode.JMP && i.TargetLabel == "false");
        Assert.IsTrue(falseJumpIndex > cmpIndex, "False path should be reached via a trailing JMP.");
    }

    [TestMethod]
    public void Scheduler_Emits_PointerCall()
    {
        var function = new VFunction("pointer_call");
        var entry = new VBlock("entry");
        function.Blocks.Add(entry);

        var pointer = new VConstInt(new BigInteger(0x1234));
        var argument = new VConstInt(new BigInteger(7));
        entry.Nodes.Add(pointer);
        entry.Nodes.Add(argument);

        var pointerCall = new VPointerCall(pointer, new[] { argument }, LirType.TInt, isPure: false, isTailCall: false, callTableIndex: null);
        entry.Nodes.Add(pointerCall);
        entry.Terminator = new VRet(pointerCall);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        var instructions = result.Function.Blocks.Single(b => b.Label == "entry").Instructions;
        var callIndex = instructions.FindIndex(i => i.Op == LirOpcode.CALLA);
        Assert.IsTrue(callIndex >= 0, "CALLA should be emitted for pointer calls.");

        var callInst = instructions[callIndex];
        Assert.AreEqual(pointerCall.Arguments.Count + 1, callInst.PopOverride, "Pointer call should pop arguments plus the target pointer.");
    }

    [TestMethod]
    public void Scheduler_Emits_CallTableTailCall()
    {
        var function = new VFunction("calltable_call");
        var entry = new VBlock("entry");
        function.Blocks.Add(entry);

        var argument = new VConstInt(new BigInteger(42));
        entry.Nodes.Add(argument);

        var pointerCall = new VPointerCall(pointer: null, new[] { argument }, LirType.TInt, isPure: false, isTailCall: true, callTableIndex: 0);
        entry.Nodes.Add(pointerCall);
        entry.Terminator = new VRet(pointerCall);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        var instructions = result.Function.Blocks.Single(b => b.Label == "entry").Instructions;
        var callInst = instructions.Single(i => i.Op == LirOpcode.CALLT);

        Assert.AreEqual(pointerCall.Arguments.Count, callInst.PopOverride, "CALLT should pop only the arguments when no pointer operand is supplied.");
        Assert.IsNotNull(callInst.Immediate, "CALLT should encode the call-table index as an immediate operand.");
        Assert.AreEqual(1, callInst.Immediate!.Length, "CALLT immediate should be one byte.");
    }

    [TestMethod]
    public void Scheduler_Emits_Try_Finally_Instructions()
    {
        var function = new VFunction("try_finally");
        var entry = new VBlock("entry");
        var tryBlock = new VBlock("try_body");
        var finallyBlock = new VBlock("try_finally");
        var mergeBlock = new VBlock("try_merge");
        function.Blocks.Add(entry);
        function.Blocks.Add(tryBlock);
        function.Blocks.Add(finallyBlock);
        function.Blocks.Add(mergeBlock);

        var tryScope = new VTry(tryBlock, finallyBlock, mergeBlock, Array.Empty<VBlock>());
        entry.Nodes.Add(tryScope);
        entry.Terminator = new VJmp(tryBlock);

        tryBlock.Terminator = new VLeave(tryScope, mergeBlock);

        finallyBlock.Nodes.Add(new VFinally(tryScope));
        finallyBlock.Terminator = new VEndFinally(tryScope, mergeBlock);

        mergeBlock.Terminator = new VRet(null);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        var entryInstructions = result.Function.Blocks.Single(b => b.Label == "entry").Instructions;
        var tryInst = entryInstructions.Single(i => i.Op == LirOpcode.TRY_L);
        Assert.IsNull(tryInst.TargetLabel, "TRY_L catch target should encode as zero when no catch block is present.");
        Assert.AreEqual(finallyBlock.Label, tryInst.TargetLabel2, "TRY_L should reference the finally block label.");

        var tryInstructions = result.Function.Blocks.Single(b => b.Label == "try_body").Instructions;
        var endTry = tryInstructions.Single(i => i.Op == LirOpcode.ENDTRY_L);
        Assert.AreEqual(mergeBlock.Label, endTry.TargetLabel, "ENDTRY_L should jump to the merge block.");

        var finallyInstructions = result.Function.Blocks.Single(b => b.Label == "try_finally").Instructions;
        Assert.IsTrue(finallyInstructions.Any(i => i.Op == LirOpcode.ENDFINALLY), "Finally block should emit ENDFINALLY.");
    }

    [TestMethod]
    public void Scheduler_Handles_TryFinally_With_Loop_Control()
    {
        const string source = @"
public sealed class C
{
    public static int M(int value)
    {
        var result = value;
        while (value < 3)
        {
            try
            {
                if (value == 0)
                    continue;
                if (value == 1)
                    break;

                result = value;
            }
            finally
            {
            }
        }

        return result;
    }
}
";

        var hirFunction = HirLoweringTestBase.LowerMethod(source);
        var module = new MirModule();
        var lowerer = new HirToMirLowerer();
        var mirFunction = lowerer.Lower(hirFunction, module);

        var selector = new InstructionSelector();
        var vFunction = selector.Select(mirFunction);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(vFunction);

        var instructions = result.Function.Blocks.SelectMany(b => b.Instructions).ToList();

        Assert.IsTrue(instructions.Any(i => i.Op == LirOpcode.TRY_L), "Scheduled code should include TRY_L instruction.");
        Assert.IsTrue(instructions.Any(i => i.Op == LirOpcode.ENDFINALLY), "Scheduled code should include ENDFINALLY instruction.");

        var endTryTargets = instructions
            .Where(i => i.Op == LirOpcode.ENDTRY_L && i.TargetLabel is not null)
            .Select(i => i.TargetLabel!)
            .ToArray();

        Assert.IsTrue(endTryTargets.Length >= 2, "Expected ENDTRY_L targets for both break and continue paths.");
        Assert.IsTrue(endTryTargets.Any(label => label.StartsWith("while_cond", StringComparison.Ordinal)), "Continue path should jump back to the loop condition block.");
        Assert.IsTrue(endTryTargets.Any(label => label.StartsWith("while_exit", StringComparison.Ordinal)), "Break path should jump to the loop exit block.");
    }

    [TestMethod]
    public void Scheduler_Encodes_Syscall_Id_LittleEndian()
    {
        var function = new VFunction("syscall_encoding");
        var block = new VBlock("entry");
        function.Blocks.Add(block);

        var argument = new VConstInt(new BigInteger(7));
        block.Nodes.Add(argument);

        var syscall = new VSyscall(0x01020304u, new VNode[] { argument }, LirType.TInt);
        block.Nodes.Add(syscall);

        block.Terminator = new VRet(syscall);

        var scheduler = new StackScheduler();
        var result = scheduler.Lower(function);

        var instruction = result.Function.Blocks.Single().Instructions.Single(i => i.Op == LirOpcode.SYSCALL);
        var expected = new byte[] { 0x04, 0x03, 0x02, 0x01 };
        CollectionAssert.AreEqual(expected, instruction.Immediate, "Syscall identifier must be emitted in little-endian order.");
    }

    [TestMethod]
    public void Scheduler_Rejects_Tail_PointerCall_With_Dynamic_Pointer()
    {
        var function = new VFunction("tail_pointer_invalid");
        var block = new VBlock("entry");
        function.Blocks.Add(block);

        var pointer = new VConstByteString(new byte[] { 0x42 });
        block.Nodes.Add(pointer);

        var pointerCall = new VPointerCall(pointer, Array.Empty<VNode>(), LirType.TVoid, isPure: false, isTailCall: true, callTableIndex: null);
        block.Nodes.Add(pointerCall);

        block.Terminator = new VRet(null);

        var scheduler = new StackScheduler();

        Assert.ThrowsException<NotSupportedException>(() => scheduler.Lower(function));
    }
}
