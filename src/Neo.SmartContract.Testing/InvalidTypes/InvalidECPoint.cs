// Copyright (C) 2015-2025 The Neo Project.
//
// InvalidECPoint.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Cryptography.ECC;

namespace Neo.SmartContract.Testing.InvalidTypes
{
    public class InvalidECPoint
    {
        /// <summary>
        /// Null ECPoint
        /// </summary>
        public static readonly ECPoint? Null = null;

        /// <summary>
        /// This will be an invalid ECPoint (ByteString)
        /// </summary>
        public static readonly ECPoint InvalidLength = new();

        /// <summary>
        /// This will be an invalid ECPoint (Integer)
        /// </summary>
        public static readonly ECPoint InvalidType = new();
    }
}
