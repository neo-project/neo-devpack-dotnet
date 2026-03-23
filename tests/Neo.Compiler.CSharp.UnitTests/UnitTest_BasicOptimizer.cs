// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_BasicOptimizer.cs file belongs to the neo project and is free
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

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_BasicOptimizer
    {
        [TestMethod]
        public void RemoveNops_RedirectsJumpsToNextInstruction()
        {
            JumpTarget target = new();
            Instruction jump = new() { OpCode = OpCode.JMP_L, Target = target };
            Instruction nop = new() { OpCode = OpCode.NOP };
            Instruction destination = new() { OpCode = OpCode.RET };
            target.Instruction = nop;

            List<Instruction> instructions = new() { jump, nop, destination };

            BasicOptimizer.RemoveNops(instructions);

            Assert.AreEqual(2, instructions.Count);
            Assert.AreSame(destination, target.Instruction);
            CollectionAssert.AreEqual(new[] { jump, destination }, instructions);
        }

        [TestMethod]
        public void RemoveNops_PreservesTerminalTargetedNop()
        {
            JumpTarget target = new();
            Instruction jump = new() { OpCode = OpCode.JMP_L, Target = target };
            Instruction terminalNop = new() { OpCode = OpCode.NOP };
            target.Instruction = terminalNop;

            List<Instruction> instructions = new() { jump, terminalNop };

            BasicOptimizer.RemoveNops(instructions);

            Assert.AreEqual(2, instructions.Count);
            Assert.AreSame(terminalNop, target.Instruction);
            CollectionAssert.AreEqual(new[] { jump, terminalNop }, instructions);
        }
    }
}
