// Copyright (C) 2015-2022 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;

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
        /// Faults if `condition` is false
        /// </summary>
        /// <param name="condition">Condition that MUST meet</param>
        /// <param name="message">The error message</param>
        /// <param name="isNotify">in a notify manner</param>
        public static void Assert(bool condition, string message, bool isNotify = false)
        {
            if (condition) return;

            if (isNotify)
            {
                Services.Runtime.Notify("Fault", new object[] { message });
                Abort();
            }
            else
            {
                Services.Runtime.Log(message);
                Assert(false);
            }
        }

        /// <summary>
        /// Abort the execution
        /// </summary>
        [OpCode(OpCode.ABORT)]
        public static extern void Abort();
    }
}
