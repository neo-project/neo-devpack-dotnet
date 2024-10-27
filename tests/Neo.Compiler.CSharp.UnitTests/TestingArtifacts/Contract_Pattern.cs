using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Pattern(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Pattern"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""between"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""between2"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":17,""safe"":false},{""name"":""between3"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":34,""safe"":false},{""name"":""testRecursivePattern"",""parameters"":[],""returntype"":""Boolean"",""offset"":79,""safe"":false},{""name"":""between4"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":134,""safe"":false},{""name"":""testNotPattern"",""parameters"":[{""name"":""x"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":143,""safe"":false},{""name"":""classify"",""parameters"":[{""name"":""measurement"",""type"":""Integer""}],""returntype"":""String"",""offset"":152,""safe"":false},{""name"":""getCalendarSeason"",""parameters"":[{""name"":""month"",""type"":""Integer""}],""returntype"":""String"",""offset"":259,""safe"":false},{""name"":""testDeclarationPattern"",""parameters"":[],""returntype"":""Void"",""offset"":421,""safe"":false},{""name"":""testTypePattern"",""parameters"":[{""name"":""o1"",""type"":""Any""}],""returntype"":""Void"",""offset"":505,""safe"":false},{""name"":""testTypePattern2"",""parameters"":[{""name"":""t"",""type"":""Any""}],""returntype"":""Integer"",""offset"":531,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/UkCVwEBeHBoEbckBAlAaABktUBXAQF4cGgRtyQECUBoAGS1QFcBAXhwaBG3JAUJIgZoADK1JgQIQGgAMrgkBQkiBmgAZLUmBAhACCYECUBoOlcCAAwUAAAAAAAAAAAAAAAAAAAAAAAAAABwaHFpStkoUMoAFLOrCJexqgmXJgQIQAgmBAlAaTpXAQF4cGgQtkBXAQF4cGjYqkBXAQF4cGgA2LUmDAwHVG9vIGxvd0BoANi4JAUJIgVoELUmCAwDTG93QGgQuCQFCSIFaBq1Jg8MCkFjY2VwdGFibGVAaBq4JAUJIgZoABS1JgkMBEhpZ2hAaAAUuCYNDAhUb28gaGlnaEBoOlcBAXhwaBOXJgUIIgVoFJcmBQgiBWgVlyYLDAZzcHJpbmdAaBaXJgUIIgVoF5cmBQgiBWgYlyYLDAZzdW1tZXJAaBmXJgUIIgVoGpcmBQgiBWgblyYLDAZhdXR1bW5AaByXJgUIIgVoEZcmBQgiBWgSlyYLDAZ3aW50ZXJACCYiDBJVbmV4cGVjdGVkIG1vbnRoOiB4NwAAiwwBLovbKDpoOlcEAAwNSGVsbG8sIFdvcmxkIXBocWnZKGlyJghqQc/nR5YMDUhlbGxvLCBXb3JsZCFyanNr2SgmHAwTZ3JlZXRpbmcyIGlzIHN0cmluZ0HP50eWQFcBAXhwaNkwJgNAaNkoJgNAaNkgJgNAQEBAVwEBeHBo2TAmBQgiBWjZKCYFCCIHaNkoCJcmBBBAaNkgJgQRQGjZIQiXJgQSQAgmBBVAaDpAhNrwLg=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("between")]
    public abstract bool? Between(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("between2")]
    public abstract bool? Between2(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("between3")]
    public abstract bool? Between3(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("between4")]
    public abstract bool? Between4(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoANi1JgwMVG9vIGxvd0BoANi4JAUJIgVoELUmCAxMb3dAaBC4JAUJIgVoGrUmDwxBY2NlcHRhYmxlQGgauCQFCSIGaAAUtSYJDEhpZ2hAaAAUuCYNDFRvbyBoaWdoQGg6
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHINT8 D8
    /// 08 : OpCode.LT
    /// 09 : OpCode.JMPIFNOT 0C
    /// 0B : OpCode.PUSHDATA1 546F6F206C6F77
    /// 14 : OpCode.RET
    /// 15 : OpCode.LDLOC0
    /// 16 : OpCode.PUSHINT8 D8
    /// 18 : OpCode.GE
    /// 19 : OpCode.JMPIF 05
    /// 1B : OpCode.PUSHF
    /// 1C : OpCode.JMP 05
    /// 1E : OpCode.LDLOC0
    /// 1F : OpCode.PUSH0
    /// 20 : OpCode.LT
    /// 21 : OpCode.JMPIFNOT 08
    /// 23 : OpCode.PUSHDATA1 4C6F77
    /// 28 : OpCode.RET
    /// 29 : OpCode.LDLOC0
    /// 2A : OpCode.PUSH0
    /// 2B : OpCode.GE
    /// 2C : OpCode.JMPIF 05
    /// 2E : OpCode.PUSHF
    /// 2F : OpCode.JMP 05
    /// 31 : OpCode.LDLOC0
    /// 32 : OpCode.PUSH10
    /// 33 : OpCode.LT
    /// 34 : OpCode.JMPIFNOT 0F
    /// 36 : OpCode.PUSHDATA1 41636365707461626C65
    /// 42 : OpCode.RET
    /// 43 : OpCode.LDLOC0
    /// 44 : OpCode.PUSH10
    /// 45 : OpCode.GE
    /// 46 : OpCode.JMPIF 05
    /// 48 : OpCode.PUSHF
    /// 49 : OpCode.JMP 06
    /// 4B : OpCode.LDLOC0
    /// 4C : OpCode.PUSHINT8 14
    /// 4E : OpCode.LT
    /// 4F : OpCode.JMPIFNOT 09
    /// 51 : OpCode.PUSHDATA1 48696768
    /// 57 : OpCode.RET
    /// 58 : OpCode.LDLOC0
    /// 59 : OpCode.PUSHINT8 14
    /// 5B : OpCode.GE
    /// 5C : OpCode.JMPIFNOT 0D
    /// 5E : OpCode.PUSHDATA1 546F6F2068696768
    /// 68 : OpCode.RET
    /// 69 : OpCode.LDLOC0
    /// 6A : OpCode.THROW
    /// </remarks>
    [DisplayName("classify")]
    public abstract string? Classify(BigInteger? measurement);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoE5cmBQgiBWgUlyYFCCIFaBWXJgsMc3ByaW5nQGgWlyYFCCIFaBeXJgUIIgVoGJcmCwxzdW1tZXJAaBmXJgUIIgVoGpcmBQgiBWgblyYLDGF1dHVtbkBoHJcmBQgiBWgRlyYFCCIFaBKXJgsMd2ludGVyQAgmIgxVbmV4cGVjdGVkIG1vbnRoOiB4NwAAiwwui9soOmg6
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSH3
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.JMPIFNOT 05
    /// 0A : OpCode.PUSHT
    /// 0B : OpCode.JMP 05
    /// 0D : OpCode.LDLOC0
    /// 0E : OpCode.PUSH4
    /// 0F : OpCode.EQUAL
    /// 10 : OpCode.JMPIFNOT 05
    /// 12 : OpCode.PUSHT
    /// 13 : OpCode.JMP 05
    /// 15 : OpCode.LDLOC0
    /// 16 : OpCode.PUSH5
    /// 17 : OpCode.EQUAL
    /// 18 : OpCode.JMPIFNOT 0B
    /// 1A : OpCode.PUSHDATA1 737072696E67
    /// 22 : OpCode.RET
    /// 23 : OpCode.LDLOC0
    /// 24 : OpCode.PUSH6
    /// 25 : OpCode.EQUAL
    /// 26 : OpCode.JMPIFNOT 05
    /// 28 : OpCode.PUSHT
    /// 29 : OpCode.JMP 05
    /// 2B : OpCode.LDLOC0
    /// 2C : OpCode.PUSH7
    /// 2D : OpCode.EQUAL
    /// 2E : OpCode.JMPIFNOT 05
    /// 30 : OpCode.PUSHT
    /// 31 : OpCode.JMP 05
    /// 33 : OpCode.LDLOC0
    /// 34 : OpCode.PUSH8
    /// 35 : OpCode.EQUAL
    /// 36 : OpCode.JMPIFNOT 0B
    /// 38 : OpCode.PUSHDATA1 73756D6D6572
    /// 40 : OpCode.RET
    /// 41 : OpCode.LDLOC0
    /// 42 : OpCode.PUSH9
    /// 43 : OpCode.EQUAL
    /// 44 : OpCode.JMPIFNOT 05
    /// 46 : OpCode.PUSHT
    /// 47 : OpCode.JMP 05
    /// 49 : OpCode.LDLOC0
    /// 4A : OpCode.PUSH10
    /// 4B : OpCode.EQUAL
    /// 4C : OpCode.JMPIFNOT 05
    /// 4E : OpCode.PUSHT
    /// 4F : OpCode.JMP 05
    /// 51 : OpCode.LDLOC0
    /// 52 : OpCode.PUSH11
    /// 53 : OpCode.EQUAL
    /// 54 : OpCode.JMPIFNOT 0B
    /// 56 : OpCode.PUSHDATA1 617574756D6E
    /// 5E : OpCode.RET
    /// 5F : OpCode.LDLOC0
    /// 60 : OpCode.PUSH12
    /// 61 : OpCode.EQUAL
    /// 62 : OpCode.JMPIFNOT 05
    /// 64 : OpCode.PUSHT
    /// 65 : OpCode.JMP 05
    /// 67 : OpCode.LDLOC0
    /// 68 : OpCode.PUSH1
    /// 69 : OpCode.EQUAL
    /// 6A : OpCode.JMPIFNOT 05
    /// 6C : OpCode.PUSHT
    /// 6D : OpCode.JMP 05
    /// 6F : OpCode.LDLOC0
    /// 70 : OpCode.PUSH2
    /// 71 : OpCode.EQUAL
    /// 72 : OpCode.JMPIFNOT 0B
    /// 74 : OpCode.PUSHDATA1 77696E746572
    /// 7C : OpCode.RET
    /// 7D : OpCode.PUSHT
    /// 7E : OpCode.JMPIFNOT 22
    /// 80 : OpCode.PUSHDATA1 556E6578706563746564206D6F6E74683A20
    /// 94 : OpCode.LDARG0
    /// 95 : OpCode.CALLT 0000
    /// 98 : OpCode.CAT
    /// 99 : OpCode.PUSHDATA1 2E
    /// 9C : OpCode.CAT
    /// 9D : OpCode.CONVERT 28
    /// 9F : OpCode.THROW
    /// A0 : OpCode.LDLOC0
    /// A1 : OpCode.THROW
    /// </remarks>
    [DisplayName("getCalendarSeason")]
    public abstract string? GetCalendarSeason(BigInteger? month);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQADEhlbGxvLCBXb3JsZCFwaHFp2ShpciYIakHP50eWDEhlbGxvLCBXb3JsZCFyanNr2SgmHAxncmVldGluZzIgaXMgc3RyaW5nQc/nR5ZA
    /// 00 : OpCode.INITSLOT 0400
    /// 03 : OpCode.PUSHDATA1 48656C6C6F2C20576F726C6421
    /// 12 : OpCode.STLOC0
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.STLOC1
    /// 15 : OpCode.LDLOC1
    /// 16 : OpCode.ISTYPE 28
    /// 18 : OpCode.LDLOC1
    /// 19 : OpCode.STLOC2
    /// 1A : OpCode.JMPIFNOT 08
    /// 1C : OpCode.LDLOC2
    /// 1D : OpCode.SYSCALL CFE74796
    /// 22 : OpCode.PUSHDATA1 48656C6C6F2C20576F726C6421
    /// 31 : OpCode.STLOC2
    /// 32 : OpCode.LDLOC2
    /// 33 : OpCode.STLOC3
    /// 34 : OpCode.LDLOC3
    /// 35 : OpCode.ISTYPE 28
    /// 37 : OpCode.JMPIFNOT 1C
    /// 39 : OpCode.PUSHDATA1 6772656574696E673220697320737472696E67
    /// 4E : OpCode.SYSCALL CFE74796
    /// 53 : OpCode.RET
    /// </remarks>
    [DisplayName("testDeclarationPattern")]
    public abstract void TestDeclarationPattern();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2KpA
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.ISNULL
    /// 07 : OpCode.NOT
    /// 08 : OpCode.RET
    /// </remarks>
    [DisplayName("testNotPattern")]
    public abstract bool? TestNotPattern(bool? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testRecursivePattern")]
    public abstract bool? TestRecursivePattern();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2TAmA0Bo2SgmA0Bo2SAmA0BAQEA=
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.ISTYPE 30
    /// 08 : OpCode.JMPIFNOT 03
    /// 0A : OpCode.RET
    /// 0B : OpCode.LDLOC0
    /// 0C : OpCode.ISTYPE 28
    /// 0E : OpCode.JMPIFNOT 03
    /// 10 : OpCode.RET
    /// 11 : OpCode.LDLOC0
    /// 12 : OpCode.ISTYPE 20
    /// 14 : OpCode.JMPIFNOT 03
    /// 16 : OpCode.RET
    /// 17 : OpCode.RET
    /// 18 : OpCode.RET
    /// 19 : OpCode.RET
    /// </remarks>
    [DisplayName("testTypePattern")]
    public abstract void TestTypePattern(object? o1 = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2TAmBQgiBWjZKCYFCCIHaNkoCJcmBBBAaNkgJgQRQGjZIQiXJgQSQAgmBBVAaDo=
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.ISTYPE 30
    /// 08 : OpCode.JMPIFNOT 05
    /// 0A : OpCode.PUSHT
    /// 0B : OpCode.JMP 05
    /// 0D : OpCode.LDLOC0
    /// 0E : OpCode.ISTYPE 28
    /// 10 : OpCode.JMPIFNOT 05
    /// 12 : OpCode.PUSHT
    /// 13 : OpCode.JMP 07
    /// 15 : OpCode.LDLOC0
    /// 16 : OpCode.ISTYPE 28
    /// 18 : OpCode.PUSHT
    /// 19 : OpCode.EQUAL
    /// 1A : OpCode.JMPIFNOT 04
    /// 1C : OpCode.PUSH0
    /// 1D : OpCode.RET
    /// 1E : OpCode.LDLOC0
    /// 1F : OpCode.ISTYPE 20
    /// 21 : OpCode.JMPIFNOT 04
    /// 23 : OpCode.PUSH1
    /// 24 : OpCode.RET
    /// 25 : OpCode.LDLOC0
    /// 26 : OpCode.ISTYPE 21
    /// 28 : OpCode.PUSHT
    /// 29 : OpCode.EQUAL
    /// 2A : OpCode.JMPIFNOT 04
    /// 2C : OpCode.PUSH2
    /// 2D : OpCode.RET
    /// 2E : OpCode.PUSHT
    /// 2F : OpCode.JMPIFNOT 04
    /// 31 : OpCode.PUSH5
    /// 32 : OpCode.RET
    /// 33 : OpCode.LDLOC0
    /// 34 : OpCode.THROW
    /// </remarks>
    [DisplayName("testTypePattern2")]
    public abstract BigInteger? TestTypePattern2(object? t = null);

    #endregion
}
