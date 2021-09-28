// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xfffdc93764dbaddd97c48f252a53ea4643faa3fd")]
    public class ContractManagement
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern long GetMinimumDeploymentFee();
        public static extern Contract GetContract(UInt160 hash);
        public static extern Contract Deploy(ByteString nefFile, string manifest);
        public static extern void Update(ByteString nefFile, string manifest);
        public static extern Contract Deploy(ByteString nefFile, string manifest, object data);
        public static extern void Update(ByteString nefFile, string manifest, object data);
        public static extern void Destroy();
    }
}
