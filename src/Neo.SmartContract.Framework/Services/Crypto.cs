// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.Services
{
    public static class Crypto
    {
        [Syscall("System.Crypto.CheckSig")]
        public extern static bool CheckSig(Cryptography.ECC.ECPoint pubkey, ByteString signature);

        [Syscall("System.Crypto.CheckMultisig")]
        public extern static bool CheckMultisig(Cryptography.ECC.ECPoint[] pubkey, ByteString[] signature);

        [Syscall("System.Crypto.Bls12381Add")]
        public extern static byte[] Bls12381Add(byte[] gt1, byte[] gt2);

        [Syscall("System.Crypto.Bls12381Mul")]
        public extern static byte[] Bls12381Mul(byte[] gt, long mul);

        [Syscall("System.Crypto.Bls12381Pairing")]
        public extern static byte[] Bls12381Pairing(byte[] g1_bytes, byte[] g2_bytes);
    }
}
