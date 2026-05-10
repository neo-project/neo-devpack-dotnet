// Copyright (C) 2015-2026 The Neo Project.
//
// RuntimeNotification.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.VM.Types;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Neo.SmartContract.Testing
{
    [DebuggerDisplay("{Sender}: {EventName}")]
    public sealed class RuntimeNotification
    {
        /// <summary>
        /// Contract that emitted the notification.
        /// </summary>
        public UInt160 Sender { get; }

        /// <summary>
        /// Notification event name.
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Notification state.
        /// </summary>
        public IReadOnlyList<StackItem> State { get; }

        /// <summary>
        /// Constructor of RuntimeNotification.
        /// </summary>
        /// <param name="sender">Contract that emitted the notification.</param>
        /// <param name="eventName">Notification event name.</param>
        /// <param name="state">Notification state.</param>
        public RuntimeNotification(UInt160 sender, string eventName, IEnumerable<StackItem> state)
        {
            Sender = sender;
            EventName = eventName;
            State = state.Select(item => item.DeepCopy(true)).ToArray();
        }
    }
}
