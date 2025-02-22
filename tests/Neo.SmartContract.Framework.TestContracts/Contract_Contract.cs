// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_Contract.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [ContractPermission(Permission.Any, Method.Any)]
    public class Contract_Contract : SmartContract
    {
        public static object Call(UInt160 scriptHash, string method, CallFlags flag, object[] args)
        {
            return Contract.Call(scriptHash, method, flag, args);
        }

#pragma warning disable CS8625
        public static object Create(byte[] nef, string manifest)
        {
            return ContractManagement.Deploy((ByteString)nef, manifest, null);
        }
#pragma warning restore CS8625

        public static int GetCallFlags()
        {
            return (int)Contract.GetCallFlags();
        }

        public static UInt160 CreateStandardAccount(ECPoint pubKey)
        {
            return Contract.CreateStandardAccount(pubKey);
        }
    }
}
