using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Neo.Compiler.Fuzzer;

namespace Neo.Compiler.Fuzzer.Tests
{
    [TestClass]
    public class ComprehensiveFuzzerTests
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
        public void TestFragmentGeneratorExpressions()
        {
            // Create a fragment generator
            var generator = new FragmentGenerator(seed: 42);

            // Test various expression types
            var expressionTypes = new Dictionary<string, Func<string>>
            {
                { "Boolean", () => generator.GenerateBooleanLiteral() },
                { "Integer", () => generator.GenerateIntegerLiteral() },
                { "String", () => generator.GenerateStringLiteral() }
            };

            foreach (var expressionType in expressionTypes)
            {
                string expression = expressionType.Value();

                // Verify the expression was generated
                Assert.IsNotNull(expression, $"{expressionType.Key} expression should not be null");
                Assert.IsTrue(expression.Length > 0, $"{expressionType.Key} expression should not be empty");
            }
        }

        [TestMethod]
        public void TestGenerateStorageOperation()
        {
            // Create a fragment generator
            var generator = new FragmentGenerator(seed: 42);

            // Generate a storage operation
            string storageOperation = generator.GenerateStorageOperation();

            // Verify the storage operation was generated
            Assert.IsNotNull(storageOperation);
            Assert.IsTrue(storageOperation.Contains("Storage"), "Storage operation should contain 'Storage'");
        }

        [TestMethod]
        public void TestContractCompilation()
        {
            // Create a simple contract for testing compilation
            string contractCode = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;
using System.ComponentModel;

namespace TestContract
{
    [DisplayName(""TestToken"")]
    public class TestToken : SmartContract
    {
        [Safe]
        public static string Name() => ""Test Token"";

        [Safe]
        public static string Symbol() => ""TEST"";

        [Safe]
        public static byte Decimals() => 8;

        [Safe]
        public static BigInteger TotalSupply() => 100_000_000;

        public static bool Main(string operation, object[] args)
        {
            if (operation == ""test"")
            {
                return true;
            }
            return false;
        }
    }
}";

            // Save the contract to a file
            string contractPath = Path.Combine(_testOutputDir, "TestContract.cs");
            File.WriteAllText(contractPath, contractCode);

            // Create a contract compiler
            var compiler = new ContractCompiler(_testOutputDir);

            // Compile the contract
            var result = compiler.Compile(contractPath);

            // Verify compilation succeeded
            Assert.IsTrue(result.Success, "Contract compilation should succeed");

            // Verify output files were created
            Assert.IsTrue(File.Exists(result.NefPath), "NEF file should be created");
            Assert.IsTrue(File.Exists(result.ManifestPath), "Manifest file should be created");
        }

        [TestMethod]
        public void TestDynamicContractFuzzerGeneration()
        {
            // Create a dynamic contract fuzzer with minimal settings
            var fuzzer = new DynamicContractFuzzer(
                outputDirectory: _testOutputDir,
                testExecution: false
            );

            // Generate a contract
            string contractName = "TestDynamicContract";
            bool success = fuzzer.GenerateAndTestSingleContract(contractName, 3);
            string contractPath = Path.Combine(_testOutputDir, $"{contractName}.cs");

            // Verify a contract file was created
            Assert.IsTrue(File.Exists(contractPath), "Contract file should be generated");

            // Verify the contract contains valid C# code
            string contractContent = File.ReadAllText(contractPath);
            Assert.IsTrue(contractContent.Contains("namespace"), "Contract should contain a namespace declaration");
            Assert.IsTrue(contractContent.Contains("public class"), "Contract should contain a class declaration");
            Assert.IsTrue(contractContent.Contains("SmartContract"), "Contract should inherit from SmartContract");
            // Note: Neo N3 contracts don't need a Main entry point
        }

        [TestMethod]
        public void TestRandomSeedReproducibility()
        {
            // Create two fragment generators with the same seed
            int seed = 12345;
            var generator1 = new FragmentGenerator(seed);
            var generator2 = new FragmentGenerator(seed);

            // Generate expressions with both generators
            string expr1 = generator1.GenerateBooleanLiteral();
            string expr2 = generator2.GenerateBooleanLiteral();

            // Verify the expressions are identical (reproducible)
            Assert.AreEqual(expr1, expr2, "Expressions generated with the same seed should be identical");
        }
    }
}
