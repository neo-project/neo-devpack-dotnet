using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Utilities.ProjectCreation;
using Neo.BuildTasks;
using Xunit;
using Xunit.Abstractions;

namespace build_tasks
{
    public partial class TestBuild : MSBuildTestBase
    {
        readonly ITestOutputHelper output;

        public TestBuild(ITestOutputHelper output)
        {
            this.output = output;
        }

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
            test_BuildContract(source);
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
            test_BuildContract(source);
        }

        void test_BuildContract(string source, string sourceName = "contract.cs")
        {
            using var testRootPath = new TestRootPath();
            InstallNccs(testRootPath);

            var sourcePath = Path.Combine(testRootPath, sourceName);
            File.WriteAllText(sourcePath, source);

            var creator = CreateDotNetSixProject(testRootPath)
                .Property("NeoContractName", "$(AssemblyName)")
                .ImportNeoBuildTools()
                .ItemPackageReference("Neo.SmartContract.Framework", version: "3.3.0")
                .AssertBuild(output);
        }

        [Fact]
        public void can_generate_contract_from_NeoContractGeneration()
        {
            using var testRootPath = new TestRootPath();
            var manifest = TestFiles.GetString("registrar.manifest");
            test_NeoContractGeneration(testRootPath, manifest);
        }

        [Fact]
        public void can_generate_contract_from_NeoContractGeneration_with_ContractNameOverride()
        {
            using var testRootPath = new TestRootPath();
            var manifest = TestFiles.GetString("registrar.manifest").Replace("DevHawk.Registrar", "DevHawk Registrar");
            test_NeoContractGeneration(testRootPath, manifest, "Registrar");
        }

        void test_NeoContractGeneration(string testRootPath, string manifest, string contractNameOverride = "")
        {
            var manifestPath = Path.Combine(testRootPath, "contract.manifest.json");
            File.WriteAllText(manifestPath, manifest);
            var metadata = new Dictionary<string, string?>() { ["ManifestPath"] = manifestPath };
            if (!string.IsNullOrEmpty(contractNameOverride))
            {
                metadata.Add("ContractNameOverride", contractNameOverride);
            }

            var creator = CreateDotNetSixProject(testRootPath)
                .ImportNeoBuildTools()
                .ReferenceNeo()
                .ItemInclude("NeoContractGeneration", "registrar", metadata: metadata)
                .AssertBuild(output);

            var generatedCodePath = Path.Combine(testRootPath, "obj/Debug/net6.0/registrar.contract-interface.cs");
            Assert.True(File.Exists(generatedCodePath), "contract interface not generated");
        }

        [Fact]
        public void can_generate_contract_from_NeoContractReference()
        {
            var source = TestFiles.GetString("registrar.source");
            using var testRootPath = new TestRootPath();
            test_NeoContractReference(testRootPath, source);
        }

        [Fact]
        public void can_generate_contract_from_NeoContractReference_with_ContractNameOverride()
        {
            var source = TestFiles.GetString("registrar.source").Replace("DevHawk.Registrar", "DevHawk Registrar");
            using var testRootPath = new TestRootPath();
            test_NeoContractReference(testRootPath, source, "Registrar");
        }

        void test_NeoContractReference(string testRootPath, string source, string contractNameOverride = "")
        {
            InstallNccs(testRootPath);

            var srcDir = Path.Combine(testRootPath, "src");
            if (!Directory.Exists(srcDir)) Directory.CreateDirectory(srcDir);
            File.WriteAllText(Path.Combine(srcDir, "contract.cs"), source);

            var srcCreator = CreateDotNetSixProject(testRootPath, "src/registrar.csproj")
                .Property("NeoContractName", "$(AssemblyName)")
                .ImportNeoBuildTools()
                .ReferenceNeoScFx()
                .Save();

            var metadata = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(contractNameOverride))
            {
                metadata.Add("ContractNameOverride", contractNameOverride);
            }

            var testCreator = CreateDotNetSixProject(testRootPath, "test/registrarTests.csproj")
                .ImportNeoBuildTools()
                .ReferenceNeo()
                .ItemInclude("NeoContractReference", srcCreator.FullPath, metadata: metadata)
                .AssertBuild(output);

            var generatedCodePath = Path.Combine(testRootPath, "test/obj/Debug/net6.0/registrar.contract-interface.cs");
            Assert.True(File.Exists(generatedCodePath), "contract interface not generated");
        }

        public static ProjectCreator CreateDotNetSixProject(string directory, string projectName = "project.csproj")
        {
            return ProjectCreator.Templates.SdkCsproj(
                path: Path.Combine(directory, projectName),
                targetFramework: "net6.0");
        }

        static void InstallNccs(string path, string version = "3.3.0")
        {
            var runner = new ProcessRunner();
            runner.RunThrow("dotnet", "new tool-manifest", path);
            runner.RunThrow("dotnet", $"tool install neo.compiler.csharp --version {version}", path);
        }
    }
}
