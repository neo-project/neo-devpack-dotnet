using Neo.VM.Types;
using System;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a binary expression in symbolic execution.
    /// </summary>
    public class SymbolicBinaryExpression : SymbolicExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicBinaryExpression"/> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <param name="op">The operator.</param>
        public SymbolicBinaryExpression(SymbolicValue left, Operator op, SymbolicValue right)
            : base(left, op, right)
        {
        }

        /// <summary>
        /// Returns a string representation of this binary expression.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            return $"({Left} {GetOperatorString(Operator)} {Right})";
        }

        /// <summary>
        /// Gets a string representation of the operator.
        /// </summary>
        private string GetOperatorString(Operator op)
        {
            return op switch
            {
                Operator.Add => "+",
                Operator.Subtract => "-",
                Operator.Multiply => "*",
                Operator.Divide => "/",
                Operator.Modulo => "%",
                Operator.Equal => "==",
                Operator.NotEqual => "!=",
                Operator.LessThan => "<",
                Operator.LessThanOrEqual => "<=",
                Operator.GreaterThan => ">",
                Operator.GreaterThanOrEqual => ">=",
                Operator.And => "&&",
                Operator.Or => "||",
                _ => op.ToString()
            };
        }
    }
}
