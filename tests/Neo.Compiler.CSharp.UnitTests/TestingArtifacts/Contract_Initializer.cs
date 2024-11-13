using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Initializer(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Initializer"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""sum1"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":31,""safe"":false},{""name"":""sum2"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":58,""safe"":false},{""name"":""anonymousObjectCreation"",""parameters"":[],""returntype"":""Void"",""offset"":91,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJRXAQAQEBLASjQLcGgQzmgRzp5AVwABeBAR0HgREtBAVwECEBASwEo07EoQeNBKEXnQcGgQzmgRzp5AVwECEBASwEo00XB4SmgQUdBFeUpoEVHQRWgQzmgRzp5AVwIADAVIZWxsbwBsEsBwaDQoQc/nR5YRDAVncmFwZRLAFAwFYXBwbGUSwBLAcWkQzjQHQc/nR5ZAp8G7Kw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADAVIZWxsbwBsEsBwaDQoQc/nR5YRDAVncmFwZRLAFAwFYXBwbGUSwBLAcWkQzjQHQc/nR5ZA
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 48656C6C6F 'Hello' [8 datoshi]
    /// 0A : OpCode.PUSHINT8 6C [1 datoshi]
    /// 0C : OpCode.PUSH2 [1 datoshi]
    /// 0D : OpCode.PACK [2048 datoshi]
    /// 0E : OpCode.STLOC0 [2 datoshi]
    /// 0F : OpCode.LDLOC0 [2 datoshi]
    /// 10 : OpCode.CALL 28 [512 datoshi]
    /// 12 : OpCode.SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 17 : OpCode.PUSH1 [1 datoshi]
    /// 18 : OpCode.PUSHDATA1 6772617065 'grape' [8 datoshi]
    /// 1F : OpCode.PUSH2 [1 datoshi]
    /// 20 : OpCode.PACK [2048 datoshi]
    /// 21 : OpCode.PUSH4 [1 datoshi]
    /// 22 : OpCode.PUSHDATA1 6170706C65 'apple' [8 datoshi]
    /// 29 : OpCode.PUSH2 [1 datoshi]
    /// 2A : OpCode.PACK [2048 datoshi]
    /// 2B : OpCode.PUSH2 [1 datoshi]
    /// 2C : OpCode.PACK [2048 datoshi]
    /// 2D : OpCode.STLOC1 [2 datoshi]
    /// 2E : OpCode.LDLOC1 [2 datoshi]
    /// 2F : OpCode.PUSH0 [1 datoshi]
    /// 30 : OpCode.PICKITEM [64 datoshi]
    /// 31 : OpCode.CALL 07 [512 datoshi]
    /// 33 : OpCode.SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 38 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("anonymousObjectCreation")]
    public abstract void AnonymousObjectCreation();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEBASwEo0C3BoEM5oEc6eQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.PUSH2 [1 datoshi]
    /// 06 : OpCode.PACK [2048 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.CALL 0B [512 datoshi]
    /// 0A : OpCode.STLOC0 [2 datoshi]
    /// 0B : OpCode.LDLOC0 [2 datoshi]
    /// 0C : OpCode.PUSH0 [1 datoshi]
    /// 0D : OpCode.PICKITEM [64 datoshi]
    /// 0E : OpCode.LDLOC0 [2 datoshi]
    /// 0F : OpCode.PUSH1 [1 datoshi]
    /// 10 : OpCode.PICKITEM [64 datoshi]
    /// 11 : OpCode.ADD [8 datoshi]
    /// 12 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEBASwEo07EoQeNBKEXnQcGgQzmgRzp5A
    /// 00 : OpCode.INITSLOT 0102 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.PUSH2 [1 datoshi]
    /// 06 : OpCode.PACK [2048 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.CALL EC [512 datoshi]
    /// 0A : OpCode.DUP [2 datoshi]
    /// 0B : OpCode.PUSH0 [1 datoshi]
    /// 0C : OpCode.LDARG0 [2 datoshi]
    /// 0D : OpCode.SETITEM [8192 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.PUSH1 [1 datoshi]
    /// 10 : OpCode.LDARG1 [2 datoshi]
    /// 11 : OpCode.SETITEM [8192 datoshi]
    /// 12 : OpCode.STLOC0 [2 datoshi]
    /// 13 : OpCode.LDLOC0 [2 datoshi]
    /// 14 : OpCode.PUSH0 [1 datoshi]
    /// 15 : OpCode.PICKITEM [64 datoshi]
    /// 16 : OpCode.LDLOC0 [2 datoshi]
    /// 17 : OpCode.PUSH1 [1 datoshi]
    /// 18 : OpCode.PICKITEM [64 datoshi]
    /// 19 : OpCode.ADD [8 datoshi]
    /// 1A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum1")]
    public abstract BigInteger? Sum1(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEBASwEo00XB4SmgQUdBFeUpoEVHQRWgQzmgRzp5A
    /// 00 : OpCode.INITSLOT 0102 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.PUSH2 [1 datoshi]
    /// 06 : OpCode.PACK [2048 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.CALL D1 [512 datoshi]
    /// 0A : OpCode.STLOC0 [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.LDLOC0 [2 datoshi]
    /// 0E : OpCode.PUSH0 [1 datoshi]
    /// 0F : OpCode.ROT [2 datoshi]
    /// 10 : OpCode.SETITEM [8192 datoshi]
    /// 11 : OpCode.DROP [2 datoshi]
    /// 12 : OpCode.LDARG1 [2 datoshi]
    /// 13 : OpCode.DUP [2 datoshi]
    /// 14 : OpCode.LDLOC0 [2 datoshi]
    /// 15 : OpCode.PUSH1 [1 datoshi]
    /// 16 : OpCode.ROT [2 datoshi]
    /// 17 : OpCode.SETITEM [8192 datoshi]
    /// 18 : OpCode.DROP [2 datoshi]
    /// 19 : OpCode.LDLOC0 [2 datoshi]
    /// 1A : OpCode.PUSH0 [1 datoshi]
    /// 1B : OpCode.PICKITEM [64 datoshi]
    /// 1C : OpCode.LDLOC0 [2 datoshi]
    /// 1D : OpCode.PUSH1 [1 datoshi]
    /// 1E : OpCode.PICKITEM [64 datoshi]
    /// 1F : OpCode.ADD [8 datoshi]
    /// 20 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum2")]
    public abstract BigInteger? Sum2(BigInteger? a, BigInteger? b);

    #endregion
}
