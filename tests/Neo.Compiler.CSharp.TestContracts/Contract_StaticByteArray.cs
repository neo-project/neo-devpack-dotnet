// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_StaticByteArray.cs file belongs to the neo project and is free
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
    public class Contract_StaticByteArray : SmartContract.Framework.SmartContract
    {
        [DisplayName("TestEvent")]
#pragma warning disable CS0067, CS0414 // Event is never used
        public static event Action<byte[]> OnEvent = default!;
#pragma warning restore CS0067, CS0414 // Event is never used

        static byte[] NeoToken = new byte[] { 0x89, 0x77, 0x20, 0xd8, 0xcd, 0x76, 0xf4, 0xf0, 0x0a, 0xbf, 0xa3, 0x7c, 0x0e, 0xdd, 0x88, 0x9c, 0x20, 0x8f, 0xde, 0x9b };

        public static byte[] TestStaticByteArray()
        {
            return NeoToken;
        }
    }
}
