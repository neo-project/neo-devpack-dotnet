// Copyright (C) 2015-2026 The Neo Project.
//
// NEO.cs file belongs to the neo project and is free
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
    [Contract("0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5")]
    public class NEO
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }

        /// <summary>
        /// Returns the symbol of the NEO token.
        /// </summary>
        public static extern string Symbol { get; }

        /// <summary>
        /// Returns the number of decimal places of the NEO token.
        /// </summary>
        public static extern byte Decimals { get; }

        /// <summary>
        /// Returns the total supply of the NEO token.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern BigInteger TotalSupply();

        /// <summary>
        /// Returns the balance of the specified account.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// <para>
        /// The execution will fail if 'account' is null.
        /// </para>
        /// </summary>
        public static extern BigInteger BalanceOf(UInt160 account);

        /// <summary>
        /// Transfers a specified amount of NEO from one account to another.
        /// CallFlags requirement: CallFlags.States | CallFlags.AllowCall | CallFlags.AllowNotify.
        /// <para>
        /// The execution will fail if:
        ///   1. The 'from', 'to' or 'amount' is null.
        ///   2. The 'amount' is less than zero.
        /// </para>
        /// <para>
        /// If the tansfer is successful, a 'Transfer' notification event will be emitted.
        /// </para>
        /// </summary>
        /// <returns>
        /// True if the transfer is successful.
        /// It will return false if:
        ///   1. The 'from' is not the caller.
        ///   2. The 'from' hasn't witnessed the current transaction.
        ///   3. The 'from' balance is less than the 'amount' and amount is not zero.
        /// </returns>
        public static extern bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object? data = null);

        /// <summary>
        /// Returns the gas per block.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern BigInteger GetGasPerBlock();

        /// <summary>
        /// Returns the register price.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern long GetRegisterPrice();

        /// <summary>
        /// Returns the unclaimed gas of the specified account.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// <para>
        /// The execution will fail:
        ///  1. if 'account' is null.
        ///  2. if 'end' is not the next block index of the last block index.
        /// </para>
        /// </summary>
        public static extern BigInteger UnclaimedGas(UInt160 account, uint end);

        /// <summary>
        /// Registers a candidate.
        /// CallFlags requirement: CallFlags.States before HF_Echidna, CallFlags.States | CallFlags.AllowNotify after HF_Echidna.
        /// <para>
        /// The execution will fail if 'pubkey' is null.
        /// </para>
        /// </summary>
        public static extern bool RegisterCandidate(ECPoint pubkey);

        /// <summary>
        /// Unregisters a candidate.
        /// CallFlags requirement: CallFlags.States before HF_Echidna, CallFlags.States | CallFlags.AllowNotify after HF_Echidna.
        /// <para>
        /// The execution will fail if 'pubkey' is null.
        /// </para>
        /// </summary>
        public static extern bool UnRegisterCandidate(ECPoint pubkey);

        /// <summary>
        /// Votes for a candidate. If 'voteTo' is null, it means unvoting.
        /// CallFlags requirement: CallFlags.States before HF_Echidna, CallFlags.States | CallFlags.AllowNotify after HF_Echidna.
        /// <para>
        /// The execution will fail if 'account' is null.
        /// </para>
        /// </summary>
        public static extern bool Vote(UInt160 account, ECPoint? voteTo);

        /// <summary>
        /// Unvotes for a candidate.
        /// CallFlags requirement: CallFlags.States before HF_Echidna, CallFlags.States | CallFlags.AllowNotify after HF_Echidna.
        /// <para>
        /// The execution will fail if 'account' is null.
        /// </para>
        /// </summary>
        public static bool Unvote(UInt160 account) => Vote(account, null);

        /// <summary>
        /// Returns the first 256 registered candidates.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern (ECPoint, BigInteger)[] GetCandidates();

        /// <summary>
        /// Returns all the registered candidates.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern Iterator<(ECPoint, BigInteger)> GetAllCandidates();

        /// <summary>
        /// Returns the votes of the specified candidate.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// <para>
        /// The execution will fail if 'pubkey' is null.
        /// </para>
        /// </summary>
        public static extern BigInteger GetCandidateVote(ECPoint pubkey);

        /// <summary>
        /// Returns the members of the committee.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern ECPoint[] GetCommittee();

        /// <summary>
        /// Returns the address of the committee.
        /// Available since HF_Cockatrice.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern UInt160 GetCommitteeAddress();

        /// <summary>
        /// Returns the validators of the next block.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern ECPoint[] GetNextBlockValidators();

        /// <summary>
        /// Returns the account state of the specified account, null if the account is not found.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// <para>
        /// The execution will fail if 'account' is null.
        /// </para>
        /// </summary>
        public static extern NeoAccountState GetAccountState(UInt160 account);
    }
}
