using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer;
using Neo.SmartContract.Manifest;
using System;
using System.IO;
using System.Text;

namespace Neo.SmartContract.Fuzzer.Tests
{
    [TestClass]
    public class SmartContractFuzzerTests
    {
        private static readonly string TestDataPath = Path.Combine("TestData");
        private static readonly string OutputPath = Path.Combine(TestDataPath, "Output");

        [TestInitialize]
        public void Setup()
        {
            // Create test directories if they don't exist
            Directory.CreateDirectory(TestDataPath);
            Directory.CreateDirectory(OutputPath);
        }

        [TestMethod]
        public void TestFuzzerInitialization()
        {
            // Create a basic configuration
            var config = new FuzzerConfiguration
            {
                NefPath = Path.Combine(TestDataPath, "SampleToken.nef"),
                ManifestPath = Path.Combine(TestDataPath, "SampleToken.manifest.json"),
                OutputDirectory = OutputPath,
                IterationsPerMethod = 1,
                GasLimit = 10_000_000,
                EnableCoverage = false
            };

            // Create the fuzzer
            var fuzzer = new SmartContractFuzzer(config);
            
            // Assert that the fuzzer was created successfully
            Assert.IsNotNull(fuzzer);
        }

        [TestMethod]
        public void TestConfigurationValidation()
        {
            // Create a valid configuration
            var config = new FuzzerConfiguration
            {
                NefPath = Path.Combine(TestDataPath, "SampleToken.nef"),
                ManifestPath = Path.Combine(TestDataPath, "SampleToken.manifest.json"),
                OutputDirectory = OutputPath,
                IterationsPerMethod = 1
            };

            // Validate the configuration
            Assert.IsTrue(config.Validate());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConfigurationValidation_InvalidIterations()
        {
            // Create an invalid configuration with zero iterations
            var config = new FuzzerConfiguration
            {
                NefPath = Path.Combine(TestDataPath, "SampleToken.nef"),
                ManifestPath = Path.Combine(TestDataPath, "SampleToken.manifest.json"),
                OutputDirectory = OutputPath,
                IterationsPerMethod = 0
            };

            // Validate the configuration (should throw)
            config.Validate();
        }
    }
}
