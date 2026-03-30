using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.SmartContract.Testing;
using Neo.VM.Types;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_BigIntegerTryParse
{
    [TestMethod]
    public void BigIntegerTryParse_ReturnsTrue_And_AssignsParsedValue()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;
using System.Numerics;

public class Contract : SmartContract
{
    [DisplayName(""test"")]
    public static (bool, BigInteger) Test(string s)
    {
        bool success = BigInteger.TryParse(s, out BigInteger result);
        return (success, result);
    }
}";

        var context = CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<BigIntegerTryParseContract>(context.CreateExecutable(), context.CreateManifest());

        var result = contract.Test("123");
        Assert.IsNotNull(result);
        var tuple = result!;

        bool success = tuple[0] switch
        {
            StackItem stackItem => stackItem.GetBoolean(),
            bool value => value,
            _ => throw new AssertFailedException($"Unexpected success result type: {tuple[0]?.GetType().Name ?? "null"}")
        };

        System.Numerics.BigInteger parsed = tuple[1] switch
        {
            System.Numerics.BigInteger value => value,
            StackItem stackItem => stackItem.GetInteger(),
            _ => throw new AssertFailedException($"Unexpected parsed value type: {tuple[1]?.GetType().Name ?? "null"}")
        };

        Assert.IsTrue(success, "BigInteger.TryParse should report success for a valid integer string.");
        Assert.AreEqual((System.Numerics.BigInteger)123, parsed);
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

    public abstract class BigIntegerTryParseContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("test")]
        public abstract object?[]? Test(string s);
    }
}
