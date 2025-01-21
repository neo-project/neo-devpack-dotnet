// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_Overflow.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Overflow : SmartContract.Framework.SmartContract
    {
        public static int AddInt(int a, int b) => a + b;
        public static int MulInt(int a, int b) => a * b;
        public static uint AddUInt(uint a, uint b) => a + b;
        public static uint MulUInt(uint a, uint b) => a * b;
    }
}
