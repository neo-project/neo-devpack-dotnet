using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Polymorphism : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Polymorphism"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":220,""safe"":false},{""name"":""test"",""parameters"":[],""returntype"":""String"",""offset"":235,""safe"":false},{""name"":""test2"",""parameters"":[],""returntype"":""String"",""offset"":250,""safe"":false},{""name"":""mul"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":265,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":178,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAAD9GAFXAAN5ep5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfIgJAVwABDARiYXNlIgJAVwADeXqgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnyICQFcAAXg0A0BXAAFAVwABDAR0ZXN0IgJAVwABeDQDQFcAAXg03kBXAAF4NJQMBS50ZXN0i9soIgJAVgMKg////woAAAAAEsBgCt7///8Kv////xLAYQpp////CgAAAAASwGJAwkpYz0o1mf///yMa////wkpZz0o1of///yOQ////wkpZz0o1kv///yOb////wkpaz0o1iv///yMw////zzQliQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("mul")]
    public abstract BigInteger? Mul(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test")]
    public abstract string? Test();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test2")]
    public abstract string? Test2();

    #endregion

    #region Constructor for internal use only

    protected Contract_Polymorphism(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
