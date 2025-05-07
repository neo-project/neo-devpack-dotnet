using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Neo.SmartContract.Fuzzer.Tests
{
    /// <summary>
    /// Integration tests for the Neo Smart Contract Fuzzer
    /// </summary>
    public class IntegrationTests
    {
        /// <summary>
        /// Tests the complete fuzzing process with symbolic execution
        /// </summary>
        [Fact]
        public void TestFuzzingWithSymbolicExecution()
        {
            // Create a temporary directory for test outputs
            string tempDir = Path.Combine(Path.GetTempPath(), "NeoFuzzerTest_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            
            try
            {
                // Create a configuration with symbolic execution enabled
                var config = new FuzzerConfiguration
                {
                    OutputDirectory = tempDir,
                    IterationsPerMethod = 5,
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
                    MaxPathsToVisualize = 10,
                    ReportFormats = new[] { FuzzerConfiguration.ReportFormat.Markdown, FuzzerConfiguration.ReportFormat.Json }
                };
                
                // Create a sample NEF file and manifest for testing
                byte[] nefBytes = CreateSampleNef();
                string nefPath = Path.Combine(tempDir, "Sample.nef");
                File.WriteAllBytes(nefPath, nefBytes);
                
                ContractManifest manifest = CreateSampleManifest();
                string manifestPath = Path.Combine(tempDir, "Sample.manifest.json");
                File.WriteAllText(manifestPath, manifest.ToJson().ToString());
                
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
                
                // Verify that report files were generated
                string mdReportPath = Path.Combine(tempDir, "Sample_fuzzing_report.md");
                string jsonReportPath = Path.Combine(tempDir, "Sample_fuzzing_report.json");
                string pathVisualizationPath = Path.Combine(tempDir, "path_visualization.html");
                
                Assert.True(File.Exists(mdReportPath), "Markdown report should be generated");
                Assert.True(File.Exists(jsonReportPath), "JSON report should be generated");
                Assert.True(File.Exists(pathVisualizationPath), "Path visualization should be generated");
                
                // Verify that the reports contain symbolic execution results
                string mdReport = File.ReadAllText(mdReportPath);
                Assert.Contains("Symbolic Execution Results", mdReport);
                Assert.Contains("Path Constraints", mdReport);
                Assert.Contains("Vulnerabilities", mdReport);
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
        /// Tests vulnerability detection during symbolic execution
        /// </summary>
        [Fact]
        public void TestVulnerabilityDetectionDuringSymbolicExecution()
        {
            // Create a script with a vulnerability (integer overflow)
            byte[] script = new ScriptBuilder()
                .EmitPush(100) // Push a value onto the stack
                .Emit(OpCode.DUP) // Duplicate it
                .EmitPush(int.MaxValue) // Push max integer value
                .Emit(OpCode.ADD) // Add them (will overflow)
                .Emit(OpCode.RET) // Return
                .ToArray();
            
            // Create a manifest for testing
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
                            Name = "vulnerableMethod",
                            Parameters = new ContractParameterDefinition[0],
                            ReturnType = "Integer",
                            Offset = 0,
                            Safe = true
                        },
                        new ContractMethodDescriptor
                        {
                            Name = "conditionalMethod",
                            Parameters = new[]
                            {
                                new ContractParameterDefinition { Name = "a", Type = "Integer" },
                                new ContractParameterDefinition { Name = "b", Type = "Integer" }
                            },
                            ReturnType = "Boolean",
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
            
            // Create a configuration with vulnerability detection enabled
            var config = new FuzzerConfiguration
            {
                EnableSymbolicExecution = true,
                SymbolicExecutionDepth = 50,
                SymbolicExecutionMaxPaths = 10,
                UseConstraintSolver = true,
                GenerateConcreteInputs = true,
                DetectVulnerabilitiesDuringSymbolicExecution = true,
                Seed = 12345
            };
            
            // Create a symbolic virtual machine
            var vm = new SymbolicVirtualMachine(script, manifest, config);
            
            // Execute the method symbolically
            var method = manifest.Abi.Methods[0];
            var results = vm.ExecuteMethod(method);
            
            // Verify that vulnerabilities were detected
            int vulnerabilityCount = results.Sum(r => r.Vulnerabilities?.Count ?? 0);
            Assert.True(vulnerabilityCount > 0, "Vulnerabilities should be detected");
            
            // Verify that at least one integer overflow vulnerability was detected
            bool hasOverflowVulnerability = results.Any(r => 
                r.Vulnerabilities != null && 
                r.Vulnerabilities.Any(v => v.Type == "Integer Overflow"));
            
            Assert.True(hasOverflowVulnerability, "Integer overflow vulnerability should be detected");
        }
        
        /// <summary>
        /// Creates a sample NEF file for testing
        /// </summary>
        private byte[] CreateSampleNef()
        {
            // Create a simple script that adds two integers
            byte[] script = new ScriptBuilder()
                .EmitPush(1)
                .EmitPush(2)
                .Emit(OpCode.ADD)
                .Emit(OpCode.RET)
                .ToArray();
            
            // Create a NEF file
            NefFile nef = new NefFile
            {
                Compiler = "test",
                Version = new Version(1, 0, 0, 0),
                Script = script,
                Tokens = new MethodToken[0]
            };
            
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                nef.Serialize(writer);
                return ms.ToArray();
            }
        }
        
        /// <summary>
        /// Creates a sample manifest for testing
        /// </summary>
        private ContractManifest CreateSampleManifest()
        {
            return new ContractManifest
            {
                Name = "SampleContract",
                Groups = new ContractGroup[0],
                SupportedStandards = new string[0],
                Abi = new ContractAbi
                {
                    Methods = new[]
                    {
                        new ContractMethodDescriptor
                        {
                            Name = "add",
                            Parameters = new[]
                            {
                                new ContractParameterDefinition { Name = "a", Type = "Integer" },
                                new ContractParameterDefinition { Name = "b", Type = "Integer" }
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
        }
    }
}