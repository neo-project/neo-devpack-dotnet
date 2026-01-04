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

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xcc5e4edd9f5f8dba8bb65734541df7a1c081c67b")]
    public class Policy
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }

        /// <summary>
        /// Get the network fee per transaction byte in the unit of datoshi, 1 datoshi = 1e-8 GAS
        /// </summary>
        public static extern long GetFeePerByte();

        /// <summary>
        /// Get the execution fee factor.
        /// The system fee is the base-fee multiplied by the execution fee factor.
        /// </summary>
        public static extern uint GetExecFeeFactor();

        /// <summary>
        /// Get the storage price for per storage byte in the unit of datoshi, 1 datoshi = 1e-8 GAS
        /// </summary>
        public static extern uint GetStoragePrice();

        /// <summary>
        /// Check if the account is blocked. True if the account is blocked, false otherwise.
        /// <para>
        /// The execution will fail if 'account' is null.
        /// </para>
        /// </summary>
        public static extern bool IsBlocked(UInt160 account);

        /// <summary>
        /// Get the attribute fee in the unit of datoshi, 1 datoshi = 1e-8 GAS
        /// <para>
        /// The execution will fail if 'attributeType' is not a valid TransactionAttributeType.
        /// </para>
        /// </summary>
        public static extern uint GetAttributeFee(TransactionAttributeType attributeType);

        /// <summary>
        /// Set the attribute fee in the unit of datoshi, 1 datoshi = 1e-8 GAS.
        /// Only committee members can call this method.
        /// <para>
        /// The execution will fail if:
        ///  1. 'attributeType' is not a valid TransactionAttributeType.
        ///  2. 'value' is greater than MaxAttributeFee(the default value is 10_0000_0000).
        ///  3. The caller is not a committee member.
        /// </para>
        /// </summary>
        public static extern void SetAttributeFee(TransactionAttributeType attributeType, uint value);
    }
}
