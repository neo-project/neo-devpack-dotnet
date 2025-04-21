using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Visualization;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Neo.SmartContract.Fuzzer.Tests.SymbolicExecution
{
    /// <summary>
    /// Integration tests for the path visualization component with the main fuzzer
    /// </summary>
    public class PathVisualizationIntegrationTests
    {
        /// <summary>
        /// Tests the path visualization with a vulnerable contract
        /// </summary>
        [Fact]
        public void TestPathVisualizationWithVulnerableContract()
        {
            // Create a temporary directory for test outputs
            string tempDir = Path.Combine(Path.GetTempPath(), "NeoFuzzerTest_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            
            try
            {
                // Create a configuration with path visualization enabled
                var config = new FuzzerConfiguration
                {
                    OutputDirectory = tempDir,
                    IterationsPerMethod = 3,
                    Seed = 12345,
                    EnableSymbolicExecution = true,
                    SymbolicExecutionDepth = 50,
                    SymbolicExecutionMaxPaths = 10,
                    UseConstraintSolver = true,
                    GenerateConcreteInputs = true,
                    DetectVulnerabilitiesDuringSymbolicExecution = true,
                    UseSymbolicResultsForFuzzing = true,
                    EnableEnhancedReporting = true,
                    EnableVulnerabilityDetection = true,
                    EnablePathVisualization = true,
                    PathVisualizationFormat = "html",
                    MaxPathsToVisualize = 10
                };
                
                // Create a vulnerable contract
                byte[] nefBytes = CreateVulnerableNefFile();
                string nefPath = Path.Combine(tempDir, "Vulnerable.nef");
                File.WriteAllBytes(nefPath, nefBytes);
                
                string manifestJson = CreateVulnerableManifest();
                string manifestPath = Path.Combine(tempDir, "Vulnerable.manifest.json");
                File.WriteAllText(manifestPath, manifestJson);
                
                // Create and run the fuzzer
                var fuzzer = new SmartContractFuzzer
                {
                    Configuration = config,
                    NefPath = nefPath,
                    ManifestPath = manifestPath
                };
                
                bool result = fuzzer.FuzzContract();
                
                // Verify that the fuzzing process completed successfully
                Assert.True(result, "Fuzzing should complete successfully");
                
                // Verify that the path visualization file was generated
                string pathVisualizationPath = Path.Combine(tempDir, "path_visualization.html");
                Assert.True(File.Exists(pathVisualizationPath), "Path visualization should be generated");
                
                // Verify that the visualization file contains expected content
                string visualizationContent = File.ReadAllText(pathVisualizationPath);
                Assert.Contains("Neo Smart Contract Fuzzer - Symbolic Execution Path Visualization", visualizationContent);
                Assert.Contains("divideByZero", visualizationContent);
                Assert.Contains("vulnerability", visualizationContent);
                Assert.Contains("Division by Zero", visualizationContent);
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
        
        /// <summary>
        /// Tests the path visualization with a contract that has multiple execution paths
        /// </summary>
        [Fact]
        public void TestPathVisualizationWithMultiPathContract()
        {
            // Create a temporary directory for test outputs
            string tempDir = Path.Combine(Path.GetTempPath(), "NeoFuzzerTest_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            
            try
            {
                // Create a configuration with path visualization enabled
                var config = new FuzzerConfiguration
                {
                    OutputDirectory = tempDir,
                    IterationsPerMethod = 3,
                    Seed = 12345,
                    EnableSymbolicExecution = true,
                    SymbolicExecutionDepth = 50,
                    SymbolicExecutionMaxPaths = 10,
                    UseConstraintSolver = true,
                    GenerateConcreteInputs = true,
                    DetectVulnerabilitiesDuringSymbolicExecution = true,
                    UseSymbolicResultsForFuzzing = true,
                    EnableEnhancedReporting = true,
                    EnableVulnerabilityDetection = true,
                    EnablePathVisualization = true,
                    PathVisualizationFormat = "html",
                    MaxPathsToVisualize = 10
                };
                
                // Create a multi-path contract
                byte[] nefBytes = CreateMultiPathNefFile();
                string nefPath = Path.Combine(tempDir, "MultiPath.nef");
                File.WriteAllBytes(nefPath, nefBytes);
                
                string manifestJson = CreateMultiPathManifest();
                string manifestPath = Path.Combine(tempDir, "MultiPath.manifest.json");
                File.WriteAllText(manifestPath, manifestJson);
                
                // Create and run the fuzzer
                var fuzzer = new SmartContractFuzzer
                {
                    Configuration = config,
                    NefPath = nefPath,
                    ManifestPath = manifestPath
                };
                
                bool result = fuzzer.FuzzContract();
                
                // Verify that the fuzzing process completed successfully
                Assert.True(result, "Fuzzing should complete successfully");
                
                // Verify that the path visualization file was generated
                string pathVisualizationPath = Path.Combine(tempDir, "path_visualization.html");
                Assert.True(File.Exists(pathVisualizationPath), "Path visualization should be generated");
                
                // Verify that the visualization file contains expected content
                string visualizationContent = File.ReadAllText(pathVisualizationPath);
                Assert.Contains("Neo Smart Contract Fuzzer - Symbolic Execution Path Visualization", visualizationContent);
                Assert.Contains("categorize", visualizationContent);
                Assert.Contains("path", visualizationContent);
                Assert.Contains("branch", visualizationContent);
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
        
        /// <summary>
        /// Creates a NEF file for a contract with a division by zero vulnerability
        /// </summary>
        /// <returns>NEF file bytes</returns>
        private byte[] CreateVulnerableNefFile()
        {
            // Create a script with a division by zero vulnerability
            byte[] script = new ScriptBuilder()
                .EmitPush(0) // Divisor (0)
                .Emit(OpCode.DUPFROMALTSTACK) // Get parameter from alt stack
                .Emit(OpCode.SWAP)
                .Emit(OpCode.DIV) // Divide by zero
                .Emit(OpCode.RET)
                .ToArray();
            
            // Create a NEF file
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                // NEF magic
                writer.Write(new byte[] { 0x4E, 0x45, 0x46, 0x33 }); // "NEF3"
                
                // Compiler name (padded to 64 bytes)
                byte[] compiler = new byte[64];
                Array.Copy(System.Text.Encoding.ASCII.GetBytes("test-compiler"), compiler, 13);
                writer.Write(compiler);
                
                // Script length and content
                writer.Write(script.Length);
                writer.Write(script);
                
                // Return the NEF bytes
                return ms.ToArray();
            }
        }
        
        /// <summary>
        /// Creates a manifest for a contract with a division by zero vulnerability
        /// </summary>
        /// <returns>Manifest JSON</returns>
        private string CreateVulnerableManifest()
        {
            // Create a manifest for the vulnerable contract
            var manifest = new ContractManifest
            {
                Name = "VulnerableContract",
                Groups = new ContractGroup[0],
                SupportedStandards = new string[0],
                Abi = new ContractAbi
                {
                    Methods = new[]
                    {
                        new ContractMethodDescriptor
                        {
                            Name = "divideByZero",
                            Parameters = new[]
                            {
                                new ContractParameterDefinition { Name = "x", Type = "Integer" }
                            },
                            ReturnType = "Integer",
                            Offset = 0,
                            Safe = true
                        }
                    },
                    Events = new ContractEventDescriptor[0]
                },
                Permissions = new[] { ContractPermission.DefaultPermission },
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
                Extra = null
            };
            
            return manifest.ToJson().ToString();
        }
        
        /// <summary>
        /// Creates a NEF file for a contract with multiple execution paths
        /// </summary>
        /// <returns>NEF file bytes</returns>
        private byte[] CreateMultiPathNefFile()
        {
            // Create a script with multiple execution paths
            // The script categorizes the input x into one of three categories:
            // 1. x < 0: return -1
            // 2. 0 <= x <= 100: return 0
            // 3. x > 100: return 1
            byte[] script = new ScriptBuilder()
                .Emit(OpCode.DUPFROMALTSTACK) // Get parameter x from alt stack
                .EmitPush(0)
                .Emit(OpCode.LT) // x < 0?
                .Emit(OpCode.JMPIFNOT, new byte[] { 10 }) // Jump to next check if not
                .EmitPush(-1) // Category 1: Negative
                .Emit(OpCode.RET)
                .Emit(OpCode.DUPFROMALTSTACK) // Get parameter x from alt stack again
                .EmitPush(100)
                .Emit(OpCode.GT) // x > 100?
                .Emit(OpCode.JMPIFNOT, new byte[] { 10 }) // Jump to next check if not
                .EmitPush(1) // Category 3: Large
                .Emit(OpCode.RET)
                .EmitPush(0) // Category 2: Medium
                .Emit(OpCode.RET)
                .ToArray();
            
            // Create a NEF file
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                // NEF magic
                writer.Write(new byte[] { 0x4E, 0x45, 0x46, 0x33 }); // "NEF3"
                
                // Compiler name (padded to 64 bytes)
                byte[] compiler = new byte[64];
                Array.Copy(System.Text.Encoding.ASCII.GetBytes("test-compiler"), compiler, 13);
                writer.Write(compiler);
                
                // Script length and content
                writer.Write(script.Length);
                writer.Write(script);
                
                // Return the NEF bytes
                return ms.ToArray();
            }
        }
        
        /// <summary>
        /// Creates a manifest for a contract with multiple execution paths
        /// </summary>
        /// <returns>Manifest JSON</returns>
        private string CreateMultiPathManifest()
        {
            // Create a manifest for the multi-path contract
            var manifest = new ContractManifest
            {
                Name = "MultiPathContract",
                Groups = new ContractGroup[0],
                SupportedStandards = new string[0],
                Abi = new ContractAbi
                {
                    Methods = new[]
                    {
                        new ContractMethodDescriptor
                        {
                            Name = "categorize",
                            Parameters = new[]
                            {
                                new ContractParameterDefinition { Name = "x", Type = "Integer" }
                            },
                            ReturnType = "Integer",
                            Offset = 0,
                            Safe = true
                        }
                    },
                    Events = new ContractEventDescriptor[0]
                },
                Permissions = new[] { ContractPermission.DefaultPermission },
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
                Extra = null
            };
            
            return manifest.ToJson().ToString();
        }
    }
}