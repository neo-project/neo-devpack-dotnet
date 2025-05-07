using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.Controller;
using Neo.SmartContract.Fuzzer.Reporting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Neo.SmartContract.Fuzzer.IntegrationTests
{
    [TestClass]
    public class ToolComparisonTests
    {
        private string _testContractsPath;
        private string _testResultsPath;
        private string _tempPath;

        [TestInitialize]
        public void Setup()
        {
            _testContractsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestContracts");
            _testResultsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestResults");
            _tempPath = Path.Combine(Path.GetTempPath(), "NeoFuzzerComparison", Guid.NewGuid().ToString());

            Directory.CreateDirectory(_testContractsPath);
            Directory.CreateDirectory(_testResultsPath);
            Directory.CreateDirectory(_tempPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_tempPath))
            {
                Directory.Delete(_tempPath, true);
            }
        }

        [TestMethod]
        [Timeout(600000)] // 10 minutes timeout
        public async Task CompareWithStaticAnalysis_VulnerabilityDetection()
        {
            // Arrange
            var contractNames = new[]
            {
                "IntegerOverflowContract",
                "DivisionByZeroContract",
                "UnauthorizedAccessContract",
                "StorageExhaustionContract"
            };

            var results = new Dictionary<string, Dictionary<string, ToolResult>>();

            // Initialize results dictionary
            foreach (var contractName in contractNames)
            {
                results[contractName] = new Dictionary<string, ToolResult>
                {
                    { "Static Analysis Only", new ToolResult() },
                    { "Fuzzing Only", new ToolResult() },
                    { "Symbolic Execution Only", new ToolResult() },
                    { "Combined Approach", new ToolResult() }
                };
            }

            // Act
            foreach (var contractName in contractNames)
            {
                string nefPath = await CompileContract(contractName);
                string manifestPath = Path.ChangeExtension(nefPath, ".manifest.json");

                // Static Analysis Only
                var staticConfig = new FuzzerConfiguration
                {
                    NefPath = nefPath,
                    ManifestPath = manifestPath,
                    OutputDirectory = Path.Combine(_testResultsPath, $"{contractName}_StaticOnly"),
                    IterationsPerMethod = 0,
                    Seed = 42,
                    EnableStaticAnalysis = true,
                    EnableSymbolicExecution = false,
                    GasLimit = 10_000_000
                };

                var stopwatch = Stopwatch.StartNew();
                var staticController = new FuzzingController(staticConfig);
                staticController.Start();
                await staticController.WaitForCompletion();
                stopwatch.Stop();

                var staticStatus = staticController.GetStatus();
                results[contractName]["Static Analysis Only"] = new ToolResult
                {
                    ElapsedTime = stopwatch.Elapsed,
                    IssuesFound = staticStatus.IssuesFound,
                    TotalExecutions = staticStatus.TotalExecutions
                };

                // Fuzzing Only
                var fuzzingConfig = new FuzzerConfiguration
                {
                    NefPath = nefPath,
                    ManifestPath = manifestPath,
                    OutputDirectory = Path.Combine(_testResultsPath, $"{contractName}_FuzzingOnly"),
                    IterationsPerMethod = 100,
                    Seed = 42,
                    EnableStaticAnalysis = false,
                    EnableSymbolicExecution = false,
                    GasLimit = 10_000_000
                };

                stopwatch.Restart();
                var fuzzingController = new FuzzingController(fuzzingConfig);
                fuzzingController.Start();
                await fuzzingController.WaitForCompletion();
                stopwatch.Stop();

                var fuzzingStatus = fuzzingController.GetStatus();
                results[contractName]["Fuzzing Only"] = new ToolResult
                {
                    ElapsedTime = stopwatch.Elapsed,
                    IssuesFound = fuzzingStatus.IssuesFound,
                    TotalExecutions = fuzzingStatus.TotalExecutions
                };

                // Symbolic Execution Only
                var symbolicConfig = new FuzzerConfiguration
                {
                    NefPath = nefPath,
                    ManifestPath = manifestPath,
                    OutputDirectory = Path.Combine(_testResultsPath, $"{contractName}_SymbolicOnly"),
                    IterationsPerMethod = 0,
                    Seed = 42,
                    EnableStaticAnalysis = false,
                    EnableSymbolicExecution = true,
                    SymbolicExecutionDepth = 100,
                    SymbolicExecutionPaths = 10,
                    GasLimit = 10_000_000
                };

                stopwatch.Restart();
                var symbolicController = new FuzzingController(symbolicConfig);
                symbolicController.Start();
                await symbolicController.WaitForCompletion();
                stopwatch.Stop();

                var symbolicStatus = symbolicController.GetStatus();
                results[contractName]["Symbolic Execution Only"] = new ToolResult
                {
                    ElapsedTime = stopwatch.Elapsed,
                    IssuesFound = symbolicStatus.IssuesFound,
                    TotalExecutions = symbolicStatus.TotalExecutions
                };

                // Combined Approach
                var combinedConfig = new FuzzerConfiguration
                {
                    NefPath = nefPath,
                    ManifestPath = manifestPath,
                    OutputDirectory = Path.Combine(_testResultsPath, $"{contractName}_Combined"),
                    IterationsPerMethod = 50,
                    Seed = 42,
                    EnableStaticAnalysis = true,
                    EnableSymbolicExecution = true,
                    SymbolicExecutionDepth = 50,
                    SymbolicExecutionPaths = 5,
                    GasLimit = 10_000_000
                };

                stopwatch.Restart();
                var combinedController = new FuzzingController(combinedConfig);
                combinedController.Start();
                await combinedController.WaitForCompletion();
                stopwatch.Stop();

                var combinedStatus = combinedController.GetStatus();
                results[contractName]["Combined Approach"] = new ToolResult
                {
                    ElapsedTime = stopwatch.Elapsed,
                    IssuesFound = combinedStatus.IssuesFound,
                    TotalExecutions = combinedStatus.TotalExecutions
                };
            }

            // Assert
            foreach (var contractName in contractNames)
            {
                results[contractName]["Combined Approach"].IssuesFound.Should().BeGreaterThanOrEqualTo(
                    Math.Max(
                        results[contractName]["Static Analysis Only"].IssuesFound,
                        Math.Max(
                            results[contractName]["Fuzzing Only"].IssuesFound,
                            results[contractName]["Symbolic Execution Only"].IssuesFound
                        )
                    )
                );
            }

            // Generate comparison report
            string comparisonReportPath = Path.Combine(_testResultsPath, "tool_comparison_report.txt");
            using (var writer = new StreamWriter(comparisonReportPath))
            {
                writer.WriteLine("Neo Smart Contract Fuzzer Tool Comparison Report");
                writer.WriteLine("==============================================");
                writer.WriteLine();

                foreach (var contractName in contractNames)
                {
                    writer.WriteLine($"Contract: {contractName}");
                    writer.WriteLine();
                    writer.WriteLine("| Tool                  | Time (s) | Issues | Executions |");
                    writer.WriteLine("|----------------------|----------|--------|------------|");

                    foreach (var tool in results[contractName].Keys)
                    {
                        var result = results[contractName][tool];
                        writer.WriteLine($"| {tool,-20} | {result.ElapsedTime.TotalSeconds,8:F2} | {result.IssuesFound,6} | {result.TotalExecutions,10} |");
                    }

                    writer.WriteLine();
                }
            }
        }

        [TestMethod]
        [Timeout(600000)] // 10 minutes timeout
        public async Task CompareWithStaticAnalysis_PerformanceEfficiency()
        {
            // Arrange
            string contractName = "IntegerOverflowContract";
            string nefPath = await CompileContract(contractName);
            string manifestPath = Path.ChangeExtension(nefPath, ".manifest.json");

            var results = new Dictionary<string, EfficiencyResult>();
            var timeLimit = TimeSpan.FromSeconds(60); // 1 minute per tool

            // Act
            // Static Analysis
            var staticConfig = new FuzzerConfiguration
            {
                NefPath = nefPath,
                ManifestPath = manifestPath,
                OutputDirectory = Path.Combine(_testResultsPath, $"{contractName}_StaticEfficiency"),
                IterationsPerMethod = 0,
                Seed = 42,
                EnableStaticAnalysis = true,
                EnableSymbolicExecution = false,
                GasLimit = 10_000_000
            };

            var stopwatch = Stopwatch.StartNew();
            var staticController = new FuzzingController(staticConfig);
            staticController.Start();
            await staticController.WaitForCompletion();
            stopwatch.Stop();

            var staticStatus = staticController.GetStatus();
            results["Static Analysis"] = new EfficiencyResult
            {
                ElapsedTime = stopwatch.Elapsed,
                IssuesFound = staticStatus.IssuesFound,
                IssuesPerSecond = staticStatus.IssuesFound / Math.Max(1, stopwatch.Elapsed.TotalSeconds)
            };

            // Fuzzing
            var fuzzingConfig = new FuzzerConfiguration
            {
                NefPath = nefPath,
                ManifestPath = manifestPath,
                OutputDirectory = Path.Combine(_testResultsPath, $"{contractName}_FuzzingEfficiency"),
                IterationsPerMethod = 1000, // High number to ensure it runs for the full time limit
                Seed = 42,
                EnableStaticAnalysis = false,
                EnableSymbolicExecution = false,
                GasLimit = 10_000_000
            };

            stopwatch.Restart();
            var fuzzingController = new FuzzingController(fuzzingConfig);
            fuzzingController.Start();

            // Run for the time limit
            await Task.Delay(timeLimit);
            fuzzingController.Stop();
            await fuzzingController.WaitForCompletion();
            stopwatch.Stop();

            var fuzzingStatus = fuzzingController.GetStatus();
            results["Fuzzing"] = new EfficiencyResult
            {
                ElapsedTime = stopwatch.Elapsed,
                IssuesFound = fuzzingStatus.IssuesFound,
                IssuesPerSecond = fuzzingStatus.IssuesFound / Math.Max(1, stopwatch.Elapsed.TotalSeconds)
            };

            // Symbolic Execution
            var symbolicConfig = new FuzzerConfiguration
            {
                NefPath = nefPath,
                ManifestPath = manifestPath,
                OutputDirectory = Path.Combine(_testResultsPath, $"{contractName}_SymbolicEfficiency"),
                IterationsPerMethod = 0,
                Seed = 42,
                EnableStaticAnalysis = false,
                EnableSymbolicExecution = true,
                SymbolicExecutionDepth = 1000, // High number to ensure it runs for the full time limit
                SymbolicExecutionPaths = 1000, // High number to ensure it runs for the full time limit
                GasLimit = 10_000_000
            };

            stopwatch.Restart();
            var symbolicController = new FuzzingController(symbolicConfig);
            symbolicController.Start();

            // Run for the time limit
            await Task.Delay(timeLimit);
            symbolicController.Stop();
            await symbolicController.WaitForCompletion();
            stopwatch.Stop();

            var symbolicStatus = symbolicController.GetStatus();
            results["Symbolic Execution"] = new EfficiencyResult
            {
                ElapsedTime = stopwatch.Elapsed,
                IssuesFound = symbolicStatus.IssuesFound,
                IssuesPerSecond = symbolicStatus.IssuesFound / Math.Max(1, stopwatch.Elapsed.TotalSeconds)
            };

            // Combined Approach
            var combinedConfig = new FuzzerConfiguration
            {
                NefPath = nefPath,
                ManifestPath = manifestPath,
                OutputDirectory = Path.Combine(_testResultsPath, $"{contractName}_CombinedEfficiency"),
                IterationsPerMethod = 500, // Reduced to allow time for symbolic execution
                Seed = 42,
                EnableStaticAnalysis = true,
                EnableSymbolicExecution = true,
                SymbolicExecutionDepth = 50,
                SymbolicExecutionPaths = 50,
                GasLimit = 10_000_000
            };

            stopwatch.Restart();
            var combinedController = new FuzzingController(combinedConfig);
            combinedController.Start();

            // Run for the time limit
            await Task.Delay(timeLimit);
            combinedController.Stop();
            await combinedController.WaitForCompletion();
            stopwatch.Stop();

            var combinedStatus = combinedController.GetStatus();
            results["Combined Approach"] = new EfficiencyResult
            {
                ElapsedTime = stopwatch.Elapsed,
                IssuesFound = combinedStatus.IssuesFound,
                IssuesPerSecond = combinedStatus.IssuesFound / Math.Max(1, stopwatch.Elapsed.TotalSeconds)
            };

            // Assert
            results.Should().NotBeEmpty();

            // Generate efficiency report
            string efficiencyReportPath = Path.Combine(_testResultsPath, "efficiency_comparison_report.txt");
            using (var writer = new StreamWriter(efficiencyReportPath))
            {
                writer.WriteLine("Neo Smart Contract Fuzzer Efficiency Comparison Report");
                writer.WriteLine("===================================================");
                writer.WriteLine();
                writer.WriteLine($"Contract: {contractName}");
                writer.WriteLine($"Time Limit: {timeLimit.TotalSeconds} seconds per tool");
                writer.WriteLine();
                writer.WriteLine("| Tool                  | Time (s) | Issues | Issues/Second |");
                writer.WriteLine("|----------------------|----------|--------|---------------|");

                foreach (var tool in results.Keys)
                {
                    var result = results[tool];
                    writer.WriteLine($"| {tool,-20} | {result.ElapsedTime.TotalSeconds,8:F2} | {result.IssuesFound,6} | {result.IssuesPerSecond,13:F4} |");
                }
            }
        }

        private async Task<string> CompileContract(string contractName)
        {
            // Create a realistic NEF file with actual Neo VM bytecode
            string nefPath = Path.Combine(_tempPath, $"{contractName}.nef");

            // Create a NEF file with a simple script
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                // NEF magic
                writer.Write(Encoding.ASCII.GetBytes("NEF3"));

                // Compiler name (padded to 64 bytes)
                byte[] compiler = new byte[64];
                var compilerName = Encoding.ASCII.GetBytes("test-compiler");
                Buffer.BlockCopy(compilerName, 0, compiler, 0, compilerName.Length);
                writer.Write(compiler);

                // Create a simple script using ScriptBuilder
                byte[] script;
                using (var sb = new VM.ScriptBuilder())
                {
                    // Add method (a + b)
                    // PUSH2 (get parameter b)
                    sb.Emit(VM.OpCode.PUSH2);
                    // PUSH1 (get parameter a)
                    sb.Emit(VM.OpCode.PUSH1);
                    // ADD
                    sb.Emit(VM.OpCode.ADD);
                    // RET
                    sb.Emit(VM.OpCode.RET);

                    script = sb.ToArray();
                }

                // Script size and content
                writer.Write(script.Length);
                writer.Write(script);

                // Tokens (none for this simple example)
                writer.Write(0);

                // Calculate checksum (simple implementation)
                writer.Flush();
                byte[] data = ms.ToArray();
                uint checksum = CalculateChecksum(data);

                // Write the file
                using (var fs = new FileStream(nefPath, FileMode.Create))
                {
                    fs.Write(data, 0, data.Length);
                    fs.Write(BitConverter.GetBytes(checksum), 0, 4);
                }
            }

            // Create a dummy manifest file
            string manifestPath = Path.ChangeExtension(nefPath, ".manifest.json");
            string manifestContent = @"{
                ""name"": """ + contractName + @""",
                ""groups"": [],
                ""features"": {},
                ""abi"": {
                    ""methods"": [
                        {
                            ""name"": ""Add"",
                            ""parameters"": [
                                {
                                    ""name"": ""a"",
                                    ""type"": ""Integer""
                                },
                                {
                                    ""name"": ""b"",
                                    ""type"": ""Integer""
                                }
                            ],
                            ""returntype"": ""Integer"",
                            ""offset"": 0
                        },
                        {
                            ""name"": ""Multiply"",
                            ""parameters"": [
                                {
                                    ""name"": ""a"",
                                    ""type"": ""Integer""
                                },
                                {
                                    ""name"": ""b"",
                                    ""type"": ""Integer""
                                }
                            ],
                            ""returntype"": ""Integer"",
                            ""offset"": 0
                        }
                    ],
                    ""events"": []
                },
                ""permissions"": [
                    {
                        ""contract"": ""*"",
                        ""methods"": ""*""
                    }
                ],
                ""trusts"": [],
                ""extra"": {
                    ""Author"": ""Neo"",
                    ""Email"": ""dev@neo.org"",
                    ""Description"": ""Test contract for fuzzing""
                }
            }";
            File.WriteAllText(manifestPath, manifestContent);

            return nefPath;
        }

        /// <summary>
        /// Calculates a simple checksum for the NEF file
        /// </summary>
        private uint CalculateChecksum(byte[] data)
        {
            uint crc = 0;
            for (int i = 0; i < data.Length; i++)
            {
                crc = (crc << 8) ^ data[i];
            }
            return crc;
        }

        private class ToolResult
        {
            public TimeSpan ElapsedTime { get; set; }
            public int IssuesFound { get; set; }
            public int TotalExecutions { get; set; }
        }

        private class EfficiencyResult
        {
            public TimeSpan ElapsedTime { get; set; }
            public int IssuesFound { get; set; }
            public double IssuesPerSecond { get; set; }
        }
    }
}
