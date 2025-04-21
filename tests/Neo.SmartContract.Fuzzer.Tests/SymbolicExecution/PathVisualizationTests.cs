using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Visualization;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Neo.SmartContract.Fuzzer.Tests.SymbolicExecution
{
    /// <summary>
    /// Tests for the path visualization component
    /// </summary>
    public class PathVisualizationTests
    {
        /// <summary>
        /// Tests the path visualization component with simple execution paths
        /// </summary>
        [Fact]
        public void TestBasicPathVisualization()
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
                    EnablePathVisualization = true,
                    PathVisualizationFormat = "html",
                    MaxPathsToVisualize = 10,
                    MaxVisualizationDepth = 20
                };
                
                // Create a list of symbolic execution results for visualization
                var symbolicResults = new List<SymbolicExecutionResult>();
                
                // Create a simple symbolic execution result
                var result = new SymbolicExecutionResult
                {
                    Method = "TestMethod",
                    IsSuccess = true,
                    ReturnValue = SymbolicValue.CreateBoolean(true),
                    ExecutionPath = new List<OpCode> { OpCode.PUSH1, OpCode.PUSH2, OpCode.ADD, OpCode.RET },
                    PathConstraints = new List<SymbolicConstraint>(),
                    GasConsumed = 100,
                    ExecutionTime = TimeSpan.FromMilliseconds(50)
                };
                
                symbolicResults.Add(result);
                
                // Create a path visualizer
                var pathVisualizer = new PathVisualizer(config);
                
                // Generate visualization
                string visualizationPath = pathVisualizer.GenerateVisualization(symbolicResults);
                
                // Verify that the visualization file was created
                Assert.True(File.Exists(visualizationPath), "Path visualization should be generated");
                
                // Verify that the visualization file contains expected content
                string visualizationContent = File.ReadAllText(visualizationPath);
                Assert.Contains("Neo Smart Contract Fuzzer - Symbolic Execution Path Visualization", visualizationContent);
                Assert.Contains("TestMethod", visualizationContent);
                Assert.Contains("PUSH1", visualizationContent);
                Assert.Contains("PUSH2", visualizationContent);
                Assert.Contains("ADD", visualizationContent);
                Assert.Contains("RET", visualizationContent);
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
        /// Tests the path visualization component with branching execution paths
        /// </summary>
        [Fact]
        public void TestBranchingPathVisualization()
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
                    EnablePathVisualization = true,
                    PathVisualizationFormat = "html",
                    MaxPathsToVisualize = 10,
                    MaxVisualizationDepth = 20
                };
                
                // Create a list of symbolic execution results for visualization
                var symbolicResults = new List<SymbolicExecutionResult>();
                
                // Create symbolic values
                var x = new SymbolicValue("x", SymbolicValue.SymbolicType.Integer, null, "x");
                var ten = new SymbolicValue("ten", SymbolicValue.SymbolicType.Integer, new VM.Types.Integer(10), "ten");
                
                // Create a symbolic execution result for the true branch (x > 10)
                var trueResult = new SymbolicExecutionResult
                {
                    Method = "BranchingMethod",
                    IsSuccess = true,
                    ReturnValue = SymbolicValue.CreateBoolean(true),
                    ExecutionPath = new List<OpCode> { OpCode.PUSH10, OpCode.DUPFROMALTSTACK, OpCode.GT, OpCode.JMPIF, OpCode.PUSH1, OpCode.RET },
                    PathConstraints = new List<SymbolicConstraint>
                    {
                        new SymbolicConstraint(x, ten, SymbolicConstraint.ConstraintType.GreaterThan)
                    },
                    GasConsumed = 100,
                    ExecutionTime = TimeSpan.FromMilliseconds(50)
                };
                
                symbolicResults.Add(trueResult);
                
                // Create a symbolic execution result for the false branch (x <= 10)
                var falseResult = new SymbolicExecutionResult
                {
                    Method = "BranchingMethod",
                    IsSuccess = true,
                    ReturnValue = SymbolicValue.CreateBoolean(false),
                    ExecutionPath = new List<OpCode> { OpCode.PUSH10, OpCode.DUPFROMALTSTACK, OpCode.GT, OpCode.JMPIFNOT, OpCode.PUSH0, OpCode.RET },
                    PathConstraints = new List<SymbolicConstraint>
                    {
                        new SymbolicConstraint(x, ten, SymbolicConstraint.ConstraintType.LessThanOrEqual)
                    },
                    GasConsumed = 100,
                    ExecutionTime = TimeSpan.FromMilliseconds(50)
                };
                
                symbolicResults.Add(falseResult);
                
                // Create a path visualizer
                var pathVisualizer = new PathVisualizer(config);
                
                // Generate visualization
                string visualizationPath = pathVisualizer.GenerateVisualization(symbolicResults);
                
                // Verify that the visualization file was created
                Assert.True(File.Exists(visualizationPath), "Path visualization should be generated");
                
                // Verify that the visualization file contains expected content
                string visualizationContent = File.ReadAllText(visualizationPath);
                Assert.Contains("Neo Smart Contract Fuzzer - Symbolic Execution Path Visualization", visualizationContent);
                Assert.Contains("BranchingMethod", visualizationContent);
                Assert.Contains("PUSH10", visualizationContent);
                Assert.Contains("GT", visualizationContent);
                Assert.Contains("JMPIF", visualizationContent);
                Assert.Contains("JMPIFNOT", visualizationContent);
                Assert.Contains("x > 10", visualizationContent);
                Assert.Contains("x <= 10", visualizationContent);
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
        /// Tests the path visualization component with vulnerability detection
        /// </summary>
        [Fact]
        public void TestVulnerabilityVisualization()
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
                    EnablePathVisualization = true,
                    PathVisualizationFormat = "html",
                    MaxPathsToVisualize = 10,
                    MaxVisualizationDepth = 20,
                    EnableVulnerabilityDetection = true
                };
                
                // Create a list of symbolic execution results for visualization
                var symbolicResults = new List<SymbolicExecutionResult>();
                
                // Create a symbolic execution result with a vulnerability
                var vulnerableResult = new SymbolicExecutionResult
                {
                    Method = "VulnerableMethod",
                    IsSuccess = true,
                    ReturnValue = SymbolicValue.CreateInteger(0),
                    ExecutionPath = new List<OpCode> { OpCode.PUSH0, OpCode.PUSH1, OpCode.DIV, OpCode.RET },
                    PathConstraints = new List<SymbolicConstraint>(),
                    GasConsumed = 100,
                    ExecutionTime = TimeSpan.FromMilliseconds(50),
                    Vulnerabilities = new List<VulnerabilityInfo>
                    {
                        new VulnerabilityInfo
                        {
                            Type = "Division by Zero",
                            Description = "Potential division by zero vulnerability",
                            Severity = "High",
                            StepIndex = 2,
                            Confidence = "High"
                        }
                    }
                };
                
                symbolicResults.Add(vulnerableResult);
                
                // Create a path visualizer
                var pathVisualizer = new PathVisualizer(config);
                
                // Generate visualization
                string visualizationPath = pathVisualizer.GenerateVisualization(symbolicResults);
                
                // Verify that the visualization file was created
                Assert.True(File.Exists(visualizationPath), "Path visualization should be generated");
                
                // Verify that the visualization file contains expected content
                string visualizationContent = File.ReadAllText(visualizationPath);
                Assert.Contains("Neo Smart Contract Fuzzer - Symbolic Execution Path Visualization", visualizationContent);
                Assert.Contains("VulnerableMethod", visualizationContent);
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
    }
}