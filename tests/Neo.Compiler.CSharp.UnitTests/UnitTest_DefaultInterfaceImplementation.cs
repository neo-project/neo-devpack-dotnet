using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_DefaultInterfaceImplementation
{
    [TestMethod]
    public void Contract_Exports_And_Uses_Interface_Default_Implementation()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public interface IDefaultGreeting
{
    [DisplayName(""greet"")]
    string Greet()
    {
        return ""hello"";
    }
}

public class Contract : SmartContract, IDefaultGreeting
{
}";

        var context = CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var manifest = context.CreateManifest();
        Assert.IsNotNull(manifest.Abi.GetMethod("greet", 0));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<DefaultGreetingContract>(context.CreateExecutable(), manifest);

        Assert.AreEqual("hello", contract.Greet());
    }

    [TestMethod]
    public void Contract_Override_Wins_Over_Interface_Default_Implementation()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public interface IDefaultGreeting
{
    [DisplayName(""greet"")]
    string Greet()
    {
        return ""hello"";
    }
}

public class Contract : SmartContract, IDefaultGreeting
{
    [DisplayName(""greet"")]
    public string Greet()
    {
        return ""override"";
    }
}";

        var context = CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var manifest = context.CreateManifest();
        var greetMethods = manifest.Abi.Methods.Where(m => m.Name == "greet").ToArray();
        Assert.AreEqual(1, greetMethods.Length);

        var engine = new TestEngine(true);
        var contract = engine.Deploy<DefaultGreetingContract>(context.CreateExecutable(), manifest);

        Assert.AreEqual("override", contract.Greet());
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

    public abstract class DefaultGreetingContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("greet")]
        public abstract string? Greet();
    }
}
