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

using Neo.SmartContract.Testing.Attributes;

namespace Neo.SmartContract.Testing.Native.Models
{
    public class Block
    {
        [FieldOrder(0)] public UInt256 Hash { get; set; } = default!;

        [FieldOrder(1)]
        public uint Version { get; set; }

        [FieldOrder(2)]
        public UInt256 PrevHash { get; set; } = default!;

        [FieldOrder(3)]
        public UInt256 MerkleRoot { get; set; } = default!;

        [FieldOrder(4)]
        public ulong Timestamp { get; set; }

        [FieldOrder(5)]
        public ulong Nonce { get; set; }

        [FieldOrder(6)]
        public uint Index { get; set; }

        [FieldOrder(7)]
        public byte PrimaryIndex { get; set; }

        [FieldOrder(8)]
        public UInt160 NextConsensus { get; set; } = default!;

        [FieldOrder(9)]
        public int TransactionsCount { get; set; }
    }
}
