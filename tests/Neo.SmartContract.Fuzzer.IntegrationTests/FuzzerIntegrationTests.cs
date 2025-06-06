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
    public class FuzzerIntegrationTests
    {
        private string _testContractsPath;
        private string _testResultsPath;
        private string _tempPath;

        [TestInitialize]
        public void Setup()
        {
            _testContractsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestContracts");
            _testResultsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestResults");
            _tempPath = Path.Combine(Path.GetTempPath(), "NeoFuzzerIntegrationTests", Guid.NewGuid().ToString());

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
        [Timeout(300000)] // 5 minutes timeout
        public async Task IntegerOverflowContract_DetectsVulnerabilities()
        {
            // Arrange
            string contractName = "IntegerOverflowContract";
            string nefPath = await CompileContract(contractName);
            string manifestPath = Path.ChangeExtension(nefPath, ".manifest.json");

            var config = new FuzzerConfiguration
            {
                NefPath = nefPath,
                ManifestPath = manifestPath,
                OutputDirectory = Path.Combine(_testResultsPath, contractName),
                IterationsPerMethod = 100,
                Seed = 42,
                EnableStaticAnalysis = true,
                EnableSymbolicExecution = true,
                SymbolicExecutionDepth = 100,
                SymbolicExecutionPaths = 10,
                GasLimit = 10_000_000
            };

            // Act
            var controller = new FuzzingController(config);
            controller.Start();
            await controller.WaitForCompletion();
            var status = controller.GetStatus();

            // Assert
            status.IssuesFound.Should().BeGreaterThan(0);

            // Check for specific issues
            string reportPath = Path.Combine(config.OutputDirectory, "report.json");
            File.Exists(reportPath).Should().BeTrue();
            string reportContent = File.ReadAllText(reportPath);
            reportContent.Should().Contain("Integer Overflow");
        }

        [TestMethod]
        [Timeout(300000)] // 5 minutes timeout
        public async Task DivisionByZeroContract_DetectsVulnerabilities()
        {
            // Arrange
            string contractName = "DivisionByZeroContract";
            string nefPath = await CompileContract(contractName);
            string manifestPath = Path.ChangeExtension(nefPath, ".manifest.json");

            var config = new FuzzerConfiguration
            {
                NefPath = nefPath,
                ManifestPath = manifestPath,
                OutputDirectory = Path.Combine(_testResultsPath, contractName),
                IterationsPerMethod = 100,
                Seed = 42,
                EnableStaticAnalysis = true,
                EnableSymbolicExecution = true,
                SymbolicExecutionDepth = 100,
                SymbolicExecutionPaths = 10,
                GasLimit = 10_000_000
            };

            // Act
            var controller = new FuzzingController(config);
            controller.Start();
            await controller.WaitForCompletion();
            var status = controller.GetStatus();

            // Assert
            status.IssuesFound.Should().BeGreaterThan(0);

            // Check for specific issues
            string reportPath = Path.Combine(config.OutputDirectory, "report.json");
            File.Exists(reportPath).Should().BeTrue();
            string reportContent = File.ReadAllText(reportPath);
            reportContent.Should().Contain("Division by Zero");
        }

        [TestMethod]
        [Timeout(300000)] // 5 minutes timeout
        public async Task UnauthorizedAccessContract_DetectsVulnerabilities()
        {
            // Arrange
            string contractName = "UnauthorizedAccessContract";
            string nefPath = await CompileContract(contractName);
            string manifestPath = Path.ChangeExtension(nefPath, ".manifest.json");

            var config = new FuzzerConfiguration
            {
                NefPath = nefPath,
                ManifestPath = manifestPath,
                OutputDirectory = Path.Combine(_testResultsPath, contractName),
                IterationsPerMethod = 100,
                Seed = 42,
                EnableStaticAnalysis = true,
                EnableSymbolicExecution = true,
                SymbolicExecutionDepth = 100,
                SymbolicExecutionPaths = 10,
                GasLimit = 10_000_000
            };

            // Act
            var controller = new FuzzingController(config);
            controller.Start();
            await controller.WaitForCompletion();
            var status = controller.GetStatus();

            // Assert
            status.IssuesFound.Should().BeGreaterThan(0);

            // Check for specific issues
            string reportPath = Path.Combine(config.OutputDirectory, "report.json");
            File.Exists(reportPath).Should().BeTrue();
            string reportContent = File.ReadAllText(reportPath);
            reportContent.Should().Contain("Missing Witness Check");
        }

        [TestMethod]
        [Timeout(300000)] // 5 minutes timeout
        public async Task StorageExhaustionContract_DetectsVulnerabilities()
        {
            // Arrange
            string contractName = "StorageExhaustionContract";
            string nefPath = await CompileContract(contractName);
            string manifestPath = Path.ChangeExtension(nefPath, ".manifest.json");

            var config = new FuzzerConfiguration
            {
                NefPath = nefPath,
                ManifestPath = manifestPath,
                OutputDirectory = Path.Combine(_testResultsPath, contractName),
                IterationsPerMethod = 100,
                Seed = 42,
                EnableStaticAnalysis = true,
                EnableSymbolicExecution = true,
                SymbolicExecutionDepth = 100,
                SymbolicExecutionPaths = 10,
                GasLimit = 10_000_000
            };

            // Act
            var controller = new FuzzingController(config);
            controller.Start();
            await controller.WaitForCompletion();
            var status = controller.GetStatus();

            // Assert
            status.IssuesFound.Should().BeGreaterThan(0);

            // Check for specific issues
            string reportPath = Path.Combine(config.OutputDirectory, "report.json");
            File.Exists(reportPath).Should().BeTrue();
            string reportContent = File.ReadAllText(reportPath);
            reportContent.Should().Contain("Storage");
        }

        [TestMethod]
        [Timeout(300000)] // 5 minutes timeout
        public async Task AllContracts_PerformanceMeasurement()
        {
            // Arrange
            var contractNames = new[]
            {
                "IntegerOverflowContract",
                "DivisionByZeroContract",
                "UnauthorizedAccessContract",
                "StorageExhaustionContract"
            };

            var results = new Dictionary<string, FuzzingStatus>();

            // Act
            foreach (var contractName in contractNames)
            {
                string nefPath = await CompileContract(contractName);
                string manifestPath = Path.ChangeExtension(nefPath, ".manifest.json");

                var config = new FuzzerConfiguration
                {
                    NefPath = nefPath,
                    ManifestPath = manifestPath,
                    OutputDirectory = Path.Combine(_testResultsPath, $"{contractName}_Performance"),
                    IterationsPerMethod = 50,
                    Seed = 42,
                    EnableStaticAnalysis = true,
                    EnableSymbolicExecution = true,
                    SymbolicExecutionDepth = 50,
                    SymbolicExecutionPaths = 5,
                    GasLimit = 10_000_000
                };

                var controller = new FuzzingController(config);
                controller.Start();
                await controller.WaitForCompletion();
                results[contractName] = controller.GetStatus();
            }

            // Assert
            foreach (var contractName in contractNames)
            {
                results[contractName].Should().NotBeNull();
                results[contractName].IssuesFound.Should().BeGreaterThan(0);
                results[contractName].ExecutionRate.Should().BeGreaterThan(0);
            }

            // Generate performance report
            string performanceReportPath = Path.Combine(_testResultsPath, "performance_report.txt");
            using (var writer = new StreamWriter(performanceReportPath))
            {
                writer.WriteLine("Neo Smart Contract Fuzzer Performance Report");
                writer.WriteLine("===========================================");
                writer.WriteLine();

                foreach (var contractName in contractNames)
                {
                    var status = results[contractName];
                    writer.WriteLine($"Contract: {contractName}");
                    writer.WriteLine($"  Elapsed Time: {status.ElapsedTime}");
                    writer.WriteLine($"  Total Executions: {status.TotalExecutions}");
                    writer.WriteLine($"  Execution Rate: {status.ExecutionRate:F2} executions/second");
                    writer.WriteLine($"  Success Rate: {status.SuccessRate:P2}");
                    writer.WriteLine($"  Issues Found: {status.IssuesFound}");
                    writer.WriteLine($"  Code Coverage: {status.CodeCoverage:P2}");
                    writer.WriteLine();
                }
            }
        }

        private async Task<string> CompileContract(string contractName)
        {
            // Get the source code for the contract
            string sourceCode = GetContractSourceCode(contractName);
            string sourcePath = Path.Combine(_tempPath, $"{contractName}.cs");
            File.WriteAllText(sourcePath, sourceCode);

            // Output paths for compiled files
            string nefPath = Path.Combine(_tempPath, $"{contractName}.nef");
            string manifestPath = Path.ChangeExtension(nefPath, ".manifest.json");

            try
            {
                // Create a temporary project file for compilation
                string projectPath = Path.Combine(_tempPath, $"{contractName}.csproj");
                string projectContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
                    <PropertyGroup>
                        <TargetFramework>net6.0</TargetFramework>
                    </PropertyGroup>
                    <ItemGroup>
                        <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.5.0"" />
                    </ItemGroup>
                </Project>";
                File.WriteAllText(projectPath, projectContent);

                // Compile the contract using the Neo compiler
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"nccs {sourcePath} -o {_tempPath}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    // If compilation fails, fall back to creating a simplified NEF file
                    Console.WriteLine($"Compilation failed: {error}");
                    Console.WriteLine("Falling back to simplified NEF file creation");
                    return CreateSimplifiedNefFile(contractName);
                }

                // Check if the NEF file was created
                if (!File.Exists(nefPath))
                {
                    Console.WriteLine("NEF file not found after compilation");
                    Console.WriteLine("Falling back to simplified NEF file creation");
                    return CreateSimplifiedNefFile(contractName);
                }

                return nefPath;
            }
            catch (Exception ex)
            {
                // If any error occurs, fall back to creating a simplified NEF file
                Console.WriteLine($"Error compiling contract: {ex.Message}");
                Console.WriteLine("Falling back to simplified NEF file creation");
                return CreateSimplifiedNefFile(contractName);
            }
        }

        /// <summary>
        /// Gets the source code for a test contract
        /// </summary>
        private string GetContractSourceCode(string contractName)
        {
            // Look for the contract source file in the TestContracts directory
            string contractPath = Path.Combine(_testContractsPath, $"{contractName}.cs");

            if (File.Exists(contractPath))
            {
                return File.ReadAllText(contractPath);
            }

            // If the file doesn't exist, generate a simple contract based on the name
            return $@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.IntegrationTests.TestContracts
{{
    [DisplayName(""{contractName}"")]
    [ManifestExtra(""Author"", ""Neo"")]
    [ManifestExtra(""Email"", ""dev@neo.org"")]
    [ManifestExtra(""Description"", ""Test contract for fuzzing"")]
    public class {contractName} : Framework.SmartContract
    {{
        // Simple methods for testing
        public static BigInteger Add(BigInteger a, BigInteger b)
        {{
            return a + b;
        }}

        public static BigInteger Multiply(BigInteger a, BigInteger b)
        {{
            return a * b;
        }}

        public static BigInteger Subtract(BigInteger a, BigInteger b)
        {{
            return a - b;
        }}

        public static BigInteger Divide(BigInteger a, BigInteger b)
        {{
            if (b == 0)
                throw new Exception(""Division by zero"");
            return a / b;
        }}
    }}
}}";
        }

        /// <summary>
        /// Creates a simplified NEF file for testing when compilation fails
        /// </summary>
        private string CreateSimplifiedNefFile(string contractName)
        {
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

            // Create a manifest file
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
    }
}
