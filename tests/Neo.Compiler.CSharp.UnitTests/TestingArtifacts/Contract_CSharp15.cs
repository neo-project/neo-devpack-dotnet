using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_CSharp15(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_CSharp15"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""convertToMillimeters"",""parameters"":[{""name"":""meters"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""describeClosedGate"",""parameters"":[],""returntype"":""String"",""offset"":49,""safe"":false},{""name"":""describeGate"",""parameters"":[{""name"":""open"",""type"":""Boolean""}],""returntype"":""String"",""offset"":61,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":85,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH5XAAF4NANAVwABeAHoA6BKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AWBHASkrKnc4QzjZAVwEBWBHAcHgmBlkRwHBoSkrKnc4QzjZAVgIKEQAAABHAYAoVAAAAEcBhQFcAAQwGY2xvc2VkQFcAAQwEb3BlbkBAtjde").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQDQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("convertToMillimeters")]
    public abstract BigInteger? ConvertToMillimeters(BigInteger? meters);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WBHASkrKnc4QzjZA
    /// LDSFLD0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// DUP [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// DEC [4 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("describeClosedGate")]
    public abstract string? DescribeClosedGate();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBWBHAcHgmBlkRwHBoSkrKnc4QzjZA
    /// INITSLOT 0101 [64 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// DEC [4 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("describeGate")]
    public abstract string? DescribeGate(bool? open);

    #endregion
}
