// Copyright (C) 2015-2025 The Neo Project.
//
// Candidate.cs file belongs to the neo project and is free
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
    public class Candidate
    {
        /// <summary>
        /// Public key
        /// </summary>
        [FieldOrder(0)]
        public ECPoint? PublicKey { get; set; }

        /// <summary>
        /// Votes
        /// </summary>
        [FieldOrder(1)]
        public BigInteger Votes { get; set; }
    }
}
