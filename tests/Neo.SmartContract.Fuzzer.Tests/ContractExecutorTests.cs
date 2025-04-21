using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer;
using Neo.SmartContract.Manifest;
using Neo.VM.Types;
using System;
using System.IO;

namespace Neo.SmartContract.Fuzzer.Tests
{
    [TestClass]
    public class ContractExecutorTests
    {
        private static readonly string TestDataPath = Path.Combine("TestData");
        private static readonly string OutputPath = Path.Combine(TestDataPath, "Output");
        private byte[] _nefBytes;
        private ContractManifest _manifest;
        private FuzzerConfiguration _config;

        [TestInitialize]
        public void Setup()
        {
            // Create test directories if they don't exist
            Directory.CreateDirectory(TestDataPath);
            Directory.CreateDirectory(OutputPath);
            
            // Load NEF and manifest files
            _nefBytes = File.ReadAllBytes(Path.Combine(TestDataPath, "SampleToken.nef"));
            string manifestJson = File.ReadAllText(Path.Combine(TestDataPath, "SampleToken.manifest.json"));
            _manifest = ContractManifest.Parse(manifestJson);
            
            // Create configuration
            _config = new FuzzerConfiguration
            {
                NefPath = Path.Combine(TestDataPath, "SampleToken.nef"),
                ManifestPath = Path.Combine(TestDataPath, "SampleToken.manifest.json"),
                OutputDirectory = OutputPath,
                IterationsPerMethod = 1,
                GasLimit = 10_000_000,
                EnableCoverage = false
            };
        }

        [TestMethod]
        public void TestContractExecutorInitialization()
        {
            // Create contract executor
            var executor = new ContractExecutor(_nefBytes, _manifest, _config);
            
            // Assert that the executor was created successfully
            Assert.IsNotNull(executor);
        }

        [TestMethod]
        public void TestExecuteMethod()
        {
            // Create contract executor
            var executor = new ContractExecutor(_nefBytes, _manifest, _config);
            
            // Get a method from the manifest
            var method = _manifest.Abi.Methods[0]; // totalSupply
            
            // Create parameters (none needed for totalSupply)
            var parameters = Array.Empty<StackItem>();
            
            // Execute the method
            var result = executor.ExecuteMethod(method, parameters, 0);
            
            // Assert that the result is not null
            Assert.IsNotNull(result);
            
            // Assert that the method name is correct
            Assert.AreEqual(method.Name, result.MethodName);
            
            // Assert that the iteration number is correct
            Assert.AreEqual(0, result.IterationNumber);
        }

        [TestMethod]
        public void TestExecuteMethodWithParameters()
        {
            // Create contract executor
            var executor = new ContractExecutor(_nefBytes, _manifest, _config);
            
            // Get a method from the manifest that requires parameters
            var method = _manifest.Abi.Methods[3]; // balanceOf
            
            // Create parameters
            var parameters = new StackItem[]
            {
                new ByteString(new byte[20]) // Hash160 parameter (all zeros)
            };
            
            // Execute the method
            var result = executor.ExecuteMethod(method, parameters, 0);
            
            // Assert that the result is not null
            Assert.IsNotNull(result);
            
            // Assert that the method name is correct
            Assert.AreEqual(method.Name, result.MethodName);
            
            // Assert that the parameters are correct
            Assert.AreEqual(parameters.Length, result.Parameters.Length);
        }
    }
}
