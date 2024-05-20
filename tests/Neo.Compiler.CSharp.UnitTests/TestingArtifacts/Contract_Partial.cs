using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Partial : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Partial"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test1"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""test2"",""parameters"":[],""returntype"":""Integer"",""offset"":4,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAAAIESICQBIiAkATf4IG"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test1")]
    public abstract BigInteger? Test1();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test2")]
    public abstract BigInteger? Test2();

    #endregion

    #region Constructor for internal use only

    protected Contract_Partial(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
