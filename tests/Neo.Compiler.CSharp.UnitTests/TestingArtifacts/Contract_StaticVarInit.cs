using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_StaticVarInit : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_StaticVarInit"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""staticInit"",""parameters"":[],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""directGet"",""parameters"":[],""returntype"":""Any"",""offset"":9,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":17,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAAAcNAUiAkBYIgJAQfm04jgiAkBWAUH5tOI42zBgQNHDlMc="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("directGet")]
    public abstract object? DirectGet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("staticInit")]
    public abstract object? StaticInit();

    #endregion

    #region Constructor for internal use only

    protected Contract_StaticVarInit(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
