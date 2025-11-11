// Copyright (C) 2015-2025 The Neo Project.
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
using Neo.Compiler;
using Neo.Compiler.Optimizer;
using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer;

[TestClass]
public class BasicOptimizerTests
{
    private static Instruction CreateConvertInstruction(Neo.VM.Types.StackItemType target) =>
        new()
        {
            OpCode = OpCode.CONVERT,
            Operand = new[] { (byte)target }
        };

    [TestMethod]
    public void RemoveRedundantConversions_RemovesConsecutiveDuplicates()
    {
        var instructions = new List<Instruction>
        {
            CreateConvertInstruction(Neo.VM.Types.StackItemType.ByteString),
            CreateConvertInstruction(Neo.VM.Types.StackItemType.ByteString),
            new Instruction { OpCode = OpCode.RET }
        };

        BasicOptimizer.RemoveRedundantConversions(instructions);

        Assert.AreEqual(2, instructions.Count);
        Assert.AreEqual(OpCode.CONVERT, instructions[0].OpCode);
        Assert.AreEqual(OpCode.RET, instructions[1].OpCode);
    }

    [TestMethod]
    public void RemoveRedundantConversions_PreservesDistinctTargets()
    {
        var instructions = new List<Instruction>
        {
            CreateConvertInstruction(Neo.VM.Types.StackItemType.ByteString),
            CreateConvertInstruction(Neo.VM.Types.StackItemType.Buffer),
            new Instruction { OpCode = OpCode.RET }
        };

        BasicOptimizer.RemoveRedundantConversions(instructions);

        Assert.AreEqual(3, instructions.Count);
        CollectionAssert.AreEqual(
            new[] { OpCode.CONVERT, OpCode.CONVERT, OpCode.RET },
            instructions.ConvertAll(i => i.OpCode));
    }
}
