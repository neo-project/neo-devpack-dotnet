using System.Runtime.CompilerServices;

namespace Neo.SmartContract.Framework
{
    public abstract class SmartContract
    {
        /// <summary>
        /// Faults if `condition` is false
        /// </summary>
        /// <param name="condition">Condition that MUST meet</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        [OpCode(OpCode.ASSERT)]
        public static extern void Assert(bool condition);

        /// <summary>
        /// Abort the execution
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        [OpCode(OpCode.ABORT)]
        public static extern void Abort();
    }
}
