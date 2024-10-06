using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Lambda(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Lambda"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""checkZero"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""checkZero2"",""parameters"":[{""name"":""num"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":7,""safe"":false},{""name"":""checkZero3"",""parameters"":[{""name"":""num"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":22,""safe"":false},{""name"":""checkPositiveOdd"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":45,""safe"":false},{""name"":""invokeSum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":52,""safe"":false},{""name"":""invokeSum2"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":60,""safe"":false},{""name"":""fibo"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":186,""safe"":false},{""name"":""changeName"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""String"",""offset"":363,""safe"":false},{""name"":""changeName2"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""String"",""offset"":390,""safe"":false},{""name"":""forEachVar"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":433,""safe"":false},{""name"":""forVar"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":499,""safe"":false},{""name"":""anyGreatThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":610,""safe"":false},{""name"":""anyGreatThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":661,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":684,""safe"":false},{""name"":""testLambdaDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":738,""safe"":false},{""name"":""testLambdaNotDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":805,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":872,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3NA1cAAXhYNkBXAAF4WDQDQFcAAnl4NkBXAQEQcHhoYQoIAAAANOxAVwABeFmXQFcAAXhaNkBXAAJ5eFs2QFcBAhFweXhoZAoQAAAANANAVwADenl4NkBXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfXJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcBAQtwaGUKCgAAAGV4XTZAVwABeBK1JgR4QHgRn0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9dNngSn0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9dNp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcBAXhmChQAAABwXgwEICEhIYvbKGZoNkBeQFcCAXhnBwoSAAAAcAocAAAAcWg2RWk2QF8HDAQgISEhi9soZwdfB0BfB0BXBgHCcHhKccpyEHMiE2lrznRobGcICikAAADPa5xza2ow7cJxaEpyynMQdCINamzOdWltNs9snHRsazDzaUBfCEBXBgHCcBBxIkN4ac5yaGpnCQpbAAAAz2lKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4yrUku8JxaEpyynMQdCINamzOdWltNs9snHRsazDzaUBfCUBXAAEKKQAAAHg0A0BXBAJ4SnDKcRByIhBoas5za3k2JgQIQGqccmppMPAJQFcAAXgQt0BXAAJ5ZwoKCQAAAHg0zUBXAAF4Xwq3QFcAAQosAAAAeDQDQFcFAsJweEpxynIQcyIRaWvOdGx5NiYFaGzPa5xza2ow72hAVwABeBC3QFcBAQoLAAAAcBF4aDZAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAQIKCwAAAHB5eGg2QFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVgwQZwsKEwAAAGAKFQAAAGIKHwAAAGNAVwABeF8Ll0BXAAF4ELckBAlAeBKiEZdAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0AQ0X/g"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWcKCgkAAAB4NM1A
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.STSFLD 0A
    /// 0006 : OpCode.PUSHA 09000000
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.CALL CD
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("anyGreatThan")]
    public abstract bool? AnyGreatThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCikAAAB4NANA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHA 29000000
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.CALL 03
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("anyGreatThanZero")]
    public abstract bool? AnyGreatThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeGYKFAAAAHBeDCAhISGL2yhmaDZA
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STSFLD6
    /// 0005 : OpCode.PUSHA 14000000
    /// 000A : OpCode.STLOC0
    /// 000B : OpCode.LDSFLD6
    /// 000C : OpCode.PUSHDATA1 20212121
    /// 0012 : OpCode.CAT
    /// 0013 : OpCode.CONVERT 28
    /// 0015 : OpCode.STSFLD6
    /// 0016 : OpCode.LDLOC0
    /// 0017 : OpCode.CALLA
    /// 0018 : OpCode.RET
    /// </remarks>
    [DisplayName("changeName")]
    public abstract string? ChangeName(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBeGcHChIAAABwChwAAABxaDZFaTZA
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STSFLD 07
    /// 0006 : OpCode.PUSHA 12000000
    /// 000B : OpCode.STLOC0
    /// 000C : OpCode.PUSHA 1C000000
    /// 0011 : OpCode.STLOC1
    /// 0012 : OpCode.LDLOC0
    /// 0013 : OpCode.CALLA
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.LDLOC1
    /// 0016 : OpCode.CALLA
    /// 0017 : OpCode.RET
    /// </remarks>
    [DisplayName("changeName2")]
    public abstract string? ChangeName2(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeFo2QA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDSFLD2
    /// 0005 : OpCode.CALLA
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("checkPositiveOdd")]
    public abstract bool? CheckPositiveOdd(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeFg2QA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDSFLD0
    /// 0005 : OpCode.CALLA
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("checkZero")]
    public abstract bool? CheckZero(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeFg0A0A=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDSFLD0
    /// 0005 : OpCode.CALL 03
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("checkZero2")]
    public abstract bool? CheckZero2(BigInteger? num);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEHB4aGEKCAAAADTsQA==
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.LDLOC0
    /// 0007 : OpCode.STSFLD1
    /// 0008 : OpCode.PUSHA 08000000
    /// 000D : OpCode.CALL EC
    /// 000F : OpCode.RET
    /// </remarks>
    [DisplayName("checkZero3")]
    public abstract bool? CheckZero3(BigInteger? num);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBC3BoZQoKAAAAZXhdNkA=
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSHNULL
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.STSFLD5
    /// 0007 : OpCode.PUSHA 0A000000
    /// 000C : OpCode.STSFLD5
    /// 000D : OpCode.LDARG0
    /// 000E : OpCode.LDSFLD5
    /// 000F : OpCode.CALLA
    /// 0010 : OpCode.RET
    /// </remarks>
    [DisplayName("fibo")]
    public abstract BigInteger? Fibo(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBwnB4SnHKchBzIhNpa850aGxnCAopAAAAz2ucc2tqMO3CcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lA
    /// 0000 : OpCode.INITSLOT 0601
    /// 0003 : OpCode.NEWARRAY0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.STLOC1
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.STLOC2
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.STLOC3
    /// 000C : OpCode.JMP 13
    /// 000E : OpCode.LDLOC1
    /// 000F : OpCode.LDLOC3
    /// 0010 : OpCode.PICKITEM
    /// 0011 : OpCode.STLOC4
    /// 0012 : OpCode.LDLOC0
    /// 0013 : OpCode.LDLOC4
    /// 0014 : OpCode.STSFLD 08
    /// 0016 : OpCode.PUSHA 29000000
    /// 001B : OpCode.APPEND
    /// 001C : OpCode.LDLOC3
    /// 001D : OpCode.INC
    /// 001E : OpCode.STLOC3
    /// 001F : OpCode.LDLOC3
    /// 0020 : OpCode.LDLOC2
    /// 0021 : OpCode.JMPLT ED
    /// 0023 : OpCode.NEWARRAY0
    /// 0024 : OpCode.STLOC1
    /// 0025 : OpCode.LDLOC0
    /// 0026 : OpCode.DUP
    /// 0027 : OpCode.STLOC2
    /// 0028 : OpCode.SIZE
    /// 0029 : OpCode.STLOC3
    /// 002A : OpCode.PUSH0
    /// 002B : OpCode.STLOC4
    /// 002C : OpCode.JMP 0D
    /// 002E : OpCode.LDLOC2
    /// 002F : OpCode.LDLOC4
    /// 0030 : OpCode.PICKITEM
    /// 0031 : OpCode.STLOC5
    /// 0032 : OpCode.LDLOC1
    /// 0033 : OpCode.LDLOC5
    /// 0034 : OpCode.CALLA
    /// 0035 : OpCode.APPEND
    /// 0036 : OpCode.LDLOC4
    /// 0037 : OpCode.INC
    /// 0038 : OpCode.STLOC4
    /// 0039 : OpCode.LDLOC4
    /// 003A : OpCode.LDLOC3
    /// 003B : OpCode.JMPLT F3
    /// 003D : OpCode.LDLOC1
    /// 003E : OpCode.RET
    /// </remarks>
    [DisplayName("forEachVar")]
    public abstract IList<object>? ForEachVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBwnAQcSJDeGnOcmhqZwkKWwAAAM9pSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeMq1JLvCcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lA
    /// 0000 : OpCode.INITSLOT 0601
    /// 0003 : OpCode.NEWARRAY0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.STLOC1
    /// 0007 : OpCode.JMP 43
    /// 0009 : OpCode.LDARG0
    /// 000A : OpCode.LDLOC1
    /// 000B : OpCode.PICKITEM
    /// 000C : OpCode.STLOC2
    /// 000D : OpCode.LDLOC0
    /// 000E : OpCode.LDLOC2
    /// 000F : OpCode.STSFLD 09
    /// 0011 : OpCode.PUSHA 5B000000
    /// 0016 : OpCode.APPEND
    /// 0017 : OpCode.LDLOC1
    /// 0018 : OpCode.DUP
    /// 0019 : OpCode.INC
    /// 001A : OpCode.DUP
    /// 001B : OpCode.PUSHINT32 00000080
    /// 0020 : OpCode.JMPGE 04
    /// 0022 : OpCode.JMP 0A
    /// 0024 : OpCode.DUP
    /// 0025 : OpCode.PUSHINT32 FFFFFF7F
    /// 002A : OpCode.JMPLE 1E
    /// 002C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0035 : OpCode.AND
    /// 0036 : OpCode.DUP
    /// 0037 : OpCode.PUSHINT32 FFFFFF7F
    /// 003C : OpCode.JMPLE 0C
    /// 003E : OpCode.PUSHINT64 0000000001000000
    /// 0047 : OpCode.SUB
    /// 0048 : OpCode.STLOC1
    /// 0049 : OpCode.DROP
    /// 004A : OpCode.LDLOC1
    /// 004B : OpCode.LDARG0
    /// 004C : OpCode.SIZE
    /// 004D : OpCode.LT
    /// 004E : OpCode.JMPIF BB
    /// 0050 : OpCode.NEWARRAY0
    /// 0051 : OpCode.STLOC1
    /// 0052 : OpCode.LDLOC0
    /// 0053 : OpCode.DUP
    /// 0054 : OpCode.STLOC2
    /// 0055 : OpCode.SIZE
    /// 0056 : OpCode.STLOC3
    /// 0057 : OpCode.PUSH0
    /// 0058 : OpCode.STLOC4
    /// 0059 : OpCode.JMP 0D
    /// 005B : OpCode.LDLOC2
    /// 005C : OpCode.LDLOC4
    /// 005D : OpCode.PICKITEM
    /// 005E : OpCode.STLOC5
    /// 005F : OpCode.LDLOC1
    /// 0060 : OpCode.LDLOC5
    /// 0061 : OpCode.CALLA
    /// 0062 : OpCode.APPEND
    /// 0063 : OpCode.LDLOC4
    /// 0064 : OpCode.INC
    /// 0065 : OpCode.STLOC4
    /// 0066 : OpCode.LDLOC4
    /// 0067 : OpCode.LDLOC3
    /// 0068 : OpCode.JMPLT F3
    /// 006A : OpCode.LDLOC1
    /// 006B : OpCode.RET
    /// </remarks>
    [DisplayName("forVar")]
    public abstract IList<object>? ForVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhbNkA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.LDSFLD3
    /// 0006 : OpCode.CALLA
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("invokeSum")]
    public abstract BigInteger? InvokeSum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEXB5eGhkChAAAAA0A0A=
    /// 0000 : OpCode.INITSLOT 0102
    /// 0003 : OpCode.PUSH1
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG1
    /// 0006 : OpCode.LDARG0
    /// 0007 : OpCode.LDLOC0
    /// 0008 : OpCode.STSFLD4
    /// 0009 : OpCode.PUSHA 10000000
    /// 000E : OpCode.CALL 03
    /// 0010 : OpCode.RET
    /// </remarks>
    [DisplayName("invokeSum2")]
    public abstract BigInteger? InvokeSum2(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBCgsAAABwEXhoNkA=
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSHA 0B000000
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.PUSH1
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.LDLOC0
    /// 000C : OpCode.CALLA
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("testLambdaDefault")]
    public abstract BigInteger? TestLambdaDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECCgsAAABweXhoNkA=
    /// 0000 : OpCode.INITSLOT 0102
    /// 0003 : OpCode.PUSHA 0B000000
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDARG1
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.LDLOC0
    /// 000C : OpCode.CALLA
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("testLambdaNotDefault")]
    public abstract BigInteger? TestLambdaNotDefault(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCiwAAAB4NANA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHA 2C000000
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.CALL 03
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("whereGreaterThanZero")]
    public abstract IList<object>? WhereGreaterThanZero(IList<object>? array);

    #endregion

}
