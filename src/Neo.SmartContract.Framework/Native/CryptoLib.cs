// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0x726cb6e0cd8628a1350a611384688911ab75f51b")]
    public static class CryptoLib
    {
        public static extern UInt160 Hash { [ContractHash] get; }

        public static extern ByteString Sha256(ByteString value);

        public static extern ByteString ripemd160(ByteString value);

        public extern static bool VerifyWithECDsa(ByteString message, Cryptography.ECC.ECPoint pubkey, ByteString signature, NamedCurve curve);
    }
}
