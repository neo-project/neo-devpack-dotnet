using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Lambda(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Lambda"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""checkZero"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""checkZero2"",""parameters"":[{""name"":""num"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":11,""safe"":false},{""name"":""checkZero3"",""parameters"":[{""name"":""num"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":30,""safe"":false},{""name"":""checkPositiveOdd"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":53,""safe"":false},{""name"":""invokeSum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":64,""safe"":false},{""name"":""invokeSum2"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":76,""safe"":false},{""name"":""fibo"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":202,""safe"":false},{""name"":""changeName"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""String"",""offset"":379,""safe"":false},{""name"":""changeName2"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""String"",""offset"":406,""safe"":false},{""name"":""forEachVar"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":449,""safe"":false},{""name"":""forVar"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":515,""safe"":false},{""name"":""anyGreatThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":626,""safe"":false},{""name"":""anyGreatThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":677,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":700,""safe"":false},{""name"":""testLambdaDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":754,""safe"":false},{""name"":""testLambdaNotDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":821,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":888,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3HA1cAAXgKdwMAADZAVwABeApsAwAANANAVwACeXg2QFcBARBweGhhCggAAAA07EBXAAF4WZdAVwABeApJAwAANkBXAAJ5eApNAwAANkBXAQIRcHl4aGQKEAAAADQDQFcAA3p5eDZAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1yeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAQELcGhlCgoAAABleF02QFcAAXgStSYEeEB4EZ9KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfXTZ4Ep9KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfXTaeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAQF4ZgoUAAAAcF4MBCAhISGL2yhmaDZAXkBXAgF4ZwcKEgAAAHAKHAAAAHFoNkVpNkBfBwwEICEhIYvbKGcHXwdAXwdAVwYBwnB4SnHKchBzIhNpa850aGxnCAopAAAAz2ucc2tqMO3CcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lAXwhAVwYBwnAQcSJDeGnOcmhqZwkKWwAAAM9pSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeMq1JLvCcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lAXwlAVwABCikAAAB4NANAVwQCeEpwynEQciIQaGrOc2t5NiYECEBqnHJqaTDwCUBXAAF4ELdAVwACeWcKCgkAAAB4NM1AVwABeF8Kt0BXAAEKLAAAAHg0A0BXBQLCcHhKccpyEHMiEWlrznRseTYmBWhsz2ucc2tqMO9oQFcAAXgQt0BXAQEKCwAAAHAReGg2QFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwECCgsAAABweXhoNkBXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFYMQFcAAXgQl0BXAAF4ELckBAlAeBKiEZdAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0CJI/8f").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWcKCgkAAAB4NM1A
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : STSFLD 0A [2 datoshi]
    /// 06 : PUSHA 09000000 [4 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : CALL CD [512 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreatThan")]
    public abstract bool? AnyGreatThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCikAAAB4NANA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHA 29000000 [4 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : CALL 03 [512 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreatThanZero")]
    public abstract bool? AnyGreatThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeGYKFAAAAHBeDAQgISEhi9soZmg2QA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STSFLD6 [2 datoshi]
    /// 05 : PUSHA 14000000 [4 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : LDSFLD6 [2 datoshi]
    /// 0C : PUSHDATA1 20212121 [8 datoshi]
    /// 12 : CAT [2048 datoshi]
    /// 13 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 15 : STSFLD6 [2 datoshi]
    /// 16 : LDLOC0 [2 datoshi]
    /// 17 : CALLA [512 datoshi]
    /// 18 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("changeName")]
    public abstract string? ChangeName(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBeGcHChIAAABwChwAAABxaDZFaTZA
    /// 00 : INITSLOT 0201 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STSFLD 07 [2 datoshi]
    /// 06 : PUSHA 12000000 [4 datoshi]
    /// 0B : STLOC0 [2 datoshi]
    /// 0C : PUSHA 1C000000 [4 datoshi]
    /// 11 : STLOC1 [2 datoshi]
    /// 12 : LDLOC0 [2 datoshi]
    /// 13 : CALLA [512 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : LDLOC1 [2 datoshi]
    /// 16 : CALLA [512 datoshi]
    /// 17 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("changeName2")]
    public abstract string? ChangeName2(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeApJAwAANkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHA 49030000 [4 datoshi]
    /// 09 : CALLA [512 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkPositiveOdd")]
    public abstract bool? CheckPositiveOdd(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAp3AwAANkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHA 77030000 [4 datoshi]
    /// 09 : CALLA [512 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZero")]
    public abstract bool? CheckZero(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeApsAwAANANA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHA 6C030000 [4 datoshi]
    /// 09 : CALL 03 [512 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZero2")]
    public abstract bool? CheckZero2(BigInteger? num);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEHB4aGEKCAAAADTsQA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDARG0 [2 datoshi]
    /// 06 : LDLOC0 [2 datoshi]
    /// 07 : STSFLD1 [2 datoshi]
    /// 08 : PUSHA 08000000 [4 datoshi]
    /// 0D : CALL EC [512 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZero3")]
    public abstract bool? CheckZero3(BigInteger? num);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBC3BoZQoKAAAAZXhdNkA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : STSFLD5 [2 datoshi]
    /// 07 : PUSHA 0A000000 [4 datoshi]
    /// 0C : STSFLD5 [2 datoshi]
    /// 0D : LDARG0 [2 datoshi]
    /// 0E : LDSFLD5 [2 datoshi]
    /// 0F : CALLA [512 datoshi]
    /// 10 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("fibo")]
    public abstract BigInteger? Fibo(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBwnB4SnHKchBzIhNpa850aGxnCAopAAAAz2ucc2tqMO3CcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lA
    /// 00 : INITSLOT 0601 [64 datoshi]
    /// 03 : NEWARRAY0 [16 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDARG0 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : STLOC1 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : STLOC2 [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : STLOC3 [2 datoshi]
    /// 0C : JMP 13 [2 datoshi]
    /// 0E : LDLOC1 [2 datoshi]
    /// 0F : LDLOC3 [2 datoshi]
    /// 10 : PICKITEM [64 datoshi]
    /// 11 : STLOC4 [2 datoshi]
    /// 12 : LDLOC0 [2 datoshi]
    /// 13 : LDLOC4 [2 datoshi]
    /// 14 : STSFLD 08 [2 datoshi]
    /// 16 : PUSHA 29000000 [4 datoshi]
    /// 1B : APPEND [8192 datoshi]
    /// 1C : LDLOC3 [2 datoshi]
    /// 1D : INC [4 datoshi]
    /// 1E : STLOC3 [2 datoshi]
    /// 1F : LDLOC3 [2 datoshi]
    /// 20 : LDLOC2 [2 datoshi]
    /// 21 : JMPLT ED [2 datoshi]
    /// 23 : NEWARRAY0 [16 datoshi]
    /// 24 : STLOC1 [2 datoshi]
    /// 25 : LDLOC0 [2 datoshi]
    /// 26 : DUP [2 datoshi]
    /// 27 : STLOC2 [2 datoshi]
    /// 28 : SIZE [4 datoshi]
    /// 29 : STLOC3 [2 datoshi]
    /// 2A : PUSH0 [1 datoshi]
    /// 2B : STLOC4 [2 datoshi]
    /// 2C : JMP 0D [2 datoshi]
    /// 2E : LDLOC2 [2 datoshi]
    /// 2F : LDLOC4 [2 datoshi]
    /// 30 : PICKITEM [64 datoshi]
    /// 31 : STLOC5 [2 datoshi]
    /// 32 : LDLOC1 [2 datoshi]
    /// 33 : LDLOC5 [2 datoshi]
    /// 34 : CALLA [512 datoshi]
    /// 35 : APPEND [8192 datoshi]
    /// 36 : LDLOC4 [2 datoshi]
    /// 37 : INC [4 datoshi]
    /// 38 : STLOC4 [2 datoshi]
    /// 39 : LDLOC4 [2 datoshi]
    /// 3A : LDLOC3 [2 datoshi]
    /// 3B : JMPLT F3 [2 datoshi]
    /// 3D : LDLOC1 [2 datoshi]
    /// 3E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("forEachVar")]
    public abstract IList<object>? ForEachVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBwnAQcSJDeGnOcmhqZwkKWwAAAM9pSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeMq1JLvCcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lA
    /// 00 : INITSLOT 0601 [64 datoshi]
    /// 03 : NEWARRAY0 [16 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : STLOC1 [2 datoshi]
    /// 07 : JMP 43 [2 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : LDLOC1 [2 datoshi]
    /// 0B : PICKITEM [64 datoshi]
    /// 0C : STLOC2 [2 datoshi]
    /// 0D : LDLOC0 [2 datoshi]
    /// 0E : LDLOC2 [2 datoshi]
    /// 0F : STSFLD 09 [2 datoshi]
    /// 11 : PUSHA 5B000000 [4 datoshi]
    /// 16 : APPEND [8192 datoshi]
    /// 17 : LDLOC1 [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : INC [4 datoshi]
    /// 1A : DUP [2 datoshi]
    /// 1B : PUSHINT32 00000080 [1 datoshi]
    /// 20 : JMPGE 04 [2 datoshi]
    /// 22 : JMP 0A [2 datoshi]
    /// 24 : DUP [2 datoshi]
    /// 25 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2A : JMPLE 1E [2 datoshi]
    /// 2C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 35 : AND [8 datoshi]
    /// 36 : DUP [2 datoshi]
    /// 37 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3C : JMPLE 0C [2 datoshi]
    /// 3E : PUSHINT64 0000000001000000 [1 datoshi]
    /// 47 : SUB [8 datoshi]
    /// 48 : STLOC1 [2 datoshi]
    /// 49 : DROP [2 datoshi]
    /// 4A : LDLOC1 [2 datoshi]
    /// 4B : LDARG0 [2 datoshi]
    /// 4C : SIZE [4 datoshi]
    /// 4D : LT [8 datoshi]
    /// 4E : JMPIF BB [2 datoshi]
    /// 50 : NEWARRAY0 [16 datoshi]
    /// 51 : STLOC1 [2 datoshi]
    /// 52 : LDLOC0 [2 datoshi]
    /// 53 : DUP [2 datoshi]
    /// 54 : STLOC2 [2 datoshi]
    /// 55 : SIZE [4 datoshi]
    /// 56 : STLOC3 [2 datoshi]
    /// 57 : PUSH0 [1 datoshi]
    /// 58 : STLOC4 [2 datoshi]
    /// 59 : JMP 0D [2 datoshi]
    /// 5B : LDLOC2 [2 datoshi]
    /// 5C : LDLOC4 [2 datoshi]
    /// 5D : PICKITEM [64 datoshi]
    /// 5E : STLOC5 [2 datoshi]
    /// 5F : LDLOC1 [2 datoshi]
    /// 60 : LDLOC5 [2 datoshi]
    /// 61 : CALLA [512 datoshi]
    /// 62 : APPEND [8192 datoshi]
    /// 63 : LDLOC4 [2 datoshi]
    /// 64 : INC [4 datoshi]
    /// 65 : STLOC4 [2 datoshi]
    /// 66 : LDLOC4 [2 datoshi]
    /// 67 : LDLOC3 [2 datoshi]
    /// 68 : JMPLT F3 [2 datoshi]
    /// 6A : LDLOC1 [2 datoshi]
    /// 6B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("forVar")]
    public abstract IList<object>? ForVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXgKTQMAADZA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : PUSHA 4D030000 [4 datoshi]
    /// 0A : CALLA [512 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("invokeSum")]
    public abstract BigInteger? InvokeSum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEXB5eGhkChAAAAA0A0A=
    /// 00 : INITSLOT 0102 [64 datoshi]
    /// 03 : PUSH1 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDARG1 [2 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : LDLOC0 [2 datoshi]
    /// 08 : STSFLD4 [2 datoshi]
    /// 09 : PUSHA 10000000 [4 datoshi]
    /// 0E : CALL 03 [512 datoshi]
    /// 10 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("invokeSum2")]
    public abstract BigInteger? InvokeSum2(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBCgsAAABwEXhoNkA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSHA 0B000000 [4 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : PUSH1 [1 datoshi]
    /// 0A : LDARG0 [2 datoshi]
    /// 0B : LDLOC0 [2 datoshi]
    /// 0C : CALLA [512 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLambdaDefault")]
    public abstract BigInteger? TestLambdaDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECCgsAAABweXhoNkA=
    /// 00 : INITSLOT 0102 [64 datoshi]
    /// 03 : PUSHA 0B000000 [4 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDARG1 [2 datoshi]
    /// 0A : LDARG0 [2 datoshi]
    /// 0B : LDLOC0 [2 datoshi]
    /// 0C : CALLA [512 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLambdaNotDefault")]
    public abstract BigInteger? TestLambdaNotDefault(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCiwAAAB4NANA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHA 2C000000 [4 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : CALL 03 [512 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("whereGreaterThanZero")]
    public abstract IList<object>? WhereGreaterThanZero(IList<object>? array);

    #endregion
}
