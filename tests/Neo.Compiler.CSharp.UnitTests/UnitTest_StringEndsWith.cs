using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StringEndsWith
{
    [TestMethod]
    public void StringEndsWith_WholeStringSuffix_ReturnsTrue()
    {
        var contract = DeployContract();

        Assert.IsTrue(contract.WholeString());
        Assert.IsFalse(contract.Mismatch());
    }

    [TestMethod]
    public void StringEndsWith_EmptySuffix_ReturnsTrue()
    {
        var contract = DeployContract();

        Assert.IsTrue(contract.EmptySuffix());
    }

    private static StringEndsWithBoundaryContract DeployContract()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""wholeString"")]
    public static bool WholeString() => ""Hello"".EndsWith(""Hello"");

    [DisplayName(""emptySuffix"")]
    public static bool EmptySuffix() => ""Hello"".EndsWith("""");

    [DisplayName(""mismatch"")]
    public static bool Mismatch() => ""Hello"".EndsWith(""Hell"");
}";

        var context = CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        return engine.Deploy<StringEndsWithBoundaryContract>(context.CreateExecutable(), context.CreateManifest());
    }

    private static CompilationContext CompileSingleContract(string sourceCode)
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(tempFile, sourceCode);

        try
        {
            var options = new CompilationOptions
            {
                Optimize = CompilationOptions.OptimizationType.All,
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

    public abstract class StringEndsWithBoundaryContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("wholeString")]
        public abstract bool? WholeString();

        [DisplayName("emptySuffix")]
        public abstract bool? EmptySuffix();

        [DisplayName("mismatch")]
        public abstract bool? Mismatch();
    }
}
