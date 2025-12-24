// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_ABIFee.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_ABIFee : SmartContract.Framework.SmartContract
    {
        // Method without fee
        public static int NoFeeMethod()
        {
            return 1;
        }

        // Method with fixed fee (1 GAS)
        [Fee(Amount = 100000000, Beneficiary = "NM7Aky765FG8NhhwtxjXRx7jEL1cnw7PBP")]
        public static int FixedFeeMethod()
        {
            return 2;
        }

        // Method with dynamic fee
        [Fee(Mode = FeeMode.Dynamic, Calculator = "0xb2a4cff31913016155e38e474a2c06d08be296cf", Beneficiary = "NM7Aky765FG8NhhwtxjXRx7jEL1cnw7PBP")]
        public static int DynamicFeeMethod()
        {
            return 3;
        }
    }
}
