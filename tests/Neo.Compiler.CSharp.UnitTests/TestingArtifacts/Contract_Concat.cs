using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Concat(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Concat"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testStringAdd1"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""testStringAdd2"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""}],""returntype"":""String"",""offset"":15,""safe"":false},{""name"":""testStringAdd3"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""},{""name"":""c"",""type"":""String""}],""returntype"":""String"",""offset"":34,""safe"":false},{""name"":""testStringAdd4"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""},{""name"":""c"",""type"":""String""},{""name"":""d"",""type"":""String""}],""returntype"":""String"",""offset"":57,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFRXAAF4DAVoZWxsb4vbKEBXAAJ4eYvbKAwFaGVsbG+L2yhAVwADeHmL2yh6i9soDAVoZWxsb4vbKEBXAAR4eYvbKHqL2yh7i9soDAVoZWxsb4vbKEDmSoTR"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStringAdd1")]
    public abstract string? TestStringAdd1(string? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHDATA1
    // 000B : CAT
    // 000C : CONVERT
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStringAdd2")]
    public abstract string? TestStringAdd2(string? a, string? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : CAT
    // 0006 : CONVERT
    // 0008 : PUSHDATA1
    // 000F : CAT
    // 0010 : CONVERT
    // 0012 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStringAdd3")]
    public abstract string? TestStringAdd3(string? a, string? b, string? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : CAT
    // 0006 : CONVERT
    // 0008 : LDARG2
    // 0009 : CAT
    // 000A : CONVERT
    // 000C : PUSHDATA1
    // 0013 : CAT
    // 0014 : CONVERT
    // 0016 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStringAdd4")]
    public abstract string? TestStringAdd4(string? a, string? b, string? c, string? d);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : CAT
    // 0006 : CONVERT
    // 0008 : LDARG2
    // 0009 : CAT
    // 000A : CONVERT
    // 000C : LDARG3
    // 000D : CAT
    // 000E : CONVERT
    // 0010 : PUSHDATA1
    // 0017 : CAT
    // 0018 : CONVERT
    // 001A : RET

    #endregion

}
