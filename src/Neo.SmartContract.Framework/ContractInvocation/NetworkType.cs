// Copyright (C) 2015-2025 The Neo Project.
//
// NetworkType.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Framework.ContractInvocation
{
    /// <summary>
    /// Represents the different Neo network types.
    /// </summary>
    public enum NetworkType
    {
        /// <summary>
        /// The main Neo network.
        /// </summary>
        Mainnet,

        /// <summary>
        /// The test Neo network.
        /// </summary>
        Testnet,

        /// <summary>
        /// A private Neo network for development.
        /// </summary>
        Privnet
    }
}
