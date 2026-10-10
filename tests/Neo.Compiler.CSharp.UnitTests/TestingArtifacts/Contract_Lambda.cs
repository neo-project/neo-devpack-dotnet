using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Lambda(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Lambda"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""checkZero"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""checkZero2"",""parameters"":[{""name"":""num"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":11,""safe"":false},{""name"":""checkZero3"",""parameters"":[{""name"":""num"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":30,""safe"":false},{""name"":""checkPositiveOdd"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":53,""safe"":false},{""name"":""invokeSum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":64,""safe"":false},{""name"":""invokeSum2"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":76,""safe"":false},{""name"":""fibo"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":176,""safe"":false},{""name"":""changeName"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""String"",""offset"":313,""safe"":false},{""name"":""changeName2"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""String"",""offset"":340,""safe"":false},{""name"":""forEachVar"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":383,""safe"":false},{""name"":""forVar"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":449,""safe"":false},{""name"":""anyGreatThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":546,""safe"":false},{""name"":""anyGreatThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":597,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Array"",""offset"":620,""safe"":false},{""name"":""testLambdaDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":674,""safe"":false},{""name"":""testLambdaNotDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":728,""safe"":false},{""name"":""testLambdaInModifier"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":782,""safe"":false},{""name"":""testLambdaScopedInModifier"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":835,""safe"":false},{""name"":""testLambdaRefReadonlyModifier"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":888,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":941,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3vA1cAAXgKrAMAADZAVwABeAqhAwAANANAVwACeXg2QFcBARBweGhhCggAAAA07EBXAAF4WbNAVwABeAp+AwAANkBXAAJ5eAqCAwAANkBXAQIRcHl4aGQKEAAAADQDQFcAA3p5eDZAVwACeHmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfXJ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwEBC3BoZQoKAAAAZXhdNkBXAAF4ErUmBHhAeJ1KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9dNnidnUrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn102nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAQF4ZgoUAAAAcF4MBCAhISGL2yhmaDZAXkBXAgF4ZwcKEgAAAHAKHAAAAHFoNkVpNkBfBwwEICEhIYvbKGcHXwdAXwdAVwYBwnB4SnHKchBzIhNpa850aGxnCAopAAAAz2ucc2tqMO3CcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lAXwhAVwYBwnAQcSI2eGnOcmhqZwkKTQAAAM9pSpxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4yjDJwnFoSnLKcxB0Ig1qbM51aW02z2ycdGxrMPNpQF8JQFcAAQopAAAAeDQDQFcEAnhKcMpxEHIiEGhqznNreTYmBAhAapxyamkw8AlAVwABeBC3QFcAAnlnCgoJAAAAeDTNQFcAAXhfCrdAVwABCiwAAAB4NANAVwUCwnB4SnHKchBzIhFpa850bHk2JgVobM9rnHNrajDvaEBXAAF4ELdAVwEBCgsAAABwEXhoNkBXAAJ4eZ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwECCgsAAABweXhoNkBXAAJ4eZ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwEBCgoAAABweGg2QFcAAXgUnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAQEKCgAAAHB4aDZAVwABeBWeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcBAQoKAAAAcHhoNkBXAAF4Fp5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVgxAVwABeBCzQFcAAXgQtyQECUB4EqIRs0BXAAJ4eZ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AByzVxQ==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWcKCgkAAAB4NM1A
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// STSFLD 0A [2 datoshi]
    /// PUSHA 09000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL CD [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreatThan")]
    public abstract bool? AnyGreatThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCikAAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 29000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreatThanZero")]
    public abstract bool? AnyGreatThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeGYKFAAAAHBeDAQgISEhi9soZmg2QA==
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STSFLD6 [2 datoshi]
    /// PUSHA 14000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDSFLD6 [2 datoshi]
    /// PUSHDATA1 20212121 [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// STSFLD6 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("changeName")]
    public abstract string? ChangeName(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBeGcHChIAAABwChwAAABxaDZFaTZA
    /// INITSLOT 0201 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STSFLD 07 [2 datoshi]
    /// PUSHA 12000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHA 1C000000 [4 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLA [512 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("changeName2")]
    public abstract string? ChangeName2(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAp+AwAANkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHA 7E030000 [4 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkPositiveOdd")]
    public abstract bool? CheckPositiveOdd(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAqsAwAANkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHA AC030000 [4 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZero")]
    public abstract bool? CheckZero(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAqhAwAANANA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHA A1030000 [4 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZero2")]
    public abstract bool? CheckZero2(BigInteger? num);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEHB4aGEKCAAAADTsQA==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STSFLD1 [2 datoshi]
    /// PUSHA 08000000 [4 datoshi]
    /// CALL EC [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZero3")]
    public abstract bool? CheckZero3(BigInteger? num);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBC3BoZQoKAAAAZXhdNkA=
    /// INITSLOT 0101 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STSFLD5 [2 datoshi]
    /// PUSHA 0A000000 [4 datoshi]
    /// STSFLD5 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDSFLD5 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("fibo")]
    public abstract BigInteger? Fibo(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBwnB4SnHKchBzIhNpa850aGxnCAopAAAAz2ucc2tqMO3CcWhKcspzEHQiDWpsznVpbTbPbJx0bGsw82lA
    /// INITSLOT 0601 [64 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC3 [2 datoshi]
    /// JMP 13 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// PUSHA 29000000 [4 datoshi]
    /// APPEND [8192 datoshi]
    /// LDLOC3 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// JMPLT ED [2 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC4 [2 datoshi]
    /// JMP 0D [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC5 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC5 [2 datoshi]
    /// CALLA [512 datoshi]
    /// APPEND [8192 datoshi]
    /// LDLOC4 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// JMPLT F3 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("forEachVar")]
    public abstract IList<object>? ForEachVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBwnAQcSI2eGnOcmhqZwkKTQAAAM9pSpxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4yjDJwnFoSnLKcxB0Ig1qbM51aW02z2ycdGxrMPNpQA==
    /// INITSLOT 0601 [64 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// JMP 36 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// STSFLD 09 [2 datoshi]
    /// PUSHA 4D000000 [4 datoshi]
    /// APPEND [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// JMPLT C9 [2 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC4 [2 datoshi]
    /// JMP 0D [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC5 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC5 [2 datoshi]
    /// CALLA [512 datoshi]
    /// APPEND [8192 datoshi]
    /// LDLOC4 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// JMPLT F3 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("forVar")]
    public abstract IList<object>? ForVar(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXgKggMAADZA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHA 82030000 [4 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("invokeSum")]
    public abstract BigInteger? InvokeSum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEXB5eGhkChAAAAA0A0A=
    /// INITSLOT 0102 [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STSFLD4 [2 datoshi]
    /// PUSHA 10000000 [4 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("invokeSum2")]
    public abstract BigInteger? InvokeSum2(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBCgsAAABwEXhoNkA=
    /// INITSLOT 0101 [64 datoshi]
    /// PUSHA 0B000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLambdaDefault")]
    public abstract BigInteger? TestLambdaDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBCgoAAABweGg2QA==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSHA 0A000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLambdaInModifier")]
    public abstract BigInteger? TestLambdaInModifier(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECCgsAAABweXhoNkA=
    /// INITSLOT 0102 [64 datoshi]
    /// PUSHA 0B000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLambdaNotDefault")]
    public abstract BigInteger? TestLambdaNotDefault(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBCgoAAABweGg2QA==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSHA 0A000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLambdaRefReadonlyModifier")]
    public abstract BigInteger? TestLambdaRefReadonlyModifier(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBCgoAAABweGg2QA==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSHA 0A000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLambdaScopedInModifier")]
    public abstract BigInteger? TestLambdaScopedInModifier(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCiwAAAB4NANA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHA 2C000000 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("whereGreaterThanZero")]
    public abstract IList<object>? WhereGreaterThanZero(IList<object>? array);

    #endregion
}
