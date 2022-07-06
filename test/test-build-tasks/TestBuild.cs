using System.IO;
using Microsoft.Build.Utilities.ProjectCreation;
using Neo.BuildTasks;
using Xunit;

namespace build_tasks
{
    public partial class TestBuild : MSBuildTestBase
    {
        [Fact]
        public void can_build_contract_that_calls_assert_with_message()
        {
            const string source = @"
using Neo.SmartContract.Framework;

namespace BuildToolsTestClasses
{
    public class TestContract : SmartContract
    {
        public static void TestAssert() { ExecutionEngine.Assert(false, ""message""); }
    }
}";
            TestBuildContract(source);
        }

        [Fact]
        public void can_build_TokenContract()
        {
            const string source = @"
using Neo.SmartContract.Framework;

    public class TestContract : TokenContract
    {
        public override byte Decimals() => 0;
        public override string Symbol() => ""TEST"";
    }
";
            TestBuildContract(source);
        }

        static void TestBuildContract(string source, string sourceName = "contract.cs")
        {
            using var testRootPath = new TestRootPath();
            InstallNccs(testRootPath);

            var sourcePath = Path.Combine(testRootPath, sourceName);
            File.WriteAllText(sourcePath, source);

            var creator = CreateContractProject(testRootPath);
            creator.TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

            Assert.True(result, string.Join('\n', buildOutput.Errors));
        }

        static ProjectCreator CreateContractProject(string rootPath, string projectName = "test.csproj")
        {
            return ProjectCreator.Templates.SdkCsproj(
                path: Path.Combine(rootPath, projectName),
                targetFramework: "net6.0")
                .Property("NeoContractName", "$(AssemblyName)")
                .ImportNeoBuildTools()
                .ItemPackageReference("Neo.SmartContract.Framework", version: "3.3.0");
        }

        static void InstallNccs(string path, string version = "3.3.0")
        {
            var runner = new ProcessRunner();
            runner.RunThrow("dotnet", "new tool-manifest", path);
            runner.RunThrow("dotnet", $"tool install neo.compiler.csharp --version {version}", path);
        }
    }
}
