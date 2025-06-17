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
    /// Tests for the AdvancedArithmeticOperations class.
    /// </summary>
    [TestClass]
    public class AdvancedArithmeticOperationsTests
    {
        private ISymbolicExecutionEngine _vm;
        private AdvancedArithmeticOperations _advancedArithmeticOps;
        private byte[] _script;

        [TestInitialize]
        public void Setup()
        {
            _script = new byte[100]; // Dummy script
            var evaluationService = new DefaultEvaluationService();
            var solver = new DefaultConstraintSolver();
            _vm = new SymbolicVirtualMachine(_script, solver, evaluationService, 100, 10);
            _advancedArithmeticOps = new AdvancedArithmeticOperations(_vm, _script);
        }

        /// <summary>
        /// Tests the POW operation with concrete values.
        /// </summary>
        [TestMethod]
        public void PowOperation_WithConcreteValues_ComputesCorrectResult()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new ConcreteValue<int>(3)); // Exponent
            _vm.CurrentState.Push(new ConcreteValue<int>(2)); // Base

            // Act
            bool result = _advancedArithmeticOps.ExecuteOperation(_vm, OpCode.POW);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, _vm.CurrentState.EvaluationStack.Count);
            var topValue = _vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(8, ((ConcreteValue<int>)topValue).Value); // 2^3 = 8
        }

        /// <summary>
        /// Tests the SQRT operation with concrete values.
        /// </summary>
        [TestMethod]
        public void SqrtOperation_WithConcreteValues_ComputesCorrectResult()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new ConcreteValue<int>(16)); // Value to take square root of

            // Act
            bool result = _advancedArithmeticOps.ExecuteOperation(_vm, OpCode.SQRT);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, _vm.CurrentState.EvaluationStack.Count);
            var topValue = _vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(4, ((ConcreteValue<int>)topValue).Value); // sqrt(16) = 4
        }

        /// <summary>
        /// Tests the MODMUL operation with concrete values.
        /// </summary>
        [TestMethod]
        public void ModMulOperation_WithConcreteValues_ComputesCorrectResult()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new ConcreteValue<int>(7)); // Modulus
            _vm.CurrentState.Push(new ConcreteValue<int>(5)); // y
            _vm.CurrentState.Push(new ConcreteValue<int>(3)); // x

            // Act
            bool result = _advancedArithmeticOps.ExecuteOperation(_vm, OpCode.MODMUL);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, _vm.CurrentState.EvaluationStack.Count);
            var topValue = _vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(1, ((ConcreteValue<int>)topValue).Value); // (3 * 5) % 7 = 15 % 7 = 1
        }

        /// <summary>
        /// Tests the MODPOW operation with concrete values.
        /// </summary>
        [TestMethod]
        public void ModPowOperation_WithConcreteValues_ComputesCorrectResult()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new ConcreteValue<int>(7)); // Modulus
            _vm.CurrentState.Push(new ConcreteValue<int>(3)); // Exponent
            _vm.CurrentState.Push(new ConcreteValue<int>(2)); // Base

            // Act
            bool result = _advancedArithmeticOps.ExecuteOperation(_vm, OpCode.MODPOW);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, _vm.CurrentState.EvaluationStack.Count);
            var topValue = _vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(ConcreteValue<int>));
            Assert.AreEqual(1, ((ConcreteValue<int>)topValue).Value); // (2^3) % 7 = 8 % 7 = 1
        }

        /// <summary>
        /// Tests the POW operation with symbolic values.
        /// </summary>
        [TestMethod]
        public void PowOperation_WithSymbolicValues_CreatesSymbolicResult()
        {
            // Arrange
            _vm.CurrentState = new SymbolicState(_script, 0, 0);
            _vm.CurrentState.Push(new SymbolicVariable("exponent", StackItemType.Integer)); // Exponent
            _vm.CurrentState.Push(new SymbolicVariable("base", StackItemType.Integer)); // Base

            // Act
            bool result = _advancedArithmeticOps.ExecuteOperation(_vm, OpCode.POW);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, _vm.CurrentState.EvaluationStack.Count);
            var topValue = _vm.CurrentState.Pop();
            Assert.IsInstanceOfType(topValue, typeof(SymbolicVariable));
            Assert.IsFalse(topValue.IsConcrete);
            Assert.AreEqual(StackItemType.Integer, topValue.Type);
        }
    }
}
