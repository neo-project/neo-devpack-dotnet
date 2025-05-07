using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer;
using Neo.SmartContract.Fuzzer.Coverage;
using Neo.SmartContract.Manifest;
using System;
using System.IO;

namespace Neo.SmartContract.Fuzzer.Tests
{
    [TestClass]
    public class CoverageTrackerTests
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
                EnableCoverage = true,
                CoverageFormat = "html"
            };
        }

        [TestMethod]
        public void TestCoverageTrackerInitialization()
        {
            // Create coverage tracker
            var tracker = new CoverageTracker(_nefBytes, _manifest, _config);
            
            // Assert that the tracker was created successfully
            Assert.IsNotNull(tracker);
        }

        [TestMethod]
        public void TestGenerateReport()
        {
            // Create coverage tracker
            var tracker = new CoverageTracker(_nefBytes, _manifest, _config);
            
            // Generate report
            tracker.GenerateReport();
            
            // Check if report files were created
            string coverageDir = Path.Combine(_config.OutputDirectory, _config.CoverageOutput);
            string htmlReport = Path.Combine(coverageDir, "index.html");
            
            // Assert that the report file exists
            Assert.IsTrue(Directory.Exists(coverageDir));
            Assert.IsTrue(File.Exists(htmlReport));
        }
    }
}
