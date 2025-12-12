// Copyright (C) 2015-2025 The Neo Project.
//
// CryptoLib.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using System;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0x726cb6e0cd8628a1350a611384688911ab75f51b")]
    public static partial class CryptoLib
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }

        public static extern ByteString Sha256(ByteString value);

        public static extern ByteString Ripemd160(ByteString value);

        [Obsolete("Use Keccak256 instead.")]
        public static extern ByteString keccak256(ByteString value);

        public static ByteString Keccak256(ByteString value)
        {
#pragma warning disable CS0618
            return keccak256(value);
#pragma warning restore CS0618
        }

        public static extern ByteString Murmur32(ByteString value, uint seed);

        [Obsolete("VerifyWithECDsa has changed its signature. Please, use a compatible version of VerifyWithECDsa with NamedCurveHash curveHash argument instead.")]
        public static extern bool VerifyWithECDsa(ByteString message, ECPoint pubkey, ByteString signature, NamedCurve curve);

        public static extern bool VerifyWithECDsa(ByteString message, ECPoint pubkey, ByteString signature, NamedCurveHash curveHash);
    }
}
