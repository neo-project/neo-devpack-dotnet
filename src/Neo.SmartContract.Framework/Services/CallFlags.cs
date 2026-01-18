// Copyright (C) 2015-2026 The Neo Project.
//
// CallFlags.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Framework.Services
{
    [Flags]
    public enum CallFlags : byte
    {
        /// <summary>
        /// No flag is set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that the contract can read the states.
        /// </summary>
        ReadStates = 0b00000001,

        /// <summary>
        /// Indicates that the contract can write the states.
        /// </summary>
        WriteStates = 0b00000010,

        /// <summary>
        /// Indicates that the contract can call other contracts.
        /// </summary>
        AllowCall = 0b00000100,
        /// <summary>
        /// Indicates that the contract can notify other contracts.
        /// </summary>
        AllowNotify = 0b00001000,

        /// <summary>
        /// Indicates that the contract can read and write the states.
        /// </summary>
        States = ReadStates | WriteStates,

        /// <summary>
        /// Indicates that the contract can read the states and call other contracts.
        /// </summary>
        ReadOnly = ReadStates | AllowCall,

        /// <summary>
        /// Indicates that the contract can read and write the states and call other contracts and notify other contracts.
        /// </summary>
        All = States | AllowCall | AllowNotify
    }
}
