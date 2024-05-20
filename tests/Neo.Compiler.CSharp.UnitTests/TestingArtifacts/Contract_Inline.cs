using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Inline : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Inline"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testInline"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Any"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAAD9bAFXAQF4cGgMBmlubGluZZcmCBEjEwEAAGgMGmlubGluZV93aXRoX29uZV9wYXJhbWV0ZXJzlyYIEyPtAAAAaAwcaW5saW5lX3dpdGhfbXVsdGlfcGFyYW1ldGVyc5cmOBMSnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8jlQAAAGgMCm5vdF9pbmxpbmWXJgk1gQAAACJ5aAwebm90X2lubGluZV93aXRoX29uZV9wYXJhbWV0ZXJzlyYHEzRZIlBoDCBub3RfaW5saW5lX3dpdGhfbXVsdGlfcGFyYW1ldGVyc5cmCBMSNDQiJGgMDWlubGluZV9uZXN0ZWSXJgY0VCIPEdsgJgYAYyIEaDoiAkARIgJAVwABeCICQFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8iAkATIgJAVAwnFw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testInline")]
    public abstract object? TestInline(string? method);

    #endregion

    #region Constructor for internal use only

    protected Contract_Inline(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
