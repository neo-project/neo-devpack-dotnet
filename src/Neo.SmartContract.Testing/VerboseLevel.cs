// Copyright (C) 2015-2026 The Neo Project.
//
// VerboseLevel.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Testing
{
    /// <summary>
    /// Controls the execution information written by <see cref="TestEngine"/>.
    /// </summary>
    public enum VerboseLevel
    {
        /// <summary>
        /// Disable execution tracing.
        /// </summary>
        None = 0,

        /// <summary>
        /// Trace execution results, contract logs, and notifications.
        /// </summary>
        Basic = 1,

        /// <summary>
        /// Also trace contract calls, returns, and storage reads and writes.
        /// </summary>
        Detailed = 2,

        /// <summary>
        /// Also trace every instruction with its gas and evaluation stack.
        /// </summary>
        Trace = 3
    }
}
