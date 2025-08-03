// Copyright (C) 2015-2025 The Neo Project.
//
// DeployedContractReference.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.ContractInvocation
{
    /// <summary>
    /// Represents a reference to a deployed contract on the blockchain.
    /// This type of reference can have different addresses across different networks.
    /// </summary>
    public sealed class DeployedContractReference : IContractReference
    {
        /// <summary>
        /// Gets the contract identifier (usually the contract name).
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Gets the resolved contract hash for the current network.
        /// </summary>
        public UInt160? ResolvedHash => NetworkContext.GetCurrentNetworkAddress();

        /// <summary>
        /// Gets the network context for this contract reference.
        /// </summary>
        public NetworkContext NetworkContext { get; }

        /// <summary>
        /// Gets a value indicating whether this contract reference is resolved
        /// for the current network.
        /// </summary>
        public bool IsResolved => ResolvedHash != null;

        /// <summary>
        /// Gets the contract manifest, if available.
        /// </summary>
        public ContractManifest? Manifest { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the manifest should be fetched
        /// from the blockchain during compilation.
        /// </summary>
        public bool FetchManifest { get; set; } = true;

        /// <summary>
        /// Initializes a new DeployedContractReference.
        /// </summary>
        /// <param name="identifier">The contract identifier</param>
        /// <param name="networkContext">The network context with addresses</param>
        public DeployedContractReference(string identifier, NetworkContext? networkContext = null)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            NetworkContext = networkContext ?? new NetworkContext();
        }

        /// <summary>
        /// Creates a deployed contract reference with a single network address.
        /// </summary>
        /// <param name="identifier">The contract identifier</param>
        /// <param name="address">The contract address</param>
        /// <param name="network">The network name (default: "privnet")</param>
        /// <returns>A new DeployedContractReference</returns>
        public static DeployedContractReference Create(string identifier, UInt160 address, string network = "privnet")
        {
            var networkContext = new NetworkContext(network);
            networkContext.SetNetworkAddress(network, address);
            return new DeployedContractReference(identifier, networkContext);
        }

        /// <summary>
        /// Creates a deployed contract reference with multiple network addresses.
        /// </summary>
        /// <param name="identifier">The contract identifier</param>
        /// <param name="privnetAddress">The address on privnet</param>
        /// <param name="testnetAddress">The address on testnet</param>
        /// <param name="mainnetAddress">The address on mainnet</param>
        /// <param name="currentNetwork">The current active network</param>
        /// <returns>A new DeployedContractReference</returns>
        public static DeployedContractReference CreateMultiNetwork(
            string identifier,
            UInt160? privnetAddress = null,
            UInt160? testnetAddress = null,
            UInt160? mainnetAddress = null,
            string currentNetwork = "privnet")
        {
            var networkContext = new NetworkContext(currentNetwork);
            
            if (privnetAddress != null)
                networkContext.SetNetworkAddress("privnet", privnetAddress);
            if (testnetAddress != null)
                networkContext.SetNetworkAddress("testnet", testnetAddress);
            if (mainnetAddress != null)
                networkContext.SetNetworkAddress("mainnet", mainnetAddress);
            
            return new DeployedContractReference(identifier, networkContext);
        }

        /// <summary>
        /// Sets the contract manifest for this reference.
        /// This method is called by the compilation system when manifest is fetched.
        /// </summary>
        /// <param name="manifest">The contract manifest</param>
        internal void SetManifest(ContractManifest manifest)
        {
            Manifest = manifest;
        }

        /// <summary>
        /// Adds an address for a specific network.
        /// </summary>
        /// <param name="network">The network name</param>
        /// <param name="address">The contract address on that network</param>
        public void AddNetworkAddress(string network, UInt160 address)
        {
            NetworkContext.SetNetworkAddress(network, address);
        }

        /// <summary>
        /// Adds an address for a specific network using string representation.
        /// </summary>
        /// <param name="network">The network name</param>
        /// <param name="address">The contract address as string</param>
        public void AddNetworkAddress(string network, string address)
        {
            NetworkContext.SetNetworkAddress(network, address);
        }
    }
}