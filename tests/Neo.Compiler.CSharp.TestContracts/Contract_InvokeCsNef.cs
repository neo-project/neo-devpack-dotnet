// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_InvokeCsNef.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_InvokeCsNef : SmartContract.Framework.SmartContract
    {
        /// <summary>
        /// One return
        /// </summary>
        public static int returnInteger()
        {
            return 42;
        }

        public static int TestMain()
        {
            return 22;
        }

        public static string returnString()
        {
            return "hello world";
        }
    }
}
