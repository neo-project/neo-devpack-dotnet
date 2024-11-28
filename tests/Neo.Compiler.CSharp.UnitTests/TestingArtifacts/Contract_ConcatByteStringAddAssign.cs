using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ConcatByteStringAddAssign(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ConcatByteStringAddAssign"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""byteStringAddAssign"",""parameters"":[{""name"":""a"",""type"":""ByteArray""},{""name"":""b"",""type"":""ByteArray""},{""name"":""c"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABpXAQMMAHBoeIvbKHBoeYvbKHBoeovbKHBoQN/RbZ0=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEDDABwaHiL2yhwaHmL2yhwaHqL2yhwaEA=
    /// 00 : INITSLOT 0103 [64 datoshi]
    /// 03 : PUSHDATA1 [8 datoshi]
    /// 05 : STLOC0 [2 datoshi]
    /// 06 : LDLOC0 [2 datoshi]
    /// 07 : LDARG0 [2 datoshi]
    /// 08 : CAT [2048 datoshi]
    /// 09 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0B : STLOC0 [2 datoshi]
    /// 0C : LDLOC0 [2 datoshi]
    /// 0D : LDARG1 [2 datoshi]
    /// 0E : CAT [2048 datoshi]
    /// 0F : CONVERT 28 'ByteString' [8192 datoshi]
    /// 11 : STLOC0 [2 datoshi]
    /// 12 : LDLOC0 [2 datoshi]
    /// 13 : LDARG2 [2 datoshi]
    /// 14 : CAT [2048 datoshi]
    /// 15 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 17 : STLOC0 [2 datoshi]
    /// 18 : LDLOC0 [2 datoshi]
    /// 19 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteStringAddAssign")]
    public abstract byte[]? ByteStringAddAssign(byte[]? a, byte[]? b, string? c);

    #endregion
}
