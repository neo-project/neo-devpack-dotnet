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

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xcc5e4edd9f5f8dba8bb65734541df7a1c081c67b")]
    public class Policy
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern long GetFeePerByte();
        public static extern uint GetExecFeeFactor();
        public static extern uint GetStoragePrice();
        public static extern bool IsBlocked(UInt160 account);
    }
}
