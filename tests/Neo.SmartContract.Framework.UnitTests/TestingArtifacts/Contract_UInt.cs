using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrBFiYXNlNThDaGVja0VuY29kZQEAAQ8AAGxXAAF4NANAVwABeErZKFDKACCzqyQECUB4sUBXAAF4NANAVwABeErZKFDKABSzqyQECUB4sUBXAAF4sapAVwABeLGqQFcAAXg0A0BXAAFBTEmS3Hg0A0BXAQIRiEoQedBwaHiLcGjbKDcAAEBJ9S2Y"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQDQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 03
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("isValidAndNotZeroUInt160")]
    public abstract bool? IsValidAndNotZeroUInt160(UInt160? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQDQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 03
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("isValidAndNotZeroUInt256")]
    public abstract bool? IsValidAndNotZeroUInt256(UInt256? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeLGqQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.NZ
    /// 05 : OpCode.NOT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("isZeroUInt160")]
    public abstract bool? IsZeroUInt160(UInt160? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeLGqQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.NZ
    /// 05 : OpCode.NOT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("isZeroUInt256")]
    public abstract bool? IsZeroUInt256(UInt256? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEYhKEHnQcGh4i3Bo2yg3AABA
    /// 00 : OpCode.INITSLOT 0102
    /// 03 : OpCode.PUSH1
    /// 04 : OpCode.NEWBUFFER
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSH0
    /// 07 : OpCode.LDARG1
    /// 08 : OpCode.SETITEM
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.LDLOC0
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.CAT
    /// 0D : OpCode.STLOC0
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.CONVERT 28
    /// 11 : OpCode.CALLT 0000
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("toAddress")]
    public abstract string? ToAddress(UInt160? value);

    #endregion
}
