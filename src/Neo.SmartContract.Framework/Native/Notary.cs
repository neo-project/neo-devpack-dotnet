// Copyright (C) 2015-2026 The Neo Project.
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
using System.Numerics;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xc1e14f19c3e60d0b9244d06dd7ba9b113135ec3b")]
    public class Notary
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }

        /// <summary>
        /// Locks a deposit until the specified block height.
        /// The deposit is created by transferring GAS to the Notary contract.
        /// <para>
        /// The execution will fail if account is null.
        /// </para>
        /// </summary>
        /// <param name="account">The account to lock the deposit.</param>
        /// <param name="till">The block height to lock the deposit until.</param>
        /// <returns>
        /// True if the `till` value is updated successfully, false if
        ///  1. the account is not witness;
        ///  2. the deposit is not found;
        ///  3. The `till` is less than the current block height + 2 or less than the previous lock height;
        /// </returns>
        public static extern bool LockDepositUntil(UInt160 account, uint till);

        /// <summary>
        /// Withdraw all deposited GAS for "from" address to "to" address. If "to" address is not specified, then "from" will be used as a sender.
        /// <para>
        /// The execution will fail if:
        ///  1. the 'from' is null;
        ///  2. the GAS transfer calling fails.
        /// </para>
        /// </summary>
        /// <returns>
        /// True if the withdrawal is successful, false if
        ///  1. the 'from' is not witness;
        ///  2. the deposit is not found;
        ///  3. The current block height is less than the deposit's lock height.
        /// </returns>
        public static extern bool Withdraw(UInt160 from, UInt160? to = null);

        /// <summary>
        /// ExpirationOf returns deposit lock height for specified address.
        /// <para>
        /// The execution will fail if the 'account' is null.
        /// </para>
        /// </summary>
        public static extern BigInteger BalanceOf(UInt160 account);

        /// <summary>
        /// ExpirationOf returns deposit lock height for specified address.
        /// <para>
        /// The execution will fail if the 'account' is null.
        /// </para>
        /// </summary>
        public static extern uint ExpirationOf(UInt160 account);

        /// <summary>
        /// Verify checks whether the transaction is signed by one of the notaries and
        /// ensures whether deposited amount of GAS is enough to pay the actual sender's fee.
        /// </summary>
        /// <returns>
        /// It returns false if:
        ///  1. The 'signature' is not valid;
        ///  2. The calling source is not a transaction or no NotaryAssisted attributed.
        ///  3. If one signer of the transaction is the Notary contract, but the WitnessScope is not None.
        ///  4. if the sender is the Notary contract, but the deposited GAS of the second signer is not enough to pay the fee.
        ///  5. The transaction is not signed by one of the notaries.
        /// </returns>
        public static extern bool Verify(byte[] signature);

        /// <summary>
        /// returns the maximum NotValidBefore delta
        /// </summary>
        public static extern uint GetMaxNotValidBeforeDelta();

        /// <summary>
        /// SetMaxNotValidBeforeDelta is Notary contract method and sets the maximum NotValidBefore delta.
        /// Only the committee can call this method.
        /// <para>
        /// The execution will fail if:
        ///  1. the 'value' is greater than MaxValidUntilBlockIncrement / 2;
        ///  2. the 'value' is less than the number of validators;
        ///  3. Not the committee.
        /// </para>
        /// </summary>
        public static extern void SetMaxNotValidBeforeDelta(uint value);
    }
}
