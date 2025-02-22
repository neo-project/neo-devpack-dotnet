// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_StaticConstruct.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_StaticConstruct : SmartContract.Framework.SmartContract
    {
        static int a;
        //define and staticvar and initit with a runtime code.
        static Contract_StaticConstruct()
        {
            int b = 3;
            a = b + 1;
        }

        public static int TestStatic()
        {
            return a;
        }
    }
}
