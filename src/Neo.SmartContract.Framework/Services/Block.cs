// Copyright (C) 2015-2025 The Neo Project.
//
// Block.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Framework.Services
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class Block
    {
        public readonly UInt256 Hash;
        public readonly uint Version;
        public readonly UInt256 PrevHash;
        public readonly UInt256 MerkleRoot;
        public readonly ulong Timestamp;
        public readonly ulong Nonce;
        public readonly uint Index;
        public readonly byte PrimaryIndex;
        public readonly UInt160 NextConsensus;
        public readonly int TransactionsCount;
    }
#pragma warning restore CS8618
}
