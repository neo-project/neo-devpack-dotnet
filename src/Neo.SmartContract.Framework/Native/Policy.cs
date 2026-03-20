// Copyright (C) 2015-2026 The Neo Project.
//
// Policy.cs file belongs to the neo project and is free
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
using System.Numerics;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xcc5e4edd9f5f8dba8bb65734541df7a1c081c67b")]
    public class Policy
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }

        /// <summary>
        /// Get the network fee per transaction byte in the unit of datoshi, 1 datoshi = 1e-8 GAS.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern long GetFeePerByte();

        /// <summary>
        /// Get the execution fee factor in the unit of datoshi, 1 datoshi = 1e-8 GAS.
        /// The system fee is the base-fee multiplied by the execution fee factor.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern uint GetExecFeeFactor();

        /// <summary>
        /// Get the storage price for per storage byte in the unit of datoshi, 1 datoshi = 1e-8 GAS.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern uint GetStoragePrice();

        /// <summary>
        /// Check if the account is blocked. True if the account is blocked, false otherwise.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// <para>
        /// The execution will fail if 'account' is null.
        /// </para>
        /// </summary>
        public static extern bool IsBlocked(UInt160 account);

        /// <summary>
        /// Returns an iterator of blocked accounts.
        /// Available since HF_Faun.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern Iterator GetBlockedAccounts();

        /// <summary>
        /// Get the attribute fee in the unit of datoshi, 1 datoshi = 1e-8 GAS.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// <para>
        /// The execution will fail if 'attributeType' is not a valid TransactionAttributeType.
        /// </para>
        /// </summary>
        public static extern uint GetAttributeFee(TransactionAttributeType attributeType);

        /// <summary>
        /// Returns an iterator over whitelisted fee contracts.
        /// Available since HF_Faun.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern Iterator GetWhitelistFeeContracts();
    }
}
