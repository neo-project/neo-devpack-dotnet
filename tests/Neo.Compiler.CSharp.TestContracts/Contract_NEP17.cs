// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_NEP17.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.TestContracts
{
    [SupportedStandards(NepStandard.Nep17)]
    public class Contract_NEP17 : Nep17Token
    {
        [Safe]
        public override byte Decimals => 8;

        public override string Symbol { [Safe] get => "TEST"; }
    }
}
