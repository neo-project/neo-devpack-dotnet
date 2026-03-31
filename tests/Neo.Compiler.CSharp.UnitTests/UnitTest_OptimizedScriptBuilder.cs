using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Optimizer;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_OptimizedScriptBuilder
{
    [TestMethod]
    public void BuildScriptWithJumpTargets_RetargetsDeletedEndTryTargetToNextLiveInstruction()
    {
        Neo.VM.Instruction endTry = new Script(new byte[] { (byte)OpCode.ENDTRY_L, 0, 0, 0, 0 }).GetInstruction(0);
        Neo.VM.Instruction deletedTarget = new Script(new byte[] { (byte)OpCode.NOP }).GetInstruction(0);
        Neo.VM.Instruction liveTarget = new Script(new byte[] { (byte)OpCode.RET }).GetInstruction(0);

        OrderedDictionary simplifiedInstructionsToAddress = new()
        {
            { endTry, 0 },
            { liveTarget, endTry.Size }
        };

        Dictionary<Neo.VM.Instruction, Neo.VM.Instruction> jumpSourceToTargets = new()
        {
            [endTry] = deletedTarget
        };

        Dictionary<int, Neo.VM.Instruction> oldAddressToInstruction = new()
        {
            [0] = endTry,
            [endTry.Size] = deletedTarget,
            [endTry.Size + deletedTarget.Size] = liveTarget
        };

        Script script = OptimizedScriptBuilder.BuildScriptWithJumpTargets(
            simplifiedInstructionsToAddress,
            jumpSourceToTargets,
            new Dictionary<Neo.VM.Instruction, (Neo.VM.Instruction, Neo.VM.Instruction)>(),
            oldAddressToInstruction);

        var rebuiltEndTry = script.GetInstruction(0);
        Assert.AreEqual(OpCode.ENDTRY_L, rebuiltEndTry.OpCode);
        Assert.AreEqual(endTry.Size, rebuiltEndTry.TokenI32);
    }

    [TestMethod]
    public void BuildScriptWithJumpTargets_SkipsDeletedInstructionsUntilNextLiveTarget()
    {
        Neo.VM.Instruction endTry = new Script(new byte[] { (byte)OpCode.ENDTRY_L, 0, 0, 0, 0 }).GetInstruction(0);
        Neo.VM.Instruction deletedTarget = new Script(new byte[] { (byte)OpCode.NOP }).GetInstruction(0);
        Neo.VM.Instruction deletedGap = new Script(new byte[] { (byte)OpCode.NOP }).GetInstruction(0);
        Neo.VM.Instruction liveTarget = new Script(new byte[] { (byte)OpCode.RET }).GetInstruction(0);

        OrderedDictionary simplifiedInstructionsToAddress = new()
        {
            { endTry, 0 },
            { liveTarget, endTry.Size }
        };

        Dictionary<Neo.VM.Instruction, Neo.VM.Instruction> jumpSourceToTargets = new()
        {
            [endTry] = deletedTarget
        };

        Dictionary<int, Neo.VM.Instruction> oldAddressToInstruction = new()
        {
            [0] = endTry,
            [endTry.Size] = deletedTarget,
            [endTry.Size + deletedTarget.Size] = deletedGap,
            [endTry.Size + deletedTarget.Size + deletedGap.Size] = liveTarget
        };

        Script script = OptimizedScriptBuilder.BuildScriptWithJumpTargets(
            simplifiedInstructionsToAddress,
            jumpSourceToTargets,
            new Dictionary<Neo.VM.Instruction, (Neo.VM.Instruction, Neo.VM.Instruction)>(),
            oldAddressToInstruction);

        var rebuiltEndTry = script.GetInstruction(0);
        Assert.AreEqual(OpCode.ENDTRY_L, rebuiltEndTry.OpCode);
        Assert.AreEqual(endTry.Size, rebuiltEndTry.TokenI32);
    }

    [TestMethod]
    public void BuildScriptWithJumpTargets_FallsBackToNextLiveInstructionAfterSourceWhenTargetTailIsDeleted()
    {
        Neo.VM.Instruction endTry = new Script(new byte[] { (byte)OpCode.ENDTRY_L, 0, 0, 0, 0 }).GetInstruction(0);
        Neo.VM.Instruction liveTarget = new Script(new byte[] { (byte)OpCode.RET }).GetInstruction(0);
        Neo.VM.Instruction deletedTarget = new Script(new byte[] { (byte)OpCode.NOP }).GetInstruction(0);

        OrderedDictionary simplifiedInstructionsToAddress = new()
        {
            { endTry, 0 },
            { liveTarget, endTry.Size }
        };

        Dictionary<Neo.VM.Instruction, Neo.VM.Instruction> jumpSourceToTargets = new()
        {
            [endTry] = deletedTarget
        };

        Dictionary<int, Neo.VM.Instruction> oldAddressToInstruction = new()
        {
            [0] = endTry,
            [endTry.Size] = liveTarget,
            [endTry.Size + liveTarget.Size] = deletedTarget
        };

        Script script = OptimizedScriptBuilder.BuildScriptWithJumpTargets(
            simplifiedInstructionsToAddress,
            jumpSourceToTargets,
            new Dictionary<Neo.VM.Instruction, (Neo.VM.Instruction, Neo.VM.Instruction)>(),
            oldAddressToInstruction);

        var rebuiltEndTry = script.GetInstruction(0);
        Assert.AreEqual(OpCode.ENDTRY_L, rebuiltEndTry.OpCode);
        Assert.AreEqual(endTry.Size, rebuiltEndTry.TokenI32);
    }

    [TestMethod]
    public void BuildScriptWithJumpTargets_ThrowsWhenDeletedEndTryTargetCannotBeResolvedWithoutOldMap()
    {
        Neo.VM.Instruction endTry = new Script(new byte[] { (byte)OpCode.ENDTRY_L, 0, 0, 0, 0 }).GetInstruction(0);
        Neo.VM.Instruction deletedTarget = new Script(new byte[] { (byte)OpCode.NOP }).GetInstruction(0);

        OrderedDictionary simplifiedInstructionsToAddress = new()
        {
            { endTry, 0 }
        };

        Dictionary<Neo.VM.Instruction, Neo.VM.Instruction> jumpSourceToTargets = new()
        {
            [endTry] = deletedTarget
        };

        var ex = Assert.ThrowsException<BadScriptException>(() => OptimizedScriptBuilder.BuildScriptWithJumpTargets(
            simplifiedInstructionsToAddress,
            jumpSourceToTargets,
            new Dictionary<Neo.VM.Instruction, (Neo.VM.Instruction, Neo.VM.Instruction)>(),
            oldAddressToInstruction: null));

        StringAssert.Contains(ex.Message, "ENDTRY");
        StringAssert.Contains(ex.Message, "deleted");
    }

    [TestMethod]
    public void BuildScriptWithJumpTargets_ThrowsForDeletedTargetOnUnsupportedOpcode()
    {
        Neo.VM.Instruction jump = new Script(new byte[] { (byte)OpCode.JMP_L, 0, 0, 0, 0 }).GetInstruction(0);
        Neo.VM.Instruction deletedTarget = new Script(new byte[] { (byte)OpCode.NOP }).GetInstruction(0);

        OrderedDictionary simplifiedInstructionsToAddress = new()
        {
            { jump, 0 }
        };

        Dictionary<Neo.VM.Instruction, Neo.VM.Instruction> jumpSourceToTargets = new()
        {
            [jump] = deletedTarget
        };

        Dictionary<int, Neo.VM.Instruction> oldAddressToInstruction = new()
        {
            [0] = jump,
            [jump.Size] = deletedTarget
        };

        var ex = Assert.ThrowsException<BadScriptException>(() => OptimizedScriptBuilder.BuildScriptWithJumpTargets(
            simplifiedInstructionsToAddress,
            jumpSourceToTargets,
            new Dictionary<Neo.VM.Instruction, (Neo.VM.Instruction, Neo.VM.Instruction)>(),
            oldAddressToInstruction));

        StringAssert.Contains(ex.Message, "JMP_L");
        StringAssert.Contains(ex.Message, "deleted");
    }
}
