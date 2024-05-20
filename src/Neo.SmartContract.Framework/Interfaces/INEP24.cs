// Copyright (C) 2015-2024 The Neo Project.
//
// INEP24.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;

namespace Neo.SmartContract.Framework.Interfaces;

/// <summary>
/// Interface of supporting royalty information for Non-Fungible Tokens (NFTs)
/// </summary>
public interface INep24
{
    /// <summary>
    /// This implements Royalty Standard: https://github.com/neo-project/proposals/pull/155/
    /// This method returns a map of NeoVM Array stack item with single or multi array, each array includes royaltyRecipient and royaltyAmount
    /// </summary>
    /// <param name="tokenId">tokenId</param>
    /// <param name="royaltyToken">royaltyToken hash for payment</param>
    /// <param name="salePrice">royaltyToken amount for payment</param>
    /// <returns>royaltyInfo</returns>
    /// <example>
    /// <code>
    ///  [Safe]
    /// public static Map<string, object>[] RoyaltyInfo(ByteString tokenId, UInt160 royaltyToken, BigInteger salePrice)
    /// {
    ///     ExecutionEngine.Assert(OwnerOf(tokenId) != null, "This TokenId doesn't exist!");
    ///     byte[] data = (byte[])Storage.Get(PrefixRoyalty + tokenId);
    ///     if (data == null)
    ///     {
    ///         var royaltyInfo = new Map<string, object>();
    ///         royaltyInfo["royaltyRecipient"] = InitialRecipient;
    ///         royaltyInfo["royaltyAmount"] = InitialFactor;
    ///         return new[] { royaltyInfo };
    ///     }
    ///     else
    ///     {
    ///         return (Map<string, object>[])StdLib.Deserialize((ByteString)data);
    ///     }
    /// }
    /// </code></example>
    public static abstract Map<string, object>[] RoyaltyInfo(
        ByteString tokenId,
        UInt160 royaltyToken,
        BigInteger salePrice);
}
