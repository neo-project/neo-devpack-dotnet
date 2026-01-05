// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_StaticVarInit.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_StaticVarInit : SmartContract.Framework.SmartContract
    {
        //define and static var and init it with a runtime code.
        static UInt160 callscript = Runtime.ExecutingScriptHash;

        public static UInt160 StaticInit()
        {
            return TestStaticInit();
        }

        public static UInt160 DirectGet()
        {
            return Runtime.ExecutingScriptHash;
        }

        static UInt160 TestStaticInit()
        {
            return callscript;
        }
    }
}
