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
    public void BuildScriptWithJumpTargets_ThrowsWhenEndTryTargetIsDeleted()
    {
        Neo.VM.Instruction endTry = new Script(new byte[] { (byte)OpCode.ENDTRY_L, 0, 0, 0, 0 }).GetInstruction(0);
        Neo.VM.Instruction deletedTarget = new Script(new byte[] { (byte)OpCode.RET }).GetInstruction(0);

        OrderedDictionary simplifiedInstructionsToAddress = new()
        {
            { endTry, 0 }
        };

        Dictionary<Neo.VM.Instruction, Neo.VM.Instruction> jumpSourceToTargets = new()
        {
            [endTry] = deletedTarget
        };

        Dictionary<int, Neo.VM.Instruction> oldAddressToInstruction = new()
        {
            [0] = endTry,
            [1] = deletedTarget
        };

        var ex = Assert.ThrowsException<BadScriptException>(() => OptimizedScriptBuilder.BuildScriptWithJumpTargets(
            simplifiedInstructionsToAddress,
            jumpSourceToTargets,
            new Dictionary<Neo.VM.Instruction, (Neo.VM.Instruction, Neo.VM.Instruction)>(),
            oldAddressToInstruction));

        StringAssert.Contains(ex.Message, "ENDTRY");
        StringAssert.Contains(ex.Message, "deleted");
    }
}
