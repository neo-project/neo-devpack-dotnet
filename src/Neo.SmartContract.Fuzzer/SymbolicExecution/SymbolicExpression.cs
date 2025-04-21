using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM.Types;
using System;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Represents a symbolic expression in the symbolic execution engine.
    /// </summary>
    public class SymbolicExpression : SymbolicValue
    {
        /// <summary>
        /// Gets the left operand of the expression.
        /// </summary>
        public SymbolicValue? Left { get; }

        /// <summary>
        /// Gets the operator of the expression.
        /// </summary>
        public Operator Operator { get; }

        /// <summary>
        /// Gets the right operand of the expression.
        /// </summary>
        public SymbolicValue? Right { get; }

        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public override StackItemType Type => StackItemType.Boolean;

        /// <summary>
        /// Initializes a new instance of the SymbolicExpression class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="op">The operator.</param>
        /// <param name="right">The right operand.</param>
        public SymbolicExpression(SymbolicValue left, Operator op, SymbolicValue right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Operator = op;
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        /// <summary>
        /// Initializes a new instance of the SymbolicExpression class for unary operations.
        /// </summary>
        /// <param name="op">The operator.</param>
        /// <param name="operand">The operand.</param>
        public SymbolicExpression(Operator op, SymbolicValue operand)
        {
            Left = operand ?? throw new ArgumentNullException(nameof(operand));
            Operator = op;
            Right = null;
        }

        /// <summary>
        /// Returns a string representation of the expression.
        /// </summary>
        /// <returns>A string representation of the expression.</returns>
        public override string ToString()
        {
            if (Right == null)
            {
                // Unary operation
                return $"{Operator}({Left})";
            }
            else
            {
                // Binary operation
                return $"({Left} {Operator} {Right})";
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is SymbolicExpression other)
            {
                if (Operator != other.Operator)
                    return false;

                if (Right == null && other.Right == null)
                {
                    // Unary operation
                    return Left?.Equals(other.Left) ?? other.Left == null;
                }
                else if (Right != null && other.Right != null)
                {
                    // Binary operation
                    return (Left?.Equals(other.Left) ?? other.Left == null) &&
                           (Right.Equals(other.Right));
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Left, Operator, Right);
        }

        /// <summary>
        /// Converts this symbolic expression to a stack item.
        /// </summary>
        /// <returns>A stack item representation of this expression.</returns>
        public override StackItem ToStackItem()
        {
            // Since this is a symbolic expression, we can't convert it to a concrete stack item
            // without a model/assignment. For testing purposes, we'll return a boolean.
            return VM.Types.Boolean.False;
        }
    }

    /// <summary>
    /// Represents operators that can be used in symbolic expressions.
    /// </summary>
    public enum Operator
    {
        /// <summary>
        /// The identity operator (no operation).
        /// </summary>
        Identity,

        /// <summary>
        /// The addition operator.
        /// </summary>
        Add,

        /// <summary>
        /// The subtraction operator.
        /// </summary>
        Subtract,

        /// <summary>
        /// The multiplication operator.
        /// </summary>
        Multiply,

        /// <summary>
        /// The division operator.
        /// </summary>
        Divide,

        /// <summary>
        /// The modulo operator.
        /// </summary>
        Modulo,

        /// <summary>
        /// The equality operator.
        /// </summary>
        Equal,

        /// <summary>
        /// The inequality operator.
        /// </summary>
        NotEqual,

        /// <summary>
        /// The less than operator.
        /// </summary>
        LessThan,

        /// <summary>
        /// The less than or equal operator.
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// The greater than operator.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// The greater than or equal operator.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// The logical AND operator.
        /// </summary>
        And,

        /// <summary>
        /// The logical OR operator.
        /// </summary>
        Or,

        /// <summary>
        /// The logical NOT operator.
        /// </summary>
        Not,

        /// <summary>
        /// The bitwise AND operator.
        /// </summary>
        BitwiseAnd,

        /// <summary>
        /// The bitwise OR operator.
        /// </summary>
        BitwiseOr,

        /// <summary>
        /// The bitwise XOR operator.
        /// </summary>
        BitwiseXor,

        /// <summary>
        /// The bitwise NOT operator.
        /// </summary>
        BitwiseNot,

        /// <summary>
        /// The left shift operator.
        /// </summary>
        LeftShift,

        /// <summary>
        /// The right shift operator.
        /// </summary>
        RightShift,

        /// <summary>
        /// The sign operator.
        /// </summary>
        Sign,

        /// <summary>
        /// The negate operator.
        /// </summary>
        Negate,

        /// <summary>
        /// The absolute value operator.
        /// </summary>
        Abs,

        /// <summary>
        /// The minimum operator.
        /// </summary>
        Min,

        /// <summary>
        /// The maximum operator.
        /// </summary>
        Max,

        /// <summary>
        /// The within operator.
        /// </summary>
        Within
    }
}
