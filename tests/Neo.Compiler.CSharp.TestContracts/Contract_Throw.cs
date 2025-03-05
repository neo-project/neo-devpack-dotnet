// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_Throw.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Throw : SmartContract.Framework.SmartContract
    {
        public static void TestMain(string[] args)
        {
            string first = args.Length >= 1 ? args[0] : throw new ArgumentException("Please supply at least one argument.");
        }
    }
}
