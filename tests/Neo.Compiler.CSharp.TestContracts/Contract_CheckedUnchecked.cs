// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_CheckedUnchecked.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_CheckedUnchecked : SmartContract.Framework.SmartContract
    {
        public static int AddChecked(int a, int b)
        {
            return checked(a + b);
        }

        public static int AddUnchecked(int a, int b)
        {
            return unchecked(a + b);
        }

        public static uint CastChecked(int a)
        {
            return checked((uint)a);
        }

        public static uint CastUnchecked(int a)
        {
            return unchecked((uint)a);
        }
    }
}
