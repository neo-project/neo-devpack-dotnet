using Neo.VM;
using System;
using System.Numerics;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types; // Assuming ConcreteValue is here

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Evaluation
{
    /// <summary>
    /// Provides evaluation services for concrete (non-symbolic) values during symbolic execution.
    /// This service expects all inputs to be concrete and will throw if symbolic inputs are provided.
    /// </summary>
    public class ConcreteEvaluation : IEvaluationService
    {
        private void EnsureConcrete(SymbolicValue value, string operation)
        {
            if (!value?.IsConcrete ?? true) // Also check for null
            {
                throw new InvalidOperationException($"Cannot perform {operation} on non-concrete type: {value?.GetType().Name}");
            }
        }

        private void EnsureConcrete(SymbolicValue left, SymbolicValue right, string operation)
        {
            if (!left?.IsConcrete ?? true)
            {
                throw new InvalidOperationException($"Left operand for {operation} must be concrete: {left?.GetType().Name}");
            }
            if (!right?.IsConcrete ?? true)
            {
                throw new InvalidOperationException($"Right operand for {operation} must be concrete: {right?.GetType().Name}");
            }
        }

        private void EnsureConcrete(SymbolicValue value, SymbolicValue min_bound, SymbolicValue max_bound, string operation)
        {
            if (!value?.IsConcrete ?? true)
            {
                throw new InvalidOperationException($"Value operand for {operation} must be concrete: {value?.GetType().Name}");
            }
            if (!min_bound?.IsConcrete ?? true)
            {
                throw new InvalidOperationException($"Min bound operand for {operation} must be concrete: {min_bound?.GetType().Name}");
            }
            if (!max_bound?.IsConcrete ?? true)
            {
                throw new InvalidOperationException($"Max bound operand for {operation} must be concrete: {max_bound?.GetType().Name}");
            }
        }

        // Unary Operations
        public SymbolicValue Increment(SymbolicValue value)
        {
            EnsureConcrete(value, "Increment");
            if (value is ConcreteValue<BigInteger> concreteValue)
            {
                return new ConcreteValue<BigInteger>(concreteValue.Value + BigInteger.One);
            }
            throw new InvalidOperationException($"Increment requires concrete BigInteger, got: {value?.GetType().Name}");
        }

        public SymbolicValue Decrement(SymbolicValue value)
        {
            EnsureConcrete(value, "Decrement");
            if (value is ConcreteValue<BigInteger> concreteValue)
            {
                return new ConcreteValue<BigInteger>(concreteValue.Value - BigInteger.One);
            }
            throw new InvalidOperationException($"Decrement requires concrete BigInteger, got: {value?.GetType().Name}");
        }

        public SymbolicValue Negate(SymbolicValue value)
        {
            EnsureConcrete(value, "Negate");
            if (value is ConcreteValue<BigInteger> concreteValue)
            {
                return new ConcreteValue<BigInteger>(-concreteValue.Value);
            }
            throw new InvalidOperationException($"Negate requires concrete BigInteger, got: {value?.GetType().Name}");
        }

        public SymbolicValue Abs(SymbolicValue value)
        {
            EnsureConcrete(value, "Abs");
            if (value is ConcreteValue<BigInteger> concreteValue)
            {
                return new ConcreteValue<BigInteger>(BigInteger.Abs(concreteValue.Value));
            }
            throw new InvalidOperationException($"Abs requires concrete BigInteger, got: {value?.GetType().Name}");
        }

        public SymbolicValue Sign(SymbolicValue value)
        {
            EnsureConcrete(value, "Sign");
            if (value is ConcreteValue<BigInteger> concreteValue)
            {
                // Return type of Sign is int, wrap it in ConcreteValue<BigInteger>
                return new ConcreteValue<BigInteger>(concreteValue.Value.Sign);
            }
            throw new InvalidOperationException($"Sign requires concrete BigInteger, got: {value?.GetType().Name}");
        }

        public SymbolicValue Not(SymbolicValue value) // Boolean NOT
        {
            EnsureConcrete(value, "Not");
            if (value is ConcreteValue<bool> boolValue)
            {
                return new ConcreteValue<bool>(!boolValue.Value);
            }
            // Neo VM also allows NOT on Integer (returns true if zero, false otherwise)
            if (value is ConcreteValue<BigInteger> intValue)
            {
                return new ConcreteValue<bool>(intValue.Value == BigInteger.Zero);
            }
            throw new InvalidOperationException($"Not requires concrete Boolean or BigInteger, got: {value?.GetType().Name}");
        }

        public SymbolicValue IsNonZero(SymbolicValue value) // NZ
        {
            EnsureConcrete(value, "IsNonZero");
            bool result = value switch
            {
                ConcreteValue<BigInteger> bi => bi.Value != BigInteger.Zero,
                ConcreteValue<bool> b => b.Value,
                ConcreteValue<byte[]> ba => ba.Value.Length > 0,
                // Add cases for other concrete StackItem types if needed
                _ => value.ToStackItem() != null && !value.ToStackItem().IsNull // Use StackItem conversion logic
            };
            return new ConcreteValue<bool>(result);
        }

        // Binary Operations
        public SymbolicValue Add(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "Add");
            if (left is ConcreteValue<BigInteger> l && right is ConcreteValue<BigInteger> r)
            {
                return new ConcreteValue<BigInteger>(l.Value + r.Value);
            }
            throw new InvalidOperationException($"Add requires concrete BigIntegers, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        public SymbolicValue Subtract(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "Subtract");
            if (left is ConcreteValue<BigInteger> l && right is ConcreteValue<BigInteger> r)
            {
                return new ConcreteValue<BigInteger>(l.Value - r.Value);
            }
            throw new InvalidOperationException($"Subtract requires concrete BigIntegers, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        public SymbolicValue Multiply(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "Multiply");
            if (left is ConcreteValue<BigInteger> l && right is ConcreteValue<BigInteger> r)
            {
                return new ConcreteValue<BigInteger>(l.Value * r.Value);
            }
            throw new InvalidOperationException($"Multiply requires concrete BigIntegers, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        public SymbolicValue Divide(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "Divide");
            if (left is ConcreteValue<BigInteger> l && right is ConcreteValue<BigInteger> r)
            {
                if (r.Value == BigInteger.Zero) throw new DivideByZeroException();
                return new ConcreteValue<BigInteger>(l.Value / r.Value);
            }
            throw new InvalidOperationException($"Divide requires concrete BigIntegers, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        public SymbolicValue Modulo(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "Modulo");
            if (left is ConcreteValue<BigInteger> l && right is ConcreteValue<BigInteger> r)
            {
                if (r.Value == BigInteger.Zero) throw new DivideByZeroException(); // NeoVM behavior
                return new ConcreteValue<BigInteger>(l.Value % r.Value);
            }
            throw new InvalidOperationException($"Modulo requires concrete BigIntegers, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        public SymbolicValue ShiftLeft(SymbolicValue value, SymbolicValue shiftAmount)
        {
            EnsureConcrete(value, shiftAmount, "ShiftLeft");
            if (value is ConcreteValue<BigInteger> val && shiftAmount is ConcreteValue<BigInteger> shift)
            {
                // NeoVM uses int for shift amount, ensure it's within range
                if (shift.Value < int.MinValue || shift.Value > int.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(shiftAmount), "Shift amount out of range for int.");
                return new ConcreteValue<BigInteger>(val.Value << (int)shift.Value);
            }
            throw new InvalidOperationException($"ShiftLeft requires concrete BigIntegers, got: {value?.GetType().Name}, {shiftAmount?.GetType().Name}");
        }

        public SymbolicValue ShiftRight(SymbolicValue value, SymbolicValue shiftAmount)
        {
            EnsureConcrete(value, shiftAmount, "ShiftRight");
            if (value is ConcreteValue<BigInteger> val && shiftAmount is ConcreteValue<BigInteger> shift)
            {
                // NeoVM uses int for shift amount, ensure it's within range
                if (shift.Value < int.MinValue || shift.Value > int.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(shiftAmount), "Shift amount out of range for int.");
                return new ConcreteValue<BigInteger>(val.Value >> (int)shift.Value);
            }
            throw new InvalidOperationException($"ShiftRight requires concrete BigIntegers, got: {value?.GetType().Name}, {shiftAmount?.GetType().Name}");
        }

        public SymbolicValue BoolAnd(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "BoolAnd");
            if (left is ConcreteValue<bool> l && right is ConcreteValue<bool> r)
            {
                return new ConcreteValue<bool>(l.Value && r.Value);
            }
            // NeoVM allows integers too
            if (left is ConcreteValue<BigInteger> lInt && right is ConcreteValue<BigInteger> rInt)
            {
                return new ConcreteValue<bool>(lInt.Value != BigInteger.Zero && rInt.Value != BigInteger.Zero);
            }
            if (left is ConcreteValue<bool> lBool && right is ConcreteValue<BigInteger> rIntB)
            {
                return new ConcreteValue<bool>(lBool.Value && rIntB.Value != BigInteger.Zero);
            }
            if (left is ConcreteValue<BigInteger> lIntB && right is ConcreteValue<bool> rBool)
            {
                return new ConcreteValue<bool>(lIntB.Value != BigInteger.Zero && rBool.Value);
            }
            throw new InvalidOperationException($"BoolAnd requires concrete Boolean or BigInteger, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        public SymbolicValue BoolOr(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "BoolOr");
            if (left is ConcreteValue<bool> l && right is ConcreteValue<bool> r)
            {
                return new ConcreteValue<bool>(l.Value || r.Value);
            }
            // NeoVM allows integers too
            if (left is ConcreteValue<BigInteger> lInt && right is ConcreteValue<BigInteger> rInt)
            {
                return new ConcreteValue<bool>(lInt.Value != BigInteger.Zero || rInt.Value != BigInteger.Zero);
            }
            if (left is ConcreteValue<bool> lBool && right is ConcreteValue<BigInteger> rIntB)
            {
                return new ConcreteValue<bool>(lBool.Value || rIntB.Value != BigInteger.Zero);
            }
            if (left is ConcreteValue<BigInteger> lIntB && right is ConcreteValue<bool> rBool)
            {
                return new ConcreteValue<bool>(lIntB.Value != BigInteger.Zero || rBool.Value);
            }
            throw new InvalidOperationException($"BoolOr requires concrete Boolean or BigInteger, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        public SymbolicValue Equal(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "Equal");

            // Handle different types of equality based on the types of the operands
            if (left is ConcreteValue<BigInteger> l && right is ConcreteValue<BigInteger> r)
            {
                return new ConcreteValue<bool>(l.Value == r.Value);
            }
            else if (left is ConcreteValue<bool> lb && right is ConcreteValue<bool> rb)
            {
                return new ConcreteValue<bool>(lb.Value == rb.Value);
            }
            else if (left is ConcreteValue<byte[]> lba && right is ConcreteValue<byte[]> rba)
            {
                // Compare byte arrays
                if (lba.Value.Length != rba.Value.Length)
                    return new ConcreteValue<bool>(false);

                for (int i = 0; i < lba.Value.Length; i++)
                {
                    if (lba.Value[i] != rba.Value[i])
                        return new ConcreteValue<bool>(false);
                }

                return new ConcreteValue<bool>(true);
            }

            // If types don't match, they're not equal
            if (left.Type != right.Type)
                return new ConcreteValue<bool>(false);

            // Default case - compare string representations
            return new ConcreteValue<bool>(left.ToString() == right.ToString());
        }

        public SymbolicValue NotEqual(SymbolicValue left, SymbolicValue right)
        {
            // Reuse Equal and negate the result
            var equalResult = Equal(left, right);
            if (equalResult is ConcreteValue<bool> boolResult)
            {
                return new ConcreteValue<bool>(!boolResult.Value);
            }

            throw new InvalidOperationException($"NotEqual operation failed: Equal did not return a boolean result");
        }

        public SymbolicValue NumericEquals(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "NumericEquals");
            if (left is ConcreteValue<BigInteger> l && right is ConcreteValue<BigInteger> r)
            {
                return new ConcreteValue<bool>(l.Value == r.Value);
            }
            throw new InvalidOperationException($"NumericEquals requires concrete BigIntegers, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        public SymbolicValue NumericNotEquals(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "NumericNotEquals");
            if (left is ConcreteValue<BigInteger> l && right is ConcreteValue<BigInteger> r)
            {
                return new ConcreteValue<bool>(l.Value != r.Value);
            }
            throw new InvalidOperationException($"NumericNotEquals requires concrete BigIntegers, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        public SymbolicValue LessThan(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "LessThan");
            if (left is ConcreteValue<BigInteger> l && right is ConcreteValue<BigInteger> r)
            {
                return new ConcreteValue<bool>(l.Value < r.Value);
            }
            throw new InvalidOperationException($"LessThan requires concrete BigIntegers, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        public SymbolicValue LessThanOrEqual(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "LessThanOrEqual");
            if (left is ConcreteValue<BigInteger> l && right is ConcreteValue<BigInteger> r)
            {
                return new ConcreteValue<bool>(l.Value <= r.Value);
            }
            throw new InvalidOperationException($"LessThanOrEqual requires concrete BigIntegers, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        public SymbolicValue GreaterThan(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "GreaterThan");
            if (left is ConcreteValue<BigInteger> l && right is ConcreteValue<BigInteger> r)
            {
                return new ConcreteValue<bool>(l.Value > r.Value);
            }
            throw new InvalidOperationException($"GreaterThan requires concrete BigIntegers, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        public SymbolicValue GreaterThanOrEqual(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "GreaterThanOrEqual");
            if (left is ConcreteValue<BigInteger> l && right is ConcreteValue<BigInteger> r)
            {
                return new ConcreteValue<bool>(l.Value >= r.Value);
            }
            throw new InvalidOperationException($"GreaterThanOrEqual requires concrete BigIntegers, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        public SymbolicValue Min(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "Min");
            if (left is ConcreteValue<BigInteger> l && right is ConcreteValue<BigInteger> r)
            {
                return new ConcreteValue<BigInteger>(BigInteger.Min(l.Value, r.Value));
            }
            throw new InvalidOperationException($"Min requires concrete BigIntegers, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        public SymbolicValue Max(SymbolicValue left, SymbolicValue right)
        {
            EnsureConcrete(left, right, "Max");
            if (left is ConcreteValue<BigInteger> l && right is ConcreteValue<BigInteger> r)
            {
                return new ConcreteValue<BigInteger>(BigInteger.Max(l.Value, r.Value));
            }
            throw new InvalidOperationException($"Max requires concrete BigIntegers, got: {left?.GetType().Name}, {right?.GetType().Name}");
        }

        // Ternary Operations
        public SymbolicValue Within(SymbolicValue value, SymbolicValue min_bound, SymbolicValue max_bound)
        {
            EnsureConcrete(value, min_bound, max_bound, "Within");
            if (value is ConcreteValue<BigInteger> v &&
                min_bound is ConcreteValue<BigInteger> min &&
                max_bound is ConcreteValue<BigInteger> max)
            {
                // NeoVM WITHIN is [min, max)
                return new ConcreteValue<bool>(v.Value >= min.Value && v.Value < max.Value);
            }
            throw new InvalidOperationException($"Within requires concrete BigIntegers, got: {value?.GetType().Name}, {min_bound?.GetType().Name}, {max_bound?.GetType().Name}");
        }

        // NOTE: Removed helper methods ConvertToNumeric, ConvertToBoolean,
        // Concatenate, CompareByteArrays as they are no longer needed with
        // the strongly-typed ConcreteValue approach. Type checking and value
        // extraction happen directly within each operation method.
    }
}
