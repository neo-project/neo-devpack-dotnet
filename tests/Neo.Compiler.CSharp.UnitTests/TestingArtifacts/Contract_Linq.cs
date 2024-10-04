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
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHA 66000000
    /// 0008 : OpCode.PUSH0
    /// 0009 : OpCode.LDARG0
    /// 000A : OpCode.CALL 03
    /// 000C : OpCode.RET
    /// </remarks>
    [DisplayName("aggregateSum")]
    public abstract BigInteger? AggregateSum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHA 55000000
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.CALL 03
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("allGreaterThanZero")]
    public abstract bool? AllGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.STSFLD0
    /// 0005 : OpCode.PUSHA 09000000
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.CALL A0
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("anyGreaterThan")]
    public abstract bool? AnyGreaterThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHA 57000000
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.CALL 03
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("anyGreaterThanZero")]
    public abstract bool? AnyGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0602
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.CALL_L AF000000
    /// 0009 : OpCode.PUSHDATA1 73656C6563746F72
    /// 0013 : OpCode.LDARG1
    /// 0014 : OpCode.CALL_L BFFDFFFF
    /// 0019 : OpCode.PUSH0
    /// 001A : OpCode.STLOC0
    /// 001B : OpCode.PUSH0
    /// 001C : OpCode.STLOC1
    /// 001D : OpCode.LDARG0
    /// 001E : OpCode.DUP
    /// 001F : OpCode.STLOC2
    /// 0020 : OpCode.SIZE
    /// 0021 : OpCode.STLOC3
    /// 0022 : OpCode.PUSH0
    /// 0023 : OpCode.STLOC4
    /// 0024 : OpCode.JMP 70
    /// 0026 : OpCode.LDLOC2
    /// 0027 : OpCode.LDLOC4
    /// 0028 : OpCode.PICKITEM
    /// 0029 : OpCode.STLOC5
    /// 002A : OpCode.LDLOC0
    /// 002B : OpCode.DUP
    /// 002C : OpCode.INC
    /// 002D : OpCode.DUP
    /// 002E : OpCode.PUSHINT32 00000080
    /// 0033 : OpCode.JMPGE 04
    /// 0035 : OpCode.JMP 0A
    /// 0037 : OpCode.DUP
    /// 0038 : OpCode.PUSHINT32 FFFFFF7F
    /// 003D : OpCode.JMPLE 1E
    /// 003F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0048 : OpCode.AND
    /// 0049 : OpCode.DUP
    /// 004A : OpCode.PUSHINT32 FFFFFF7F
    /// 004F : OpCode.JMPLE 0C
    /// 0051 : OpCode.PUSHINT64 0000000001000000
    /// 005A : OpCode.SUB
    /// 005B : OpCode.STLOC0
    /// 005C : OpCode.DROP
    /// 005D : OpCode.LDLOC1
    /// 005E : OpCode.LDLOC5
    /// 005F : OpCode.LDARG1
    /// 0060 : OpCode.CALLA
    /// 0061 : OpCode.ADD
    /// 0062 : OpCode.DUP
    /// 0063 : OpCode.PUSHINT32 00000080
    /// 0068 : OpCode.JMPGE 04
    /// 006A : OpCode.JMP 0A
    /// 006C : OpCode.DUP
    /// 006D : OpCode.PUSHINT32 FFFFFF7F
    /// 0072 : OpCode.JMPLE 1E
    /// 0074 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 007D : OpCode.AND
    /// 007E : OpCode.DUP
    /// 007F : OpCode.PUSHINT32 FFFFFF7F
    /// 0084 : OpCode.JMPLE 0C
    /// 0086 : OpCode.PUSHINT64 0000000001000000
    /// 008F : OpCode.SUB
    /// 0090 : OpCode.STLOC1
    /// 0091 : OpCode.LDLOC4
    /// 0092 : OpCode.INC
    /// 0093 : OpCode.STLOC4
    /// 0094 : OpCode.LDLOC4
    /// 0095 : OpCode.LDLOC3
    /// 0096 : OpCode.JMPLT 90
    /// 0098 : OpCode.LDLOC0
    /// 0099 : OpCode.PUSH0
    /// 009A : OpCode.EQUAL
    /// 009B : OpCode.JMPIFNOT 14
    /// 009D : OpCode.PUSHDATA1 736F7572636520697320656D707479
    /// 00AE : OpCode.THROW
    /// 00AF : OpCode.LDLOC1
    /// 00B0 : OpCode.LDLOC0
    /// 00B1 : OpCode.DIV
    /// 00B2 : OpCode.RET
    /// </remarks>
    [DisplayName("average")]
    public abstract BigInteger? Average(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHA D6000000
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.CALL 03
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("averageTwice")]
    public abstract BigInteger? AverageTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.STSFLD1
    /// 0005 : OpCode.PUSHA 40000000
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.CALL 03
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("contains")]
    public abstract bool? Contains(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0502
    /// 0003 : OpCode.NEWARRAY0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.STLOC1
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.STLOC2
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.STLOC3
    /// 000C : OpCode.JMP 1F
    /// 000E : OpCode.LDLOC1
    /// 000F : OpCode.LDLOC3
    /// 0010 : OpCode.PICKITEM
    /// 0011 : OpCode.STLOC4
    /// 0012 : OpCode.LDLOC0
    /// 0013 : OpCode.PUSH0
    /// 0014 : OpCode.PUSHNULL
    /// 0015 : OpCode.PUSH2
    /// 0016 : OpCode.PACK
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.CALL 34
    /// 001A : OpCode.LDLOC4
    /// 001B : OpCode.CALLT 0000
    /// 001E : OpCode.OVER
    /// 001F : OpCode.PUSH0
    /// 0020 : OpCode.ROT
    /// 0021 : OpCode.SETITEM
    /// 0022 : OpCode.LDLOC4
    /// 0023 : OpCode.OVER
    /// 0024 : OpCode.PUSH1
    /// 0025 : OpCode.ROT
    /// 0026 : OpCode.SETITEM
    /// 0027 : OpCode.APPEND
    /// 0028 : OpCode.LDLOC3
    /// 0029 : OpCode.INC
    /// 002A : OpCode.STLOC3
    /// 002B : OpCode.LDLOC3
    /// 002C : OpCode.LDLOC2
    /// 002D : OpCode.JMPLT E1
    /// 002F : OpCode.PUSH0
    /// 0030 : OpCode.PUSHNULL
    /// 0031 : OpCode.PUSH2
    /// 0032 : OpCode.PACK
    /// 0033 : OpCode.DUP
    /// 0034 : OpCode.CALL 18
    /// 0036 : OpCode.LDARG1
    /// 0037 : OpCode.CALLT 0000
    /// 003A : OpCode.OVER
    /// 003B : OpCode.PUSH0
    /// 003C : OpCode.ROT
    /// 003D : OpCode.SETITEM
    /// 003E : OpCode.LDARG1
    /// 003F : OpCode.OVER
    /// 0040 : OpCode.PUSH1
    /// 0041 : OpCode.ROT
    /// 0042 : OpCode.SETITEM
    /// 0043 : OpCode.STLOC1
    /// 0044 : OpCode.LDLOC1
    /// 0045 : OpCode.LDLOC0
    /// 0046 : OpCode.CALL_L 66FFFFFF
    /// 004B : OpCode.RET
    /// </remarks>
    [DisplayName("containsPerson")]
    public abstract bool? ContainsPerson(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0502
    /// 0003 : OpCode.NEWARRAY0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.STLOC1
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.STLOC2
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.STLOC3
    /// 000C : OpCode.JMP 1F
    /// 000E : OpCode.LDLOC1
    /// 000F : OpCode.LDLOC3
    /// 0010 : OpCode.PICKITEM
    /// 0011 : OpCode.STLOC4
    /// 0012 : OpCode.LDLOC0
    /// 0013 : OpCode.PUSH0
    /// 0014 : OpCode.PUSHNULL
    /// 0015 : OpCode.PUSH2
    /// 0016 : OpCode.PACK
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.CALL E0
    /// 001A : OpCode.LDLOC4
    /// 001B : OpCode.CALLT 0000
    /// 001E : OpCode.OVER
    /// 001F : OpCode.PUSH0
    /// 0020 : OpCode.ROT
    /// 0021 : OpCode.SETITEM
    /// 0022 : OpCode.LDLOC4
    /// 0023 : OpCode.OVER
    /// 0024 : OpCode.PUSH1
    /// 0025 : OpCode.ROT
    /// 0026 : OpCode.SETITEM
    /// 0027 : OpCode.APPEND
    /// 0028 : OpCode.LDLOC3
    /// 0029 : OpCode.INC
    /// 002A : OpCode.STLOC3
    /// 002B : OpCode.LDLOC3
    /// 002C : OpCode.LDLOC2
    /// 002D : OpCode.JMPLT E1
    /// 002F : OpCode.LDLOC0
    /// 0030 : OpCode.LDARG1
    /// 0031 : OpCode.PICKITEM
    /// 0032 : OpCode.STLOC1
    /// 0033 : OpCode.LDLOC1
    /// 0034 : OpCode.LDLOC0
    /// 0035 : OpCode.CALL_L 23FFFFFF
    /// 003A : OpCode.RET
    /// </remarks>
    [DisplayName("containsPersonIndex")]
    public abstract bool? ContainsPersonIndex(IList<object>? array, BigInteger? targetIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0502
    /// 0003 : OpCode.NEWARRAY0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.STLOC1
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.STLOC2
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.STLOC3
    /// 000C : OpCode.JMP 22
    /// 000E : OpCode.LDLOC1
    /// 000F : OpCode.LDLOC3
    /// 0010 : OpCode.PICKITEM
    /// 0011 : OpCode.STLOC4
    /// 0012 : OpCode.LDLOC0
    /// 0013 : OpCode.NEWSTRUCT0
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.PUSHNULL
    /// 0016 : OpCode.APPEND
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.PUSH0
    /// 0019 : OpCode.APPEND
    /// 001A : OpCode.DUP
    /// 001B : OpCode.CALL 37
    /// 001D : OpCode.LDLOC4
    /// 001E : OpCode.CALLT 0000
    /// 0021 : OpCode.OVER
    /// 0022 : OpCode.PUSH0
    /// 0023 : OpCode.ROT
    /// 0024 : OpCode.SETITEM
    /// 0025 : OpCode.LDLOC4
    /// 0026 : OpCode.OVER
    /// 0027 : OpCode.PUSH1
    /// 0028 : OpCode.ROT
    /// 0029 : OpCode.SETITEM
    /// 002A : OpCode.APPEND
    /// 002B : OpCode.LDLOC3
    /// 002C : OpCode.INC
    /// 002D : OpCode.STLOC3
    /// 002E : OpCode.LDLOC3
    /// 002F : OpCode.LDLOC2
    /// 0030 : OpCode.JMPLT DE
    /// 0032 : OpCode.NEWSTRUCT0
    /// 0033 : OpCode.DUP
    /// 0034 : OpCode.PUSHNULL
    /// 0035 : OpCode.APPEND
    /// 0036 : OpCode.DUP
    /// 0037 : OpCode.PUSH0
    /// 0038 : OpCode.APPEND
    /// 0039 : OpCode.DUP
    /// 003A : OpCode.CALL 18
    /// 003C : OpCode.LDARG1
    /// 003D : OpCode.CALLT 0000
    /// 0040 : OpCode.OVER
    /// 0041 : OpCode.PUSH0
    /// 0042 : OpCode.ROT
    /// 0043 : OpCode.SETITEM
    /// 0044 : OpCode.LDARG1
    /// 0045 : OpCode.OVER
    /// 0046 : OpCode.PUSH1
    /// 0047 : OpCode.ROT
    /// 0048 : OpCode.SETITEM
    /// 0049 : OpCode.STLOC1
    /// 004A : OpCode.LDLOC1
    /// 004B : OpCode.LDLOC0
    /// 004C : OpCode.CALL_L D1FEFFFF
    /// 0051 : OpCode.RET
    /// </remarks>
    [DisplayName("containsPersonS")]
    public abstract bool? ContainsPersonS(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.CALL AF
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("containsText")]
    public abstract bool? ContainsText(IList<object>? array, string? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0502
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.CALL 63
    /// 0006 : OpCode.PUSHDATA1 707265646963617465
    /// 0011 : OpCode.LDARG1
    /// 0012 : OpCode.CALL_L 41FCFFFF
    /// 0017 : OpCode.PUSH0
    /// 0018 : OpCode.STLOC0
    /// 0019 : OpCode.LDARG0
    /// 001A : OpCode.DUP
    /// 001B : OpCode.STLOC1
    /// 001C : OpCode.SIZE
    /// 001D : OpCode.STLOC2
    /// 001E : OpCode.PUSH0
    /// 001F : OpCode.STLOC3
    /// 0020 : OpCode.JMP 41
    /// 0022 : OpCode.LDLOC1
    /// 0023 : OpCode.LDLOC3
    /// 0024 : OpCode.PICKITEM
    /// 0025 : OpCode.STLOC4
    /// 0026 : OpCode.LDLOC4
    /// 0027 : OpCode.LDARG1
    /// 0028 : OpCode.CALLA
    /// 0029 : OpCode.JMPIFNOT 35
    /// 002B : OpCode.LDLOC0
    /// 002C : OpCode.DUP
    /// 002D : OpCode.INC
    /// 002E : OpCode.DUP
    /// 002F : OpCode.PUSHINT32 00000080
    /// 0034 : OpCode.JMPGE 04
    /// 0036 : OpCode.JMP 0A
    /// 0038 : OpCode.DUP
    /// 0039 : OpCode.PUSHINT32 FFFFFF7F
    /// 003E : OpCode.JMPLE 1E
    /// 0040 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0049 : OpCode.AND
    /// 004A : OpCode.DUP
    /// 004B : OpCode.PUSHINT32 FFFFFF7F
    /// 0050 : OpCode.JMPLE 0C
    /// 0052 : OpCode.PUSHINT64 0000000001000000
    /// 005B : OpCode.SUB
    /// 005C : OpCode.STLOC0
    /// 005D : OpCode.DROP
    /// 005E : OpCode.LDLOC3
    /// 005F : OpCode.INC
    /// 0060 : OpCode.STLOC3
    /// 0061 : OpCode.LDLOC3
    /// 0062 : OpCode.LDLOC2
    /// 0063 : OpCode.JMPLT BF
    /// 0065 : OpCode.LDLOC0
    /// 0066 : OpCode.RET
    /// </remarks>
    [DisplayName("count")]
    public abstract BigInteger? Count(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHA 8A000000
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.CALL 03
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("countGreaterThanZero")]
    public abstract BigInteger? CountGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHM1
    /// 0004 : OpCode.PUSHA 57000000
    /// 0009 : OpCode.LDARG0
    /// 000A : OpCode.CALL 03
    /// 000C : OpCode.RET
    /// </remarks>
    [DisplayName("firstGreaterThanZero")]
    public abstract BigInteger? FirstGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.CALL 04
    /// 0006 : OpCode.NOT
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isEmpty")]
    public abstract bool? IsEmpty(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0501
    /// 0003 : OpCode.NEWARRAY0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.STLOC1
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.STLOC2
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.STLOC3
    /// 000C : OpCode.JMP 22
    /// 000E : OpCode.LDLOC1
    /// 000F : OpCode.LDLOC3
    /// 0010 : OpCode.PICKITEM
    /// 0011 : OpCode.STLOC4
    /// 0012 : OpCode.LDLOC0
    /// 0013 : OpCode.PUSH0
    /// 0014 : OpCode.PUSHNULL
    /// 0015 : OpCode.PUSH2
    /// 0016 : OpCode.PACK
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.CALL_L 5FFEFFFF
    /// 001D : OpCode.LDLOC4
    /// 001E : OpCode.CALLT 0000
    /// 0021 : OpCode.OVER
    /// 0022 : OpCode.PUSH0
    /// 0023 : OpCode.ROT
    /// 0024 : OpCode.SETITEM
    /// 0025 : OpCode.LDLOC4
    /// 0026 : OpCode.OVER
    /// 0027 : OpCode.PUSH1
    /// 0028 : OpCode.ROT
    /// 0029 : OpCode.SETITEM
    /// 002A : OpCode.APPEND
    /// 002B : OpCode.LDLOC3
    /// 002C : OpCode.INC
    /// 002D : OpCode.STLOC3
    /// 002E : OpCode.LDLOC3
    /// 002F : OpCode.LDLOC2
    /// 0030 : OpCode.JMPLT DE
    /// 0032 : OpCode.PUSHA 0C000000
    /// 0037 : OpCode.LDLOC0
    /// 0038 : OpCode.CALL_L 46FFFFFF
    /// 003D : OpCode.RET
    /// </remarks>
    [DisplayName("selectPersonS")]
    public abstract object? SelectPersonS(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHA 56000000
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.CALL 03
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("selectTwice")]
    public abstract object? SelectTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0502
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.CALL 57
    /// 0006 : OpCode.NEWARRAY0
    /// 0007 : OpCode.STLOC0
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.STLOC1
    /// 000B : OpCode.SIZE
    /// 000C : OpCode.STLOC2
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.STLOC3
    /// 000F : OpCode.JMP 46
    /// 0011 : OpCode.LDLOC1
    /// 0012 : OpCode.LDLOC3
    /// 0013 : OpCode.PICKITEM
    /// 0014 : OpCode.STLOC4
    /// 0015 : OpCode.LDARG1
    /// 0016 : OpCode.PUSH0
    /// 0017 : OpCode.GT
    /// 0018 : OpCode.JMPIFNOT 37
    /// 001A : OpCode.LDARG1
    /// 001B : OpCode.DUP
    /// 001C : OpCode.DEC
    /// 001D : OpCode.DUP
    /// 001E : OpCode.PUSHINT32 00000080
    /// 0023 : OpCode.JMPGE 04
    /// 0025 : OpCode.JMP 0A
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.PUSHINT32 FFFFFF7F
    /// 002D : OpCode.JMPLE 1E
    /// 002F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0038 : OpCode.AND
    /// 0039 : OpCode.DUP
    /// 003A : OpCode.PUSHINT32 FFFFFF7F
    /// 003F : OpCode.JMPLE 0C
    /// 0041 : OpCode.PUSHINT64 0000000001000000
    /// 004A : OpCode.SUB
    /// 004B : OpCode.STARG1
    /// 004C : OpCode.DROP
    /// 004D : OpCode.JMP 05
    /// 004F : OpCode.LDLOC0
    /// 0050 : OpCode.LDLOC4
    /// 0051 : OpCode.APPEND
    /// 0052 : OpCode.LDLOC3
    /// 0053 : OpCode.INC
    /// 0054 : OpCode.STLOC3
    /// 0055 : OpCode.LDLOC3
    /// 0056 : OpCode.LDLOC2
    /// 0057 : OpCode.JMPLT BA
    /// 0059 : OpCode.LDLOC0
    /// 005A : OpCode.RET
    /// </remarks>
    [DisplayName("skip")]
    public abstract object? Skip(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0502
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.CALL 5E
    /// 0006 : OpCode.PUSHDATA1 73656C6563746F72
    /// 0010 : OpCode.LDARG1
    /// 0011 : OpCode.CALL_L 49F8FFFF
    /// 0016 : OpCode.PUSH0
    /// 0017 : OpCode.STLOC0
    /// 0018 : OpCode.LDARG0
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.STLOC1
    /// 001B : OpCode.SIZE
    /// 001C : OpCode.STLOC2
    /// 001D : OpCode.PUSH0
    /// 001E : OpCode.STLOC3
    /// 001F : OpCode.JMP 3D
    /// 0021 : OpCode.LDLOC1
    /// 0022 : OpCode.LDLOC3
    /// 0023 : OpCode.PICKITEM
    /// 0024 : OpCode.STLOC4
    /// 0025 : OpCode.LDLOC0
    /// 0026 : OpCode.LDLOC4
    /// 0027 : OpCode.LDARG1
    /// 0028 : OpCode.CALLA
    /// 0029 : OpCode.ADD
    /// 002A : OpCode.DUP
    /// 002B : OpCode.PUSHINT32 00000080
    /// 0030 : OpCode.JMPGE 04
    /// 0032 : OpCode.JMP 0A
    /// 0034 : OpCode.DUP
    /// 0035 : OpCode.PUSHINT32 FFFFFF7F
    /// 003A : OpCode.JMPLE 1E
    /// 003C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0045 : OpCode.AND
    /// 0046 : OpCode.DUP
    /// 0047 : OpCode.PUSHINT32 FFFFFF7F
    /// 004C : OpCode.JMPLE 0C
    /// 004E : OpCode.PUSHINT64 0000000001000000
    /// 0057 : OpCode.SUB
    /// 0058 : OpCode.STLOC0
    /// 0059 : OpCode.LDLOC3
    /// 005A : OpCode.INC
    /// 005B : OpCode.STLOC3
    /// 005C : OpCode.LDLOC3
    /// 005D : OpCode.LDLOC2
    /// 005E : OpCode.JMPLT C3
    /// 0060 : OpCode.LDLOC0
    /// 0061 : OpCode.RET
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHA 85000000
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.CALL 03
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("sumTwice")]
    public abstract BigInteger? SumTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0502
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.CALL 57
    /// 0006 : OpCode.NEWARRAY0
    /// 0007 : OpCode.STLOC0
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.STLOC1
    /// 000B : OpCode.SIZE
    /// 000C : OpCode.STLOC2
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.STLOC3
    /// 000F : OpCode.JMP 46
    /// 0011 : OpCode.LDLOC1
    /// 0012 : OpCode.LDLOC3
    /// 0013 : OpCode.PICKITEM
    /// 0014 : OpCode.STLOC4
    /// 0015 : OpCode.LDARG1
    /// 0016 : OpCode.PUSH0
    /// 0017 : OpCode.LE
    /// 0018 : OpCode.JMPIFNOT 04
    /// 001A : OpCode.JMP 3F
    /// 001C : OpCode.LDLOC0
    /// 001D : OpCode.LDLOC4
    /// 001E : OpCode.APPEND
    /// 001F : OpCode.LDARG1
    /// 0020 : OpCode.DUP
    /// 0021 : OpCode.DEC
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.PUSHINT32 00000080
    /// 0028 : OpCode.JMPGE 04
    /// 002A : OpCode.JMP 0A
    /// 002C : OpCode.DUP
    /// 002D : OpCode.PUSHINT32 FFFFFF7F
    /// 0032 : OpCode.JMPLE 1E
    /// 0034 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 003D : OpCode.AND
    /// 003E : OpCode.DUP
    /// 003F : OpCode.PUSHINT32 FFFFFF7F
    /// 0044 : OpCode.JMPLE 0C
    /// 0046 : OpCode.PUSHINT64 0000000001000000
    /// 004F : OpCode.SUB
    /// 0050 : OpCode.STARG1
    /// 0051 : OpCode.DROP
    /// 0052 : OpCode.LDLOC3
    /// 0053 : OpCode.INC
    /// 0054 : OpCode.STLOC3
    /// 0055 : OpCode.LDLOC3
    /// 0056 : OpCode.LDLOC2
    /// 0057 : OpCode.JMPLT BA
    /// 0059 : OpCode.LDLOC0
    /// 005A : OpCode.RET
    /// </remarks>
    [DisplayName("take")]
    public abstract object? Take(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0503
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.CALL 4F
    /// 0006 : OpCode.PUSHDATA1 6B657953656C6563746F72
    /// 0013 : OpCode.LDARG1
    /// 0014 : OpCode.CALL_L DDF6FFFF
    /// 0019 : OpCode.PUSHDATA1 656C656D656E7453656C6563746F72
    /// 002A : OpCode.LDARG2
    /// 002B : OpCode.CALL_L C6F6FFFF
    /// 0030 : OpCode.NEWMAP
    /// 0031 : OpCode.STLOC0
    /// 0032 : OpCode.LDARG0
    /// 0033 : OpCode.DUP
    /// 0034 : OpCode.STLOC1
    /// 0035 : OpCode.SIZE
    /// 0036 : OpCode.STLOC2
    /// 0037 : OpCode.PUSH0
    /// 0038 : OpCode.STLOC3
    /// 0039 : OpCode.JMP 14
    /// 003B : OpCode.LDLOC1
    /// 003C : OpCode.LDLOC3
    /// 003D : OpCode.PICKITEM
    /// 003E : OpCode.STLOC4
    /// 003F : OpCode.LDLOC4
    /// 0040 : OpCode.LDARG2
    /// 0041 : OpCode.CALLA
    /// 0042 : OpCode.DUP
    /// 0043 : OpCode.LDLOC4
    /// 0044 : OpCode.LDARG1
    /// 0045 : OpCode.CALLA
    /// 0046 : OpCode.LDLOC0
    /// 0047 : OpCode.REVERSE3
    /// 0048 : OpCode.SETITEM
    /// 0049 : OpCode.DROP
    /// 004A : OpCode.LDLOC3
    /// 004B : OpCode.INC
    /// 004C : OpCode.STLOC3
    /// 004D : OpCode.LDLOC3
    /// 004E : OpCode.LDLOC2
    /// 004F : OpCode.JMPLT EC
    /// 0051 : OpCode.LDLOC0
    /// 0052 : OpCode.RET
    /// </remarks>
    [DisplayName("toMap")]
    public abstract object? ToMap(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHA 5A000000
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.CALL 03
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("whereGreaterThanZero")]
    public abstract object? WhereGreaterThanZero(IList<object>? array);

    #endregion

}
