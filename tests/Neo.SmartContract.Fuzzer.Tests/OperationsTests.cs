using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Operations;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.VM;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.NewOperationsTests
{
    /// <summary>
    /// Tests for the new operations classes.
    /// </summary>
    [TestClass]
    public class OperationsTests
    {
        /// <summary>
        /// Tests the PUSH1 operation.
        /// </summary>
        [TestMethod]
        public void TestPush1Operation()
        {
            // Arrange
            var script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            var limits = new ExecutionEngineLimits();
            var vm = new SymbolicVirtualMachine(new Script(script), solver, evaluationService, limits);

            // Create the stack operations
            var stackOps = new StackOperations(vm, script);

            // Act
            bool result = stackOps.ExecuteOperation(vm, OpCode.PUSH1);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, vm.CurrentState.EvaluationStack.Count);
            var topValue = vm.CurrentState.Pop();
            Assert.IsTrue(topValue.IsConcrete);
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<BigInteger>));
            Assert.AreEqual(new BigInteger(1), ((ConcreteValue<BigInteger>)topValue).Value);
        }

        /// <summary>
        /// Tests the POW operation.
        /// </summary>
        [TestMethod]
        public void TestPowOperation()
        {
            // Arrange
            var script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            var limits = new ExecutionEngineLimits();
            var vm = new SymbolicVirtualMachine(new Script(script), solver, evaluationService, limits);
            var advancedArithmeticOps = new AdvancedArithmeticOperations(vm, script);
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(2))); // Base
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(3))); // Exponent

            // Act
            bool result = advancedArithmeticOps.ExecuteOperation(vm, OpCode.POW);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, vm.CurrentState.EvaluationStack.Count);
            var topValue = vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<BigInteger>));
            Assert.AreEqual(new BigInteger(8), ((ConcreteValue<BigInteger>)topValue).Value); // 2^3 = 8
        }

        /// <summary>
        /// Tests the NIP operation.
        /// </summary>
        [TestMethod]
        public void TestNipOperation()
        {
            // Arrange
            var script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            var limits = new ExecutionEngineLimits();
            var vm = new SymbolicVirtualMachine(new Script(script), solver, evaluationService, limits);
            var extendedStackOps = new ExtendedStackOperations(vm, script);
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(1))); // Bottom
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(2))); // Middle (to be removed)
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(3))); // Top

            // Act
            bool result = extendedStackOps.ExecuteOperation(vm, OpCode.NIP);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, vm.CurrentState.EvaluationStack.Count);
            var topValue = vm.CurrentState.Pop();
            var bottomValue = vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<BigInteger>));
            Assert.IsInstanceOfType(bottomValue, typeof(ConcreteValue<BigInteger>));
            Assert.AreEqual(new BigInteger(3), ((ConcreteValue<BigInteger>)topValue).Value); // Top item remains
            Assert.AreEqual(new BigInteger(1), ((ConcreteValue<BigInteger>)bottomValue).Value); // Bottom item remains
        }

        /// <summary>
        /// Tests the ABORTMSG operation.
        /// </summary>
        [TestMethod]
        public void TestAbortMsgOperation()
        {
            // Arrange
            var script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            var limits = new ExecutionEngineLimits();
            var vm = new SymbolicVirtualMachine(new Script(script), solver, evaluationService, limits);
            var extensionOps = new ExtensionOperations(vm, script);
            vm.CurrentState.Push(new SymbolicByteArray(new byte[] { 1, 2, 3 })); // Message

            // Act
            bool result = extensionOps.ExecuteOperation(vm, OpCode.ABORTMSG);

            // Assert
            Assert.IsTrue(result);
            // The operation should have created pending states: one for abort, one for non-abort, and the original state
            Assert.AreEqual(3, vm.PendingStates.Count);
        }

        /// <summary>
        /// Tests the SQRT operation.
        /// </summary>
        [TestMethod]
        public void TestSqrtOperation()
        {
            // Arrange
            var script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            var limits = new ExecutionEngineLimits();
            var vm = new SymbolicVirtualMachine(new Script(script), solver, evaluationService, limits);
            var advancedArithmeticOps = new AdvancedArithmeticOperations(vm, script);
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(16))); // Value to take square root of

            // Act
            bool result = advancedArithmeticOps.ExecuteOperation(vm, OpCode.SQRT);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, vm.CurrentState.EvaluationStack.Count);
            var topValue = vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<BigInteger>));
            Assert.AreEqual(new BigInteger(4), ((ConcreteValue<BigInteger>)topValue).Value); // sqrt(16) = 4
        }

        /// <summary>
        /// Tests the MODMUL operation.
        /// </summary>
        [TestMethod]
        public void TestModMulOperation()
        {
            // Arrange
            var script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            var limits = new ExecutionEngineLimits();
            var vm = new SymbolicVirtualMachine(new Script(script), solver, evaluationService, limits);
            var advancedArithmeticOps = new AdvancedArithmeticOperations(vm, script);
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(3))); // x
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(5))); // y
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(7))); // Modulus

            // Act
            bool result = advancedArithmeticOps.ExecuteOperation(vm, OpCode.MODMUL);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, vm.CurrentState.EvaluationStack.Count);
            var topValue = vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<BigInteger>));
            Assert.AreEqual(new BigInteger(1), ((ConcreteValue<BigInteger>)topValue).Value); // (3 * 5) % 7 = 15 % 7 = 1
        }

        /// <summary>
        /// Tests the MODPOW operation.
        /// </summary>
        [TestMethod]
        public void TestModPowOperation()
        {
            // Arrange
            var script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            var limits = new ExecutionEngineLimits();
            var vm = new SymbolicVirtualMachine(new Script(script), solver, evaluationService, limits);
            var advancedArithmeticOps = new AdvancedArithmeticOperations(vm, script);
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(2))); // Base
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(3))); // Exponent
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(7))); // Modulus

            // Act
            bool result = advancedArithmeticOps.ExecuteOperation(vm, OpCode.MODPOW);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, vm.CurrentState.EvaluationStack.Count);
            var topValue = vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<BigInteger>));
            Assert.AreEqual(new BigInteger(1), ((ConcreteValue<BigInteger>)topValue).Value); // (2^3) % 7 = 8 % 7 = 1
        }

        /// <summary>
        /// Tests the CLEAR operation.
        /// </summary>
        [TestMethod]
        public void TestClearOperation()
        {
            // Arrange
            var script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            var limits = new ExecutionEngineLimits();
            var vm = new SymbolicVirtualMachine(new Script(script), solver, evaluationService, limits);
            var extendedStackOps = new ExtendedStackOperations(vm, script);
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(1)));
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(2)));
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(3)));

            // Act
            bool result = extendedStackOps.ExecuteOperation(vm, OpCode.CLEAR);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, vm.CurrentState.EvaluationStack.Count);
        }

        /// <summary>
        /// Tests the REVERSE3 operation.
        /// </summary>
        [TestMethod]
        public void TestReverse3Operation()
        {
            // Arrange
            var script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            var limits = new ExecutionEngineLimits();
            var vm = new SymbolicVirtualMachine(new Script(script), solver, evaluationService, limits);
            var extendedStackOps = new ExtendedStackOperations(vm, script);
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(1))); // Bottom
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(2))); // Middle
            vm.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(3))); // Top

            // Act
            bool result = extendedStackOps.ExecuteOperation(vm, OpCode.REVERSE3);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(3, vm.CurrentState.EvaluationStack.Count);
            var topValue = vm.CurrentState.Pop();
            var middleValue = vm.CurrentState.Pop();
            var bottomValue = vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<BigInteger>));
            Assert.IsInstanceOfType(middleValue, typeof(ConcreteValue<BigInteger>));
            Assert.IsInstanceOfType(bottomValue, typeof(ConcreteValue<BigInteger>));
            Assert.AreEqual(new BigInteger(1), ((ConcreteValue<BigInteger>)topValue).Value);
            Assert.AreEqual(new BigInteger(2), ((ConcreteValue<BigInteger>)middleValue).Value);
            Assert.AreEqual(new BigInteger(3), ((ConcreteValue<BigInteger>)bottomValue).Value);
        }
    }
}
