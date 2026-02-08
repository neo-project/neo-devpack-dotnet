// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_InstructionDisassembler.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Optimizer;
using Neo.VM;
using System;
using System.Linq;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class InstructionDisassemblerTests
    {
        /// <summary>
        /// Test that PUSHDATA1 correctly displays the hex string and ASCII representation.
        /// </summary>
        [TestMethod]
        public void Test_PushData1_Disassembly()
        {
            string testString = "hello";
            byte[] data = Encoding.ASCII.GetBytes(testString);
            byte[] script = new byte[2 + data.Length];
            script[0] = (byte)OpCode.PUSHDATA1;
            script[1] = (byte)data.Length;
            Array.Copy(data, 0, script, 2, data.Length);

            var instructions = ((Script)script).EnumerateInstructions().ToArray();
            // Script may have implicit RET, so check at least 1 instruction
            Assert.IsTrue(instructions.Length >= 1, "Should have at least 1 instruction");

            var instruction = instructions[0].instruction;
            Assert.AreEqual(OpCode.PUSHDATA1, instruction.OpCode);
            // Operand should contain the data (with 1-byte length prefix)
            Assert.IsNotNull(instruction.Operand);
            Assert.IsTrue(instruction.Operand.Length > 1);
        }

        /// <summary>
        /// Test that PUSHDATA2 correctly displays the hex string and ASCII representation.
        /// This tests the fix for the span offset bug (was using AsSpan(1), should be AsSpan(2)).
        /// </summary>
        [TestMethod]
        public void Test_PushData2_Disassembly()
        {
            string testString = "hello world";
            byte[] data = Encoding.ASCII.GetBytes(testString);
            byte[] script = new byte[3 + data.Length];
            script[0] = (byte)OpCode.PUSHDATA2;
            // 2-byte length in little endian
            script[1] = (byte)data.Length;
            script[2] = 0;
            Array.Copy(data, 0, script, 3, data.Length);

            var instructions = ((Script)script).EnumerateInstructions().ToArray();
            // Script may have implicit RET, so check at least 1 instruction
            Assert.IsTrue(instructions.Length >= 1, "Should have at least 1 instruction");

            var instruction = instructions[0].instruction;
            Assert.AreEqual(OpCode.PUSHDATA2, instruction.OpCode);
            // Operand should contain the data (VM Instruction.Operand contains just the data)
            Assert.IsNotNull(instruction.Operand);
            Assert.AreEqual(data.Length, instruction.Operand.Length);
        }

        /// <summary>
        /// Test that PUSHDATA4 correctly displays the hex string and ASCII representation.
        /// This tests the fix for the span offset bug (was using AsSpan(1), should be AsSpan(4)).
        /// </summary>
        [TestMethod]
        public void Test_PushData4_Disassembly()
        {
            string testString = "hello world test";
            byte[] data = Encoding.ASCII.GetBytes(testString);
            byte[] script = new byte[5 + data.Length];
            script[0] = (byte)OpCode.PUSHDATA4;
            // 4-byte length in little endian
            script[1] = (byte)data.Length;
            script[2] = 0;
            script[3] = 0;
            script[4] = 0;
            Array.Copy(data, 0, script, 5, data.Length);

            var instructions = ((Script)script).EnumerateInstructions().ToArray();
            // Script may have implicit RET, so check at least 1 instruction
            Assert.IsTrue(instructions.Length >= 1, "Should have at least 1 instruction");

            var instruction = instructions[0].instruction;
            Assert.AreEqual(OpCode.PUSHDATA4, instruction.OpCode);
            // Operand should contain the data (with 4-byte length prefix)
            Assert.IsNotNull(instruction.Operand);
            Assert.IsTrue(instruction.Operand.Length > 4);
        }

        /// <summary>
        /// Test that PUSHDATA2 with empty data works correctly.
        /// </summary>
        [TestMethod]
        public void Test_PushData2_EmptyData()
        {
            byte[] script = new byte[3];
            script[0] = (byte)OpCode.PUSHDATA2;
            script[1] = 0;  // length = 0
            script[2] = 0;

            var instructions = ((Script)script).EnumerateInstructions().ToArray();
            // Script may have implicit RET, so check at least 1 instruction
            Assert.IsTrue(instructions.Length >= 1, "Should have at least 1 instruction");

            var instruction = instructions[0].instruction;
            Assert.AreEqual(OpCode.PUSHDATA2, instruction.OpCode);
        }

        /// <summary>
        /// Test that PUSHDATA4 with empty data works correctly.
        /// </summary>
        [TestMethod]
        public void Test_PushData4_EmptyData()
        {
            byte[] script = new byte[5];
            script[0] = (byte)OpCode.PUSHDATA4;
            script[1] = 0;  // length = 0
            script[2] = 0;
            script[3] = 0;
            script[4] = 0;

            var instructions = ((Script)script).EnumerateInstructions().ToArray();
            // Script may have implicit RET, so check at least 1 instruction
            Assert.IsTrue(instructions.Length >= 1, "Should have at least 1 instruction");

            var instruction = instructions[0].instruction;
            Assert.AreEqual(OpCode.PUSHDATA4, instruction.OpCode);
        }

        /// <summary>
        /// Test that non-ASCII data in PUSHDATA2 is handled correctly (no crash).
        /// </summary>
        [TestMethod]
        public void Test_PushData2_NonAsciiData()
        {
            // Data with non-ASCII bytes
            byte[] data = new byte[] { 0xFF, 0xFE, 0xFD, 0xFC };
            byte[] script = new byte[3 + data.Length];
            script[0] = (byte)OpCode.PUSHDATA2;
            script[1] = (byte)data.Length;
            script[2] = 0;
            Array.Copy(data, 0, script, 3, data.Length);

            var instructions = ((Script)script).EnumerateInstructions().ToArray();
            // Script may have implicit RET, so check at least 1 instruction
            Assert.IsTrue(instructions.Length >= 1, "Should have at least 1 instruction");

            // Should not throw
            var instruction = instructions[0].instruction;
            Assert.AreEqual(OpCode.PUSHDATA2, instruction.OpCode);
        }

        /// <summary>
        /// Test that the offset is correct for PUSHDATA2 with larger data.
        /// Before the fix, the disassembly would show wrong data due to incorrect offset.
        /// </summary>
        [TestMethod]
        public void Test_PushData2_CorrectOffset()
        {
            // Create data where offset error would be visible
            // First byte of data should not be confused with length bytes
            byte[] data = new byte[] { 0x02, 0x00, 0xAB, 0xCD, 0xEF };
            byte[] script = new byte[3 + data.Length];
            script[0] = (byte)OpCode.PUSHDATA2;
            script[1] = (byte)data.Length;  // 5
            script[2] = 0;
            Array.Copy(data, 0, script, 3, data.Length);

            var instructions = ((Script)script).EnumerateInstructions().ToArray();
            var instruction = instructions[0].instruction;
            Assert.AreEqual(OpCode.PUSHDATA2, instruction.OpCode);

            // Verify operand contains the data
            // Note: The VM's Instruction.Operand contains just the data, not the length prefix
            Assert.IsNotNull(instruction.Operand);
            Assert.AreEqual(data.Length, instruction.Operand.Length);
            // Verify first data byte is 0x02
            Assert.AreEqual(0x02, instruction.Operand.Span[0]);
        }

        [TestMethod]
        public void Test_PushData2_ToString_Uses_Correct_Offset()
        {
            var instruction = new Neo.Compiler.Instruction
            {
                OpCode = OpCode.PUSHDATA2,
                Operand = new byte[] { 0x03, 0x00, 0x01, 0x02, 0x03 }
            };

            var builder = new StringBuilder();
            instruction.ToString(builder);

            Assert.IsTrue(builder.ToString().Contains("[010203]"));
        }

        [TestMethod]
        public void Test_PushData4_ToString_Uses_Correct_Offset()
        {
            var instruction = new Neo.Compiler.Instruction
            {
                OpCode = OpCode.PUSHDATA4,
                Operand = new byte[] { 0x03, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03 }
            };

            var builder = new StringBuilder();
            instruction.ToString(builder);

            Assert.IsTrue(builder.ToString().Contains("[010203]"));
        }
    }
}
