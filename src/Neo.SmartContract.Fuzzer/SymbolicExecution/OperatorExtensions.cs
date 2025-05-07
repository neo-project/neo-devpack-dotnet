using System;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Extension methods for the Operator enum.
    /// </summary>
    public static class OperatorExtensions
    {
        /// <summary>
        /// Compares two operators for equality.
        /// </summary>
        /// <param name="op1">The first operator.</param>
        /// <param name="op2">The second operator.</param>
        /// <returns>True if the operators are equal, false otherwise.</returns>
        public static bool OperatorEquals(this Operator op1, Operator op2)
        {
            return op1.ToString() == op2.ToString();
        }
    }
}
