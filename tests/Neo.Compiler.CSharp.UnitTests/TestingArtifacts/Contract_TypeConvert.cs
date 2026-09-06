using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_TypeConvert(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_TypeConvert"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testType"",""parameters"":[],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""intToBytes"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":98,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHFXCQAQcGhKJAZFDAEASsqNcRJyakokBkUMAQBKyo1zDAED2zB0bNshdRCIdm7bIXcHGMQAdwhvCBBo0G8IEWnQbwgSatBvCBNr0G8IFGzQbwgVbdBvCBZu0G8IF28H0G8IQFcAAXhKJAZFDAEASsqNQFcgCMI=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEokBkUMAQBKyo1A
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// LEFT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("intToBytes")]
    public abstract byte[]? IntToBytes(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwkAEHBoSiQGRQwBAErKjXEScmpKJAZFDAEASsqNcwwBA9swdGzbIXUQiHZu2yF3BxjEAHcIbwgQaNBvCBFp0G8IEmrQbwgTa9BvCBRs0G8IFW3QbwgWbtBvCBdvB9BvCEA=
    /// INITSLOT 0900 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// LEFT [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// LEFT [2048 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSHDATA1 03 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// STLOC5 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// STLOC6 [2 datoshi]
    /// LDLOC6 [2 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// STLOC 07 [2 datoshi]
    /// PUSH8 [1 datoshi]
    /// NEWARRAY_T 00 'Any' [512 datoshi]
    /// STLOC 08 [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// LDLOC2 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// LDLOC3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// LDLOC4 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH5 [1 datoshi]
    /// LDLOC5 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH6 [1 datoshi]
    /// LDLOC6 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSH7 [1 datoshi]
    /// LDLOC 07 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testType")]
    public abstract object? TestType();

    #endregion
}
