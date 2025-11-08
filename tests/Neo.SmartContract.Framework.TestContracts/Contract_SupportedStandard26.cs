// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_SupportedStandard26.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using System.ComponentModel;
using System.Numerics;
using Neo.SmartContract.Framework.Interfaces;

namespace Neo.SmartContract.Framework.TestContracts
{
    [DisplayName(nameof(Contract_SupportedStandard26))]
    [ContractDescription("<Description Here>")]
    [ContractAuthor("<Your Name Or Company Here>", "<Your Public Email Here>")]
    [ContractVersion("<Version String Here>")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(NepStandard.Nep26)]
    public class Contract_SupportedStandard26 : SmartContract, INEP26, Neo.SmartContract.Framework.Interfaces.INEP30
    {
        public static bool Verify(params object[] args) => true;

        public void OnNEP11Payment(UInt160 from, BigInteger amount, string tokenId, object? data = null)
        {
        }
    }
}
