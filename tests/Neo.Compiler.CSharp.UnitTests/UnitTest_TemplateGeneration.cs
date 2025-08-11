using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_TemplateGeneration
    {
        private string _testDirectory;

        [TestInitialize]
        public void TestSetup()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), $"nccs_test_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [TestMethod]
        public void Test_NewCommand_BasicTemplate()
        {
            // Arrange
            var contractName = "TestBasicContract";
            var outputPath = Path.Combine(_testDirectory, contractName);

            // Act
            var args = new[] { "new", contractName, "--template", "basic", "--output", outputPath };
            var result = Program.Main(args);

            // Assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(Directory.Exists(outputPath));
            Assert.IsTrue(File.Exists(Path.Combine(outputPath, $"{contractName}.cs")));
            Assert.IsTrue(File.Exists(Path.Combine(outputPath, $"{contractName}.csproj")));

            // Verify contract content
            var contractContent = File.ReadAllText(Path.Combine(outputPath, $"{contractName}.cs"));
            Assert.IsTrue(contractContent.Contains($"public class {contractName} : SmartContract"));
            Assert.IsTrue(contractContent.Contains("HelloWorld"));
            Assert.IsTrue(contractContent.Contains("GetOwner"));
        }

        [TestMethod]
        public void Test_NewCommand_Nep17Template()
        {
            // Arrange
            var contractName = "TestToken";
            var outputPath = Path.Combine(_testDirectory, contractName);

            // Act
            var args = new[] { "new", contractName, "--template", "nep17", "--output", outputPath };
            var result = Program.Main(args);

            // Assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(Directory.Exists(outputPath));

            var contractContent = File.ReadAllText(Path.Combine(outputPath, $"{contractName}.cs"));
            Assert.IsTrue(contractContent.Contains($"public class {contractName} : Nep17Token"));
            Assert.IsTrue(contractContent.Contains("[SupportedStandards(NepStandard.Nep17)]"));
            Assert.IsTrue(contractContent.Contains("public override string Symbol"));
            Assert.IsTrue(contractContent.Contains("public override byte Decimals"));
            Assert.IsTrue(contractContent.Contains("Mint"));
            Assert.IsTrue(contractContent.Contains("Burn"));
        }

        [TestMethod]
        public void Test_NewCommand_NftTemplate()
        {
            // Arrange
            var contractName = "TestNFT";
            var outputPath = Path.Combine(_testDirectory, contractName);

            // Act
            var args = new[] { "new", contractName, "--template", "nft", "--output", outputPath };
            var result = Program.Main(args);

            // Assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(Directory.Exists(outputPath));

            var contractContent = File.ReadAllText(Path.Combine(outputPath, $"{contractName}.cs"));
            Assert.IsTrue(contractContent.Contains($"public class {contractName} : Nep11Token<{contractName}State>"));
            Assert.IsTrue(contractContent.Contains("[SupportedStandards(NepStandard.Nep11)]"));
            Assert.IsTrue(contractContent.Contains($"public class {contractName}State : Nep11TokenState"));
        }

        [TestMethod]
        public void Test_NewCommand_OracleTemplate()
        {
            // Arrange
            var contractName = "TestOracle";
            var outputPath = Path.Combine(_testDirectory, contractName);

            // Act
            var args = new[] { "new", contractName, "--template", "oracle", "--output", outputPath };
            var result = Program.Main(args);

            // Assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(Directory.Exists(outputPath));

            var contractContent = File.ReadAllText(Path.Combine(outputPath, $"{contractName}.cs"));
            Assert.IsTrue(contractContent.Contains("RequestData"));
            Assert.IsTrue(contractContent.Contains("OnOracleResponse"));
            Assert.IsTrue(contractContent.Contains("Oracle.Request"));
        }

        [TestMethod]
        public void Test_NewCommand_OwnableTemplate()
        {
            // Arrange
            var contractName = "TestOwnable";
            var outputPath = Path.Combine(_testDirectory, contractName);

            // Act
            var args = new[] { "new", contractName, "--template", "ownable", "--output", outputPath };
            var result = Program.Main(args);

            // Assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(Directory.Exists(outputPath));

            var contractContent = File.ReadAllText(Path.Combine(outputPath, $"{contractName}.cs"));
            Assert.IsTrue(contractContent.Contains("SetAdmin"));
            Assert.IsTrue(contractContent.Contains("IsAdmin"));
            Assert.IsTrue(contractContent.Contains("HasPermission"));
        }

        [TestMethod]
        public void Test_NewCommand_ForceOverwrite()
        {
            // Arrange
            var contractName = "TestOverwrite";
            var outputPath = Path.Combine(_testDirectory, contractName);

            // Create initial contract
            var args1 = new[] { "new", contractName, "--template", "basic", "--output", outputPath };
            Program.Main(args1);

            // Modify a file to detect overwrite
            var contractFile = Path.Combine(outputPath, $"{contractName}.cs");
            File.WriteAllText(contractFile, "// Modified content");

            // Act - Try to overwrite with force flag
            var args2 = new[] { "new", contractName, "--template", "nep17", "--output", outputPath, "--force" };
            var result = Program.Main(args2);

            // Assert
            Assert.AreEqual(0, result);
            var newContent = File.ReadAllText(contractFile);
            Assert.IsFalse(newContent.StartsWith("// Modified content"));
            Assert.IsTrue(newContent.Contains("Nep17Token"));
        }

        [TestMethod]
        public void Test_NewCommand_InvalidTemplate()
        {
            // Arrange
            var contractName = "TestInvalid";
            var outputPath = Path.Combine(_testDirectory, contractName);

            // Capture console output
            using (var sw = new StringWriter())
            {
                Console.SetError(sw);

                // Act
                var args = new[] { "new", contractName, "--template", "invalid", "--output", outputPath };
                var result = Program.Main(args);

                // Assert
                var output = sw.ToString();
                Assert.IsTrue(output.Contains("Unknown template: invalid"));
                Assert.IsFalse(Directory.Exists(outputPath));
            }
        }

        [TestMethod]
        public void Test_NewCommand_CleanContractName()
        {
            // Arrange
            var contractName = "123-Test Contract!@#";
            var expectedCleanName = "_123TestContract";
            var outputPath = Path.Combine(_testDirectory, "output");

            // Act
            var args = new[] { "new", contractName, "--template", "basic", "--output", outputPath };
            var result = Program.Main(args);

            // Assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(File.Exists(Path.Combine(outputPath, $"{expectedCleanName}.cs")));

            var contractContent = File.ReadAllText(Path.Combine(outputPath, $"{expectedCleanName}.cs"));
            Assert.IsTrue(contractContent.Contains($"public class {expectedCleanName}"));
        }

        [TestMethod]
        public void Test_NewCommand_DefaultTemplate()
        {
            // Arrange
            var contractName = "TestDefault";
            var outputPath = Path.Combine(_testDirectory, contractName);

            // Act - No template specified, should use "basic"
            var args = new[] { "new", contractName, "--output", outputPath };
            var result = Program.Main(args);

            // Assert
            Assert.AreEqual(0, result);
            var contractContent = File.ReadAllText(Path.Combine(outputPath, $"{contractName}.cs"));
            Assert.IsTrue(contractContent.Contains($"public class {contractName} : SmartContract"));
            Assert.IsTrue(contractContent.Contains("HelloWorld"));
        }

        [TestMethod]
        public void Test_NewCommand_ProjectFileContent()
        {
            // Arrange
            var contractName = "TestProject";
            var outputPath = Path.Combine(_testDirectory, contractName);

            // Act
            var args = new[] { "new", contractName, "--output", outputPath };
            var result = Program.Main(args);

            // Assert
            Assert.AreEqual(0, result);
            var projectContent = File.ReadAllText(Path.Combine(outputPath, $"{contractName}.csproj"));
            Assert.IsTrue(projectContent.Contains("<TargetFramework>net9.0</TargetFramework>"));
            Assert.IsTrue(projectContent.Contains("Neo.SmartContract.Framework"));
            Assert.IsTrue(projectContent.Contains("Version=\"3.8.1-*\""));
        }

        [TestMethod]
        public void Test_GeneratedContract_Compiles()
        {
            // Arrange
            var contractName = "TestCompilation";
            var outputPath = Path.Combine(_testDirectory, contractName);

            // Generate contract
            var generateArgs = new[] { "new", contractName, "--template", "basic", "--output", outputPath };
            var generateResult = Program.Main(generateArgs);
            Assert.AreEqual(0, generateResult);

            // Act - Try to compile the generated contract
            var projectPath = Path.Combine(outputPath, $"{contractName}.csproj");
            var compileArgs = new[] { "compile", projectPath };
            var compileResult = Program.Main(compileArgs);

            // Assert
            Assert.AreEqual(0, compileResult);
            Assert.IsTrue(File.Exists(Path.Combine(outputPath, "bin", "sc", $"{contractName}.nef")));
            Assert.IsTrue(File.Exists(Path.Combine(outputPath, "bin", "sc", $"{contractName}.manifest.json")));
        }
    }
}
