namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents operators that can be used in symbolic expressions.
    /// </summary>
    public enum Operator
    {
        // Identity (no operation, just a wrapper)
        Identity,
        // Arithmetic operators
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        Sign,
        Abs,
        Negate,

        // Logical operators (Boolean)
        And,
        Or,
        Not,

        // Comparison operators
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,

        // Bitwise operators
        LeftShift,
        RightShift,
        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,
        BitwiseNot,

        // Comparison/Bound operators
        Min,
        Max,
        Within,

        // String/Buffer operators (TODO: If needed)
        // Concat,
        // Substring,

        // Type conversion (TODO: If needed)
        // Convert,

        // Special placeholder for Syscall results (TODO: Define specific syscalls?)
        Syscall
    }
}
