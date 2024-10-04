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
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHDATA1 68656C6C6F
    /// 000B : OpCode.CAT
    /// 000C : OpCode.CONVERT 28
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("testStringAdd1")]
    public abstract string? TestStringAdd1(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.CAT
    /// 0006 : OpCode.CONVERT 28
    /// 0008 : OpCode.PUSHDATA1 68656C6C6F
    /// 000F : OpCode.CAT
    /// 0010 : OpCode.CONVERT 28
    /// 0012 : OpCode.RET
    /// </remarks>
    [DisplayName("testStringAdd2")]
    public abstract string? TestStringAdd2(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0003
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.CAT
    /// 0006 : OpCode.CONVERT 28
    /// 0008 : OpCode.LDARG2
    /// 0009 : OpCode.CAT
    /// 000A : OpCode.CONVERT 28
    /// 000C : OpCode.PUSHDATA1 68656C6C6F
    /// 0013 : OpCode.CAT
    /// 0014 : OpCode.CONVERT 28
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("testStringAdd3")]
    public abstract string? TestStringAdd3(string? a, string? b, string? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0004
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.CAT
    /// 0006 : OpCode.CONVERT 28
    /// 0008 : OpCode.LDARG2
    /// 0009 : OpCode.CAT
    /// 000A : OpCode.CONVERT 28
    /// 000C : OpCode.LDARG3
    /// 000D : OpCode.CAT
    /// 000E : OpCode.CONVERT 28
    /// 0010 : OpCode.PUSHDATA1 68656C6C6F
    /// 0017 : OpCode.CAT
    /// 0018 : OpCode.CONVERT 28
    /// 001A : OpCode.RET
    /// </remarks>
    [DisplayName("testStringAdd4")]
    public abstract string? TestStringAdd4(string? a, string? b, string? c, string? d);

    #endregion

}
