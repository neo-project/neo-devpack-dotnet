using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_CompilationEngineReference
{
    [TestMethod]
    public void CompileSources_ReturnsContractsInSourceOrder()
    {
        const string source = @"using Neo.SmartContract.Framework;

public class AlphaContract : SmartContract
{
    public static int Main() => 1;
}

public class BetaContract : SmartContract
{
    public static int Main() => 2;
}

public class GammaContract : SmartContract
{
    public static int Main() => 3;
}

public class DeltaContract : SmartContract
{
    public static int Main() => 4;
}";

        var contexts = CompileContracts(source);

        CollectionAssert.AreEqual(
            new[] { "AlphaContract", "BetaContract", "GammaContract", "DeltaContract" },
            contexts.Select(p => p.ContractName).ToArray());
    }

    [TestMethod]
    public void GetCompilation_ReportsRestoreFailureExitCode()
    {
        var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempFolder);
        var projectFile = Path.Combine(tempFolder, "BadRestore.csproj");
        File.WriteAllText(projectFile, """
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
""");

        try
        {
            var engine = new CompilationEngine(new CompilationOptions());
            var exception = Assert.ThrowsException<InvalidOperationException>(() => engine.GetCompilation(projectFile));

            StringAssert.Contains(exception.Message, "dotnet restore failed");
            StringAssert.Contains(exception.Message, "exit code");
            StringAssert.Contains(exception.Message, projectFile);
        }
        finally
        {
            Directory.Delete(tempFolder, recursive: true);
        }
    }

    [TestMethod]
    public void UnsupportedDependencyAssetTypeIncludesContext()
    {
        const string dependencyName = "bad.asset/1.0.0";
        const string assetType = "unsupported-kind";
        var engine = new CompilationEngine(new CompilationOptions());
        var assets = new JObject
        {
            ["libraries"] = new JObject
            {
                [dependencyName] = new JObject
                {
                    ["type"] = assetType
                }
            },
            ["project"] = new JObject
            {
                ["restore"] = new JObject
                {
                    ["packagesPath"] = Path.GetTempPath()
                }
            }
        };
        var method = typeof(CompilationEngine).GetMethod("GetReference", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(method);

        var exception = Assert.ThrowsException<TargetInvocationException>(() =>
            method.Invoke(engine, new object[]
            {
                dependencyName,
                new JObject(),
                assets,
                Path.GetTempPath(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            }));

        Assert.IsInstanceOfType(exception.InnerException, typeof(NotSupportedException));
        var innerException = (NotSupportedException)exception.InnerException!;
        StringAssert.Contains(innerException.Message, assetType);
        StringAssert.Contains(innerException.Message, dependencyName);
    }

    private static CompilationContext[] CompileContracts(string sourceCode)
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(tempFile, sourceCode);

        try
        {
            var engine = new CompilationEngine(new CompilationOptions
            {
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = NullableContextOptions.Enable,
                SkipRestoreIfAssetsPresent = true
            });
            var repoRoot = Syntax.SyntaxProbeLoader.GetRepositoryRoot();
            var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");

            return engine.CompileSources(new CompilationSourceReferences
            {
                Projects = new[] { frameworkProject }
            }, tempFile).ToArray();
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
