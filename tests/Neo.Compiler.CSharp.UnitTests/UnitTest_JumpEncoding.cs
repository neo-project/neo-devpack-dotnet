using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Optimizer;
using Neo.VM;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_JumpEncoding
{
    [TestMethod]
    public void SmallConditional_UsesShortJumpEncoding_EvenWithoutOptimizations()
    {
        const string source = @"using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static int Main(int x)
    {
        if (x > 0)
            return 1;
        return 2;
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var opcodes = ((Script)context.CreateExecutable().Script)
            .EnumerateInstructions()
            .Select(tuple => tuple.instruction.OpCode)
            .ToArray();

        Assert.IsTrue(opcodes.Any(opcode =>
            opcode == OpCode.JMP ||
            opcode == OpCode.JMPIF ||
            opcode == OpCode.JMPIFNOT ||
            opcode == OpCode.JMP_L ||
            opcode == OpCode.JMPIF_L ||
            opcode == OpCode.JMPIFNOT_L),
            "Expected the compiled script to contain a branch instruction.");

        CollectionAssert.DoesNotContain(opcodes, OpCode.JMP_L);
        CollectionAssert.DoesNotContain(opcodes, OpCode.JMPIF_L);
        CollectionAssert.DoesNotContain(opcodes, OpCode.JMPIFNOT_L);
    }
}
