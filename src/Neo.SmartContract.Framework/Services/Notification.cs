// Copyright (C) 2015-2024 The Neo Project.
//
// Notification.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Framework.Services
{
    public class Notification
    {
        /// <summary>
        /// Sender script hash
        /// </summary>
        public readonly UInt160 ScriptHash;

        /// <summary>
        /// Notification's name
        /// </summary>
        public readonly string EventName;

        /// <summary>
        /// Notification's state
        /// </summary>
        public readonly object[] State;
    }
}
