// Copyright (C) 2015-2026 The Neo Project.
//
// RoyaltyNep11Token.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Interfaces;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.SmartContract.Framework
{
    /// <summary>
    /// A NEP-11 non-fungible token base that also implements the NEP-24 royalty standard, the
    /// Neo analog of EIP-2981. A marketplace calls <see cref="RoyaltyInfo"/> to learn who should
    /// receive a royalty on a sale and how much.
    /// </summary>
    /// <remarks>
    /// Royalties are expressed in basis points (hundredths of a percent) of the sale price; the
    /// denominator is <see cref="RoyaltyDenominator"/> (10000 = 100%). A royalty can be set as a
    /// contract-wide default and overridden per token; a per-token royalty takes precedence.
    /// <para>
    /// The royalty setters are <c>protected</c> and perform no access control of their own — wrap
    /// them behind your contract's authorization (for example an owner or role check), exactly as
    /// you already gate <c>Mint</c>.
    /// </para>
    /// </remarks>
    [SupportedStandards(NepStandard.Nep11, NepStandard.Nep24)]
    public abstract class RoyaltyNep11Token<TokenState> : Nep11Token<TokenState>, INep24
        where TokenState : Nep11TokenState
    {
        protected const byte Prefix_DefaultRoyalty = 0x05;
        protected const byte Prefix_TokenRoyalty = 0x06;

        private static LocalStorageMap DefaultRoyaltyStorage => new(Prefix_DefaultRoyalty);

        /// <summary>
        /// The denominator royalties are measured against: 10000 basis points = 100% of the sale
        /// price.
        /// </summary>
        public const int RoyaltyDenominator = 10000;

        /// <summary>
        /// Returns the royalty payment information for selling <paramref name="tokenId"/> at
        /// <paramref name="salePrice"/>. Each returned map carries <c>royaltyRecipient</c>
        /// (the <see cref="UInt160"/> to pay) and <c>royaltyAmount</c> (the amount, computed as a
        /// fraction of the sale price). The amount is denominated in the same asset as
        /// <paramref name="salePrice"/>; <paramref name="royaltyToken"/> identifies that settlement
        /// asset and does not change the proportion. Returns an empty array when no royalty is
        /// configured for the token.
        /// </summary>
        [Safe]
        public static Map<string, object>[] RoyaltyInfo(ByteString tokenId, UInt160 royaltyToken, BigInteger salePrice)
        {
            // Reverts when the token does not exist.
            OwnerOf(tokenId);
            ExecutionEngine.Assert(salePrice >= 0, "The argument \"salePrice\" must be non-negative.");

            ByteString? data = new StorageMap(Prefix_TokenRoyalty)[tokenId]
                ?? DefaultRoyaltyStorage.Get(ByteString.Empty);
            if (data is null)
                return new Map<string, object>[0];

            object[] royalty = (object[])StdLib.Deserialize(data);
            UInt160 recipient = (UInt160)royalty[0];
            BigInteger basisPoints = (BigInteger)royalty[1];

            Map<string, object> info = new()
            {
                ["royaltyRecipient"] = recipient,
                ["royaltyAmount"] = salePrice * basisPoints / RoyaltyDenominator,
            };
            return new[] { info };
        }

        /// <summary>
        /// Sets the contract-wide default royalty paid to <paramref name="recipient"/>, expressed
        /// as <paramref name="basisPoints"/> of the sale price. No access control; gate this behind
        /// your own authorization.
        /// </summary>
        protected static void SetDefaultRoyalty(UInt160 recipient, BigInteger basisPoints)
        {
            DefaultRoyaltyStorage.Put(ByteString.Empty, SerializeRoyalty(recipient, basisPoints));
        }

        /// <summary>
        /// Removes the contract-wide default royalty. No access control; gate this behind your own
        /// authorization.
        /// </summary>
        protected static void DeleteDefaultRoyalty()
        {
            DefaultRoyaltyStorage.Delete(ByteString.Empty);
        }

        /// <summary>
        /// Sets a per-token royalty for <paramref name="tokenId"/>, overriding the default. No
        /// access control; gate this behind your own authorization.
        /// </summary>
        protected static void SetTokenRoyalty(ByteString tokenId, UInt160 recipient, BigInteger basisPoints)
        {
            // Reverts when the token does not exist.
            OwnerOf(tokenId);
            new StorageMap(Prefix_TokenRoyalty)[tokenId] = SerializeRoyalty(recipient, basisPoints);
        }

        /// <summary>
        /// Removes the per-token royalty for <paramref name="tokenId"/>, so it falls back to the
        /// default. No access control; gate this behind your own authorization.
        /// </summary>
        protected static void DeleteTokenRoyalty(ByteString tokenId)
        {
            new StorageMap(Prefix_TokenRoyalty).Delete(tokenId);
        }

        private static ByteString SerializeRoyalty(UInt160 recipient, BigInteger basisPoints)
        {
            ExecutionEngine.Assert(recipient.IsValidAndNotZero, "The royalty recipient is invalid.");
            ExecutionEngine.Assert(basisPoints >= 0 && basisPoints <= RoyaltyDenominator,
                "The royalty basis points must be between [0, 10000].");
            return StdLib.Serialize(new object[] { recipient, basisPoints });
        }
    }
}
