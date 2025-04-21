using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Manifest;
using Neo.VM.Types;

namespace Neo.SmartContract.Fuzzer.Tests.Performance
{
    /// <summary>
    /// Performance tests for the Neo Smart Contract Fuzzer
    /// </summary>
    [TestClass]
    [TestCategory("Performance")]
    public class ExecutionPerformanceTests
    {
        private readonly string _testDataPath = Path.Combine("TestData");
        private string _outputPath;
        private string _manifestPath;
        private string _nefPath;
        private TestContext _testContext;

        /// <summary>
        /// Gets or sets the test context which provides information about the current test run
        /// </summary>
        public TestContext TestContext
        {
            get { return _testContext; }
            set { _testContext = value; }
        }

        /// <summary>
        /// Initialize test data and output helper
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _manifestPath = Path.Combine(_testDataPath, "SampleToken.manifest.json");
            _nefPath = Path.Combine(_testDataPath, "SampleToken.nef");
            _outputPath = Path.Combine(Path.GetTempPath(), "FuzzerPerformanceTest");
            
            // Create test directory
            Directory.CreateDirectory(_outputPath);
        }

        /// <summary>
        /// Clean up test resources
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_outputPath))
            {
                try
                {
                    Directory.Delete(_outputPath, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        /// <summary>
        /// Test parameter generation performance
        /// </summary>
        [TestMethod]
        public void ParameterGenerationPerformanceTest()
        {
            // Arrange
            var config = new FuzzerConfiguration
            {
                RandomSeed = 12345, // Fixed seed for reproducibility
                EnableEnhancedParameterGeneration = true,
                EnableBoundaryValueTesting = true,
                EnableEdgeCaseGeneration = true
            };
            
            var parameterGenerator = new EnhancedParameterGenerator(config);
            var parameterTypes = new[]
            {
                "Boolean",
                "Integer",
                "String",
                "Hash160",
                "Hash256",
                "ByteArray",
                "PublicKey",
                "Signature",
                "Array",
                "Map"
            };
            
            const int iterations = 1000;
            var results = new Dictionary<string, TimeSpan>();
            
            // Act
            foreach (var type in parameterTypes)
            {
                var stopwatch = Stopwatch.StartNew();
                
                for (int i = 0; i < iterations; i++)
                {
                    parameterGenerator.GenerateParameter(type);
                }
                
                stopwatch.Stop();
                results[type] = stopwatch.Elapsed;
            }
            
            // Assert & Report
            TestContext.WriteLine("Parameter Generation Performance (1000 iterations):");
            TestContext.WriteLine("Type\tTotal Time\tAvg Time (ms)");
            
            foreach (var result in results)
            {
                double avgMs = result.Value.TotalMilliseconds / iterations;
                TestContext.WriteLine($"{result.Key}\t{result.Value}\t{avgMs:F3}");
                
                // Ensure generation is reasonably fast
                Assert.IsTrue(avgMs < 10, $"Parameter generation for {result.Key} is too slow: {avgMs:F3}ms");
            }
        }

        /// <summary>
        /// Test contract execution performance
        /// </summary>
        [Fact]
        public void ContractExecutionPerformanceTest()
        {
            // Arrange
            var manifestJson = File.ReadAllText(_manifestPath);
            var manifest = ContractManifest.Parse(manifestJson);
            var nefBytes = File.ReadAllBytes(_nefPath);
            
            var config = new FuzzerConfiguration
            {
                RandomSeed = 12345, // Fixed seed for reproducibility
                GasLimit = 20000000
            };
            
            var parameterGenerator = new EnhancedParameterGenerator(config);
            var executor = new ContractExecutor(nefBytes, manifest);
            
            const int iterations = 100;
            var results = new Dictionary<string, TimeSpan>();
            
            // Act
            foreach (var method in manifest.Abi.Methods)
            {
                // Skip methods with no parameters or void return type for simplicity
                if (method.Parameters.Length == 0 || method.ReturnType == "Void")
                    continue;
                
                var stopwatch = Stopwatch.StartNew();
                
                for (int i = 0; i < iterations; i++)
                {
                    // Generate parameters
                    var parameters = new StackItem[method.Parameters.Length];
                    for (int j = 0; j < method.Parameters.Length; j++)
                    {
                        parameters[j] = parameterGenerator.GenerateParameter(method.Parameters[j].Type);
                    }
                    
                    // Execute method
                    executor.ExecuteMethod(method, parameters, i);
                }
                
                stopwatch.Stop();
                results[method.Name] = stopwatch.Elapsed;
            }
            
            // Assert & Report
            TestContext.WriteLine("Contract Execution Performance (100 iterations):");
            TestContext.WriteLine("Method\tTotal Time\tAvg Time (ms)");
            
            foreach (var result in results)
            {
                double avgMs = result.Value.TotalMilliseconds / iterations;
                TestContext.WriteLine($"{result.Key}\t{result.Value}\t{avgMs:F3}");
                
                // Ensure execution is reasonably fast
                Assert.IsTrue(avgMs < 100, $"Contract execution for {result.Key} is too slow: {avgMs:F3}ms");
            }
        }

        /// <summary>
        /// Test report generation performance
        /// </summary>
        [Fact]
        public void ReportGenerationPerformanceTest()
        {
            // Arrange
            var manifestJson = File.ReadAllText(_manifestPath);
            var manifest = ContractManifest.Parse(manifestJson);
            
            // Create a large number of execution results
            var results = new List<ExecutionResult>();
            var random = new Random(12345);
            
            for (int i = 0; i < 1000; i++)
            {
                string methodName = manifest.Abi.Methods[i % manifest.Abi.Methods.Length].Name;
                bool success = random.Next(100) < 90; // 90% success rate
                
                var result = new ExecutionResult
                {
                    MethodName = methodName,
                    Parameters = new StackItem[] { new Integer(i) },
                    Success = success,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMilliseconds(random.Next(10, 100)),
                    GasConsumed = success ? random.Next(500, 2000) : 0,
                    ReturnValue = success ? new Integer(random.Next(1000)) : null,
                    ErrorMessage = success ? null : "Test error message"
                };
                
                results.Add(result);
            }
            
            var metricsCollector = new MetricsCollector(results, manifest.Abi.Methods);
            var formatter = new ReportFormatter("SampleToken", metricsCollector, _outputPath);
            
            // Act & Assert
            var formats = new[]
            {
                ReportFormatter.FormatType.Markdown,
                ReportFormatter.FormatType.Json,
                ReportFormatter.FormatType.Html,
                ReportFormatter.FormatType.Csv
            };
            
            TestContext.WriteLine("Report Generation Performance (1000 execution results):");
            TestContext.WriteLine("Format\tTime");
            
            foreach (var format in formats)
            {
                var stopwatch = Stopwatch.StartNew();
                formatter.GenerateReport(format);
                stopwatch.Stop();
                
                TestContext.WriteLine($"{format}\t{stopwatch.Elapsed}");
                
                // Ensure report generation is reasonably fast
                Assert.IsTrue(stopwatch.ElapsedMilliseconds < 5000, $"Report generation for {format} is too slow: {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        /// <summary>
        /// Test end-to-end fuzzing performance
        /// </summary>
        [Fact]
        public void EndToEndFuzzingPerformanceTest()
        {
            // Arrange
            var config = new FuzzerConfiguration
            {
                NefPath = _nefPath,
                ManifestPath = _manifestPath,
                OutputDirectory = _outputPath,
                IterationsPerMethod = 10,
                SaveInputsAndResults = true,
                EnableCoverage = false, // Disable coverage for performance testing
                RandomSeed = 12345, // Fixed seed for reproducibility
                EnableEnhancedParameterGeneration = true,
                EnableBoundaryValueTesting = true,
                EnableEdgeCaseGeneration = true,
                EnableEnhancedReporting = true,
                ReportFormats = new[] { FuzzerConfiguration.ReportFormat.Markdown }
            };
            
            var fuzzer = new SmartContractFuzzer(config);
            
            // Act
            var stopwatch = Stopwatch.StartNew();
            bool result = fuzzer.FuzzContract(_nefPath, _manifestPath);
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(result);
            
            // Report performance
            TestContext.WriteLine($"End-to-End Fuzzing Performance (10 iterations per method):");
            TestContext.WriteLine($"Total Time: {stopwatch.Elapsed}");
            
            // Calculate executions per second
            int totalMethods = fuzzer.GetMethodCount();
            int totalExecutions = totalMethods * config.IterationsPerMethod;
            double executionsPerSecond = totalExecutions / stopwatch.Elapsed.TotalSeconds;
            
            TestContext.WriteLine($"Methods: {totalMethods}");
            TestContext.WriteLine($"Total Executions: {totalExecutions}");
            TestContext.WriteLine($"Executions per Second: {executionsPerSecond:F2}");
            
            // Ensure fuzzing is reasonably fast
            Assert.IsTrue(executionsPerSecond > 1, $"Fuzzing performance is too slow: {executionsPerSecond:F2} executions per second");
        }

        /// <summary>
        /// Test memory usage during fuzzing
        /// </summary>
        [Fact]
        public void MemoryUsageTest()
        {
            // Arrange
            var config = new FuzzerConfiguration
            {
                NefPath = _nefPath,
                ManifestPath = _manifestPath,
                OutputDirectory = _outputPath,
                IterationsPerMethod = 20,
                SaveInputsAndResults = true,
                EnableCoverage = false, // Disable coverage for performance testing
                RandomSeed = 12345, // Fixed seed for reproducibility
                EnableEnhancedParameterGeneration = true,
                EnableBoundaryValueTesting = true,
                EnableEdgeCaseGeneration = true,
                EnableEnhancedReporting = true,
                ReportFormats = new[] { FuzzerConfiguration.ReportFormat.Markdown }
            };
            
            var fuzzer = new SmartContractFuzzer(config);
            
            // Force garbage collection before test
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            // Get initial memory usage
            long initialMemory = GC.GetTotalMemory(true);
            
            // Act
            bool result = fuzzer.FuzzContract(_nefPath, _manifestPath);
            
            // Get final memory usage
            long finalMemory = GC.GetTotalMemory(false);
            long memoryUsed = finalMemory - initialMemory;
            
            // Force garbage collection after test
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            long memoryAfterGC = GC.GetTotalMemory(true);
            long memoryLeaked = memoryAfterGC - initialMemory;
            
            // Assert
            Assert.IsTrue(result);
            
            // Report memory usage
            TestContext.WriteLine($"Memory Usage (20 iterations per method):");
            TestContext.WriteLine($"Initial Memory: {initialMemory / 1024 / 1024:F2} MB");
            TestContext.WriteLine($"Final Memory: {finalMemory / 1024 / 1024:F2} MB");
            TestContext.WriteLine($"Memory Used: {memoryUsed / 1024 / 1024:F2} MB");
            TestContext.WriteLine($"Memory After GC: {memoryAfterGC / 1024 / 1024:F2} MB");
            TestContext.WriteLine($"Memory Leaked: {memoryLeaked / 1024 / 1024:F2} MB");
            
            // Ensure memory usage is reasonable
            Assert.IsTrue(memoryUsed < 1024 * 1024 * 100, $"Memory usage is too high: {memoryUsed / 1024 / 1024:F2} MB");
            // Ensure memory leak is minimal
            Assert.IsTrue(memoryLeaked < 1024 * 1024 * 10, $"Memory leak detected: {memoryLeaked / 1024 / 1024:F2} MB");
        }

        /// <summary>
        /// Test scalability with a large number of iterations
        /// </summary>
        [Fact(Skip = "Long-running test, run manually")]
        public void ScalabilityTest()
        {
            // Arrange
            var config = new FuzzerConfiguration
            {
                NefPath = _nefPath,
                ManifestPath = _manifestPath,
                OutputDirectory = _outputPath,
                IterationsPerMethod = 1000, // Large number of iterations
                SaveInputsAndResults = false, // Disable saving to avoid disk I/O overhead
                EnableCoverage = false, // Disable coverage for performance testing
                RandomSeed = 12345, // Fixed seed for reproducibility
                EnableEnhancedParameterGeneration = true,
                EnableBoundaryValueTesting = true,
                EnableEdgeCaseGeneration = true,
                EnableEnhancedReporting = true,
                ReportFormats = new[] { FuzzerConfiguration.ReportFormat.Markdown }
            };
            
            var fuzzer = new SmartContractFuzzer(config);
            
            // Act
            var stopwatch = Stopwatch.StartNew();
            bool result = fuzzer.FuzzContract(_nefPath, _manifestPath);
            stopwatch.Stop();
            
            // Assert
            Assert.IsTrue(result);
            
            // Report performance
            TestContext.WriteLine($"Scalability Test (1000 iterations per method):");
            TestContext.WriteLine($"Total Time: {stopwatch.Elapsed}");
            
            // Calculate executions per second
            int totalMethods = fuzzer.GetMethodCount();
            int totalExecutions = totalMethods * config.IterationsPerMethod;
            double executionsPerSecond = totalExecutions / stopwatch.Elapsed.TotalSeconds;
            
            TestContext.WriteLine($"Methods: {totalMethods}");
            TestContext.WriteLine($"Total Executions: {totalExecutions}");
            TestContext.WriteLine($"Executions per Second: {executionsPerSecond:F2}");
            
            // Ensure fuzzing remains reasonably fast even with many iterations
            Assert.IsTrue(executionsPerSecond > 1, $"Fuzzing performance is too slow: {executionsPerSecond:F2} executions per second");
        }
    }
}