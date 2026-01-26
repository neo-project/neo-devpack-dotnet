// Copyright (C) 2015-2026 The Neo Project.
//
// CryptoLib.BLS12_381.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Native
{
    public static partial class CryptoLib
    {
        /// <summary>
        /// Serializes the Bls12_381(G1Affine, G1Projective, G2Affine, G2Projective, Gt) InteropInterface to a byte array.
        /// No CallFlags requirement.
        /// <para>
        /// The execution will fail if:
        ///  1. the 'data' is null.
        ///  2. the 'data' is not a valid Bls12_381 InteropInterface.
        /// </para>
        /// </summary>
        public static extern byte[] Bls12381Serialize(object data);

        /// <summary>
        /// Deserializes the byte array to a Bls12_381 InteropInterface(G1Affine, G2Affine, Gt).
        /// No CallFlags requirement.
        /// <para>
        /// The execution will fail if:
        ///  1. the 'data' is null.
        ///  2. the 'data' is not a valid Bls12_381 InteropInterface.
        /// </para>
        /// </summary>
        public static extern object Bls12381Deserialize(byte[] data);

        /// <summary>
        /// Checks if two Bls12_381 InteropInterface are equal.
        /// No CallFlags requirement.
        /// <para>
        /// The execution will fail if:
        ///  1. the 'x' or 'y' is null.
        ///  2. the 'x' or 'y' is not a valid Bls12_381 InteropInterface.
        ///  3. the 'x' and 'y' are not the same type.
        /// </para>
        /// </summary>
        public static extern bool Bls12381Equal(object x, object y);

        /// <summary>
        /// Adds two Bls12_381 InteropInterface.
        /// No CallFlags requirement.
        /// <para>
        /// If 'x' is G1Affine, 'y' must be G1Affine or G1Projective;
        /// If 'x' is G1Projective, 'y' must be G1Affine or G1Projective;
        /// If 'x' is G2Affine, 'y' must be G2Affine or G2Projective;
        /// If 'x' is G2Projective, 'y' must be G2Affine or G2Projective;
        /// If 'x' is Gt, 'y' must be Gt;
        /// </para>
        /// <para>
        /// The execution will fail if:
        ///  1. the 'x' or 'y' is null.
        ///  2. the 'x' or 'y' is not a valid Bls12_381 InteropInterface.
        ///  3. the type of 'y' mismatch with 'x'.
        /// </para>
        /// </summary>
        public static extern object Bls12381Add(object x, object y);

        /// <summary>
        /// Multiplies a Bls12_381 InteropInterface(G1Affine, G1Projective, G2Affine, G2Projective, Gt) with a scalar.
        /// No CallFlags requirement.
        /// The `multiplier` must be a 32-byte little-endian scalar,
        /// and if `multiplierIsNegative` is true, the scalar will take the negative value.
        /// <para>
        /// The execution will fail if:
        ///  1. the 'x' is null.
        ///  2. the 'x' is not a valid Bls12_381 InteropInterface.
        ///  3. the 'multiplier' is null.
        ///  4. the 'multiplier' is not a 32-byte little-endian scalar.
        /// </para>
        /// </summary>
        public static extern object Bls12381Mul(object x, byte[] multiplier, bool multiplierIsNegative);

        /// <summary>
        /// Performs a pairing operation on two Bls12_381 InteropInterface.
        /// The 'g1' must be G1Affine or G1Projective and the 'g2' must be G2Affine or G2Projective;
        /// No CallFlags requirement.
        /// <para>
        /// The execution will fail if:
        ///  1. the 'g1' or 'g2' is null.
        ///  2. the 'g1' is not G1Affine or G1Projective;
        ///  3. the 'g2' is not G2Affine or G2Projective.
        /// </para>
        /// </summary>
        public static extern object Bls12381Pairing(object g1, object g2);
    }
}
