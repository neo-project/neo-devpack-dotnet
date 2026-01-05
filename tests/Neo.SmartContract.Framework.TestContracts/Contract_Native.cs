// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Native.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Native;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Native : SmartContract
    {
        [DisplayName("NEO_Decimals")]
        public static int NEO_Decimals()
        {
            return NEO.Decimals;
        }

#pragma warning disable CS8625
        [DisplayName("NEO_Transfer")]
        public static bool NEO_Transfer(UInt160 from, UInt160 to, BigInteger amount)
        {
            return NEO.Transfer(from, to, amount, null);
        }
#pragma warning restore CS8625

        [DisplayName("NEO_BalanceOf")]
        public static BigInteger NEO_BalanceOf(UInt160 account)
        {
            return NEO.BalanceOf(account);
        }

        [DisplayName("NEO_GetAccountState")]
        public static object NEO_GetAccountState(UInt160 account)
        {
            return NEO.GetAccountState(account);
        }

        [DisplayName("NEO_GetGasPerBlock")]
        public static BigInteger NEO_GetGasPerBlock()
        {
            return NEO.GetGasPerBlock();
        }

        [DisplayName("NEO_UnclaimedGas")]
        public static BigInteger NEO_UnclaimedGas(UInt160 account, uint end)
        {
            return NEO.UnclaimedGas(account, end);
        }

        [DisplayName("NEO_RegisterCandidate")]
        public static bool NEO_RegisterCandidate(ECPoint pubkey)
        {
            return NEO.RegisterCandidate(pubkey);
        }

        [DisplayName("NEO_GetCandidates")]
        public static (ECPoint, BigInteger)[] NEO_GetCandidates()
        {
            return NEO.GetCandidates();
        }

        [DisplayName("GAS_Decimals")]
        public static int GAS_Decimals()
        {
            return GAS.Decimals;
        }

        [DisplayName("Policy_GetFeePerByte")]
        public static long Policy_GetFeePerByte()
        {
            return Policy.GetFeePerByte();
        }

        [DisplayName("Policy_IsBlocked")]
        public static bool Policy_IsBlocked(UInt160 account)
        {
            return Policy.IsBlocked(account);
        }
    }
}
