// Copyright (C) 2015-2025 The Neo Project.
//
// NetworkContext.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;

namespace Neo.SmartContract.Framework.ContractInvocation
{
    /// <summary>
    /// Represents the network context for contract deployment and invocation.
    /// Manages different contract addresses across multiple network environments.
    /// </summary>
    public sealed class NetworkContext
    {
        private readonly Dictionary<string, UInt160> _networkAddresses;

        /// <summary>
        /// Gets the current active network name.
        /// </summary>
        public string CurrentNetwork { get; private set; }

        /// <summary>
        /// Gets all configured networks for this context.
        /// </summary>
        public IReadOnlyCollection<string> ConfiguredNetworks => _networkAddresses.Keys;

        /// <summary>
        /// Initializes a new NetworkContext with the specified current network.
        /// </summary>
        /// <param name="currentNetwork">The name of the current active network</param>
        public NetworkContext(string currentNetwork = "privnet")
        {
            CurrentNetwork = currentNetwork ?? throw new ArgumentNullException(nameof(currentNetwork));
            _networkAddresses = new Dictionary<string, UInt160>();
        }

        /// <summary>
        /// Sets the contract address for a specific network.
        /// </summary>
        /// <param name="network">The network name (e.g., "privnet", "testnet", "mainnet")</param>
        /// <param name="address">The contract address on that network</param>
        public void SetNetworkAddress(string network, UInt160 address)
        {
            if (string.IsNullOrEmpty(network))
                throw new ArgumentNullException(nameof(network));
            if (address is null)
                throw new ArgumentNullException(nameof(address));

            _networkAddresses[network] = address;
        }

        /// <summary>
        /// Sets the contract address for a specific network using a string representation.
        /// </summary>
        /// <param name="network">The network name</param>
        /// <param name="address">The contract address as a string (hex or NEO address)</param>
        public void SetNetworkAddress(string network, string address)
        {
            if (string.IsNullOrEmpty(network))
                throw new ArgumentNullException(nameof(network));
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            // The actual conversion logic will be handled by the compiler
            // This is a placeholder for the contract invocation system
            var uint160 = ParseAddressString(address);
            SetNetworkAddress(network, uint160);
        }

        /// <summary>
        /// Gets the contract address for the current active network.
        /// </summary>
        /// <returns>The contract address, or null if not configured for the current network</returns>
        public UInt160? GetCurrentNetworkAddress()
        {
            return _networkAddresses.TryGetValue(CurrentNetwork, out var address) ? address : null;
        }

        /// <summary>
        /// Gets the contract address for a specific network.
        /// </summary>
        /// <param name="network">The network name</param>
        /// <returns>The contract address, or null if not configured for that network</returns>
        public UInt160? GetNetworkAddress(string network)
        {
            if (string.IsNullOrEmpty(network))
                return null;

            return _networkAddresses.TryGetValue(network, out var address) ? address : null;
        }

        /// <summary>
        /// Switches the current active network.
        /// </summary>
        /// <param name="network">The network to switch to</param>
        public void SwitchNetwork(string network)
        {
            if (string.IsNullOrEmpty(network))
                throw new ArgumentNullException(nameof(network));

            CurrentNetwork = network;
        }

        /// <summary>
        /// Checks if the context has an address configured for the specified network.
        /// </summary>
        /// <param name="network">The network name to check</param>
        /// <returns>True if the network has a configured address</returns>
        public bool HasNetworkAddress(string network)
        {
            return !string.IsNullOrEmpty(network) && _networkAddresses.ContainsKey(network);
        }

        private static UInt160 ParseAddressString(string address)
        {
            // This method will be replaced by compiler logic
            // For now, return a default value
            return UInt160.Zero;
        }
    }
}
