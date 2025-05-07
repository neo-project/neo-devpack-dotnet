using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM.Types;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Provides symbolic evaluation of operations.
    /// </summary>
    public class SymbolicEvaluation : IEvaluationService
    {
        // --- Unary Operations ---
        public SymbolicValue Increment(SymbolicValue value)
        {
            if (value.IsConcrete)
            {
                if (value is ConcreteValue<int> intValue)
                {
                    return new ConcreteValue<int>(intValue.Value + 1);
                }
                else if (value is ConcreteValue<BigInteger> bigIntValue)
                {
                    return new ConcreteValue<BigInteger>(bigIntValue.Value + 1);
                }
            }

            return new SymbolicVariable($"Inc_{value}", SymbolicType.Integer);
        }

        public SymbolicValue Decrement(SymbolicValue value)
        {
            if (value.IsConcrete)
            {
                if (value is ConcreteValue<int> intValue)
                {
                    return new ConcreteValue<int>(intValue.Value - 1);
                }
                else if (value is ConcreteValue<BigInteger> bigIntValue)
                {
                    return new ConcreteValue<BigInteger>(bigIntValue.Value - 1);
                }
            }

            return new SymbolicVariable($"Dec_{value}", SymbolicType.Integer);
        }

        public SymbolicValue Negate(SymbolicValue value)
        {
            if (value.IsConcrete)
            {
                if (value is ConcreteValue<int> intValue)
                {
                    return new ConcreteValue<int>(-intValue.Value);
                }
                else if (value is ConcreteValue<BigInteger> bigIntValue)
                {
                    return new ConcreteValue<BigInteger>(-bigIntValue.Value);
                }
            }

            return new SymbolicVariable($"Neg_{value}", SymbolicType.Integer);
        }

        public SymbolicValue Abs(SymbolicValue value)
        {
            if (value.IsConcrete)
            {
                if (value is ConcreteValue<int> intValue)
                {
                    return new ConcreteValue<int>(Math.Abs(intValue.Value));
                }
                else if (value is ConcreteValue<BigInteger> bigIntValue)
                {
                    return new ConcreteValue<BigInteger>(BigInteger.Abs(bigIntValue.Value));
                }
            }

            return new SymbolicVariable($"Abs_{value}", SymbolicType.Integer);
        }

        public SymbolicValue Sign(SymbolicValue value)
        {
            if (value.IsConcrete)
            {
                if (value is ConcreteValue<int> intValue)
                {
                    return new ConcreteValue<int>(Math.Sign(intValue.Value));
                }
                else if (value is ConcreteValue<BigInteger> bigIntValue)
                {
                    return new ConcreteValue<int>(bigIntValue.Value.Sign);
                }
            }

            return new SymbolicVariable($"Sign_{value}", SymbolicType.Integer);
        }

        public SymbolicValue Not(SymbolicValue value)
        {
            if (value.IsConcrete)
            {
                if (value is ConcreteValue<int> intValue)
                {
                    return new ConcreteValue<int>(~intValue.Value);
                }
                else if (value is ConcreteValue<BigInteger> bigIntValue)
                {
                    return new ConcreteValue<BigInteger>(~bigIntValue.Value);
                }
            }

            return new SymbolicVariable($"Not_{value}", SymbolicType.Integer);
        }

        public SymbolicValue IsNonZero(SymbolicValue value)
        {
            if (value.IsConcrete)
            {
                if (value is ConcreteValue<int> intValue)
                {
                    return new ConcreteValue<bool>(intValue.Value != 0);
                }
                else if (value is ConcreteValue<BigInteger> bigIntValue)
                {
                    return new ConcreteValue<bool>(bigIntValue.Value != 0);
                }
                else if (value is ConcreteValue<bool> boolValue)
                {
                    return new ConcreteValue<bool>(boolValue.Value);
                }
            }

            return new SymbolicVariable($"IsNonZero_{value}", SymbolicType.Boolean);
        }

        // --- Binary Operations ---
        public SymbolicValue Add(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    return new ConcreteValue<int>(leftInt.Value + rightInt.Value);
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    return new ConcreteValue<BigInteger>(leftBigInt.Value + rightBigInt.Value);
                }
            }

            return new SymbolicVariable($"Add_{left}_{right}", SymbolicType.Integer);
        }

        public SymbolicValue Subtract(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    return new ConcreteValue<int>(leftInt.Value - rightInt.Value);
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    return new ConcreteValue<BigInteger>(leftBigInt.Value - rightBigInt.Value);
                }
            }

            return new SymbolicVariable($"Sub_{left}_{right}", SymbolicType.Integer);
        }

        public SymbolicValue Multiply(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    return new ConcreteValue<int>(leftInt.Value * rightInt.Value);
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    return new ConcreteValue<BigInteger>(leftBigInt.Value * rightBigInt.Value);
                }
            }

            return new SymbolicVariable($"Mul_{left}_{right}", SymbolicType.Integer);
        }

        public SymbolicValue Divide(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    if (rightInt.Value == 0)
                    {
                        throw new DivideByZeroException();
                    }
                    return new ConcreteValue<int>(leftInt.Value / rightInt.Value);
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    if (rightBigInt.Value == 0)
                    {
                        throw new DivideByZeroException();
                    }
                    return new ConcreteValue<BigInteger>(leftBigInt.Value / rightBigInt.Value);
                }
            }

            return new SymbolicVariable($"Div_{left}_{right}", SymbolicType.Integer);
        }

        public SymbolicValue Modulo(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    if (rightInt.Value == 0)
                    {
                        throw new DivideByZeroException();
                    }
                    return new ConcreteValue<int>(leftInt.Value % rightInt.Value);
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    if (rightBigInt.Value == 0)
                    {
                        throw new DivideByZeroException();
                    }
                    return new ConcreteValue<BigInteger>(leftBigInt.Value % rightBigInt.Value);
                }
            }

            return new SymbolicVariable($"Mod_{left}_{right}", SymbolicType.Integer);
        }

        public SymbolicValue ShiftLeft(SymbolicValue value, SymbolicValue shiftAmount)
        {
            if (value.IsConcrete && shiftAmount.IsConcrete)
            {
                if (value is ConcreteValue<int> intValue && shiftAmount is ConcreteValue<int> intShift)
                {
                    return new ConcreteValue<int>(intValue.Value << intShift.Value);
                }
                else if (value is ConcreteValue<BigInteger> bigIntValue && shiftAmount is ConcreteValue<int> intShift2)
                {
                    return new ConcreteValue<BigInteger>(bigIntValue.Value << intShift2.Value);
                }
            }

            return new SymbolicVariable($"ShiftLeft_{value}_{shiftAmount}", SymbolicType.Integer);
        }

        public SymbolicValue ShiftRight(SymbolicValue value, SymbolicValue shiftAmount)
        {
            if (value.IsConcrete && shiftAmount.IsConcrete)
            {
                if (value is ConcreteValue<int> intValue && shiftAmount is ConcreteValue<int> intShift)
                {
                    return new ConcreteValue<int>(intValue.Value >> intShift.Value);
                }
                else if (value is ConcreteValue<BigInteger> bigIntValue && shiftAmount is ConcreteValue<int> intShift2)
                {
                    return new ConcreteValue<BigInteger>(bigIntValue.Value >> intShift2.Value);
                }
            }

            return new SymbolicVariable($"ShiftRight_{value}_{shiftAmount}", SymbolicType.Integer);
        }

        public SymbolicValue BoolAnd(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<bool> leftBool && right is ConcreteValue<bool> rightBool)
                {
                    return new ConcreteValue<bool>(leftBool.Value && rightBool.Value);
                }
            }

            return new SymbolicVariable($"BoolAnd_{left}_{right}", SymbolicType.Boolean);
        }

        public SymbolicValue BoolOr(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<bool> leftBool && right is ConcreteValue<bool> rightBool)
                {
                    return new ConcreteValue<bool>(leftBool.Value || rightBool.Value);
                }
            }

            return new SymbolicVariable($"BoolOr_{left}_{right}", SymbolicType.Boolean);
        }

        public SymbolicValue Equal(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    return new ConcreteValue<bool>(leftInt.Value == rightInt.Value);
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    return new ConcreteValue<bool>(leftBigInt.Value == rightBigInt.Value);
                }
                else if (left is ConcreteValue<bool> leftBool && right is ConcreteValue<bool> rightBool)
                {
                    return new ConcreteValue<bool>(leftBool.Value == rightBool.Value);
                }
            }

            return new SymbolicVariable($"Equal_{left}_{right}", SymbolicType.Boolean);
        }

        public SymbolicValue NotEqual(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    return new ConcreteValue<bool>(leftInt.Value != rightInt.Value);
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    return new ConcreteValue<bool>(leftBigInt.Value != rightBigInt.Value);
                }
                else if (left is ConcreteValue<bool> leftBool && right is ConcreteValue<bool> rightBool)
                {
                    return new ConcreteValue<bool>(leftBool.Value != rightBool.Value);
                }
            }

            return new SymbolicVariable($"NotEqual_{left}_{right}", SymbolicType.Boolean);
        }

        public SymbolicValue NumericEquals(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    return new ConcreteValue<bool>(leftInt.Value == rightInt.Value);
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    return new ConcreteValue<bool>(leftBigInt.Value == rightBigInt.Value);
                }
            }

            return new SymbolicVariable($"NumericEquals_{left}_{right}", SymbolicType.Boolean);
        }

        public SymbolicValue NumericNotEquals(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    return new ConcreteValue<bool>(leftInt.Value != rightInt.Value);
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    return new ConcreteValue<bool>(leftBigInt.Value != rightBigInt.Value);
                }
            }

            return new SymbolicVariable($"NumericNotEquals_{left}_{right}", SymbolicType.Boolean);
        }

        public SymbolicValue LessThan(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    return new ConcreteValue<bool>(leftInt.Value < rightInt.Value);
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    return new ConcreteValue<bool>(leftBigInt.Value < rightBigInt.Value);
                }
            }

            return new SymbolicVariable($"LessThan_{left}_{right}", SymbolicType.Boolean);
        }

        public SymbolicValue LessThanOrEqual(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    return new ConcreteValue<bool>(leftInt.Value <= rightInt.Value);
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    return new ConcreteValue<bool>(leftBigInt.Value <= rightBigInt.Value);
                }
            }

            return new SymbolicVariable($"LessThanOrEqual_{left}_{right}", SymbolicType.Boolean);
        }

        public SymbolicValue GreaterThan(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    return new ConcreteValue<bool>(leftInt.Value > rightInt.Value);
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    return new ConcreteValue<bool>(leftBigInt.Value > rightBigInt.Value);
                }
            }

            return new SymbolicVariable($"GreaterThan_{left}_{right}", SymbolicType.Boolean);
        }

        public SymbolicValue GreaterThanOrEqual(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    return new ConcreteValue<bool>(leftInt.Value >= rightInt.Value);
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    return new ConcreteValue<bool>(leftBigInt.Value >= rightBigInt.Value);
                }
            }

            return new SymbolicVariable($"GreaterThanOrEqual_{left}_{right}", SymbolicType.Boolean);
        }

        public SymbolicValue Min(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    return new ConcreteValue<int>(Math.Min(leftInt.Value, rightInt.Value));
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    return new ConcreteValue<BigInteger>(BigInteger.Min(leftBigInt.Value, rightBigInt.Value));
                }
            }

            return new SymbolicVariable($"Min_{left}_{right}", SymbolicType.Integer);
        }

        public SymbolicValue Max(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete)
            {
                if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
                {
                    return new ConcreteValue<int>(Math.Max(leftInt.Value, rightInt.Value));
                }
                else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
                {
                    return new ConcreteValue<BigInteger>(BigInteger.Max(leftBigInt.Value, rightBigInt.Value));
                }
            }

            return new SymbolicVariable($"Max_{left}_{right}", SymbolicType.Integer);
        }

        // --- Ternary Operations ---
        public SymbolicValue Within(SymbolicValue value, SymbolicValue min_bound, SymbolicValue max_bound)
        {
            if (value.IsConcrete && min_bound.IsConcrete && max_bound.IsConcrete)
            {
                if (value is ConcreteValue<int> intValue &&
                    min_bound is ConcreteValue<int> intMin &&
                    max_bound is ConcreteValue<int> intMax)
                {
                    return new ConcreteValue<bool>(intValue.Value >= intMin.Value && intValue.Value < intMax.Value);
                }
                else if (value is ConcreteValue<BigInteger> bigIntValue &&
                         min_bound is ConcreteValue<BigInteger> bigIntMin &&
                         max_bound is ConcreteValue<BigInteger> bigIntMax)
                {
                    return new ConcreteValue<bool>(bigIntValue.Value >= bigIntMin.Value && bigIntValue.Value < bigIntMax.Value);
                }
            }

            return new SymbolicVariable($"Within_{value}_{min_bound}_{max_bound}", SymbolicType.Boolean);
        }

        /// <summary>
        /// Evaluates a comparison operation on symbolic values.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <param name="operation">The operation to perform.</param>
        /// <returns>The result of the comparison.</returns>
        public SymbolicValue EvaluateComparison(SymbolicValue left, SymbolicValue right, string operation)
        {
            // Create a symbolic expression representing the operation
            Operator op = GetOperatorFromString(operation);
            return new SymbolicExpression(left, op, right);
        }

        /// <summary>
        /// Converts a string operation to an Operator enum value.
        /// </summary>
        /// <param name="operation">The operation string.</param>
        /// <returns>The corresponding Operator enum value.</returns>
        private Operator GetOperatorFromString(string operation)
        {
            switch (operation)
            {
                case "==": return Operator.Equal;
                case "!=": return Operator.NotEqual;
                case ">": return Operator.GreaterThan;
                case "<": return Operator.LessThan;
                case ">=": return Operator.GreaterThanOrEqual;
                case "<=": return Operator.LessThanOrEqual;
                default: return Operator.Equal; // Default case
            }
        }
    }
}
