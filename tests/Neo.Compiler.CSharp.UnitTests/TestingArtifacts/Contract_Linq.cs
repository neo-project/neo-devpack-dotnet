using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Linq(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Linq"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""aggregateSum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""allGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":156,""safe"":false},{""name"":""isEmpty"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":249,""safe"":false},{""name"":""anyGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":309,""safe"":false},{""name"":""anyGreaterThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":405,""safe"":false},{""name"":""average"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":426,""safe"":false},{""name"":""averageTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":618,""safe"":false},{""name"":""count"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":886,""safe"":false},{""name"":""countGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":999,""safe"":false},{""name"":""contains"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1146,""safe"":false},{""name"":""containsText"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1230,""safe"":false},{""name"":""containsPerson"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1238,""safe"":false},{""name"":""containsPersonIndex"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""targetIndex"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1322,""safe"":false},{""name"":""containsPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1381,""safe"":false},{""name"":""firstGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1451,""safe"":false},{""name"":""selectTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1548,""safe"":false},{""name"":""selectPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1689,""safe"":false},{""name"":""skip"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1773,""safe"":false},{""name"":""sum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1897,""safe"":false},{""name"":""sumTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1987,""safe"":false},{""name"":""take"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":2175,""safe"":false},{""name"":""toMap"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2299,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2469,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2568,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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SnHKchBzIiJpa850aBALEsBKNXH+//9sNwAASxBR0GxLEVHQz2ucc2tqMN4KDAAAAGg1R////0BXAAEQCxK/eBDOSxBR0HgRzksRUdBAVwACeXg0A0BXBQJ4NFfCcHhKccpyEHMiRmlrznR5ELcmN3lKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+BRSIFaGzPa5xza2owumhAVwABeNgmEwwOc291cmNlIGlzIG51bGw6QFcAAXg0A0BXBQF4Nd36//8QcHhKccpyEHMiO2lrznRobJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGucc2tqMMVoQFcAAQqEAAAAeDQDQFcFAng0XgwIc2VsZWN0b3J5NXD4//8QcHhKccpyEHMiPWlrznRobHk2nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wa5xza2oww2hAVwABeNgmEwwOc291cmNlIGlzIG51bGw6QFcAARJ4oEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwACeXg0A0BXBQJ4NFfCcHhKccpyEHMiRmlrznR5ELYmBCI/aGzPeUqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4FFa5xza2owumhAVwABeNgmEwwOc291cmNlIGlzIG51bGw6QFcBAQoaAAAAeDUU/f//cAqPAAAACo8AAABoNBhAVwABEAsSv3g3AABLEFHQeEsRUdBAVwUDeDRPDAtrZXlTZWxlY3Rvcnk1D/f//wwPZWxlbWVudFNlbGVjdG9yejX49v//yHB4SnHKchBzIhRpa850bHo2Smx5NmhT0EVrnHNrajDsaEBXAAF42CYTDA5zb3VyY2UgaXMgbnVsbDpAVwABeEBXAAF4EM5AVwABClkAAAB4NANAVwUCeDQzDAlwcmVkaWNhdGV5NY32///CcHhKccpyEHMiEWlrznRseTYmBWhsz2ucc2tqMO9oQFcAAXjYJhMMDnNvdXJjZSBpcyBudWxsOkBXAAF4ELdAVgJAsTBTwQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCmQAAAAQeDQDQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 64000000 [4 datoshi]
    /// 08 : OpCode.PUSH0 [1 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.CALL 03 [512 datoshi]
    /// 0C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("aggregateSum")]
    public abstract BigInteger? AggregateSum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClMAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 53000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("allGreaterThanZero")]
    public abstract bool? AllGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWAKCQAAAHg0oUA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.STSFLD0 [2 datoshi]
    /// 05 : OpCode.PUSHA 09000000 [4 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.CALL A1 [512 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreaterThan")]
    public abstract bool? AnyGreaterThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClYAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 56000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreaterThanZero")]
    public abstract bool? AnyGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYCeDWuAAAADHNlbGVjdG9yeTXG/f//EHAQcXhKcspzEHQicGpsznVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpbXk2nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xbJx0bGswkGixJBQMc291cmNlIGlzIGVtcHR5OmlooUA=
    /// 00 : OpCode.INITSLOT 0602 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALL_L AE000000 [512 datoshi]
    /// 09 : OpCode.PUSHDATA1 73656C6563746F72 [8 datoshi]
    /// 13 : OpCode.LDARG1 [2 datoshi]
    /// 14 : OpCode.CALL_L C6FDFFFF [512 datoshi]
    /// 19 : OpCode.PUSH0 [1 datoshi]
    /// 1A : OpCode.STLOC0 [2 datoshi]
    /// 1B : OpCode.PUSH0 [1 datoshi]
    /// 1C : OpCode.STLOC1 [2 datoshi]
    /// 1D : OpCode.LDARG0 [2 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.STLOC2 [2 datoshi]
    /// 20 : OpCode.SIZE [4 datoshi]
    /// 21 : OpCode.STLOC3 [2 datoshi]
    /// 22 : OpCode.PUSH0 [1 datoshi]
    /// 23 : OpCode.STLOC4 [2 datoshi]
    /// 24 : OpCode.JMP 70 [2 datoshi]
    /// 26 : OpCode.LDLOC2 [2 datoshi]
    /// 27 : OpCode.LDLOC4 [2 datoshi]
    /// 28 : OpCode.PICKITEM [64 datoshi]
    /// 29 : OpCode.STLOC5 [2 datoshi]
    /// 2A : OpCode.LDLOC0 [2 datoshi]
    /// 2B : OpCode.DUP [2 datoshi]
    /// 2C : OpCode.INC [4 datoshi]
    /// 2D : OpCode.DUP [2 datoshi]
    /// 2E : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 33 : OpCode.JMPGE 04 [2 datoshi]
    /// 35 : OpCode.JMP 0A [2 datoshi]
    /// 37 : OpCode.DUP [2 datoshi]
    /// 38 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3D : OpCode.JMPLE 1E [2 datoshi]
    /// 3F : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 48 : OpCode.AND [8 datoshi]
    /// 49 : OpCode.DUP [2 datoshi]
    /// 4A : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 4F : OpCode.JMPLE 0C [2 datoshi]
    /// 51 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 5A : OpCode.SUB [8 datoshi]
    /// 5B : OpCode.STLOC0 [2 datoshi]
    /// 5C : OpCode.DROP [2 datoshi]
    /// 5D : OpCode.LDLOC1 [2 datoshi]
    /// 5E : OpCode.LDLOC5 [2 datoshi]
    /// 5F : OpCode.LDARG1 [2 datoshi]
    /// 60 : OpCode.CALLA [512 datoshi]
    /// 61 : OpCode.ADD [8 datoshi]
    /// 62 : OpCode.DUP [2 datoshi]
    /// 63 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 68 : OpCode.JMPGE 04 [2 datoshi]
    /// 6A : OpCode.JMP 0A [2 datoshi]
    /// 6C : OpCode.DUP [2 datoshi]
    /// 6D : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 72 : OpCode.JMPLE 1E [2 datoshi]
    /// 74 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 7D : OpCode.AND [8 datoshi]
    /// 7E : OpCode.DUP [2 datoshi]
    /// 7F : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 84 : OpCode.JMPLE 0C [2 datoshi]
    /// 86 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 8F : OpCode.SUB [8 datoshi]
    /// 90 : OpCode.STLOC1 [2 datoshi]
    /// 91 : OpCode.LDLOC4 [2 datoshi]
    /// 92 : OpCode.INC [4 datoshi]
    /// 93 : OpCode.STLOC4 [2 datoshi]
    /// 94 : OpCode.LDLOC4 [2 datoshi]
    /// 95 : OpCode.LDLOC3 [2 datoshi]
    /// 96 : OpCode.JMPLT 90 [2 datoshi]
    /// 98 : OpCode.LDLOC0 [2 datoshi]
    /// 99 : OpCode.NZ [4 datoshi]
    /// 9A : OpCode.JMPIF 14 [2 datoshi]
    /// 9C : OpCode.PUSHDATA1 736F7572636520697320656D707479 [8 datoshi]
    /// AD : OpCode.THROW [512 datoshi]
    /// AE : OpCode.LDLOC1 [2 datoshi]
    /// AF : OpCode.LDLOC0 [2 datoshi]
    /// B0 : OpCode.DIV [8 datoshi]
    /// B1 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("average")]
    public abstract BigInteger? Average(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCtQAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA D4000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("averageTwice")]
    public abstract BigInteger? AverageTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWEKQAAAAHg0A0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.STSFLD1 [2 datoshi]
    /// 05 : OpCode.PUSHA 40000000 [4 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.CALL 03 [512 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("contains")]
    public abstract bool? Contains(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIh9pa850aBALEsBKNDRsNwAASxBR0GxLEVHQz2ucc2tqMOEQCxLASjQYeTcAAEsQUdB5SxFR0HFpaDVm////QA==
    /// 00 : OpCode.INITSLOT 0502 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.STLOC1 [2 datoshi]
    /// 08 : OpCode.SIZE [4 datoshi]
    /// 09 : OpCode.STLOC2 [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.STLOC3 [2 datoshi]
    /// 0C : OpCode.JMP 1F [2 datoshi]
    /// 0E : OpCode.LDLOC1 [2 datoshi]
    /// 0F : OpCode.LDLOC3 [2 datoshi]
    /// 10 : OpCode.PICKITEM [64 datoshi]
    /// 11 : OpCode.STLOC4 [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.PUSH0 [1 datoshi]
    /// 14 : OpCode.PUSHNULL [1 datoshi]
    /// 15 : OpCode.PUSH2 [1 datoshi]
    /// 16 : OpCode.PACK [2048 datoshi]
    /// 17 : OpCode.DUP [2 datoshi]
    /// 18 : OpCode.CALL 34 [512 datoshi]
    /// 1A : OpCode.LDLOC4 [2 datoshi]
    /// 1B : OpCode.CALLT 0000 [32768 datoshi]
    /// 1E : OpCode.OVER [2 datoshi]
    /// 1F : OpCode.PUSH0 [1 datoshi]
    /// 20 : OpCode.ROT [2 datoshi]
    /// 21 : OpCode.SETITEM [8192 datoshi]
    /// 22 : OpCode.LDLOC4 [2 datoshi]
    /// 23 : OpCode.OVER [2 datoshi]
    /// 24 : OpCode.PUSH1 [1 datoshi]
    /// 25 : OpCode.ROT [2 datoshi]
    /// 26 : OpCode.SETITEM [8192 datoshi]
    /// 27 : OpCode.APPEND [8192 datoshi]
    /// 28 : OpCode.LDLOC3 [2 datoshi]
    /// 29 : OpCode.INC [4 datoshi]
    /// 2A : OpCode.STLOC3 [2 datoshi]
    /// 2B : OpCode.LDLOC3 [2 datoshi]
    /// 2C : OpCode.LDLOC2 [2 datoshi]
    /// 2D : OpCode.JMPLT E1 [2 datoshi]
    /// 2F : OpCode.PUSH0 [1 datoshi]
    /// 30 : OpCode.PUSHNULL [1 datoshi]
    /// 31 : OpCode.PUSH2 [1 datoshi]
    /// 32 : OpCode.PACK [2048 datoshi]
    /// 33 : OpCode.DUP [2 datoshi]
    /// 34 : OpCode.CALL 18 [512 datoshi]
    /// 36 : OpCode.LDARG1 [2 datoshi]
    /// 37 : OpCode.CALLT 0000 [32768 datoshi]
    /// 3A : OpCode.OVER [2 datoshi]
    /// 3B : OpCode.PUSH0 [1 datoshi]
    /// 3C : OpCode.ROT [2 datoshi]
    /// 3D : OpCode.SETITEM [8192 datoshi]
    /// 3E : OpCode.LDARG1 [2 datoshi]
    /// 3F : OpCode.OVER [2 datoshi]
    /// 40 : OpCode.PUSH1 [1 datoshi]
    /// 41 : OpCode.ROT [2 datoshi]
    /// 42 : OpCode.SETITEM [8192 datoshi]
    /// 43 : OpCode.STLOC1 [2 datoshi]
    /// 44 : OpCode.LDLOC1 [2 datoshi]
    /// 45 : OpCode.LDLOC0 [2 datoshi]
    /// 46 : OpCode.CALL_L 66FFFFFF [512 datoshi]
    /// 4B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPerson")]
    public abstract bool? ContainsPerson(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIh9pa850aBALEsBKNOBsNwAASxBR0GxLEVHQz2ucc2tqMOFoec5xaWg1I////0A=
    /// 00 : OpCode.INITSLOT 0502 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.STLOC1 [2 datoshi]
    /// 08 : OpCode.SIZE [4 datoshi]
    /// 09 : OpCode.STLOC2 [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.STLOC3 [2 datoshi]
    /// 0C : OpCode.JMP 1F [2 datoshi]
    /// 0E : OpCode.LDLOC1 [2 datoshi]
    /// 0F : OpCode.LDLOC3 [2 datoshi]
    /// 10 : OpCode.PICKITEM [64 datoshi]
    /// 11 : OpCode.STLOC4 [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.PUSH0 [1 datoshi]
    /// 14 : OpCode.PUSHNULL [1 datoshi]
    /// 15 : OpCode.PUSH2 [1 datoshi]
    /// 16 : OpCode.PACK [2048 datoshi]
    /// 17 : OpCode.DUP [2 datoshi]
    /// 18 : OpCode.CALL E0 [512 datoshi]
    /// 1A : OpCode.LDLOC4 [2 datoshi]
    /// 1B : OpCode.CALLT 0000 [32768 datoshi]
    /// 1E : OpCode.OVER [2 datoshi]
    /// 1F : OpCode.PUSH0 [1 datoshi]
    /// 20 : OpCode.ROT [2 datoshi]
    /// 21 : OpCode.SETITEM [8192 datoshi]
    /// 22 : OpCode.LDLOC4 [2 datoshi]
    /// 23 : OpCode.OVER [2 datoshi]
    /// 24 : OpCode.PUSH1 [1 datoshi]
    /// 25 : OpCode.ROT [2 datoshi]
    /// 26 : OpCode.SETITEM [8192 datoshi]
    /// 27 : OpCode.APPEND [8192 datoshi]
    /// 28 : OpCode.LDLOC3 [2 datoshi]
    /// 29 : OpCode.INC [4 datoshi]
    /// 2A : OpCode.STLOC3 [2 datoshi]
    /// 2B : OpCode.LDLOC3 [2 datoshi]
    /// 2C : OpCode.LDLOC2 [2 datoshi]
    /// 2D : OpCode.JMPLT E1 [2 datoshi]
    /// 2F : OpCode.LDLOC0 [2 datoshi]
    /// 30 : OpCode.LDARG1 [2 datoshi]
    /// 31 : OpCode.PICKITEM [64 datoshi]
    /// 32 : OpCode.STLOC1 [2 datoshi]
    /// 33 : OpCode.LDLOC1 [2 datoshi]
    /// 34 : OpCode.LDLOC0 [2 datoshi]
    /// 35 : OpCode.CALL_L 23FFFFFF [512 datoshi]
    /// 3A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPersonIndex")]
    public abstract bool? ContainsPersonIndex(IList<object>? array, BigInteger? targetIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIhxpa850aBALEr9sNwAASxBR0GxLEVHQz2ucc2tqMOQQCxK/eTcAAEsQUdB5SxFR0HFpaDXd/v//QA==
    /// 00 : OpCode.INITSLOT 0502 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.STLOC1 [2 datoshi]
    /// 08 : OpCode.SIZE [4 datoshi]
    /// 09 : OpCode.STLOC2 [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.STLOC3 [2 datoshi]
    /// 0C : OpCode.JMP 1C [2 datoshi]
    /// 0E : OpCode.LDLOC1 [2 datoshi]
    /// 0F : OpCode.LDLOC3 [2 datoshi]
    /// 10 : OpCode.PICKITEM [64 datoshi]
    /// 11 : OpCode.STLOC4 [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.PUSH0 [1 datoshi]
    /// 14 : OpCode.PUSHNULL [1 datoshi]
    /// 15 : OpCode.PUSH2 [1 datoshi]
    /// 16 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 17 : OpCode.LDLOC4 [2 datoshi]
    /// 18 : OpCode.CALLT 0000 [32768 datoshi]
    /// 1B : OpCode.OVER [2 datoshi]
    /// 1C : OpCode.PUSH0 [1 datoshi]
    /// 1D : OpCode.ROT [2 datoshi]
    /// 1E : OpCode.SETITEM [8192 datoshi]
    /// 1F : OpCode.LDLOC4 [2 datoshi]
    /// 20 : OpCode.OVER [2 datoshi]
    /// 21 : OpCode.PUSH1 [1 datoshi]
    /// 22 : OpCode.ROT [2 datoshi]
    /// 23 : OpCode.SETITEM [8192 datoshi]
    /// 24 : OpCode.APPEND [8192 datoshi]
    /// 25 : OpCode.LDLOC3 [2 datoshi]
    /// 26 : OpCode.INC [4 datoshi]
    /// 27 : OpCode.STLOC3 [2 datoshi]
    /// 28 : OpCode.LDLOC3 [2 datoshi]
    /// 29 : OpCode.LDLOC2 [2 datoshi]
    /// 2A : OpCode.JMPLT E4 [2 datoshi]
    /// 2C : OpCode.PUSH0 [1 datoshi]
    /// 2D : OpCode.PUSHNULL [1 datoshi]
    /// 2E : OpCode.PUSH2 [1 datoshi]
    /// 2F : OpCode.PACKSTRUCT [2048 datoshi]
    /// 30 : OpCode.LDARG1 [2 datoshi]
    /// 31 : OpCode.CALLT 0000 [32768 datoshi]
    /// 34 : OpCode.OVER [2 datoshi]
    /// 35 : OpCode.PUSH0 [1 datoshi]
    /// 36 : OpCode.ROT [2 datoshi]
    /// 37 : OpCode.SETITEM [8192 datoshi]
    /// 38 : OpCode.LDARG1 [2 datoshi]
    /// 39 : OpCode.OVER [2 datoshi]
    /// 3A : OpCode.PUSH1 [1 datoshi]
    /// 3B : OpCode.ROT [2 datoshi]
    /// 3C : OpCode.SETITEM [8192 datoshi]
    /// 3D : OpCode.STLOC1 [2 datoshi]
    /// 3E : OpCode.LDLOC1 [2 datoshi]
    /// 3F : OpCode.LDLOC0 [2 datoshi]
    /// 40 : OpCode.CALL_L DDFEFFFF [512 datoshi]
    /// 45 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPersonS")]
    public abstract bool? ContainsPersonS(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0r0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.CALL AF [512 datoshi]
    /// 07 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsText")]
    public abstract bool? ContainsText(IList<object>? array, string? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDRjDHByZWRpY2F0ZXk1S/z//xBweEpxynIQcyJBaWvOdGx5NiY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFa5xza2owv2hA
    /// 00 : OpCode.INITSLOT 0502 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALL 63 [512 datoshi]
    /// 06 : OpCode.PUSHDATA1 707265646963617465 [8 datoshi]
    /// 11 : OpCode.LDARG1 [2 datoshi]
    /// 12 : OpCode.CALL_L 4BFCFFFF [512 datoshi]
    /// 17 : OpCode.PUSH0 [1 datoshi]
    /// 18 : OpCode.STLOC0 [2 datoshi]
    /// 19 : OpCode.LDARG0 [2 datoshi]
    /// 1A : OpCode.DUP [2 datoshi]
    /// 1B : OpCode.STLOC1 [2 datoshi]
    /// 1C : OpCode.SIZE [4 datoshi]
    /// 1D : OpCode.STLOC2 [2 datoshi]
    /// 1E : OpCode.PUSH0 [1 datoshi]
    /// 1F : OpCode.STLOC3 [2 datoshi]
    /// 20 : OpCode.JMP 41 [2 datoshi]
    /// 22 : OpCode.LDLOC1 [2 datoshi]
    /// 23 : OpCode.LDLOC3 [2 datoshi]
    /// 24 : OpCode.PICKITEM [64 datoshi]
    /// 25 : OpCode.STLOC4 [2 datoshi]
    /// 26 : OpCode.LDLOC4 [2 datoshi]
    /// 27 : OpCode.LDARG1 [2 datoshi]
    /// 28 : OpCode.CALLA [512 datoshi]
    /// 29 : OpCode.JMPIFNOT 35 [2 datoshi]
    /// 2B : OpCode.LDLOC0 [2 datoshi]
    /// 2C : OpCode.DUP [2 datoshi]
    /// 2D : OpCode.INC [4 datoshi]
    /// 2E : OpCode.DUP [2 datoshi]
    /// 2F : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 34 : OpCode.JMPGE 04 [2 datoshi]
    /// 36 : OpCode.JMP 0A [2 datoshi]
    /// 38 : OpCode.DUP [2 datoshi]
    /// 39 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3E : OpCode.JMPLE 1E [2 datoshi]
    /// 40 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 49 : OpCode.AND [8 datoshi]
    /// 4A : OpCode.DUP [2 datoshi]
    /// 4B : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 50 : OpCode.JMPLE 0C [2 datoshi]
    /// 52 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 5B : OpCode.SUB [8 datoshi]
    /// 5C : OpCode.STLOC0 [2 datoshi]
    /// 5D : OpCode.DROP [2 datoshi]
    /// 5E : OpCode.LDLOC3 [2 datoshi]
    /// 5F : OpCode.INC [4 datoshi]
    /// 60 : OpCode.STLOC3 [2 datoshi]
    /// 61 : OpCode.LDLOC3 [2 datoshi]
    /// 62 : OpCode.LDLOC2 [2 datoshi]
    /// 63 : OpCode.JMPLT BF [2 datoshi]
    /// 65 : OpCode.LDLOC0 [2 datoshi]
    /// 66 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("count")]
    public abstract BigInteger? Count(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCokAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 89000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("countGreaterThanZero")]
    public abstract BigInteger? CountGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDwpWAAAAeDQDQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHM1 [1 datoshi]
    /// 04 : OpCode.PUSHA 56000000 [4 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.CALL 03 [512 datoshi]
    /// 0C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("firstGreaterThanZero")]
    public abstract BigInteger? FirstGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQEqkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALL 04 [512 datoshi]
    /// 06 : OpCode.NOT [4 datoshi]
    /// 07 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEmpty")]
    public abstract bool? IsEmpty(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUBwnB4SnHKchBzIiJpa850aBALEsBKNXH+//9sNwAASxBR0GxLEVHQz2ucc2tqMN4KDAAAAGg1R////0A=
    /// 00 : OpCode.INITSLOT 0501 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.STLOC1 [2 datoshi]
    /// 08 : OpCode.SIZE [4 datoshi]
    /// 09 : OpCode.STLOC2 [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.STLOC3 [2 datoshi]
    /// 0C : OpCode.JMP 22 [2 datoshi]
    /// 0E : OpCode.LDLOC1 [2 datoshi]
    /// 0F : OpCode.LDLOC3 [2 datoshi]
    /// 10 : OpCode.PICKITEM [64 datoshi]
    /// 11 : OpCode.STLOC4 [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.PUSH0 [1 datoshi]
    /// 14 : OpCode.PUSHNULL [1 datoshi]
    /// 15 : OpCode.PUSH2 [1 datoshi]
    /// 16 : OpCode.PACK [2048 datoshi]
    /// 17 : OpCode.DUP [2 datoshi]
    /// 18 : OpCode.CALL_L 71FEFFFF [512 datoshi]
    /// 1D : OpCode.LDLOC4 [2 datoshi]
    /// 1E : OpCode.CALLT 0000 [32768 datoshi]
    /// 21 : OpCode.OVER [2 datoshi]
    /// 22 : OpCode.PUSH0 [1 datoshi]
    /// 23 : OpCode.ROT [2 datoshi]
    /// 24 : OpCode.SETITEM [8192 datoshi]
    /// 25 : OpCode.LDLOC4 [2 datoshi]
    /// 26 : OpCode.OVER [2 datoshi]
    /// 27 : OpCode.PUSH1 [1 datoshi]
    /// 28 : OpCode.ROT [2 datoshi]
    /// 29 : OpCode.SETITEM [8192 datoshi]
    /// 2A : OpCode.APPEND [8192 datoshi]
    /// 2B : OpCode.LDLOC3 [2 datoshi]
    /// 2C : OpCode.INC [4 datoshi]
    /// 2D : OpCode.STLOC3 [2 datoshi]
    /// 2E : OpCode.LDLOC3 [2 datoshi]
    /// 2F : OpCode.LDLOC2 [2 datoshi]
    /// 30 : OpCode.JMPLT DE [2 datoshi]
    /// 32 : OpCode.PUSHA 0C000000 [4 datoshi]
    /// 37 : OpCode.LDLOC0 [2 datoshi]
    /// 38 : OpCode.CALL_L 47FFFFFF [512 datoshi]
    /// 3D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("selectPersonS")]
    public abstract object? SelectPersonS(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClUAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 55000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("selectTwice")]
    public abstract object? SelectTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDRXwnB4SnHKchBzIkZpa850eRC3Jjd5Sp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgUUiBWhsz2ucc2tqMLpoQA==
    /// 00 : OpCode.INITSLOT 0502 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALL 57 [512 datoshi]
    /// 06 : OpCode.NEWARRAY0 [16 datoshi]
    /// 07 : OpCode.STLOC0 [2 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.DUP [2 datoshi]
    /// 0A : OpCode.STLOC1 [2 datoshi]
    /// 0B : OpCode.SIZE [4 datoshi]
    /// 0C : OpCode.STLOC2 [2 datoshi]
    /// 0D : OpCode.PUSH0 [1 datoshi]
    /// 0E : OpCode.STLOC3 [2 datoshi]
    /// 0F : OpCode.JMP 46 [2 datoshi]
    /// 11 : OpCode.LDLOC1 [2 datoshi]
    /// 12 : OpCode.LDLOC3 [2 datoshi]
    /// 13 : OpCode.PICKITEM [64 datoshi]
    /// 14 : OpCode.STLOC4 [2 datoshi]
    /// 15 : OpCode.LDARG1 [2 datoshi]
    /// 16 : OpCode.PUSH0 [1 datoshi]
    /// 17 : OpCode.GT [8 datoshi]
    /// 18 : OpCode.JMPIFNOT 37 [2 datoshi]
    /// 1A : OpCode.LDARG1 [2 datoshi]
    /// 1B : OpCode.DUP [2 datoshi]
    /// 1C : OpCode.DEC [4 datoshi]
    /// 1D : OpCode.DUP [2 datoshi]
    /// 1E : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 23 : OpCode.JMPGE 04 [2 datoshi]
    /// 25 : OpCode.JMP 0A [2 datoshi]
    /// 27 : OpCode.DUP [2 datoshi]
    /// 28 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2D : OpCode.JMPLE 1E [2 datoshi]
    /// 2F : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 38 : OpCode.AND [8 datoshi]
    /// 39 : OpCode.DUP [2 datoshi]
    /// 3A : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3F : OpCode.JMPLE 0C [2 datoshi]
    /// 41 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 4A : OpCode.SUB [8 datoshi]
    /// 4B : OpCode.STARG1 [2 datoshi]
    /// 4C : OpCode.DROP [2 datoshi]
    /// 4D : OpCode.JMP 05 [2 datoshi]
    /// 4F : OpCode.LDLOC0 [2 datoshi]
    /// 50 : OpCode.LDLOC4 [2 datoshi]
    /// 51 : OpCode.APPEND [8192 datoshi]
    /// 52 : OpCode.LDLOC3 [2 datoshi]
    /// 53 : OpCode.INC [4 datoshi]
    /// 54 : OpCode.STLOC3 [2 datoshi]
    /// 55 : OpCode.LDLOC3 [2 datoshi]
    /// 56 : OpCode.LDLOC2 [2 datoshi]
    /// 57 : OpCode.JMPLT BA [2 datoshi]
    /// 59 : OpCode.LDLOC0 [2 datoshi]
    /// 5A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("skip")]
    public abstract object? Skip(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDReDHNlbGVjdG9yeTVw+P//EHB4SnHKchBzIj1pa850aGx5Np5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGucc2tqMMNoQA==
    /// 00 : OpCode.INITSLOT 0502 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALL 5E [512 datoshi]
    /// 06 : OpCode.PUSHDATA1 73656C6563746F72 [8 datoshi]
    /// 10 : OpCode.LDARG1 [2 datoshi]
    /// 11 : OpCode.CALL_L 70F8FFFF [512 datoshi]
    /// 16 : OpCode.PUSH0 [1 datoshi]
    /// 17 : OpCode.STLOC0 [2 datoshi]
    /// 18 : OpCode.LDARG0 [2 datoshi]
    /// 19 : OpCode.DUP [2 datoshi]
    /// 1A : OpCode.STLOC1 [2 datoshi]
    /// 1B : OpCode.SIZE [4 datoshi]
    /// 1C : OpCode.STLOC2 [2 datoshi]
    /// 1D : OpCode.PUSH0 [1 datoshi]
    /// 1E : OpCode.STLOC3 [2 datoshi]
    /// 1F : OpCode.JMP 3D [2 datoshi]
    /// 21 : OpCode.LDLOC1 [2 datoshi]
    /// 22 : OpCode.LDLOC3 [2 datoshi]
    /// 23 : OpCode.PICKITEM [64 datoshi]
    /// 24 : OpCode.STLOC4 [2 datoshi]
    /// 25 : OpCode.LDLOC0 [2 datoshi]
    /// 26 : OpCode.LDLOC4 [2 datoshi]
    /// 27 : OpCode.LDARG1 [2 datoshi]
    /// 28 : OpCode.CALLA [512 datoshi]
    /// 29 : OpCode.ADD [8 datoshi]
    /// 2A : OpCode.DUP [2 datoshi]
    /// 2B : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 30 : OpCode.JMPGE 04 [2 datoshi]
    /// 32 : OpCode.JMP 0A [2 datoshi]
    /// 34 : OpCode.DUP [2 datoshi]
    /// 35 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3A : OpCode.JMPLE 1E [2 datoshi]
    /// 3C : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 45 : OpCode.AND [8 datoshi]
    /// 46 : OpCode.DUP [2 datoshi]
    /// 47 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 4C : OpCode.JMPLE 0C [2 datoshi]
    /// 4E : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 57 : OpCode.SUB [8 datoshi]
    /// 58 : OpCode.STLOC0 [2 datoshi]
    /// 59 : OpCode.LDLOC3 [2 datoshi]
    /// 5A : OpCode.INC [4 datoshi]
    /// 5B : OpCode.STLOC3 [2 datoshi]
    /// 5C : OpCode.LDLOC3 [2 datoshi]
    /// 5D : OpCode.LDLOC2 [2 datoshi]
    /// 5E : OpCode.JMPLT C3 [2 datoshi]
    /// 60 : OpCode.LDLOC0 [2 datoshi]
    /// 61 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCoQAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 84000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sumTwice")]
    public abstract BigInteger? SumTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDRXwnB4SnHKchBzIkZpa850eRC2JgQiP2hsz3lKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+BRWucc2tqMLpoQA==
    /// 00 : OpCode.INITSLOT 0502 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALL 57 [512 datoshi]
    /// 06 : OpCode.NEWARRAY0 [16 datoshi]
    /// 07 : OpCode.STLOC0 [2 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.DUP [2 datoshi]
    /// 0A : OpCode.STLOC1 [2 datoshi]
    /// 0B : OpCode.SIZE [4 datoshi]
    /// 0C : OpCode.STLOC2 [2 datoshi]
    /// 0D : OpCode.PUSH0 [1 datoshi]
    /// 0E : OpCode.STLOC3 [2 datoshi]
    /// 0F : OpCode.JMP 46 [2 datoshi]
    /// 11 : OpCode.LDLOC1 [2 datoshi]
    /// 12 : OpCode.LDLOC3 [2 datoshi]
    /// 13 : OpCode.PICKITEM [64 datoshi]
    /// 14 : OpCode.STLOC4 [2 datoshi]
    /// 15 : OpCode.LDARG1 [2 datoshi]
    /// 16 : OpCode.PUSH0 [1 datoshi]
    /// 17 : OpCode.LE [8 datoshi]
    /// 18 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 1A : OpCode.JMP 3F [2 datoshi]
    /// 1C : OpCode.LDLOC0 [2 datoshi]
    /// 1D : OpCode.LDLOC4 [2 datoshi]
    /// 1E : OpCode.APPEND [8192 datoshi]
    /// 1F : OpCode.LDARG1 [2 datoshi]
    /// 20 : OpCode.DUP [2 datoshi]
    /// 21 : OpCode.DEC [4 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 28 : OpCode.JMPGE 04 [2 datoshi]
    /// 2A : OpCode.JMP 0A [2 datoshi]
    /// 2C : OpCode.DUP [2 datoshi]
    /// 2D : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 32 : OpCode.JMPLE 1E [2 datoshi]
    /// 34 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 3D : OpCode.AND [8 datoshi]
    /// 3E : OpCode.DUP [2 datoshi]
    /// 3F : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 44 : OpCode.JMPLE 0C [2 datoshi]
    /// 46 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 4F : OpCode.SUB [8 datoshi]
    /// 50 : OpCode.STARG1 [2 datoshi]
    /// 51 : OpCode.DROP [2 datoshi]
    /// 52 : OpCode.LDLOC3 [2 datoshi]
    /// 53 : OpCode.INC [4 datoshi]
    /// 54 : OpCode.STLOC3 [2 datoshi]
    /// 55 : OpCode.LDLOC3 [2 datoshi]
    /// 56 : OpCode.LDLOC2 [2 datoshi]
    /// 57 : OpCode.JMPLT BA [2 datoshi]
    /// 59 : OpCode.LDLOC0 [2 datoshi]
    /// 5A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("take")]
    public abstract object? Take(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUDeDRPDGtleVNlbGVjdG9yeTUP9///DGVsZW1lbnRTZWxlY3Rvcno1+Pb//8hweEpxynIQcyIUaWvOdGx6NkpseTZoU9BFa5xza2ow7GhA
    /// 00 : OpCode.INITSLOT 0503 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALL 4F [512 datoshi]
    /// 06 : OpCode.PUSHDATA1 6B657953656C6563746F72 [8 datoshi]
    /// 13 : OpCode.LDARG1 [2 datoshi]
    /// 14 : OpCode.CALL_L 0FF7FFFF [512 datoshi]
    /// 19 : OpCode.PUSHDATA1 656C656D656E7453656C6563746F72 [8 datoshi]
    /// 2A : OpCode.LDARG2 [2 datoshi]
    /// 2B : OpCode.CALL_L F8F6FFFF [512 datoshi]
    /// 30 : OpCode.NEWMAP [8 datoshi]
    /// 31 : OpCode.STLOC0 [2 datoshi]
    /// 32 : OpCode.LDARG0 [2 datoshi]
    /// 33 : OpCode.DUP [2 datoshi]
    /// 34 : OpCode.STLOC1 [2 datoshi]
    /// 35 : OpCode.SIZE [4 datoshi]
    /// 36 : OpCode.STLOC2 [2 datoshi]
    /// 37 : OpCode.PUSH0 [1 datoshi]
    /// 38 : OpCode.STLOC3 [2 datoshi]
    /// 39 : OpCode.JMP 14 [2 datoshi]
    /// 3B : OpCode.LDLOC1 [2 datoshi]
    /// 3C : OpCode.LDLOC3 [2 datoshi]
    /// 3D : OpCode.PICKITEM [64 datoshi]
    /// 3E : OpCode.STLOC4 [2 datoshi]
    /// 3F : OpCode.LDLOC4 [2 datoshi]
    /// 40 : OpCode.LDARG2 [2 datoshi]
    /// 41 : OpCode.CALLA [512 datoshi]
    /// 42 : OpCode.DUP [2 datoshi]
    /// 43 : OpCode.LDLOC4 [2 datoshi]
    /// 44 : OpCode.LDARG1 [2 datoshi]
    /// 45 : OpCode.CALLA [512 datoshi]
    /// 46 : OpCode.LDLOC0 [2 datoshi]
    /// 47 : OpCode.REVERSE3 [2 datoshi]
    /// 48 : OpCode.SETITEM [8192 datoshi]
    /// 49 : OpCode.DROP [2 datoshi]
    /// 4A : OpCode.LDLOC3 [2 datoshi]
    /// 4B : OpCode.INC [4 datoshi]
    /// 4C : OpCode.STLOC3 [2 datoshi]
    /// 4D : OpCode.LDLOC3 [2 datoshi]
    /// 4E : OpCode.LDLOC2 [2 datoshi]
    /// 4F : OpCode.JMPLT EC [2 datoshi]
    /// 51 : OpCode.LDLOC0 [2 datoshi]
    /// 52 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("toMap")]
    public abstract object? ToMap(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClkAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 59000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("whereGreaterThanZero")]
    public abstract object? WhereGreaterThanZero(IList<object>? array);

    #endregion
}
