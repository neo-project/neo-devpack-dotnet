// Copyright (C) 2015-2026 The Neo Project.
//
// BasicOptimizerTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.Optimizer;
using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class BasicOptimizerTests
    {
        [TestMethod]
        public void RemoveNops_KeepsTargetedTrailingNop()
        {
            Instruction trailingNop = new() { OpCode = OpCode.NOP };
            JumpTarget jumpTarget = new() { Instruction = trailingNop };
            Instruction jump = new() { OpCode = OpCode.JMP, Target = jumpTarget };
            List<Instruction> instructions = [jump, trailingNop];

            BasicOptimizer.RemoveNops(instructions);

            Assert.AreEqual(2, instructions.Count);
            Assert.AreSame(trailingNop, instructions[1]);
            Assert.AreSame(trailingNop, jumpTarget.Instruction);
        }

        [TestMethod]
        public void RemoveNops_RetargetsBranchesToNextLiveInstruction()
        {
            Instruction removed1 = new() { OpCode = OpCode.NOP };
            Instruction removed2 = new() { OpCode = OpCode.NOP };
            Instruction next = new() { OpCode = OpCode.RET };
            JumpTarget target1 = new() { Instruction = removed1 };
            JumpTarget target2 = new() { Instruction = removed2 };
            Instruction jump = new() { OpCode = OpCode.JMP, Target = target1 };
            Instruction tryInstruction = new() { OpCode = OpCode.TRY_L, Target = target2, Target2 = target1 };
            List<Instruction> instructions = [jump, tryInstruction, removed1, removed2, next];

            BasicOptimizer.RemoveNops(instructions);

            CollectionAssert.AreEqual(new[] { jump, tryInstruction, next }, instructions);
            Assert.AreSame(next, target1.Instruction);
            Assert.AreSame(next, target2.Instruction);
            Assert.AreSame(next, tryInstruction.Target2?.Instruction);
        }
    }
}
