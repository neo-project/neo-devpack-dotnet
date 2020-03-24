using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.SmartContract.Framework
{
    public static class Debug
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
