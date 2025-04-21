using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System;
using System.Linq; // Keep for simplification checks
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Evaluation
{
    /// <summary>
    /// Provides evaluation services for symbolic expressions during symbolic execution.
    /// This service creates and simplifies symbolic expressions for operations involving at least one symbolic value.
    /// It delegates to ConcreteEvaluation if all operands are concrete.
    /// </summary>
    public class SymbolicEvaluation : IEvaluationService
    {
        private readonly ConcreteEvaluation _concreteEval = new ConcreteEvaluation();

        // --- OpCode to Operator Mapping ---
        private static Operator MapOpCodeToOperator(OpCode vmOpCode)
        {
            return vmOpCode switch
            {
                // Arithmetic
                OpCode.INC => throw new NotImplementedException("INC operation should be handled directly, not creating an expression."), // Should be handled before Simplify
                OpCode.DEC => throw new NotImplementedException("DEC operation should be handled directly, not creating an expression."), // Should be handled before Simplify
                OpCode.SIGN => Operator.Sign,
                OpCode.NEGATE => Operator.Negate,
                OpCode.ABS => Operator.Abs,
                OpCode.ADD => Operator.Add,
                OpCode.SUB => Operator.Subtract,
                OpCode.MUL => Operator.Multiply,
                OpCode.DIV => Operator.Divide,
                OpCode.MOD => Operator.Modulo,
                // Bitwise (assuming they map directly for now)
                OpCode.SHL => Operator.LeftShift,
                OpCode.SHR => Operator.RightShift,
                // Logical
                OpCode.NOT => Operator.Not,
                OpCode.BOOLAND => Operator.And, // Map BOOLAND to logical And
                OpCode.BOOLOR => Operator.Or,   // Map BOOLOR to logical Or
                // Comparisons
                OpCode.NZ => Operator.NotEqual, // NZ checks if value is not zero (equivalent to != 0 symbolically)
                OpCode.NUMEQUAL => Operator.Equal,
                OpCode.NUMNOTEQUAL => Operator.NotEqual,
                OpCode.LT => Operator.LessThan,
                OpCode.LE => Operator.LessThanOrEqual,
                OpCode.GT => Operator.GreaterThan,
                OpCode.GE => Operator.GreaterThanOrEqual,
                OpCode.MIN => Operator.Min,
                OpCode.MAX => Operator.Max,
                OpCode.WITHIN => Operator.Within,
                // Default case for unhandled opcodes
                _ => throw new ArgumentOutOfRangeException(nameof(vmOpCode), $"OpCode {vmOpCode} cannot be mapped to a symbolic operator.")
            };
        }

        // --- Helper for Simplification ---

        /// <summary>
        /// Simplifies a potential symbolic expression resulting from an operation.
        /// If all inputs were concrete, it delegates to ConcreteEvaluation.
        /// If inputs were symbolic, it creates a SymbolicExpression and attempts algebraic simplification.
        /// </summary>
        private SymbolicValue Simplify(OpCode opcode, SymbolicValue operand)
        {
            if (operand.IsConcrete)
            {
                // If the single operand is concrete, the operation should be handled by ConcreteEvaluation.
                // This path might indicate a logic error elsewhere, as SymbolicEvaluation
                // should ideally only be called when simplification *might* be needed.
                // However, we can delegate for robustness.
                try
                {
                    // Use ConcreteEvaluation for concrete unary operations
                    return opcode switch
                    {
                        OpCode.INC => _concreteEval.Increment(operand),
                        OpCode.DEC => _concreteEval.Decrement(operand),
                        OpCode.NEGATE => _concreteEval.Negate(operand),
                        OpCode.ABS => _concreteEval.Abs(operand),
                        OpCode.SIGN => _concreteEval.Sign(operand),
                        OpCode.NOT => _concreteEval.Not(operand),
                        OpCode.NZ => _concreteEval.IsNonZero(operand),
                        // Convert concrete operand to expression if no specific concrete eval exists for this opcode
                        // Use the Unary Operation constructor from SymbolicExpression.cs
                        _ => new SymbolicExpression(MapOpCodeToOperator(opcode), operand)
                    };
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is DivideByZeroException || ex is ArgumentOutOfRangeException)
                {
                    // If concrete evaluation fails (e.g., type mismatch), return the symbolic expression.
                    // Use the Unary Operation constructor from SymbolicExpression.cs
                    return new SymbolicExpression(MapOpCodeToOperator(opcode), operand);
                }
            }

            // If operand is symbolic, create the expression
            // Apply algebraic simplifications for unary operations
            var op = MapOpCodeToOperator(opcode);

            // NOT(NOT(x)) = x
            if (op == Operator.Not &&
                operand is SymbolicExpression notExpr &&
                notExpr.Operator == Operator.Not)
            {
                return notExpr.Left; // Return the inner operand
            }

            // Negate(Negate(x)) = x
            if (op == Operator.Negate &&
                operand is SymbolicExpression negateExpr &&
                negateExpr.Operator == Operator.Negate)
            {
                return negateExpr.Left; // Return the inner operand
            }

            // Create the expression if no simplification applies
            return new SymbolicExpression(op, operand);
        }

        private SymbolicValue Simplify(OpCode opcode, SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                // If both operands are concrete, delegate to ConcreteEvaluation.
                try
                {
                    return opcode switch
                    {
                        OpCode.ADD => _concreteEval.Add(left, right),
                        OpCode.SUB => _concreteEval.Subtract(left, right),
                        OpCode.MUL => _concreteEval.Multiply(left, right),
                        OpCode.DIV => _concreteEval.Divide(left, right),
                        OpCode.MOD => _concreteEval.Modulo(left, right),
                        OpCode.SHL => _concreteEval.ShiftLeft(left, right),
                        OpCode.SHR => _concreteEval.ShiftRight(left, right),
                        OpCode.BOOLAND => _concreteEval.BoolAnd(left, right),
                        OpCode.BOOLOR => _concreteEval.BoolOr(left, right),
                        OpCode.NUMEQUAL => _concreteEval.NumericEquals(left, right),
                        OpCode.NUMNOTEQUAL => _concreteEval.NumericNotEquals(left, right),
                        OpCode.LT => _concreteEval.LessThan(left, right),
                        OpCode.LE => _concreteEval.LessThanOrEqual(left, right),
                        OpCode.GT => _concreteEval.GreaterThan(left, right),
                        OpCode.GE => _concreteEval.GreaterThanOrEqual(left, right),
                        OpCode.MIN => _concreteEval.Min(left, right),
                        OpCode.MAX => _concreteEval.Max(left, right),
                        // Convert concrete operands to expressions if no specific concrete eval exists
                        // Use the Binary Operation constructor from SymbolicExpression.cs
                        _ => new SymbolicExpression(left, MapOpCodeToOperator(opcode), right)
                    };
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is DivideByZeroException || ex is ArgumentOutOfRangeException)
                {
                    // If concrete evaluation fails, return the symbolic expression.
                    // Use the Binary Operation constructor from SymbolicExpression.cs
                    return new SymbolicExpression(left, MapOpCodeToOperator(opcode), right);
                }
            }

            // At least one operand is symbolic, convert operands and create expression
            // Use the Binary Operation constructor from SymbolicExpression.cs
            var expression = new SymbolicExpression(left, MapOpCodeToOperator(opcode), right);

            // Simplify x + 0 = x or 0 + x = x
            if (opcode == OpCode.ADD)
            {
                if (IsConstantZero(right)) return left; // Return the original SymbolicValue left
                if (IsConstantZero(left)) return right; // Return the original SymbolicValue right
            }
            // Simplify x - 0 = x
            else if (opcode == OpCode.SUB && IsConstantZero(right))
            {
                return left; // Return the original SymbolicValue left
            }
            // Simplify x * 1 = x or 1 * x = x
            else if (opcode == OpCode.MUL)
            {
                if (IsConstantOne(right)) return left; // Return the original SymbolicValue left
                if (IsConstantOne(left)) return right; // Return the original SymbolicValue right
                                                       // Simplify x * 0 = 0 or 0 * x = 0
                if (IsConstantZero(right)) return right; // Return the concrete zero SymbolicValue
                if (IsConstantZero(left)) return left;   // Return the concrete zero SymbolicValue
            }
            // Simplify x / 1 = x
            else if (opcode == OpCode.DIV && IsConstantOne(right))
            {
                return left; // Return the original SymbolicValue left
            }
            // Boolean identities
            else if (opcode == OpCode.BOOLAND)
            {
                // x && true = x
                if (IsConstantTrue(right)) return left;
                if (IsConstantTrue(left)) return right;
                // x && false = false
                if (IsConstantFalse(right)) return right;
                if (IsConstantFalse(left)) return left;
            }
            // Boolean OR identities
            else if (opcode == OpCode.BOOLOR)
            {
                // x || true = true
                if (IsConstantTrue(right)) return right;
                if (IsConstantTrue(left)) return left;
                // x || false = x
                if (IsConstantFalse(right)) return left;
                if (IsConstantFalse(left)) return right;
            }
            // Subtraction identities
            else if (opcode == OpCode.SUB)
            {
                // x - x = 0
                if (left.Equals(right))
                {
                    return new ConcreteValue<BigInteger>(BigInteger.Zero);
                }
            }

            return expression; // Return the SymbolicExpression
        }

        // --- Ternary Operations ---
        //public SymbolicValue Simplify(OpCode opcode, SymbolicValue value, SymbolicValue min_bound, SymbolicValue max_bound)
        //{
        //    // If all operands are concrete, evaluate directly
        //    if (value.IsConcrete && min_bound.IsConcrete && max_bound.IsConcrete)
        //    {
        //        try
        //        {
        //            return opcode switch
        //            {
        //                OpCode.WITHIN => _concreteEval.Within(value, min_bound, max_bound),
        //                // Use function call expression for unknown concrete ternary op
        //                // Use the Function Call constructor from SymbolicExpression.cs
        //                _ => new SymbolicExpression(opcode.ToString(), // Use opcode name as func name
        //                                           value,
        //                                           min_bound,
        //                                           max_bound)
        //            };
        //        }
        //         catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentOutOfRangeException)
        //         {
        //              // If concrete evaluation fails, return the symbolic expression.
        //              // Use the Function Call constructor from SymbolicExpression.cs
        //              return new SymbolicExpression(opcode.ToString(),
        //                                           value,
        //                                           min_bound,
        //                                           max_bound);
        //         }
        //    }
        //    // If any operand is symbolic, use function call expression
        //    // TODO: Revisit this representation. WITHIN is a boolean result.
        //    // Maybe create a composite expression: AND(GE(value, min), LT(value, max))
        //    // Use the Function Call constructor from SymbolicExpression.cs - THIS IS LIKELY WRONG FOR WITHIN
        //    return new SymbolicExpression(opcode.ToString(),
        //                                value,
        //                                min_bound,
        //                                max_bound);
        //    // TODO: Add algebraic simplifications for ternary ops if applicable
        //}

        // --- Helpers for Constant Checking ---
        private bool IsConstantZero(SymbolicValue value)
        {
            return value is ConcreteValue<BigInteger> c && c.Value == BigInteger.Zero;
        }

        private bool IsConstantOne(SymbolicValue value)
        {
            return value is ConcreteValue<BigInteger> c && c.Value == BigInteger.One;
        }

        private bool IsConstantTrue(SymbolicValue value)
        {
            return value is ConcreteValue<bool> c && c.Value == true;
        }

        private bool IsConstantFalse(SymbolicValue value)
        {
            return value is ConcreteValue<bool> c && c.Value == false;
        }

        // --- IEvaluationService Implementation ---

        // Unary Operations
        public SymbolicValue Increment(SymbolicValue value) => Simplify(OpCode.INC, value);
        public SymbolicValue Decrement(SymbolicValue value) => Simplify(OpCode.DEC, value);
        public SymbolicValue Negate(SymbolicValue value) => Simplify(OpCode.NEGATE, value);
        public SymbolicValue Abs(SymbolicValue value) => Simplify(OpCode.ABS, value);
        public SymbolicValue Sign(SymbolicValue value) => Simplify(OpCode.SIGN, value);
        public SymbolicValue Not(SymbolicValue value) => Simplify(OpCode.NOT, value);
        public SymbolicValue IsNonZero(SymbolicValue value) => Simplify(OpCode.NZ, value);

        // Binary Operations
        public SymbolicValue Add(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.ADD, left, right);
        public SymbolicValue Subtract(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.SUB, left, right);
        public SymbolicValue Multiply(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.MUL, left, right);
        public SymbolicValue Divide(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.DIV, left, right);
        public SymbolicValue Modulo(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.MOD, left, right);
        public SymbolicValue ShiftLeft(SymbolicValue value, SymbolicValue shiftAmount) => Simplify(OpCode.SHL, value, shiftAmount);
        public SymbolicValue ShiftRight(SymbolicValue value, SymbolicValue shiftAmount) => Simplify(OpCode.SHR, value, shiftAmount);
        public SymbolicValue BoolAnd(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.BOOLAND, left, right);
        public SymbolicValue BoolOr(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.BOOLOR, left, right);
        public SymbolicValue Equal(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.EQUAL, left, right);
        public SymbolicValue NotEqual(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.NOTEQUAL, left, right);
        public SymbolicValue NumericEquals(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.NUMEQUAL, left, right);
        public SymbolicValue NumericNotEquals(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.NUMNOTEQUAL, left, right);
        public SymbolicValue LessThan(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.LT, left, right);
        public SymbolicValue LessThanOrEqual(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.LE, left, right);
        public SymbolicValue GreaterThan(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.GT, left, right);
        public SymbolicValue GreaterThanOrEqual(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.GE, left, right);
        public SymbolicValue Min(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.MIN, left, right);
        public SymbolicValue Max(SymbolicValue left, SymbolicValue right) => Simplify(OpCode.MAX, left, right);

        // Ternary Operations
        public SymbolicValue Within(SymbolicValue value, SymbolicValue min_bound, SymbolicValue max_bound)
            => throw new NotImplementedException("Within operation is not implemented.");

        // --- Removed Old Methods ---
        // EvaluateArithmetic, EvaluateComparison, EvaluateUnary, EvaluateLogical
        // ConvertToSymbolicExpression, TrySimplify (replaced by new Simplify overloads)
        // IsArithmeticOperation, IsComparisonOperation, IsZero, IsOne (replaced by inline checks or new helpers)
    }
}
