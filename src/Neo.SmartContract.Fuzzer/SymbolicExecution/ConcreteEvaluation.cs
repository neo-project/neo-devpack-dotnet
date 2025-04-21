using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM.Types;
using System;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Provides concrete evaluation of operations.
    /// </summary>
    public class ConcreteEvaluation : IEvaluationService
    {
        // --- Unary Operations ---
        public SymbolicValue Increment(SymbolicValue value)
        {
            if (value is ConcreteValue<int> intValue)
            {
                return new ConcreteValue<int>(intValue.Value + 1);
            }
            else if (value is ConcreteValue<BigInteger> bigIntValue)
            {
                return new ConcreteValue<BigInteger>(bigIntValue.Value + 1);
            }
            else
            {
                throw new NotSupportedException($"Cannot increment value of type {value.GetType().Name}");
            }
        }

        public SymbolicValue Decrement(SymbolicValue value)
        {
            if (value is ConcreteValue<int> intValue)
            {
                return new ConcreteValue<int>(intValue.Value - 1);
            }
            else if (value is ConcreteValue<BigInteger> bigIntValue)
            {
                return new ConcreteValue<BigInteger>(bigIntValue.Value - 1);
            }
            else
            {
                throw new NotSupportedException($"Cannot decrement value of type {value.GetType().Name}");
            }
        }

        public SymbolicValue Negate(SymbolicValue value)
        {
            if (value is ConcreteValue<int> intValue)
            {
                return new ConcreteValue<int>(-intValue.Value);
            }
            else if (value is ConcreteValue<BigInteger> bigIntValue)
            {
                return new ConcreteValue<BigInteger>(-bigIntValue.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot negate value of type {value.GetType().Name}");
            }
        }

        public SymbolicValue Abs(SymbolicValue value)
        {
            if (value is ConcreteValue<int> intValue)
            {
                return new ConcreteValue<int>(Math.Abs(intValue.Value));
            }
            else if (value is ConcreteValue<BigInteger> bigIntValue)
            {
                return new ConcreteValue<BigInteger>(BigInteger.Abs(bigIntValue.Value));
            }
            else
            {
                throw new NotSupportedException($"Cannot compute absolute value of type {value.GetType().Name}");
            }
        }

        public SymbolicValue Sign(SymbolicValue value)
        {
            if (value is ConcreteValue<int> intValue)
            {
                return new ConcreteValue<int>(Math.Sign(intValue.Value));
            }
            else if (value is ConcreteValue<BigInteger> bigIntValue)
            {
                return new ConcreteValue<int>(bigIntValue.Value.Sign);
            }
            else
            {
                throw new NotSupportedException($"Cannot compute sign of value of type {value.GetType().Name}");
            }
        }

        public SymbolicValue Not(SymbolicValue value)
        {
            if (value is ConcreteValue<int> intValue)
            {
                return new ConcreteValue<int>(~intValue.Value);
            }
            else if (value is ConcreteValue<BigInteger> bigIntValue)
            {
                return new ConcreteValue<BigInteger>(~bigIntValue.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot compute bitwise NOT of value of type {value.GetType().Name}");
            }
        }

        public SymbolicValue IsNonZero(SymbolicValue value)
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
            else
            {
                throw new NotSupportedException($"Cannot check if value of type {value.GetType().Name} is non-zero");
            }
        }

        // --- Binary Operations ---
        public SymbolicValue Add(SymbolicValue left, SymbolicValue right)
        {
            if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
            {
                return new ConcreteValue<int>(leftInt.Value + rightInt.Value);
            }
            else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
            {
                return new ConcreteValue<BigInteger>(leftBigInt.Value + rightBigInt.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot add values of types {left.GetType().Name} and {right.GetType().Name}");
            }
        }

        public SymbolicValue Subtract(SymbolicValue left, SymbolicValue right)
        {
            if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
            {
                return new ConcreteValue<int>(leftInt.Value - rightInt.Value);
            }
            else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
            {
                return new ConcreteValue<BigInteger>(leftBigInt.Value - rightBigInt.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot subtract values of types {left.GetType().Name} and {right.GetType().Name}");
            }
        }

        public SymbolicValue Multiply(SymbolicValue left, SymbolicValue right)
        {
            if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
            {
                return new ConcreteValue<int>(leftInt.Value * rightInt.Value);
            }
            else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
            {
                return new ConcreteValue<BigInteger>(leftBigInt.Value * rightBigInt.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot multiply values of types {left.GetType().Name} and {right.GetType().Name}");
            }
        }

        public SymbolicValue Divide(SymbolicValue left, SymbolicValue right)
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
            else
            {
                throw new NotSupportedException($"Cannot divide values of types {left.GetType().Name} and {right.GetType().Name}");
            }
        }

        public SymbolicValue Modulo(SymbolicValue left, SymbolicValue right)
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
            else
            {
                throw new NotSupportedException($"Cannot compute modulo of values of types {left.GetType().Name} and {right.GetType().Name}");
            }
        }

        public SymbolicValue ShiftLeft(SymbolicValue value, SymbolicValue shiftAmount)
        {
            if (value is ConcreteValue<int> intValue && shiftAmount is ConcreteValue<int> intShift)
            {
                return new ConcreteValue<int>(intValue.Value << intShift.Value);
            }
            else if (value is ConcreteValue<BigInteger> bigIntValue && shiftAmount is ConcreteValue<int> intShift2)
            {
                return new ConcreteValue<BigInteger>(bigIntValue.Value << intShift2.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot shift left value of type {value.GetType().Name} by amount of type {shiftAmount.GetType().Name}");
            }
        }

        public SymbolicValue ShiftRight(SymbolicValue value, SymbolicValue shiftAmount)
        {
            if (value is ConcreteValue<int> intValue && shiftAmount is ConcreteValue<int> intShift)
            {
                return new ConcreteValue<int>(intValue.Value >> intShift.Value);
            }
            else if (value is ConcreteValue<BigInteger> bigIntValue && shiftAmount is ConcreteValue<int> intShift2)
            {
                return new ConcreteValue<BigInteger>(bigIntValue.Value >> intShift2.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot shift right value of type {value.GetType().Name} by amount of type {shiftAmount.GetType().Name}");
            }
        }

        public SymbolicValue BoolAnd(SymbolicValue left, SymbolicValue right)
        {
            if (left is ConcreteValue<bool> leftBool && right is ConcreteValue<bool> rightBool)
            {
                return new ConcreteValue<bool>(leftBool.Value && rightBool.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot compute boolean AND of values of types {left.GetType().Name} and {right.GetType().Name}");
            }
        }

        public SymbolicValue BoolOr(SymbolicValue left, SymbolicValue right)
        {
            if (left is ConcreteValue<bool> leftBool && right is ConcreteValue<bool> rightBool)
            {
                return new ConcreteValue<bool>(leftBool.Value || rightBool.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot compute boolean OR of values of types {left.GetType().Name} and {right.GetType().Name}");
            }
        }

        public SymbolicValue Equal(SymbolicValue left, SymbolicValue right)
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
            else
            {
                throw new NotSupportedException($"Cannot check equality of values of types {left.GetType().Name} and {right.GetType().Name}");
            }
        }

        public SymbolicValue NotEqual(SymbolicValue left, SymbolicValue right)
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
            else
            {
                throw new NotSupportedException($"Cannot check inequality of values of types {left.GetType().Name} and {right.GetType().Name}");
            }
        }

        public SymbolicValue NumericEquals(SymbolicValue left, SymbolicValue right)
        {
            if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
            {
                return new ConcreteValue<bool>(leftInt.Value == rightInt.Value);
            }
            else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
            {
                return new ConcreteValue<bool>(leftBigInt.Value == rightBigInt.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot check numeric equality of values of types {left.GetType().Name} and {right.GetType().Name}");
            }
        }

        public SymbolicValue NumericNotEquals(SymbolicValue left, SymbolicValue right)
        {
            if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
            {
                return new ConcreteValue<bool>(leftInt.Value != rightInt.Value);
            }
            else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
            {
                return new ConcreteValue<bool>(leftBigInt.Value != rightBigInt.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot check numeric inequality of values of types {left.GetType().Name} and {right.GetType().Name}");
            }
        }

        public SymbolicValue LessThan(SymbolicValue left, SymbolicValue right)
        {
            if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
            {
                return new ConcreteValue<bool>(leftInt.Value < rightInt.Value);
            }
            else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
            {
                return new ConcreteValue<bool>(leftBigInt.Value < rightBigInt.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot check if value of type {left.GetType().Name} is less than value of type {right.GetType().Name}");
            }
        }

        public SymbolicValue LessThanOrEqual(SymbolicValue left, SymbolicValue right)
        {
            if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
            {
                return new ConcreteValue<bool>(leftInt.Value <= rightInt.Value);
            }
            else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
            {
                return new ConcreteValue<bool>(leftBigInt.Value <= rightBigInt.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot check if value of type {left.GetType().Name} is less than or equal to value of type {right.GetType().Name}");
            }
        }

        public SymbolicValue GreaterThan(SymbolicValue left, SymbolicValue right)
        {
            if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
            {
                return new ConcreteValue<bool>(leftInt.Value > rightInt.Value);
            }
            else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
            {
                return new ConcreteValue<bool>(leftBigInt.Value > rightBigInt.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot check if value of type {left.GetType().Name} is greater than value of type {right.GetType().Name}");
            }
        }

        public SymbolicValue GreaterThanOrEqual(SymbolicValue left, SymbolicValue right)
        {
            if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
            {
                return new ConcreteValue<bool>(leftInt.Value >= rightInt.Value);
            }
            else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
            {
                return new ConcreteValue<bool>(leftBigInt.Value >= rightBigInt.Value);
            }
            else
            {
                throw new NotSupportedException($"Cannot check if value of type {left.GetType().Name} is greater than or equal to value of type {right.GetType().Name}");
            }
        }

        public SymbolicValue Min(SymbolicValue left, SymbolicValue right)
        {
            if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
            {
                return new ConcreteValue<int>(Math.Min(leftInt.Value, rightInt.Value));
            }
            else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
            {
                return new ConcreteValue<BigInteger>(BigInteger.Min(leftBigInt.Value, rightBigInt.Value));
            }
            else
            {
                throw new NotSupportedException($"Cannot compute minimum of values of types {left.GetType().Name} and {right.GetType().Name}");
            }
        }

        public SymbolicValue Max(SymbolicValue left, SymbolicValue right)
        {
            if (left is ConcreteValue<int> leftInt && right is ConcreteValue<int> rightInt)
            {
                return new ConcreteValue<int>(Math.Max(leftInt.Value, rightInt.Value));
            }
            else if (left is ConcreteValue<BigInteger> leftBigInt && right is ConcreteValue<BigInteger> rightBigInt)
            {
                return new ConcreteValue<BigInteger>(BigInteger.Max(leftBigInt.Value, rightBigInt.Value));
            }
            else
            {
                throw new NotSupportedException($"Cannot compute maximum of values of types {left.GetType().Name} and {right.GetType().Name}");
            }
        }

        // --- Ternary Operations ---
        public SymbolicValue Within(SymbolicValue value, SymbolicValue min_bound, SymbolicValue max_bound)
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
            else
            {
                throw new NotSupportedException($"Cannot check if value of type {value.GetType().Name} is within bounds of types {min_bound.GetType().Name} and {max_bound.GetType().Name}");
            }
        }
    }
}
