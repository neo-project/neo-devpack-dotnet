// Copyright (C) 2015-2025 The Neo Project.
//
// Ledger.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xda65b600f7124ce6c79950c1772a36403104f2be")]
    public class Ledger
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }

        /// <summary>
        /// Get the hash of the current block.
        /// </summary>
        public static extern UInt256 CurrentHash { get; }

        /// <summary>
        /// Get the index of the current block.
        /// </summary>
        public static extern uint CurrentIndex { get; }

        /// <summary>
        /// Get the block(without detailed transactions data) with the specified index,
        /// or null if the block is not found or (The height of the block - currentHeight) is greater than MaxTraceableBlocks.
        /// </summary>
        public static extern Block GetBlock(uint index);

        /// <summary>
        /// Get the block(without detailed transactions data) with the specified hash,
        /// or null if the block is not found or (The height of the block - currentHeight) is greater than MaxTraceableBlocks.
        /// <para>
        /// The execution will fail if 'hash' is null.
        /// </para>
        /// </summary>
        public static extern Block GetBlock(UInt256 hash);

        /// <summary>
        /// Get the transaction with the specified hash,
        /// or null if the transaction is not found or (The height of the block - currentHeight) is greater than MaxTraceableBlocks.
        /// <para>
        /// The execution will fail if 'hash' is null.
        /// </para>
        /// </summary>
        public static extern Transaction? GetTransaction(UInt256 hash);

        /// <summary>
        /// Get the 'txIndex'-th transaction from the block with the specified hash,
        /// or null if the block is not found or ('blockHash' - currentHash) is greater than MaxTraceableBlocks.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'blockHash' is null.
        ///  2. The 'txIndex' is less than zero or greater than the number of transactions in the block.
        /// </para>
        /// </summary>
        public static extern Transaction? GetTransactionFromBlock(UInt256 blockHash, int txIndex);

        /// <summary>
        /// Get the 'txIndex'-th transaction from the block at the specified height,
        /// or null if the block is not found or ('blockHeight' - currentHeight) is greater than MaxTraceableBlocks.
        /// <para>
        /// The execution will fail if the 'txIndex' is less than zero or greater than the number of transactions in the block.
        /// </para>
        /// </summary>
        public static extern Transaction? GetTransactionFromBlock(uint blockHeight, int txIndex);

        /// <summary>
        /// Returns the height of the block that contains the transaction with the specified hash,
        /// or -1 if the transaction is not found or (The height of the block - currentHeight) is greater than MaxTraceableBlocks.
        /// <para>
        /// The execution will fail if 'hash' is null.
        /// </para>
        /// </summary>
        public static extern int GetTransactionHeight(UInt256 hash);

        /// <summary>
        /// Returns the signers of the transaction with the specified hash,
        /// or null if the transaction is not found or (The height of the block - currentHeight) is greater than MaxTraceableBlocks.
        /// <para>
        /// The execution will fail if 'hash' is null.
        /// </para>
        /// </summary>
        public static extern Signer[]? GetTransactionSigners(UInt256 hash);

        /// <summary>
        /// Returns the VM state of the transaction with the specified hash,
        /// or VMState.NONE if the transaction is not found or (The height of the block - currentHeight) is greater than MaxTraceableBlocks.
        /// <para>
        /// The execution will fail if 'hash' is null.
        /// </para>
        /// </summary>
        public static extern VMState GetTransactionVMState(UInt256 hash);
    }
}
