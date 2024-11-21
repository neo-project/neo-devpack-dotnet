using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_AllowOverflow(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_AllowOverflow"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""arrayElement"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""shiftRight"",""parameters"":[{""name"":""b"",""type"":""Integer""},{""name"":""e"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":18,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0e26a6a9b6f37a54d5666aaa2efb71dc75abfdfa"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABlXAQEXFhUUExIREBjAcGh4zkBXAAJ4ealAIalIfw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBFxYVFBMSERAYwHBoeM5A
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSH7 [1 datoshi]
    /// 04 : OpCode.PUSH6 [1 datoshi]
    /// 05 : OpCode.PUSH5 [1 datoshi]
    /// 06 : OpCode.PUSH4 [1 datoshi]
    /// 07 : OpCode.PUSH3 [1 datoshi]
    /// 08 : OpCode.PUSH2 [1 datoshi]
    /// 09 : OpCode.PUSH1 [1 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.PUSH8 [1 datoshi]
    /// 0C : OpCode.PACK [2048 datoshi]
    /// 0D : OpCode.STLOC0 [2 datoshi]
    /// 0E : OpCode.LDLOC0 [2 datoshi]
    /// 0F : OpCode.LDARG0 [2 datoshi]
    /// 10 : OpCode.PICKITEM [64 datoshi]
    /// 11 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("arrayElement")]
    public abstract BigInteger? ArrayElement(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmpQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.SHR [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("shiftRight")]
    public abstract BigInteger? ShiftRight(BigInteger? b, BigInteger? e);

    #endregion
}
