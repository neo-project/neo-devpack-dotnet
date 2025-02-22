// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_Create.cs file belongs to the neo project and is free
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
    public class Contract_Create : SmartContract
    {
        public static string OldContract()
        {
            return ContractManagement.GetContract(Runtime.CallingScriptHash).Manifest.Name;
        }

        public static Contract GetContractById(int id)
        {
            return ContractManagement.GetContractById(id);
        }

        public static object GetContractHashes()
        {
            var iter = ContractManagement.GetContractHashes();
            iter.Next();
            return iter.Value.Item2;
        }

#pragma warning disable CS8625
        public static void Update(byte[] nef, string manifest)
        {
            ContractManagement.Update((ByteString)nef, manifest, null);
        }
#pragma warning restore CS8625

        public static void Destroy()
        {
            ContractManagement.Destroy();
        }

        public static int GetCallFlags()
        {
            return (int)Contract.GetCallFlags();
        }
    }
}
