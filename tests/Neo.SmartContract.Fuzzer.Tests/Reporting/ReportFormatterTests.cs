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
    /// Tests for the ReportFormatter class
    /// </summary>
    public class ReportFormatterTests
    {
        private readonly string _testDataPath = Path.Combine("TestData");
        private readonly string _outputPath;
        private readonly string _manifestPath;
        private readonly MetricsCollector _metricsCollector;

        /// <summary>
        /// Initialize test data
        /// </summary>
        public ReportFormatterTests()
        {
            _manifestPath = Path.Combine(_testDataPath, "SampleToken.manifest.json");
            _outputPath = Path.Combine(Path.GetTempPath(), "FuzzerReportTest");
            
            // Create test directory
            Directory.CreateDirectory(_outputPath);
            
            // Create test data
            var manifestJson = File.ReadAllText(_manifestPath);
            var manifest = ContractManifest.Parse(manifestJson);
            
            var results = new List<ExecutionResult>
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
            
            _metricsCollector = new MetricsCollector(results, manifest.Abi.Methods);
        }

        /// <summary>
        /// Clean up test resources
        /// </summary>
        ~ReportFormatterTests()
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
        /// Test generation of Markdown report
        /// </summary>
        [Fact]
        public void MarkdownReportGenerationTest()
        {
            // Arrange
            var formatter = new ReportFormatter("SampleToken", _metricsCollector, _outputPath);
            
            // Act
            string reportPath = formatter.GenerateReport(ReportFormatter.FormatType.Markdown);
            
            // Assert
            Assert.True(File.Exists(reportPath));
            Assert.EndsWith(".md", reportPath);
            
            // Check content
            string content = File.ReadAllText(reportPath);
            Assert.Contains("# Fuzzing Report: SampleToken", content);
            Assert.Contains("## Overall Metrics", content);
            Assert.Contains("## Method Metrics", content);
            Assert.Contains("## Detailed Method Analysis", content);
            Assert.Contains("### transfer", content);
            Assert.Contains("### balanceOf", content);
            Assert.Contains("## Recommendations", content);
        }

        /// <summary>
        /// Test generation of JSON report
        /// </summary>
        [Fact]
        public void JsonReportGenerationTest()
        {
            // Arrange
            var formatter = new ReportFormatter("SampleToken", _metricsCollector, _outputPath);
            
            // Act
            string reportPath = formatter.GenerateReport(ReportFormatter.FormatType.Json);
            
            // Assert
            Assert.True(File.Exists(reportPath));
            Assert.EndsWith(".json", reportPath);
            
            // Check content
            string content = File.ReadAllText(reportPath);
            Assert.Contains("\"ContractName\"", content);
            Assert.Contains("\"GeneratedAt\"", content);
            Assert.Contains("\"OverallMetrics\"", content);
            Assert.Contains("\"MethodMetrics\"", content);
            Assert.Contains("\"transfer\"", content);
            Assert.Contains("\"balanceOf\"", content);
        }

        /// <summary>
        /// Test generation of HTML report
        /// </summary>
        [Fact]
        public void HtmlReportGenerationTest()
        {
            // Arrange
            var formatter = new ReportFormatter("SampleToken", _metricsCollector, _outputPath);
            
            // Act
            string reportPath = formatter.GenerateReport(ReportFormatter.FormatType.Html);
            
            // Assert
            Assert.True(File.Exists(reportPath));
            Assert.EndsWith(".html", reportPath);
            
            // Check content
            string content = File.ReadAllText(reportPath);
            Assert.Contains("<!DOCTYPE html>", content);
            Assert.Contains("<title>Fuzzing Report: SampleToken</title>", content);
            Assert.Contains("<h2>Overall Metrics</h2>", content);
            Assert.Contains("<h2>Method Metrics</h2>", content);
            Assert.Contains("<h2>Detailed Method Analysis</h2>", content);
            Assert.Contains("<h3>transfer</h3>", content);
            Assert.Contains("<h3>balanceOf</h3>", content);
            Assert.Contains("<h2>Recommendations</h2>", content);
        }

        /// <summary>
        /// Test generation of CSV report
        /// </summary>
        [Fact]
        public void CsvReportGenerationTest()
        {
            // Arrange
            var formatter = new ReportFormatter("SampleToken", _metricsCollector, _outputPath);
            
            // Act
            string reportPath = formatter.GenerateReport(ReportFormatter.FormatType.Csv);
            
            // Assert
            Assert.True(File.Exists(reportPath));
            Assert.EndsWith(".csv", reportPath);
            
            // Check content
            string content = File.ReadAllText(reportPath);
            Assert.Contains("Method,Parameters,ReturnType,Safe,TotalExecutions,SuccessfulExecutions,FailedExecutions,SuccessRate", content);
            Assert.Contains("transfer,", content);
            Assert.Contains("balanceOf,", content);
            Assert.Contains("Method,ErrorType,Count,Percentage", content);
        }

        /// <summary>
        /// Test generation of recommendations
        /// </summary>
        [Fact]
        public void RecommendationsGenerationTest()
        {
            // Arrange
            var formatter = new ReportFormatter("SampleToken", _metricsCollector, _outputPath);
            
            // Act
            string reportPath = formatter.GenerateReport(ReportFormatter.FormatType.Markdown);
            
            // Assert
            Assert.True(File.Exists(reportPath));
            
            // Check recommendations
            string content = File.ReadAllText(reportPath);
            Assert.Contains("## Recommendations", content);
            
            // The transfer method has a 2/3 success rate, which is below 70%
            Assert.Contains("Method `transfer` has a low success rate", content);
        }

        /// <summary>
        /// Test handling of invalid format
        /// </summary>
        [Fact]
        public void InvalidFormatHandlingTest()
        {
            // Arrange
            var formatter = new ReportFormatter("SampleToken", _metricsCollector, _outputPath);
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => formatter.GenerateReport((ReportFormatter.FormatType)999));
        }
    }
}