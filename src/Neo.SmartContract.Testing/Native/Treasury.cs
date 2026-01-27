// Copyright (C) 2015-2026 The Neo Project.
//
// Treasury.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Native;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native;

public abstract class Treasury(SmartContractInitialize initialize) : SmartContract(initialize)
{
    #region Compiled data

    public static Manifest.ContractManifest Manifest { get; } =
        NativeContract.Treasury.GetContractState(ProtocolSettings.Default, uint.MaxValue).Manifest;

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("verify")]
    public abstract bool? Verify();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("onNEP17Payment")]
    public abstract void OnNEP17Payment(UInt160? from, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("onNEP11Payment")]
    public abstract void OnNEP11Payment(UInt160? from, BigInteger? amount, byte[]? tokenId, object? data = null);

    #endregion
}
