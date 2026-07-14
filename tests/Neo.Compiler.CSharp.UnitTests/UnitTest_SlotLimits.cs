using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.SmartContract;
using Neo.VM;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_SlotLimits
{
    [TestMethod]
    public void Methods_WithMoreThan255Parameters_FailCompilationCleanly()
    {
        var context = TestHelper.CompileSingleContract(BuildParameterOverflowSource(256));
        Assert.IsFalse(context.Success, "Compilation should fail cleanly when parameter count exceeds the VM slot limit.");
        StringAssert.Contains(string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())), "255 parameters");
    }

    [TestMethod]
    public void Methods_WithMoreThan255Locals_FailCompilationCleanly()
    {
        var context = TestHelper.CompileSingleContract(BuildLocalOverflowSource(256));
        Assert.IsFalse(context.Success, "Compilation should fail cleanly when local count exceeds the VM slot limit.");
        StringAssert.Contains(string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())), "255 local");
    }

    [TestMethod]
    public void Contracts_With255StaticSlots_CompileSuccessfully()
    {
        var context = TestHelper.CompileSingleContract(BuildStaticSlotSource(255));
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var initialize = context.CreateManifest().Abi.GetMethod("_initialize", 0);
        Assert.IsNotNull(initialize);
        var instruction = ((Script)context.CreateExecutable().Script).GetInstruction(initialize.Offset);
        Assert.AreEqual(OpCode.INITSSLOT, instruction.OpCode);
        Assert.AreEqual(byte.MaxValue, instruction.Operand.Span[0]);
    }

    [TestMethod]
    public void Contracts_WithMoreThan255StaticSlots_FailCompilationCleanly()
    {
        var context = TestHelper.CompileSingleContract(BuildStaticSlotSource(256));
        Assert.IsFalse(context.Success, "Compilation should fail cleanly when the static slot count exceeds the VM limit.");
        StringAssert.Contains(string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())), "255 static slots");
    }

    [TestMethod]
    public void MethodTokens_Accept128UniqueTokensAndReject129th()
    {
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static void Main() { }
            }
            """);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        for (var i = 0; i < 128; i++)
        {
            Assert.AreEqual((ushort)i, context.AddMethodToken(UInt160.Zero, $"method{i}", 0, false, CallFlags.All));
        }

        Assert.AreEqual((ushort)0, context.AddMethodToken(UInt160.Zero, "method0", 0, false, CallFlags.All));
        Assert.AreEqual(128, context.CreateExecutable().Tokens.Length);

        var exception = Assert.ThrowsException<CompilationException>(
            () => context.AddMethodToken(UInt160.Zero, "method128", 0, false, CallFlags.All));
        StringAssert.Contains(exception.Message, "limit(128) exceeded");
    }

    private static string BuildParameterOverflowSource(int parameterCount)
    {
        var parameters = string.Join(", ", Enumerable.Range(0, parameterCount).Select(i => $"int p{i}"));
        return $$"""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static int Main({{parameters}})
    {
        return 0;
    }
}
""";
    }

    private static string BuildLocalOverflowSource(int localCount)
    {
        var body = new StringBuilder();
        for (var i = 0; i < localCount; i++)
        {
            body.Append("        int v").Append(i).Append(" = ").Append(i).AppendLine(";");
        }

        return $$"""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static int Main()
    {
{{body}}        return 0;
    }
}
""";
    }

    private static string BuildStaticSlotSource(int staticSlotCount)
    {
        var fields = new StringBuilder();
        var body = new StringBuilder("        int sum = 0;\n");
        for (var i = 0; i < staticSlotCount; i++)
        {
            fields.Append("    private static int Field").Append(i).AppendLine(";");
            body.Append("        sum += Field").Append(i).AppendLine(";");
        }

        return $$"""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
{{fields}}
    public static int Main()
    {
{{body}}        return sum;
    }
}
""";
    }
}
