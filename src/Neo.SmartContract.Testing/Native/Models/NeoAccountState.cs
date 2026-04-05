// Copyright (C) 2015-2026 The Neo Project.
//
// NeoAccountState.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Cryptography.ECC;
using Neo.SmartContract.Testing.Attributes;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native.Models
{
    public class NeoAccountState
    {
        [FieldOrder(0)]
        public BigInteger Balance { get; set; }

        [FieldOrder(1)]
        public uint BalanceHeight { get; set; }

        [FieldOrder(2)]
        public ECPoint? VoteTo { get; set; }

        [FieldOrder(3)]
        public BigInteger LastGasPerVote { get; set; }
    }
}
