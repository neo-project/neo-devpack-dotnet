// Copyright (C) 2015-2025 The Neo Project.
//
// IContractReference.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.ContractInvocation
{
    /// <summary>
    /// Interface for contract references that can be resolved to contract addresses
    /// during compilation or runtime, depending on the contract deployment status.
    /// </summary>
    public interface IContractReference
    {
        /// <summary>
        /// Gets the contract identifier for this reference.
        /// Can be a name, path, or hash depending on the reference type.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Gets the resolved contract hash if available.
        /// Returns null for development contracts without deployed addresses.
        /// </summary>
        UInt160? ResolvedHash { get; }

        /// <summary>
        /// Gets the network context for this contract reference.
        /// Used to differentiate between privnet, testnet, and mainnet deployments.
        /// </summary>
        NetworkContext NetworkContext { get; }

        /// <summary>
        /// Indicates whether this contract reference is resolved and ready for invocation.
        /// </summary>
        bool IsResolved { get; }
    }
}