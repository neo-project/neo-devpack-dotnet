using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.Syntax;
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
}
