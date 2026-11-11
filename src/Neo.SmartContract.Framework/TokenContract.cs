// Copyright (C) 2015-2026 The Neo Project.
//
// TokenContract.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Neo.SmartContract.Framework
{
    public abstract class TokenContract : SmartContract
    {
        protected const byte Prefix_TotalSupply = 0x00;
        protected const byte Prefix_Balance = 0x01;

        /// <summary>
        /// Builds a storage key with the same helper used by every access to this namespace.
        /// The key is typically a fixed prefix followed by the caller-provided key, which
        /// keeps reads and writes consistent and avoids storage key collisions.
        /// </summary>
        /// <remarks>
        /// This raw-key form is a performance optimization. The recommended way is to use
        /// <see cref="StorageMap"/> / <see cref="LocalStorageMap"/>, which is safer against storage injection.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static byte[] BalanceKey(ByteString key) => new byte[1] { Prefix_Balance }.Concat(key);

        public abstract string Symbol { [Safe] get; }

        public abstract byte Decimals { [Safe] get; }

        [Stored(Prefix_TotalSupply)]
        public static BigInteger TotalSupply { [Safe] get; protected set; }

        [Safe]
        public static BigInteger BalanceOf(UInt160 owner)
        {
            if (!owner.IsValid)
                throw new Exception("The argument \"owner\" is invalid.");
            return (BigInteger)Storage.Get(BalanceKey(owner))!;
        }

        protected static bool UpdateBalance(UInt160 owner, BigInteger increment)
        {
            var key = BalanceKey(owner);
            BigInteger balance = (BigInteger)Storage.Get(key)!;
            balance += increment;
            if (balance < 0)
                return false;
            if (balance.IsZero)
                Storage.Delete(key);
            else
                Storage.Put(key, balance);
            return true;
        }
    }
}
