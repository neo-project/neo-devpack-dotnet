// Copyright (C) 2015-2025 The Neo Project.
//
// CompilationContext.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.ContractInvocation;
using System;
using System.Collections.Generic;

namespace Neo.Compiler.ContractInvocation
{
    /// <summary>
    /// Provides context information during contract compilation.
    /// This class manages compilation state, errors, warnings, and blockchain interface.
    /// </summary>
    public class CompilationContext
    {
        private readonly List<CompilationMessage> _messages;
        private IBlockchainInterface? _blockchainInterface;

        /// <summary>
        /// Gets the project directory being compiled.
        /// </summary>
        public string? ProjectDirectory { get; set; }

        /// <summary>
        /// Gets the output directory for compiled artifacts.
        /// </summary>
        public string? OutputDirectory { get; set; }

        /// <summary>
        /// Gets the current network context.
        /// </summary>
        public NetworkContext NetworkContext { get; set; }

        /// <summary>
        /// Gets the compilation messages (errors, warnings, info).
        /// </summary>
        public IReadOnlyList<CompilationMessage> Messages => _messages;

        /// <summary>
        /// Gets or sets the RPC endpoint for blockchain queries.
        /// </summary>
        public string? RpcEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the timeout for blockchain queries in milliseconds.
        /// </summary>
        public int RpcTimeoutMs { get; set; } = 30000;

        /// <summary>
        /// Gets or sets whether to compile dependency contracts.
        /// </summary>
        public bool CompileDependencies { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to fetch manifests from the blockchain.
        /// </summary>
        public bool FetchManifests { get; set; } = true;

        /// <summary>
        /// Initializes a new CompilationContext.
        /// </summary>
        public CompilationContext()
        {
            _messages = new List<CompilationMessage>();
            NetworkContext = new NetworkContext();
        }

        /// <summary>
        /// Reports an error message.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="location">Optional location information</param>
        public void ReportError(string message, string? location = null)
        {
            _messages.Add(new CompilationMessage(CompilationMessageType.Error, message, location));
        }

        /// <summary>
        /// Reports a warning message.
        /// </summary>
        /// <param name="message">The warning message</param>
        /// <param name="location">Optional location information</param>
        public void ReportWarning(string message, string? location = null)
        {
            _messages.Add(new CompilationMessage(CompilationMessageType.Warning, message, location));
        }

        /// <summary>
        /// Reports an informational message.
        /// </summary>
        /// <param name="message">The informational message</param>
        /// <param name="location">Optional location information</param>
        public void ReportInfo(string message, string? location = null)
        {
            _messages.Add(new CompilationMessage(CompilationMessageType.Info, message, location));
        }

        /// <summary>
        /// Gets whether there are any error messages.
        /// </summary>
        public bool HasErrors => _messages.Exists(m => m.Type == CompilationMessageType.Error);

        /// <summary>
        /// Gets whether there are any warning messages.
        /// </summary>
        public bool HasWarnings => _messages.Exists(m => m.Type == CompilationMessageType.Warning);

        /// <summary>
        /// Gets the blockchain interface for this compilation context.
        /// </summary>
        /// <returns>The blockchain interface</returns>
        public IBlockchainInterface GetBlockchainInterface()
        {
            if (_blockchainInterface == null)
            {
                if (!string.IsNullOrEmpty(RpcEndpoint) && FetchManifests)
                {
                    _blockchainInterface = new CompilationTimeBlockchainInterface(RpcEndpoint, RpcTimeoutMs);
                }
                else
                {
                    // Use a null implementation that doesn't fetch from blockchain
                    _blockchainInterface = new NullBlockchainInterface();
                }
            }

            return _blockchainInterface;
        }

        /// <summary>
        /// Sets a custom blockchain interface.
        /// </summary>
        /// <param name="blockchainInterface">The blockchain interface to use</param>
        public void SetBlockchainInterface(IBlockchainInterface blockchainInterface)
        {
            _blockchainInterface = blockchainInterface;
        }

        /// <summary>
        /// Clears all compilation messages.
        /// </summary>
        public void ClearMessages()
        {
            _messages.Clear();
        }
    }

    /// <summary>
    /// Represents a compilation message (error, warning, or info).
    /// </summary>
    public class CompilationMessage
    {
        /// <summary>
        /// Gets the message type.
        /// </summary>
        public CompilationMessageType Type { get; }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the optional location information.
        /// </summary>
        public string? Location { get; }

        /// <summary>
        /// Gets the timestamp when the message was created.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Initializes a new CompilationMessage.
        /// </summary>
        /// <param name="type">The message type</param>
        /// <param name="message">The message text</param>
        /// <param name="location">Optional location information</param>
        public CompilationMessage(CompilationMessageType type, string message, string? location = null)
        {
            Type = type;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Location = location;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Returns a string representation of the message.
        /// </summary>
        public override string ToString()
        {
            var prefix = Type switch
            {
                CompilationMessageType.Error => "Error",
                CompilationMessageType.Warning => "Warning",
                CompilationMessageType.Info => "Info",
                _ => "Unknown"
            };

            var locationPart = string.IsNullOrEmpty(Location) ? "" : $" at {Location}";
            return $"{prefix}{locationPart}: {Message}";
        }
    }

    /// <summary>
    /// Specifies the type of compilation message.
    /// </summary>
    public enum CompilationMessageType
    {
        /// <summary>
        /// An error message that prevents successful compilation.
        /// </summary>
        Error,

        /// <summary>
        /// A warning message that indicates a potential issue.
        /// </summary>
        Warning,

        /// <summary>
        /// An informational message.
        /// </summary>
        Info
    }

    /// <summary>
    /// A null implementation of IBlockchainInterface that doesn't perform any blockchain queries.
    /// </summary>
    internal class NullBlockchainInterface : IBlockchainInterface
    {
        public bool ContractExists(UInt160 contractHash) => false;
        public uint GetBlockHeight() => 0;
        public ContractManifest? GetContractManifest(UInt160 contractHash) => null;
        public byte[]? GetContractNef(UInt160 contractHash) => null;
    }
}