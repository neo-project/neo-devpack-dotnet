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
    /// Script: VwABeArZAQAANkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHA D9010000
    /// 09 : OpCode.CALLA
    /// 0A : OpCode.RET
    /// </remarks>
    [DisplayName("checkPositiveOdd")]
    public abstract bool? CheckPositiveOdd(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAoHAgAANkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHA 07020000
    /// 09 : OpCode.CALLA
    /// 0A : OpCode.RET
    /// </remarks>
    [DisplayName("checkZero")]
    public abstract bool? CheckZero(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAr8AQAANANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHA FC010000
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
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
    /// Script: VwYBwnAQcSIVeGnOcmhqZwkKLQAAAM9pSpxxRWl4yrUk6cJxaEpyynMQdCINamzOdWltNs9snHRsazDzaUA=
    /// 00 : OpCode.INITSLOT 0601
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.JMP 15
    /// 09 : OpCode.LDARG0
    /// 0A : OpCode.LDLOC1
    /// 0B : OpCode.PICKITEM
    /// 0C : OpCode.STLOC2
    /// 0D : OpCode.LDLOC0
    /// 0E : OpCode.LDLOC2
    /// 0F : OpCode.STSFLD 09
    /// 11 : OpCode.PUSHA 2D000000
    /// 16 : OpCode.APPEND
    /// 17 : OpCode.LDLOC1
    /// 18 : OpCode.DUP
    /// 19 : OpCode.INC
    /// 1A : OpCode.STLOC1
    /// 1B : OpCode.DROP
    /// 1C : OpCode.LDLOC1
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.SIZE
    /// 1F : OpCode.LT
    /// 20 : OpCode.JMPIF E9
    /// 22 : OpCode.NEWARRAY0
    /// 23 : OpCode.STLOC1
    /// 24 : OpCode.LDLOC0
    /// 25 : OpCode.DUP
    /// 26 : OpCode.STLOC2
    /// 27 : OpCode.SIZE
    /// 28 : OpCode.STLOC3
    /// 29 : OpCode.PUSH0
    /// 2A : OpCode.STLOC4
    /// 2B : OpCode.JMP 0D
    /// 2D : OpCode.LDLOC2
    /// 2E : OpCode.LDLOC4
    /// 2F : OpCode.PICKITEM
    /// 30 : OpCode.STLOC5
    /// 31 : OpCode.LDLOC1
    /// 32 : OpCode.LDLOC5
    /// 33 : OpCode.CALLA
    /// 34 : OpCode.APPEND
    /// 35 : OpCode.LDLOC4
    /// 36 : OpCode.INC
    /// 37 : OpCode.STLOC4
    /// 38 : OpCode.LDLOC4
    /// 39 : OpCode.LDLOC3
    /// 3A : OpCode.JMPLT F3
    /// 3C : OpCode.LDLOC1
    /// 3D : OpCode.RET
    /// </remarks>
    [DisplayName("forVar")]
    public abstract IList<object>? ForVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXgK3QEAADZA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.PUSHA DD010000
    /// 0A : OpCode.CALLA
    /// 0B : OpCode.RET
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
