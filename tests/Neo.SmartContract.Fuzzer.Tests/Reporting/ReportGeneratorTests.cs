using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.Controller;
using Neo.SmartContract.Fuzzer.Reporting;
using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;

namespace Neo.SmartContract.Fuzzer.Tests.Reporting
{
    [TestClass]
    public class ReportGeneratorTests
    {
        private string _tempDirectory;
        private ReportGenerator _reportGenerator;
        private FuzzerConfiguration _config;
        private List<string> _formats;

        [TestInitialize]
        public void Setup()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), "NeoFuzzerTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);

            _config = new FuzzerConfiguration
            {
                NefPath = "test.nef",
                ManifestPath = "test.manifest.json",
                OutputDirectory = _tempDirectory
            };

            _formats = new List<string> { "json", "text" };
            _reportGenerator = new ReportGenerator(_tempDirectory, _formats, _config);
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
        public void GenerateReports_CreatesReportFiles()
        {
            // Arrange
            var issues = new List<IssueReport>
            {
                new IssueReport
                {
                    IssueType = "Test Issue",
                    Severity = IssueSeverity.High,
                    Description = "Test description",
                    MethodName = "TestMethod"
                }
            };

            var status = new FuzzingStatus
            {
                TotalExecutions = 100,
                SuccessfulExecutions = 90,
                FailedExecutions = 10,
                IssuesFound = 1,
                CodeCoverage = 0.8
            };

            // Act
            _reportGenerator.GenerateReports(issues, status);

            // Assert
            File.Exists(Path.Combine(_tempDirectory, "report.json")).Should().BeTrue();
            File.Exists(Path.Combine(_tempDirectory, "report.txt")).Should().BeTrue();
        }

        [TestMethod]
        public void GenerateReports_JsonFormat_CreatesValidJsonFile()
        {
            // Arrange
            var issues = new List<IssueReport>
            {
                new IssueReport
                {
                    IssueType = "Test Issue",
                    Severity = IssueSeverity.High,
                    Description = "Test description",
                    MethodName = "TestMethod"
                }
            };

            var status = new FuzzingStatus
            {
                TotalExecutions = 100,
                SuccessfulExecutions = 90,
                FailedExecutions = 10,
                IssuesFound = 1,
                CodeCoverage = 0.8
            };

            // Act
            _reportGenerator.GenerateReports(issues, status);

            // Assert
            string jsonPath = Path.Combine(_tempDirectory, "report.json");
            File.Exists(jsonPath).Should().BeTrue();
            
            string jsonContent = File.ReadAllText(jsonPath);
            jsonContent.Should().Contain("\"Contract\":");
            jsonContent.Should().Contain("\"Issues\":");
            jsonContent.Should().Contain("\"Test Issue\"");
        }

        [TestMethod]
        public void GenerateReports_TextFormat_CreatesValidTextFile()
        {
            // Arrange
            var issues = new List<IssueReport>
            {
                new IssueReport
                {
                    IssueType = "Test Issue",
                    Severity = IssueSeverity.High,
                    Description = "Test description",
                    MethodName = "TestMethod"
                }
            };

            var status = new FuzzingStatus
            {
                TotalExecutions = 100,
                SuccessfulExecutions = 90,
                FailedExecutions = 10,
                IssuesFound = 1,
                CodeCoverage = 0.8
            };

            // Act
            _reportGenerator.GenerateReports(issues, status);

            // Assert
            string textPath = Path.Combine(_tempDirectory, "report.txt");
            File.Exists(textPath).Should().BeTrue();
            
            string textContent = File.ReadAllText(textPath);
            textContent.Should().Contain("Neo Smart Contract Fuzzing Report");
            textContent.Should().Contain("Test Issue");
            textContent.Should().Contain("Test description");
        }

        [TestMethod]
        public void GenerateReports_NoIssues_CreatesReportsWithNoIssuesMessage()
        {
            // Arrange
            var issues = new List<IssueReport>();

            var status = new FuzzingStatus
            {
                TotalExecutions = 100,
                SuccessfulExecutions = 100,
                FailedExecutions = 0,
                IssuesFound = 0,
                CodeCoverage = 0.9
            };

            // Act
            _reportGenerator.GenerateReports(issues, status);

            // Assert
            string textPath = Path.Combine(_tempDirectory, "report.txt");
            File.Exists(textPath).Should().BeTrue();
            
            string textContent = File.ReadAllText(textPath);
            textContent.Should().Contain("No issues found");
        }

        [TestMethod]
        public void GenerateReports_UnsupportedFormat_LogsWarning()
        {
            // Arrange
            var issues = new List<IssueReport>();
            var status = new FuzzingStatus();
            
            var formats = new List<string> { "unsupported" };
            var reportGenerator = new ReportGenerator(_tempDirectory, formats, _config);

            // Act & Assert
            // We can't directly test console output, but we can verify that no exception is thrown
            Action act = () => reportGenerator.GenerateReports(issues, status);
            act.Should().NotThrow();
        }
    }
}
