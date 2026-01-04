// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_SupportedStandard11Enum.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Interfaces;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [SupportedStandards(NepStandard.Nep11)]
    public class Contract_SupportedStandard11Enum : Nep11Token<Nep11TokenState>, INEP26
    {
        public static bool TestStandard()
        {
            return true;
        }

        public override string Symbol { [Safe] get; } = "EXAMPLE";

        public void OnNEP11Payment(UInt160 from, BigInteger amount, string tokenId, object? data = null)
        {
        }
    }
}
