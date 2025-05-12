using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Neo.Compiler.Fuzzer;

namespace Neo.Compiler.Fuzzer.Tests
{
    [TestClass]
    public class DynamicContractFuzzerTests
    {
        private string _testOutputDir;

        [TestInitialize]
        public void TestSetup()
        {
            // Create a temporary directory for test output
            _testOutputDir = Path.Combine(Path.GetTempPath(), "Neo.Compiler.Fuzzer.Tests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testOutputDir);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Clean up the test output directory
            if (Directory.Exists(_testOutputDir))
            {
                Directory.Delete(_testOutputDir, true);
            }
        }

        [TestMethod]
        public void TestDynamicContractFuzzerInitialization()
        {
            // Create a dynamic contract fuzzer with minimal settings
            var fuzzer = new DynamicContractFuzzer(
                outputDirectory: _testOutputDir,
                testExecution: false
            );

            // Verify the fuzzer was created successfully
            Assert.IsNotNull(fuzzer);
        }

        [TestMethod]
        public void TestGenerateAndTestSingleContract()
        {
            // Create a dynamic contract fuzzer
            var fuzzer = new DynamicContractFuzzer(
                outputDirectory: _testOutputDir,
                testExecution: false
            );

            // Generate a contract
            string contractName = "TestContract";
            bool success = fuzzer.GenerateAndTestSingleContract(contractName, 3);
            string contractPath = Path.Combine(_testOutputDir, $"{contractName}.cs");

            // Verify the contract file was created
            Assert.IsTrue(File.Exists(contractPath), "Contract file should be created");

            // Verify the contract content
            var contractContent = File.ReadAllText(contractPath);
            Assert.IsTrue(contractContent.Contains("namespace"), "Contract should contain a namespace declaration");
            Assert.IsTrue(contractContent.Contains("class"), "Contract should contain a class declaration");
            Assert.IsTrue(contractContent.Contains("SmartContract"), "Contract should inherit from SmartContract");
        }

        [TestMethod]
        public void TestRunTests()
        {
            // Create a dynamic contract fuzzer
            var fuzzer = new DynamicContractFuzzer(
                outputDirectory: _testOutputDir,
                testExecution: false
            );

            // Run tests with minimal iterations
            bool result = fuzzer.RunTests(iterations: 1, featuresPerContract: 3);

            // We don't verify the result as it may vary depending on the random features selected
            // Just verify that the method completed without exceptions

            // Verify the report file was created
            string reportPath = Path.Combine(_testOutputDir, "DynamicContractFuzzerReport.md");
            Assert.IsTrue(File.Exists(reportPath), "Report file should be created");
        }

        [TestMethod]
        public void TestGenerateAndTestSingleContractWithCustomName()
        {
            // Create a dynamic contract fuzzer
            var fuzzer = new DynamicContractFuzzer(
                outputDirectory: _testOutputDir,
                testExecution: false
            );

            // Generate and test a single contract
            string contractName = "SingleTestContract";
            bool success = fuzzer.GenerateAndTestSingleContract(contractName, 3);

            // Note: The success flag might be false if compilation fails, but the file is still created
            // We're just testing that the file is created, not that compilation succeeds

            // Verify the contract file was created
            string contractPath = Path.Combine(_testOutputDir, $"{contractName}.cs");
            Assert.IsTrue(File.Exists(contractPath), "Contract file should be created");
        }
    }
}
