// Copyright (C) 2015-2025 The Neo Project.
//
// Oracle.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Native;
using System.ComponentModel;

namespace Neo.SmartContract.Testing.Native;

public abstract class Oracle : SmartContract, TestingStandards.IVerificable
{
    #region Compiled data

    public static Manifest.ContractManifest Manifest { get; } =
        NativeContract.Oracle.GetContractState(ProtocolSettings.Default, uint.MaxValue).Manifest;

    #endregion

    #region Events

    public delegate void delOracleRequest(ulong Id, UInt160 RequestContract, string Url, string? Filter);

    [DisplayName("OracleRequest")]
#pragma warning disable CS0067 // Event is never used
    public event delOracleRequest? OnOracleRequest;
#pragma warning restore CS0067 // Event is never used

    public delegate void delOracleResponse(ulong Id, UInt256 OriginalTx);

    [DisplayName("OracleResponse")]
#pragma warning disable CS0067 // Event is never used
    public event delOracleResponse? OnOracleResponse;
#pragma warning restore CS0067 // Event is never used

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract long Price { [DisplayName("getPrice")] get; [DisplayName("setPrice")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract bool? Verify { [DisplayName("verify")] get; }

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("finish")]
    public abstract void Finish();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("request")]
    public abstract void Request(string url, string? filter, string callback, object? userData, ulong gasForResponse);

    #endregion

    #region Constructor for internal use only

    protected Oracle(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
