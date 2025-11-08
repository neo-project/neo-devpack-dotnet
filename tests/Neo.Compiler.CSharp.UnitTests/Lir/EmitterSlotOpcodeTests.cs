using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.LIR;
using Neo.Compiler.LIR.Backend;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests.Lir;

[TestClass]
public sealed class EmitterSlotOpcodeTests
{
    [TestMethod]
    public void NeoEmitter_Encodes_SlotOpcodes()
    {
        var function = new LirFunction("slot_ops");
        var block = new LirBlock("entry");
        function.Blocks.Add(block);

        block.Instructions.Add(new LirInst(LirOpcode.INITSSLOT) { Immediate = new byte[] { 0x03 } });
        block.Instructions.Add(new LirInst(LirOpcode.INITSLOT) { Immediate = new byte[] { 0x02, 0x01 } });
        block.Instructions.Add(new LirInst(LirOpcode.LDARG) { Immediate = new byte[] { 0x02 } });   // short form
        block.Instructions.Add(new LirInst(LirOpcode.LDARG) { Immediate = new byte[] { 0x0A } });   // general form
        block.Instructions.Add(new LirInst(LirOpcode.STARG) { Immediate = new byte[] { 0x00 } });   // short form
        block.Instructions.Add(new LirInst(LirOpcode.STARG) { Immediate = new byte[] { 0x09 } });   // general form
        block.Instructions.Add(new LirInst(LirOpcode.LDLOC) { Immediate = new byte[] { 0x01 } });   // short form
        block.Instructions.Add(new LirInst(LirOpcode.LDLOC) { Immediate = new byte[] { 0x10 } });   // general form
        block.Instructions.Add(new LirInst(LirOpcode.STLOC) { Immediate = new byte[] { 0x03 } });   // short form
        block.Instructions.Add(new LirInst(LirOpcode.STLOC) { Immediate = new byte[] { 0x12 } });   // general form

        var emitter = new NeoEmitter();
        var result = emitter.Emit(function, assumedMaxStack: 0);

        var expected = new List<byte>
        {
            (byte)OpCode.INITSSLOT, 0x03,
            (byte)OpCode.INITSLOT, 0x02, 0x01,
            (byte)OpCode.LDARG2,
            (byte)OpCode.LDARG, 0x0A,
            (byte)OpCode.STARG0,
            (byte)OpCode.STARG, 0x09,
            (byte)OpCode.LDLOC1,
            (byte)OpCode.LDLOC, 0x10,
            (byte)OpCode.STLOC3,
            (byte)OpCode.STLOC, 0x12
        };

        CollectionAssert.AreEqual(expected.ToArray(), result.Code, "Emitted bytecode did not match expected slot opcodes.");
    }

    [TestMethod]
    public void NeoEmitter_Encodes_CompareJump()
    {
        var function = new LirFunction("cmp");
        var entry = new LirBlock("entry");
        var trueBlock = new LirBlock("true");
        var falseBlock = new LirBlock("false");
        function.Blocks.Add(entry);
        function.Blocks.Add(trueBlock);
        function.Blocks.Add(falseBlock);

        entry.Instructions.Add(new LirInst(LirOpcode.PUSHINT) { Immediate = new BigInteger(1).ToByteArray() });
        entry.Instructions.Add(new LirInst(LirOpcode.PUSHINT) { Immediate = new BigInteger(0).ToByteArray() });
        entry.Instructions.Add(new LirInst(LirOpcode.JMPEQ) { TargetLabel = "true" });
        entry.Instructions.Add(new LirInst(LirOpcode.JMP) { TargetLabel = "false" });

        trueBlock.Instructions.Add(new LirInst(LirOpcode.RET));
        falseBlock.Instructions.Add(new LirInst(LirOpcode.RET));

        var emitter = new NeoEmitter();
        var result = emitter.Emit(function, assumedMaxStack: 2);

        CollectionAssert.Contains(result.Code, (byte)OpCode.JMPEQ_L, "JMPEQ should encode to the long NeoVM form.");
    }

    [TestMethod]
    public void NeoEmitter_Encodes_PointerCall()
    {
        var function = new LirFunction("pointer_call");
        var entry = new LirBlock("entry");
        function.Blocks.Add(entry);

        entry.Instructions.Add(new LirInst(LirOpcode.PUSHINT) { Immediate = new BigInteger(0x1234).ToByteArray() });
        entry.Instructions.Add(new LirInst(LirOpcode.CALLA)
        {
            PopOverride = 1,
            PushOverride = 0
        });

        var emitter = new NeoEmitter();
        var result = emitter.Emit(function, assumedMaxStack: 1);

        CollectionAssert.Contains(result.Code, (byte)OpCode.CALLA, "CALLA should be emitted for pointer calls.");
    }

    [TestMethod]
    public void NeoEmitter_Encodes_CallTablePointerCall()
    {
        var function = new LirFunction("calltable_call");
        var entry = new LirBlock("entry");
        function.Blocks.Add(entry);

        entry.Instructions.Add(new LirInst(LirOpcode.PUSHINT) { Immediate = new BigInteger(7).ToByteArray() });
        entry.Instructions.Add(new LirInst(LirOpcode.CALLT)
        {
            PopOverride = 1,
            PushOverride = 1,
            Immediate = new byte[] { 0x2A }
        });

        var emitter = new NeoEmitter();
        var result = emitter.Emit(function, assumedMaxStack: 2);

        CollectionAssert.Contains(result.Code, (byte)OpCode.CALLT, "CALLT should be emitted for call-table pointer calls.");
        var callIndex = Array.IndexOf(result.Code, (byte)OpCode.CALLT);
        Assert.IsTrue(callIndex >= 0 && callIndex < result.Code.Length - 1, "CALLT opcode should have a trailing immediate byte.");
        Assert.AreEqual(0x2A, result.Code[callIndex + 1], "CALLT immediate should encode the call-table index.");
    }

    [TestMethod]
    public void NeoEmitter_Encodes_Try_Finally()
    {
        var function = new LirFunction("try_finally");
        var entry = new LirBlock("entry");
        var tryBlock = new LirBlock("try_body");
        var finallyBlock = new LirBlock("try_finally");
        var mergeBlock = new LirBlock("try_merge");
        function.Blocks.Add(entry);
        function.Blocks.Add(tryBlock);
        function.Blocks.Add(finallyBlock);
        function.Blocks.Add(mergeBlock);

        entry.Instructions.Add(new LirInst(LirOpcode.TRY_L)
        {
            TargetLabel = null,
            TargetLabel2 = finallyBlock.Label
        });
        entry.Instructions.Add(new LirInst(LirOpcode.JMP) { TargetLabel = tryBlock.Label });

        tryBlock.Instructions.Add(new LirInst(LirOpcode.ENDTRY_L)
        {
            TargetLabel = mergeBlock.Label
        });

        finallyBlock.Instructions.Add(new LirInst(LirOpcode.ENDFINALLY));
        mergeBlock.Instructions.Add(new LirInst(LirOpcode.RET));

        var emitter = new NeoEmitter();
        var result = emitter.Emit(function, assumedMaxStack: 1);

        Assert.IsTrue(Array.IndexOf(result.Code, (byte)OpCode.TRY_L) >= 0, "Encoded script should include TRY_L.");
        Assert.IsTrue(Array.IndexOf(result.Code, (byte)OpCode.ENDTRY_L) >= 0, "Encoded script should include ENDTRY_L.");
        Assert.IsTrue(Array.IndexOf(result.Code, (byte)OpCode.ENDFINALLY) >= 0, "Encoded script should include ENDFINALLY.");

        var tryIndex = Array.IndexOf(result.Code, (byte)OpCode.TRY_L);
        Assert.IsTrue(tryIndex >= 0 && tryIndex <= result.Code.Length - 9, "TRY_L should reserve space for two 4-byte offsets.");
        var catchOffset = BitConverter.ToInt32(result.Code, tryIndex + 1);
        Assert.AreEqual(0, catchOffset, "TRY_L catch offset should be zero when no catch block is present.");
    }
}
