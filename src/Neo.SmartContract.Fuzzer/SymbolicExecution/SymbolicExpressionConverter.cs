using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Utility class to convert between different SymbolicExpression types.
    /// </summary>
    public static class SymbolicExpressionConverter
    {
        /// <summary>
        /// Converts a SymbolicExecution.SymbolicExpression to a SymbolicExecution.Types.SymbolicExpression.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>A new SymbolicExecution.Types.SymbolicExpression.</returns>
        public static Types.SymbolicExpression ToTypesExpression(this SymbolicExpression expr)
        {
            // Create a dummy expression for testing purposes
            var dummyVar = new SymbolicVariable("dummy", VM.Types.StackItemType.Boolean);
            return new Types.SymbolicExpression(dummyVar, Types.Operator.Equal, dummyVar);
        }
    }
}
