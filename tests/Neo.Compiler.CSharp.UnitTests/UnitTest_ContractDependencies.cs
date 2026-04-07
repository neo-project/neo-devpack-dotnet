using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ContractDependencies
{
    private const string SelfReferentialContractSource = """
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static Contract? Next { get; set; }
}
""";

    [TestMethod]
    public void CompileSources_SelfReferentialContract_CompilesSuccessfully()
    {
        var context = CompileSingleSource(SelfReferentialContractSource);

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
    }

    [TestMethod]
    public void PrepareProjectContracts_SelfReferentialContract_CompilesSuccessfully()
    {
        using var project = TemporaryProject.Create("SelfReferenceContract", SelfReferentialContractSource);

        var options = new CompilationOptions
        {
            Optimize = CompilationOptions.OptimizationType.All,
            Nullable = NullableContextOptions.Enable,
            SkipRestoreIfAssetsPresent = true
        };

        var engine = new CompilationEngine(options);
        var (sortedClasses, classDependencies, allClassSymbols) = engine.PrepareProjectContracts(project.ProjectPath);
        var context = engine.CompileProject(project.ProjectPath, sortedClasses, classDependencies, allClassSymbols, "Contract").Single();

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
    }

    private static CompilationContext CompileSingleSource(string sourceCode)
    {
        using var project = TemporaryProject.Create("SingleSourceContract", sourceCode);

        var options = new CompilationOptions
        {
            Optimize = CompilationOptions.OptimizationType.All,
            Nullable = NullableContextOptions.Enable,
            SkipRestoreIfAssetsPresent = true
        };

        var engine = new CompilationEngine(options);
        var references = new CompilationSourceReferences
        {
            Projects = new[] { project.FrameworkProjectPath }
        };

        var contexts = engine.CompileSources(references, project.SourcePath);
        Assert.AreEqual(1, contexts.Count, "Expected exactly one contract compilation context.");
        return contexts[0];
    }

    private sealed class TemporaryProject : IDisposable
    {
        private TemporaryProject(string directory, string projectPath, string sourcePath, string frameworkProjectPath)
        {
            Directory = directory;
            ProjectPath = projectPath;
            SourcePath = sourcePath;
            FrameworkProjectPath = frameworkProjectPath;
        }

        public string Directory { get; }

        public string ProjectPath { get; }

        public string SourcePath { get; }

        public string FrameworkProjectPath { get; }

        public static TemporaryProject Create(string projectName, string sourceCode)
        {
            var directory = Path.Combine(Path.GetTempPath(), "Neo.Compiler.UnitTests", Guid.NewGuid().ToString("N"));
            System.IO.Directory.CreateDirectory(directory);

            var repoRoot = SyntaxProbeLoader.GetRepositoryRoot();
            var frameworkProjectPath = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");
            var sourcePath = Path.Combine(directory, "Contract.cs");
            var projectPath = Path.Combine(directory, $"{projectName}.csproj");

            File.WriteAllText(sourcePath, sourceCode);
            File.WriteAllText(projectPath, $$"""
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>{{RuntimeAssemblyResolver.CompilerTargetFrameworkMoniker}}</TargetFramework>
    <LangVersion>preview</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="{{frameworkProjectPath}}" />
  </ItemGroup>
</Project>
""");

            return new TemporaryProject(directory, projectPath, sourcePath, frameworkProjectPath);
        }

        public void Dispose()
        {
            if (System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.Delete(Directory, true);
            }
        }
    }
}
