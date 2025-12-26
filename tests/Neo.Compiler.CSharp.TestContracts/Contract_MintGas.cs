// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_MintGas.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_MintGas : SmartContract.Framework.SmartContract
    {
        /// <summary>
        /// Charges a fixed fee of 1 GAS
        /// </summary>
        public static void ChargeFixedFee()
        {
            Runtime.MintGas(100000000); // 1 GAS
        }

        /// <summary>
        /// Charges a dynamic fee based on data size
        /// </summary>
        public static void ChargeDynamicFee(byte[] data)
        {
            BigInteger fee = data.Length * 1000; // 0.00001 GAS per byte
            Runtime.MintGas(fee);
        }

        /// <summary>
        /// Charges a custom fee amount
        /// </summary>
        public static void ChargeCustomFee(BigInteger amount)
        {
            Runtime.MintGas(amount);
        }
    }
}
