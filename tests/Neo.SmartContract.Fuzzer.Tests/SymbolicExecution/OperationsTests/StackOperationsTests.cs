using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Operations;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.Tests.SymbolicExecution.OperationsTests
{
    /// <summary>
    /// Tests for the StackOperations class.
    /// </summary>
    [TestClass]
    public class StackOperationsTests
    {
        private ISymbolicExecutionEngine _vm;
        private StackOperations _stackOperations;
        private byte[] _script;

        [TestInitialize]
        public void Setup()
        {
            _script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            _vm = new SymbolicVirtualMachine(_script, solver, evaluationService, 100, 10);
            _stackOperations = new StackOperations(_vm, _script);
        }

        /// <summary>
        /// Tests the PUSH operations.
        /// </summary>
        [TestMethod]
        public void PushOperation_PushesValueToStack()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);

            // Act - Test PUSH1
            bool result = _stackOperations.ExecuteOperation(_vm, OpCode.PUSH1);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, _vm.CurrentState.EvaluationStack.Count);
            var topValue = _vm.CurrentState.Pop();
            Assert.IsTrue(topValue.IsConcrete);
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(1, ((ConcreteValue<int>)topValue).Value);
        }

        /// <summary>
        /// Tests the DUP operation.
        /// </summary>
        [TestMethod]
        public void DupOperation_DuplicatesTopStackValue()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new ConcreteValue<int>(42));

            // Act
            bool result = _stackOperations.ExecuteOperation(_vm, OpCode.DUP);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, _vm.CurrentState.EvaluationStack.Count);
            var topValue = _vm.CurrentState.Pop();
            var secondValue = _vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.IsInstanceOfType(secondValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(42, ((ConcreteValue<int>)topValue).Value);
            Assert.AreEqual(42, ((ConcreteValue<int>)secondValue).Value);
        }

        /// <summary>
        /// Tests the SWAP operation.
        /// </summary>
        [TestMethod]
        public void SwapOperation_SwapsTwoTopStackValues()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new ConcreteValue<int>(1));
            _vm.CurrentState.Push(new ConcreteValue<int>(2));

            // Act
            bool result = _stackOperations.ExecuteOperation(_vm, OpCode.SWAP);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, _vm.CurrentState.EvaluationStack.Count);
            var topValue = _vm.CurrentState.Pop();
            var secondValue = _vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.IsInstanceOfType(secondValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(1, ((ConcreteValue<int>)topValue).Value);
            Assert.AreEqual(2, ((ConcreteValue<int>)secondValue).Value);
        }

        /// <summary>
        /// Tests the ROT operation.
        /// </summary>
        [TestMethod]
        public void RotOperation_RotatesThreeTopStackValues()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new ConcreteValue<int>(1)); // Bottom
            _vm.CurrentState.Push(new ConcreteValue<int>(2)); // Middle
            _vm.CurrentState.Push(new ConcreteValue<int>(3)); // Top

            // Act
            bool result = _stackOperations.ExecuteOperation(_vm, OpCode.ROT);

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
            Assert.AreEqual(3, ((ConcreteValue<int>)middleValue).Value);
            Assert.AreEqual(2, ((ConcreteValue<int>)bottomValue).Value);
        }

        /// <summary>
        /// Tests the DEPTH operation.
        /// </summary>
        [TestMethod]
        public void DepthOperation_PushesStackSizeToStack()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new ConcreteValue<int>(1));
            _vm.CurrentState.Push(new ConcreteValue<int>(2));
            _vm.CurrentState.Push(new ConcreteValue<int>(3));

            // Act
            bool result = _stackOperations.ExecuteOperation(_vm, OpCode.DEPTH);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(4, _vm.CurrentState.EvaluationStack.Count);
            var topValue = _vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(3, ((ConcreteValue<int>)topValue).Value);
        }

        /// <summary>
        /// Tests the PUSHDATA1 operation.
        /// </summary>
        [TestMethod]
        public void PushData1Operation_PushesByteStringToStack()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            // Setup a script with PUSHDATA1 instruction
            var scriptWithPushData = new byte[]
            {
                (byte)OpCode.PUSHDATA1,   // PUSHDATA1 opcode
                0x03,                      // Length (3 bytes)
                0x01, 0x02, 0x03          // Data bytes
            };
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            var vmWithPushData = new SymbolicVirtualMachine(scriptWithPushData, solver, evaluationService, 100, 10);
            var stackOps = new StackOperations(vmWithPushData, scriptWithPushData);

            // Act
            bool result = stackOps.ExecuteOperation(vmWithPushData, OpCode.PUSHDATA1);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, vmWithPushData.CurrentState.EvaluationStack.Count);
            var topValue = vmWithPushData.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(SymbolicByteArray));
            var byteArray = (SymbolicByteArray)topValue;
            Assert.AreEqual(3, byteArray.Value.Length);
            Assert.AreEqual(0x01, byteArray.Value.Span[0]);
            Assert.AreEqual(0x02, byteArray.Value.Span[1]);
            Assert.AreEqual(0x03, byteArray.Value.Span[2]);
        }

        /// <summary>
        /// Tests stack operations with stack underflow.
        /// </summary>
        [TestMethod]
        public void StackUnderflow_HaltsExecution()
        {
            // Arrange
            var state = new SymbolicState(_script, 0, 0);
            _vm.CurrentState = state;
            // Empty stack

            // Act - Try to execute SWAP which requires 2 stack items
            bool result = _stackOperations.ExecuteOperation(_vm, OpCode.SWAP);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(VMState.FAULT, ((SymbolicState)_vm.CurrentState).VMState);
        }
    }
}
