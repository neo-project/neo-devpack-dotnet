// Copyright (C) 2015-2026 The Neo Project.
//
// INEP17.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using System.Numerics;

namespace Neo.SmartContract.Framework.Interfaces;

/// <summary>
/// Interface of the NEP-17 fungible token standard.
/// </summary>
[SupportedStandards(NepStandard.Nep17)]
public interface INEP17
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
    /// Transfers tokens between accounts.
    /// </summary>
    public static abstract bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object? data = null);
}
