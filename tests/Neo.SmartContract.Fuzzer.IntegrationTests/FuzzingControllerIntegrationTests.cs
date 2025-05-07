using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.Controller;
using Neo.SmartContract.Fuzzer.Coverage;
using Neo.SmartContract.Fuzzer.Feedback;
using Neo.SmartContract.Manifest;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;

namespace Neo.SmartContract.Fuzzer.IntegrationTests
{
    [TestClass]
    public class FuzzingControllerIntegrationTests
    {
        private string _testContractPath;
        private byte[] _nefBytes;
        private ContractManifest _manifest;
        private string _outputDir;
        private string _corpusDir;
        private FuzzerConfiguration _config;
        private FuzzingController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Set up test contract path
            _testContractPath = Path.Combine(TestContext.TestRunDirectory, "TestContracts", "SimpleContract.nef");
            
            // Create output and corpus directories
            _outputDir = Path.Combine(TestContext.TestRunDirectory, "TestOutput", "Fuzzing");
            _corpusDir = Path.Combine(_outputDir, "corpus");
            Directory.CreateDirectory(_outputDir);
            Directory.CreateDirectory(_corpusDir);
            
            // If test contract exists, load it
            if (File.Exists(_testContractPath))
            {
                _nefBytes = File.ReadAllBytes(_testContractPath);
                
                // Load manifest
                string manifestPath = Path.ChangeExtension(_testContractPath, ".manifest.json");
                if (File.Exists(manifestPath))
                {
                    string manifestJson = File.ReadAllText(manifestPath);
                    _manifest = ContractManifest.Parse(manifestJson);
                }
                else
                {
                    // Create a simple manifest for testing
                    _manifest = new ContractManifest
                    {
                        Name = "SimpleContract",
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
                }
            }
            else
            {
                // Create dummy NEF bytes and manifest for testing
                _nefBytes = new byte[100]; // Dummy NEF bytes
                new Random(12345).NextBytes(_nefBytes);
                
                _manifest = new ContractManifest
                {
                    Name = "DummyContract",
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
            }
            
            // Create configuration
            _config = new FuzzerConfiguration
            {
                OutputDirectory = _outputDir,
                CorpusDirectory = _corpusDir,
                Seed = 12345,
                IterationsPerMethod = 10, // Small number for testing
                MaxExecutionTime = 5, // 5 seconds for testing
                EnableCoverage = true,
                EnableCoverageGuidedFuzzing = true,
                PrioritizeBranchCoverage = true,
                PrioritizePathCoverage = true,
                VerboseLogging = true
            };
            
            // Initialize controller
            _controller = new FuzzingController(_nefBytes, _manifest, _config);
        }

        [TestMethod]
        public async Task StartFuzzing_CompletesSuccessfully()
        {
            // Act
            await _controller.StartFuzzing();
            
            // Wait for fuzzing to complete
            while (_controller.Status.Status == FuzzingStatusType.Running)
            {
                await Task.Delay(100);
            }
            
            // Assert
            _controller.Status.Status.Should().Be(FuzzingStatusType.Completed);
            _controller.Status.TotalExecutions.Should().BeGreaterThan(0);
            _controller.Status.CodeCoverage.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task StartFuzzing_GeneratesCoverageReport()
        {
            // Act
            await _controller.StartFuzzing();
            
            // Wait for fuzzing to complete
            while (_controller.Status.Status == FuzzingStatusType.Running)
            {
                await Task.Delay(100);
            }
            
            // Assert
            string coverageDir = Path.Combine(_outputDir, "coverage");
            Directory.Exists(coverageDir).Should().BeTrue();
            
            // Check for HTML coverage report
            string htmlReport = Path.Combine(coverageDir, "index.html");
            File.Exists(htmlReport).Should().BeTrue();
        }

        [TestMethod]
        public async Task StartFuzzing_GeneratesCorpus()
        {
            // Act
            await _controller.StartFuzzing();
            
            // Wait for fuzzing to complete
            while (_controller.Status.Status == FuzzingStatusType.Running)
            {
                await Task.Delay(100);
            }
            
            // Assert
            Directory.Exists(_corpusDir).Should().BeTrue();
            Directory.GetFiles(_corpusDir, "*.json").Length.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task StartFuzzing_GeneratesReport()
        {
            // Act
            await _controller.StartFuzzing();
            
            // Wait for fuzzing to complete
            while (_controller.Status.Status == FuzzingStatusType.Running)
            {
                await Task.Delay(100);
            }
            
            // Assert
            string reportPath = Path.Combine(_outputDir, "fuzzing_report.html");
            File.Exists(reportPath).Should().BeTrue();
        }

        [TestMethod]
        public async Task StartFuzzing_WithCoverageDisabled_CompletesSuccessfully()
        {
            // Arrange
            _config.EnableCoverage = false;
            _config.EnableCoverageGuidedFuzzing = false;
            _controller = new FuzzingController(_nefBytes, _manifest, _config);
            
            // Act
            await _controller.StartFuzzing();
            
            // Wait for fuzzing to complete
            while (_controller.Status.Status == FuzzingStatusType.Running)
            {
                await Task.Delay(100);
            }
            
            // Assert
            _controller.Status.Status.Should().Be(FuzzingStatusType.Completed);
            _controller.Status.TotalExecutions.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task StopFuzzing_StopsFuzzingProcess()
        {
            // Arrange
            _config.IterationsPerMethod = 1000; // More iterations to ensure it's still running when we stop it
            _controller = new FuzzingController(_nefBytes, _manifest, _config);
            
            // Act
            await _controller.StartFuzzing();
            
            // Wait a bit to ensure fuzzing has started
            await Task.Delay(500);
            
            // Stop fuzzing
            await _controller.StopFuzzing();
            
            // Assert
            _controller.Status.Status.Should().Be(FuzzingStatusType.Stopped);
        }

        [TestMethod]
        public void GetStatus_ReturnsCurrentStatus()
        {
            // Act
            var status = _controller.Status;
            
            // Assert
            status.Should().NotBeNull();
            status.Status.Should().Be(FuzzingStatusType.NotStarted);
            status.TotalExecutions.Should().Be(0);
            status.SuccessfulExecutions.Should().Be(0);
            status.FailedExecutions.Should().Be(0);
            status.IssuesFound.Should().Be(0);
            status.CodeCoverage.Should().Be(0);
        }

        [TestMethod]
        public async Task GetIssues_ReturnsDetectedIssues()
        {
            // Act
            await _controller.StartFuzzing();
            
            // Wait for fuzzing to complete
            while (_controller.Status.Status == FuzzingStatusType.Running)
            {
                await Task.Delay(100);
            }
            
            var issues = _controller.GetIssues();
            
            // Assert
            issues.Should().NotBeNull();
            // We can't assert exact count as it depends on the contract
        }

        public TestContext TestContext { get; set; }
    }
}
