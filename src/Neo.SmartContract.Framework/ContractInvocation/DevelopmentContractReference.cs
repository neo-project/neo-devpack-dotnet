// Copyright (C) 2015-2025 The Neo Project.
//
// DevelopmentContractReference.cs file belongs to the neo project and is free
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
    /// Represents a reference to a contract under development.
    /// This type of reference is resolved at compilation time based on project dependencies.
    /// </summary>
    public sealed class DevelopmentContractReference : IContractReference
    {
        /// <summary>
        /// Gets the project path or name of the contract under development.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Gets the resolved contract hash. For development contracts, this is null
        /// until the contract is compiled and has a deterministic hash.
        /// </summary>
        public UInt160? ResolvedHash { get; private set; }

        /// <summary>
        /// Gets the network context for this contract reference.
        /// </summary>
        public NetworkContext NetworkContext { get; }

        /// <summary>
        /// Gets a value indicating whether this contract reference is resolved.
        /// Development contracts are considered resolved when they have been compiled.
        /// </summary>
        public bool IsResolved => ResolvedHash != null;

        /// <summary>
        /// Gets the relative path to the contract project.
        /// </summary>
        public string ProjectPath { get; }

        /// <summary>
        /// Gets a value indicating whether this reference should be compiled
        /// as a dependency before the current contract.
        /// </summary>
        public bool CompileAsDependency { get; set; } = true;

        /// <summary>
        /// Initializes a new DevelopmentContractReference.
        /// </summary>
        /// <param name="identifier">The contract identifier (usually project name)</param>
        /// <param name="projectPath">The relative path to the contract project</param>
        /// <param name="networkContext">The network context for address resolution</param>
        public DevelopmentContractReference(string identifier, string projectPath, NetworkContext? networkContext = null)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            ProjectPath = projectPath ?? throw new ArgumentNullException(nameof(projectPath));
            NetworkContext = networkContext ?? new NetworkContext();
        }

        /// <summary>
        /// Resolves the contract hash after compilation.
        /// This method is called by the compilation system.
        /// </summary>
        /// <param name="hash">The computed contract hash</param>
        internal void ResolveHash(UInt160 hash)
        {
            ResolvedHash = hash ?? throw new ArgumentNullException(nameof(hash));
        }

        /// <summary>
        /// Creates a development contract reference from a project path.
        /// </summary>
        /// <param name="projectPath">The path to the contract project</param>
        /// <returns>A new DevelopmentContractReference</returns>
        public static DevelopmentContractReference FromProject(string projectPath)
        {
            if (string.IsNullOrEmpty(projectPath))
                throw new ArgumentNullException(nameof(projectPath));

            var identifier = System.IO.Path.GetFileNameWithoutExtension(projectPath);
            return new DevelopmentContractReference(identifier, projectPath);
        }
    }
}
