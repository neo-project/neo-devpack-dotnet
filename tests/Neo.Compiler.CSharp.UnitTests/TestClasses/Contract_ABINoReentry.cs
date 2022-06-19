// Copyright (C) 2015-2022 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses;

public class Contract_ABINoReentry : SmartContract.Framework.SmartContract
{
    static int s = 1;

    public static int UnitTest_001()
    {
        return 1;
    }

    [NoReentry]
    public static int UnitTest_002()
    {
        return 2;
    }

    public static int UnitTest_003()
    {
        return 3;
    }
}
