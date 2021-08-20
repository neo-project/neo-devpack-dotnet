// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Framework.Services
{
    public class Notification : IApiInterface
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
