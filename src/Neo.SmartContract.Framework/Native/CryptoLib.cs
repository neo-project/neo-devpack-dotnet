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

        /// <summary>
        /// Recovers the public key from a secp256k1 signature in bytes format.
        /// Available from HF_Echidna.
        /// </summary>
        /// <param name="messageHash">The hash of the message that was signed. It cannot be null.</param>
        /// <param name="signature">
        /// The signature in raw bytes format if it's length is 64.
        /// Otherwise, it's the 65-byte signature in format: r[32] + s[32] + v[1]. 64-bytes for eip-2098, where v must be 27 or 28.
        /// It cannot be null.
        /// </param>
        /// <returns>The recovered public key in compressed format, or null if recovery fails.</returns>
        public static extern ByteString RecoverSecp256K1(ByteString messageHash, ByteString signature);

        public static extern ByteString Sha256(ByteString value);

        public static extern ByteString Ripemd160(ByteString value);

        public static extern ByteString keccak256(ByteString value);

        public static extern ByteString Murmur32(ByteString value, uint seed);

        [Obsolete("VerifyWithECDsa has changed its signature. Please, use a compatible version of VerifyWithECDsa with NamedCurveHash curveHash argument instead.")]
        public static extern bool VerifyWithECDsa(ByteString message, ECPoint pubkey, ByteString signature, NamedCurve curve);

        public static extern bool VerifyWithECDsa(ByteString message, ECPoint pubkey, ByteString signature, NamedCurveHash curveHash);

        /// <summary>
        /// Verifies that a digital signature is appropriate for the provided key and message using the Ed25519 algorithm.
        /// Available from HF_Echidna.
        /// </summary>
        /// <param name="message">The signed message. It cannot be null.</param>
        /// <param name="pubkey">The 32-bytes length Ed25519 public key to be used. It cannot be null.</param>
        /// <param name="signature">The 64-bytes length signature to be verified. It cannot be null.</param>
        /// <returns>true if the signature is valid; otherwise, false.</returns>
        public static extern bool VerifyWithEd25519(ByteString message, ByteString pubkey, ByteString signature);
    }
}
