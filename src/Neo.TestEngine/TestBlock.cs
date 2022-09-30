// Copyright (C) 2015-2022 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Native;

namespace Neo.TestingEngine
{
    public class TestBlock
    {
        internal Block Block { get; }
        internal UInt256? Hash { get; }
        internal TransactionState[] Transactions { get; }

        public TestBlock(Block block, TransactionState[] txs, UInt256? blockHash = null)
        {
            Block = block;
            Hash = blockHash;
            Transactions = txs;
        }
    }
}
