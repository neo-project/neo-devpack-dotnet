using System;
using System.IO;
using System.Linq;
using Moq;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;
using Xunit;

namespace Neo.SmartContract.Fuzzer.Tests.Core
{
    /// <summary>
    /// Tests for the SmartContractFuzzer class
    /// </summary>
    public class SmartContractFuzzerTests
    {
        private readonly string _testDataPath = Path.Combine("TestData");
        private readonly string _manifestPath;
        private readonly string _nefPath;

        /// <summary>
        /// Initialize test data
        /// </summary>
        public SmartContractFuzzerTests()
        {
            _manifestPath = Path.Combine(_testDataPath, "SampleToken.manifest.json");
            _nefPath = Path.Combine(_testDataPath, "SampleToken.nef");
        }

        /// <summary>
        /// Test initialization of the fuzzer with valid configuration
        /// </summary>
        [Fact]
        public void InitializationTest()
        {
            // Arrange
            var config = new FuzzerConfiguration
            {
                OutputDirectory = Path.Combine(Path.GetTempPath(), "FuzzerTest"),
                IterationsPerMethod = 5,
                Seed = 12345,
                EnableCoverage = false
            };
            
            // Act
            var fuzzer = new SmartContractFuzzer(config);
            
            // Assert
            Assert.NotNull(fuzzer);
            
            // Clean up
            if (Directory.Exists(config.OutputDirectory))
            {
                Directory.Delete(config.OutputDirectory, true);
            }
        }

        /// <summary>
        /// Test loading a contract from NEF and manifest files
        /// </summary>
        [Fact]
        public void LoadContractTest()
        {
            // Arrange
            var config = new FuzzerConfiguration
            {
                OutputDirectory = Path.Combine(Path.GetTempPath(), "FuzzerTest"),
                IterationsPerMethod = 1,
                EnableCoverage = false
            };
            
            var fuzzer = new SmartContractFuzzer(config);
            
            // Act
            bool result = fuzzer.LoadContract(_nefPath, _manifestPath);
            
            // Assert
            Assert.True(result);
            
            // Clean up
            if (Directory.Exists(config.OutputDirectory))
            {
                Directory.Delete(config.OutputDirectory, true);
            }
        }

        /// <summary>
        /// Test filtering methods based on configuration
        /// </summary>
        [Fact]
        public void FilterMethodsTest()
        {
            // Arrange
            var manifestJson = File.ReadAllText(_manifestPath);
            var manifest = ContractManifest.Parse(manifestJson);
            
            // Create a fuzzer with method filtering
            var config = new FuzzerConfiguration
            {
                OutputDirectory = Path.Combine(Path.GetTempPath(), "FuzzerTest"),
                IterationsPerMethod = 1,
                EnableCoverage = false,
                IncludeMethods = new[] { "transfer", "balanceOf" },
                ExcludeMethods = new[] { "_deploy" },
                SkipParameterlessMethods = true,
                IncludePrivateMethods = false
            };
            
            // Use reflection to access the private FilterMethods method
            var fuzzer = new SmartContractFuzzer(config);
            var methodInfo = typeof(SmartContractFuzzer).GetMethod("FilterMethods", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Act
            var filteredMethods = (ContractMethodDescriptor[])methodInfo.Invoke(fuzzer, new object[] { manifest.Abi.Methods });
            
            // Assert
            Assert.Equal(2, filteredMethods.Length);
            Assert.Contains(filteredMethods, m => m.Name == "transfer");
            Assert.Contains(filteredMethods, m => m.Name == "balanceOf");
            Assert.DoesNotContain(filteredMethods, m => m.Name == "_deploy");
            Assert.DoesNotContain(filteredMethods, m => m.Name == "totalSupply"); // Parameterless
            
            // Clean up
            if (Directory.Exists(config.OutputDirectory))
            {
                Directory.Delete(config.OutputDirectory, true);
            }
        }

        /// <summary>
        /// Test fuzzing a method with mocked dependencies
        /// </summary>
        [Fact]
        public void FuzzMethodTest()
        {
            // Arrange
            var config = new FuzzerConfiguration
            {
                OutputDirectory = Path.Combine(Path.GetTempPath(), "FuzzerTest"),
                IterationsPerMethod = 3,
                EnableCoverage = false,
                SaveInputsAndResults = true
            };
            
            // Create mocks
            var mockContractExecutor = new Mock<ContractExecutor>(MockBehavior.Loose, null, null, config);
            
            // Set up mock behavior
            mockContractExecutor.Setup(m => m.ExecuteMethod(It.IsAny<ContractMethodDescriptor>(), It.IsAny<StackItem[]>(), It.IsAny<int>()))
                .Returns((ContractMethodDescriptor method, StackItem[] parameters, int iteration) => 
                {
                    return new ExecutionResult
                    {
                        MethodName = method.Name,
                        Parameters = parameters,
                        Success = true,
                        StartTime = DateTime.Now,
                        EndTime = DateTime.Now.AddMilliseconds(10),
                        GasConsumed = 1000,
                        ReturnValue = new Integer(42)
                    };
                });
            
            // Create a test method
            var method = new ContractMethodDescriptor
            {
                Name = "testMethod",
                Parameters = new[]
                {
                    new ContractParameterDefinition { Name = "param1", Type = "Integer" }
                },
                ReturnType = "Integer",
                Safe = true
            };
            
            // Use reflection to access the private FuzzMethod method
            var fuzzer = new SmartContractFuzzer(config);
            
            // Set private fields using reflection
            typeof(SmartContractFuzzer).GetField("_contractExecutor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(fuzzer, mockContractExecutor.Object);
            
            typeof(SmartContractFuzzer).GetField("_parameterGenerator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(fuzzer, new ParameterGeneration.EnhancedParameterGenerator(seed: 12345));
            
            var methodInfo = typeof(SmartContractFuzzer).GetMethod("FuzzMethod", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Act
            methodInfo.Invoke(fuzzer, new object[] { method });
            
            // Assert
            mockContractExecutor.Verify(m => m.ExecuteMethod(It.IsAny<ContractMethodDescriptor>(), It.IsAny<StackItem[]>(), It.IsAny<int>()), Times.Exactly(3));
            
            // Check if results directory was created
            string methodDir = Path.Combine(config.OutputDirectory, "Results", "testMethod");
            Assert.True(Directory.Exists(methodDir));
            
            // Check if input and result files were created
            var inputFiles = Directory.GetFiles(methodDir, "input_*.json");
            var resultFiles = Directory.GetFiles(methodDir, "result_*.json");
            Assert.Equal(3, inputFiles.Length);
            Assert.Equal(3, resultFiles.Length);
            
            // Clean up
            if (Directory.Exists(config.OutputDirectory))
            {
                Directory.Delete(config.OutputDirectory, true);
            }
        }

        /// <summary>
        /// Test handling of execution errors
        /// </summary>
        [Fact]
        public void ExecutionErrorHandlingTest()
        {
            // Arrange
            var config = new FuzzerConfiguration
            {
                OutputDirectory = Path.Combine(Path.GetTempPath(), "FuzzerTest"),
                IterationsPerMethod = 3,
                EnableCoverage = false
            };
            
            // Create mocks
            var mockContractExecutor = new Mock<ContractExecutor>(MockBehavior.Loose, null, null, config);
            
            // Set up mock behavior to throw an exception on the second iteration
            mockContractExecutor.Setup(m => m.ExecuteMethod(It.IsAny<ContractMethodDescriptor>(), It.IsAny<StackItem[]>(), It.Is<int>(i => i != 1)))
                .Returns((ContractMethodDescriptor method, StackItem[] parameters, int iteration) => 
                {
                    return new ExecutionResult
                    {
                        MethodName = method.Name,
                        Parameters = parameters,
                        Success = true,
                        StartTime = DateTime.Now,
                        EndTime = DateTime.Now.AddMilliseconds(10),
                        GasConsumed = 1000,
                        ReturnValue = new Integer(42)
                    };
                });
            
            mockContractExecutor.Setup(m => m.ExecuteMethod(It.IsAny<ContractMethodDescriptor>(), It.IsAny<StackItem[]>(), 1))
                .Throws(new Exception("Test exception"));
            
            // Create a test method
            var method = new ContractMethodDescriptor
            {
                Name = "testMethod",
                Parameters = new[]
                {
                    new ContractParameterDefinition { Name = "param1", Type = "Integer" }
                },
                ReturnType = "Integer",
                Safe = true
            };
            
            // Use reflection to access the private FuzzMethod method
            var fuzzer = new SmartContractFuzzer(config);
            
            // Set private fields using reflection
            typeof(SmartContractFuzzer).GetField("_contractExecutor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(fuzzer, mockContractExecutor.Object);
            
            typeof(SmartContractFuzzer).GetField("_parameterGenerator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(fuzzer, new ParameterGeneration.EnhancedParameterGenerator(seed: 12345));
            
            var methodInfo = typeof(SmartContractFuzzer).GetMethod("FuzzMethod", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Act & Assert
            // This should not throw an exception, as the fuzzer should handle the exception internally
            methodInfo.Invoke(fuzzer, new object[] { method });
            
            // Verify that ExecuteMethod was called for all iterations
            mockContractExecutor.Verify(m => m.ExecuteMethod(It.IsAny<ContractMethodDescriptor>(), It.IsAny<StackItem[]>(), It.IsAny<int>()), Times.Exactly(3));
            
            // Clean up
            if (Directory.Exists(config.OutputDirectory))
            {
                Directory.Delete(config.OutputDirectory, true);
            }
        }
    }
}