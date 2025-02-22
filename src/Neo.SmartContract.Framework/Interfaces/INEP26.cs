// Copyright (C) 2015-2024 The Neo Project.
//
// INEP26.cs file belongs to the neo project and is free
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
/// Interface of method that indicate a contract receives NEP-11 Payment
/// </summary>
public interface INEP26
{
    /// <summary>
    /// Contracts should implement the <see cref="OnNEP11Payment"/> method
    /// to receive NFT (NEP11) tokens.
    /// </summary>
    /// <param name="from">The address of the payer</param>
    /// <param name="amount">The amount of token to be transferred</param>
    /// <param name="tokenId">The token id to be transferred</param>
    /// <param name="data">Additional payment description data</param>
    /// <remarks>
    /// This interface method is defined as non-static,
    /// but if you need it to be static, you can directly
    /// remove the interface and define it as a static method.
    /// Both static and non-static methods of smart contract interface works,
    /// they differ on how you process static field.
    /// </remarks>
    public void OnNEP11Payment(UInt160 from, BigInteger amount, string tokenId, object? data = null);
}
