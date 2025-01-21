// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_OnDeployment2.cs file belongs to the neo project and is free
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
    public class Contract_OnDeployment2 : SmartContract.Framework.SmartContract
    {
        public static void _deploy(object data, bool update)
        {
            Runtime.Log("Deployed");
        }
    }
}
