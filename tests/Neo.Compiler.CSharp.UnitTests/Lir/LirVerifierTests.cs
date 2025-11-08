using System;
using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.LIR;

namespace Neo.Compiler.CSharp.UnitTests.Lir;

[TestClass]
public sealed class LirVerifierTests
{
    [TestMethod]
    public void Verifier_Fails_On_Invalid_PushData_Length()
    {
        var function = new LirFunction("payload_check");
        var block = new LirBlock("entry");
        function.Blocks.Add(block);

        var oversized = new byte[256];
        Array.Fill(oversized, (byte)0x01);

        block.Instructions.Add(new LirInst(LirOpcode.PUSHDATA1) { Immediate = oversized });

        var verifier = new LirVerifier();
        var result = verifier.Verify(function);

        Assert.IsFalse(result.Ok, "Verifier should reject payloads that exceed opcode limits.");
        Assert.IsTrue(result.Errors.Any(e => e.Contains("payload exceeds maximum")), "Expected payload length error.");
    }

    [TestMethod]
    public void Verifier_Fails_On_Unknown_Label_Target()
    {
        var function = new LirFunction("branch_check");
        var block = new LirBlock("entry");
        function.Blocks.Add(block);

        block.Instructions.Add(new LirInst(LirOpcode.PUSHINT) { Immediate = BigInteger.Zero.ToByteArray() });
        block.Instructions.Add(new LirInst(LirOpcode.JMPIF) { TargetLabel = "missing" });

        var verifier = new LirVerifier();
        var result = verifier.Verify(function);

        Assert.IsFalse(result.Ok, "Verifier should reject jumps targeting unknown labels.");
        Assert.AreEqual(1, result.Errors.Count);
        StringAssert.Contains(result.Errors[0], "unknown label");
    }

    [TestMethod]
    public void Verifier_Passes_On_Well_formed_Function()
    {
        var function = new LirFunction("happy_path");
        var entry = new LirBlock("entry");
        var exit = new LirBlock("exit");

        function.Blocks.Add(entry);
        function.Blocks.Add(exit);

        entry.Instructions.Add(new LirInst(LirOpcode.JMP) { TargetLabel = "exit" });

        exit.Instructions.Add(new LirInst(LirOpcode.RET));

        var verifier = new LirVerifier();
        var result = verifier.Verify(function);

        Assert.IsTrue(result.Ok, $"Unexpected verifier failure: {string.Join(", ", result.Errors)}");
    }

    [TestMethod]
    public void Verifier_Rejects_Block_Without_Terminator()
    {
        var function = new LirFunction("unterminated");
        var block = new LirBlock("entry");
        function.Blocks.Add(block);

        block.Instructions.Add(new LirInst(LirOpcode.PUSH0));

        var verifier = new LirVerifier();
        var result = verifier.Verify(function);

        Assert.IsFalse(result.Ok, "Verifier should fail when a block lacks a terminator.");
        Assert.AreEqual(1, result.Errors.Count);
        StringAssert.Contains(result.Errors[0], "does not end with a control-flow terminator");
    }
}
