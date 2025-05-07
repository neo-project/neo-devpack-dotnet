namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Represents the type of a symbolic expression operation.
    /// </summary>
    public enum SymbolicExpressionType
    {
        /// <summary>
        /// Unknown operation type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Identity operation (no operation).
        /// </summary>
        Identity,

        /// <summary>
        /// Addition operation.
        /// </summary>
        Add,

        /// <summary>
        /// Subtraction operation.
        /// </summary>
        Subtract,

        /// <summary>
        /// Multiplication operation.
        /// </summary>
        Multiply,

        /// <summary>
        /// Division operation.
        /// </summary>
        Divide,

        /// <summary>
        /// Modulo operation.
        /// </summary>
        Modulo,

        /// <summary>
        /// Equality comparison operation.
        /// </summary>
        Equal,

        /// <summary>
        /// Inequality comparison operation.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Less than comparison operation.
        /// </summary>
        LessThan,

        /// <summary>
        /// Less than or equal comparison operation.
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Greater than comparison operation.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Greater than or equal comparison operation.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Logical AND operation.
        /// </summary>
        And,

        /// <summary>
        /// Logical OR operation.
        /// </summary>
        Or,

        /// <summary>
        /// Logical NOT operation.
        /// </summary>
        Not,

        /// <summary>
        /// Bitwise AND operation.
        /// </summary>
        BitwiseAnd,

        /// <summary>
        /// Bitwise OR operation.
        /// </summary>
        BitwiseOr,

        /// <summary>
        /// Bitwise XOR operation.
        /// </summary>
        BitwiseXor,

        /// <summary>
        /// Bitwise NOT operation.
        /// </summary>
        BitwiseNot,

        /// <summary>
        /// Left shift operation.
        /// </summary>
        LeftShift,

        /// <summary>
        /// Right shift operation.
        /// </summary>
        RightShift,

        /// <summary>
        /// Sign operation.
        /// </summary>
        Sign,

        /// <summary>
        /// Negate operation.
        /// </summary>
        Negate,

        /// <summary>
        /// Absolute value operation.
        /// </summary>
        Abs,

        /// <summary>
        /// Minimum operation.
        /// </summary>
        Min,

        /// <summary>
        /// Maximum operation.
        /// </summary>
        Max,

        /// <summary>
        /// Within range operation.
        /// </summary>
        Within
    }
}
