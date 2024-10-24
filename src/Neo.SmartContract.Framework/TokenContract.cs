// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
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
        protected static readonly ByteString Prefix_TotalSupply = "\x00";
        protected static readonly ByteString Prefix_Balance = "\x01";

        public abstract string Symbol { [Safe] get; }

        public abstract byte Decimals { [Safe] get; }

        public static BigInteger TotalSupply
        {
            [Safe]
            get
            {
                ByteString? stored = Storage.Get(Prefix_TotalSupply);
                return stored != null ? (BigInteger)stored : 0;
            }
            protected set
            {
                Storage.Put(Prefix_TotalSupply, value);
            }
        }

        [Safe]
        public static BigInteger BalanceOf(UInt160 owner)
        {
            if (owner is null || !owner.IsValid)
                throw new Exception("The argument \"owner\" is invalid.");
            ByteString? balanceStored = Storage.Get(Prefix_Balance + owner);
            return balanceStored != null ? (BigInteger)balanceStored : 0;
        }

        protected static bool UpdateBalance(UInt160 owner, BigInteger increment)
        {
            StorageContext currentContext = Storage.CurrentContext;
            ByteString ownerKey = Prefix_Balance + owner;
            ByteString? balanceStored = Storage.Get(currentContext, ownerKey);
            BigInteger balance = balanceStored != null ? (BigInteger)balanceStored : 0;
            balance += increment;
            if (balance < 0)
                return false;
            if (balance == 0)
                Storage.Delete(currentContext, ownerKey);
            else
                Storage.Put(currentContext, ownerKey, balance);
            return true;
        }
    }
}
