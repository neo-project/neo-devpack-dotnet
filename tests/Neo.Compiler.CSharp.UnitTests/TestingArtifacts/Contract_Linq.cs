using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Linq(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Linq"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""aggregateSum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""allGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":156,""safe"":false},{""name"":""isEmpty"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":249,""safe"":false},{""name"":""anyGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":309,""safe"":false},{""name"":""anyGreaterThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":405,""safe"":false},{""name"":""average"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":426,""safe"":false},{""name"":""averageTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":619,""safe"":false},{""name"":""count"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":888,""safe"":false},{""name"":""countGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1001,""safe"":false},{""name"":""contains"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1148,""safe"":false},{""name"":""containsText"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1232,""safe"":false},{""name"":""containsPerson"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1240,""safe"":false},{""name"":""containsPersonIndex"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""targetIndex"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1328,""safe"":false},{""name"":""containsPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1387,""safe"":false},{""name"":""firstGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1471,""safe"":false},{""name"":""selectTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1568,""safe"":false},{""name"":""selectPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1709,""safe"":false},{""name"":""skip"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1799,""safe"":false},{""name"":""sum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1923,""safe"":false},{""name"":""sumTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":2013,""safe"":false},{""name"":""take"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":2201,""safe"":false},{""name"":""toMap"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2325,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2501,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2600,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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fzIMAwAAAAABAAAAn0BXBQHCcHhKccpyEHMiImlrznRoEAsSwEo1X/7//2w3AABLEFHQbEsRUdDPa5xza2ow3goMAAAAaDVH////QFcAARALEr9KNcT+//94EM5LEFHQeBHOSxFR0EBXAAJ5eDQDQFcFAng0V8JweEpxynIQcyJGaWvOdHkQtyY3eUqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4FFIgVobM9rnHNrajC6aEBXAAF42CYTDA5zb3VyY2UgaXMgbnVsbDpAVwABeDQDQFcFAXg1xPr//xBweEpxynIQcyI7aWvOdGhsnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wa5xza2owxWhAVwABCoQAAAB4NANAVwUCeDReDAhzZWxlY3Rvcnk1Vvj//xBweEpxynIQcyI9aWvOdGhseTaeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BrnHNrajDDaEBXAAF42CYTDA5zb3VyY2UgaXMgbnVsbDpAVwABEnigSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAJ5eDQDQFcFAng0V8JweEpxynIQcyJGaWvOdHkQtiYEIj9obM95Sp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgUVrnHNrajC6aEBXAAF42CYTDA5zb3VyY2UgaXMgbnVsbDpAVwEBChoAAAB4NQ79//9wCpUAAAAKlQAAAGg0HkBXAAEQCxK/SjV9/P//eDcAAEsQUdB4SxFR0EBXBQN4NE8MC2tleVNlbGVjdG9yeTXv9v//DA9lbGVtZW50U2VsZWN0b3J6Ndj2///IcHhKccpyEHMiFGlrznRsejZKbHk2aFPQRWucc2tqMOxoQFcAAXjYJhMMDnNvdXJjZSBpcyBudWxsOkBXAAF4QFcAAXgQzkBXAAEKWQAAAHg0A0BXBQJ4NDMMCXByZWRpY2F0ZXk1bfb//8JweEpxynIQcyIRaWvOdGx5NiYFaGzPa5xza2ow72hAVwABeNgmEwwOc291cmNlIGlzIG51bGw6QFcAAXgQt0BWAkDtMoTf"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCmQAAAAQeDQDQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHA 64000000 [4 datoshi]
    /// 08 : PUSH0 [1 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : CALL 03 [512 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("aggregateSum")]
    public abstract BigInteger? AggregateSum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClMAAAB4NANA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHA 53000000 [4 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : CALL 03 [512 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("allGreaterThanZero")]
    public abstract bool? AllGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWAKCQAAAHg0oUA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : STSFLD0 [2 datoshi]
    /// 05 : PUSHA 09000000 [4 datoshi]
    /// 0A : LDARG0 [2 datoshi]
    /// 0B : CALL A1 [512 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreaterThan")]
    public abstract bool? AnyGreaterThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClYAAAB4NANA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHA 56000000 [4 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : CALL 03 [512 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreaterThanZero")]
    public abstract bool? AnyGreaterThanZero(IList<object>? array);

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
    [DisplayName("average")]
    public abstract BigInteger? Average(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCtUAAAB4NANA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHA D5000000 [4 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : CALL 03 [512 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("averageTwice")]
    public abstract BigInteger? AverageTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0A0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : CALL 03 [512 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("contains")]
    public abstract bool? Contains(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIh9pa850aBALEsBKNDRsNwAASxBR0GxLEVHQz2ucc2tqMOEQCxLASjQYeTcAAEsQUdB5SxFR0HFpaDVm////QA==
    /// 00 : INITSLOT 0502 [64 datoshi]
    /// 03 : NEWARRAY0 [16 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDARG0 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : STLOC1 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : STLOC2 [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : STLOC3 [2 datoshi]
    /// 0C : JMP 1F [2 datoshi]
    /// 0E : LDLOC1 [2 datoshi]
    /// 0F : LDLOC3 [2 datoshi]
    /// 10 : PICKITEM [64 datoshi]
    /// 11 : STLOC4 [2 datoshi]
    /// 12 : LDLOC0 [2 datoshi]
    /// 13 : PUSH0 [1 datoshi]
    /// 14 : PUSHNULL [1 datoshi]
    /// 15 : PUSH2 [1 datoshi]
    /// 16 : PACK [2048 datoshi]
    /// 17 : DUP [2 datoshi]
    /// 18 : CALL 34 [512 datoshi]
    /// 1A : LDLOC4 [2 datoshi]
    /// 1B : CALLT 0000 [32768 datoshi]
    /// 1E : OVER [2 datoshi]
    /// 1F : PUSH0 [1 datoshi]
    /// 20 : ROT [2 datoshi]
    /// 21 : SETITEM [8192 datoshi]
    /// 22 : LDLOC4 [2 datoshi]
    /// 23 : OVER [2 datoshi]
    /// 24 : PUSH1 [1 datoshi]
    /// 25 : ROT [2 datoshi]
    /// 26 : SETITEM [8192 datoshi]
    /// 27 : APPEND [8192 datoshi]
    /// 28 : LDLOC3 [2 datoshi]
    /// 29 : INC [4 datoshi]
    /// 2A : STLOC3 [2 datoshi]
    /// 2B : LDLOC3 [2 datoshi]
    /// 2C : LDLOC2 [2 datoshi]
    /// 2D : JMPLT E1 [2 datoshi]
    /// 2F : PUSH0 [1 datoshi]
    /// 30 : PUSHNULL [1 datoshi]
    /// 31 : PUSH2 [1 datoshi]
    /// 32 : PACK [2048 datoshi]
    /// 33 : DUP [2 datoshi]
    /// 34 : CALL 18 [512 datoshi]
    /// 36 : LDARG1 [2 datoshi]
    /// 37 : CALLT 0000 [32768 datoshi]
    /// 3A : OVER [2 datoshi]
    /// 3B : PUSH0 [1 datoshi]
    /// 3C : ROT [2 datoshi]
    /// 3D : SETITEM [8192 datoshi]
    /// 3E : LDARG1 [2 datoshi]
    /// 3F : OVER [2 datoshi]
    /// 40 : PUSH1 [1 datoshi]
    /// 41 : ROT [2 datoshi]
    /// 42 : SETITEM [8192 datoshi]
    /// 43 : STLOC1 [2 datoshi]
    /// 44 : LDLOC1 [2 datoshi]
    /// 45 : LDLOC0 [2 datoshi]
    /// 46 : CALL_L 66FFFFFF [512 datoshi]
    /// 4B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPerson")]
    public abstract bool? ContainsPerson(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIh9pa850aBALEsBKNNxsNwAASxBR0GxLEVHQz2ucc2tqMOFoec5xaWg1H////0A=
    /// 00 : INITSLOT 0502 [64 datoshi]
    /// 03 : NEWARRAY0 [16 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDARG0 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : STLOC1 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : STLOC2 [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : STLOC3 [2 datoshi]
    /// 0C : JMP 1F [2 datoshi]
    /// 0E : LDLOC1 [2 datoshi]
    /// 0F : LDLOC3 [2 datoshi]
    /// 10 : PICKITEM [64 datoshi]
    /// 11 : STLOC4 [2 datoshi]
    /// 12 : LDLOC0 [2 datoshi]
    /// 13 : PUSH0 [1 datoshi]
    /// 14 : PUSHNULL [1 datoshi]
    /// 15 : PUSH2 [1 datoshi]
    /// 16 : PACK [2048 datoshi]
    /// 17 : DUP [2 datoshi]
    /// 18 : CALL DC [512 datoshi]
    /// 1A : LDLOC4 [2 datoshi]
    /// 1B : CALLT 0000 [32768 datoshi]
    /// 1E : OVER [2 datoshi]
    /// 1F : PUSH0 [1 datoshi]
    /// 20 : ROT [2 datoshi]
    /// 21 : SETITEM [8192 datoshi]
    /// 22 : LDLOC4 [2 datoshi]
    /// 23 : OVER [2 datoshi]
    /// 24 : PUSH1 [1 datoshi]
    /// 25 : ROT [2 datoshi]
    /// 26 : SETITEM [8192 datoshi]
    /// 27 : APPEND [8192 datoshi]
    /// 28 : LDLOC3 [2 datoshi]
    /// 29 : INC [4 datoshi]
    /// 2A : STLOC3 [2 datoshi]
    /// 2B : LDLOC3 [2 datoshi]
    /// 2C : LDLOC2 [2 datoshi]
    /// 2D : JMPLT E1 [2 datoshi]
    /// 2F : LDLOC0 [2 datoshi]
    /// 30 : LDARG1 [2 datoshi]
    /// 31 : PICKITEM [64 datoshi]
    /// 32 : STLOC1 [2 datoshi]
    /// 33 : LDLOC1 [2 datoshi]
    /// 34 : LDLOC0 [2 datoshi]
    /// 35 : CALL_L 1FFFFFFF [512 datoshi]
    /// 3A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPersonIndex")]
    public abstract bool? ContainsPersonIndex(IList<object>? array, BigInteger? targetIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIh9pa850aBALEr9KNDRsNwAASxBR0GxLEVHQz2ucc2tqMOEQCxK/SjQYeTcAAEsQUdB5SxFR0HFpaDXT/v//QA==
    /// 00 : INITSLOT 0502 [64 datoshi]
    /// 03 : NEWARRAY0 [16 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDARG0 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : STLOC1 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : STLOC2 [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : STLOC3 [2 datoshi]
    /// 0C : JMP 1F [2 datoshi]
    /// 0E : LDLOC1 [2 datoshi]
    /// 0F : LDLOC3 [2 datoshi]
    /// 10 : PICKITEM [64 datoshi]
    /// 11 : STLOC4 [2 datoshi]
    /// 12 : LDLOC0 [2 datoshi]
    /// 13 : PUSH0 [1 datoshi]
    /// 14 : PUSHNULL [1 datoshi]
    /// 15 : PUSH2 [1 datoshi]
    /// 16 : PACKSTRUCT [2048 datoshi]
    /// 17 : DUP [2 datoshi]
    /// 18 : CALL 34 [512 datoshi]
    /// 1A : LDLOC4 [2 datoshi]
    /// 1B : CALLT 0000 [32768 datoshi]
    /// 1E : OVER [2 datoshi]
    /// 1F : PUSH0 [1 datoshi]
    /// 20 : ROT [2 datoshi]
    /// 21 : SETITEM [8192 datoshi]
    /// 22 : LDLOC4 [2 datoshi]
    /// 23 : OVER [2 datoshi]
    /// 24 : PUSH1 [1 datoshi]
    /// 25 : ROT [2 datoshi]
    /// 26 : SETITEM [8192 datoshi]
    /// 27 : APPEND [8192 datoshi]
    /// 28 : LDLOC3 [2 datoshi]
    /// 29 : INC [4 datoshi]
    /// 2A : STLOC3 [2 datoshi]
    /// 2B : LDLOC3 [2 datoshi]
    /// 2C : LDLOC2 [2 datoshi]
    /// 2D : JMPLT E1 [2 datoshi]
    /// 2F : PUSH0 [1 datoshi]
    /// 30 : PUSHNULL [1 datoshi]
    /// 31 : PUSH2 [1 datoshi]
    /// 32 : PACKSTRUCT [2048 datoshi]
    /// 33 : DUP [2 datoshi]
    /// 34 : CALL 18 [512 datoshi]
    /// 36 : LDARG1 [2 datoshi]
    /// 37 : CALLT 0000 [32768 datoshi]
    /// 3A : OVER [2 datoshi]
    /// 3B : PUSH0 [1 datoshi]
    /// 3C : ROT [2 datoshi]
    /// 3D : SETITEM [8192 datoshi]
    /// 3E : LDARG1 [2 datoshi]
    /// 3F : OVER [2 datoshi]
    /// 40 : PUSH1 [1 datoshi]
    /// 41 : ROT [2 datoshi]
    /// 42 : SETITEM [8192 datoshi]
    /// 43 : STLOC1 [2 datoshi]
    /// 44 : LDLOC1 [2 datoshi]
    /// 45 : LDLOC0 [2 datoshi]
    /// 46 : CALL_L D3FEFFFF [512 datoshi]
    /// 4B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPersonS")]
    public abstract bool? ContainsPersonS(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0r0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : CALL AF [512 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsText")]
    public abstract bool? ContainsText(IList<object>? array, string? target);

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
    [DisplayName("count")]
    public abstract BigInteger? Count(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCokAAAB4NANA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHA 89000000 [4 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : CALL 03 [512 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("countGreaterThanZero")]
    public abstract BigInteger? CountGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDwpWAAAAeDQDQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHM1 [1 datoshi]
    /// 04 : PUSHA 56000000 [4 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : CALL 03 [512 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("firstGreaterThanZero")]
    public abstract BigInteger? FirstGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQEqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : CALL 04 [512 datoshi]
    /// 06 : NOT [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEmpty")]
    public abstract bool? IsEmpty(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUBwnB4SnHKchBzIiJpa850aBALEsBKNV/+//9sNwAASxBR0GxLEVHQz2ucc2tqMN4KDAAAAGg1R////0A=
    /// 00 : INITSLOT 0501 [64 datoshi]
    /// 03 : NEWARRAY0 [16 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDARG0 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : STLOC1 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : STLOC2 [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : STLOC3 [2 datoshi]
    /// 0C : JMP 22 [2 datoshi]
    /// 0E : LDLOC1 [2 datoshi]
    /// 0F : LDLOC3 [2 datoshi]
    /// 10 : PICKITEM [64 datoshi]
    /// 11 : STLOC4 [2 datoshi]
    /// 12 : LDLOC0 [2 datoshi]
    /// 13 : PUSH0 [1 datoshi]
    /// 14 : PUSHNULL [1 datoshi]
    /// 15 : PUSH2 [1 datoshi]
    /// 16 : PACK [2048 datoshi]
    /// 17 : DUP [2 datoshi]
    /// 18 : CALL_L 5FFEFFFF [512 datoshi]
    /// 1D : LDLOC4 [2 datoshi]
    /// 1E : CALLT 0000 [32768 datoshi]
    /// 21 : OVER [2 datoshi]
    /// 22 : PUSH0 [1 datoshi]
    /// 23 : ROT [2 datoshi]
    /// 24 : SETITEM [8192 datoshi]
    /// 25 : LDLOC4 [2 datoshi]
    /// 26 : OVER [2 datoshi]
    /// 27 : PUSH1 [1 datoshi]
    /// 28 : ROT [2 datoshi]
    /// 29 : SETITEM [8192 datoshi]
    /// 2A : APPEND [8192 datoshi]
    /// 2B : LDLOC3 [2 datoshi]
    /// 2C : INC [4 datoshi]
    /// 2D : STLOC3 [2 datoshi]
    /// 2E : LDLOC3 [2 datoshi]
    /// 2F : LDLOC2 [2 datoshi]
    /// 30 : JMPLT DE [2 datoshi]
    /// 32 : PUSHA 0C000000 [4 datoshi]
    /// 37 : LDLOC0 [2 datoshi]
    /// 38 : CALL_L 47FFFFFF [512 datoshi]
    /// 3D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("selectPersonS")]
    public abstract object? SelectPersonS(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClUAAAB4NANA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHA 55000000 [4 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : CALL 03 [512 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("selectTwice")]
    public abstract object? SelectTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0A0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : CALL 03 [512 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("skip")]
    public abstract object? Skip(IList<object>? array, BigInteger? count);

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
    [DisplayName("sum")]
    public abstract BigInteger? Sum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCoQAAAB4NANA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHA 84000000 [4 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : CALL 03 [512 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sumTwice")]
    public abstract BigInteger? SumTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0A0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : CALL 03 [512 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("take")]
    public abstract object? Take(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBChoAAAB4NQ79//9wCpUAAAAKlQAAAGg0HkA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSHA 1A000000 [4 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : CALL_L 0EFDFFFF [512 datoshi]
    /// 0E : STLOC0 [2 datoshi]
    /// 0F : PUSHA 95000000 [4 datoshi]
    /// 14 : PUSHA 95000000 [4 datoshi]
    /// 19 : LDLOC0 [2 datoshi]
    /// 1A : CALL 1E [512 datoshi]
    /// 1C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("toMap")]
    public abstract object? ToMap(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClkAAAB4NANA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHA 59000000 [4 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : CALL 03 [512 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("whereGreaterThanZero")]
    public abstract object? WhereGreaterThanZero(IList<object>? array);

    #endregion
}
