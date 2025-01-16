// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_IOracle.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Interfaces;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_IOracle : SmartContract, IOracle
    {
        public void OnOracleResponse(string url, object userData, OracleResponseCode code, string result)
        {
            if (Runtime.CallingScriptHash != Oracle.Hash)
                throw new System.Exception("Unauthorized!");

            Runtime.Log("Oracle call!");
        }
    }
}
