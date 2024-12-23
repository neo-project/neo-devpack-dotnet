using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABpXAQMMAHBoeIvbKHBoeYvbKHBoeovbKHBoQN/RbZ0="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEDDABwaHiL2yhwaHmL2yhwaHqL2yhwaEA=
    /// INITSLOT 0103 [64 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteStringAddAssign")]
    public abstract byte[]? ByteStringAddAssign(byte[]? a, byte[]? b, string? c);

    #endregion
}
