using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_FieldKeyword(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_FieldKeyword"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""update"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":47,""safe"":false},{""name"":""recordLastPositiveSequence"",""parameters"":[{""name"":""first"",""type"":""Integer""},{""name"":""second"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":56,""safe"":false},{""name"":""recordLastNonZeroSequence"",""parameters"":[{""name"":""first"",""type"":""Integer""},{""name"":""second"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":68,""safe"":false},{""name"":""accumulateWallet"",""parameters"":[{""name"":""firstDeposit"",""type"":""Integer""},{""name"":""secondDeposit"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":80,""safe"":false},{""name"":""trackPositiveWallet"",""parameters"":[{""name"":""first"",""type"":""Boolean""},{""name"":""second"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":167,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":215,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy4xMC4yK2UyY2FjZTgzZTQ1YTNkZDJlY2U5YjRmNjNkMTU1YjMyNjkuLi4AAAAAAOBYQFcAAXgQuCYFeCIDEGBAWUBXAAF4ELcmBXgiA1lhQFpAVwABeLEkBVoiA3hiQFcAAXg0zzTLQFcAAng01nk00zTPQFcAAng02nk01zTTQFcBAgkQEsBwaHhQNAtoeVA0Bmg0PUBXAAJ5ELgmKngQznmeSsoUMiMD/////wAAAACRSgL///9/MhEDAAAAAAEAAACfIgV4EM5KeBBR0EVAVwABeBDOQFcBAgkQEsBwaHhQNAtoeVA0Bmg0FkBXAAJ4Ec4mBQgiA3lKeBFR0EVAVwABeBHOQFYDEGAQYRBiQIWQyrs=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECCRASwHBoeFA0C2h5UDQGaDQ9QA==
    /// INITSLOT 0102 [64 datoshi]
    /// PUSHF [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// CALL 0B [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// SWAP [2 datoshi]
    /// CALL 06 [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 3D [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("accumulateWallet")]
    public abstract BigInteger? AccumulateWallet(BigInteger? firstDeposit, BigInteger? secondDeposit);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeDTaeTTXNNNA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL DA [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// CALL D7 [512 datoshi]
    /// CALL D3 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("recordLastNonZeroSequence")]
    public abstract BigInteger? RecordLastNonZeroSequence(BigInteger? first, BigInteger? second);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeDTWeTTTNM9A
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL D6 [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// CALL D3 [512 datoshi]
    /// CALL CF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("recordLastPositiveSequence")]
    public abstract BigInteger? RecordLastPositiveSequence(BigInteger? first, BigInteger? second);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECCRASwHBoeFA0C2h5UDQGaDQWQA==
    /// INITSLOT 0102 [64 datoshi]
    /// PUSHF [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// CALL 0B [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// SWAP [2 datoshi]
    /// CALL 06 [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 16 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("trackPositiveWallet")]
    public abstract bool? TrackPositiveWallet(bool? first, bool? second);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDTPNMtA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL CF [512 datoshi]
    /// CALL CB [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("update")]
    public abstract BigInteger? Update(BigInteger? value);

    #endregion
}
