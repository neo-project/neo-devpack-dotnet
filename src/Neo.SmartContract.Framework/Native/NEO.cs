// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using Neo.Cryptography.ECC;
using Neo.SmartContract.Framework.Attributes;
using System.Numerics;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5")]
    public class NEO
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern string Symbol { get; }
        public static extern byte Decimals { get; }
        public static extern BigInteger TotalSupply();
        public static extern BigInteger BalanceOf(UInt160 account);
        public static extern bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data = null);

        public static extern BigInteger GetGasPerBlock();
        public static extern BigInteger UnclaimedGas(UInt160 account, uint end);

        public static extern bool RegisterCandidate(ECPoint pubkey);
        public static extern bool UnRegisterCandidate(ECPoint pubkey);
        public static extern bool Vote(UInt160 account, ECPoint voteTo);
        public static extern (ECPoint, BigInteger)[] GetCandidates();
        public static extern ECPoint[] GetCommittee();
        public static extern ECPoint[] GetNextBlockValidators();
        public static extern NeoAccountState GetAccountState(UInt160 account);
    }
}
