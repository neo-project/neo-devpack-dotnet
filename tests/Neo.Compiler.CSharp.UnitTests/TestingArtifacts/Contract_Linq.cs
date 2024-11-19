using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Linq(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Linq"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""aggregateSum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""allGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":156,""safe"":false},{""name"":""isEmpty"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":249,""safe"":false},{""name"":""anyGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":309,""safe"":false},{""name"":""anyGreaterThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":405,""safe"":false},{""name"":""average"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":426,""safe"":false},{""name"":""averageTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":618,""safe"":false},{""name"":""count"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":886,""safe"":false},{""name"":""countGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":999,""safe"":false},{""name"":""contains"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1146,""safe"":false},{""name"":""containsText"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1230,""safe"":false},{""name"":""containsPerson"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1238,""safe"":false},{""name"":""containsPersonIndex"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""targetIndex"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1326,""safe"":false},{""name"":""containsPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1385,""safe"":false},{""name"":""firstGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1469,""safe"":false},{""name"":""selectTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1566,""safe"":false},{""name"":""selectPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1707,""safe"":false},{""name"":""skip"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1797,""safe"":false},{""name"":""sum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1921,""safe"":false},{""name"":""sumTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":2011,""safe"":false},{""name"":""take"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":2199,""safe"":false},{""name"":""toMap"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2323,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2499,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2598,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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DAMAAAAAAQAAAJ9AVwUBwnB4SnHKchBzIiJpa850aBALEsBKNV/+//9sNwAASxBR0GxLEVHQz2ucc2tqMN4KDAAAAGg1R////0BXAAEQCxK/SjXE/v//eBDOSxBR0HgRzksRUdBAVwACeXg0A0BXBQJ4NFfCcHhKccpyEHMiRmlrznR5ELcmN3lKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+BRSIFaGzPa5xza2owumhAVwABeNgmEwwOc291cmNlIGlzIG51bGw6QFcAAXg0A0BXBQF4NcX6//8QcHhKccpyEHMiO2lrznRobJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGucc2tqMMVoQFcAAQqEAAAAeDQDQFcFAng0XgwIc2VsZWN0b3J5NVj4//8QcHhKccpyEHMiPWlrznRobHk2nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wa5xza2oww2hAVwABeNgmEwwOc291cmNlIGlzIG51bGw6QFcAARJ4oEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwACeXg0A0BXBQJ4NFfCcHhKccpyEHMiRmlrznR5ELYmBCI/aGzPeUqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4FFa5xza2owumhAVwABeNgmEwwOc291cmNlIGlzIG51bGw6QFcBAQoaAAAAeDUO/f//cAqVAAAACpUAAABoNB5AVwABEAsSv0o1ffz//3g3AABLEFHQeEsRUdBAVwUDeDRPDAtrZXlTZWxlY3Rvcnk18fb//wwPZWxlbWVudFNlbGVjdG9yejXa9v//yHB4SnHKchBzIhRpa850bHo2Smx5NmhT0EVrnHNrajDsaEBXAAF42CYTDA5zb3VyY2UgaXMgbnVsbDpAVwABeEBXAAF4EM5AVwABClkAAAB4NANAVwUCeDQzDAlwcmVkaWNhdGV5NW/2///CcHhKccpyEHMiEWlrznRseTYmBWhsz2ucc2tqMO9oQFcAAXjYJhMMDnNvdXJjZSBpcyBudWxsOkBXAAF4ELdAVgJAKHBaYg=="));

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
    /// Script: VwABCtQAAAB4NANA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHA D4000000 [4 datoshi]
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
