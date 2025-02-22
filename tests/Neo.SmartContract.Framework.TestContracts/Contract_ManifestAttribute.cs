// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_ManifestAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [ContractAuthor("core-dev")]
    [ContractEmail("dev@neo.org")]
    [ContractVersion("v3.6.3")]
    [ContractDescription("This is a test contract.")]
    [ManifestExtra("ExtraKey", "ExtraValue")]
    public class Contract_ManifestAttribute : SmartContract
    {
        [NoReentrant]
        public void reentrantTest(int value)
        {
            if (value == 0) return;
            if (value == 123)
            {
                reentrantTest(0);
            }
        }
    }
}
