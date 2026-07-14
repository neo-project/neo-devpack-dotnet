using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_AnalyzerExecution
{
    [TestMethod]
    public void CompileProject_ReportsNeoAnalyzerErrorsBeforeLowering()
    {
        using var project = TempContractProject.Create("""
using Neo.SmartContract.Framework;
using System.Diagnostics;

public class Contract : SmartContract
{
    public static void Main()
    {
        Debug.WriteLine("unsupported");
    }
}
""");

        var result = CreateEngine().CompileProject(project.ProjectFile).Single();
        var diagnostics = result.Diagnostics.Where(item => item.Id == "NC4028").ToArray();

        Assert.IsFalse(result.Success);
        Assert.IsTrue(diagnostics.Length > 0);
        Assert.IsTrue(diagnostics.All(item => item.Severity == DiagnosticSeverity.Error));
        Assert.IsTrue(diagnostics.All(item => item.Location.SourceTree?.FilePath == project.SourceFile));
        Assert.IsTrue(diagnostics.Any(item => item.Location.GetLineSpan().StartLinePosition.Line + 1 == 8));
        Assert.IsFalse(result.Diagnostics.Any(item => item.Id == "NC1002"));
    }

    [TestMethod]
    public void CompileProject_PreservesSupportedContracts()
    {
        using var project = TempContractProject.Create("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static int Main(int value) => value + 1;
}
""");

        var result = CreateEngine().CompileProject(project.ProjectFile).Single();

        Assert.IsTrue(result.Success, string.Join(Environment.NewLine, result.Diagnostics));
        Assert.IsFalse(result.Diagnostics.Any(item => item.Id.StartsWith("NC4", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void CompileProject_ReportsAnalyzerWarningsWithoutBlockingOutput()
    {
        using var project = TempContractProject.Create("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static int Main(int value)
    {
        try
        {
            return value + 1;
        }
        catch (System.InvalidOperationException)
        {
            return 0;
        }
    }
}
""");

        var result = CreateEngine().CompileProject(project.ProjectFile).Single();
        var diagnostic = result.Diagnostics.Single(item => item.Id == "NC4027");

        Assert.IsTrue(result.Success, string.Join(Environment.NewLine, result.Diagnostics));
        Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.Severity);
    }

    [TestMethod]
    public void CompileProject_DoesNotChangeLibraryBehaviorUnlessEnabled()
    {
        using var project = TempContractProject.Create("""
using Neo.SmartContract.Framework;
using System.Diagnostics;

public class Contract : SmartContract
{
    public static void Main()
    {
        Debug.WriteLine("unsupported");
    }
}
""");

        var result = new CompilationEngine(new CompilationOptions
        {
            SkipRestoreIfAssetsPresent = true
        }).CompileProject(project.ProjectFile).Single();

        Assert.IsFalse(result.Success);
        Assert.IsFalse(result.Diagnostics.Any(item => item.Id == "NC4028"));
        Assert.IsTrue(result.Diagnostics.Any(item => item.Id == "NC1002"));
    }

    [TestMethod]
    public void ProgramMain_RunsNeoAnalyzersByDefault()
    {
        using var project = TempContractProject.Create("""
using Neo.SmartContract.Framework;
using System.Diagnostics;

public class Contract : SmartContract
{
    public static void Main()
    {
        Debug.WriteLine("unsupported");
    }
}
""");
        using var output = new StringWriter();
        using var error = new StringWriter();
        var originalOut = Console.Out;
        var originalError = Console.Error;
        int exitCode;

        try
        {
            Console.SetOut(output);
            Console.SetError(error);
            exitCode = Program.Main([project.ProjectFile]);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }

        Assert.AreEqual(1, exitCode);
        StringAssert.Contains(error.ToString(), "error NC4028");
        Assert.IsFalse(error.ToString().Contains("NC1002", StringComparison.Ordinal));
    }

    private static CompilationEngine CreateEngine() => new(new CompilationOptions
    {
        SkipRestoreIfAssetsPresent = true,
        RunAnalyzers = true
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

        public static TempContractProject Create(string source)
        {
            var projectDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(projectDirectory);
            var projectFile = Path.Combine(projectDirectory, "Contract.csproj");
            var sourceFile = Path.Combine(projectDirectory, "Contract.cs");
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
            File.WriteAllText(sourceFile, source);

            return new TempContractProject(projectDirectory, projectFile, sourceFile);
        }

        public void Dispose()
        {
            Directory.Delete(_projectDirectory, recursive: true);
        }
    }
}
