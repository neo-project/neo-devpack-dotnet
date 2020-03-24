namespace Neo.SmartContract.Framework
{
    public class SmartContract
    {
        /// <summary>
        /// Faults if `condition` is false
        /// </summary>
        /// <param name="condition">Condition that MUST meet</param>
        [OpCode(OpCode.ASSERT)]
        public extern static void Assert(bool condition);

        /// <summary>
        /// Abort the execution
        /// </summary>
        [OpCode(OpCode.ABORT)]
        public extern static void Abort();
    }
}
