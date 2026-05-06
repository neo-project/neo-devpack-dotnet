using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Optimizer;
using Neo.VM;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_SwitchJumpTable
{
    [TestMethod]
    public void IntegerSwitch_UsesBranchTreeInsteadOfLinearEqualityChain()
    {
        const string source = @"using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static int Main(int x)
    {
        switch (x)
        {
            case 0: return 0;
            case 1: return 1;
            case 2: return 2;
            case 3: return 3;
            case 4: return 4;
            case 5: return 5;
            case 6: return 6;
            case 7: return 7;
            case 8: return 8;
            case 9: return 9;
            case 10: return 10;
            case 11: return 11;
            case 12: return 12;
            case 13: return 13;
            case 14: return 14;
            case 15: return 15;
            default: return -1;
        }
    }
}";

        AssertUsesOptimizedDispatch(source, "Contract.Main(int)");
    }

    [TestMethod]
    public void SupportedIntegralSwitchTypes_UseOptimizedDispatch()
    {
        AssertUsesOptimizedDispatch(BuildSwitchContract("byte", ["(byte)0", "(byte)1", "(byte)2", "(byte)3", "(byte)4", "(byte)5", "(byte)6", "(byte)7"]), "Contract.Main(byte)");
        AssertUsesOptimizedDispatch(BuildSwitchContract("sbyte", ["(sbyte)0", "(sbyte)1", "(sbyte)2", "(sbyte)3", "(sbyte)4", "(sbyte)5", "(sbyte)6", "(sbyte)7"]), "Contract.Main(sbyte)");
        AssertUsesOptimizedDispatch(BuildSwitchContract("short", ["(short)0", "(short)1", "(short)2", "(short)3", "(short)4", "(short)5", "(short)6", "(short)7"]), "Contract.Main(short)");
        AssertUsesOptimizedDispatch(BuildSwitchContract("ushort", ["(ushort)0", "(ushort)1", "(ushort)2", "(ushort)3", "(ushort)4", "(ushort)5", "(ushort)6", "(ushort)7"]), "Contract.Main(ushort)");
        AssertUsesOptimizedDispatch(BuildSwitchContract("uint", ["0U", "1U", "2U", "3U", "4U", "5U", "6U", "7U"]), "Contract.Main(uint)");
        AssertUsesOptimizedDispatch(BuildSwitchContract("long", ["0L", "1L", "2L", "3L", "4L", "5L", "6L", "7L"]), "Contract.Main(long)");
        AssertUsesOptimizedDispatch(BuildSwitchContract("ulong", ["0UL", "1UL", "2UL", "3UL", "4UL", "5UL", "6UL", "7UL"]), "Contract.Main(ulong)");
        AssertUsesOptimizedDispatch(BuildSwitchContract("char", ["'a'", "'b'", "'c'", "'d'", "'e'", "'f'", "'g'", "'h'"]), "Contract.Main(char)");
    }

    [TestMethod]
    public void IntegerSwitch_BelowOptimizationThreshold_UsesLinearEqualityChain()
    {
        var source = BuildSwitchContract("int", ["0", "1", "2", "3", "4", "5", "6"]);

        AssertUsesLinearEqualityDispatch(source, "Contract.Main(int)");
    }

    [TestMethod]
    public void TryGetIntegerConstant_SupportsBoolConstants_AndRejectsStrings()
    {
        var (boolModel, boolExpression) = CreateSemanticModel("class Test { const bool Value = true; }", "true");
        var (stringModel, stringExpression) = CreateSemanticModel("class Test { const string Value = \"neo\"; }", "\"neo\"");

        AssertTryGetIntegerConstant(boolModel, boolExpression, true, BigInteger.One);
        AssertTryGetIntegerConstant(stringModel, stringExpression, false, BigInteger.Zero);
    }

    [TestMethod]
    public void IntegerSwitch_WithoutDefault_StillUsesOptimizedDispatch()
    {
        const string source = @"using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static int Main(int x)
    {
        switch (x)
        {
            case 0: return 0;
            case 1: return 1;
            case 2: return 2;
            case 3: return 3;
            case 4: return 4;
            case 5: return 5;
            case 6: return 6;
            case 7: return 7;
        }

        return -1;
    }
}";

        AssertUsesOptimizedDispatch(source, "Contract.Main(int)");
    }

    private static string BuildSwitchContract(string inputType, string[] caseLabels)
    {
        var cases = string.Join(Environment.NewLine, caseLabels.Select((label, index) => $"            case {label}: return {index};"));
        return $@"using Neo.SmartContract.Framework;

public class Contract : SmartContract
{{
    public static int Main({inputType} x)
    {{
        switch (x)
        {{
{cases}
            default: return -1;
        }}
    }}
}}";
    }

    private static void AssertUsesOptimizedDispatch(string source, string methodId)
    {
        var opcodes = GetMethodOpcodes(source, methodId);

        CollectionAssert.DoesNotContain(opcodes, OpCode.EQUAL);
        Assert.IsTrue(opcodes.Any(IsRangeBranch),
            "Expected optimized switch dispatch to contain at least one range-branch opcode.");
    }

    private static void AssertUsesLinearEqualityDispatch(string source, string methodId)
    {
        var opcodes = GetMethodOpcodes(source, methodId);

        CollectionAssert.Contains(opcodes, OpCode.EQUAL);
        Assert.IsFalse(opcodes.Any(IsRangeBranch),
            "Expected fallback switch dispatch to use equality checks instead of range branches.");
    }

    private static OpCode[] GetMethodOpcodes(string source, string methodId)
    {
        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var nef = context.CreateExecutable();
        var debugInfo = context.CreateDebugInformation();
        var (start, end) = GetMethodRange(debugInfo, methodId);

        return ((Script)nef.Script)
            .EnumerateInstructions()
            .Where(i => i.address >= start && i.address <= end)
            .Select(i => i.instruction.OpCode)
            .ToArray();
    }

    private static bool IsRangeBranch(OpCode opcode) =>
        opcode == OpCode.JMPGT || opcode == OpCode.JMPGT_L ||
        opcode == OpCode.JMPGE || opcode == OpCode.JMPGE_L ||
        opcode == OpCode.JMPLT || opcode == OpCode.JMPLT_L ||
        opcode == OpCode.JMPLE || opcode == OpCode.JMPLE_L;

    private static void AssertTryGetIntegerConstant(
        SemanticModel model,
        ExpressionSyntax expression,
        bool expectedResult,
        BigInteger expectedValue)
    {
        var method = typeof(CompilationContext).Assembly.GetType("Neo.Compiler.MethodConvert")!
            .GetMethod("TryGetIntegerConstant", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.IsNotNull(method, "Unable to locate TryGetIntegerConstant.");

        object?[] parameters = [model, expression, null];
        var result = (bool)method.Invoke(null, parameters)!;

        Assert.AreEqual(expectedResult, result);
        Assert.AreEqual(expectedValue, parameters[2] is BigInteger value ? value : BigInteger.Zero);
    }

    private static (SemanticModel model, ExpressionSyntax expression) CreateSemanticModel(string source, string expressionText)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            assemblyName: $"SwitchJumpTable_{Guid.NewGuid():N}",
            syntaxTrees: [syntaxTree],
            references:
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            ]);

        var expression = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<ExpressionSyntax>()
            .First(node => node.ToString() == expressionText);

        return (compilation.GetSemanticModel(syntaxTree), expression);
    }

    private static (int start, int end) GetMethodRange(JObject debugInfo, string methodId)
    {
        var methods = (JArray)debugInfo["methods"]!;
        var method = methods
            .OfType<JObject>()
            .FirstOrDefault(m => string.Equals(m["id"]?.GetString(), methodId, StringComparison.Ordinal));

        Assert.IsNotNull(method, $"Unable to find method '{methodId}' in debug info.");

        var range = method["range"]!.GetString();
        var dashIndex = range.IndexOf('-', StringComparison.Ordinal);
        Assert.IsTrue(dashIndex > 0, "Method range should include a dash-delimited offset span.");

        var start = int.Parse(range[..dashIndex]);
        var end = int.Parse(range[(dashIndex + 1)..]);
        return (start, end);
    }
}
