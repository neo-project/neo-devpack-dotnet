using System;
using System.IO;
using System.Text.Json;
using Xunit;

namespace Neo.SmartContract.Fuzzer.Tests.Core
{
    /// <summary>
    /// Tests for the FuzzerConfiguration class
    /// </summary>
    public class FuzzerConfigurationTests
    {
        private readonly string _testDataPath = Path.Combine("TestData");

        /// <summary>
        /// Test that default configuration values are set correctly
        /// </summary>
        [Fact]
        public void DefaultConfigurationTest()
        {
            // Arrange & Act
            var config = new FuzzerConfiguration();
            
            // Assert
            Assert.Equal(100, config.IterationsPerMethod);
            Assert.True(config.SaveInputsAndResults);
            Assert.True(config.EnableCoverage);
            Assert.Equal("html", config.CoverageFormat);
            Assert.Equal("coverage-report", config.CoverageOutput);
            Assert.True(config.EnableEnhancedReporting);
            Assert.True(config.EnableVulnerabilityDetection);
            Assert.Single(config.ReportFormats);
            Assert.Equal(FuzzerConfiguration.ReportFormat.Markdown, config.ReportFormats[0]);
        }

        /// <summary>
        /// Test loading configuration from a JSON file
        /// </summary>
        [Fact]
        public void LoadConfigurationFromJsonTest()
        {
            // Arrange
            string configPath = Path.Combine(_testDataPath, "test-config.json");
            string configJson = @"{
                ""IterationsPerMethod"": 50,
                ""Seed"": 12345,
                ""GasLimit"": 20000000,
                ""SaveInputsAndResults"": true,
                ""SkipParameterlessMethods"": true,
                ""IncludePrivateMethods"": false,
                ""IncludeMethods"": [""transfer"", ""balanceOf""],
                ""ExcludeMethods"": [""_deploy""],
                ""EnableCoverage"": true,
                ""CoverageFormat"": ""json"",
                ""CoverageOutput"": ""custom-coverage"",
                ""EnableEnhancedReporting"": true,
                ""EnableVulnerabilityDetection"": true,
                ""ReportFormats"": [""Markdown"", ""Json""]
            }";
            
            Directory.CreateDirectory(_testDataPath);
            File.WriteAllText(configPath, configJson);
            
            // Act
            var config = FuzzerConfiguration.FromJsonFile(configPath);
            
            // Assert
            Assert.Equal(50, config.IterationsPerMethod);
            Assert.Equal(12345, config.Seed);
            Assert.Equal(20000000, config.GasLimit);
            Assert.True(config.SaveInputsAndResults);
            Assert.True(config.SkipParameterlessMethods);
            Assert.False(config.IncludePrivateMethods);
            Assert.Equal(2, config.IncludeMethods.Length);
            Assert.Contains("transfer", config.IncludeMethods);
            Assert.Contains("balanceOf", config.IncludeMethods);
            Assert.Single(config.ExcludeMethods);
            Assert.Contains("_deploy", config.ExcludeMethods);
            Assert.True(config.EnableCoverage);
            Assert.Equal("json", config.CoverageFormat);
            Assert.Equal("custom-coverage", config.CoverageOutput);
            Assert.True(config.EnableEnhancedReporting);
            Assert.True(config.EnableVulnerabilityDetection);
            Assert.Equal(2, config.ReportFormats.Length);
            Assert.Contains(FuzzerConfiguration.ReportFormat.Markdown, config.ReportFormats);
            Assert.Contains(FuzzerConfiguration.ReportFormat.Json, config.ReportFormats);
        }

        /// <summary>
        /// Test validation of configuration values
        /// </summary>
        [Fact]
        public void ValidateConfigurationTest()
        {
            // Arrange
            var config = new FuzzerConfiguration
            {
                IterationsPerMethod = 0, // Invalid value
                GasLimit = -1 // Invalid value
            };
            
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => config.Validate());
            Assert.Contains("IterationsPerMethod", exception.Message);
            
            // Fix iterations and test gas limit
            config.IterationsPerMethod = 10;
            exception = Assert.Throws<ArgumentException>(() => config.Validate());
            Assert.Contains("GasLimit", exception.Message);
            
            // Fix gas limit and ensure validation passes
            config.GasLimit = 1000000;
            config.Validate(); // Should not throw
        }

        /// <summary>
        /// Test creating a configuration with command-line arguments
        /// </summary>
        [Fact]
        public void CommandLineArgumentsTest()
        {
            // Arrange
            string[] args = new[]
            {
                "--iterations", "50",
                "--seed", "12345",
                "--gas-limit", "20000000",
                "--no-save",
                "--skip-parameterless",
                "--include-method", "transfer",
                "--include-method", "balanceOf",
                "--exclude-method", "_deploy",
                "--coverage-format", "json",
                "--coverage-output", "custom-coverage",
                "--report-formats", "markdown,json",
                "--disable-vulnerability-detection"
            };
            
            // Act
            var config = FuzzerConfiguration.FromCommandLineArgs(args);
            
            // Assert
            Assert.Equal(50, config.IterationsPerMethod);
            Assert.Equal(12345, config.Seed);
            Assert.Equal(20000000, config.GasLimit);
            Assert.False(config.SaveInputsAndResults);
            Assert.True(config.SkipParameterlessMethods);
            Assert.Equal(2, config.IncludeMethods.Length);
            Assert.Contains("transfer", config.IncludeMethods);
            Assert.Contains("balanceOf", config.IncludeMethods);
            Assert.Single(config.ExcludeMethods);
            Assert.Contains("_deploy", config.ExcludeMethods);
            Assert.True(config.EnableCoverage);
            Assert.Equal("json", config.CoverageFormat);
            Assert.Equal("custom-coverage", config.CoverageOutput);
            Assert.True(config.EnableEnhancedReporting);
            Assert.False(config.EnableVulnerabilityDetection);
            Assert.Equal(2, config.ReportFormats.Length);
            Assert.Contains(FuzzerConfiguration.ReportFormat.Markdown, config.ReportFormats);
            Assert.Contains(FuzzerConfiguration.ReportFormat.Json, config.ReportFormats);
        }
    }
}