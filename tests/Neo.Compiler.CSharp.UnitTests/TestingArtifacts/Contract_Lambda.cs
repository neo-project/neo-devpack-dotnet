using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Lambda(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Lambda"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""checkZero"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""checkZero2"",""parameters"":[{""name"":""num"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":11,""safe"":false},{""name"":""checkZero3"",""parameters"":[{""name"":""num"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":30,""safe"":false},{""name"":""checkPositiveOdd"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":53,""safe"":false},{""name"":""invokeSum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":64,""safe"":false},{""name"":""invokeSum2"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":76,""safe"":false},{""name"":""fibo"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":110,""safe"":false},{""name"":""changeName"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""String"",""offset"":149,""safe"":false},{""name"":""changeName2"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""String"",""offset"":176,""safe"":false},{""name"":""forEachVar"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":219,""safe"":false},{""name"":""forVar"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":285,""safe"":false},{""name"":""anyGreatThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":350,""safe"":false},{""name"":""anyGreatThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":401,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":424,""safe"":false},{""name"":""testLambdaDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":478,""safe"":false},{""name"":""testLambdaNotDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":499,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":520,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0pAlcAAXgKBwIAADZAVwABeAr8AQAANANAVwACeXg2QFcBARBweGhhCggAAAA07EBXAAF4WZdAVwABeArZAQAANkBXAAJ5eArdAQAANkBXAQIRcHl4aGQKEAAAADQDQFcAA3p5eDZAVwACeHmeXJ5AVwEBC3BoZQoKAAAAZXhdNkBXAAF4ErUmBHhAeBGfXTZ4Ep9dNp5AVwEBeGYKFAAAAHBeDAQgISEhi9soZmg2QF5AVwIBeGcHChIAAABwChwAAABxaDZFaTZAXwcMBCAhISGL2yhnB18HQF8HQFcGAcJweEpxynIQcyITaWvOdGhsZwgKKQAAAM9rnHNrajDtwnFoSnLKcxB0Ig1qbM51aW02z2ycdGxrMPNpQF8IQFcGAcJwEHEiFXhpznJoamcJCi0AAADPaUqccUVpeMq1JOnCcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lAXwlAVwABCikAAAB4NANAVwQCeEpwynEQciIQaGrOc2t5NiYECEBqnHJqaTDwCUBXAAF4ELdAVwACeWcKCgkAAAB4NM1AVwABeF8Kt0BXAAEKLAAAAHg0A0BXBQLCcHhKccpyEHMiEWlrznRseTYmBWhsz2ucc2tqMO9oQFcAAXgQt0BXAQEKCwAAAHAReGg2QFcAAnh5nkBXAQIKCwAAAHB5eGg2QFcAAnh5nkBWDEBXAAF4EJdAVwABeBC3JAQJQHgSohGXQFcAAnh5nkADeYs5"));

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
    /// Script: VwEBeGYKFAAAAHBeDAQgISEhi9soZmg2QA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.STSFLD6 [2 datoshi]
    /// 05 : OpCode.PUSHA 14000000 [4 datoshi]
    /// 0A : OpCode.STLOC0 [2 datoshi]
    /// 0B : OpCode.LDSFLD6 [2 datoshi]
    /// 0C : OpCode.PUSHDATA1 20212121 [8 datoshi]
    /// 12 : OpCode.CAT [2048 datoshi]
    /// 13 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
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
    /// Script: VwABeArZAQAANkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSHA D9010000 [4 datoshi]
    /// 09 : OpCode.CALLA [512 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkPositiveOdd")]
    public abstract bool? CheckPositiveOdd(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAoHAgAANkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSHA 07020000 [4 datoshi]
    /// 09 : OpCode.CALLA [512 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZero")]
    public abstract bool? CheckZero(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAr8AQAANANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSHA FC010000 [4 datoshi]
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
    /// Script: VwYBwnAQcSIVeGnOcmhqZwkKLQAAAM9pSpxxRWl4yrUk6cJxaEpyynMQdCINamzOdWltNs9snHRsazDzaUA=
    /// 00 : OpCode.INITSLOT 0601 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.STLOC1 [2 datoshi]
    /// 07 : OpCode.JMP 15 [2 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.LDLOC1 [2 datoshi]
    /// 0B : OpCode.PICKITEM [64 datoshi]
    /// 0C : OpCode.STLOC2 [2 datoshi]
    /// 0D : OpCode.LDLOC0 [2 datoshi]
    /// 0E : OpCode.LDLOC2 [2 datoshi]
    /// 0F : OpCode.STSFLD 09 [2 datoshi]
    /// 11 : OpCode.PUSHA 2D000000 [4 datoshi]
    /// 16 : OpCode.APPEND [8192 datoshi]
    /// 17 : OpCode.LDLOC1 [2 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.INC [4 datoshi]
    /// 1A : OpCode.STLOC1 [2 datoshi]
    /// 1B : OpCode.DROP [2 datoshi]
    /// 1C : OpCode.LDLOC1 [2 datoshi]
    /// 1D : OpCode.LDARG0 [2 datoshi]
    /// 1E : OpCode.SIZE [4 datoshi]
    /// 1F : OpCode.LT [8 datoshi]
    /// 20 : OpCode.JMPIF E9 [2 datoshi]
    /// 22 : OpCode.NEWARRAY0 [16 datoshi]
    /// 23 : OpCode.STLOC1 [2 datoshi]
    /// 24 : OpCode.LDLOC0 [2 datoshi]
    /// 25 : OpCode.DUP [2 datoshi]
    /// 26 : OpCode.STLOC2 [2 datoshi]
    /// 27 : OpCode.SIZE [4 datoshi]
    /// 28 : OpCode.STLOC3 [2 datoshi]
    /// 29 : OpCode.PUSH0 [1 datoshi]
    /// 2A : OpCode.STLOC4 [2 datoshi]
    /// 2B : OpCode.JMP 0D [2 datoshi]
    /// 2D : OpCode.LDLOC2 [2 datoshi]
    /// 2E : OpCode.LDLOC4 [2 datoshi]
    /// 2F : OpCode.PICKITEM [64 datoshi]
    /// 30 : OpCode.STLOC5 [2 datoshi]
    /// 31 : OpCode.LDLOC1 [2 datoshi]
    /// 32 : OpCode.LDLOC5 [2 datoshi]
    /// 33 : OpCode.CALLA [512 datoshi]
    /// 34 : OpCode.APPEND [8192 datoshi]
    /// 35 : OpCode.LDLOC4 [2 datoshi]
    /// 36 : OpCode.INC [4 datoshi]
    /// 37 : OpCode.STLOC4 [2 datoshi]
    /// 38 : OpCode.LDLOC4 [2 datoshi]
    /// 39 : OpCode.LDLOC3 [2 datoshi]
    /// 3A : OpCode.JMPLT F3 [2 datoshi]
    /// 3C : OpCode.LDLOC1 [2 datoshi]
    /// 3D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("forVar")]
    public abstract IList<object>? ForVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXgK3QEAADZA
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.PUSHA DD010000 [4 datoshi]
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
