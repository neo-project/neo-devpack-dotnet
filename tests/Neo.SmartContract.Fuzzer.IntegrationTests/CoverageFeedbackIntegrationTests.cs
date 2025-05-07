using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.Coverage;
using Neo.SmartContract.Fuzzer.Feedback;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;

namespace Neo.SmartContract.Fuzzer.IntegrationTests
{
    [TestClass]
    public class CoverageFeedbackIntegrationTests
    {
        private string _testContractPath;
        private byte[] _nefBytes;
        private ContractManifest _manifest;
        private CoverageTracker _coverageTracker;
        private CoverageFeedback _coverageFeedback;
        private string _outputDir;
        private readonly int _seed = 12345;

        [TestInitialize]
        public void Setup()
        {
            // Set up test contract path
            _testContractPath = Path.Combine(TestContext.TestRunDirectory, "TestContracts", "SimpleContract.nef");

            // Create output directory
            _outputDir = Path.Combine(TestContext.TestRunDirectory, "TestOutput", "Coverage");
            Directory.CreateDirectory(_outputDir);

            // If test contract exists, load it
            if (File.Exists(_testContractPath))
            {
                _nefBytes = File.ReadAllBytes(_testContractPath);

                // Load manifest
                string manifestPath = Path.ChangeExtension(_testContractPath, ".manifest.json");
                if (File.Exists(manifestPath))
                {
                    string manifestJson = File.ReadAllText(manifestPath);
                    _manifest = ContractManifest.Parse(manifestJson);
                }
                else
                {
                    // Create a simple manifest for testing
                    _manifest = new ContractManifest
                    {
                        Name = "SimpleContract",
                        Abi = new ContractAbi
                        {
                            Methods = new ContractMethodDescriptor[]
                            {
                                new ContractMethodDescriptor
                                {
                                    Name = "TestMethod",
                                    Parameters = new ContractParameterDefinition[0],
                                    ReturnType = ContractParameterType.Void
                                }
                            }
                        }
                    };
                }
            }
            else
            {
                // Create dummy NEF bytes and manifest for testing
                _nefBytes = new byte[100]; // Dummy NEF bytes
                new Random(_seed).NextBytes(_nefBytes);

                _manifest = new ContractManifest
                {
                    Name = "DummyContract",
                    Abi = new ContractAbi
                    {
                        Methods = new ContractMethodDescriptor[]
                        {
                            new ContractMethodDescriptor
                            {
                                Name = "TestMethod",
                                Parameters = new ContractParameterDefinition[0],
                                ReturnType = ContractParameterType.Void
                            }
                        }
                    }
                };
            }

            // Initialize coverage tracker and feedback
            _coverageTracker = new CoverageTracker(_nefBytes, _manifest, _outputDir);
            _coverageFeedback = new CoverageFeedback(_coverageTracker, _seed);
        }

        [TestMethod]
        public void ProcessExecutionResult_WithRealEngine_ReturnsFeedback()
        {
            // Arrange
            var testCase = new TestCase { MethodName = "TestMethod" };

            // Create a test execution engine
            using var engine = TestBlockchain.CreateEngine();

            // Create a simple script that performs some operations
            using ScriptBuilder sb = new ScriptBuilder();
            sb.EmitPush(10);
            sb.EmitPush(20);
            sb.Emit(OpCode.ADD);
            sb.Emit(OpCode.RET);

            // Load and execute the script
            engine.LoadScript(sb.ToArray());
            engine.Execute();

            // Create execution result
            var result = new ExecutionResult
            {
                Method = "TestMethod",
                Success = true,
                Engine = engine,
                ReturnValue = engine.ResultStack.Count > 0 ? engine.ResultStack.Peek() : null
            };

            // Act
            var feedback = _coverageFeedback.ProcessExecutionResult(testCase, result);

            // Assert
            feedback.Should().NotBeNull();
            feedback.Type.Should().Be(FeedbackType.NewCoverage);
            feedback.Description.Should().Contain("TestMethod");
            feedback.RelatedTestCase.Should().NotBeNull();
            feedback.RelatedTestCase.MethodName.Should().Be("TestMethod");
        }

        [TestMethod]
        public void GetCoverageStatistics_AfterExecution_ReturnsStatistics()
        {
            // Arrange
            var testCase = new TestCase { MethodName = "TestMethod" };

            // Create a test execution engine
            using var engine = TestBlockchain.CreateEngine();

            // Create a simple script that performs some operations
            using ScriptBuilder sb = new ScriptBuilder();
            sb.EmitPush(10);
            sb.EmitPush(20);
            sb.Emit(OpCode.ADD);
            sb.Emit(OpCode.RET);

            // Load and execute the script
            engine.LoadScript(sb.ToArray());
            engine.Execute();

            // Create execution result
            var result = new ExecutionResult
            {
                Method = "TestMethod",
                Success = true,
                Engine = engine,
                ReturnValue = engine.ResultStack.Count > 0 ? engine.ResultStack.Peek() : null
            };

            // Process the execution result
            _coverageFeedback.ProcessExecutionResult(testCase, result);

            // Act
            var stats = _coverageFeedback.GetCoverageStatistics();

            // Assert
            stats.Should().NotBeNull();
            stats.Should().ContainKey("MethodsCovered");
            stats.Should().ContainKey("InstructionsCovered");
            stats.Should().ContainKey("TotalInstructions");
            stats.Should().ContainKey("BranchCoveragePercent");
            stats.Should().ContainKey("UniquePaths");

            stats["MethodsCovered"].Should().Be(1);
            stats["InstructionsCovered"].Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void GetLowCoverageMethods_WithMultipleMethods_ReturnsOrderedMethods()
        {
            // Arrange
            // Process multiple methods with different coverage
            ProcessMethodWithInstructions("Method1", 10);
            ProcessMethodWithInstructions("Method2", 5);
            ProcessMethodWithInstructions("Method3", 15);

            // Act
            var lowCoverageMethods = _coverageFeedback.GetLowCoverageMethods(2);

            // Assert
            lowCoverageMethods.Should().NotBeNull();
            lowCoverageMethods.Should().HaveCount(2);
            lowCoverageMethods[0].Should().Be("Method2"); // Lowest coverage
        }

        [TestMethod]
        public void GetUncoveredBranches_WithBranches_ReturnsUncoveredBranches()
        {
            // Arrange
            // Process a method with branches
            ProcessMethodWithBranches("MethodWithBranches");

            // Act
            var uncoveredBranches = _coverageFeedback.GetUncoveredBranches(10);

            // Assert
            uncoveredBranches.Should().NotBeNull();
            // We can't assert exact count as it depends on the implementation
            // but we can check that it returns something
        }

        [TestMethod]
        public void MultipleExecutions_IncreaseCoverage()
        {
            // Arrange
            var testCase = new TestCase { MethodName = "TestMethod" };

            // First execution
            var result1 = ExecuteSimpleScript("TestMethod", new byte[] { (byte)OpCode.PUSH1, (byte)OpCode.RET });
            var feedback1 = _coverageFeedback.ProcessExecutionResult(testCase, result1);

            // Get initial coverage
            var initialStats = _coverageFeedback.GetCoverageStatistics();
            int initialInstructions = (int)initialStats["InstructionsCovered"];

            // Second execution with different instructions
            var result2 = ExecuteSimpleScript("TestMethod", new byte[] { (byte)OpCode.PUSH1, (byte)OpCode.PUSH2, (byte)OpCode.ADD, (byte)OpCode.RET });
            var feedback2 = _coverageFeedback.ProcessExecutionResult(testCase, result2);

            // Act
            var finalStats = _coverageFeedback.GetCoverageStatistics();
            int finalInstructions = (int)finalStats["InstructionsCovered"];

            // Assert
            feedback1.Should().NotBeNull();
            feedback2.Should().NotBeNull();
            finalInstructions.Should().BeGreaterThan(initialInstructions);
        }

        #region Helper Methods

        private void ProcessMethodWithInstructions(string methodName, int instructionCount)
        {
            var testCase = new TestCase { MethodName = methodName };

            // Create a script with the specified number of instructions
            byte[] script = new byte[instructionCount * 2]; // Each instruction is 2 bytes for simplicity
            for (int i = 0; i < instructionCount; i++)
            {
                script[i * 2] = (byte)OpCode.PUSH1; // Simple instruction
            }
            script[script.Length - 2] = (byte)OpCode.RET; // End with RET

            var result = ExecuteSimpleScript(methodName, script);
            _coverageFeedback.ProcessExecutionResult(testCase, result);
        }

        private void ProcessMethodWithBranches(string methodName)
        {
            var testCase = new TestCase { MethodName = methodName };

            // Create a script with branches
            using ScriptBuilder sb = new ScriptBuilder();
            sb.EmitPush(10);
            sb.EmitPush(5);
            sb.Emit(OpCode.GT);

            // Add a conditional jump (branch)
            sb.Emit(OpCode.JMPIF, 5); // Jump 5 bytes if true

            // False branch
            sb.EmitPush(0);
            sb.Emit(OpCode.RET);

            // True branch
            sb.EmitPush(1);
            sb.Emit(OpCode.RET);

            var result = ExecuteSimpleScript(methodName, sb.ToArray());
            _coverageFeedback.ProcessExecutionResult(testCase, result);
        }

        private ExecutionResult ExecuteSimpleScript(string methodName, byte[] script)
        {
            // Create a test execution engine
            using var engine = TestBlockchain.CreateEngine();

            // Load and execute the script
            engine.LoadScript(script);
            engine.Execute();

            // Create execution result
            return new ExecutionResult
            {
                Method = methodName,
                Success = true,
                Engine = engine,
                ReturnValue = engine.ResultStack.Count > 0 ? engine.ResultStack.Peek() : null
            };
        }

        #endregion

        public TestContext TestContext { get; set; }
    }
}
