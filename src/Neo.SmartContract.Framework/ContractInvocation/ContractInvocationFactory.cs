// Copyright (C) 2015-2025 The Neo Project.
//
// ContractInvocationFactory.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using Neo.SmartContract.Framework.ContractInvocation.Attributes;

namespace Neo.SmartContract.Framework.ContractInvocation
{
    /// <summary>
    /// Factory class for creating and managing contract references and proxies.
    /// This class provides high-level APIs for contract invocation setup.
    /// </summary>
    public static class ContractInvocationFactory
    {
        private static readonly Dictionary<string, IContractReference> _registeredContracts = new();
        private static NetworkContext? _defaultNetworkContext;

        /// <summary>
        /// Gets or sets the default network context used for new contract references.
        /// </summary>
        public static NetworkContext DefaultNetworkContext
        {
            get => _defaultNetworkContext ??= new NetworkContext();
            set => _defaultNetworkContext = value;
        }

        /// <summary>
        /// Registers a development contract reference.
        /// </summary>
        /// <param name="identifier">The contract identifier</param>
        /// <param name="projectPath">The relative path to the contract project</param>
        /// <param name="networkContext">Optional network context</param>
        /// <returns>The created contract reference</returns>
        public static DevelopmentContractReference RegisterDevelopmentContract(
            string identifier,
            string projectPath,
            NetworkContext? networkContext = null)
        {
            var reference = new DevelopmentContractReference(identifier, projectPath, networkContext ?? DefaultNetworkContext);
            _registeredContracts[identifier] = reference;
            return reference;
        }

        /// <summary>
        /// Registers a deployed contract reference.
        /// </summary>
        /// <param name="identifier">The contract identifier</param>
        /// <param name="networkContext">The network context with addresses</param>
        /// <returns>The created contract reference</returns>
        public static DeployedContractReference RegisterDeployedContract(
            string identifier,
            NetworkContext? networkContext = null)
        {
            var reference = new DeployedContractReference(identifier, networkContext ?? DefaultNetworkContext);
            _registeredContracts[identifier] = reference;
            return reference;
        }

        /// <summary>
        /// Registers a deployed contract reference with a single address.
        /// </summary>
        /// <param name="identifier">The contract identifier</param>
        /// <param name="address">The contract address</param>
        /// <param name="network">The network name</param>
        /// <returns>The created contract reference</returns>
        public static DeployedContractReference RegisterDeployedContract(
            string identifier,
            UInt160 address,
            string network = "privnet")
        {
            var reference = DeployedContractReference.Create(identifier, address, network);
            _registeredContracts[identifier] = reference;
            return reference;
        }

        /// <summary>
        /// Registers a deployed contract reference with multiple network addresses.
        /// </summary>
        /// <param name="identifier">The contract identifier</param>
        /// <param name="privnetAddress">The address on privnet</param>
        /// <param name="testnetAddress">The address on testnet</param>
        /// <param name="mainnetAddress">The address on mainnet</param>
        /// <param name="currentNetwork">The current active network</param>
        /// <returns>The created contract reference</returns>
        public static DeployedContractReference RegisterMultiNetworkContract(
            string identifier,
            UInt160? privnetAddress = null,
            UInt160? testnetAddress = null,
            UInt160? mainnetAddress = null,
            string currentNetwork = "privnet")
        {
            var reference = DeployedContractReference.CreateMultiNetwork(
                identifier, privnetAddress, testnetAddress, mainnetAddress, currentNetwork);
            _registeredContracts[identifier] = reference;
            return reference;
        }

        /// <summary>
        /// Gets a registered contract reference by identifier.
        /// </summary>
        /// <param name="identifier">The contract identifier</param>
        /// <returns>The contract reference, or null if not found</returns>
        public static IContractReference? GetContractReference(string identifier)
        {
            return _registeredContracts.TryGetValue(identifier, out var reference) ? reference : null;
        }

        /// <summary>
        /// Creates a contract reference from a ContractReferenceAttribute.
        /// </summary>
        /// <param name="attribute">The attribute containing reference information</param>
        /// <returns>The created contract reference</returns>
        public static IContractReference CreateFromAttribute(ContractReferenceAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            var networkContext = CreateNetworkContextFromAttribute(attribute);

            return attribute.ReferenceType switch
            {
                ContractReferenceType.Development => new DevelopmentContractReference(
                    attribute.Identifier,
                    attribute.ProjectPath ?? attribute.Identifier,
                    networkContext),
                ContractReferenceType.Deployed => new DeployedContractReference(
                    attribute.Identifier,
                    networkContext),
                ContractReferenceType.Auto => DetermineReferenceType(attribute, networkContext),
                _ => throw new ArgumentException($"Unknown contract reference type: {attribute.ReferenceType}")
            };
        }

        /// <summary>
        /// Gets all registered contract references.
        /// </summary>
        /// <returns>A collection of all registered contract references</returns>
        public static IReadOnlyCollection<IContractReference> GetAllRegisteredContracts()
        {
            return _registeredContracts.Values;
        }

        /// <summary>
        /// Clears all registered contract references.
        /// </summary>
        public static void ClearRegisteredContracts()
        {
            _registeredContracts.Clear();
        }

        /// <summary>
        /// Sets the current network for all registered contracts.
        /// </summary>
        /// <param name="network">The network to switch to</param>
        public static void SwitchNetwork(string network)
        {
            DefaultNetworkContext.SwitchNetwork(network);
            foreach (var reference in _registeredContracts.Values)
            {
                reference.NetworkContext.SwitchNetwork(network);
            }
        }

        private static NetworkContext CreateNetworkContextFromAttribute(ContractReferenceAttribute attribute)
        {
            var networkContext = new NetworkContext(DefaultNetworkContext.CurrentNetwork);

            if (!string.IsNullOrEmpty(attribute.PrivnetAddress))
                networkContext.SetNetworkAddress("privnet", attribute.PrivnetAddress);
            if (!string.IsNullOrEmpty(attribute.TestnetAddress))
                networkContext.SetNetworkAddress("testnet", attribute.TestnetAddress);
            if (!string.IsNullOrEmpty(attribute.MainnetAddress))
                networkContext.SetNetworkAddress("mainnet", attribute.MainnetAddress);

            return networkContext;
        }

        private static IContractReference DetermineReferenceType(ContractReferenceAttribute attribute, NetworkContext networkContext)
        {
            // Auto-detection logic:
            // 1. If ProjectPath is specified, treat as development contract
            // 2. If any network addresses are specified, treat as deployed contract
            // 3. If identifier looks like a path, treat as development contract
            // 4. Otherwise, treat as deployed contract

            if (!string.IsNullOrEmpty(attribute.ProjectPath))
            {
                return new DevelopmentContractReference(attribute.Identifier, attribute.ProjectPath, networkContext);
            }

            if (!string.IsNullOrEmpty(attribute.PrivnetAddress) ||
                !string.IsNullOrEmpty(attribute.TestnetAddress) ||
                !string.IsNullOrEmpty(attribute.MainnetAddress))
            {
                return new DeployedContractReference(attribute.Identifier, networkContext);
            }

            if (attribute.Identifier.Contains("/") || attribute.Identifier.Contains("\\") || attribute.Identifier.EndsWith(".csproj"))
            {
                return new DevelopmentContractReference(attribute.Identifier, attribute.Identifier, networkContext);
            }

            return new DeployedContractReference(attribute.Identifier, networkContext);
        }
    }
}
