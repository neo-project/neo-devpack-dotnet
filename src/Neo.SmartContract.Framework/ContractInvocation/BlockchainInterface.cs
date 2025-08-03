// Copyright (C) 2015-2025 The Neo Project.
//
// BlockchainInterface.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;

namespace Neo.SmartContract.Framework.ContractInvocation
{
    /// <summary>
    /// Interface for blockchain interactions during compilation.
    /// This interface abstracts blockchain queries for contract information.
    /// </summary>
    public interface IBlockchainInterface
    {
        /// <summary>
        /// Gets the contract manifest for the specified contract hash.
        /// </summary>
        /// <param name="contractHash">The contract hash</param>
        /// <returns>The contract manifest, or null if not found</returns>
        ContractManifest? GetContractManifest(UInt160 contractHash);

        /// <summary>
        /// Gets the contract NEF for the specified contract hash.
        /// </summary>
        /// <param name="contractHash">The contract hash</param>
        /// <returns>The contract NEF, or null if not found</returns>
        byte[]? GetContractNef(UInt160 contractHash);

        /// <summary>
        /// Checks if a contract exists at the specified hash.
        /// </summary>
        /// <param name="contractHash">The contract hash to check</param>
        /// <returns>True if the contract exists</returns>
        bool ContractExists(UInt160 contractHash);

        /// <summary>
        /// Gets the current block height.
        /// </summary>
        /// <returns>The current block height</returns>
        uint GetBlockHeight();
    }

    /// <summary>
    /// Provides blockchain interface functionality for compilation-time contract resolution.
    /// This class is used by the compiler to fetch contract information from the blockchain.
    /// </summary>
    public class CompilationTimeBlockchainInterface : IBlockchainInterface
    {
        private readonly string _rpcEndpoint;
        private readonly int _timeoutMs;

        /// <summary>
        /// Initializes a new CompilationTimeBlockchainInterface.
        /// </summary>
        /// <param name="rpcEndpoint">The RPC endpoint to connect to</param>
        /// <param name="timeoutMs">The timeout in milliseconds for RPC calls</param>
        public CompilationTimeBlockchainInterface(string rpcEndpoint, int timeoutMs = 30000)
        {
            _rpcEndpoint = rpcEndpoint ?? throw new ArgumentNullException(nameof(rpcEndpoint));
            _timeoutMs = timeoutMs;
        }

        /// <summary>
        /// Gets the contract manifest for the specified contract hash.
        /// </summary>
        /// <param name="contractHash">The contract hash</param>
        /// <returns>The contract manifest, or null if not found</returns>
        public ContractManifest? GetContractManifest(UInt160 contractHash)
        {
            // This method will be implemented to make RPC calls during compilation
            // For now, return null to indicate the contract information is not available
            return null;
        }

        /// <summary>
        /// Gets the contract NEF for the specified contract hash.
        /// </summary>
        /// <param name="contractHash">The contract hash</param>
        /// <returns>The contract NEF, or null if not found</returns>
        public byte[]? GetContractNef(UInt160 contractHash)
        {
            // This method will be implemented to make RPC calls during compilation
            // For now, return null to indicate the contract information is not available
            return null;
        }

        /// <summary>
        /// Checks if a contract exists at the specified hash.
        /// </summary>
        /// <param name="contractHash">The contract hash to check</param>
        /// <returns>True if the contract exists</returns>
        public bool ContractExists(UInt160 contractHash)
        {
            // This method will be implemented to make RPC calls during compilation
            // For now, return false to indicate the contract information is not available
            return false;
        }

        /// <summary>
        /// Gets the current block height.
        /// </summary>
        /// <returns>The current block height</returns>
        public uint GetBlockHeight()
        {
            // This method will be implemented to make RPC calls during compilation
            // For now, return 0 to indicate the information is not available
            return 0;
        }
    }

    /// <summary>
    /// Runtime blockchain interface that uses contract system calls.
    /// This class is used during actual contract execution.
    /// </summary>
    public class RuntimeBlockchainInterface : IBlockchainInterface
    {
        /// <summary>
        /// Gets the contract manifest for the specified contract hash.
        /// </summary>
        /// <param name="contractHash">The contract hash</param>
        /// <returns>The contract manifest, or null if not found</returns>
        public ContractManifest? GetContractManifest(UInt160 contractHash)
        {
            try
            {
                var contract = ContractManagement.GetContract(contractHash);
                return contract?.Manifest;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the contract NEF for the specified contract hash.
        /// </summary>
        /// <param name="contractHash">The contract hash</param>
        /// <returns>The contract NEF, or null if not found</returns>
        public byte[]? GetContractNef(UInt160 contractHash)
        {
            try
            {
                var contract = ContractManagement.GetContract(contractHash);
                return (byte[]?)contract?.Nef;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if a contract exists at the specified hash.
        /// </summary>
        /// <param name="contractHash">The contract hash to check</param>
        /// <returns>True if the contract exists</returns>
        public bool ContractExists(UInt160 contractHash)
        {
            try
            {
                var contract = ContractManagement.GetContract(contractHash);
                return contract != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the current block height.
        /// </summary>
        /// <returns>The current block height</returns>
        public uint GetBlockHeight()
        {
            return Ledger.CurrentIndex;
        }
    }
}