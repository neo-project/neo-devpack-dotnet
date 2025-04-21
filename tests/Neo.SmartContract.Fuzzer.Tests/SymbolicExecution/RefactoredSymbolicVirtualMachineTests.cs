using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.Tests.SymbolicExecution
{
    /// <summary>
    /// Tests for the refactored symbolic virtual machine implementation.
    /// These tests verify that the refactored implementation produces the same results as the original.
    /// </summary>
    [TestClass]
    public class RefactoredSymbolicVirtualMachineTests
    {
        private IConstraintSolver _solver;

        [TestInitialize]
        public void Setup()
        {
            _solver = new Z3ConstraintSolver();
        }

        /// <summary>
        /// Tests that both implementations produce the same results for a simple arithmetic script.
        /// </summary>
        [TestMethod]
        public void Test_SimpleArithmetic_BothImplementationsSame()
        {
            // Arrange
            byte[] script = new byte[]
            {
                (byte)OpCode.PUSH1,   // Push 1
                (byte)OpCode.PUSH2,   // Push 2
                (byte)OpCode.ADD,     // Add (1 + 2 = 3)
                (byte)OpCode.RET      // Return
            };

            // Act
            var originalResult = ExecuteWithOriginal(script);
            var refactoredResult = ExecuteWithRefactored(script);

            // Assert
            Assert.AreEqual(originalResult.ExecutionPaths.Count, refactoredResult.ExecutionPaths.Count);
            Assert.AreEqual(HaltReason.Normal, originalResult.ExecutionPaths[0].HaltReason);
            Assert.AreEqual(HaltReason.Normal, refactoredResult.ExecutionPaths[0].HaltReason);
            CompareStacks(originalResult.ExecutionPaths[0].FinalState, refactoredResult.ExecutionPaths[0].FinalState);
        }

        /// <summary>
        /// Tests that both implementations handle the ABORT opcode correctly.
        /// </summary>
        [TestMethod]
        public void Test_AbortOpcode_BothImplementationsSame()
        {
            // Arrange
            byte[] script = new byte[]
            {
                (byte)OpCode.ABORT    // Abort execution
            };

            // Act
            var originalResult = ExecuteWithOriginal(script);
            var refactoredResult = ExecuteWithRefactored(script);

            // Assert
            Assert.AreEqual(originalResult.ExecutionPaths.Count, refactoredResult.ExecutionPaths.Count);
            Assert.AreEqual(HaltReason.Abort, originalResult.ExecutionPaths[0].HaltReason);
            Assert.AreEqual(HaltReason.Abort, refactoredResult.ExecutionPaths[0].HaltReason);
        }

        /// <summary>
        /// Tests that both implementations handle conditional jumps with concrete values correctly.
        /// </summary>
        [TestMethod]
        public void Test_ConcreteConditionalJump_BothImplementationsSame()
        {
            // Arrange
            byte[] script = new byte[]
            {
                (byte)OpCode.PUSH1,   // Push 1 (true)
                (byte)OpCode.JMPIF,   // Jump if true
                0x03,                 // Jump offset
                (byte)OpCode.PUSH2,   // Push 2 (if false path)
                (byte)OpCode.JMP,     // Jump past the true path
                0x02,                 // Jump offset
                (byte)OpCode.PUSH3,   // Push 3 (if true path)
                (byte)OpCode.RET      // Return
            };

            // Act
            var originalResult = ExecuteWithOriginal(script);
            var refactoredResult = ExecuteWithRefactored(script);

            // Assert
            Assert.AreEqual(originalResult.ExecutionPaths.Count, refactoredResult.ExecutionPaths.Count);
            Assert.AreEqual(HaltReason.Normal, originalResult.ExecutionPaths[0].HaltReason);
            Assert.AreEqual(HaltReason.Normal, refactoredResult.ExecutionPaths[0].HaltReason);
            CompareStacks(originalResult.ExecutionPaths[0].FinalState, refactoredResult.ExecutionPaths[0].FinalState);
        }

        /// <summary>
        /// Tests that both implementations handle conditional jumps with symbolic values correctly.
        /// </summary>
        [TestMethod]
        public void Test_SymbolicConditionalJump_BothImplementationsSame()
        {
            // Arrange
            byte[] script = new byte[]
            {
                (byte)OpCode.PUSHDATA1,   // Push symbolic variable
                0x01,                      // Length
                0x00,                      // Placeholder
                (byte)OpCode.JMPIF,        // Jump if true
                0x03,                      // Jump offset
                (byte)OpCode.PUSH2,        // Push 2 (if false path)
                (byte)OpCode.JMP,          // Jump past the true path
                0x02,                      // Jump offset
                (byte)OpCode.PUSH3,        // Push 3 (if true path)
                (byte)OpCode.RET           // Return
            };

            // Initialize symbolic state
            var initialState = new SymbolicState();
            initialState.PushSymbolicVariable("var1");

            // Act
            var originalResult = ExecuteWithOriginal(script, initialState);
            var refactoredResult = ExecuteWithRefactored(script, initialState);

            // Assert
            Assert.AreEqual(originalResult.ExecutionPaths.Count, refactoredResult.ExecutionPaths.Count);
            Assert.AreEqual(2, originalResult.ExecutionPaths.Count);
            Assert.AreEqual(2, refactoredResult.ExecutionPaths.Count);
        }

        /// <summary>
        /// Tests that both implementations handle ByteString comparisons correctly.
        /// </summary>
        [TestMethod]
        public void Test_ByteStringComparison_BothImplementationsSame()
        {
            // Arrange
            byte[] script = new byte[]
            {
                (byte)OpCode.PUSHDATA1,   // Push symbolic variable
                0x01,                      // Length
                0x00,                      // Placeholder
                (byte)OpCode.PUSHDATA1,    // Push concrete ByteString
                0x01,                      // Length
                0x01,                      // Value 0x01
                (byte)OpCode.EQUAL,        // Compare
                (byte)OpCode.JMPIF,        // Jump if true
                0x03,                      // Jump offset
                (byte)OpCode.PUSH2,        // Push 2 (if false path)
                (byte)OpCode.JMP,          // Jump past the true path
                0x02,                      // Jump offset
                (byte)OpCode.PUSH3,        // Push 3 (if true path)
                (byte)OpCode.RET           // Return
            };

            // Initialize symbolic state
            var initialState = new SymbolicState();
            initialState.PushSymbolicVariable("var1");

            // Act
            var originalResult = ExecuteWithOriginal(script, initialState);
            var refactoredResult = ExecuteWithRefactored(script, initialState);

            // Assert
            Assert.AreEqual(originalResult.ExecutionPaths.Count, refactoredResult.ExecutionPaths.Count);
            Assert.AreEqual(2, originalResult.ExecutionPaths.Count);
            Assert.AreEqual(2, refactoredResult.ExecutionPaths.Count);
        }

        #region Helper Methods

        /// <summary>
        /// Executes a script with the original symbolic virtual machine.
        /// </summary>
        /// <param name="script">The script to execute.</param>
        /// <param name="initialState">The initial state.</param>
        /// <returns>The execution result.</returns>
        private SymbolicExecutionResult ExecuteWithOriginal(byte[] script, SymbolicState initialState = null)
        {
            var vm = new SymbolicVirtualMachine(script, _solver, initialState?.Clone());
            return vm.Execute();
        }

        /// <summary>
        /// Executes a script with the refactored symbolic virtual machine.
        /// </summary>
        /// <param name="script">The script to execute.</param>
        /// <param name="initialState">The initial state.</param>
        /// <returns>The execution result.</returns>
        private SymbolicExecutionResult ExecuteWithRefactored(byte[] script, SymbolicState initialState = null)
        {
            var vm = new RefactoredSymbolicVirtualMachine(script, _solver, initialState?.Clone());
            return vm.Execute();
        }

        /// <summary>
        /// Compares the stacks of two symbolic states.
        /// </summary>
        /// <param name="original">The original state.</param>
        /// <param name="refactored">The refactored state.</param>
        private void CompareStacks(SymbolicState original, SymbolicState refactored)
        {
            Assert.AreEqual(original.Stack.Count, refactored.Stack.Count);

            for (int i = 0; i < original.Stack.Count; i++)
            {
                var originalValue = original.Stack.ElementAt(i);
                var refactoredValue = refactored.Stack.ElementAt(i);

                // For concrete values, compare the actual values
                if (originalValue.IsConcrete && refactoredValue.IsConcrete)
                {
                    if (originalValue is ConcreteValue<int> originalInt && refactoredValue is ConcreteValue<int> refactoredInt)
                    {
                        Assert.AreEqual(originalInt.Value, refactoredInt.Value);
                    }
                    else if (originalValue is ConcreteValue<bool> originalBool && refactoredValue is ConcreteValue<bool> refactoredBool)
                    {
                        Assert.AreEqual(originalBool.Value, refactoredBool.Value);
                    }
                    // Add more type comparisons as needed
                }
                // For symbolic values, compare the structure
                else
                {
                    // This is a simplistic comparison - in practice, you would need more sophisticated
                    // comparison logic for symbolic expressions
                    Assert.AreEqual(originalValue.IsConcrete, refactoredValue.IsConcrete);
                    Assert.AreEqual(originalValue.GetType(), refactoredValue.GetType());
                }
            }
        }

        #endregion
    }
}