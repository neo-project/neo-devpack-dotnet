using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.Extensions;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_CallSemantics
{
    [TestMethod]
    public void EmptyInstanceMethodCall_EvaluatesReceiverAndArgumentSideEffects()
    {
        const string source = @"using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    private static int _instanceCounter;
    private static int _argCounter;

    private class Holder
    {
        public void Touch(int ignored)
        {
        }
    }

    private static Holder NewHolder()
    {
        _instanceCounter = 10;
        return new Holder();
    }

    public static int Main()
    {
        _instanceCounter = 0;
        _argCounter = 0;
        NewHolder().Touch(_argCounter = 7);
        return _instanceCounter * 100 + _argCounter;
    }
}";

        var context = CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var nef = context.CreateExecutable();
        var manifest = context.CreateManifest();
        var methodName = manifest.Abi.Methods.Single(m => m.Name.Equals("Main", StringComparison.OrdinalIgnoreCase)).Name;

        var engine = new TestEngine(true);
        var state = engine.Native.ContractManagement.Deploy(
            nef.ToArray(),
            Encoding.UTF8.GetBytes(manifest.ToJson().ToString(false)))
            ?? throw new AssertFailedException("Contract deployment returned null state.");

        using var script = new ScriptBuilder();
        script.EmitDynamicCall(state.Hash, methodName);

        Assert.AreEqual(new BigInteger(1007), engine.Execute(script.ToArray()).GetInteger());
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
}
