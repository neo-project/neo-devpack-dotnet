using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.SmartContract.Fuzzer.Controller;
using Neo.SmartContract.Fuzzer.Detectors;
using Neo.SmartContract.Fuzzer.Feedback;
using Neo.SmartContract.Fuzzer.InputGeneration;
using Neo.SmartContract.Fuzzer.Reporting;
using Neo.SmartContract.Fuzzer.StaticAnalysis;
using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;

namespace Neo.SmartContract.Fuzzer.Tests.Controller
{
    [TestClass]
    public class FuzzingControllerTests
    {
        private string _tempDirectory;
        private FuzzerConfiguration _config;
        private Mock<ContractManifest> _mockManifest;
        private byte[] _nefBytes;
        private FuzzingController _controller;

        [TestInitialize]
        public void Setup()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), "NeoFuzzerTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);

            _config = new FuzzerConfiguration
            {
                NefPath = Path.Combine(_tempDirectory, "test.nef"),
                ManifestPath = Path.Combine(_tempDirectory, "test.manifest.json"),
                OutputDirectory = _tempDirectory,
                IterationsPerMethod = 1,
                Seed = 42,
                EnableStaticAnalysis = false,
                EnableSymbolicExecution = false
            };

            // Create dummy NEF file
            _nefBytes = new byte[] { 0x01, 0x02, 0x03 };
            File.WriteAllBytes(_config.NefPath, _nefBytes);

            // Create dummy manifest file
            var manifest = new ContractManifest
            {
                Name = "TestContract",
                Abi = new ContractAbi
                {
                    Methods = new ContractMethodDescriptor[]
                    {
                        new ContractMethodDescriptor
                        {
                            Name = "Transfer",
                            Parameters = new ContractParameterDefinition[]
                            {
                                new ContractParameterDefinition { Name = "from", Type = "Hash160" },
                                new ContractParameterDefinition { Name = "to", Type = "Hash160" },
                                new ContractParameterDefinition { Name = "amount", Type = "Integer" }
                            }
                        }
                    }
                }
            };
            File.WriteAllText(_config.ManifestPath, manifest.ToJson().ToString());

            _mockManifest = new Mock<ContractManifest>();
            _mockManifest.Setup(m => m.Abi).Returns(manifest.Abi);
            _mockManifest.Setup(m => m.Name).Returns(manifest.Name);

            _controller = new FuzzingController(_config);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }

        [TestMethod]
        public void Constructor_ValidConfig_CreatesController()
        {
            // Arrange & Act
            var controller = new FuzzingController(_config);

            // Assert
            controller.Should().NotBeNull();
        }

        [TestMethod]
        public void Constructor_NullConfig_ThrowsArgumentNullException()
        {
            // Act & Assert
            Action act = () => new FuzzingController(null);
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("config");
        }

        [TestMethod]
        public void Start_StartsTask()
        {
            // Arrange
            var controller = new FuzzingController(_config);

            // Act
            controller.Start();

            // Assert
            var status = controller.GetStatus();
            status.Should().NotBeNull();
            
            // Clean up
            controller.Stop();
            controller.WaitForCompletion().Wait();
        }

        [TestMethod]
        public void Stop_StopsTask()
        {
            // Arrange
            var controller = new FuzzingController(_config);
            controller.Start();

            // Act
            controller.Stop();
            controller.WaitForCompletion().Wait();

            // Assert
            var status = controller.GetStatus();
            status.Should().NotBeNull();
        }

        [TestMethod]
        public void GetStatus_ReturnsStatus()
        {
            // Arrange
            var controller = new FuzzingController(_config);

            // Act
            var status = controller.GetStatus();

            // Assert
            status.Should().NotBeNull();
            status.ElapsedTime.Should().NotBeNull();
        }

        [TestMethod]
        public async Task WaitForCompletion_CompletesWhenTaskCompletes()
        {
            // Arrange
            var controller = new FuzzingController(_config);
            controller.Start();

            // Act
            await controller.WaitForCompletion();

            // Assert
            var status = controller.GetStatus();
            status.Should().NotBeNull();
        }
    }
}
