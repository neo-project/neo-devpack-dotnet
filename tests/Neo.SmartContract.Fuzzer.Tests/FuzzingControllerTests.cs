using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.Controller;
using Neo.SmartContract.Manifest;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Neo.SmartContract.Fuzzer.Tests
{
    [TestClass]
    public class FuzzingControllerTests
    {
        private byte[] _nefBytes;
        private ContractManifest _manifest;
        private FuzzerConfiguration _config;

        [TestInitialize]
        public void Setup()
        {
            // Create dummy NEF bytes
            _nefBytes = new byte[100];
            new Random(12345).NextBytes(_nefBytes);

            // Create a simple manifest for testing
            _manifest = new ContractManifest
            {
                Name = "TestContract",
                Abi = new ContractAbi
                {
                    Methods = new ContractMethodDescriptor[]
                    {
                        new ContractMethodDescriptor
                        {
                            Name = "TestMethod",
                            Parameters = new ContractParameterDefinition[0],
                            ReturnType = ContractParameterType.Void
                        }
                    }
                }
            };

            // Create configuration
            _config = new FuzzerConfiguration
            {
                OutputDirectory = Path.Combine(Path.GetTempPath(), "FuzzerTest"),
                IterationsPerMethod = 2, // Small number for testing
                MaxExecutionTime = 5, // 5 seconds for testing
                EnableCoverage = true,
                EnableCoverageGuidedFuzzing = true,
                PrioritizeBranchCoverage = true,
                PrioritizePathCoverage = true
            };

            // Create output directory
            Directory.CreateDirectory(_config.OutputDirectory);
        }

        [TestMethod]
        public void Constructor_InitializesProperties()
        {
            // Act
            var controller = new FuzzingController(_nefBytes, _manifest, _config);

            // Assert
            Assert.IsNotNull(controller);
            Assert.IsNotNull(controller.Status);
            Assert.AreEqual(FuzzingStatusType.NotStarted, controller.Status.Status);
        }

        [TestMethod]
        public async Task StartFuzzing_UpdatesStatus()
        {
            // Arrange
            var controller = new FuzzingController(_nefBytes, _manifest, _config);

            // Act
            await controller.StartFuzzing();

            // Assert
            Assert.AreEqual(FuzzingStatusType.Completed, controller.Status.Status);
        }

        [TestMethod]
        public async Task StopFuzzing_StopsFuzzing()
        {
            // Arrange
            var controller = new FuzzingController(_nefBytes, _manifest, _config);

            // Act
            await controller.StartFuzzing();
            await controller.StopFuzzing();

            // Assert
            Assert.AreEqual(FuzzingStatusType.Stopped, controller.Status.Status);
        }

        [TestMethod]
        public void GetIssues_ReturnsIssues()
        {
            // Arrange
            var controller = new FuzzingController(_nefBytes, _manifest, _config);

            // Act
            var issues = controller.GetIssues();

            // Assert
            Assert.IsNotNull(issues);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up the output directory
            if (Directory.Exists(_config.OutputDirectory))
            {
                try
                {
                    Directory.Delete(_config.OutputDirectory, true);
                }
                catch
                {
                    // Ignore errors during cleanup
                }
            }
        }
    }
}
