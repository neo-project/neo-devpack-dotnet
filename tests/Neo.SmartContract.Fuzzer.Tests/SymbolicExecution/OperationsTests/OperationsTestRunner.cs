using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Operations;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.VM;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.Tests.SymbolicExecution.OperationsTests
{
    /// <summary>
    /// Test runner for the operations tests.
    /// </summary>
    [TestClass]
    public class OperationsTestRunner
    {
        /// <summary>
        /// Tests the PUSH1 operation.
        /// </summary>
        [TestMethod]
        public void TestPush1Operation()
        {
            // Arrange
            byte[] script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            var vm = new SymbolicVirtualMachine(script, solver, evaluationService, 100, 10);
            var stackOps = new StackOperations(vm, script);
            vm.CurrentState = new SymbolicState(script, 0, 0);

            // Act
            bool result = stackOps.ExecuteOperation(vm, OpCode.PUSH1);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, vm.CurrentState.EvaluationStack.Count);
            var topValue = vm.CurrentState.Pop();
            Assert.IsTrue(topValue.IsConcrete);
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(1, ((ConcreteValue<int>)topValue).Value);
        }

        /// <summary>
        /// Tests the POW operation.
        /// </summary>
        [TestMethod]
        public void TestPowOperation()
        {
            // Arrange
            byte[] script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            var vm = new SymbolicVirtualMachine(script, solver, evaluationService, 100, 10);
            var advancedArithmeticOps = new AdvancedArithmeticOperations(vm, script);
            vm.CurrentState = new SymbolicState(script, 0, 0);
            vm.CurrentState.Push(new ConcreteValue<int>(2)); // Base
            vm.CurrentState.Push(new ConcreteValue<int>(3)); // Exponent

            // Act
            bool result = advancedArithmeticOps.ExecuteOperation(vm, OpCode.POW);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, vm.CurrentState.EvaluationStack.Count);
            var topValue = vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(8, ((ConcreteValue<int>)topValue).Value); // 2^3 = 8
        }

        /// <summary>
        /// Tests the NIP operation.
        /// </summary>
        [TestMethod]
        public void TestNipOperation()
        {
            // Arrange
            byte[] script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            var vm = new SymbolicVirtualMachine(script, solver, evaluationService, 100, 10);
            var extendedStackOps = new ExtendedStackOperations(vm, script);
            vm.CurrentState = new SymbolicState(script, 0, 0);
            vm.CurrentState.Push(new ConcreteValue<int>(1)); // Bottom
            vm.CurrentState.Push(new ConcreteValue<int>(2)); // Middle (to be removed)
            vm.CurrentState.Push(new ConcreteValue<int>(3)); // Top

            // Act
            bool result = extendedStackOps.ExecuteOperation(vm, OpCode.NIP);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, vm.CurrentState.EvaluationStack.Count);
            var topValue = vm.CurrentState.Pop();
            var bottomValue = vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.IsInstanceOfType(bottomValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(3, ((ConcreteValue<int>)topValue).Value); // Top item remains
            Assert.AreEqual(1, ((ConcreteValue<int>)bottomValue).Value); // Bottom item remains
        }

        /// <summary>
        /// Tests the ABORTMSG operation.
        /// </summary>
        [TestMethod]
        public void TestAbortMsgOperation()
        {
            // Arrange
            byte[] script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            var vm = new SymbolicVirtualMachine(script, solver, evaluationService, 100, 10);
            var extensionOps = new ExtensionOperations(vm, script);
            vm.CurrentState = new SymbolicState(script, 0, 0);
            vm.CurrentState.Push(new SymbolicByteArray(new byte[] { 1, 2, 3 })); // Message

            // Act
            bool result = extensionOps.ExecuteOperation(vm, OpCode.ABORTMSG);

            // Assert
            Assert.IsTrue(result);
            // The operation should have created two pending states: one for abort and one for non-abort
            Assert.AreEqual(2, vm.PendingStates.Count);
        }
    }
}
