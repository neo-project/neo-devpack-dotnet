using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_UInt(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_UInt"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""isValidAndNotZeroUInt256"",""parameters"":[{""name"":""value"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""isValidAndNotZeroUInt160"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":27,""safe"":false},{""name"":""isZeroUInt256"",""parameters"":[{""name"":""value"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":54,""safe"":false},{""name"":""isZeroUInt160"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":61,""safe"":false},{""name"":""toAddress"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""String"",""offset"":68,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""base58CheckEncode""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrBFiYXNlNThDaGVja0VuY29kZQEAAQ8AAGxXAAF4NANAVwABeErZKFDKACCzqyQECUB4sUBXAAF4NANAVwABeErZKFDKABSzqyQECUB4sUBXAAF4sapAVwABeLGqQFcAAXg0A0BXAAFBTEmS3Hg0A0BXAQIRiEoQedBwaHiLcGjbKDcAAEBJ9S2Y").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQDQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : CALL 03 [512 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isValidAndNotZeroUInt160")]
    public abstract bool? IsValidAndNotZeroUInt160(UInt160? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQDQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : CALL 03 [512 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isValidAndNotZeroUInt256")]
    public abstract bool? IsValidAndNotZeroUInt256(UInt256? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeLGqQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : NZ [4 datoshi]
    /// 05 : NOT [4 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isZeroUInt160")]
    public abstract bool? IsZeroUInt160(UInt160? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeLGqQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : NZ [4 datoshi]
    /// 05 : NOT [4 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isZeroUInt256")]
    public abstract bool? IsZeroUInt256(UInt256? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQDQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : CALL 03 [512 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("toAddress")]
    public abstract string? ToAddress(UInt160? value);

    #endregion
}
