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
    [DisplayName("anyGreatThan")]
    public abstract bool? AnyGreatThan(IList<object>? array, BigInteger? target);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : STSFLD
    // 0006 : PUSHA
    // 000B : LDARG0
    // 000C : CALL
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("anyGreatThanZero")]
    public abstract bool? AnyGreatThanZero(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : PUSHA
    // 0008 : LDARG0
    // 0009 : CALL
    // 000B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("changeName")]
    public abstract string? ChangeName(string? name);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STSFLD6
    // 0005 : PUSHA
    // 000A : STLOC0
    // 000B : LDSFLD6
    // 000C : PUSHDATA1
    // 0012 : CAT
    // 0013 : CONVERT
    // 0015 : STSFLD6
    // 0016 : LDLOC0
    // 0017 : CALLA
    // 0018 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("changeName2")]
    public abstract string? ChangeName2(string? name);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STSFLD
    // 0006 : PUSHA
    // 000B : STLOC0
    // 000C : PUSHA
    // 0011 : STLOC1
    // 0012 : LDLOC0
    // 0013 : CALLA
    // 0014 : DROP
    // 0015 : LDLOC1
    // 0016 : CALLA
    // 0017 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkPositiveOdd")]
    public abstract bool? CheckPositiveOdd(BigInteger? i);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDSFLD2
    // 0005 : CALLA
    // 0006 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkZero")]
    public abstract bool? CheckZero(BigInteger? i);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDSFLD0
    // 0005 : CALLA
    // 0006 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkZero2")]
    public abstract bool? CheckZero2(BigInteger? num);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDSFLD0
    // 0005 : CALL
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkZero3")]
    public abstract bool? CheckZero3(BigInteger? num);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : LDARG0
    // 0006 : LDLOC0
    // 0007 : STSFLD1
    // 0008 : PUSHA
    // 000D : CALL
    // 000F : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("fibo")]
    public abstract BigInteger? Fibo(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : PUSHNULL
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : STSFLD5
    // 0007 : PUSHA
    // 000C : STSFLD5
    // 000D : LDARG0
    // 000E : LDSFLD5
    // 000F : CALLA
    // 0010 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("forEachVar")]
    public abstract IList<object>? ForEachVar(IList<object>? array);
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
    // 0013 : LDLOC4
    // 0014 : STSFLD
    // 0016 : PUSHA
    // 001B : APPEND
    // 001C : LDLOC3
    // 001D : INC
    // 001E : STLOC3
    // 001F : LDLOC3
    // 0020 : LDLOC2
    // 0021 : JMPLT
    // 0023 : NEWARRAY0
    // 0024 : STLOC1
    // 0025 : LDLOC0
    // 0026 : DUP
    // 0027 : STLOC2
    // 0028 : SIZE
    // 0029 : STLOC3
    // 002A : PUSH0
    // 002B : STLOC4
    // 002C : JMP
    // 002E : LDLOC2
    // 002F : LDLOC4
    // 0030 : PICKITEM
    // 0031 : STLOC5
    // 0032 : LDLOC1
    // 0033 : LDLOC5
    // 0034 : CALLA
    // 0035 : APPEND
    // 0036 : LDLOC4
    // 0037 : INC
    // 0038 : STLOC4
    // 0039 : LDLOC4
    // 003A : LDLOC3
    // 003B : JMPLT
    // 003D : LDLOC1
    // 003E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("forVar")]
    public abstract IList<object>? ForVar(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : NEWARRAY0
    // 0004 : STLOC0
    // 0005 : PUSH0
    // 0006 : STLOC1
    // 0007 : JMP
    // 0009 : LDARG0
    // 000A : LDLOC1
    // 000B : PICKITEM
    // 000C : STLOC2
    // 000D : LDLOC0
    // 000E : LDLOC2
    // 000F : STSFLD
    // 0011 : PUSHA
    // 0016 : APPEND
    // 0017 : LDLOC1
    // 0018 : DUP
    // 0019 : INC
    // 001A : DUP
    // 001B : PUSHINT32
    // 0020 : JMPGE
    // 0022 : JMP
    // 0024 : DUP
    // 0025 : PUSHINT32
    // 002A : JMPLE
    // 002C : PUSHINT64
    // 0035 : AND
    // 0036 : DUP
    // 0037 : PUSHINT32
    // 003C : JMPLE
    // 003E : PUSHINT64
    // 0047 : SUB
    // 0048 : STLOC1
    // 0049 : DROP
    // 004A : LDLOC1
    // 004B : LDARG0
    // 004C : SIZE
    // 004D : LT
    // 004E : JMPIF
    // 0050 : NEWARRAY0
    // 0051 : STLOC1
    // 0052 : LDLOC0
    // 0053 : DUP
    // 0054 : STLOC2
    // 0055 : SIZE
    // 0056 : STLOC3
    // 0057 : PUSH0
    // 0058 : STLOC4
    // 0059 : JMP
    // 005B : LDLOC2
    // 005C : LDLOC4
    // 005D : PICKITEM
    // 005E : STLOC5
    // 005F : LDLOC1
    // 0060 : LDLOC5
    // 0061 : CALLA
    // 0062 : APPEND
    // 0063 : LDLOC4
    // 0064 : INC
    // 0065 : STLOC4
    // 0066 : LDLOC4
    // 0067 : LDLOC3
    // 0068 : JMPLT
    // 006A : LDLOC1
    // 006B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("invokeSum")]
    public abstract BigInteger? InvokeSum(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : LDARG0
    // 0005 : LDSFLD3
    // 0006 : CALLA
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("invokeSum2")]
    public abstract BigInteger? InvokeSum2(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : PUSH1
    // 0004 : STLOC0
    // 0005 : LDARG1
    // 0006 : LDARG0
    // 0007 : LDLOC0
    // 0008 : STSFLD4
    // 0009 : PUSHA
    // 000E : CALL
    // 0010 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testLambdaDefault")]
    public abstract BigInteger? TestLambdaDefault(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : PUSHA
    // 0008 : STLOC0
    // 0009 : PUSH1
    // 000A : LDARG0
    // 000B : LDLOC0
    // 000C : CALLA
    // 000D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testLambdaNotDefault")]
    public abstract BigInteger? TestLambdaNotDefault(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : PUSHA
    // 0008 : STLOC0
    // 0009 : LDARG1
    // 000A : LDARG0
    // 000B : LDLOC0
    // 000C : CALLA
    // 000D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("whereGreaterThanZero")]
    public abstract IList<object>? WhereGreaterThanZero(IList<object>? array);
    // 0000 : INITSLOT
    // 0003 : PUSHA
    // 0008 : LDARG0
    // 0009 : CALL
    // 000B : RET

    #endregion

}
