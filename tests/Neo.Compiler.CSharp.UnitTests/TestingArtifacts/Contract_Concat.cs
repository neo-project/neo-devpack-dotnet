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
    /// Script: VwABeAxoZWxsb4vbKEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSHDATA1 68656C6C6F [8 datoshi]
    /// 0B : OpCode.CAT [2048 datoshi]
    /// 0C : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 0E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringAdd1")]
    public abstract string? TestStringAdd1(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmL2ygMaGVsbG+L2yhA
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.CAT [2048 datoshi]
    /// 06 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 08 : OpCode.PUSHDATA1 68656C6C6F [8 datoshi]
    /// 0F : OpCode.CAT [2048 datoshi]
    /// 10 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 12 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringAdd2")]
    public abstract string? TestStringAdd2(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHmL2yh6i9soDGhlbGxvi9soQA==
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.CAT [2048 datoshi]
    /// 06 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 08 : OpCode.LDARG2 [2 datoshi]
    /// 09 : OpCode.CAT [2048 datoshi]
    /// 0A : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 0C : OpCode.PUSHDATA1 68656C6C6F [8 datoshi]
    /// 13 : OpCode.CAT [2048 datoshi]
    /// 14 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringAdd3")]
    public abstract string? TestStringAdd3(string? a, string? b, string? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwAEeHmL2yh6i9soe4vbKAxoZWxsb4vbKEA=
    /// 00 : OpCode.INITSLOT 0004 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.CAT [2048 datoshi]
    /// 06 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 08 : OpCode.LDARG2 [2 datoshi]
    /// 09 : OpCode.CAT [2048 datoshi]
    /// 0A : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 0C : OpCode.LDARG3 [2 datoshi]
    /// 0D : OpCode.CAT [2048 datoshi]
    /// 0E : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 10 : OpCode.PUSHDATA1 68656C6C6F [8 datoshi]
    /// 17 : OpCode.CAT [2048 datoshi]
    /// 18 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 1A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringAdd4")]
    public abstract string? TestStringAdd4(string? a, string? b, string? c, string? d);

    #endregion
}
