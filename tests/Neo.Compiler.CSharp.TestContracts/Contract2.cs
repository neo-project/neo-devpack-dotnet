// Copyright (C) 2015-2024 The Neo Project.
//
// Contract2.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract2 : SmartContract.Framework.SmartContract
    {
        public static byte UnitTest_002(object arg1, object arg2)
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb[2];
        }
    }
}
