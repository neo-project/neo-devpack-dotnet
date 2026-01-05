// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Event.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Event : SmartContract.Framework.SmartContract
    {
        public static int MyStaticVar1;
        public static bool MyStaticVar2;

        [DisplayName("transfer")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static event Action<byte[], byte[], BigInteger> Transferred;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public static void test()
        {
            MyStaticVar1 = 7;
            MyStaticVar2 = true;
            Transferred(new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 }, MyStaticVar1);
        }
    }
}
