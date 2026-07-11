using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ContractIdentity
{
    [TestMethod]
    public void CompileProject_DistinguishesContractsWithSameSimpleName()
    {
        using var project = TempContractProject.Create();
        project.WriteSource("Contracts.cs", """
using Neo.SmartContract.Framework;
using System.ComponentModel;

namespace First
{
    public class SharedContract : SmartContract
    {
        [DisplayName("first")]
        public static int First() => 1;
    }
}

namespace Second
{
    public class SharedContract : SmartContract
    {
        [DisplayName("second")]
        public static int Second() => 2;
    }
}

namespace Third
{
    public class UniqueContract : SmartContract
    {
        [DisplayName("unique")]
        public static int Unique() => 3;
    }
}
""");

        var engine = new CompilationEngine(new CompilationOptions());
        var contexts = engine.CompileProject(project.ProjectFile);

        Assert.AreEqual(3, contexts.Count);
        var first = contexts.Single(context => context.TargetContract.ToDisplayString() == "First.SharedContract");
        var second = contexts.Single(context => context.TargetContract.ToDisplayString() == "Second.SharedContract");
        var unique = contexts.Single(context => context.TargetContract.ToDisplayString() == "Third.UniqueContract");
        Assert.IsTrue(first.Success, string.Join(Environment.NewLine, first.Diagnostics));
        Assert.IsTrue(second.Success, string.Join(Environment.NewLine, second.Diagnostics));
        Assert.IsTrue(unique.Success, string.Join(Environment.NewLine, unique.Diagnostics));
        CollectionAssert.AreEqual(new[] { "first" }, first.CreateManifest().Abi.Methods.Select(method => method.Name).ToArray());
        CollectionAssert.AreEqual(new[] { "second" }, second.CreateManifest().Abi.Methods.Select(method => method.Name).ToArray());
        CollectionAssert.AreEqual(new[] { "unique" }, unique.CreateManifest().Abi.Methods.Select(method => method.Name).ToArray());

        var (sortedClasses, classDependencies, allClassSymbols) = engine.PrepareProjectContracts(project.ProjectFile);
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            engine.CompileProject(project.ProjectFile, sortedClasses, classDependencies, allClassSymbols, "SharedContract"));
        StringAssert.Contains(exception.Message, "First.SharedContract");
        StringAssert.Contains(exception.Message, "Second.SharedContract");

        var selected = engine.CompileProject(
            project.ProjectFile,
            sortedClasses,
            classDependencies,
            allClassSymbols,
            "First.SharedContract").Single();
        Assert.AreEqual("First.SharedContract", selected.TargetContract.ToDisplayString());
        CollectionAssert.AreEqual(new[] { "first" }, selected.CreateManifest().Abi.Methods.Select(method => method.Name).ToArray());

        var selectedBySimpleName = engine.CompileProject(
            project.ProjectFile,
            sortedClasses,
            classDependencies,
            allClassSymbols,
            "UniqueContract").Single();
        Assert.AreEqual("Third.UniqueContract", selectedBySimpleName.TargetContract.ToDisplayString());
        CollectionAssert.AreEqual(new[] { "unique" }, selectedBySimpleName.CreateManifest().Abi.Methods.Select(method => method.Name).ToArray());
    }

    private sealed class TempContractProject : IDisposable
    {
        public string ProjectDirectory { get; }
        public string ProjectFile { get; }

        private TempContractProject(string projectDirectory, string projectFile)
        {
            ProjectDirectory = projectDirectory;
            ProjectFile = projectFile;
        }

        public static TempContractProject Create()
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempFolder);
            var projectFile = Path.Combine(tempFolder, "ContractProject.csproj");
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

            return new TempContractProject(tempFolder, projectFile);
        }

        public void WriteSource(string name, string source)
        {
            File.WriteAllText(Path.Combine(ProjectDirectory, name), source);
        }

        public void Dispose()
        {
            Directory.Delete(ProjectDirectory, recursive: true);
        }
    }
}
