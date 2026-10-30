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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Linq"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""aggregateSum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""allGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":143,""safe"":false},{""name"":""isEmpty"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":236,""safe"":false},{""name"":""anyGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":296,""safe"":false},{""name"":""anyGreaterThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":392,""safe"":false},{""name"":""average"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":413,""safe"":false},{""name"":""averageLong"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":546,""safe"":false},{""name"":""averageTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":679,""safe"":false},{""name"":""count"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":875,""safe"":false},{""name"":""countGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":948,""safe"":false},{""name"":""contains"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1055,""safe"":false},{""name"":""containsText"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1139,""safe"":false},{""name"":""containsPerson"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1147,""safe"":false},{""name"":""containsPersonIndex"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""targetIndex"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1217,""safe"":false},{""name"":""containsPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1273,""safe"":false},{""name"":""firstGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1354,""safe"":false},{""name"":""selectTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1451,""safe"":false},{""name"":""selectPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1579,""safe"":false},{""name"":""skip"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1669,""safe"":false},{""name"":""sum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1747,""safe"":false},{""name"":""sumLong"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1797,""safe"":false},{""name"":""sumTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1847,""safe"":false},{""name"":""take"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1982,""safe"":false},{""name"":""toMap"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2060,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2254,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2353,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

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
    /// Script: VwABCmQAAAAQeDQDQA==
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 64000000 [4 datoshi]
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
    /// Script: VwABClMAAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 53000000 [4 datoshi]
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
    /// Script: VwACeWAKCQAAAHg0oUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// STSFLD0 [2 datoshi]
    /// PUSHA 09000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL A1 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreaterThan")]
    public abstract bool? AnyGreaterThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClYAAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 56000000 [4 datoshi]
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
    /// Script: VwABCpkAAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 99000000 [4 datoshi]
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
    /// Script: VwUCwnB4SnHKchBzIhxpa850aBALEsBsNwAASxBR0GxLEVHQz2ucc2tqMOQQCxLAeTcAAEsQUdB5SxFR0HFpaDVs////QA==
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
    /// CALL_L 6CFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPerson")]
    public abstract bool? ContainsPerson(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIhxpa850aBALEsBsNwAASxBR0GxLEVHQz2ucc2tqMORoec5xaWg1NP///0A=
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
    /// CALL_L 34FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPersonIndex")]
    public abstract bool? ContainsPersonIndex(IList<object>? array, BigInteger? targetIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIhxpa850aBALEr9sNwAASxBR0GxLEVHQz2ucc2tqMOQQCxK/eTcAAEsQUdB5SxFR0HFpEE0RzhFNEM4Sv0ZoNeP+//9A
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
    /// PUSH0 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// NIP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL_L E3FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPersonS")]
    public abstract bool? ContainsPersonS(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0r0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL AF [512 datoshi]
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
    /// Script: VwABCmEAAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 61000000 [4 datoshi]
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
    /// Script: VwABDwpWAAAAeDQDQA==
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHM1 [1 datoshi]
    /// PUSHA 56000000 [4 datoshi]
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
    /// Script: VwUBwnB4SnHKchBzIhxpa850aBALEsBsNwAASxBR0GxLEVHQz2ucc2tqMORoCgwAAABQNVn///9A
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
    /// CALL_L 59FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("selectPersonS")]
    public abstract object? SelectPersonS(IList<object>? array);

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
    /// Script: VwABClwAAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 5C000000 [4 datoshi]
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
    /// Script: VwEBeAocAAAAUDWh/f//cGgKmgAAAAqlAAAAElI0I0A=
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHA 1C000000 [4 datoshi]
    /// SWAP [2 datoshi]
    /// CALL_L A1FDFFFF [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHA 9A000000 [4 datoshi]
    /// PUSHA A5000000 [4 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// CALL 23 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("toMap")]
    public abstract object? ToMap(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClkAAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 59000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("whereGreaterThanZero")]
    public abstract object? WhereGreaterThanZero(IList<object>? array);

    #endregion
}
