using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_CompilationEngineReuse
{
    [TestMethod]
    public void CompileProject_ReusedEngineCompilesEachProjectSources()
    {
        using var firstProject = TempContractProject.Create("FirstContract", 1);
        using var secondProject = TempContractProject.Create("SecondContract", 2);
        var engine = CreateEngine();

        var firstResult = engine.CompileProject(firstProject.ProjectFile).Single();
        var secondResult = engine.CompileProject(secondProject.ProjectFile).Single();

        Assert.AreEqual("FirstContract", firstResult.ContractName);
        Assert.AreEqual("SecondContract", secondResult.ContractName);
    }

    [TestMethod]
    public void PrepareProjectContracts_ReusedEnginePreparesEachProjectSources()
    {
        using var firstProject = TempContractProject.Create("FirstContract", 1);
        using var secondProject = TempContractProject.Create("SecondContract", 2);
        var engine = CreateEngine();

        var (firstClasses, _, _) = engine.PrepareProjectContracts(firstProject.ProjectFile);
        var (secondClasses, _, _) = engine.PrepareProjectContracts(secondProject.ProjectFile);

        CollectionAssert.AreEqual(new[] { "FirstContract" }, firstClasses.Select(p => p.Name).ToArray());
        CollectionAssert.AreEqual(new[] { "SecondContract" }, secondClasses.Select(p => p.Name).ToArray());
    }

    [TestMethod]
    public void CompileProject_PreparedOverloadRejectsDifferentPreparedProject()
    {
        using var firstProject = TempContractProject.Create("FirstContract", 1);
        using var secondProject = TempContractProject.Create("SecondContract", 2);
        var engine = CreateEngine();

        var (firstClasses, firstDependencies, firstSymbols) = engine.PrepareProjectContracts(firstProject.ProjectFile);
        var (secondClasses, secondDependencies, secondSymbols) = engine.PrepareProjectContracts(secondProject.ProjectFile);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            engine.CompileProject(firstProject.ProjectFile, firstClasses, firstDependencies, firstSymbols));

        StringAssert.Contains(exception.Message, firstProject.ProjectFile);
        StringAssert.Contains(exception.Message, nameof(CompilationEngine.PrepareProjectContracts));

        var secondResult = engine.CompileProject(secondProject.ProjectFile, secondClasses, secondDependencies, secondSymbols).Single();
        Assert.AreEqual("SecondContract", secondResult.ContractName);
    }

    [TestMethod]
    public void CompileSources_ReusedEngineRefreshesTemporaryProject()
    {
        using var firstProject = TempContractProject.Create("FirstContract", 1);
        using var secondProject = TempContractProject.Create("SecondContract", 2);
        var engine = CreateEngine();

        var firstResult = engine.CompileSources(firstProject.SourceFile).Single();
        var secondResult = engine.CompileSources(secondProject.SourceFile).Single();

        Assert.AreEqual("FirstContract", firstResult.ContractName);
        Assert.AreEqual("SecondContract", secondResult.ContractName);
    }

    private static CompilationEngine CreateEngine() => new(new CompilationOptions
    {
        SkipRestoreIfAssetsPresent = true
    });

    private sealed class TempContractProject : IDisposable
    {
        private readonly string _projectDirectory;

        public string ProjectFile { get; }
        public string SourceFile { get; }

        private TempContractProject(string projectDirectory, string projectFile, string sourceFile)
        {
            _projectDirectory = projectDirectory;
            ProjectFile = projectFile;
            SourceFile = sourceFile;
        }

        public static TempContractProject Create(string contractName, int returnValue)
        {
            var projectDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(projectDirectory);
            var projectFile = Path.Combine(projectDirectory, "ContractProject.csproj");
            var sourceFile = Path.Combine(projectDirectory, $"{contractName}.cs");
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
            File.WriteAllText(sourceFile, $$"""
using Neo.SmartContract.Framework;

public class {{contractName}} : SmartContract
{
    public static int Main() => {{returnValue}};
}
""");

            return new TempContractProject(projectDirectory, projectFile, sourceFile);
        }

        public void Dispose()
        {
            Directory.Delete(_projectDirectory, recursive: true);
        }
    }
}
