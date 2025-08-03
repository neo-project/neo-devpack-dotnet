// Copyright (C) 2015-2025 The Neo Project.
//
// ContractReferenceAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Framework.ContractInvocation.Attributes
{
    /// <summary>
    /// Specifies that a field or property represents a contract reference
    /// that should be resolved during compilation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ContractReferenceAttribute : Attribute
    {
        /// <summary>
        /// Gets the contract identifier (name, path, or hash).
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Gets or sets the type of contract reference.
        /// </summary>
        public ContractReferenceType ReferenceType { get; set; } = ContractReferenceType.Auto;

        /// <summary>
        /// Gets or sets the project path for development contracts.
        /// </summary>
        public string? ProjectPath { get; set; }

        /// <summary>
        /// Gets or sets the contract address for the private network.
        /// </summary>
        public string? PrivnetAddress { get; set; }

        /// <summary>
        /// Gets or sets the contract address for the test network.
        /// </summary>
        public string? TestnetAddress { get; set; }

        /// <summary>
        /// Gets or sets the contract address for the main network.
        /// </summary>
        public string? MainnetAddress { get; set; }

        /// <summary>
        /// Gets or sets whether to fetch the manifest from the blockchain.
        /// Only applicable for deployed contracts.
        /// </summary>
        public bool FetchManifest { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to compile the referenced contract as a dependency.
        /// Only applicable for development contracts.
        /// </summary>
        public bool CompileAsDependency { get; set; } = true;

        /// <summary>
        /// Initializes a new ContractReferenceAttribute.
        /// </summary>
        /// <param name="identifier">The contract identifier</param>
        public ContractReferenceAttribute(string identifier)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        }
    }

    /// <summary>
    /// Specifies the type of contract reference.
    /// </summary>
    public enum ContractReferenceType
    {
        /// <summary>
        /// Automatically determine the reference type based on the identifier and context.
        /// </summary>
        Auto,

        /// <summary>
        /// Reference to a contract under development.
        /// </summary>
        Development,

        /// <summary>
        /// Reference to a deployed contract on the blockchain.
        /// </summary>
        Deployed
    }
}