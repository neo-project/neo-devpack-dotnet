// Copyright (C) 2015-2024 The Neo Project.
//
// IWithdrawable.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#nullable enable

namespace Neo.SmartContract.Framework.Interfaces;

/// <summary>
/// Interface of method that indicate a contract can be withdrawn.
/// </summary>
public interface IWithdrawable
{
    /// <summary>
    /// The verification method. This must be called when withdrawing tokens from the contract.
    /// If the contract address is included in the transaction signature, this method verifies the signature.
    /// Example:
    /// <code>
    ///     public static bool Verify(params object [] args) => Runtime.CheckWitness(Owner);
    /// </code>
    /// <code>
    /// {
    ///   "name": "verify",
    ///   "safe": false,
    ///   "parameters": [],
    ///   "returntype": "bool"
    /// }
    /// </code>
    /// </summary>
    /// <remarks>Verify method can take arbitrary parameters</remarks>
    public static abstract bool Verify(params object[] args);
}
