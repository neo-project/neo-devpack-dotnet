using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Linq(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Linq"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""aggregateSum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""allGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":158,""safe"":false},{""name"":""isEmpty"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":253,""safe"":false},{""name"":""anyGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":314,""safe"":false},{""name"":""anyGreaterThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":411,""safe"":false},{""name"":""average"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":432,""safe"":false},{""name"":""averageTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":626,""safe"":false},{""name"":""count"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":896,""safe"":false},{""name"":""countGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1010,""safe"":false},{""name"":""contains"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1158,""safe"":false},{""name"":""containsText"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1242,""safe"":false},{""name"":""containsPerson"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1250,""safe"":false},{""name"":""containsPersonIndex"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""targetIndex"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1334,""safe"":false},{""name"":""containsPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1393,""safe"":false},{""name"":""firstGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1479,""safe"":false},{""name"":""selectTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1577,""safe"":false},{""name"":""selectPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1719,""safe"":false},{""name"":""skip"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1812,""safe"":false},{""name"":""sum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1937,""safe"":false},{""name"":""sumTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":2027,""safe"":false},{""name"":""take"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":2216,""safe"":false},{""name"":""toMap"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2341,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2521,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2621,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/UAKVwABCmYAAAAQeDQDQFcEA3g0JgwEZnVuY3o0N3hKcMpxEHIiDmhqznNreXo2gWqccmppMPJ5QFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwACeAuXJhF5DAggaXMgbnVsbIvbKDpAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAEKVQAAAHg0A0BXBAJ4NC4MCXByZWRpY2F0ZXk0lXhKcMpxEHIiEWhqznNreTaqJgQJQGqccmppMO8IQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwABeBC3QFcAAXg0BKpAVwQBeDQXeEpwynEQciIIaGrOcwhAamkw+AlAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkBXAAEKVwAAAHg0A0BXBAJ4NDAMCXByZWRpY2F0ZXk1+f7//3hKcMpxEHIiEGhqznNreTYmBAhAapxyamkw8AlAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkBXAAF4ELdAVwACeWAKCQAAAHg0oEBXAAF4WLdAVwABeDQDQFcGAXg1nQAAABBwEHF4SnLKcxB0Im5qbM51aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaW2eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FsnHRsazCSaBCXJhQMD3NvdXJjZSBpcyBlbXB0eTppaKFAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkBXAAEK1gAAAHg0A0BXBgJ4Na8AAAAMCHNlbGVjdG9yeTW//f//EHAQcXhKcspzEHQicGpsznVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpbXk2nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xbJx0bGswkGgQlyYUDA9zb3VyY2UgaXMgZW1wdHk6aWihQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwABEnigSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAF4NANAVwUBeDRNEHB4SnHKchBzIjxpa850aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFa5xza2owxGhAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkBXAAEKigAAAHg0A0BXBQJ4NGMMCXByZWRpY2F0ZXk1Qfz//xBweEpxynIQcyJBaWvOdGx5NiY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFa5xza2owv2hAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkBXAAF4ELdAVwACeXg0A0BXAAJ5YQpAAAAAeDQDQFcEAng12vz//wwJcHJlZGljYXRleTWg+///eEpwynEQciIQaGrOc2t5NiYECEBqnHJqaTDwCUBXAAF4WZdAVwACeXg0r0BXBQLCcHhKccpyEHMiH2lrznRoEAsSwEo0NGw3AABLEFHQbEsRUdDPa5xza2ow4RALEsBKNBh5NwAASxBR0HlLEVHQcWloNWb///9AVwABeBAL0EBXBQLCcHhKccpyEHMiH2lrznRoEAsSwEo04Gw3AABLEFHQbEsRUdDPa5xza2ow4Wh5znFpaDUj////QFcFAsJweEpxynIQcyIiaWvOdGjFSgvPShDPSjQ3bDcAAEsQUdBsSxFR0M9rnHNrajDexUoLz0oQz0o0GHk3AABLEFHQeUsRUdBxaWg10f7//0BXAAFAVwABDwpXAAAAeDQDQFcEA3g0MAwJcHJlZGljYXRleTVr+v//eEpwynEQciIQaGrOc2t5NiYEa0BqnHJqaTDwekBXAAF4C5cmEwwOc291cmNlIGlzIG51bGw6QFcAAXgQt0BXAAEKVgAAAHg0A0BXBQJ4NC8MCHNlbGVjdG9yeTUL+v//wnB4SnHKchBzIg5pa850aGx5Ns9rnHNrajDyaEBXAAF4C5cmEwwOc291cmNlIGlzIG51bGw6QFcAAXgSoEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwUBwnB4SnHKchBzIiJpa850aBALEsBKNV/+//9sNwAASxBR0GxLEVHQz2ucc2tqMN4KDAAAAGg1Rv///0BXAAHFSgvPShDPSjXD/v//eBDOSxBR0HgRzksRUdBAVwACeXg0A0BXBQJ4NFfCcHhKccpyEHMiRmlrznR5ELcmN3lKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+BRSIFaGzPa5xza2owumhAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkBXAAF4NANAVwUBeDW8+v//EHB4SnHKchBzIjtpa850aGyeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BrnHNrajDFaEBXAAEKhQAAAHg0A0BXBQJ4NF4MCHNlbGVjdG9yeTVJ+P//EHB4SnHKchBzIj1pa850aGx5Np5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGucc2tqMMNoQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwABEnigSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAJ5eDQDQFcFAng0V8JweEpxynIQcyJGaWvOdHkQtiYEIj9obM95Sp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgUVrnHNrajC6aEBXAAF4C5cmEwwOc291cmNlIGlzIG51bGw6QFcBAQoaAAAAeDUH/f//cAqZAAAACpkAAABoNCFAVwABxUoLz0oQz0o1dvz//3g3AABLEFHQeEsRUdBAVwUDeDRPDAtrZXlTZWxlY3Rvcnk13fb//wwPZWxlbWVudFNlbGVjdG9yejXG9v//yHB4SnHKchBzIhRpa850bHo2Smx5NmhT0EVrnHNrajDsaEBXAAF4C5cmEwwOc291cmNlIGlzIG51bGw6QFcAAXhAVwABeBDOQFcAAQpaAAAAeDQDQFcFAng0MwwJcHJlZGljYXRleTVa9v//wnB4SnHKchBzIhFpa850bHk2JgVobM9rnHNrajDvaEBXAAF4C5cmEwwOc291cmNlIGlzIG51bGw6QFcAAXgQt0BWAkB2hJ2t"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("aggregateSum")]
    public abstract BigInteger? AggregateSum(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : PUSHA
    // 0008 : PUSH0
    // 0009 : LDARG0
    // 000A : CALL
    // 000C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("allGreaterThanZero")]
    public abstract bool? AllGreaterThanZero(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : PUSHA
    // 0008 : LDARG0
    // 0009 : CALL
    // 000B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("anyGreaterThan")]
    public abstract bool? AnyGreaterThan(IList<object>? array, BigInteger? target);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : STSFLD0
    // 0005 : PUSHA
    // 000A : LDARG0
    // 000B : CALL
    // 000D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("anyGreaterThanZero")]
    public abstract bool? AnyGreaterThanZero(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : PUSHA
    // 0008 : LDARG0
    // 0009 : CALL
    // 000B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("average")]
    public abstract BigInteger? Average(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : CALL_L
    // 0009 : PUSHDATA1
    // 0013 : LDARG1
    // 0014 : CALL_L
    // 0019 : PUSH0
    // 001A : STLOC0
    // 001B : PUSH0
    // 001C : STLOC1
    // 001D : LDARG0
    // 001E : DUP
    // 001F : STLOC2
    // 0020 : SIZE
    // 0021 : STLOC3
    // 0022 : PUSH0
    // 0023 : STLOC4
    // 0024 : JMP
    // 0026 : LDLOC2
    // 0027 : LDLOC4
    // 0028 : PICKITEM
    // 0029 : STLOC5
    // 002A : LDLOC0
    // 002B : DUP
    // 002C : INC
    // 002D : DUP
    // 002E : PUSHINT32
    // 0033 : JMPGE
    // 0035 : JMP
    // 0037 : DUP
    // 0038 : PUSHINT32
    // 003D : JMPLE
    // 003F : PUSHINT64
    // 0048 : AND
    // 0049 : DUP
    // 004A : PUSHINT32
    // 004F : JMPLE
    // 0051 : PUSHINT64
    // 005A : SUB
    // 005B : STLOC0
    // 005C : DROP
    // 005D : LDLOC1
    // 005E : LDLOC5
    // 005F : LDARG1
    // 0060 : CALLA
    // 0061 : ADD
    // 0062 : DUP
    // 0063 : PUSHINT32
    // 0068 : JMPGE
    // 006A : JMP
    // 006C : DUP
    // 006D : PUSHINT32
    // 0072 : JMPLE
    // 0074 : PUSHINT64
    // 007D : AND
    // 007E : DUP
    // 007F : PUSHINT32
    // 0084 : JMPLE
    // 0086 : PUSHINT64
    // 008F : SUB
    // 0090 : STLOC1
    // 0091 : LDLOC4
    // 0092 : INC
    // 0093 : STLOC4
    // 0094 : LDLOC4
    // 0095 : LDLOC3
    // 0096 : JMPLT
    // 0098 : LDLOC0
    // 0099 : PUSH0
    // 009A : EQUAL
    // 009B : JMPIFNOT
    // 009D : PUSHDATA1
    // 00AE : THROW
    // 00AF : LDLOC1
    // 00B0 : LDLOC0
    // 00B1 : DIV
    // 00B2 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("averageTwice")]
    public abstract BigInteger? AverageTwice(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : PUSHA
    // 0008 : LDARG0
    // 0009 : CALL
    // 000B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("contains")]
    public abstract bool? Contains(IList<object>? array, BigInteger? target);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : STSFLD1
    // 0005 : PUSHA
    // 000A : LDARG0
    // 000B : CALL
    // 000D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("containsPerson")]
    public abstract bool? ContainsPerson(IList<object>? array, BigInteger? target);
    // 0000 : INITSLOT
    // 0003 : NEWARRAY0
    // 0004 : STLOC0
    // 0005 : LDARG0
    // 0006 : DUP
    // 0007 : STLOC1
    // 0008 : SIZE
    // 0009 : STLOC2
    // 000A : PUSH0
    // 000B : STLOC3
    // 000C : JMP
    // 000E : LDLOC1
    // 000F : LDLOC3
    // 0010 : PICKITEM
    // 0011 : STLOC4
    // 0012 : LDLOC0
    // 0013 : PUSH0
    // 0014 : PUSHNULL
    // 0015 : PUSH2
    // 0016 : PACK
    // 0017 : DUP
    // 0018 : CALL
    // 001A : LDLOC4
    // 001B : CALLT
    // 001E : OVER
    // 001F : PUSH0
    // 0020 : ROT
    // 0021 : SETITEM
    // 0022 : LDLOC4
    // 0023 : OVER
    // 0024 : PUSH1
    // 0025 : ROT
    // 0026 : SETITEM
    // 0027 : APPEND
    // 0028 : LDLOC3
    // 0029 : INC
    // 002A : STLOC3
    // 002B : LDLOC3
    // 002C : LDLOC2
    // 002D : JMPLT
    // 002F : PUSH0
    // 0030 : PUSHNULL
    // 0031 : PUSH2
    // 0032 : PACK
    // 0033 : DUP
    // 0034 : CALL
    // 0036 : LDARG1
    // 0037 : CALLT
    // 003A : OVER
    // 003B : PUSH0
    // 003C : ROT
    // 003D : SETITEM
    // 003E : LDARG1
    // 003F : OVER
    // 0040 : PUSH1
    // 0041 : ROT
    // 0042 : SETITEM
    // 0043 : STLOC1
    // 0044 : LDLOC1
    // 0045 : LDLOC0
    // 0046 : CALL_L
    // 004B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("containsPersonIndex")]
    public abstract bool? ContainsPersonIndex(IList<object>? array, BigInteger? targetIndex);
    // 0000 : INITSLOT
    // 0003 : NEWARRAY0
    // 0004 : STLOC0
    // 0005 : LDARG0
    // 0006 : DUP
    // 0007 : STLOC1
    // 0008 : SIZE
    // 0009 : STLOC2
    // 000A : PUSH0
    // 000B : STLOC3
    // 000C : JMP
    // 000E : LDLOC1
    // 000F : LDLOC3
    // 0010 : PICKITEM
    // 0011 : STLOC4
    // 0012 : LDLOC0
    // 0013 : PUSH0
    // 0014 : PUSHNULL
    // 0015 : PUSH2
    // 0016 : PACK
    // 0017 : DUP
    // 0018 : CALL
    // 001A : LDLOC4
    // 001B : CALLT
    // 001E : OVER
    // 001F : PUSH0
    // 0020 : ROT
    // 0021 : SETITEM
    // 0022 : LDLOC4
    // 0023 : OVER
    // 0024 : PUSH1
    // 0025 : ROT
    // 0026 : SETITEM
    // 0027 : APPEND
    // 0028 : LDLOC3
    // 0029 : INC
    // 002A : STLOC3
    // 002B : LDLOC3
    // 002C : LDLOC2
    // 002D : JMPLT
    // 002F : LDLOC0
    // 0030 : LDARG1
    // 0031 : PICKITEM
    // 0032 : STLOC1
    // 0033 : LDLOC1
    // 0034 : LDLOC0
    // 0035 : CALL_L
    // 003A : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("containsPersonS")]
    public abstract bool? ContainsPersonS(IList<object>? array, BigInteger? target);
    // 0000 : INITSLOT
    // 0003 : NEWARRAY0
    // 0004 : STLOC0
    // 0005 : LDARG0
    // 0006 : DUP
    // 0007 : STLOC1
    // 0008 : SIZE
    // 0009 : STLOC2
    // 000A : PUSH0
    // 000B : STLOC3
    // 000C : JMP
    // 000E : LDLOC1
    // 000F : LDLOC3
    // 0010 : PICKITEM
    // 0011 : STLOC4
    // 0012 : LDLOC0
    // 0013 : NEWSTRUCT0
    // 0014 : DUP
    // 0015 : PUSHNULL
    // 0016 : APPEND
    // 0017 : DUP
    // 0018 : PUSH0
    // 0019 : APPEND
    // 001A : DUP
    // 001B : CALL
    // 001D : LDLOC4
    // 001E : CALLT
    // 0021 : OVER
    // 0022 : PUSH0
    // 0023 : ROT
    // 0024 : SETITEM
    // 0025 : LDLOC4
    // 0026 : OVER
    // 0027 : PUSH1
    // 0028 : ROT
    // 0029 : SETITEM
    // 002A : APPEND
    // 002B : LDLOC3
    // 002C : INC
    // 002D : STLOC3
    // 002E : LDLOC3
    // 002F : LDLOC2
    // 0030 : JMPLT
    // 0032 : NEWSTRUCT0
    // 0033 : DUP
    // 0034 : PUSHNULL
    // 0035 : APPEND
    // 0036 : DUP
    // 0037 : PUSH0
    // 0038 : APPEND
    // 0039 : DUP
    // 003A : CALL
    // 003C : LDARG1
    // 003D : CALLT
    // 0040 : OVER
    // 0041 : PUSH0
    // 0042 : ROT
    // 0043 : SETITEM
    // 0044 : LDARG1
    // 0045 : OVER
    // 0046 : PUSH1
    // 0047 : ROT
    // 0048 : SETITEM
    // 0049 : STLOC1
    // 004A : LDLOC1
    // 004B : LDLOC0
    // 004C : CALL_L
    // 0051 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("containsText")]
    public abstract bool? ContainsText(IList<object>? array, string? target);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : LDARG0
    // 0005 : CALL
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("count")]
    public abstract BigInteger? Count(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : CALL
    // 0006 : PUSHDATA1
    // 0011 : LDARG1
    // 0012 : CALL_L
    // 0017 : PUSH0
    // 0018 : STLOC0
    // 0019 : LDARG0
    // 001A : DUP
    // 001B : STLOC1
    // 001C : SIZE
    // 001D : STLOC2
    // 001E : PUSH0
    // 001F : STLOC3
    // 0020 : JMP
    // 0022 : LDLOC1
    // 0023 : LDLOC3
    // 0024 : PICKITEM
    // 0025 : STLOC4
    // 0026 : LDLOC4
    // 0027 : LDARG1
    // 0028 : CALLA
    // 0029 : JMPIFNOT
    // 002B : LDLOC0
    // 002C : DUP
    // 002D : INC
    // 002E : DUP
    // 002F : PUSHINT32
    // 0034 : JMPGE
    // 0036 : JMP
    // 0038 : DUP
    // 0039 : PUSHINT32
    // 003E : JMPLE
    // 0040 : PUSHINT64
    // 0049 : AND
    // 004A : DUP
    // 004B : PUSHINT32
    // 0050 : JMPLE
    // 0052 : PUSHINT64
    // 005B : SUB
    // 005C : STLOC0
    // 005D : DROP
    // 005E : LDLOC3
    // 005F : INC
    // 0060 : STLOC3
    // 0061 : LDLOC3
    // 0062 : LDLOC2
    // 0063 : JMPLT
    // 0065 : LDLOC0
    // 0066 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("countGreaterThanZero")]
    public abstract BigInteger? CountGreaterThanZero(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : PUSHA
    // 0008 : LDARG0
    // 0009 : CALL
    // 000B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("firstGreaterThanZero")]
    public abstract BigInteger? FirstGreaterThanZero(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : PUSHM1
    // 0004 : PUSHA
    // 0009 : LDARG0
    // 000A : CALL
    // 000C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEmpty")]
    public abstract bool? IsEmpty(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : CALL
    // 0006 : NOT
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("selectPersonS")]
    public abstract object? SelectPersonS(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : NEWARRAY0
    // 0004 : STLOC0
    // 0005 : LDARG0
    // 0006 : DUP
    // 0007 : STLOC1
    // 0008 : SIZE
    // 0009 : STLOC2
    // 000A : PUSH0
    // 000B : STLOC3
    // 000C : JMP
    // 000E : LDLOC1
    // 000F : LDLOC3
    // 0010 : PICKITEM
    // 0011 : STLOC4
    // 0012 : LDLOC0
    // 0013 : PUSH0
    // 0014 : PUSHNULL
    // 0015 : PUSH2
    // 0016 : PACK
    // 0017 : DUP
    // 0018 : CALL_L
    // 001D : LDLOC4
    // 001E : CALLT
    // 0021 : OVER
    // 0022 : PUSH0
    // 0023 : ROT
    // 0024 : SETITEM
    // 0025 : LDLOC4
    // 0026 : OVER
    // 0027 : PUSH1
    // 0028 : ROT
    // 0029 : SETITEM
    // 002A : APPEND
    // 002B : LDLOC3
    // 002C : INC
    // 002D : STLOC3
    // 002E : LDLOC3
    // 002F : LDLOC2
    // 0030 : JMPLT
    // 0032 : PUSHA
    // 0037 : LDLOC0
    // 0038 : CALL_L
    // 003D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("selectTwice")]
    public abstract object? SelectTwice(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : PUSHA
    // 0008 : LDARG0
    // 0009 : CALL
    // 000B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("skip")]
    public abstract object? Skip(IList<object>? array, BigInteger? count);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : CALL
    // 0006 : NEWARRAY0
    // 0007 : STLOC0
    // 0008 : LDARG0
    // 0009 : DUP
    // 000A : STLOC1
    // 000B : SIZE
    // 000C : STLOC2
    // 000D : PUSH0
    // 000E : STLOC3
    // 000F : JMP
    // 0011 : LDLOC1
    // 0012 : LDLOC3
    // 0013 : PICKITEM
    // 0014 : STLOC4
    // 0015 : LDARG1
    // 0016 : PUSH0
    // 0017 : GT
    // 0018 : JMPIFNOT
    // 001A : LDARG1
    // 001B : DUP
    // 001C : DEC
    // 001D : DUP
    // 001E : PUSHINT32
    // 0023 : JMPGE
    // 0025 : JMP
    // 0027 : DUP
    // 0028 : PUSHINT32
    // 002D : JMPLE
    // 002F : PUSHINT64
    // 0038 : AND
    // 0039 : DUP
    // 003A : PUSHINT32
    // 003F : JMPLE
    // 0041 : PUSHINT64
    // 004A : SUB
    // 004B : STARG1
    // 004C : DROP
    // 004D : JMP
    // 004F : LDLOC0
    // 0050 : LDLOC4
    // 0051 : APPEND
    // 0052 : LDLOC3
    // 0053 : INC
    // 0054 : STLOC3
    // 0055 : LDLOC3
    // 0056 : LDLOC2
    // 0057 : JMPLT
    // 0059 : LDLOC0
    // 005A : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : CALL
    // 0006 : PUSHDATA1
    // 0010 : LDARG1
    // 0011 : CALL_L
    // 0016 : PUSH0
    // 0017 : STLOC0
    // 0018 : LDARG0
    // 0019 : DUP
    // 001A : STLOC1
    // 001B : SIZE
    // 001C : STLOC2
    // 001D : PUSH0
    // 001E : STLOC3
    // 001F : JMP
    // 0021 : LDLOC1
    // 0022 : LDLOC3
    // 0023 : PICKITEM
    // 0024 : STLOC4
    // 0025 : LDLOC0
    // 0026 : LDLOC4
    // 0027 : LDARG1
    // 0028 : CALLA
    // 0029 : ADD
    // 002A : DUP
    // 002B : PUSHINT32
    // 0030 : JMPGE
    // 0032 : JMP
    // 0034 : DUP
    // 0035 : PUSHINT32
    // 003A : JMPLE
    // 003C : PUSHINT64
    // 0045 : AND
    // 0046 : DUP
    // 0047 : PUSHINT32
    // 004C : JMPLE
    // 004E : PUSHINT64
    // 0057 : SUB
    // 0058 : STLOC0
    // 0059 : LDLOC3
    // 005A : INC
    // 005B : STLOC3
    // 005C : LDLOC3
    // 005D : LDLOC2
    // 005E : JMPLT
    // 0060 : LDLOC0
    // 0061 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sumTwice")]
    public abstract BigInteger? SumTwice(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : PUSHA
    // 0008 : LDARG0
    // 0009 : CALL
    // 000B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("take")]
    public abstract object? Take(IList<object>? array, BigInteger? count);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : CALL
    // 0006 : NEWARRAY0
    // 0007 : STLOC0
    // 0008 : LDARG0
    // 0009 : DUP
    // 000A : STLOC1
    // 000B : SIZE
    // 000C : STLOC2
    // 000D : PUSH0
    // 000E : STLOC3
    // 000F : JMP
    // 0011 : LDLOC1
    // 0012 : LDLOC3
    // 0013 : PICKITEM
    // 0014 : STLOC4
    // 0015 : LDARG1
    // 0016 : PUSH0
    // 0017 : LE
    // 0018 : JMPIFNOT
    // 001A : JMP
    // 001C : LDLOC0
    // 001D : LDLOC4
    // 001E : APPEND
    // 001F : LDARG1
    // 0020 : DUP
    // 0021 : DEC
    // 0022 : DUP
    // 0023 : PUSHINT32
    // 0028 : JMPGE
    // 002A : JMP
    // 002C : DUP
    // 002D : PUSHINT32
    // 0032 : JMPLE
    // 0034 : PUSHINT64
    // 003D : AND
    // 003E : DUP
    // 003F : PUSHINT32
    // 0044 : JMPLE
    // 0046 : PUSHINT64
    // 004F : SUB
    // 0050 : STARG1
    // 0051 : DROP
    // 0052 : LDLOC3
    // 0053 : INC
    // 0054 : STLOC3
    // 0055 : LDLOC3
    // 0056 : LDLOC2
    // 0057 : JMPLT
    // 0059 : LDLOC0
    // 005A : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("toMap")]
    public abstract object? ToMap(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : CALL
    // 0006 : PUSHDATA1
    // 0013 : LDARG1
    // 0014 : CALL_L
    // 0019 : PUSHDATA1
    // 002A : LDARG2
    // 002B : CALL_L
    // 0030 : NEWMAP
    // 0031 : STLOC0
    // 0032 : LDARG0
    // 0033 : DUP
    // 0034 : STLOC1
    // 0035 : SIZE
    // 0036 : STLOC2
    // 0037 : PUSH0
    // 0038 : STLOC3
    // 0039 : JMP
    // 003B : LDLOC1
    // 003C : LDLOC3
    // 003D : PICKITEM
    // 003E : STLOC4
    // 003F : LDLOC4
    // 0040 : LDARG2
    // 0041 : CALLA
    // 0042 : DUP
    // 0043 : LDLOC4
    // 0044 : LDARG1
    // 0045 : CALLA
    // 0046 : LDLOC0
    // 0047 : REVERSE3
    // 0048 : SETITEM
    // 0049 : DROP
    // 004A : LDLOC3
    // 004B : INC
    // 004C : STLOC3
    // 004D : LDLOC3
    // 004E : LDLOC2
    // 004F : JMPLT
    // 0051 : LDLOC0
    // 0052 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("whereGreaterThanZero")]
    public abstract object? WhereGreaterThanZero(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : PUSHA
    // 0008 : LDARG0
    // 0009 : CALL
    // 000B : RET

    #endregion

}
