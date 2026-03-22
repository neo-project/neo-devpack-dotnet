using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Optimizer;
using Neo.VM;
using System;
using System.IO;
using System.Linq;

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

        var context = CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var nef = context.CreateExecutable();
        var debugInfo = context.CreateDebugInformation();
        var (start, end) = GetMethodRange(debugInfo, "Contract.Main(int)");

        var opcodes = ((Script)nef.Script)
            .EnumerateInstructions()
            .Where(i => i.address >= start && i.address <= end)
            .Select(i => i.instruction.OpCode)
            .ToArray();

        // The naive lowering emits `EQUAL` + `JMPIF` for each case label. When optimized, we should
        // see range-branch opcodes (JMPLT/JMPGE/etc) and avoid a linear chain of EQUALs.
        CollectionAssert.DoesNotContain(opcodes, OpCode.EQUAL);

        Assert.IsTrue(opcodes.Any(opcode =>
                opcode == OpCode.JMPGT || opcode == OpCode.JMPGT_L ||
                opcode == OpCode.JMPGE || opcode == OpCode.JMPGE_L ||
                opcode == OpCode.JMPLT || opcode == OpCode.JMPLT_L ||
                opcode == OpCode.JMPLE || opcode == OpCode.JMPLE_L),
            "Expected optimized switch dispatch to contain at least one range-branch opcode.");
    }

    private static CompilationContext CompileSingleContract(string sourceCode)
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(tempFile, sourceCode);

        try
        {
            var options = new CompilationOptions
            {
                Optimize = CompilationOptions.OptimizationType.Basic,
                Nullable = NullableContextOptions.Enable,
                SkipRestoreIfAssetsPresent = true
            };

            var engine = new CompilationEngine(options);
            var repoRoot = Syntax.SyntaxProbeLoader.GetRepositoryRoot();
            var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");

            var contexts = engine.CompileSources(new CompilationSourceReferences
            {
                Projects = new[] { frameworkProject }
            }, tempFile);

            Assert.AreEqual(1, contexts.Count, "Expected exactly one contract compilation context.");
            return contexts[0];
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
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

