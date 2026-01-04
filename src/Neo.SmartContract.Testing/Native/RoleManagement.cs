// Copyright (C) 2015-2026 The Neo Project.
//
// RoleManagement.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Cryptography.ECC;
using Neo.SmartContract.Native;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native;

public abstract class RoleManagement(SmartContractInitialize initialize) : SmartContract(initialize)
{
    #region Compiled data

    public static Manifest.ContractManifest Manifest { get; } =
        NativeContract.RoleManagement.GetContractState(ProtocolSettings.Default, uint.MaxValue).Manifest;

    #endregion

    #region Events

    public delegate void delDesignation(BigInteger Role, BigInteger BlockIndex);

    [DisplayName("Designation")]
#pragma warning disable CS0067 // Event is never used
    public event delDesignation? OnDesignation;
#pragma warning restore CS0067 // Event is never used

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getDesignatedByRole")]
    public abstract ECPoint[] GetDesignatedByRole(Role role, uint index);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("designateAsRole")]
    public abstract void DesignateAsRole(Role role, ECPoint[] nodes);

    #endregion
}
