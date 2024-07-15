using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Inline : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Inline"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testInline"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Integer"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1qAVcBAXhwaAwGaW5saW5llyYIESMRAQAAaAwaaW5saW5lX3dpdGhfb25lX3BhcmFtZXRlcnOXJggTI+sAAABoDBxpbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyY4ExKeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnyOTAAAAaAwKbm90X2lubGluZZcmCTV/AAAAIndoDB5ub3RfaW5saW5lX3dpdGhfb25lX3BhcmFtZXRlcnOXJgcTNFciTmgMIG5vdF9pbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyYIExI0MiIiaAwNaW5saW5lX25lc3RlZJcmBjRSIg0IJgYAYyIEaDoiAkARIgJAVwABeCICQFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8iAkATIgJAfadMxw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testInline")]
    public abstract BigInteger? TestInline(string? method);

    #endregion

    #region Constructor for internal use only

    protected Contract_Inline(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
