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
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSH7 [1 datoshi]
    /// 04 : PUSH6 [1 datoshi]
    /// 05 : PUSH5 [1 datoshi]
    /// 06 : PUSH4 [1 datoshi]
    /// 07 : PUSH3 [1 datoshi]
    /// 08 : PUSH2 [1 datoshi]
    /// 09 : PUSH1 [1 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : PUSH8 [1 datoshi]
    /// 0C : PACK [2048 datoshi]
    /// 0D : STLOC0 [2 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : PICKITEM [64 datoshi]
    /// 11 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("arrayElement")]
    public abstract BigInteger? ArrayElement(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmpQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : SHR [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("shiftRight")]
    public abstract BigInteger? ShiftRight(BigInteger? b, BigInteger? e);

    #endregion
}
