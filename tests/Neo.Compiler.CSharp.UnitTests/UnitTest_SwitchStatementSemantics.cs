using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_SwitchStatementSemantics
{
    [TestMethod]
    public void Switch_DefaultFirst_EvaluatesCasesBeforeDefaultJump()
    {
        const string source = @"using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static int Main(int x)
    {
        switch (x)
        {
            default:
                return 99;
            case 1:
                return 1;
        }
    }
}";

        var context = CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var methodBlock = ExtractMethodBlock(context.CreateAssembly(), "Contract.Main(int)");
        var instructionLines = methodBlock
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(line => line.Contains(':'))
            .ToArray();

        var stlocIndex = Array.FindIndex(instructionLines, line => line.Contains("STLOC 0", StringComparison.Ordinal));
        Assert.IsTrue(stlocIndex >= 0, "Expected switch method to store governing expression in local slot.");
        Assert.IsTrue(stlocIndex + 1 < instructionLines.Length, "Expected instructions after the switch value storage.");
        StringAssert.Contains(instructionLines[stlocIndex + 1], "LDLOC 0", "Switch should begin case evaluation before emitting default fallback jump.");
    }

    private static CompilationContext CompileSingleContract(string sourceCode)
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(tempFile, sourceCode);

        try
        {
            var options = new CompilationOptions
            {
                Optimize = CompilationOptions.OptimizationType.None,
                Nullable = NullableContextOptions.Enable,
                SkipRestoreIfAssetsPresent = true
            };

            var engine = new CompilationEngine(options);
            var repoRoot = SyntaxProbeLoader.GetRepositoryRoot();
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

    private static string ExtractMethodBlock(string assembly, string methodSignature)
    {
        var normalized = assembly.Replace("\r\n", "\n", StringComparison.Ordinal);
        var marker = $"// {methodSignature}";
        var start = normalized.IndexOf(marker, StringComparison.Ordinal);
        Assert.IsTrue(start >= 0, $"Method section '{methodSignature}' was not found in generated assembly.\n{assembly}");

        var next = normalized.IndexOf("\n// ", start + marker.Length, StringComparison.Ordinal);
        if (next < 0) next = normalized.Length;

        return normalized[start..next];
    }
}
