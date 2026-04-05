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
        /// Get the execution fee factor.
        /// The system fee is the base-fee multiplied by the execution fee factor.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern uint GetExecFeeFactor();

        /// <summary>
        /// Get the execution fee factor in picoGAS, 1 picoGAS = 1e-12 GAS.
        /// Available since HF_Faun.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern BigInteger GetExecPicoFeeFactor();

        /// <summary>
        /// Get the storage price for per storage byte in the unit of datoshi, 1 datoshi = 1e-8 GAS.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern uint GetStoragePrice();

        /// <summary>
        /// Gets the block generation time in milliseconds.
        /// Available since HF_Echidna.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern uint GetMillisecondsPerBlock();

        /// <summary>
        /// Gets the maximum valid-until-block increment.
        /// Available since HF_Echidna.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern uint GetMaxValidUntilBlockIncrement();

        /// <summary>
        /// Gets the maximum traceable blocks value.
        /// Available since HF_Echidna.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern uint GetMaxTraceableBlocks();

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

        /// <summary>
        /// Blocks an account.
        /// CallFlags requirement: CallFlags.States before HF_Faun, CallFlags.All after HF_Faun.
        /// </summary>
        public static extern bool BlockAccount(UInt160 account);

        /// <summary>
        /// Sets the fee for the specified transaction attribute type.
        /// CallFlags requirement: CallFlags.States.
        /// </summary>
        public static extern void SetAttributeFee(TransactionAttributeType attributeType, uint value);

        /// <summary>
        /// Unblocks an account.
        /// CallFlags requirement: CallFlags.States.
        /// </summary>
        public static extern bool UnblockAccount(UInt160 account);

        /// <summary>
        /// Recovers NEP-17 funds from a blocked account to Treasury.
        /// Available since HF_Faun.
        /// CallFlags requirement: CallFlags.All.
        /// </summary>
        public static extern bool RecoverFund(UInt160 account, UInt160 token);

        /// <summary>
        /// Sets a whitelisted fixed-fee contract entry.
        /// Available since HF_Faun.
        /// CallFlags requirement: CallFlags.States | CallFlags.AllowNotify.
        /// </summary>
        public static extern void SetWhitelistFeeContract(UInt160 contractHash, string method, int argCount, long fixedFee);

        /// <summary>
        /// Removes a whitelisted fixed-fee contract entry.
        /// Available since HF_Faun.
        /// CallFlags requirement: CallFlags.States | CallFlags.AllowNotify.
        /// </summary>
        public static extern void RemoveWhitelistFeeContract(UInt160 contractHash, string method, int argCount);

        /// <summary>
        /// Sets the fee per transaction byte.
        /// CallFlags requirement: CallFlags.States.
        /// </summary>
        public static extern void SetFeePerByte(long value);

        /// <summary>
        /// Sets the execution fee factor.
        /// CallFlags requirement: CallFlags.States.
        /// </summary>
        public static extern void SetExecFeeFactor(ulong value);

        /// <summary>
        /// Sets the storage price.
        /// CallFlags requirement: CallFlags.States.
        /// </summary>
        public static extern void SetStoragePrice(uint value);

        /// <summary>
        /// Sets the block generation time in milliseconds.
        /// Available since HF_Echidna.
        /// CallFlags requirement: CallFlags.States | CallFlags.AllowNotify.
        /// </summary>
        public static extern void SetMillisecondsPerBlock(uint value);

        /// <summary>
        /// Sets the maximum valid-until-block increment.
        /// Available since HF_Echidna.
        /// CallFlags requirement: CallFlags.States.
        /// </summary>
        public static extern void SetMaxValidUntilBlockIncrement(uint value);

        /// <summary>
        /// Sets the maximum traceable blocks value.
        /// Available since HF_Echidna.
        /// CallFlags requirement: CallFlags.States.
        /// </summary>
        public static extern void SetMaxTraceableBlocks(uint value);
    }
}
