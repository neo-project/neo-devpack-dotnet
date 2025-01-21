// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_NEP11.cs file belongs to the neo project and is free
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
    [SupportedStandards(NepStandard.Nep11)]
    public class Contract_NEP11 : Nep11Token<TokenState>
    {
        public override string Symbol { [Safe] get => "TEST"; }
    }

    public class TokenState : Nep11TokenState
    {

    }
}
