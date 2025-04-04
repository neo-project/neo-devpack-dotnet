// Copyright (C) 2015-2025 The Neo Project.
//
// Ledger.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xda65b600f7124ce6c79950c1772a36403104f2be")]
    public class Ledger
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }
        public static extern UInt256 CurrentHash { get; }
        public static extern uint CurrentIndex { get; }
        public static extern Block GetBlock(uint index);
        public static extern Block GetBlock(UInt256 hash);
        public static extern Transaction GetTransaction(UInt256 hash);
        public static extern Transaction GetTransactionFromBlock(UInt256 blockHash, int txIndex);
        public static extern Transaction GetTransactionFromBlock(uint blockHeight, int txIndex);
        public static extern int GetTransactionHeight(UInt256 hash);
        public static extern Signer[] GetTransactionSigners(UInt256 hash);
        public static extern VMState GetTransactionVMState(UInt256 hash);
    }
}
