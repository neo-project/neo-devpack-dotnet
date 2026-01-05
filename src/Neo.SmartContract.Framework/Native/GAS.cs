// Copyright (C) 2015-2026 The Neo Project.
//
// GAS.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using Neo.SmartContract.Framework.Attributes;
using System.Numerics;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xd2a4cff31913016155e38e474a2c06d08be276cf")]
    public class GAS
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }
        public static extern string Symbol { get; }
        public static extern byte Decimals { get; }

        /// <summary>
        /// Returns the total supply of the GAS token.
        /// </summary>
        public static extern BigInteger TotalSupply();

        /// <summary>
        /// Returns the balance of the specified account.
        /// <para>
        /// The execution will fail if 'account' is null.
        /// </para>
        /// </summary>
        public static extern BigInteger BalanceOf(UInt160 account);

        /// <summary>
        /// Transfers a specified amount of GAS from one account to another.
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
        /// True if the transfer is successful;
        /// It will return false if:
        ///   1. The 'from' is not the caller.
        ///   2. The 'from' hasn't witnessed the current transaction.
        ///   3. The 'from' balance is less than the 'amount' and amount is not zero.
        /// </returns>
        public static extern bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object? data = null);
    }
}
