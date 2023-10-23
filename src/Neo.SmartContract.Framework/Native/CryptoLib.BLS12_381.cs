// Copyright (C) 2015-2023 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Native
{
    public static partial class CryptoLib
    {
        public static extern byte[] Bls12381Serialize(object data);

        public static extern object Bls12381Deserialize(byte[] data);

        public static extern bool Bls12381Equal(object x, object y);

        public static extern object Bls12381Add(object x, object y);

        public static extern object Bls12381Mul(object x, byte[] mul, bool neg);

        public static extern object Bls12381Pairing(object g1, object g2);
    }
}
