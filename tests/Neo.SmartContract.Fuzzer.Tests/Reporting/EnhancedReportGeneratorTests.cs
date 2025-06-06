using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neo.SmartContract.Fuzzer.Reporting;
using Neo.SmartContract.Manifest;
using Neo.VM.Types;
using Xunit;

namespace Neo.SmartContract.Fuzzer.Tests.Reporting
{
    /// <summary>
    /// Tests for the EnhancedReportGenerator class
    /// </summary>
    public class EnhancedReportGeneratorTests
    {
        private readonly string _testDataPath = Path.Combine("TestData");
        private readonly string _outputPath;
        private readonly string _manifestPath;
        private readonly List<ExecutionResult> _results;
        private readonly ContractMethodDescriptor[] _methods;

        /// <summary>
        /// Initialize test data
        /// </summary>
        public EnhancedReportGeneratorTests()
        {
            _manifestPath = Path.Combine(_testDataPath, "SampleToken.manifest.json");
            _outputPath = Path.Combine(Path.GetTempPath(), "FuzzerReportTest");
            
            // Create test directory
            Directory.CreateDirectory(_outputPath);
            
            // Create test data
            var manifestJson = File.ReadAllText(_manifestPath);
            var manifest = ContractManifest.Parse(manifestJson);
            _methods = manifest.Abi.Methods;
            
            _results = new List<ExecutionResult>
            {
                new ExecutionResult
                {
                    MethodName = "transfer",
                    Parameters = new StackItem[] { new ByteString(new byte[20]), new ByteString(new byte[20]), new Integer(100) },
                    Success = true,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMilliseconds(50),
                    GasConsumed = 1000,
                    ReturnValue = StackItem.True
                },
                new ExecutionResult
                {
                    MethodName = "transfer",
                    Parameters = new StackItem[] { new ByteString(new byte[20]), new ByteString(new byte[20]), new Integer(200) },
                    Success = true,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMilliseconds(60),
                    GasConsumed = 1200,
                    ReturnValue = StackItem.True
                },
                new ExecutionResult
                {
                    MethodName = "transfer",
                    Parameters = new StackItem[] { new ByteString(new byte[20]), new ByteString(new byte[20]), new Integer(-100) },
                    Success = false,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMilliseconds(30),
                    ErrorMessage = "Negative amount"
                },
                new ExecutionResult
                {
                    MethodName = "balanceOf",
                    Parameters = new StackItem[] { new ByteString(new byte[20]) },
                    Success = true,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMilliseconds(20),
                    GasConsumed = 500,
                    ReturnValue = new Integer(1000)
                }
            };
        }

        /// <summary>
        /// Clean up test resources
        /// </summary>
        ~EnhancedReportGeneratorTests()
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
        /// Test generation of default report
        /// </summary>
        [Fact]
        public void DefaultReportGenerationTest()
        {
            // Arrange
            var generator = new EnhancedReportGenerator(_outputPath, "SampleToken", _methods, _results, 100);
            
            // Act
            string reportPath = generator.GenerateReport();
            
            // Assert
            Assert.True(File.Exists(reportPath));
            Assert.EndsWith(".md", reportPath);
            
            // Check content
            string content = File.ReadAllText(reportPath);
            Assert.Contains("# Fuzzing Report: SampleToken", content);
            Assert.Contains("## Overall Metrics", content);
            Assert.Contains("## Method Metrics", content);
            Assert.Contains("## Detailed Method Analysis", content);
        }

        /// <summary>
        /// Test generation of multiple report formats
        /// </summary>
        [Fact]
        public void MultipleReportFormatsTest()
        {
            // Arrange
            var generator = new EnhancedReportGenerator(_outputPath, "SampleToken", _methods, _results, 100);
            var formats = new[]
            {
                ReportFormatter.FormatType.Markdown,
                ReportFormatter.FormatType.Json,
                ReportFormatter.FormatType.Html
            };
            
            // Act
            var reportPaths = generator.GenerateReports(formats);
            
            // Assert
            Assert.Equal(3, reportPaths.Count);
            Assert.True(reportPaths.ContainsKey(ReportFormatter.FormatType.Markdown));
            Assert.True(reportPaths.ContainsKey(ReportFormatter.FormatType.Json));
            Assert.True(reportPaths.ContainsKey(ReportFormatter.FormatType.Html));
            
            // Check that all files exist
            foreach (var path in reportPaths.Values)
            {
                Assert.True(File.Exists(path));
            }
            
            // Check file extensions
            Assert.EndsWith(".md", reportPaths[ReportFormatter.FormatType.Markdown]);
            Assert.EndsWith(".json", reportPaths[ReportFormatter.FormatType.Json]);
            Assert.EndsWith(".html", reportPaths[ReportFormatter.FormatType.Html]);
        }

        /// <summary>
        /// Test access to metrics collector
        /// </summary>
        [Fact]
        public void MetricsCollectorAccessTest()
        {
            // Arrange
            var generator = new EnhancedReportGenerator(_outputPath, "SampleToken", _methods, _results, 100);
            
            // Act
            var metricsCollector = generator.GetMetricsCollector();
            
            // Assert
            Assert.NotNull(metricsCollector);
            
            // Check that metrics are correctly calculated
            var overallMetrics = metricsCollector.GetOverallMetrics();
            Assert.Equal(4, overallMetrics.TotalExecutions);
            Assert.Equal(3, overallMetrics.SuccessfulExecutions);
            Assert.Equal(1, overallMetrics.FailedExecutions);
            
            // Check method metrics
            var methodMetrics = metricsCollector.GetAllMethodMetrics();
            Assert.Equal(2, methodMetrics.Count);
            Assert.True(methodMetrics.ContainsKey("transfer"));
            Assert.True(methodMetrics.ContainsKey("balanceOf"));
        }

        /// <summary>
        /// Test creation of reports directory
        /// </summary>
        [Fact]
        public void ReportsDirectoryCreationTest()
        {
            // Arrange
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            
            try
            {
                // Act
                var generator = new EnhancedReportGenerator(tempPath, "SampleToken", _methods, _results, 100);
                generator.GenerateReport();
                
                // Assert
                string reportsDirectory = Path.Combine(tempPath, "Reports");
                Assert.True(Directory.Exists(reportsDirectory));
                
                // Check that report file was created
                string[] files = Directory.GetFiles(reportsDirectory);
                Assert.Single(files);
                Assert.EndsWith(".md", files[0]);
            }
            finally
            {
                // Clean up
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }
            }
        }

        /// <summary>
        /// Test handling of invalid parameters
        /// </summary>
        [Fact]
        public void InvalidParametersTest()
        {
            // Arrange & Act & Assert
            
            // Null output directory
            Assert.Throws<ArgumentNullException>(() => new EnhancedReportGenerator(null, "SampleToken", _methods, _results, 100));
            
            // Null contract name
            Assert.Throws<ArgumentNullException>(() => new EnhancedReportGenerator(_outputPath, null, _methods, _results, 100));
            
            // Null methods
            Assert.Throws<ArgumentNullException>(() => new EnhancedReportGenerator(_outputPath, "SampleToken", null, _results, 100));
            
            // Null results
            Assert.Throws<ArgumentNullException>(() => new EnhancedReportGenerator(_outputPath, "SampleToken", _methods, null, 100));
        }
    }
}