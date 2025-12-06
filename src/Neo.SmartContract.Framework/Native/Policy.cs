// Copyright (C) 2015-2025 The Neo Project.
//
// Policy.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xcc5e4edd9f5f8dba8bb65734541df7a1c081c67b")]
    public class Policy
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }
        public static extern long GetFeePerByte();
        public static extern uint GetExecFeeFactor();
        public static extern uint GetStoragePrice();
        public static extern bool IsBlocked(UInt160 account);
        public static extern uint GetAttributeFee(TransactionAttributeType attributeType);
        public static extern void SetAttributeFee(TransactionAttributeType attributeType, uint value);
        public static extern void SetFeePerByte(long value);
        public static extern void SetExecFeeFactor(uint value);
        public static extern void SetStoragePrice(uint value);
        public static extern bool BlockAccount(UInt160 account);
        public static extern bool UnblockAccount(UInt160 account);
        public static extern uint GetMaxValidUntilBlockIncrement();
        public static extern void SetMaxValidUntilBlockIncrement(uint value);
        public static extern uint GetMaxTraceableBlocks();
        public static extern void SetMaxTraceableBlocks(uint value);
    }
}
