using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Operations
{
    /// <summary>
    /// Handles all advanced arithmetic operations in the symbolic virtual machine.
    /// This class is responsible for operations like POW, SQRT, MODMUL, and MODPOW.
    /// </summary>
    public class AdvancedArithmeticOperations : BaseOperations
    {
        /// <summary>
        /// The script being executed.
        /// </summary>
        private readonly byte[] _script;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedArithmeticOperations"/> class.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="script">The script being executed.</param>
        public AdvancedArithmeticOperations(ISymbolicExecutionEngine engine, byte[] script) : base(engine)
        {
            _script = script ?? throw new ArgumentNullException(nameof(script));
        }

        /// <summary>
        /// Executes an advanced arithmetic operation if supported by this handler.
        /// </summary>
        /// <param name="engine">The symbolic execution engine.</param>
        /// <param name="opcode">The operation code to execute.</param>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        public override bool ExecuteOperation(ISymbolicExecutionEngine engine, OpCode opcode)
        {
            switch (opcode)
            {
                case OpCode.POW:
                    return HandlePow();
                case OpCode.SQRT:
                    return HandleSqrt();
                case OpCode.MODMUL:
                    return HandleModMul();
                case OpCode.MODPOW:
                    return HandleModPow();
                default:
                    return false;
            }
        }

        /// <summary>
        /// Handles the POW operation, which raises x to the power of y.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandlePow()
        {
            // Pop the exponent and base from the stack
            var exponent = _engine.CurrentState.Pop();
            var @base = _engine.CurrentState.Pop();

            if (exponent == null || @base == null)
            {
                LogDebug("POW: Stack underflow");
                return false;
            }

            // If both operands are concrete values, compute the result
            if (@base is ConcreteValue<int> intBase && exponent is ConcreteValue<int> intExponent)
            {
                try
                {
                    var result = (int)Math.Pow(intBase.Value, intExponent.Value);
                    _engine.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(result)));
                    LogDebug($"Computed {intBase.Value} ^ {intExponent.Value} = {result}");
                }
                catch (OverflowException)
                {
                    LogDebug($"POW: Overflow computing {intBase.Value} ^ {intExponent.Value}");
                    ((SymbolicState)_engine.CurrentState).Halt(VMState.FAULT);
                    return false;
                }
            }
            else if (@base is ConcreteValue<BigInteger> bigIntBase && exponent is ConcreteValue<BigInteger> bigIntExponent)
            {
                try
                {
                    var result = BigInteger.Pow(bigIntBase.Value, (int)bigIntExponent.Value);
                    _engine.CurrentState.Push(new ConcreteValue<BigInteger>(result));
                    LogDebug($"Computed {bigIntBase.Value} ^ {bigIntExponent.Value} = {result}");
                }
                catch (OverflowException)
                {
                    LogDebug($"POW: Overflow computing {bigIntBase.Value} ^ {bigIntExponent.Value}");
                    ((SymbolicState)_engine.CurrentState).Halt(VMState.FAULT);
                    return false;
                }
            }
            else
            {
                // If either operand is symbolic, create a symbolic variable to represent the result
                var symbolicResult = new SymbolicVariable($"Pow_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Integer);
                _engine.CurrentState.Push(symbolicResult);
                LogDebug($"Created symbolic result for POW: {symbolicResult}");
            }

            return true;
        }

        /// <summary>
        /// Handles the SQRT operation, which computes the square root of x.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleSqrt()
        {
            // Pop the operand from the stack
            var operand = _engine.CurrentState.Pop();

            if (operand == null)
            {
                LogDebug("SQRT: Stack underflow");
                return false;
            }

            // If the operand is a concrete value, compute the result
            if (operand is ConcreteValue<int> intOperand)
            {
                if (intOperand.Value < 0)
                {
                    LogDebug($"SQRT: Cannot compute square root of negative number {intOperand.Value}");
                    ((SymbolicState)_engine.CurrentState).Halt(VMState.FAULT);
                    return false;
                }

                var result = (int)Math.Sqrt(intOperand.Value);
                _engine.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(result)));
                LogDebug($"Computed sqrt({intOperand.Value}) = {result}");
            }
            else if (operand is ConcreteValue<BigInteger> bigIntOperand)
            {
                if (bigIntOperand.Value < 0)
                {
                    LogDebug($"SQRT: Cannot compute square root of negative number {bigIntOperand.Value}");
                    ((SymbolicState)_engine.CurrentState).Halt(VMState.FAULT);
                    return false;
                }

                // Compute the square root of a BigInteger using Newton's method
                var result = Sqrt(bigIntOperand.Value);
                _engine.CurrentState.Push(new ConcreteValue<BigInteger>(result));
                LogDebug($"Computed sqrt({bigIntOperand.Value}) = {result}");
            }
            else
            {
                // If the operand is symbolic, create a symbolic variable to represent the result
                var symbolicResult = new SymbolicVariable($"Sqrt_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Integer);
                _engine.CurrentState.Push(symbolicResult);
                LogDebug($"Created symbolic result for SQRT: {symbolicResult}");
            }

            return true;
        }

        /// <summary>
        /// Handles the MODMUL operation, which computes (x * y) % m.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleModMul()
        {
            // Pop the modulus, y, and x from the stack
            var modulus = _engine.CurrentState.Pop();
            var y = _engine.CurrentState.Pop();
            var x = _engine.CurrentState.Pop();

            if (modulus == null || y == null || x == null)
            {
                LogDebug("MODMUL: Stack underflow");
                return false;
            }

            // If all operands are concrete values, compute the result
            if (x is ConcreteValue<int> intX && y is ConcreteValue<int> intY && modulus is ConcreteValue<int> intModulus)
            {
                if (intModulus.Value == 0)
                {
                    LogDebug("MODMUL: Division by zero");
                    ((SymbolicState)_engine.CurrentState).Halt(VMState.FAULT);
                    return false;
                }

                try
                {
                    var result = (int)(((long)intX.Value * intY.Value) % intModulus.Value);
                    _engine.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(result)));
                    LogDebug($"Computed ({intX.Value} * {intY.Value}) % {intModulus.Value} = {result}");
                }
                catch (OverflowException)
                {
                    LogDebug($"MODMUL: Overflow computing ({intX.Value} * {intY.Value}) % {intModulus.Value}");
                    ((SymbolicState)_engine.CurrentState).Halt(VMState.FAULT);
                    return false;
                }
            }
            else if (x is ConcreteValue<BigInteger> bigIntX && y is ConcreteValue<BigInteger> bigIntY && modulus is ConcreteValue<BigInteger> bigIntModulus)
            {
                if (bigIntModulus.Value == 0)
                {
                    LogDebug("MODMUL: Division by zero");
                    ((SymbolicState)_engine.CurrentState).Halt(VMState.FAULT);
                    return false;
                }

                try
                {
                    var result = (bigIntX.Value * bigIntY.Value) % bigIntModulus.Value;
                    _engine.CurrentState.Push(new ConcreteValue<BigInteger>(result));
                    LogDebug($"Computed ({bigIntX.Value} * {bigIntY.Value}) % {bigIntModulus.Value} = {result}");
                }
                catch (DivideByZeroException)
                {
                    LogDebug("MODMUL: Division by zero");
                    ((SymbolicState)_engine.CurrentState).Halt(VMState.FAULT);
                    return false;
                }
            }
            else
            {
                // If any operand is symbolic, create a symbolic variable to represent the result
                var symbolicResult = new SymbolicVariable($"ModMul_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Integer);
                _engine.CurrentState.Push(symbolicResult);
                LogDebug($"Created symbolic result for MODMUL: {symbolicResult}");
            }

            return true;
        }

        /// <summary>
        /// Handles the MODPOW operation, which computes (x ^ y) % m.
        /// </summary>
        /// <returns>True if the operation was handled, false otherwise.</returns>
        private bool HandleModPow()
        {
            // Pop the modulus, exponent, and base from the stack
            var modulus = _engine.CurrentState.Pop();
            var exponent = _engine.CurrentState.Pop();
            var @base = _engine.CurrentState.Pop();

            if (modulus == null || exponent == null || @base == null)
            {
                LogDebug("MODPOW: Stack underflow");
                return false;
            }

            // If all operands are concrete values, compute the result
            if (@base is ConcreteValue<int> intBase && exponent is ConcreteValue<int> intExponent && modulus is ConcreteValue<int> intModulus)
            {
                if (intModulus.Value == 0)
                {
                    LogDebug("MODPOW: Division by zero");
                    ((SymbolicState)_engine.CurrentState).Halt(VMState.FAULT);
                    return false;
                }

                try
                {
                    var result = ModPow(intBase.Value, intExponent.Value, intModulus.Value);
                    _engine.CurrentState.Push(new ConcreteValue<BigInteger>(new BigInteger(result)));
                    LogDebug($"Computed ({intBase.Value} ^ {intExponent.Value}) % {intModulus.Value} = {result}");
                }
                catch (OverflowException)
                {
                    LogDebug($"MODPOW: Overflow computing ({intBase.Value} ^ {intExponent.Value}) % {intModulus.Value}");
                    ((SymbolicState)_engine.CurrentState).Halt(VMState.FAULT);
                    return false;
                }
            }
            else if (@base is ConcreteValue<BigInteger> bigIntBase && exponent is ConcreteValue<BigInteger> bigIntExponent && modulus is ConcreteValue<BigInteger> bigIntModulus)
            {
                if (bigIntModulus.Value == 0)
                {
                    LogDebug("MODPOW: Division by zero");
                    ((SymbolicState)_engine.CurrentState).Halt(VMState.FAULT);
                    return false;
                }

                try
                {
                    var result = BigInteger.ModPow(bigIntBase.Value, bigIntExponent.Value, bigIntModulus.Value);
                    _engine.CurrentState.Push(new ConcreteValue<BigInteger>(result));
                    LogDebug($"Computed ({bigIntBase.Value} ^ {bigIntExponent.Value}) % {bigIntModulus.Value} = {result}");
                }
                catch (DivideByZeroException)
                {
                    LogDebug("MODPOW: Division by zero");
                    ((SymbolicState)_engine.CurrentState).Halt(VMState.FAULT);
                    return false;
                }
            }
            else
            {
                // If any operand is symbolic, create a symbolic variable to represent the result
                var symbolicResult = new SymbolicVariable($"ModPow_Result_{_engine.CurrentState.InstructionPointer}", VM.Types.StackItemType.Integer);
                _engine.CurrentState.Push(symbolicResult);
                LogDebug($"Created symbolic result for MODPOW: {symbolicResult}");
            }

            return true;
        }

        /// <summary>
        /// Computes the square root of a BigInteger using Newton's method.
        /// </summary>
        /// <param name="n">The number to compute the square root of.</param>
        /// <returns>The square root of n.</returns>
        private static BigInteger Sqrt(BigInteger n)
        {
            if (n == 0) return 0;
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "Cannot compute square root of negative number.");

            BigInteger x = n;
            BigInteger y = (x + 1) / 2;

            while (y < x)
            {
                x = y;
                y = (x + n / x) / 2;
            }

            return x;
        }

        /// <summary>
        /// Computes (base ^ exponent) % modulus for integers.
        /// </summary>
        /// <param name="base">The base.</param>
        /// <param name="exponent">The exponent.</param>
        /// <param name="modulus">The modulus.</param>
        /// <returns>(base ^ exponent) % modulus.</returns>
        private static int ModPow(int @base, int exponent, int modulus)
        {
            if (modulus == 0) throw new DivideByZeroException("Modulus cannot be zero.");
            if (exponent < 0) throw new ArgumentOutOfRangeException(nameof(exponent), "Exponent cannot be negative.");

            long result = 1;
            long b = @base % modulus;

            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                {
                    result = (result * b) % modulus;
                }

                exponent >>= 1;
                b = (b * b) % modulus;
            }

            return (int)result;
        }
    }
}
