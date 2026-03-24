// Copyright (C) 2015-2026 The Neo Project.
//
// CompressJumpsTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.Optimizer;
using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class CompressJumpsTests
    {
        [TestMethod]
        public void CompressJumps_CompressesForwardJumpThatBecomesShortAfterSelfShrink()
        {
            // Arrange: Build a forward long jump whose delta is just out of sbyte range (129),
            // but would become in-range once the jump itself compresses (delta - 3 = 126).
            Instruction target = new() { OpCode = OpCode.RET };
            JumpTarget jt = new() { Instruction = target };
            Instruction jump = new() { OpCode = OpCode.JMP_L, Target = jt };

            List<Instruction> instructions = [jump];
            for (int i = 0; i < 124; i++)
                instructions.Add(new Instruction { OpCode = OpCode.NOP });
            instructions.Add(target);

            instructions.RebuildOffsets();

            // Act
            BasicOptimizer.CompressJumps(instructions);

            // Assert
            Assert.AreEqual(OpCode.JMP, jump.OpCode);
            Assert.AreEqual(126, target.Offset);
        }

        [TestMethod]
        public void CompressJumps_CompressesBackwardJumpWithoutSelfShrinkAdjustment()
        {
            // Arrange: Build a backward long jump with an in-range negative delta. Backward jumps
            // must not apply the self-shrink adjustment that is only valid for forward targets.
            Instruction target = new() { OpCode = OpCode.PUSH0 };
            JumpTarget jt = new() { Instruction = target };

            List<Instruction> instructions = [target];
            for (int i = 0; i < 124; i++)
                instructions.Add(new Instruction { OpCode = OpCode.NOP });

            Instruction jump = new() { OpCode = OpCode.JMP_L, Target = jt };
            instructions.Add(jump);

            instructions.RebuildOffsets();

            // Act
            BasicOptimizer.CompressJumps(instructions);

            // Assert
            Assert.AreEqual(OpCode.JMP, jump.OpCode);
            Assert.AreEqual(125, jump.Offset);
            Assert.AreEqual(0, target.Offset);
        }
    }
}
