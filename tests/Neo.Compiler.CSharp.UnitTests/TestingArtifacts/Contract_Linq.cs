using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Linq(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Linq"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""aggregateSum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""allGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":145,""safe"":false},{""name"":""isEmpty"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":240,""safe"":false},{""name"":""anyGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":300,""safe"":false},{""name"":""anyGreaterThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":398,""safe"":false},{""name"":""average"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":419,""safe"":false},{""name"":""averageLong"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":552,""safe"":false},{""name"":""averageTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":685,""safe"":false},{""name"":""count"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":885,""safe"":false},{""name"":""countEnumerable"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":926,""safe"":false},{""name"":""countStrings"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1001,""safe"":false},{""name"":""countBigIntegers"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1008,""safe"":false},{""name"":""countStructs"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1015,""safe"":false},{""name"":""countNullable"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1022,""safe"":false},{""name"":""countGeneric"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1032,""safe"":false},{""name"":""countNull"",""parameters"":[],""returntype"":""Integer"",""offset"":1049,""safe"":false},{""name"":""countGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1061,""safe"":false},{""name"":""contains"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1170,""safe"":false},{""name"":""containsText"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1256,""safe"":false},{""name"":""containsPerson"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1264,""safe"":false},{""name"":""containsPersonIndex"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""targetIndex"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1334,""safe"":false},{""name"":""containsPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1390,""safe"":false},{""name"":""firstGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1460,""safe"":false},{""name"":""selectTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1559,""safe"":false},{""name"":""selectPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1689,""safe"":false},{""name"":""skip"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1768,""safe"":false},{""name"":""sum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1846,""safe"":false},{""name"":""sumLong"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1896,""safe"":false},{""name"":""sumTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1946,""safe"":false},{""name"":""take"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":2083,""safe"":false},{""name"":""toMap"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2161,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2337,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2438,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"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").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCmYAAAAQeDQDQA==
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 66000000 [4 datoshi]
    /// PUSH0 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("aggregateSum")]
    public abstract BigInteger? AggregateSum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClUAAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 55000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("allGreaterThanZero")]
    public abstract bool? AllGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWAKCQAAAHg0n0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// STSFLD0 [2 datoshi]
    /// PUSHA 09000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 9F [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreaterThan")]
    public abstract bool? AnyGreaterThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClgAAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 58000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreaterThanZero")]
    public abstract bool? AnyGreaterThanZero(IList<object>? array);

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
    [DisplayName("average")]
    public abstract BigInteger? Average(IList<object>? array);

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
    [DisplayName("averageLong")]
    public abstract BigInteger? AverageLong(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCp0AAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 9D000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("averageTwice")]
    public abstract BigInteger? AverageTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0A0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("contains")]
    public abstract bool? Contains(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIhxpa850aBALEsBsNwAASxBR0GxLEVHQz2ucc2tqMOQQCxLAeTcAAEsQUdB5SxFR0HFpaDVq////QA==
    /// INITSLOT 0502 [64 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC3 [2 datoshi]
    /// JMP 1C [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// LDLOC4 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// OVER [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC4 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// APPEND [8192 datoshi]
    /// LDLOC3 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// JMPLT E4 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// OVER [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDARG1 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL_L 6AFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPerson")]
    public abstract bool? ContainsPerson(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIhxpa850aBALEsBsNwAASxBR0GxLEVHQz2ucc2tqMORoec5xaWg1Mv///0A=
    /// INITSLOT 0502 [64 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC3 [2 datoshi]
    /// JMP 1C [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// LDLOC4 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// OVER [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC4 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// APPEND [8192 datoshi]
    /// LDLOC3 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// JMPLT E4 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL_L 32FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPersonIndex")]
    public abstract bool? ContainsPersonIndex(IList<object>? array, BigInteger? targetIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIhxpa850aBALEr9sNwAASxBR0GxLEVHQz2ucc2tqMOQQCxK/eTcAAEsQUdB5SxFR0HFpaDXs/v//QA==
    /// INITSLOT 0502 [64 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC3 [2 datoshi]
    /// JMP 1C [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDLOC4 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// OVER [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC4 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// APPEND [8192 datoshi]
    /// LDLOC3 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// JMPLT E4 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// LDARG1 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// OVER [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDARG1 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL_L ECFEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPersonS")]
    public abstract bool? ContainsPersonS(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0rUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL AD [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsText")]
    public abstract bool? ContainsText(IList<object>? array, string? target);

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
    [DisplayName("count")]
    public abstract BigInteger? Count(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDSIQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 88 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("countBigIntegers")]
    public abstract BigInteger? CountBigIntegers(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoNANA
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("countEnumerable")]
    public abstract BigInteger? CountEnumerable(IList<object>? array);

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
    [DisplayName("countGeneric")]
    public abstract BigInteger? CountGeneric(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCmMAAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 63000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("countGreaterThanZero")]
    public abstract BigInteger? CountGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3BoNV3///9A
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL_L 5DFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("countNull")]
    public abstract BigInteger? CountNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDV6////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 7AFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("countNullable")]
    public abstract BigInteger? CountNullable(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDSPQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 8F [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("countStrings")]
    public abstract BigInteger? CountStrings(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDSBQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 81 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("countStructs")]
    public abstract BigInteger? CountStructs(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDwpYAAAAeDQDQA==
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHM1 [1 datoshi]
    /// PUSHA 58000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("firstGreaterThanZero")]
    public abstract BigInteger? FirstGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQEqkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 04 [512 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEmpty")]
    public abstract bool? IsEmpty(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUBwnB4SnHKchBzIhxpa850aBALEsBsNwAASxBR0GxLEVHQz2ucc2tqMORoCgwAAABQNVf///9A
    /// INITSLOT 0501 [64 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC3 [2 datoshi]
    /// JMP 1C [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// LDLOC4 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// OVER [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC4 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// APPEND [8192 datoshi]
    /// LDLOC3 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// JMPLT E4 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHA 0C000000 [4 datoshi]
    /// SWAP [2 datoshi]
    /// CALL_L 57FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("selectPersonS")]
    public abstract object? SelectPersonS(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClcAAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 57000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("selectTwice")]
    public abstract object? SelectTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0A0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("skip")]
    public abstract object? Skip(IList<object>? array, BigInteger? count);

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
    [DisplayName("sum")]
    public abstract BigInteger? Sum(IList<object>? array);

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
    [DisplayName("sumLong")]
    public abstract BigInteger? SumLong(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCl4AAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 5E000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("sumTwice")]
    public abstract BigInteger? SumTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0A0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("take")]
    public abstract object? Take(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeAocAAAAUDWo/f//cGgKkwAAAAqTAAAAElI0GEA=
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHA 1C000000 [4 datoshi]
    /// SWAP [2 datoshi]
    /// CALL_L A8FDFFFF [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHA 93000000 [4 datoshi]
    /// PUSHA 93000000 [4 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// CALL 18 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("toMap")]
    public abstract object? ToMap(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClsAAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 5B000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("whereGreaterThanZero")]
    public abstract object? WhereGreaterThanZero(IList<object>? array);

    #endregion
}
