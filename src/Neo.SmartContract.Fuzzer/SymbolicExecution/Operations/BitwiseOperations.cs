using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Operations
{
    /// <summary>
    /// Handles all bitwise operations in the symbolic virtual machine.
    /// This class is responsible for operations like AND, OR, XOR, and INVERT.
    /// </summary>
    public class BitwiseOperations : BaseOperations
    {
        /// <summary>
        /// The evaluation service used to evaluate symbolic expressions.
        /// </summary>
        private readonly IEvaluationService _evaluationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitwiseOperations"/> class.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="evaluationService">The evaluation service.</param>
        public BitwiseOperations(ISymbolicExecutionEngine engine, IEvaluationService evaluationService) : base(engine)
        {
            _evaluationService = evaluationService ?? throw new ArgumentNullException(nameof(evaluationService));
        }

        /// <summary>
        /// Executes a bitwise operation if supported by this handler.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        public override bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode)
        {
            switch (opcode)
            {
                case OpCode.INVERT:
                    return ExecuteInvert();
                case OpCode.AND:
                    return ExecuteAnd();
                case OpCode.OR:
                    return ExecuteOr();
                case OpCode.XOR:
                    return ExecuteXor();
                default:
                    return false;
            }
        }

        /// <summary>
        /// Executes the INVERT operation.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool ExecuteInvert()
        {
            var value = _engine.CurrentState.Pop();

            if (value == null)
            {
                LogDebug("INVERT: Stack underflow");
                return false;
            }

            // Handle concrete values
            if (value is ConcreteValue<BigInteger> intValue)
            {
                var result = ~intValue.Value;
                _engine.CurrentState.Push(new ConcreteValue<BigInteger>(result));
                LogDebug($"INVERT: {intValue.Value} -> {result}");
                return true;
            }

            // Handle concrete boolean values
            if (value is ConcreteValue<bool> boolValue)
            {
                var result = !boolValue.Value;
                _engine.CurrentState.Push(new ConcreteValue<bool>(result));
                LogDebug($"INVERT: {boolValue.Value} -> {result}");
                return true;
            }

            // Handle symbolic values
            var symbolicResult = new SymbolicExpression(value, Operator.BitwiseNot, null);
            _engine.CurrentState.Push(symbolicResult);
            LogDebug($"INVERT: {value} -> {symbolicResult}");
            return true;
        }

        /// <summary>
        /// Executes the AND operation.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool ExecuteAnd()
        {
            var right = _engine.CurrentState.Pop();
            var left = _engine.CurrentState.Pop();

            if (left == null || right == null)
            {
                LogDebug("AND: Stack underflow");
                return false;
            }

            // Handle concrete integer values
            if (left is ConcreteValue<BigInteger> leftInt && right is ConcreteValue<BigInteger> rightInt)
            {
                var result = leftInt.Value & rightInt.Value;
                _engine.CurrentState.Push(new ConcreteValue<BigInteger>(result));
                LogDebug($"AND: {leftInt.Value} & {rightInt.Value} -> {result}");
                return true;
            }

            // Handle concrete boolean values
            if (left is ConcreteValue<bool> leftBool && right is ConcreteValue<bool> rightBool)
            {
                var result = leftBool.Value && rightBool.Value;
                _engine.CurrentState.Push(new ConcreteValue<bool>(result));
                LogDebug($"AND: {leftBool.Value} && {rightBool.Value} -> {result}");
                return true;
            }

            // Handle symbolic values
            var symbolicResult = new SymbolicExpression(left, Operator.BitwiseAnd, right);
            _engine.CurrentState.Push(symbolicResult);
            LogDebug($"AND: {left} & {right} -> {symbolicResult}");
            return true;
        }

        /// <summary>
        /// Executes the OR operation.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool ExecuteOr()
        {
            var right = _engine.CurrentState.Pop();
            var left = _engine.CurrentState.Pop();

            if (left == null || right == null)
            {
                LogDebug("OR: Stack underflow");
                return false;
            }

            // Handle concrete integer values
            if (left is ConcreteValue<BigInteger> leftInt && right is ConcreteValue<BigInteger> rightInt)
            {
                var result = leftInt.Value | rightInt.Value;
                _engine.CurrentState.Push(new ConcreteValue<BigInteger>(result));
                LogDebug($"OR: {leftInt.Value} | {rightInt.Value} -> {result}");
                return true;
            }

            // Handle concrete boolean values
            if (left is ConcreteValue<bool> leftBool && right is ConcreteValue<bool> rightBool)
            {
                var result = leftBool.Value || rightBool.Value;
                _engine.CurrentState.Push(new ConcreteValue<bool>(result));
                LogDebug($"OR: {leftBool.Value} || {rightBool.Value} -> {result}");
                return true;
            }

            // Handle symbolic values
            var symbolicResult = new SymbolicExpression(left, Operator.BitwiseOr, right);
            _engine.CurrentState.Push(symbolicResult);
            LogDebug($"OR: {left} | {right} -> {symbolicResult}");
            return true;
        }

        /// <summary>
        /// Executes the XOR operation.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool ExecuteXor()
        {
            var right = _engine.CurrentState.Pop();
            var left = _engine.CurrentState.Pop();

            if (left == null || right == null)
            {
                LogDebug("XOR: Stack underflow");
                return false;
            }

            // Handle concrete integer values
            if (left is ConcreteValue<BigInteger> leftInt && right is ConcreteValue<BigInteger> rightInt)
            {
                var result = leftInt.Value ^ rightInt.Value;
                _engine.CurrentState.Push(new ConcreteValue<BigInteger>(result));
                LogDebug($"XOR: {leftInt.Value} ^ {rightInt.Value} -> {result}");
                return true;
            }

            // Handle concrete boolean values
            if (left is ConcreteValue<bool> leftBool && right is ConcreteValue<bool> rightBool)
            {
                var result = leftBool.Value ^ rightBool.Value;
                _engine.CurrentState.Push(new ConcreteValue<bool>(result));
                LogDebug($"XOR: {leftBool.Value} ^ {rightBool.Value} -> {result}");
                return true;
            }

            // Handle symbolic values
            var symbolicResult = new SymbolicExpression(left, Operator.BitwiseXor, right);
            _engine.CurrentState.Push(symbolicResult);
            LogDebug($"XOR: {left} ^ {right} -> {symbolicResult}");
            return true;
        }
    }
}
