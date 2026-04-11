using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractsolution
{
    [TestClass]
    public class NeoContractSolutionTemplateFilesTests
    {
        private static readonly string TemplateRoot = Path.GetFullPath("../../../../../src/Neo.SmartContract.Template/templates/neocontractsolution");
        private static readonly string ContractProjectPath = Path.Combine(TemplateRoot, "NeoContractSolution", "NeoContractSolution.csproj");
        private static readonly string ToolManifestPath = Path.Combine(TemplateRoot, ".config", "dotnet-tools.json");
        private static readonly string UnitTestProjectPath = Path.Combine(TemplateRoot, "NeoContractSolution.UnitTests", "NeoContractSolution.UnitTests.csproj");
        private static readonly string UnitTestSourcePath = Path.Combine(TemplateRoot, "NeoContractSolution.UnitTests", "SmartContractTests.cs");

        [TestMethod]
        public void SolutionTemplateContractProjectGeneratesTestingArtifacts()
        {
            var content = File.ReadAllText(ContractProjectPath);

            StringAssert.Contains(content, "dotnet tool restore");
            StringAssert.Contains(content, "dotnet tool run nccs");
            StringAssert.Contains(content, "--generate-artifacts source");
        }

        [TestMethod]
        public void SolutionTemplateUnitTestsReferenceGeneratedContractArtifacts()
        {
            var content = File.ReadAllText(UnitTestProjectPath);

            StringAssert.Contains(content, @"..\NeoContractSolution\NeoContractSolution.csproj");
            StringAssert.Contains(content, @"Contract.artifacts.cs");
            StringAssert.Contains(content, "BeforeTargets=\"CoreCompile\"");
        }

        [TestMethod]
        public void SolutionTemplateIncludesLocalCompilerToolManifest()
        {
            Assert.IsTrue(File.Exists(ToolManifestPath), "The solution template should include a local dotnet tool manifest for nccs.");

            var content = File.ReadAllText(ToolManifestPath);

            StringAssert.Contains(content, "\"neo.compiler.csharp\"");
            StringAssert.Contains(content, "\"nccs\"");
        }

        [TestMethod]
        public void SolutionTemplateUnitTestsUseOwnableTestsPattern()
        {
            var content = File.ReadAllText(UnitTestSourcePath);

            StringAssert.Contains(content, "OwnableTests<");
            StringAssert.Contains(content, "using ContractArtifact = Neo.SmartContract.Testing.Contract;");
            StringAssert.Contains(content, "ContractArtifact.Nef");
            StringAssert.Contains(content, "ContractArtifact.Manifest");
            StringAssert.Contains(content, "Contract.MyMethod()");
            StringAssert.Contains(content, "Contract.Update(");
            StringAssert.Contains(content, "Assert.ThrowsException<TestException>");
        }
    }
}
