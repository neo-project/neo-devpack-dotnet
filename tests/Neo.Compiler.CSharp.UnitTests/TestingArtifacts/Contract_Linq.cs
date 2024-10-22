using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Linq(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Linq"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""aggregateSum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""allGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":158,""safe"":false},{""name"":""isEmpty"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":253,""safe"":false},{""name"":""anyGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":314,""safe"":false},{""name"":""anyGreaterThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":411,""safe"":false},{""name"":""average"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":432,""safe"":false},{""name"":""averageTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":626,""safe"":false},{""name"":""count"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":896,""safe"":false},{""name"":""countGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1010,""safe"":false},{""name"":""contains"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1158,""safe"":false},{""name"":""containsText"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1242,""safe"":false},{""name"":""containsPerson"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1250,""safe"":false},{""name"":""containsPersonIndex"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""targetIndex"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1334,""safe"":false},{""name"":""containsPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1393,""safe"":false},{""name"":""firstGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1469,""safe"":false},{""name"":""selectTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1567,""safe"":false},{""name"":""selectPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1709,""safe"":false},{""name"":""skip"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1796,""safe"":false},{""name"":""sum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1921,""safe"":false},{""name"":""sumTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":2011,""safe"":false},{""name"":""take"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":2200,""safe"":false},{""name"":""toMap"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2325,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":2499,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2599,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/SoKVwABCmYAAAAQeDQDQFcEA3g0JgwEZnVuY3o0N3hKcMpxEHIiDmhqznNreXo2gWqccmppMPJ5QFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwACeAuXJhF5DAggaXMgbnVsbIvbKDpAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAEKVQAAAHg0A0BXBAJ4NC4MCXByZWRpY2F0ZXk0lXhKcMpxEHIiEWhqznNreTaqJgQJQGqccmppMO8IQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwABeBC3QFcAAXg0BKpAVwQBeDQXeEpwynEQciIIaGrOcwhAamkw+AlAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkBXAAEKVwAAAHg0A0BXBAJ4NDAMCXByZWRpY2F0ZXk1+f7//3hKcMpxEHIiEGhqznNreTYmBAhAapxyamkw8AlAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkBXAAF4ELdAVwACeWAKCQAAAHg0oEBXAAF4WLdAVwABeDQDQFcGAXg1nQAAABBwEHF4SnLKcxB0Im5qbM51aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaW2eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FsnHRsazCSaBCXJhQMD3NvdXJjZSBpcyBlbXB0eTppaKFAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkBXAAEK1gAAAHg0A0BXBgJ4Na8AAAAMCHNlbGVjdG9yeTW//f//EHAQcXhKcspzEHQicGpsznVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpbXk2nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xbJx0bGswkGgQlyYUDA9zb3VyY2UgaXMgZW1wdHk6aWihQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwABEnigSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAF4NANAVwUBeDRNEHB4SnHKchBzIjxpa850aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFa5xza2owxGhAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkBXAAEKigAAAHg0A0BXBQJ4NGMMCXByZWRpY2F0ZXk1Qfz//xBweEpxynIQcyJBaWvOdGx5NiY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFa5xza2owv2hAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkBXAAF4ELdAVwACeXg0A0BXAAJ5YQpAAAAAeDQDQFcEAng12vz//wwJcHJlZGljYXRleTWg+///eEpwynEQciIQaGrOc2t5NiYECEBqnHJqaTDwCUBXAAF4WZdAVwACeXg0r0BXBQLCcHhKccpyEHMiH2lrznRoEAsSwEo0NGw3AABLEFHQbEsRUdDPa5xza2ow4RALEsBKNBh5NwAASxBR0HlLEVHQcWloNWb///9AVwABeBAL0EBXBQLCcHhKccpyEHMiH2lrznRoEAsSwEo04Gw3AABLEFHQbEsRUdDPa5xza2ow4Wh5znFpaDUj////QFcFAsJweEpxynIQcyIfaWvOdGjFSgvPShDPbDcAAEsQUdBsSxFR0M9rnHNrajDhxUoLz0oQz3k3AABLEFHQeUsRUdBxaWg11/7//0BXAAEPClcAAAB4NANAVwQDeDQwDAlwcmVkaWNhdGV5NXX6//94SnDKcRByIhBoas5za3k2JgRrQGqccmppMPB6QFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwABeBC3QFcAAQpWAAAAeDQDQFcFAng0LwwIc2VsZWN0b3J5NRX6///CcHhKccpyEHMiDmlrznRobHk2z2ucc2tqMPJoQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwABeBKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXBQHCcHhKccpyEHMiImlrznRoEAsSwEo1af7//2w3AABLEFHQbEsRUdDPa5xza2ow3goMAAAAaDVG////QFcAAcVKC89KEM94EM5LEFHQeBHOSxFR0EBXAAJ5eDQDQFcFAng0V8JweEpxynIQcyJGaWvOdHkQtyY3eUqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4FFIgVobM9rnHNrajC6aEBXAAF4C5cmEwwOc291cmNlIGlzIG51bGw6QFcAAXg0A0BXBQF4Ncz6//8QcHhKccpyEHMiO2lrznRobJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGucc2tqMMVoQFcAAQqFAAAAeDQDQFcFAng0XgwIc2VsZWN0b3J5NVn4//8QcHhKccpyEHMiPWlrznRobHk2nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wa5xza2oww2hAVwABeAuXJhMMDnNvdXJjZSBpcyBudWxsOkBXAAESeKBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAAnl4NANAVwUCeDRXwnB4SnHKchBzIkZpa850eRC2JgQiP2hsz3lKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+BRWucc2tqMLpoQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwEBChoAAAB4NQ39//9wCpMAAAAKkwAAAGg0G0BXAAHFSgvPShDPeDcAAEsQUdB4SxFR0EBXBQN4NE8MC2tleVNlbGVjdG9yeTXz9v//DA9lbGVtZW50U2VsZWN0b3J6Ndz2///IcHhKccpyEHMiFGlrznRsejZKbHk2aFPQRWucc2tqMOxoQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwABeEBXAAF4EM5AVwABCloAAAB4NANAVwUCeDQzDAlwcmVkaWNhdGV5NXD2///CcHhKccpyEHMiEWlrznRseTYmBWhsz2ucc2tqMO9oQFcAAXgLlyYTDA5zb3VyY2UgaXMgbnVsbDpAVwABeBC3QFYCQD/Si6g="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCmYAAAAQeDQDQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 66000000
    /// 08 : OpCode.PUSH0
    /// 09 : OpCode.LDARG0
    /// 0A : OpCode.CALL 03
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("aggregateSum")]
    public abstract BigInteger? AggregateSum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClUAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 55000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("allGreaterThanZero")]
    public abstract bool? AllGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWAKCQAAAHg0oEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.STSFLD0
    /// 05 : OpCode.PUSHA 09000000
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.CALL A0
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("anyGreaterThan")]
    public abstract bool? AnyGreaterThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClcAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 57000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("anyGreaterThanZero")]
    public abstract bool? AnyGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYCeDWvAAAADHNlbGVjdG9yeTW//f//EHAQcXhKcspzEHQicGpsznVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpbXk2nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xbJx0bGswkGgQlyYUDHNvdXJjZSBpcyBlbXB0eTppaKFA
    /// 00 : OpCode.INITSLOT 0602
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL_L AF000000
    /// 09 : OpCode.PUSHDATA1 73656C6563746F72
    /// 13 : OpCode.LDARG1
    /// 14 : OpCode.CALL_L BFFDFFFF
    /// 19 : OpCode.PUSH0
    /// 1A : OpCode.STLOC0
    /// 1B : OpCode.PUSH0
    /// 1C : OpCode.STLOC1
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.DUP
    /// 1F : OpCode.STLOC2
    /// 20 : OpCode.SIZE
    /// 21 : OpCode.STLOC3
    /// 22 : OpCode.PUSH0
    /// 23 : OpCode.STLOC4
    /// 24 : OpCode.JMP 70
    /// 26 : OpCode.LDLOC2
    /// 27 : OpCode.LDLOC4
    /// 28 : OpCode.PICKITEM
    /// 29 : OpCode.STLOC5
    /// 2A : OpCode.LDLOC0
    /// 2B : OpCode.DUP
    /// 2C : OpCode.INC
    /// 2D : OpCode.DUP
    /// 2E : OpCode.PUSHINT32 00000080
    /// 33 : OpCode.JMPGE 04
    /// 35 : OpCode.JMP 0A
    /// 37 : OpCode.DUP
    /// 38 : OpCode.PUSHINT32 FFFFFF7F
    /// 3D : OpCode.JMPLE 1E
    /// 3F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 48 : OpCode.AND
    /// 49 : OpCode.DUP
    /// 4A : OpCode.PUSHINT32 FFFFFF7F
    /// 4F : OpCode.JMPLE 0C
    /// 51 : OpCode.PUSHINT64 0000000001000000
    /// 5A : OpCode.SUB
    /// 5B : OpCode.STLOC0
    /// 5C : OpCode.DROP
    /// 5D : OpCode.LDLOC1
    /// 5E : OpCode.LDLOC5
    /// 5F : OpCode.LDARG1
    /// 60 : OpCode.CALLA
    /// 61 : OpCode.ADD
    /// 62 : OpCode.DUP
    /// 63 : OpCode.PUSHINT32 00000080
    /// 68 : OpCode.JMPGE 04
    /// 6A : OpCode.JMP 0A
    /// 6C : OpCode.DUP
    /// 6D : OpCode.PUSHINT32 FFFFFF7F
    /// 72 : OpCode.JMPLE 1E
    /// 74 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 7D : OpCode.AND
    /// 7E : OpCode.DUP
    /// 7F : OpCode.PUSHINT32 FFFFFF7F
    /// 84 : OpCode.JMPLE 0C
    /// 86 : OpCode.PUSHINT64 0000000001000000
    /// 8F : OpCode.SUB
    /// 90 : OpCode.STLOC1
    /// 91 : OpCode.LDLOC4
    /// 92 : OpCode.INC
    /// 93 : OpCode.STLOC4
    /// 94 : OpCode.LDLOC4
    /// 95 : OpCode.LDLOC3
    /// 96 : OpCode.JMPLT 90
    /// 98 : OpCode.LDLOC0
    /// 99 : OpCode.PUSH0
    /// 9A : OpCode.EQUAL
    /// 9B : OpCode.JMPIFNOT 14
    /// 9D : OpCode.PUSHDATA1 736F7572636520697320656D707479
    /// AE : OpCode.THROW
    /// AF : OpCode.LDLOC1
    /// B0 : OpCode.LDLOC0
    /// B1 : OpCode.DIV
    /// B2 : OpCode.RET
    /// </remarks>
    [DisplayName("average")]
    public abstract BigInteger? Average(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCtYAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA D6000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("averageTwice")]
    public abstract BigInteger? AverageTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWEKQAAAAHg0A0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.STSFLD1
    /// 05 : OpCode.PUSHA 40000000
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.CALL 03
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("contains")]
    public abstract bool? Contains(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIh9pa850aBALEsBKNDRsNwAASxBR0GxLEVHQz2ucc2tqMOEQCxLASjQYeTcAAEsQUdB5SxFR0HFpaDVm////QA==
    /// 00 : OpCode.INITSLOT 0502
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.DUP
    /// 07 : OpCode.STLOC1
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.STLOC2
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC3
    /// 0C : OpCode.JMP 1F
    /// 0E : OpCode.LDLOC1
    /// 0F : OpCode.LDLOC3
    /// 10 : OpCode.PICKITEM
    /// 11 : OpCode.STLOC4
    /// 12 : OpCode.LDLOC0
    /// 13 : OpCode.PUSH0
    /// 14 : OpCode.PUSHNULL
    /// 15 : OpCode.PUSH2
    /// 16 : OpCode.PACK
    /// 17 : OpCode.DUP
    /// 18 : OpCode.CALL 34
    /// 1A : OpCode.LDLOC4
    /// 1B : OpCode.CALLT 0000
    /// 1E : OpCode.OVER
    /// 1F : OpCode.PUSH0
    /// 20 : OpCode.ROT
    /// 21 : OpCode.SETITEM
    /// 22 : OpCode.LDLOC4
    /// 23 : OpCode.OVER
    /// 24 : OpCode.PUSH1
    /// 25 : OpCode.ROT
    /// 26 : OpCode.SETITEM
    /// 27 : OpCode.APPEND
    /// 28 : OpCode.LDLOC3
    /// 29 : OpCode.INC
    /// 2A : OpCode.STLOC3
    /// 2B : OpCode.LDLOC3
    /// 2C : OpCode.LDLOC2
    /// 2D : OpCode.JMPLT E1
    /// 2F : OpCode.PUSH0
    /// 30 : OpCode.PUSHNULL
    /// 31 : OpCode.PUSH2
    /// 32 : OpCode.PACK
    /// 33 : OpCode.DUP
    /// 34 : OpCode.CALL 18
    /// 36 : OpCode.LDARG1
    /// 37 : OpCode.CALLT 0000
    /// 3A : OpCode.OVER
    /// 3B : OpCode.PUSH0
    /// 3C : OpCode.ROT
    /// 3D : OpCode.SETITEM
    /// 3E : OpCode.LDARG1
    /// 3F : OpCode.OVER
    /// 40 : OpCode.PUSH1
    /// 41 : OpCode.ROT
    /// 42 : OpCode.SETITEM
    /// 43 : OpCode.STLOC1
    /// 44 : OpCode.LDLOC1
    /// 45 : OpCode.LDLOC0
    /// 46 : OpCode.CALL_L 66FFFFFF
    /// 4B : OpCode.RET
    /// </remarks>
    [DisplayName("containsPerson")]
    public abstract bool? ContainsPerson(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIh9pa850aBALEsBKNOBsNwAASxBR0GxLEVHQz2ucc2tqMOFoec5xaWg1I////0A=
    /// 00 : OpCode.INITSLOT 0502
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.DUP
    /// 07 : OpCode.STLOC1
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.STLOC2
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC3
    /// 0C : OpCode.JMP 1F
    /// 0E : OpCode.LDLOC1
    /// 0F : OpCode.LDLOC3
    /// 10 : OpCode.PICKITEM
    /// 11 : OpCode.STLOC4
    /// 12 : OpCode.LDLOC0
    /// 13 : OpCode.PUSH0
    /// 14 : OpCode.PUSHNULL
    /// 15 : OpCode.PUSH2
    /// 16 : OpCode.PACK
    /// 17 : OpCode.DUP
    /// 18 : OpCode.CALL E0
    /// 1A : OpCode.LDLOC4
    /// 1B : OpCode.CALLT 0000
    /// 1E : OpCode.OVER
    /// 1F : OpCode.PUSH0
    /// 20 : OpCode.ROT
    /// 21 : OpCode.SETITEM
    /// 22 : OpCode.LDLOC4
    /// 23 : OpCode.OVER
    /// 24 : OpCode.PUSH1
    /// 25 : OpCode.ROT
    /// 26 : OpCode.SETITEM
    /// 27 : OpCode.APPEND
    /// 28 : OpCode.LDLOC3
    /// 29 : OpCode.INC
    /// 2A : OpCode.STLOC3
    /// 2B : OpCode.LDLOC3
    /// 2C : OpCode.LDLOC2
    /// 2D : OpCode.JMPLT E1
    /// 2F : OpCode.LDLOC0
    /// 30 : OpCode.LDARG1
    /// 31 : OpCode.PICKITEM
    /// 32 : OpCode.STLOC1
    /// 33 : OpCode.LDLOC1
    /// 34 : OpCode.LDLOC0
    /// 35 : OpCode.CALL_L 23FFFFFF
    /// 3A : OpCode.RET
    /// </remarks>
    [DisplayName("containsPersonIndex")]
    public abstract bool? ContainsPersonIndex(IList<object>? array, BigInteger? targetIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIh9pa850aMVKC89KEM9sNwAASxBR0GxLEVHQz2ucc2tqMOHFSgvPShDPeTcAAEsQUdB5SxFR0HFpaDXX/v//QA==
    /// 00 : OpCode.INITSLOT 0502
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.DUP
    /// 07 : OpCode.STLOC1
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.STLOC2
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC3
    /// 0C : OpCode.JMP 1F
    /// 0E : OpCode.LDLOC1
    /// 0F : OpCode.LDLOC3
    /// 10 : OpCode.PICKITEM
    /// 11 : OpCode.STLOC4
    /// 12 : OpCode.LDLOC0
    /// 13 : OpCode.NEWSTRUCT0
    /// 14 : OpCode.DUP
    /// 15 : OpCode.PUSHNULL
    /// 16 : OpCode.APPEND
    /// 17 : OpCode.DUP
    /// 18 : OpCode.PUSH0
    /// 19 : OpCode.APPEND
    /// 1A : OpCode.LDLOC4
    /// 1B : OpCode.CALLT 0000
    /// 1E : OpCode.OVER
    /// 1F : OpCode.PUSH0
    /// 20 : OpCode.ROT
    /// 21 : OpCode.SETITEM
    /// 22 : OpCode.LDLOC4
    /// 23 : OpCode.OVER
    /// 24 : OpCode.PUSH1
    /// 25 : OpCode.ROT
    /// 26 : OpCode.SETITEM
    /// 27 : OpCode.APPEND
    /// 28 : OpCode.LDLOC3
    /// 29 : OpCode.INC
    /// 2A : OpCode.STLOC3
    /// 2B : OpCode.LDLOC3
    /// 2C : OpCode.LDLOC2
    /// 2D : OpCode.JMPLT E1
    /// 2F : OpCode.NEWSTRUCT0
    /// 30 : OpCode.DUP
    /// 31 : OpCode.PUSHNULL
    /// 32 : OpCode.APPEND
    /// 33 : OpCode.DUP
    /// 34 : OpCode.PUSH0
    /// 35 : OpCode.APPEND
    /// 36 : OpCode.LDARG1
    /// 37 : OpCode.CALLT 0000
    /// 3A : OpCode.OVER
    /// 3B : OpCode.PUSH0
    /// 3C : OpCode.ROT
    /// 3D : OpCode.SETITEM
    /// 3E : OpCode.LDARG1
    /// 3F : OpCode.OVER
    /// 40 : OpCode.PUSH1
    /// 41 : OpCode.ROT
    /// 42 : OpCode.SETITEM
    /// 43 : OpCode.STLOC1
    /// 44 : OpCode.LDLOC1
    /// 45 : OpCode.LDLOC0
    /// 46 : OpCode.CALL_L D7FEFFFF
    /// 4B : OpCode.RET
    /// </remarks>
    [DisplayName("containsPersonS")]
    public abstract bool? ContainsPersonS(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0r0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.CALL AF
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("containsText")]
    public abstract bool? ContainsText(IList<object>? array, string? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDRjDHByZWRpY2F0ZXk1Qfz//xBweEpxynIQcyJBaWvOdGx5NiY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFa5xza2owv2hA
    /// 00 : OpCode.INITSLOT 0502
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 63
    /// 06 : OpCode.PUSHDATA1 707265646963617465
    /// 11 : OpCode.LDARG1
    /// 12 : OpCode.CALL_L 41FCFFFF
    /// 17 : OpCode.PUSH0
    /// 18 : OpCode.STLOC0
    /// 19 : OpCode.LDARG0
    /// 1A : OpCode.DUP
    /// 1B : OpCode.STLOC1
    /// 1C : OpCode.SIZE
    /// 1D : OpCode.STLOC2
    /// 1E : OpCode.PUSH0
    /// 1F : OpCode.STLOC3
    /// 20 : OpCode.JMP 41
    /// 22 : OpCode.LDLOC1
    /// 23 : OpCode.LDLOC3
    /// 24 : OpCode.PICKITEM
    /// 25 : OpCode.STLOC4
    /// 26 : OpCode.LDLOC4
    /// 27 : OpCode.LDARG1
    /// 28 : OpCode.CALLA
    /// 29 : OpCode.JMPIFNOT 35
    /// 2B : OpCode.LDLOC0
    /// 2C : OpCode.DUP
    /// 2D : OpCode.INC
    /// 2E : OpCode.DUP
    /// 2F : OpCode.PUSHINT32 00000080
    /// 34 : OpCode.JMPGE 04
    /// 36 : OpCode.JMP 0A
    /// 38 : OpCode.DUP
    /// 39 : OpCode.PUSHINT32 FFFFFF7F
    /// 3E : OpCode.JMPLE 1E
    /// 40 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 49 : OpCode.AND
    /// 4A : OpCode.DUP
    /// 4B : OpCode.PUSHINT32 FFFFFF7F
    /// 50 : OpCode.JMPLE 0C
    /// 52 : OpCode.PUSHINT64 0000000001000000
    /// 5B : OpCode.SUB
    /// 5C : OpCode.STLOC0
    /// 5D : OpCode.DROP
    /// 5E : OpCode.LDLOC3
    /// 5F : OpCode.INC
    /// 60 : OpCode.STLOC3
    /// 61 : OpCode.LDLOC3
    /// 62 : OpCode.LDLOC2
    /// 63 : OpCode.JMPLT BF
    /// 65 : OpCode.LDLOC0
    /// 66 : OpCode.RET
    /// </remarks>
    [DisplayName("count")]
    public abstract BigInteger? Count(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCooAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 8A000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("countGreaterThanZero")]
    public abstract BigInteger? CountGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDwpXAAAAeDQDQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHM1
    /// 04 : OpCode.PUSHA 57000000
    /// 09 : OpCode.LDARG0
    /// 0A : OpCode.CALL 03
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("firstGreaterThanZero")]
    public abstract BigInteger? FirstGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQEqkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 04
    /// 06 : OpCode.NOT
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isEmpty")]
    public abstract bool? IsEmpty(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUBwnB4SnHKchBzIiJpa850aBALEsBKNWn+//9sNwAASxBR0GxLEVHQz2ucc2tqMN4KDAAAAGg1Rv///0A=
    /// 00 : OpCode.INITSLOT 0501
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.DUP
    /// 07 : OpCode.STLOC1
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.STLOC2
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC3
    /// 0C : OpCode.JMP 22
    /// 0E : OpCode.LDLOC1
    /// 0F : OpCode.LDLOC3
    /// 10 : OpCode.PICKITEM
    /// 11 : OpCode.STLOC4
    /// 12 : OpCode.LDLOC0
    /// 13 : OpCode.PUSH0
    /// 14 : OpCode.PUSHNULL
    /// 15 : OpCode.PUSH2
    /// 16 : OpCode.PACK
    /// 17 : OpCode.DUP
    /// 18 : OpCode.CALL_L 69FEFFFF
    /// 1D : OpCode.LDLOC4
    /// 1E : OpCode.CALLT 0000
    /// 21 : OpCode.OVER
    /// 22 : OpCode.PUSH0
    /// 23 : OpCode.ROT
    /// 24 : OpCode.SETITEM
    /// 25 : OpCode.LDLOC4
    /// 26 : OpCode.OVER
    /// 27 : OpCode.PUSH1
    /// 28 : OpCode.ROT
    /// 29 : OpCode.SETITEM
    /// 2A : OpCode.APPEND
    /// 2B : OpCode.LDLOC3
    /// 2C : OpCode.INC
    /// 2D : OpCode.STLOC3
    /// 2E : OpCode.LDLOC3
    /// 2F : OpCode.LDLOC2
    /// 30 : OpCode.JMPLT DE
    /// 32 : OpCode.PUSHA 0C000000
    /// 37 : OpCode.LDLOC0
    /// 38 : OpCode.CALL_L 46FFFFFF
    /// 3D : OpCode.RET
    /// </remarks>
    [DisplayName("selectPersonS")]
    public abstract object? SelectPersonS(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClYAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 56000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("selectTwice")]
    public abstract object? SelectTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDRXwnB4SnHKchBzIkZpa850eRC3Jjd5Sp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgUUiBWhsz2ucc2tqMLpoQA==
    /// 00 : OpCode.INITSLOT 0502
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 57
    /// 06 : OpCode.NEWARRAY0
    /// 07 : OpCode.STLOC0
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.DUP
    /// 0A : OpCode.STLOC1
    /// 0B : OpCode.SIZE
    /// 0C : OpCode.STLOC2
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.STLOC3
    /// 0F : OpCode.JMP 46
    /// 11 : OpCode.LDLOC1
    /// 12 : OpCode.LDLOC3
    /// 13 : OpCode.PICKITEM
    /// 14 : OpCode.STLOC4
    /// 15 : OpCode.LDARG1
    /// 16 : OpCode.PUSH0
    /// 17 : OpCode.GT
    /// 18 : OpCode.JMPIFNOT 37
    /// 1A : OpCode.LDARG1
    /// 1B : OpCode.DUP
    /// 1C : OpCode.DEC
    /// 1D : OpCode.DUP
    /// 1E : OpCode.PUSHINT32 00000080
    /// 23 : OpCode.JMPGE 04
    /// 25 : OpCode.JMP 0A
    /// 27 : OpCode.DUP
    /// 28 : OpCode.PUSHINT32 FFFFFF7F
    /// 2D : OpCode.JMPLE 1E
    /// 2F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 38 : OpCode.AND
    /// 39 : OpCode.DUP
    /// 3A : OpCode.PUSHINT32 FFFFFF7F
    /// 3F : OpCode.JMPLE 0C
    /// 41 : OpCode.PUSHINT64 0000000001000000
    /// 4A : OpCode.SUB
    /// 4B : OpCode.STARG1
    /// 4C : OpCode.DROP
    /// 4D : OpCode.JMP 05
    /// 4F : OpCode.LDLOC0
    /// 50 : OpCode.LDLOC4
    /// 51 : OpCode.APPEND
    /// 52 : OpCode.LDLOC3
    /// 53 : OpCode.INC
    /// 54 : OpCode.STLOC3
    /// 55 : OpCode.LDLOC3
    /// 56 : OpCode.LDLOC2
    /// 57 : OpCode.JMPLT BA
    /// 59 : OpCode.LDLOC0
    /// 5A : OpCode.RET
    /// </remarks>
    [DisplayName("skip")]
    public abstract object? Skip(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDReDHNlbGVjdG9yeTVZ+P//EHB4SnHKchBzIj1pa850aGx5Np5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGucc2tqMMNoQA==
    /// 00 : OpCode.INITSLOT 0502
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 5E
    /// 06 : OpCode.PUSHDATA1 73656C6563746F72
    /// 10 : OpCode.LDARG1
    /// 11 : OpCode.CALL_L 59F8FFFF
    /// 16 : OpCode.PUSH0
    /// 17 : OpCode.STLOC0
    /// 18 : OpCode.LDARG0
    /// 19 : OpCode.DUP
    /// 1A : OpCode.STLOC1
    /// 1B : OpCode.SIZE
    /// 1C : OpCode.STLOC2
    /// 1D : OpCode.PUSH0
    /// 1E : OpCode.STLOC3
    /// 1F : OpCode.JMP 3D
    /// 21 : OpCode.LDLOC1
    /// 22 : OpCode.LDLOC3
    /// 23 : OpCode.PICKITEM
    /// 24 : OpCode.STLOC4
    /// 25 : OpCode.LDLOC0
    /// 26 : OpCode.LDLOC4
    /// 27 : OpCode.LDARG1
    /// 28 : OpCode.CALLA
    /// 29 : OpCode.ADD
    /// 2A : OpCode.DUP
    /// 2B : OpCode.PUSHINT32 00000080
    /// 30 : OpCode.JMPGE 04
    /// 32 : OpCode.JMP 0A
    /// 34 : OpCode.DUP
    /// 35 : OpCode.PUSHINT32 FFFFFF7F
    /// 3A : OpCode.JMPLE 1E
    /// 3C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 45 : OpCode.AND
    /// 46 : OpCode.DUP
    /// 47 : OpCode.PUSHINT32 FFFFFF7F
    /// 4C : OpCode.JMPLE 0C
    /// 4E : OpCode.PUSHINT64 0000000001000000
    /// 57 : OpCode.SUB
    /// 58 : OpCode.STLOC0
    /// 59 : OpCode.LDLOC3
    /// 5A : OpCode.INC
    /// 5B : OpCode.STLOC3
    /// 5C : OpCode.LDLOC3
    /// 5D : OpCode.LDLOC2
    /// 5E : OpCode.JMPLT C3
    /// 60 : OpCode.LDLOC0
    /// 61 : OpCode.RET
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCoUAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 85000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("sumTwice")]
    public abstract BigInteger? SumTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDRXwnB4SnHKchBzIkZpa850eRC2JgQiP2hsz3lKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+BRWucc2tqMLpoQA==
    /// 00 : OpCode.INITSLOT 0502
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 57
    /// 06 : OpCode.NEWARRAY0
    /// 07 : OpCode.STLOC0
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.DUP
    /// 0A : OpCode.STLOC1
    /// 0B : OpCode.SIZE
    /// 0C : OpCode.STLOC2
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.STLOC3
    /// 0F : OpCode.JMP 46
    /// 11 : OpCode.LDLOC1
    /// 12 : OpCode.LDLOC3
    /// 13 : OpCode.PICKITEM
    /// 14 : OpCode.STLOC4
    /// 15 : OpCode.LDARG1
    /// 16 : OpCode.PUSH0
    /// 17 : OpCode.LE
    /// 18 : OpCode.JMPIFNOT 04
    /// 1A : OpCode.JMP 3F
    /// 1C : OpCode.LDLOC0
    /// 1D : OpCode.LDLOC4
    /// 1E : OpCode.APPEND
    /// 1F : OpCode.LDARG1
    /// 20 : OpCode.DUP
    /// 21 : OpCode.DEC
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT32 00000080
    /// 28 : OpCode.JMPGE 04
    /// 2A : OpCode.JMP 0A
    /// 2C : OpCode.DUP
    /// 2D : OpCode.PUSHINT32 FFFFFF7F
    /// 32 : OpCode.JMPLE 1E
    /// 34 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 3D : OpCode.AND
    /// 3E : OpCode.DUP
    /// 3F : OpCode.PUSHINT32 FFFFFF7F
    /// 44 : OpCode.JMPLE 0C
    /// 46 : OpCode.PUSHINT64 0000000001000000
    /// 4F : OpCode.SUB
    /// 50 : OpCode.STARG1
    /// 51 : OpCode.DROP
    /// 52 : OpCode.LDLOC3
    /// 53 : OpCode.INC
    /// 54 : OpCode.STLOC3
    /// 55 : OpCode.LDLOC3
    /// 56 : OpCode.LDLOC2
    /// 57 : OpCode.JMPLT BA
    /// 59 : OpCode.LDLOC0
    /// 5A : OpCode.RET
    /// </remarks>
    [DisplayName("take")]
    public abstract object? Take(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUDeDRPDGtleVNlbGVjdG9yeTXz9v//DGVsZW1lbnRTZWxlY3Rvcno13Pb//8hweEpxynIQcyIUaWvOdGx6NkpseTZoU9BFa5xza2ow7GhA
    /// 00 : OpCode.INITSLOT 0503
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 4F
    /// 06 : OpCode.PUSHDATA1 6B657953656C6563746F72
    /// 13 : OpCode.LDARG1
    /// 14 : OpCode.CALL_L F3F6FFFF
    /// 19 : OpCode.PUSHDATA1 656C656D656E7453656C6563746F72
    /// 2A : OpCode.LDARG2
    /// 2B : OpCode.CALL_L DCF6FFFF
    /// 30 : OpCode.NEWMAP
    /// 31 : OpCode.STLOC0
    /// 32 : OpCode.LDARG0
    /// 33 : OpCode.DUP
    /// 34 : OpCode.STLOC1
    /// 35 : OpCode.SIZE
    /// 36 : OpCode.STLOC2
    /// 37 : OpCode.PUSH0
    /// 38 : OpCode.STLOC3
    /// 39 : OpCode.JMP 14
    /// 3B : OpCode.LDLOC1
    /// 3C : OpCode.LDLOC3
    /// 3D : OpCode.PICKITEM
    /// 3E : OpCode.STLOC4
    /// 3F : OpCode.LDLOC4
    /// 40 : OpCode.LDARG2
    /// 41 : OpCode.CALLA
    /// 42 : OpCode.DUP
    /// 43 : OpCode.LDLOC4
    /// 44 : OpCode.LDARG1
    /// 45 : OpCode.CALLA
    /// 46 : OpCode.LDLOC0
    /// 47 : OpCode.REVERSE3
    /// 48 : OpCode.SETITEM
    /// 49 : OpCode.DROP
    /// 4A : OpCode.LDLOC3
    /// 4B : OpCode.INC
    /// 4C : OpCode.STLOC3
    /// 4D : OpCode.LDLOC3
    /// 4E : OpCode.LDLOC2
    /// 4F : OpCode.JMPLT EC
    /// 51 : OpCode.LDLOC0
    /// 52 : OpCode.RET
    /// </remarks>
    [DisplayName("toMap")]
    public abstract object? ToMap(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCloAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 5A000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("whereGreaterThanZero")]
    public abstract object? WhereGreaterThanZero(IList<object>? array);

    #endregion
}
