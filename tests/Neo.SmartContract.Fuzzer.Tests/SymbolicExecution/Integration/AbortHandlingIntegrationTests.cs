using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.Tests.SymbolicExecution.Integration
{
    /// <summary>
    /// Integration tests for proper handling of the ABORT opcode in the symbolic virtual machine.
    /// These tests specifically verify that the previously identified issue with ABORT handling has been fixed.
    /// </summary>
    [TestClass]
    public class AbortHandlingIntegrationTests
    {
        private IConstraintSolver _solver;

        [TestInitialize]
        public void Setup()
        {
            _solver = new Z3ConstraintSolver();
        }

        /// <summary>
        /// Tests that the refactored implementation correctly handles the ABORT opcode.
        /// This test verifies that the NullReferenceException that previously occurred is now fixed.
        /// </summary>
        [TestMethod]
        public void AbortOpcode_SetsCorrectHaltReason()
        {
            // Arrange - Create a script with just an ABORT opcode
            byte[] script = new byte[] { (byte)OpCode.ABORT };
            
            // Act - Execute the script with the refactored VM
            var vm = new RefactoredSymbolicVirtualMachine(script, _solver);
            var result = vm.Execute();
            
            // Assert - Verify the execution halted with HaltReason.Abort
            Assert.AreEqual(1, result.ExecutionPaths.Count);
            Assert.AreEqual(HaltReason.Abort, result.ExecutionPaths[0].HaltReason);
        }

        /// <summary>
        /// Tests that the wrapper class correctly delegates ABORT handling to the appropriate implementation.
        /// </summary>
        [TestMethod]
        public void Wrapper_AbortOpcode_DelegatesCorrectly()
        {
            // Arrange - Create a script with just an ABORT opcode
            byte[] script = new byte[] { (byte)OpCode.ABORT };
            
            // Act - Execute with original implementation via wrapper
            var originalWrapper = new SymbolicVirtualMachineWrapper(script, _solver, useRefactoredImplementation: false);
            var originalResult = originalWrapper.Execute();
            
            // Act - Execute with refactored implementation via wrapper
            var refactoredWrapper = new SymbolicVirtualMachineWrapper(script, _solver, useRefactoredImplementation: true);
            var refactoredResult = refactoredWrapper.Execute();
            
            // Assert - Verify both produce the correct HaltReason.Abort
            Assert.AreEqual(1, originalResult.ExecutionPaths.Count);
            Assert.AreEqual(HaltReason.Abort, originalResult.ExecutionPaths[0].HaltReason);
            
            Assert.AreEqual(1, refactoredResult.ExecutionPaths.Count);
            Assert.AreEqual(HaltReason.Abort, refactoredResult.ExecutionPaths[0].HaltReason);
        }

        /// <summary>
        /// Tests the more complex case that originally revealed the ABORT handling issue.
        /// This test simulates the TestSimpleBranchAndAbort scenario from SymbolicExecutionEngineIntegrationTests.
        /// </summary>
        [TestMethod]
        public void ComplexScenario_WithAbort_HandlesCorrectly()
        {
            // Arrange - Create a script that includes a conditional branch and an ABORT
            // This simulates the scenario in TestSimpleBranchAndAbort
            byte[] script = new byte[]
            {
                (byte)OpCode.PUSHDATA1,   // Push symbolic variable
                0x01,                      // Length
                0x00,                      // Placeholder
                (byte)OpCode.JMPIF,        // Jump if true
                0x03,                      // Jump offset (skip over ABORT)
                (byte)OpCode.ABORT,        // Abort execution (taken if condition is false)
                (byte)OpCode.PUSH1,        // Push 1 (taken if condition is true)
                (byte)OpCode.RET           // Return
            };
            
            // Initialize symbolic state with a variable
            var initialState = new SymbolicState();
            initialState.PushSymbolicVariable("testCondition");
            
            // Act - Execute with the refactored VM
            var vm = new RefactoredSymbolicVirtualMachine(script, _solver, initialState);
            var result = vm.Execute();
            
            // Assert - Verify we explored both paths (normal return and abort)
            Assert.AreEqual(2, result.ExecutionPaths.Count);
            
            // Find the paths by halt reason
            var abortPath = result.ExecutionPaths.FirstOrDefault(p => p.HaltReason == HaltReason.Abort);
            var normalPath = result.ExecutionPaths.FirstOrDefault(p => p.HaltReason == HaltReason.Normal);
            
            // Verify both paths were found
            Assert.IsNotNull(abortPath, "Abort path was not explored");
            Assert.IsNotNull(normalPath, "Normal path was not explored");
            
            // Verify the abort path has the correct constraints
            // The condition must be false for the ABORT to be reached
            Assert.IsTrue(abortPath.FinalState.PathConstraints.Any(c => 
                c is SymbolicExpression expr && 
                expr.Op == Operator.Not));
            
            // Verify the normal path has the correct constraints
            // The condition must be true for the RET to be reached
            Assert.IsTrue(normalPath.FinalState.PathConstraints.Any(c => 
                c is SymbolicExpression expr && 
                expr.Op != Operator.Not));
        }

        /// <summary>
        /// Tests that the wrapper class correctly handles the complex scenario with both implementations.
        /// </summary>
        [TestMethod]
        public void Wrapper_ComplexScenario_BothImplementationsConsistent()
        {
            // Arrange - Create a script that includes a conditional branch and an ABORT
            byte[] script = new byte[]
            {
                (byte)OpCode.PUSHDATA1,   // Push symbolic variable
                0x01,                      // Length
                0x00,                      // Placeholder
                (byte)OpCode.JMPIF,        // Jump if true
                0x03,                      // Jump offset (skip over ABORT)
                (byte)OpCode.ABORT,        // Abort execution (taken if condition is false)
                (byte)OpCode.PUSH1,        // Push 1 (taken if condition is true)
                (byte)OpCode.RET           // Return
            };
            
            // Initialize symbolic state with a variable
            var initialState = new SymbolicState();
            initialState.PushSymbolicVariable("testCondition");
            
            // Act - Execute with both implementations via wrapper
            var originalWrapper = new SymbolicVirtualMachineWrapper(
                script, _solver, initialState.Clone(), useRefactoredImplementation: false);
            var originalResult = originalWrapper.Execute();
            
            var refactoredWrapper = new SymbolicVirtualMachineWrapper(
                script, _solver, initialState.Clone(), useRefactoredImplementation: true);
            var refactoredResult = refactoredWrapper.Execute();
            
            // Assert - Verify both produce the same number of paths
            Assert.AreEqual(originalResult.ExecutionPaths.Count, refactoredResult.ExecutionPaths.Count);
            Assert.AreEqual(2, originalResult.ExecutionPaths.Count);
            
            // Count paths by halt reason for original implementation
            int originalAbortCount = originalResult.ExecutionPaths.Count(p => p.HaltReason == HaltReason.Abort);
            int originalNormalCount = originalResult.ExecutionPaths.Count(p => p.HaltReason == HaltReason.Normal);
            
            // Count paths by halt reason for refactored implementation
            int refactoredAbortCount = refactoredResult.ExecutionPaths.Count(p => p.HaltReason == HaltReason.Abort);
            int refactoredNormalCount = refactoredResult.ExecutionPaths.Count(p => p.HaltReason == HaltReason.Normal);
            
            // Verify both implementations found the same types of paths
            Assert.AreEqual(originalAbortCount, refactoredAbortCount);
            Assert.AreEqual(originalNormalCount, refactoredNormalCount);
            Assert.AreEqual(1, originalAbortCount);
            Assert.AreEqual(1, originalNormalCount);
        }
    }
}