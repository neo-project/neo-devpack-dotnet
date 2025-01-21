// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_SequencePointInserter.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_SequencePointInserter : SmartContract
    {
        public static int test(int a)
        {
            if (a == 1) return 23;
            return 45;
        }
    }
}
