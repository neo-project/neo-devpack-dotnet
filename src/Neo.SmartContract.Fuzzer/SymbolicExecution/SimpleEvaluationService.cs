using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Simple implementation of the evaluation service.
    /// </summary>
    public class SimpleEvaluationService : IEvaluationService
    {
        // --- Unary Operations ---
        public SymbolicValue Increment(SymbolicValue value)
        {
            if (value.IsConcrete && value.TryGetInteger(out var intValue))
            {
                return new SymbolicValue<BigInteger>(intValue + BigInteger.One);
            }
            return new SymbolicExpression(value, Operator.Add, new SymbolicValue<BigInteger>(BigInteger.One));
        }

        public SymbolicValue Decrement(SymbolicValue value)
        {
            if (value.IsConcrete && value.TryGetInteger(out var intValue))
            {
                return new SymbolicValue<BigInteger>(intValue - BigInteger.One);
            }
            return new SymbolicExpression(value, Operator.Subtract, new SymbolicValue<BigInteger>(BigInteger.One));
        }

        public SymbolicValue Negate(SymbolicValue value)
        {
            if (value.IsConcrete && value.TryGetInteger(out var intValue))
            {
                return new SymbolicValue<BigInteger>(-intValue);
            }
            return new SymbolicExpression(value, Operator.Negate, null);
        }

        public SymbolicValue Abs(SymbolicValue value)
        {
            if (value.IsConcrete && value.TryGetInteger(out var intValue))
            {
                return new SymbolicValue<BigInteger>(BigInteger.Abs(intValue));
            }
            return new SymbolicExpression(value, Operator.Abs, null);
        }

        public SymbolicValue Sign(SymbolicValue value)
        {
            if (value.IsConcrete && value.TryGetInteger(out var intValue))
            {
                return new SymbolicValue<BigInteger>(intValue.Sign);
            }
            return new SymbolicExpression(value, Operator.Sign, null);
        }

        public SymbolicValue Not(SymbolicValue value)
        {
            if (value.IsConcrete && value.TryGetBoolean(out var boolValue))
            {
                return new SymbolicValue<bool>(!boolValue);
            }
            return new SymbolicExpression(value, Operator.Not, null);
        }

        public SymbolicValue IsNonZero(SymbolicValue value)
        {
            if (value.IsConcrete && value.TryGetInteger(out var intValue))
            {
                return new SymbolicValue<bool>(intValue != BigInteger.Zero);
            }
            return new SymbolicExpression(value, Operator.NotEqual, new SymbolicValue<BigInteger>(BigInteger.Zero));
        }

        // --- Binary Operations ---
        public SymbolicValue Add(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetInteger(out var leftInt) && right.TryGetInteger(out var rightInt))
            {
                return new SymbolicValue<BigInteger>(leftInt + rightInt);
            }
            return new SymbolicExpression(left, Operator.Add, right);
        }

        public SymbolicValue Subtract(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetInteger(out var leftInt) && right.TryGetInteger(out var rightInt))
            {
                return new SymbolicValue<BigInteger>(leftInt - rightInt);
            }
            return new SymbolicExpression(left, Operator.Subtract, right);
        }

        public SymbolicValue Multiply(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetInteger(out var leftInt) && right.TryGetInteger(out var rightInt))
            {
                return new SymbolicValue<BigInteger>(leftInt * rightInt);
            }
            return new SymbolicExpression(left, Operator.Multiply, right);
        }

        public SymbolicValue Divide(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetInteger(out var leftInt) && right.TryGetInteger(out var rightInt))
            {
                if (rightInt == BigInteger.Zero)
                    throw new DivideByZeroException();
                return new SymbolicValue<BigInteger>(leftInt / rightInt);
            }
            return new SymbolicExpression(left, Operator.Divide, right);
        }

        public SymbolicValue Modulo(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetInteger(out var leftInt) && right.TryGetInteger(out var rightInt))
            {
                if (rightInt == BigInteger.Zero)
                    throw new DivideByZeroException();
                return new SymbolicValue<BigInteger>(leftInt % rightInt);
            }
            return new SymbolicExpression(left, Operator.Modulo, right);
        }

        public SymbolicValue ShiftLeft(SymbolicValue value, SymbolicValue shiftAmount)
        {
            if (value.IsConcrete && shiftAmount.IsConcrete &&
                value.TryGetInteger(out var valueInt) && shiftAmount.TryGetInteger(out var shiftInt))
            {
                return new SymbolicValue<BigInteger>(valueInt << (int)shiftInt);
            }
            return new SymbolicExpression(value, Operator.LeftShift, shiftAmount);
        }

        public SymbolicValue ShiftRight(SymbolicValue value, SymbolicValue shiftAmount)
        {
            if (value.IsConcrete && shiftAmount.IsConcrete &&
                value.TryGetInteger(out var valueInt) && shiftAmount.TryGetInteger(out var shiftInt))
            {
                return new SymbolicValue<BigInteger>(valueInt >> (int)shiftInt);
            }
            return new SymbolicExpression(value, Operator.RightShift, shiftAmount);
        }

        public SymbolicValue BoolAnd(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetBoolean(out var leftBool) && right.TryGetBoolean(out var rightBool))
            {
                return new SymbolicValue<bool>(leftBool && rightBool);
            }
            return new SymbolicExpression(left, Operator.And, right);
        }

        public SymbolicValue BoolOr(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetBoolean(out var leftBool) && right.TryGetBoolean(out var rightBool))
            {
                return new SymbolicValue<bool>(leftBool || rightBool);
            }
            return new SymbolicExpression(left, Operator.Or, right);
        }

        public SymbolicValue Equal(SymbolicValue left, SymbolicValue right)
        {
            // Handle different types of equality based on the types of the operands
            if (left.IsConcrete && right.IsConcrete)
            {
                // Try integer comparison
                if (left.TryGetInteger(out var leftInt) && right.TryGetInteger(out var rightInt))
                {
                    return new SymbolicValue<bool>(leftInt == rightInt);
                }

                // Try boolean comparison
                if (left.TryGetBoolean(out var leftBool) && right.TryGetBoolean(out var rightBool))
                {
                    return new SymbolicValue<bool>(leftBool == rightBool);
                }

                // Try byte array comparison
                if (left.TryGetBytes(out var leftBytes) && right.TryGetBytes(out var rightBytes))
                {
                    if (leftBytes.Length != rightBytes.Length)
                        return new SymbolicValue<bool>(false);

                    for (int i = 0; i < leftBytes.Length; i++)
                    {
                        if (leftBytes[i] != rightBytes[i])
                            return new SymbolicValue<bool>(false);
                    }

                    return new SymbolicValue<bool>(true);
                }

                // If types don't match, they're not equal
                if (left.Type != right.Type)
                    return new SymbolicValue<bool>(false);

                // Default case - compare string representations
                return new SymbolicValue<bool>(left.ToString() == right.ToString());
            }

            return new SymbolicExpression(left, Operator.Equal, right);
        }

        public SymbolicValue NotEqual(SymbolicValue left, SymbolicValue right)
        {
            // Reuse Equal and negate the result if concrete
            var equalResult = Equal(left, right);

            if (equalResult.IsConcrete && equalResult.TryGetBoolean(out var boolResult))
            {
                return new SymbolicValue<bool>(!boolResult);
            }

            return new SymbolicExpression(left, Operator.NotEqual, right);
        }

        public SymbolicValue NumericEquals(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetInteger(out var leftInt) && right.TryGetInteger(out var rightInt))
            {
                return new SymbolicValue<bool>(leftInt == rightInt);
            }
            return new SymbolicExpression(left, Operator.Equal, right);
        }

        public SymbolicValue NumericNotEquals(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetInteger(out var leftInt) && right.TryGetInteger(out var rightInt))
            {
                return new SymbolicValue<bool>(leftInt != rightInt);
            }
            return new SymbolicExpression(left, Operator.NotEqual, right);
        }

        public SymbolicValue LessThan(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetInteger(out var leftInt) && right.TryGetInteger(out var rightInt))
            {
                return new SymbolicValue<bool>(leftInt < rightInt);
            }
            return new SymbolicExpression(left, Operator.LessThan, right);
        }

        public SymbolicValue LessThanOrEqual(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetInteger(out var leftInt) && right.TryGetInteger(out var rightInt))
            {
                return new SymbolicValue<bool>(leftInt <= rightInt);
            }
            return new SymbolicExpression(left, Operator.LessThanOrEqual, right);
        }

        public SymbolicValue GreaterThan(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetInteger(out var leftInt) && right.TryGetInteger(out var rightInt))
            {
                return new SymbolicValue<bool>(leftInt > rightInt);
            }
            return new SymbolicExpression(left, Operator.GreaterThan, right);
        }

        public SymbolicValue GreaterThanOrEqual(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetInteger(out var leftInt) && right.TryGetInteger(out var rightInt))
            {
                return new SymbolicValue<bool>(leftInt >= rightInt);
            }
            return new SymbolicExpression(left, Operator.GreaterThanOrEqual, right);
        }

        public SymbolicValue Min(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetInteger(out var leftInt) && right.TryGetInteger(out var rightInt))
            {
                return new SymbolicValue<BigInteger>(BigInteger.Min(leftInt, rightInt));
            }
            return new SymbolicExpression(left, Operator.Min, right);
        }

        public SymbolicValue Max(SymbolicValue left, SymbolicValue right)
        {
            if (left.IsConcrete && right.IsConcrete &&
                left.TryGetInteger(out var leftInt) && right.TryGetInteger(out var rightInt))
            {
                return new SymbolicValue<BigInteger>(BigInteger.Max(leftInt, rightInt));
            }
            return new SymbolicExpression(left, Operator.Max, right);
        }

        // --- Ternary Operations ---
        public SymbolicValue Within(SymbolicValue value, SymbolicValue min_bound, SymbolicValue max_bound)
        {
            if (value.IsConcrete && min_bound.IsConcrete && max_bound.IsConcrete &&
                value.TryGetInteger(out var valueInt) &&
                min_bound.TryGetInteger(out var minInt) &&
                max_bound.TryGetInteger(out var maxInt))
            {
                return new SymbolicValue<bool>(valueInt >= minInt && valueInt < maxInt);
            }

            // Create a compound expression for x >= min && x < max
            var greaterThanOrEqual = new SymbolicExpression(value, Operator.GreaterThanOrEqual, min_bound);
            var lessThan = new SymbolicExpression(value, Operator.LessThan, max_bound);
            return new SymbolicExpression(greaterThanOrEqual, Operator.And, lessThan);
        }
    }
}
