// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Debug.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Debug : SmartContract.Framework.SmartContract
    {
        public static int TestElse()
        {
#if DEBUG
            Runtime.Debug("Debug compilation");
            return 1;
#else
            return 2;
#endif
        }

        public static int TestIf()
        {
            int ret = 2;
#if DEBUG
            ret = 1;
#endif
            return ret;
        }
    }
}
