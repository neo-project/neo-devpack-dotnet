using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3HA1cAAXgKdwMAADZAVwABeApsAwAANANAVwACeXg2QFcBARBweGhhCggAAAA07EBXAAF4WZdAVwABeApJAwAANkBXAAJ5eApNAwAANkBXAQIRcHl4aGQKEAAAADQDQFcAA3p5eDZAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1yeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAQELcGhlCgoAAABleF02QFcAAXgStSYEeEB4EZ9KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfXTZ4Ep9KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfXTaeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAQF4ZgoUAAAAcF4MBCAhISGL2yhmaDZAXkBXAgF4ZwcKEgAAAHAKHAAAAHFoNkVpNkBfBwwEICEhIYvbKGcHXwdAXwdAVwYBwnB4SnHKchBzIhNpa850aGxnCAopAAAAz2ucc2tqMO3CcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lAXwhAVwYBwnAQcSJDeGnOcmhqZwkKWwAAAM9pSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeMq1JLvCcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lAXwlAVwABCikAAAB4NANAVwQCeEpwynEQciIQaGrOc2t5NiYECEBqnHJqaTDwCUBXAAF4ELdAVwACeWcKCgkAAAB4NM1AVwABeF8Kt0BXAAEKLAAAAHg0A0BXBQLCcHhKccpyEHMiEWlrznRseTYmBWhsz2ucc2tqMO9oQFcAAXgQt0BXAQEKCwAAAHAReGg2QFcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwECCgsAAABweXhoNkBXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFYMQFcAAXgQl0BXAAF4ELckBAlAeBKiEZdAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0CJI/8f"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWcKCgkAAAB4NM1A
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.STSFLD 0A [2 datoshi]
    /// 06 : OpCode.PUSHA 09000000 [4 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.CALL CD [512 datoshi]
    /// 0E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreatThan")]
    public abstract bool? AnyGreatThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCikAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 29000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreatThanZero")]
    public abstract bool? AnyGreatThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeGYKFAAAAHBeDCAhISGL2yhmaDZA
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.STSFLD6 [2 datoshi]
    /// 05 : OpCode.PUSHA 14000000 [4 datoshi]
    /// 0A : OpCode.STLOC0 [2 datoshi]
    /// 0B : OpCode.LDSFLD6 [2 datoshi]
    /// 0C : OpCode.PUSHDATA1 20212121 [8 datoshi]
    /// 12 : OpCode.CAT [2048 datoshi]
    /// 13 : OpCode.CONVERT 40 'ByteString' [8192 datoshi]
    /// 15 : OpCode.STSFLD6 [2 datoshi]
    /// 16 : OpCode.LDLOC0 [2 datoshi]
    /// 17 : OpCode.CALLA [512 datoshi]
    /// 18 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("changeName")]
    public abstract string? ChangeName(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBeGcHChIAAABwChwAAABxaDZFaTZA
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.STSFLD 07 [2 datoshi]
    /// 06 : OpCode.PUSHA 12000000 [4 datoshi]
    /// 0B : OpCode.STLOC0 [2 datoshi]
    /// 0C : OpCode.PUSHA 1C000000 [4 datoshi]
    /// 11 : OpCode.STLOC1 [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.CALLA [512 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.LDLOC1 [2 datoshi]
    /// 16 : OpCode.CALLA [512 datoshi]
    /// 17 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("changeName2")]
    public abstract string? ChangeName2(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeApJAwAANkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSHA 49030000 [4 datoshi]
    /// 09 : OpCode.CALLA [512 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkPositiveOdd")]
    public abstract bool? CheckPositiveOdd(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAp3AwAANkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSHA 77030000 [4 datoshi]
    /// 09 : OpCode.CALLA [512 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZero")]
    public abstract bool? CheckZero(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeApsAwAANANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSHA 6C030000 [4 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZero2")]
    public abstract bool? CheckZero2(BigInteger? num);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEHB4aGEKCAAAADTsQA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// 06 : OpCode.LDLOC0 [2 datoshi]
    /// 07 : OpCode.STSFLD1 [2 datoshi]
    /// 08 : OpCode.PUSHA 08000000 [4 datoshi]
    /// 0D : OpCode.CALL EC [512 datoshi]
    /// 0F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZero3")]
    public abstract bool? CheckZero3(BigInteger? num);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBC3BoZQoKAAAAZXhdNkA=
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSHNULL [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.STSFLD5 [2 datoshi]
    /// 07 : OpCode.PUSHA 0A000000 [4 datoshi]
    /// 0C : OpCode.STSFLD5 [2 datoshi]
    /// 0D : OpCode.LDARG0 [2 datoshi]
    /// 0E : OpCode.LDSFLD5 [2 datoshi]
    /// 0F : OpCode.CALLA [512 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("fibo")]
    public abstract BigInteger? Fibo(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBwnB4SnHKchBzIhNpa850aGxnCAopAAAAz2ucc2tqMO3CcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lA
    /// 00 : OpCode.INITSLOT 0601 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.STLOC1 [2 datoshi]
    /// 08 : OpCode.SIZE [4 datoshi]
    /// 09 : OpCode.STLOC2 [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.STLOC3 [2 datoshi]
    /// 0C : OpCode.JMP 13 [2 datoshi]
    /// 0E : OpCode.LDLOC1 [2 datoshi]
    /// 0F : OpCode.LDLOC3 [2 datoshi]
    /// 10 : OpCode.PICKITEM [64 datoshi]
    /// 11 : OpCode.STLOC4 [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.LDLOC4 [2 datoshi]
    /// 14 : OpCode.STSFLD 08 [2 datoshi]
    /// 16 : OpCode.PUSHA 29000000 [4 datoshi]
    /// 1B : OpCode.APPEND [8192 datoshi]
    /// 1C : OpCode.LDLOC3 [2 datoshi]
    /// 1D : OpCode.INC [4 datoshi]
    /// 1E : OpCode.STLOC3 [2 datoshi]
    /// 1F : OpCode.LDLOC3 [2 datoshi]
    /// 20 : OpCode.LDLOC2 [2 datoshi]
    /// 21 : OpCode.JMPLT ED [2 datoshi]
    /// 23 : OpCode.NEWARRAY0 [16 datoshi]
    /// 24 : OpCode.STLOC1 [2 datoshi]
    /// 25 : OpCode.LDLOC0 [2 datoshi]
    /// 26 : OpCode.DUP [2 datoshi]
    /// 27 : OpCode.STLOC2 [2 datoshi]
    /// 28 : OpCode.SIZE [4 datoshi]
    /// 29 : OpCode.STLOC3 [2 datoshi]
    /// 2A : OpCode.PUSH0 [1 datoshi]
    /// 2B : OpCode.STLOC4 [2 datoshi]
    /// 2C : OpCode.JMP 0D [2 datoshi]
    /// 2E : OpCode.LDLOC2 [2 datoshi]
    /// 2F : OpCode.LDLOC4 [2 datoshi]
    /// 30 : OpCode.PICKITEM [64 datoshi]
    /// 31 : OpCode.STLOC5 [2 datoshi]
    /// 32 : OpCode.LDLOC1 [2 datoshi]
    /// 33 : OpCode.LDLOC5 [2 datoshi]
    /// 34 : OpCode.CALLA [512 datoshi]
    /// 35 : OpCode.APPEND [8192 datoshi]
    /// 36 : OpCode.LDLOC4 [2 datoshi]
    /// 37 : OpCode.INC [4 datoshi]
    /// 38 : OpCode.STLOC4 [2 datoshi]
    /// 39 : OpCode.LDLOC4 [2 datoshi]
    /// 3A : OpCode.LDLOC3 [2 datoshi]
    /// 3B : OpCode.JMPLT F3 [2 datoshi]
    /// 3D : OpCode.LDLOC1 [2 datoshi]
    /// 3E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("forEachVar")]
    public abstract IList<object>? ForEachVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBwnAQcSJDeGnOcmhqZwkKWwAAAM9pSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeMq1JLvCcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lA
    /// 00 : OpCode.INITSLOT 0601 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.STLOC1 [2 datoshi]
    /// 07 : OpCode.JMP 43 [2 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.LDLOC1 [2 datoshi]
    /// 0B : OpCode.PICKITEM [64 datoshi]
    /// 0C : OpCode.STLOC2 [2 datoshi]
    /// 0D : OpCode.LDLOC0 [2 datoshi]
    /// 0E : OpCode.LDLOC2 [2 datoshi]
    /// 0F : OpCode.STSFLD 09 [2 datoshi]
    /// 11 : OpCode.PUSHA 5B000000 [4 datoshi]
    /// 16 : OpCode.APPEND [8192 datoshi]
    /// 17 : OpCode.LDLOC1 [2 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.INC [4 datoshi]
    /// 1A : OpCode.DUP [2 datoshi]
    /// 1B : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 20 : OpCode.JMPGE 04 [2 datoshi]
    /// 22 : OpCode.JMP 0A [2 datoshi]
    /// 24 : OpCode.DUP [2 datoshi]
    /// 25 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2A : OpCode.JMPLE 1E [2 datoshi]
    /// 2C : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 35 : OpCode.AND [8 datoshi]
    /// 36 : OpCode.DUP [2 datoshi]
    /// 37 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3C : OpCode.JMPLE 0C [2 datoshi]
    /// 3E : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 47 : OpCode.SUB [8 datoshi]
    /// 48 : OpCode.STLOC1 [2 datoshi]
    /// 49 : OpCode.DROP [2 datoshi]
    /// 4A : OpCode.LDLOC1 [2 datoshi]
    /// 4B : OpCode.LDARG0 [2 datoshi]
    /// 4C : OpCode.SIZE [4 datoshi]
    /// 4D : OpCode.LT [8 datoshi]
    /// 4E : OpCode.JMPIF BB [2 datoshi]
    /// 50 : OpCode.NEWARRAY0 [16 datoshi]
    /// 51 : OpCode.STLOC1 [2 datoshi]
    /// 52 : OpCode.LDLOC0 [2 datoshi]
    /// 53 : OpCode.DUP [2 datoshi]
    /// 54 : OpCode.STLOC2 [2 datoshi]
    /// 55 : OpCode.SIZE [4 datoshi]
    /// 56 : OpCode.STLOC3 [2 datoshi]
    /// 57 : OpCode.PUSH0 [1 datoshi]
    /// 58 : OpCode.STLOC4 [2 datoshi]
    /// 59 : OpCode.JMP 0D [2 datoshi]
    /// 5B : OpCode.LDLOC2 [2 datoshi]
    /// 5C : OpCode.LDLOC4 [2 datoshi]
    /// 5D : OpCode.PICKITEM [64 datoshi]
    /// 5E : OpCode.STLOC5 [2 datoshi]
    /// 5F : OpCode.LDLOC1 [2 datoshi]
    /// 60 : OpCode.LDLOC5 [2 datoshi]
    /// 61 : OpCode.CALLA [512 datoshi]
    /// 62 : OpCode.APPEND [8192 datoshi]
    /// 63 : OpCode.LDLOC4 [2 datoshi]
    /// 64 : OpCode.INC [4 datoshi]
    /// 65 : OpCode.STLOC4 [2 datoshi]
    /// 66 : OpCode.LDLOC4 [2 datoshi]
    /// 67 : OpCode.LDLOC3 [2 datoshi]
    /// 68 : OpCode.JMPLT F3 [2 datoshi]
    /// 6A : OpCode.LDLOC1 [2 datoshi]
    /// 6B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("forVar")]
    public abstract IList<object>? ForVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXgKTQMAADZA
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.PUSHA 4D030000 [4 datoshi]
    /// 0A : OpCode.CALLA [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("invokeSum")]
    public abstract BigInteger? InvokeSum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEXB5eGhkChAAAAA0A0A=
    /// 00 : OpCode.INITSLOT 0102 [64 datoshi]
    /// 03 : OpCode.PUSH1 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG1 [2 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.LDLOC0 [2 datoshi]
    /// 08 : OpCode.STSFLD4 [2 datoshi]
    /// 09 : OpCode.PUSHA 10000000 [4 datoshi]
    /// 0E : OpCode.CALL 03 [512 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("invokeSum2")]
    public abstract BigInteger? InvokeSum2(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBCgsAAABwEXhoNkA=
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSHA 0B000000 [4 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.PUSH1 [1 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.LDLOC0 [2 datoshi]
    /// 0C : OpCode.CALLA [512 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLambdaDefault")]
    public abstract BigInteger? TestLambdaDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECCgsAAABweXhoNkA=
    /// 00 : OpCode.INITSLOT 0102 [64 datoshi]
    /// 03 : OpCode.PUSHA 0B000000 [4 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.LDARG1 [2 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.LDLOC0 [2 datoshi]
    /// 0C : OpCode.CALLA [512 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLambdaNotDefault")]
    public abstract BigInteger? TestLambdaNotDefault(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCiwAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 2C000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("whereGreaterThanZero")]
    public abstract IList<object>? WhereGreaterThanZero(IList<object>? array);

    #endregion
}
