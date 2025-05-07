using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Provides conversion methods between different Operator enum types.
    /// </summary>
    public static class OperatorConverter
    {
        /// <summary>
        /// Converts from Neo.SmartContract.Fuzzer.SymbolicExecution.Operator to Neo.SmartContract.Fuzzer.SymbolicExecution.Types.Operator.
        /// </summary>
        /// <param name="op">The operator to convert.</param>
        /// <returns>The converted operator.</returns>
        public static Types.Operator ToTypesOperator(this Operator op)
        {
            return op switch
            {
                Operator.Identity => Types.Operator.Identity,
                Operator.Add => Types.Operator.Add,
                Operator.Subtract => Types.Operator.Subtract,
                Operator.Multiply => Types.Operator.Multiply,
                Operator.Divide => Types.Operator.Divide,
                Operator.Modulo => Types.Operator.Modulo,
                Operator.Equal => Types.Operator.Equal,
                Operator.NotEqual => Types.Operator.NotEqual,
                Operator.LessThan => Types.Operator.LessThan,
                Operator.LessThanOrEqual => Types.Operator.LessThanOrEqual,
                Operator.GreaterThan => Types.Operator.GreaterThan,
                Operator.GreaterThanOrEqual => Types.Operator.GreaterThanOrEqual,
                Operator.And => Types.Operator.And,
                Operator.Or => Types.Operator.Or,
                Operator.Not => Types.Operator.Not,
                Operator.BitwiseAnd => Types.Operator.BitwiseAnd,
                Operator.BitwiseOr => Types.Operator.BitwiseOr,
                Operator.BitwiseXor => Types.Operator.BitwiseXor,
                Operator.BitwiseNot => Types.Operator.BitwiseNot,
                Operator.LeftShift => Types.Operator.LeftShift,
                Operator.RightShift => Types.Operator.RightShift,
                Operator.Sign => Types.Operator.Sign,
                Operator.Negate => Types.Operator.Negate,
                Operator.Abs => Types.Operator.Abs,
                Operator.Min => Types.Operator.Min,
                Operator.Max => Types.Operator.Max,
                Operator.Within => Types.Operator.Within,
                _ => Types.Operator.Identity
            };
        }

        /// <summary>
        /// Converts from Neo.SmartContract.Fuzzer.SymbolicExecution.Types.Operator to Neo.SmartContract.Fuzzer.SymbolicExecution.Operator.
        /// </summary>
        /// <param name="op">The operator to convert.</param>
        /// <returns>The converted operator.</returns>
        public static Operator ToOperator(this Types.Operator op)
        {
            return op switch
            {
                Types.Operator.Identity => Operator.Identity,
                Types.Operator.Add => Operator.Add,
                Types.Operator.Subtract => Operator.Subtract,
                Types.Operator.Multiply => Operator.Multiply,
                Types.Operator.Divide => Operator.Divide,
                Types.Operator.Modulo => Operator.Modulo,
                Types.Operator.Equal => Operator.Equal,
                Types.Operator.NotEqual => Operator.NotEqual,
                Types.Operator.LessThan => Operator.LessThan,
                Types.Operator.LessThanOrEqual => Operator.LessThanOrEqual,
                Types.Operator.GreaterThan => Operator.GreaterThan,
                Types.Operator.GreaterThanOrEqual => Operator.GreaterThanOrEqual,
                Types.Operator.And => Operator.And,
                Types.Operator.Or => Operator.Or,
                Types.Operator.Not => Operator.Not,
                Types.Operator.BitwiseAnd => Operator.BitwiseAnd,
                Types.Operator.BitwiseOr => Operator.BitwiseOr,
                Types.Operator.BitwiseXor => Operator.BitwiseXor,
                Types.Operator.BitwiseNot => Operator.BitwiseNot,
                Types.Operator.LeftShift => Operator.LeftShift,
                Types.Operator.RightShift => Operator.RightShift,
                Types.Operator.Sign => Operator.Sign,
                Types.Operator.Negate => Operator.Negate,
                Types.Operator.Abs => Operator.Abs,
                Types.Operator.Min => Operator.Min,
                Types.Operator.Max => Operator.Max,
                Types.Operator.Within => Operator.Within,
                _ => Operator.Identity
            };
        }

        /// <summary>
        /// Checks if two operators are equivalent, even if they are of different enum types.
        /// </summary>
        /// <param name="op1">The first operator.</param>
        /// <param name="op2">The second operator.</param>
        /// <returns>True if the operators are equivalent, false otherwise.</returns>
        public static bool OperatorEquals(this Operator op1, Types.Operator op2)
        {
            return op1.ToTypesOperator() == op2;
        }

        /// <summary>
        /// Checks if two operators are equivalent, even if they are of different enum types.
        /// </summary>
        /// <param name="op1">The first operator.</param>
        /// <param name="op2">The second operator.</param>
        /// <returns>True if the operators are equivalent, false otherwise.</returns>
        public static bool OperatorEquals(this Types.Operator op1, Operator op2)
        {
            return op1.ToOperator() == op2;
        }
    }
}
