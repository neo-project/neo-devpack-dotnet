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
    public void PrepareProjectContracts_RecordsForwardContractDependencies()
    {
        var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempFolder);
        var projectFile = Path.Combine(tempFolder, "ForwardDependency.csproj");
        var sourceFile = Path.Combine(tempFolder, "ForwardDependency.cs");
        var repoRoot = Syntax.SyntaxProbeLoader.GetRepositoryRoot();
        var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");

        File.WriteAllText(projectFile, $$"""
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="{{frameworkProject}}" />
  </ItemGroup>
</Project>
""");

        File.WriteAllText(sourceFile, """
using Neo.SmartContract.Framework;

public class AlphaContract : SmartContract
{
    private BetaContract? _beta;

    public static int Main() => 1;
}

public class BetaContract : SmartContract
{
    public static int Main() => 2;
}
""");

        try
        {
            var engine = new CompilationEngine(new CompilationOptions
            {
                SkipRestoreIfAssetsPresent = true
            });

            var (_, classDependencies, _) = engine.PrepareProjectContracts(projectFile);
            var alpha = classDependencies.Keys.Single(p => p.Name == "AlphaContract");
            var beta = classDependencies.Keys.Single(p => p.Name == "BetaContract");

            Assert.IsTrue(classDependencies[alpha].Any(p => SymbolEqualityComparer.Default.Equals(p, beta)),
                "PrepareProjectContracts should record dependencies on contracts declared later in the project.");
        }
        finally
        {
            Directory.Delete(tempFolder, recursive: true);
        }
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
}
