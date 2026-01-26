// Copyright (C) 2015-2026 The Neo Project.
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
        /// Recovers the public key in compressed format from messageHash and signature, or null if cannot recover the public key.
        /// Available since HF_Echidna.
        /// </summary>
        /// <param name="messageHash">The 32-byte hash of the message that was signed. It cannot be null.</param>
        /// <param name="signature">
        /// The signature, either:
        /// - 65 bytes: r[32] + s[32] + v[1], where v is in [0..3] or [27..30], or
        /// - 64 bytes (EIP-2098 compact): r[32] + yParityAndS[32] (highest bit encodes parity).
        /// It cannot be null.
        /// </param>
        public static extern ByteString? RecoverSecp256K1(ByteString messageHash, ByteString signature);

        /// <summary>
        /// Computes the SHA-256 hash in bytes of the input value.
        /// <para>
        /// The execution will fail if 'value' is null.
        /// </para>
        /// </summary>
        public static extern ByteString Sha256(ByteString value);


        /// <summary>
        /// Computes the RIPEMD-160 hash in bytes of the input value.
        /// <para>
        /// The execution will fail if 'value' is null.
        /// </para>
        /// </summary>
        public static extern ByteString Ripemd160(ByteString value);

        /// <summary>
        /// Computes the Keccak-256 hash in bytes of the input value.
        /// Available since HF_Cockatrice.
        /// <para>
        /// The execution will fail if 'value' is null.
        /// </para>
        /// </summary>
        public static extern ByteString Keccak256(ByteString value);

        /// <summary>
        /// Computes the Murmur32 hash of the input value with the specified seed.
        /// The execution will fail if 'value' is null.
        /// </summary>
        /// <returns>The Murmur32 hash of the input value in bytes format.</returns>
        public static extern ByteString Murmur32(ByteString value, uint seed);

        [Obsolete("VerifyWithECDsa has changed its signature. Please, use a compatible version of VerifyWithECDsa with NamedCurveHash curveHash argument instead.")]
        public static extern bool VerifyWithECDsa(ByteString message, ECPoint pubkey, ByteString signature, NamedCurve curve);

        /// <summary>
        /// Verifies that a digital signature is appropriate for the provided key and message using the ECDSA algorithm.
        /// Available since HF_Cockatrice.
        /// <para>
        /// The execution will fail if:
        ///  1. 'message', 'pubkey', or 'signature' is null.
        ///  2. The 'pubkey' is not a valid ECPoint.
        ///  3. The 'curveHash' is not valid NamedCurveHash value.
        /// </para>
        /// </summary>
        public static extern bool VerifyWithECDsa(ByteString message, ECPoint pubkey, ByteString signature, NamedCurveHash curveHash);

        /// <summary>
        /// Verifies that a digital signature is appropriate for the provided key and message using the Ed25519 algorithm.
        /// Available since HF_Echidna.
        /// <para>
        /// The execution will fail if:
        ///  1. 'message', 'pubkey', or 'signature' is null.
        /// </para>
        /// </summary>
        public static extern bool VerifyWithEd25519(ByteString message, ByteString pubkey, ByteString signature);
    }
}
