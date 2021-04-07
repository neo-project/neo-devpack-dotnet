namespace Neo.SmartContract.Framework
{
    public static class ExecutionEngine
    {
        /// <summary>
        /// Faults if `condition` is false
        /// </summary>
        /// <param name="condition">Condition that MUST meet</param>
        [OpCode(OpCode.ASSERT)]
        public static extern void Assert(bool condition);

        /// <summary>
        /// Abort the execution
        /// </summary>
        [OpCode(OpCode.ABORT)]
        public static extern void Abort();
    }
}
