// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_SupportedStandards.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    // Both NEP-10 and NEP-5 are obsolete, but this is just a test contract
    [SupportedStandards("NEP-10", "NEP-5")]
    public class Contract_SupportedStandards : SmartContract
    {
        public static bool TestStandard()
        {
            return true;
        }
    }
}
