// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_CryptoAndEvents.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;

namespace Neo.Compiler.CSharp.TestContracts
{
    /// <summary>
    /// Covers the two output paths that a contract exposes to the outside world: the notification
    /// arguments that off-chain listeners read, and the signature checks that gate a call.
    /// </summary>
    public class Contract_CryptoAndEvents : SmartContract.Framework.SmartContract
    {
        public static event Action<int, string, bool, byte[], long> Mixed = null!;

        /// <summary>
        /// Raises an event with one argument of each supported kind and returns the sum.
        /// </summary>
        public static int EmitMixed(int a, int b)
        {
            Mixed(a, "text", true, new byte[] { 0x10, 0x20 }, (long)a * b);
            return a + b;
        }

        /// <summary>
        /// The overload that existing contracts use; it is obsolete in the framework but still has
        /// to verify signatures correctly.
        /// </summary>
#pragma warning disable CS0618
        public static bool VerifyR1Legacy(byte[] message, byte[] publicKey, byte[] signature)
        {
            return CryptoLib.VerifyWithECDsa((ByteString)message, (ECPoint)publicKey, (ByteString)signature, NamedCurve.secp256r1);
        }
#pragma warning restore CS0618

        public static bool VerifyR1(byte[] message, byte[] publicKey, byte[] signature)
        {
            return CryptoLib.VerifyWithECDsa((ByteString)message, (ECPoint)publicKey, (ByteString)signature, NamedCurveHash.secp256r1SHA256);
        }

        public static bool VerifyK1(byte[] message, byte[] publicKey, byte[] signature)
        {
            return CryptoLib.VerifyWithECDsa((ByteString)message, (ECPoint)publicKey, (ByteString)signature, NamedCurveHash.secp256k1SHA256);
        }
    }
}
