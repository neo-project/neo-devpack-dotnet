// Copyright (C) 2015-2026 The Neo Project.
//
// ContractManagement.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Iterators;
using Neo.SmartContract.Native;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native;

public abstract class ContractManagement(SmartContractInitialize initialize) : SmartContract(initialize)
{
    #region Compiled data

    public static Manifest.ContractManifest Manifest { get; } =
        NativeContract.ContractManagement.GetContractState(ProtocolSettings.Default, uint.MaxValue).Manifest;

    #endregion

    #region Events

    public delegate void delDeploy(UInt160 Hash);

    [DisplayName("Deploy")]
#pragma warning disable CS0067 // Event is never used
    public event delDeploy? OnDeploy;
#pragma warning restore CS0067 // Event is never used

    public delegate void delDestroy(UInt160 Hash);

    [DisplayName("Destroy")]
#pragma warning disable CS0067 // Event is never used
    public event delDestroy? OnDestroy;
#pragma warning restore CS0067 // Event is never used

    public delegate void delUpdate(UInt160 Hash);

    [DisplayName("Update")]
#pragma warning disable CS0067 // Event is never used
    public event delUpdate? OnUpdate;
#pragma warning restore CS0067 // Event is never used

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract IIterator ContractHashes { [DisplayName("getContractHashes")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger MinimumDeploymentFee { [DisplayName("getMinimumDeploymentFee")] get; [DisplayName("setMinimumDeploymentFee")] set; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getContract")]
    public abstract ContractState? GetContract(UInt160 hash);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getContractById")]
    public abstract ContractState? GetContractById(int id);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("hasMethod")]
    public abstract bool HasMethod(UInt160 hash, string method, int pcount);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("deploy")]
    public abstract ContractState Deploy(byte[] nefFile, byte[] manifest);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("deploy")]
    public abstract ContractState Deploy(byte[] nefFile, byte[] manifest, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("destroy")]
    public abstract void Destroy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, byte[]? manifest);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, byte[]? manifest, object? data = null);

    #endregion
}
