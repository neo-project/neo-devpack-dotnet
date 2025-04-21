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
    /// Tests for the ExtendedStackOperations class.
    /// </summary>
    [TestClass]
    public class ExtendedStackOperationsTests
    {
        private ISymbolicExecutionEngine _vm;
        private ExtendedStackOperations _extendedStackOps;
        private byte[] _script;

        [TestInitialize]
        public void Setup()
        {
            _script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            _vm = new SymbolicVirtualMachine(_script, solver, evaluationService, 100, 10);
            _extendedStackOps = new ExtendedStackOperations(_vm, _script);
        }

        /// <summary>
        /// Tests the NIP operation.
        /// </summary>
        [TestMethod]
        public void NipOperation_RemovesSecondItemFromStack()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new ConcreteValue<int>(1)); // Bottom
            _vm.CurrentState.Push(new ConcreteValue<int>(2)); // Middle (to be removed)
            _vm.CurrentState.Push(new ConcreteValue<int>(3)); // Top

            // Act
            bool result = _extendedStackOps.ExecuteOperation(_vm, OpCode.NIP);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, _vm.CurrentState.EvaluationStack.Count);
            var topValue = _vm.CurrentState.Pop();
            var bottomValue = _vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.IsInstanceOfType(bottomValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(3, ((ConcreteValue<int>)topValue).Value); // Top item remains
            Assert.AreEqual(1, ((ConcreteValue<int>)bottomValue).Value); // Bottom item remains
        }

        /// <summary>
        /// Tests the XDROP operation.
        /// </summary>
        [TestMethod]
        public void XDropOperation_RemovesItemAtSpecifiedIndex()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new ConcreteValue<int>(1)); // Item at index 2 (to be removed)
            _vm.CurrentState.Push(new ConcreteValue<int>(2)); // Item at index 1
            _vm.CurrentState.Push(new ConcreteValue<int>(3)); // Item at index 0
            _vm.CurrentState.Push(new ConcreteValue<int>(2)); // Index to remove (removes item at index 2)

            // Act
            bool result = _extendedStackOps.ExecuteOperation(_vm, OpCode.XDROP);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, _vm.CurrentState.EvaluationStack.Count);
            var topValue = _vm.CurrentState.Pop();
            var bottomValue = _vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.IsInstanceOfType(bottomValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(3, ((ConcreteValue<int>)topValue).Value);
            Assert.AreEqual(2, ((ConcreteValue<int>)bottomValue).Value);
        }

        /// <summary>
        /// Tests the CLEAR operation.
        /// </summary>
        [TestMethod]
        public void ClearOperation_RemovesAllItemsFromStack()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new ConcreteValue<int>(1));
            _vm.CurrentState.Push(new ConcreteValue<int>(2));
            _vm.CurrentState.Push(new ConcreteValue<int>(3));

            // Act
            bool result = _extendedStackOps.ExecuteOperation(_vm, OpCode.CLEAR);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, _vm.CurrentState.EvaluationStack.Count);
        }

        /// <summary>
        /// Tests the REVERSE3 operation.
        /// </summary>
        [TestMethod]
        public void Reverse3Operation_ReversesTop3ItemsOnStack()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new ConcreteValue<int>(1)); // Bottom
            _vm.CurrentState.Push(new ConcreteValue<int>(2)); // Middle
            _vm.CurrentState.Push(new ConcreteValue<int>(3)); // Top

            // Act
            bool result = _extendedStackOps.ExecuteOperation(_vm, OpCode.REVERSE3);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(3, _vm.CurrentState.EvaluationStack.Count);
            var topValue = _vm.CurrentState.Pop();
            var middleValue = _vm.CurrentState.Pop();
            var bottomValue = _vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.IsInstanceOfType(middleValue, typeof(ConcreteValue<int>));
            Assert.IsInstanceOfType(bottomValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(1, ((ConcreteValue<int>)topValue).Value);
            Assert.AreEqual(2, ((ConcreteValue<int>)middleValue).Value);
            Assert.AreEqual(3, ((ConcreteValue<int>)bottomValue).Value);
        }

        /// <summary>
        /// Tests the REVERSE4 operation.
        /// </summary>
        [TestMethod]
        public void Reverse4Operation_ReversesTop4ItemsOnStack()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new ConcreteValue<int>(1)); // Bottom
            _vm.CurrentState.Push(new ConcreteValue<int>(2)); // Middle-bottom
            _vm.CurrentState.Push(new ConcreteValue<int>(3)); // Middle-top
            _vm.CurrentState.Push(new ConcreteValue<int>(4)); // Top

            // Act
            bool result = _extendedStackOps.ExecuteOperation(_vm, OpCode.REVERSE4);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(4, _vm.CurrentState.EvaluationStack.Count);
            var topValue = _vm.CurrentState.Pop();
            var middleTopValue = _vm.CurrentState.Pop();
            var middleBottomValue = _vm.CurrentState.Pop();
            var bottomValue = _vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.IsInstanceOfType(middleTopValue, typeof(ConcreteValue<int>));
            Assert.IsInstanceOfType(middleBottomValue, typeof(ConcreteValue<int>));
            Assert.IsInstanceOfType(bottomValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(1, ((ConcreteValue<int>)topValue).Value);
            Assert.AreEqual(2, ((ConcreteValue<int>)middleTopValue).Value);
            Assert.AreEqual(3, ((ConcreteValue<int>)middleBottomValue).Value);
            Assert.AreEqual(4, ((ConcreteValue<int>)bottomValue).Value);
        }

        /// <summary>
        /// Tests the REVERSEN operation.
        /// </summary>
        [TestMethod]
        public void ReverseNOperation_ReversesTopNItemsOnStack()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new ConcreteValue<int>(1)); // Bottom
            _vm.CurrentState.Push(new ConcreteValue<int>(2)); // Middle-bottom
            _vm.CurrentState.Push(new ConcreteValue<int>(3)); // Middle-top
            _vm.CurrentState.Push(new ConcreteValue<int>(4)); // Top
            _vm.CurrentState.Push(new ConcreteValue<int>(4)); // Count to reverse

            // Act
            bool result = _extendedStackOps.ExecuteOperation(_vm, OpCode.REVERSEN);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(4, _vm.CurrentState.EvaluationStack.Count);
            var topValue = _vm.CurrentState.Pop();
            var middleTopValue = _vm.CurrentState.Pop();
            var middleBottomValue = _vm.CurrentState.Pop();
            var bottomValue = _vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.IsInstanceOfType(middleTopValue, typeof(ConcreteValue<int>));
            Assert.IsInstanceOfType(middleBottomValue, typeof(ConcreteValue<int>));
            Assert.IsInstanceOfType(bottomValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(1, ((ConcreteValue<int>)topValue).Value);
            Assert.AreEqual(2, ((ConcreteValue<int>)middleTopValue).Value);
            Assert.AreEqual(3, ((ConcreteValue<int>)middleBottomValue).Value);
            Assert.AreEqual(4, ((ConcreteValue<int>)bottomValue).Value);
        }
    }
}
