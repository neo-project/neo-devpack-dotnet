using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ProjectEvaluation
{
    [TestMethod]
    public void GetCompilation_HonorsEvaluatedDefineConstants()
    {
        using var project = TempContractProject.Create("""
  <PropertyGroup Condition="'$(Configuration)' == 'Debug'">
    <DefineConstants>$(DefineConstants);CONTRACT_ENABLED</DefineConstants>
  </PropertyGroup>
""");
        project.WriteSource("Contract.cs", """
using Neo.SmartContract.Framework;

#if CONTRACT_ENABLED
public class Contract : SmartContract
{
    public static int Main() => 1;
}
#else
public class DisabledContract
{
}
#endif
""");

        var compilation = new CompilationEngine(new CompilationOptions()).GetCompilation(project.ProjectFile);

        Assert.IsNotNull(compilation.GetTypeByMetadataName("Contract"));
        Assert.IsNull(compilation.GetTypeByMetadataName("DisabledContract"));
    }

    [TestMethod]
    public void GetCompilation_HonorsDisabledDefaultCompileItemsAndGlobInclude()
    {
        using var project = TempContractProject.Create("""
  <PropertyGroup>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Contracts/**/*.cs" />
  </ItemGroup>
""");
        project.WriteSource(Path.Combine("Contracts", "Contract.cs"), """
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static int Main() => 1;
}
""");
        project.WriteSource("Excluded.cs", """
public class Excluded
{
}
""");

        var compilation = new CompilationEngine(new CompilationOptions()).GetCompilation(project.ProjectFile);
        var sourceFileNames = compilation.SyntaxTrees.Select(tree => Path.GetFileName(tree.FilePath)).ToArray();

        CollectionAssert.AreEqual(new[] { "Contract.cs" }, sourceFileNames);
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

        public static TempContractProject Create(string extraProjectItems)
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
{{extraProjectItems}}
</Project>
""");

            return new TempContractProject(tempFolder, projectFile);
        }

        public void WriteSource(string name, string source)
        {
            var path = Path.Combine(ProjectDirectory, name);
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(path, source);
        }

        public void Dispose()
        {
            Directory.Delete(ProjectDirectory, recursive: true);
        }
    }
}
