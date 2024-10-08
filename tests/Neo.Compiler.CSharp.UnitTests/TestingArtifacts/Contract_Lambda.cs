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
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.STSFLD 0A
    /// 06 : OpCode.PUSHA 09000000
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.CALL CD
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("anyGreatThan")]
    public abstract bool? AnyGreatThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCikAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 29000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("anyGreatThanZero")]
    public abstract bool? AnyGreatThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeGYKFAAAAHBeDCAhISGL2yhmaDZA
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STSFLD6
    /// 05 : OpCode.PUSHA 14000000
    /// 0A : OpCode.STLOC0
    /// 0B : OpCode.LDSFLD6
    /// 0C : OpCode.PUSHDATA1 20212121
    /// 12 : OpCode.CAT
    /// 13 : OpCode.CONVERT 28
    /// 15 : OpCode.STSFLD6
    /// 16 : OpCode.LDLOC0
    /// 17 : OpCode.CALLA
    /// 18 : OpCode.RET
    /// </remarks>
    [DisplayName("changeName")]
    public abstract string? ChangeName(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBeGcHChIAAABwChwAAABxaDZFaTZA
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STSFLD 07
    /// 06 : OpCode.PUSHA 12000000
    /// 0B : OpCode.STLOC0
    /// 0C : OpCode.PUSHA 1C000000
    /// 11 : OpCode.STLOC1
    /// 12 : OpCode.LDLOC0
    /// 13 : OpCode.CALLA
    /// 14 : OpCode.DROP
    /// 15 : OpCode.LDLOC1
    /// 16 : OpCode.CALLA
    /// 17 : OpCode.RET
    /// </remarks>
    [DisplayName("changeName2")]
    public abstract string? ChangeName2(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeFo2QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDSFLD2
    /// 05 : OpCode.CALLA
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("checkPositiveOdd")]
    public abstract bool? CheckPositiveOdd(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeFg2QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDSFLD0
    /// 05 : OpCode.CALLA
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("checkZero")]
    public abstract bool? CheckZero(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeFg0A0A=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDSFLD0
    /// 05 : OpCode.CALL 03
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("checkZero2")]
    public abstract bool? CheckZero2(BigInteger? num);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEHB4aGEKCAAAADTsQA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.LDLOC0
    /// 07 : OpCode.STSFLD1
    /// 08 : OpCode.PUSHA 08000000
    /// 0D : OpCode.CALL EC
    /// 0F : OpCode.RET
    /// </remarks>
    [DisplayName("checkZero3")]
    public abstract bool? CheckZero3(BigInteger? num);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBC3BoZQoKAAAAZXhdNkA=
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSHNULL
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.STSFLD5
    /// 07 : OpCode.PUSHA 0A000000
    /// 0C : OpCode.STSFLD5
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.LDSFLD5
    /// 0F : OpCode.CALLA
    /// 10 : OpCode.RET
    /// </remarks>
    [DisplayName("fibo")]
    public abstract BigInteger? Fibo(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBwnB4SnHKchBzIhNpa850aGxnCAopAAAAz2ucc2tqMO3CcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lA
    /// 00 : OpCode.INITSLOT 0601
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.DUP
    /// 07 : OpCode.STLOC1
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.STLOC2
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC3
    /// 0C : OpCode.JMP 13
    /// 0E : OpCode.LDLOC1
    /// 0F : OpCode.LDLOC3
    /// 10 : OpCode.PICKITEM
    /// 11 : OpCode.STLOC4
    /// 12 : OpCode.LDLOC0
    /// 13 : OpCode.LDLOC4
    /// 14 : OpCode.STSFLD 08
    /// 16 : OpCode.PUSHA 29000000
    /// 1B : OpCode.APPEND
    /// 1C : OpCode.LDLOC3
    /// 1D : OpCode.INC
    /// 1E : OpCode.STLOC3
    /// 1F : OpCode.LDLOC3
    /// 20 : OpCode.LDLOC2
    /// 21 : OpCode.JMPLT ED
    /// 23 : OpCode.NEWARRAY0
    /// 24 : OpCode.STLOC1
    /// 25 : OpCode.LDLOC0
    /// 26 : OpCode.DUP
    /// 27 : OpCode.STLOC2
    /// 28 : OpCode.SIZE
    /// 29 : OpCode.STLOC3
    /// 2A : OpCode.PUSH0
    /// 2B : OpCode.STLOC4
    /// 2C : OpCode.JMP 0D
    /// 2E : OpCode.LDLOC2
    /// 2F : OpCode.LDLOC4
    /// 30 : OpCode.PICKITEM
    /// 31 : OpCode.STLOC5
    /// 32 : OpCode.LDLOC1
    /// 33 : OpCode.LDLOC5
    /// 34 : OpCode.CALLA
    /// 35 : OpCode.APPEND
    /// 36 : OpCode.LDLOC4
    /// 37 : OpCode.INC
    /// 38 : OpCode.STLOC4
    /// 39 : OpCode.LDLOC4
    /// 3A : OpCode.LDLOC3
    /// 3B : OpCode.JMPLT F3
    /// 3D : OpCode.LDLOC1
    /// 3E : OpCode.RET
    /// </remarks>
    [DisplayName("forEachVar")]
    public abstract IList<object>? ForEachVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBwnAQcSJDeGnOcmhqZwkKWwAAAM9pSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeMq1JLvCcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lA
    /// 00 : OpCode.INITSLOT 0601
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.JMP 43
    /// 09 : OpCode.LDARG0
    /// 0A : OpCode.LDLOC1
    /// 0B : OpCode.PICKITEM
    /// 0C : OpCode.STLOC2
    /// 0D : OpCode.LDLOC0
    /// 0E : OpCode.LDLOC2
    /// 0F : OpCode.STSFLD 09
    /// 11 : OpCode.PUSHA 5B000000
    /// 16 : OpCode.APPEND
    /// 17 : OpCode.LDLOC1
    /// 18 : OpCode.DUP
    /// 19 : OpCode.INC
    /// 1A : OpCode.DUP
    /// 1B : OpCode.PUSHINT32 00000080
    /// 20 : OpCode.JMPGE 04
    /// 22 : OpCode.JMP 0A
    /// 24 : OpCode.DUP
    /// 25 : OpCode.PUSHINT32 FFFFFF7F
    /// 2A : OpCode.JMPLE 1E
    /// 2C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 35 : OpCode.AND
    /// 36 : OpCode.DUP
    /// 37 : OpCode.PUSHINT32 FFFFFF7F
    /// 3C : OpCode.JMPLE 0C
    /// 3E : OpCode.PUSHINT64 0000000001000000
    /// 47 : OpCode.SUB
    /// 48 : OpCode.STLOC1
    /// 49 : OpCode.DROP
    /// 4A : OpCode.LDLOC1
    /// 4B : OpCode.LDARG0
    /// 4C : OpCode.SIZE
    /// 4D : OpCode.LT
    /// 4E : OpCode.JMPIF BB
    /// 50 : OpCode.NEWARRAY0
    /// 51 : OpCode.STLOC1
    /// 52 : OpCode.LDLOC0
    /// 53 : OpCode.DUP
    /// 54 : OpCode.STLOC2
    /// 55 : OpCode.SIZE
    /// 56 : OpCode.STLOC3
    /// 57 : OpCode.PUSH0
    /// 58 : OpCode.STLOC4
    /// 59 : OpCode.JMP 0D
    /// 5B : OpCode.LDLOC2
    /// 5C : OpCode.LDLOC4
    /// 5D : OpCode.PICKITEM
    /// 5E : OpCode.STLOC5
    /// 5F : OpCode.LDLOC1
    /// 60 : OpCode.LDLOC5
    /// 61 : OpCode.CALLA
    /// 62 : OpCode.APPEND
    /// 63 : OpCode.LDLOC4
    /// 64 : OpCode.INC
    /// 65 : OpCode.STLOC4
    /// 66 : OpCode.LDLOC4
    /// 67 : OpCode.LDLOC3
    /// 68 : OpCode.JMPLT F3
    /// 6A : OpCode.LDLOC1
    /// 6B : OpCode.RET
    /// </remarks>
    [DisplayName("forVar")]
    public abstract IList<object>? ForVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhbNkA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.LDSFLD3
    /// 06 : OpCode.CALLA
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("invokeSum")]
    public abstract BigInteger? InvokeSum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEXB5eGhkChAAAAA0A0A=
    /// 00 : OpCode.INITSLOT 0102
    /// 03 : OpCode.PUSH1
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG1
    /// 06 : OpCode.LDARG0
    /// 07 : OpCode.LDLOC0
    /// 08 : OpCode.STSFLD4
    /// 09 : OpCode.PUSHA 10000000
    /// 0E : OpCode.CALL 03
    /// 10 : OpCode.RET
    /// </remarks>
    [DisplayName("invokeSum2")]
    public abstract BigInteger? InvokeSum2(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBCgsAAABwEXhoNkA=
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSHA 0B000000
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.PUSH1
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.LDLOC0
    /// 0C : OpCode.CALLA
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("testLambdaDefault")]
    public abstract BigInteger? TestLambdaDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECCgsAAABweXhoNkA=
    /// 00 : OpCode.INITSLOT 0102
    /// 03 : OpCode.PUSHA 0B000000
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDARG1
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.LDLOC0
    /// 0C : OpCode.CALLA
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("testLambdaNotDefault")]
    public abstract BigInteger? TestLambdaNotDefault(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCiwAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 2C000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("whereGreaterThanZero")]
    public abstract IList<object>? WhereGreaterThanZero(IList<object>? array);

    #endregion
}
