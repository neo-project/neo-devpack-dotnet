using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_UInt(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_UInt"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""isValidAndNotZeroUInt256"",""parameters"":[{""name"":""value"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""isValidAndNotZeroUInt160"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":29,""safe"":false},{""name"":""isZeroUInt256"",""parameters"":[{""name"":""value"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":58,""safe"":false},{""name"":""isZeroUInt160"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":65,""safe"":false},{""name"":""toAddress"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""String"",""offset"":72,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""base58CheckEncode""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrBFiYXNlNThDaGVja0VuY29kZQEAAQ8AAHBXAAF4NANAVwABeErZKFDKACCzqyQECUB4ELOqQFcAAXg0A0BXAAF4StkoUMoAFLOrJAQJQHgQs6pAVwABeBCzQFcAAXgQs0BXAAF4NANAVwABQUxJktx4NANAVwECEYhKEHnQcGh4i3Bo2yg3AABAaeV4IA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQDQA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.CALL 03 	-> 512 datoshi
    /// 06 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("isValidAndNotZeroUInt160")]
    public abstract bool? IsValidAndNotZeroUInt160(UInt160? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQDQA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.CALL 03 	-> 512 datoshi
    /// 06 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("isValidAndNotZeroUInt256")]
    public abstract bool? IsValidAndNotZeroUInt256(UInt256? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBCzQA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.PUSH0 	-> 1 datoshi
    /// 05 : OpCode.NUMEQUAL 	-> 8 datoshi
    /// 06 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("isZeroUInt160")]
    public abstract bool? IsZeroUInt160(UInt160? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBCzQA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.PUSH0 	-> 1 datoshi
    /// 05 : OpCode.NUMEQUAL 	-> 8 datoshi
    /// 06 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("isZeroUInt256")]
    public abstract bool? IsZeroUInt256(UInt256? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEYhKEHnQcGh4i3Bo2yg3AABA
    /// 00 : OpCode.INITSLOT 0102 	-> 64 datoshi
    /// 03 : OpCode.PUSH1 	-> 1 datoshi
    /// 04 : OpCode.NEWBUFFER 	-> 256 datoshi
    /// 05 : OpCode.DUP 	-> 2 datoshi
    /// 06 : OpCode.PUSH0 	-> 1 datoshi
    /// 07 : OpCode.LDARG1 	-> 2 datoshi
    /// 08 : OpCode.SETITEM 	-> 8192 datoshi
    /// 09 : OpCode.STLOC0 	-> 2 datoshi
    /// 0A : OpCode.LDLOC0 	-> 2 datoshi
    /// 0B : OpCode.LDARG0 	-> 2 datoshi
    /// 0C : OpCode.CAT 	-> 2048 datoshi
    /// 0D : OpCode.STLOC0 	-> 2 datoshi
    /// 0E : OpCode.LDLOC0 	-> 2 datoshi
    /// 0F : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 11 : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 14 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("toAddress")]
    public abstract string? ToAddress(UInt160? value);

    #endregion
}
