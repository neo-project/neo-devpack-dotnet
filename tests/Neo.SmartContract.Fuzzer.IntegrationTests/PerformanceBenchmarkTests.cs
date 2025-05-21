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
    public class PerformanceBenchmarkTests
    {
        private string _testContractsPath;
        private string _testResultsPath;
        private string _tempPath;

        [TestInitialize]
        public void Setup()
        {
            _testContractsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestContracts");
            _testResultsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestResults");
            _tempPath = Path.Combine(Path.GetTempPath(), "NeoFuzzerBenchmarks", Guid.NewGuid().ToString());

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
        public async Task SymbolicExecution_PerformanceBenchmark()
        {
            // Arrange
            string contractName = "IntegerOverflowContract";
            string nefPath = await CompileContract(contractName);
            string manifestPath = Path.ChangeExtension(nefPath, ".manifest.json");

            var benchmarkResults = new List<BenchmarkResult>();
            var depths = new[] { 10, 50, 100 };
            var paths = new[] { 5, 10, 20 };

            // Act
            foreach (int depth in depths)
            {
                foreach (int pathCount in paths)
                {
                    var config = new FuzzerConfiguration
                    {
                        NefPath = nefPath,
                        ManifestPath = manifestPath,
                        OutputDirectory = Path.Combine(_testResultsPath, $"{contractName}_Benchmark_D{depth}_P{pathCount}"),
                        IterationsPerMethod = 10,
                        Seed = 42,
                        EnableStaticAnalysis = false,
                        EnableSymbolicExecution = true,
                        SymbolicExecutionDepth = depth,
                        SymbolicExecutionPaths = pathCount,
                        GasLimit = 10_000_000
                    };

                    var stopwatch = Stopwatch.StartNew();
                    var controller = new FuzzingController(config);
                    controller.Start();
                    await controller.WaitForCompletion();
                    stopwatch.Stop();

                    var status = controller.GetStatus();

                    benchmarkResults.Add(new BenchmarkResult
                    {
                        Depth = depth,
                        PathCount = pathCount,
                        ElapsedTime = stopwatch.Elapsed,
                        IssuesFound = status.IssuesFound,
                        TotalExecutions = status.TotalExecutions,
                        SuccessRate = status.SuccessRate,
                        ExecutionRate = status.ExecutionRate
                    });
                }
            }

            // Assert
            benchmarkResults.Should().NotBeEmpty();

            // Generate benchmark report
            string benchmarkReportPath = Path.Combine(_testResultsPath, "symbolic_execution_benchmark.txt");
            using (var writer = new StreamWriter(benchmarkReportPath))
            {
                writer.WriteLine("Neo Smart Contract Fuzzer Symbolic Execution Benchmark");
                writer.WriteLine("====================================================");
                writer.WriteLine();
                writer.WriteLine("Contract: " + contractName);
                writer.WriteLine();
                writer.WriteLine("| Depth | Paths | Time (s) | Issues | Executions | Success Rate | Exec/s |");
                writer.WriteLine("|-------|-------|----------|--------|------------|--------------|--------|");

                foreach (var result in benchmarkResults.OrderBy(r => r.Depth).ThenBy(r => r.PathCount))
                {
                    writer.WriteLine($"| {result.Depth,5} | {result.PathCount,5} | {result.ElapsedTime.TotalSeconds,8:F2} | {result.IssuesFound,6} | {result.TotalExecutions,10} | {result.SuccessRate,12:P2} | {result.ExecutionRate,6:F2} |");
                }
            }
        }

        [TestMethod]
        [Timeout(600000)] // 10 minutes timeout
        public async Task FeedbackGuidedFuzzing_PerformanceBenchmark()
        {
            // Arrange
            string contractName = "IntegerOverflowContract";
            string nefPath = await CompileContract(contractName);
            string manifestPath = Path.ChangeExtension(nefPath, ".manifest.json");

            var benchmarkResults = new List<BenchmarkResult>();
            var iterations = new[] { 10, 50, 100, 200 };

            // Act
            foreach (int iteration in iterations)
            {
                var config = new FuzzerConfiguration
                {
                    NefPath = nefPath,
                    ManifestPath = manifestPath,
                    OutputDirectory = Path.Combine(_testResultsPath, $"{contractName}_Benchmark_I{iteration}"),
                    IterationsPerMethod = iteration,
                    Seed = 42,
                    EnableStaticAnalysis = false,
                    EnableSymbolicExecution = false,
                    GasLimit = 10_000_000
                };

                var stopwatch = Stopwatch.StartNew();
                var controller = new FuzzingController(config);
                controller.Start();
                await controller.WaitForCompletion();
                stopwatch.Stop();

                var status = controller.GetStatus();

                benchmarkResults.Add(new BenchmarkResult
                {
                    Iterations = iteration,
                    ElapsedTime = stopwatch.Elapsed,
                    IssuesFound = status.IssuesFound,
                    TotalExecutions = status.TotalExecutions,
                    SuccessRate = status.SuccessRate,
                    ExecutionRate = status.ExecutionRate
                });
            }

            // Assert
            benchmarkResults.Should().NotBeEmpty();

            // Generate benchmark report
            string benchmarkReportPath = Path.Combine(_testResultsPath, "feedback_guided_fuzzing_benchmark.txt");
            using (var writer = new StreamWriter(benchmarkReportPath))
            {
                writer.WriteLine("Neo Smart Contract Fuzzer Feedback-Guided Fuzzing Benchmark");
                writer.WriteLine("=======================================================");
                writer.WriteLine();
                writer.WriteLine("Contract: " + contractName);
                writer.WriteLine();
                writer.WriteLine("| Iterations | Time (s) | Issues | Executions | Success Rate | Exec/s |");
                writer.WriteLine("|------------|----------|--------|------------|--------------|--------|");

                foreach (var result in benchmarkResults.OrderBy(r => r.Iterations))
                {
                    writer.WriteLine($"| {result.Iterations,10} | {result.ElapsedTime.TotalSeconds,8:F2} | {result.IssuesFound,6} | {result.TotalExecutions,10} | {result.SuccessRate,12:P2} | {result.ExecutionRate,6:F2} |");
                }
            }
        }

        [TestMethod]
        [Timeout(600000)] // 10 minutes timeout
        public async Task CombinedApproach_PerformanceBenchmark()
        {
            // Arrange
            string contractName = "IntegerOverflowContract";
            string nefPath = await CompileContract(contractName);
            string manifestPath = Path.ChangeExtension(nefPath, ".manifest.json");

            var benchmarkResults = new List<BenchmarkResult>();

            // Test different combinations of techniques
            var configurations = new[]
            {
                new { Name = "Fuzzing Only", EnableFuzzing = true, EnableSymbolic = false, EnableStatic = false },
                new { Name = "Symbolic Only", EnableFuzzing = false, EnableSymbolic = true, EnableStatic = false },
                new { Name = "Static Only", EnableFuzzing = false, EnableSymbolic = false, EnableStatic = true },
                new { Name = "Fuzzing + Symbolic", EnableFuzzing = true, EnableSymbolic = true, EnableStatic = false },
                new { Name = "Fuzzing + Static", EnableFuzzing = true, EnableSymbolic = false, EnableStatic = true },
                new { Name = "Symbolic + Static", EnableFuzzing = false, EnableSymbolic = true, EnableStatic = true },
                new { Name = "All Techniques", EnableFuzzing = true, EnableSymbolic = true, EnableStatic = true }
            };

            // Act
            foreach (var config in configurations)
            {
                var fuzzerConfig = new FuzzerConfiguration
                {
                    NefPath = nefPath,
                    ManifestPath = manifestPath,
                    OutputDirectory = Path.Combine(_testResultsPath, $"{contractName}_Benchmark_{config.Name.Replace(" ", "_")}"),
                    IterationsPerMethod = config.EnableFuzzing ? 50 : 0,
                    Seed = 42,
                    EnableStaticAnalysis = config.EnableStatic,
                    EnableSymbolicExecution = config.EnableSymbolic,
                    SymbolicExecutionDepth = 50,
                    SymbolicExecutionPaths = 10,
                    GasLimit = 10_000_000
                };

                var stopwatch = Stopwatch.StartNew();
                var controller = new FuzzingController(fuzzerConfig);
                controller.Start();
                await controller.WaitForCompletion();
                stopwatch.Stop();

                var status = controller.GetStatus();

                benchmarkResults.Add(new BenchmarkResult
                {
                    Name = config.Name,
                    ElapsedTime = stopwatch.Elapsed,
                    IssuesFound = status.IssuesFound,
                    TotalExecutions = status.TotalExecutions,
                    SuccessRate = status.SuccessRate,
                    ExecutionRate = status.ExecutionRate
                });
            }

            // Assert
            benchmarkResults.Should().NotBeEmpty();

            // Generate benchmark report
            string benchmarkReportPath = Path.Combine(_testResultsPath, "combined_approach_benchmark.txt");
            using (var writer = new StreamWriter(benchmarkReportPath))
            {
                writer.WriteLine("Neo Smart Contract Fuzzer Combined Approach Benchmark");
                writer.WriteLine("===================================================");
                writer.WriteLine();
                writer.WriteLine("Contract: " + contractName);
                writer.WriteLine();
                writer.WriteLine("| Technique          | Time (s) | Issues | Executions | Success Rate | Exec/s |");
                writer.WriteLine("|--------------------+----------+--------+------------+--------------+--------|");

                foreach (var result in benchmarkResults)
                {
                    writer.WriteLine($"| {result.Name,-18} | {result.ElapsedTime.TotalSeconds,8:F2} | {result.IssuesFound,6} | {result.TotalExecutions,10} | {result.SuccessRate,12:P2} | {result.ExecutionRate,6:F2} |");
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

        private class BenchmarkResult
        {
            public string Name { get; set; }
            public int Depth { get; set; }
            public int PathCount { get; set; }
            public int Iterations { get; set; }
            public TimeSpan ElapsedTime { get; set; }
            public int IssuesFound { get; set; }
            public int TotalExecutions { get; set; }
            public double SuccessRate { get; set; }
            public double ExecutionRate { get; set; }
        }
    }
}
