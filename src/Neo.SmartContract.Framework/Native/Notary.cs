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
using System.Numerics;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xc1e14f19c3e60d0b9244d06dd7ba9b113135ec3b")]
    public class Notary
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }
        public static extern bool LockDepositUntil(UInt160 addr, uint till);
        public static extern bool Withdraw(UInt160 from, UInt160 to);
        public static extern BigInteger BalanceOf(UInt160 acc);
        public static extern uint ExpirationOf(UInt160 acc);
        public static extern bool Verify(byte[] sig);
        public static extern uint GetMaxNotValidBeforeDelta();
        public static extern void SetMaxNotValidBeforeDelta(uint value);
    }
}
