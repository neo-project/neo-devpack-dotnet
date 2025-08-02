// Copyright (C) 2015-2025 The Neo Project.
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

namespace Neo.SmartContract.Framework
{
    public abstract class TokenContract : SmartContract
    {
        protected const byte Prefix_TotalSupply = 0x00;
        protected const byte Prefix_Balance = 0x01;

        public abstract string Symbol { [Safe] get; }

        public abstract byte Decimals { [Safe] get; }

        [Stored(Prefix_TotalSupply)]
        public static BigInteger TotalSupply { [Safe] get; protected set; }

        [Safe]
        public static BigInteger BalanceOf(UInt160 owner)
        {
            if (!owner.IsValid)
                throw new Exception("The argument \"owner\" is invalid.");
            StorageMap balanceMap = new(Storage.CurrentContext, Prefix_Balance);
            return (BigInteger)balanceMap[owner]!;
        }

        protected static bool UpdateBalance(UInt160 owner, BigInteger increment)
        {
            StorageMap balanceMap = new(Storage.CurrentContext, Prefix_Balance);
            BigInteger balance = (BigInteger)balanceMap[owner]!;
            balance += increment;
            if (balance < 0)
                return false;
            if (balance.IsZero)
                balanceMap.Delete(owner);
            else
                balanceMap.Put(owner, balance);
            return true;
        }
    }
}
