// Copyright (C) 2015-2026 The Neo Project.
//
// RuntimeLog.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Diagnostics;

namespace Neo.SmartContract.Testing
{
    [DebuggerDisplay("{Sender}: {Message}")]
    public sealed class RuntimeLog
    {
        /// <summary>
        /// Contract that emitted the log.
        /// </summary>
        public UInt160 Sender { get; }

        /// <summary>
        /// Log message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Constructor of RuntimeLog.
        /// </summary>
        /// <param name="sender">Contract that emitted the log.</param>
        /// <param name="message">Log message.</param>
        public RuntimeLog(UInt160 sender, string message)
        {
            Sender = sender;
            Message = message;
        }
    }
}
