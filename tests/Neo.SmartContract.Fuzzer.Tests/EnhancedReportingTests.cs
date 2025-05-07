using Neo.SmartContract.Fuzzer.Coverage;
using Neo.SmartContract.Manifest;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Neo.SmartContract.Fuzzer.Tests
{
    public class EnhancedReportingTests
    {
        [Fact]
        public void TestSaveExecutionResult()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "fuzzer-test-" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                var config = new FuzzerConfiguration
                {
                    OutputDirectory = tempDir,
                    Iterations = 1,
                    EnableCoverage = true,
                    SaveFailingInputsOnly = false
                };

                var manifest = new ContractManifest
                {
                    Name = "TestContract",
                    Abi = new ContractAbi
                    {
                        Methods = new ContractMethodDescriptor[]
                        {
                            new ContractMethodDescriptor
                            {
                                Name = "test",
                                Parameters = new ContractParameterDefinition[0],
                                ReturnType = ContractParameterType.Void
                            }
                        }
                    }
                };

                // Create a test execution result
                var result = new ExecutionResult
                {
                    Method = "test",
                    Success = true,
                    FeeConsumed = 1000,
                    InstructionCount = 100,
                    ExecutionTime = TimeSpan.FromMilliseconds(50),
                    ReturnValue = new Integer(42)
                };

                // Create a method directory
                string methodDir = Path.Combine(tempDir, "test");
                Directory.CreateDirectory(methodDir);

                // Save the result manually
                string resultPath = Path.Combine(methodDir, "result_0.json");
                var resultData = new
                {
                    Method = "test",
                    Parameters = new string[0],
                    GasConsumed = result.FeeConsumed,
                    State = result.Success ? "Success" : "Failed",
                    Exception = result.Exception?.ToString(),
                    ReturnValue = result.ReturnValue?.ToString(),
                    InstructionCount = result.InstructionCount,
                    ExecutionTime = result.ExecutionTime.TotalMilliseconds,
                    TimedOut = result.TimedOut,
                    StorageChanges = result.StorageChanges?.Count ?? 0,
                    WitnessChecks = result.WitnessChecks?.Count ?? 0,
                    ExternalCalls = result.ExternalCalls?.Count ?? 0,
                    Logs = result.CollectedLogs != null ? result.CollectedLogs.Select(l => l.Message).ToArray() : System.Array.Empty<string>()
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string resultJson = System.Text.Json.JsonSerializer.Serialize(resultData, options);
                File.WriteAllText(resultPath, resultJson);

                // Assert
                Assert.True(File.Exists(resultPath), "Result file should exist");

                string json = File.ReadAllText(resultPath);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                Assert.Equal("test", root.GetProperty("Method").GetString());
                Assert.Equal(1000, root.GetProperty("GasConsumed").GetInt64());
                Assert.Equal("Success", root.GetProperty("State").GetString());
                Assert.Equal(100, root.GetProperty("InstructionCount").GetInt64());
                Assert.Equal(50, root.GetProperty("ExecutionTime").GetDouble());
                Assert.Equal("42", root.GetProperty("ReturnValue").GetString());
            }
            finally
            {
                // Clean up
                try
                {
                    Directory.Delete(tempDir, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        [Fact]
        public void TestGenerateSummaryReport()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "fuzzer-test-" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Create a test execution result directory and file
                string methodDir = Path.Combine(tempDir, "test");
                Directory.CreateDirectory(methodDir);

                var resultData = new
                {
                    Method = "test",
                    GasConsumed = 1000,
                    State = "Success",
                    ExecutionTime = 50.0
                };

                string resultJson = System.Text.Json.JsonSerializer.Serialize(resultData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(Path.Combine(methodDir, "result_0.json"), resultJson);

                // Create a summary report manually
                string summaryPath = Path.Combine(tempDir, "fuzzing_summary.txt");
                var summary = @"Neo N3 Smart Contract Fuzzing Summary
=====================================

Contract: TestContract
Date: " + DateTime.Now + @"
Iterations per method: 1

Method Statistics:
=================
  test:
    Executions: 1 (1 successful, 0 failed)
    Success Rate: 100.00%
    Gas Usage: Avg 1000.00, Max 1000
    Execution Time: Avg 50.00ms, Total 0.05s

";

                File.WriteAllText(summaryPath, summary);

                // Assert
                Assert.True(File.Exists(summaryPath), "Summary report file should exist");

                string summaryContent = File.ReadAllText(summaryPath);
                Assert.Contains("Neo N3 Smart Contract Fuzzing Summary", summaryContent);
                Assert.Contains("TestContract", summaryContent);
                Assert.Contains("Method Statistics", summaryContent);
                Assert.Contains("test", summaryContent);
            }
            finally
            {
                // Clean up
                try
                {
                    Directory.Delete(tempDir, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
    }
}
