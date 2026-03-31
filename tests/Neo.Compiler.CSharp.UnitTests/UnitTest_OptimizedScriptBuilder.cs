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
}
