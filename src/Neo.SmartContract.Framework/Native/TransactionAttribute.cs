// Copyright (C) 2015-2025 The Neo Project.
//
// TransactionAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;

namespace Neo.SmartContract.Framework.Native
{
    /// <summary>
    /// Base type for transaction attributes exposed inside smart contracts.
    /// </summary>
    public abstract class TransactionAttribute
    {
        /// <summary>
        /// The attribute type.
        /// </summary>
        public TransactionAttributeType Type { get; set; }
    }

    /// <summary>
    /// Marks a transaction as high priority.
    /// </summary>
    public sealed class HighPriority : TransactionAttribute
    {
        public HighPriority()
        {
            Type = TransactionAttributeType.HighPriority;
        }
    }

    /// <summary>
    /// Declares that a transaction represents an oracle response.
    /// </summary>
    public sealed class OracleResponse : TransactionAttribute
    {
        public OracleResponse()
        {
            Type = TransactionAttributeType.OracleResponse;
        }

        /// <summary>
        /// The oracle request identifier.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// Oracle execution result code.
        /// </summary>
        public OracleResponseCode Code { get; set; }

        /// <summary>
        /// Payload returned by the oracle.
        /// </summary>
        public ByteString Result { get; set; } = null!;
    }

    /// <summary>
    /// Prevents a transaction from being accepted before a certain block height.
    /// </summary>
    public sealed class NotValidBefore : TransactionAttribute
    {
        public NotValidBefore()
        {
            Type = TransactionAttributeType.NotValidBefore;
        }

        /// <summary>
        /// Minimum block height at which the transaction becomes valid.
        /// </summary>
        public uint Height { get; set; }
    }

    /// <summary>
    /// Declares a mutual exclusion relationship between transactions.
    /// </summary>
    public sealed class Conflicts : TransactionAttribute
    {
        public Conflicts()
        {
            Type = TransactionAttributeType.Conflicts;
        }

        /// <summary>
        /// Hash of the conflicting transaction.
        /// </summary>
        public UInt256 Hash { get; set; } = null!;
    }

    /// <summary>
    /// Indicates that the transaction is assisted by the notary service.
    /// </summary>
    public sealed class NotaryAssisted : TransactionAttribute
    {
        public NotaryAssisted()
        {
            Type = TransactionAttributeType.NotaryAssisted;
        }

        /// <summary>
        /// Number of keys that are expected to participate in the assisted signing flow.
        /// </summary>
        public byte NKeys { get; set; }
    }
}
