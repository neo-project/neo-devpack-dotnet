using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Inline : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Inline"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testInline"",""parameters"":[{""name"":""method"",""type"":""String""}],""returntype"":""Integer"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAAD9agFXAQF4cGgMBmlubGluZZcmCBEjEQEAAGgMGmlubGluZV93aXRoX29uZV9wYXJhbWV0ZXJzlyYIEyPrAAAAaAwcaW5saW5lX3dpdGhfbXVsdGlfcGFyYW1ldGVyc5cmOBMSnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8jkwAAAGgMCm5vdF9pbmxpbmWXJgk1fwAAACJ3aAwebm90X2lubGluZV93aXRoX29uZV9wYXJhbWV0ZXJzlyYHEzRXIk5oDCBub3RfaW5saW5lX3dpdGhfbXVsdGlfcGFyYW1ldGVyc5cmCBMSNDIiImgMDWlubGluZV9uZXN0ZWSXJgY0UiINCCYGAGMiBGg6IgJAESICQFcAAXgiAkBXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfIgJAEyICQLgCiBg="));

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
