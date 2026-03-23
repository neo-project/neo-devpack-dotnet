// Copyright (C) 2015-2026 The Neo Project.
//
// INEP11.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.SmartContract.Framework.Interfaces;

/// <summary>
/// Interface of the NEP-11 non-fungible token standard.
/// </summary>
[SupportedStandards(NepStandard.Nep11)]
public interface INEP11
{
    /// <summary>
    /// Contract symbol.
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// Contract decimals.
    /// </summary>
    public byte Decimals { get; }

    /// <summary>
    /// Returns the total token supply.
    /// </summary>
    public static abstract BigInteger TotalSupply { get; }

    /// <summary>
    /// Returns the token balance for an account.
    /// </summary>
    public static abstract BigInteger BalanceOf(UInt160 owner);

    /// <summary>
    /// Returns the token owner.
    /// </summary>
    public static abstract UInt160 OwnerOf(ByteString tokenId);

    /// <summary>
    /// Returns the token properties.
    /// </summary>
    public Map<string, object> Properties(ByteString tokenId);

    /// <summary>
    /// Enumerates all token IDs.
    /// </summary>
    public static abstract Iterator Tokens();

    /// <summary>
    /// Enumerates the token IDs owned by an account.
    /// </summary>
    public static abstract Iterator TokensOf(UInt160 owner);

    /// <summary>
    /// Transfers a token.
    /// </summary>
    public static abstract bool Transfer(UInt160 to, ByteString tokenId, object? data = null);
}
