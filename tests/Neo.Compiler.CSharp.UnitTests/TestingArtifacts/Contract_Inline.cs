using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Inline : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Inline"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testInline"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Integer"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3DAVcBAXhwaAwGaW5saW5llyYIESNCAQAAaAwaaW5saW5lX3dpdGhfb25lX3BhcmFtZXRlcnOXJggTIxwBAABoDBxpbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyZoExKeSgIAAACALgQiCkoC////fzJOBf////8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAkUoC////fzIkBQAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAnyOUAAAAaAwKbm90X2lubGluZZcmCTWAAAAAInpoDB5ub3RfaW5saW5lX3dpdGhfb25lX3BhcmFtZXRlcnOXJgcTNFYiUWgMIG5vdF9pbmxpbmVfd2l0aF9tdWx0aV9wYXJhbWV0ZXJzlyYIExI0LyIlaAwNaW5saW5lX25lc3RlZJcmCTV9AAAAIgsIJgYAYyIEaDpAEUBXAAF4QFcAAnh5nkoCAAAAgC4EIgpKAv///38yTgX/////AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJFKAv///38yJAUAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJ9AE0CqIXvA"));

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
