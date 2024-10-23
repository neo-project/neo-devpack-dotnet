using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Pattern(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Pattern"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""between"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""between2"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":17,""safe"":false},{""name"":""between3"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":34,""safe"":false},{""name"":""testRecursivePattern"",""parameters"":[],""returntype"":""Boolean"",""offset"":79,""safe"":false},{""name"":""between4"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":134,""safe"":false},{""name"":""testNotPattern"",""parameters"":[{""name"":""x"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":143,""safe"":false},{""name"":""classify"",""parameters"":[{""name"":""measurement"",""type"":""Integer""}],""returntype"":""String"",""offset"":153,""safe"":false},{""name"":""getCalendarSeason"",""parameters"":[{""name"":""month"",""type"":""Integer""}],""returntype"":""String"",""offset"":260,""safe"":false},{""name"":""testDeclarationPattern"",""parameters"":[],""returntype"":""Void"",""offset"":422,""safe"":false},{""name"":""testTypePattern"",""parameters"":[{""name"":""o1"",""type"":""Any""}],""returntype"":""Void"",""offset"":506,""safe"":false},{""name"":""testTypePattern2"",""parameters"":[{""name"":""t"",""type"":""Any""}],""returntype"":""Integer"",""offset"":532,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/UoCVwEBeHBoEbckBAlAaABktUBXAQF4cGgRtyQECUBoAGS1QFcBAXhwaBG3JAUJIgZoADK1JgQIQGgAMrgkBQkiBmgAZLUmBAhACCYECUBoOlcCAAwUAAAAAAAAAAAAAAAAAAAAAAAAAABwaHFpStkoUMoAFLOrCJcQswmXJgQIQAgmBAlAaTpXAQF4cGgQtkBXAQF4cGgLl6pAVwEBeHBoANi1JgwMB1RvbyBsb3dAaADYuCQFCSIFaBC1JggMA0xvd0BoELgkBQkiBWgatSYPDApBY2NlcHRhYmxlQGgauCQFCSIGaAAUtSYJDARIaWdoQGgAFLgmDQwIVG9vIGhpZ2hAaDpXAQF4cGgTlyYFCCIFaBSXJgUIIgVoFZcmCwwGc3ByaW5nQGgWlyYFCCIFaBeXJgUIIgVoGJcmCwwGc3VtbWVyQGgZlyYFCCIFaBqXJgUIIgVoG5cmCwwGYXV0dW1uQGgclyYFCCIFaBGXJgUIIgVoEpcmCwwGd2ludGVyQAgmIgwSVW5leHBlY3RlZCBtb250aDogeDcAAIsMAS6L2yg6aDpXBAAMDUhlbGxvLCBXb3JsZCFwaHFp2ShpciYIakHP50eWDA1IZWxsbywgV29ybGQhcmpza9koJhwME2dyZWV0aW5nMiBpcyBzdHJpbmdBz+dHlkBXAQF4cGjZMCYDQGjZKCYDQGjZICYDQEBAQFcBAXhwaNkwJgUIIgVo2SgmBQgiB2jZKAiXJgQQQGjZICYEEUBo2SEIlyYEEkAIJgQVQGg6QGU5aS0="));

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
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.STLOC0	[2 datoshi]
    /// 05 : OpCode.LDLOC0	[2 datoshi]
    /// 06 : OpCode.PUSHINT8 D8	[1 datoshi]
    /// 08 : OpCode.LT	[8 datoshi]
    /// 09 : OpCode.JMPIFNOT 0C	[2 datoshi]
    /// 0B : OpCode.PUSHDATA1 546F6F206C6F77	[8 datoshi]
    /// 14 : OpCode.RET	[0 datoshi]
    /// 15 : OpCode.LDLOC0	[2 datoshi]
    /// 16 : OpCode.PUSHINT8 D8	[1 datoshi]
    /// 18 : OpCode.GE	[8 datoshi]
    /// 19 : OpCode.JMPIF 05	[2 datoshi]
    /// 1B : OpCode.PUSHF	[1 datoshi]
    /// 1C : OpCode.JMP 05	[2 datoshi]
    /// 1E : OpCode.LDLOC0	[2 datoshi]
    /// 1F : OpCode.PUSH0	[1 datoshi]
    /// 20 : OpCode.LT	[8 datoshi]
    /// 21 : OpCode.JMPIFNOT 08	[2 datoshi]
    /// 23 : OpCode.PUSHDATA1 4C6F77	[8 datoshi]
    /// 28 : OpCode.RET	[0 datoshi]
    /// 29 : OpCode.LDLOC0	[2 datoshi]
    /// 2A : OpCode.PUSH0	[1 datoshi]
    /// 2B : OpCode.GE	[8 datoshi]
    /// 2C : OpCode.JMPIF 05	[2 datoshi]
    /// 2E : OpCode.PUSHF	[1 datoshi]
    /// 2F : OpCode.JMP 05	[2 datoshi]
    /// 31 : OpCode.LDLOC0	[2 datoshi]
    /// 32 : OpCode.PUSH10	[1 datoshi]
    /// 33 : OpCode.LT	[8 datoshi]
    /// 34 : OpCode.JMPIFNOT 0F	[2 datoshi]
    /// 36 : OpCode.PUSHDATA1 41636365707461626C65	[8 datoshi]
    /// 42 : OpCode.RET	[0 datoshi]
    /// 43 : OpCode.LDLOC0	[2 datoshi]
    /// 44 : OpCode.PUSH10	[1 datoshi]
    /// 45 : OpCode.GE	[8 datoshi]
    /// 46 : OpCode.JMPIF 05	[2 datoshi]
    /// 48 : OpCode.PUSHF	[1 datoshi]
    /// 49 : OpCode.JMP 06	[2 datoshi]
    /// 4B : OpCode.LDLOC0	[2 datoshi]
    /// 4C : OpCode.PUSHINT8 14	[1 datoshi]
    /// 4E : OpCode.LT	[8 datoshi]
    /// 4F : OpCode.JMPIFNOT 09	[2 datoshi]
    /// 51 : OpCode.PUSHDATA1 48696768	[8 datoshi]
    /// 57 : OpCode.RET	[0 datoshi]
    /// 58 : OpCode.LDLOC0	[2 datoshi]
    /// 59 : OpCode.PUSHINT8 14	[1 datoshi]
    /// 5B : OpCode.GE	[8 datoshi]
    /// 5C : OpCode.JMPIFNOT 0D	[2 datoshi]
    /// 5E : OpCode.PUSHDATA1 546F6F2068696768	[8 datoshi]
    /// 68 : OpCode.RET	[0 datoshi]
    /// 69 : OpCode.LDLOC0	[2 datoshi]
    /// 6A : OpCode.THROW	[512 datoshi]
    /// </remarks>
    [DisplayName("classify")]
    public abstract string? Classify(BigInteger? measurement);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoE5cmBQgiBWgUlyYFCCIFaBWXJgsMc3ByaW5nQGgWlyYFCCIFaBeXJgUIIgVoGJcmCwxzdW1tZXJAaBmXJgUIIgVoGpcmBQgiBWgblyYLDGF1dHVtbkBoHJcmBQgiBWgRlyYFCCIFaBKXJgsMd2ludGVyQAgmIgxVbmV4cGVjdGVkIG1vbnRoOiB4NwAAiwwui9soOmg6
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.STLOC0	[2 datoshi]
    /// 05 : OpCode.LDLOC0	[2 datoshi]
    /// 06 : OpCode.PUSH3	[1 datoshi]
    /// 07 : OpCode.EQUAL	[32 datoshi]
    /// 08 : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 0A : OpCode.PUSHT	[1 datoshi]
    /// 0B : OpCode.JMP 05	[2 datoshi]
    /// 0D : OpCode.LDLOC0	[2 datoshi]
    /// 0E : OpCode.PUSH4	[1 datoshi]
    /// 0F : OpCode.EQUAL	[32 datoshi]
    /// 10 : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 12 : OpCode.PUSHT	[1 datoshi]
    /// 13 : OpCode.JMP 05	[2 datoshi]
    /// 15 : OpCode.LDLOC0	[2 datoshi]
    /// 16 : OpCode.PUSH5	[1 datoshi]
    /// 17 : OpCode.EQUAL	[32 datoshi]
    /// 18 : OpCode.JMPIFNOT 0B	[2 datoshi]
    /// 1A : OpCode.PUSHDATA1 737072696E67	[8 datoshi]
    /// 22 : OpCode.RET	[0 datoshi]
    /// 23 : OpCode.LDLOC0	[2 datoshi]
    /// 24 : OpCode.PUSH6	[1 datoshi]
    /// 25 : OpCode.EQUAL	[32 datoshi]
    /// 26 : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 28 : OpCode.PUSHT	[1 datoshi]
    /// 29 : OpCode.JMP 05	[2 datoshi]
    /// 2B : OpCode.LDLOC0	[2 datoshi]
    /// 2C : OpCode.PUSH7	[1 datoshi]
    /// 2D : OpCode.EQUAL	[32 datoshi]
    /// 2E : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 30 : OpCode.PUSHT	[1 datoshi]
    /// 31 : OpCode.JMP 05	[2 datoshi]
    /// 33 : OpCode.LDLOC0	[2 datoshi]
    /// 34 : OpCode.PUSH8	[1 datoshi]
    /// 35 : OpCode.EQUAL	[32 datoshi]
    /// 36 : OpCode.JMPIFNOT 0B	[2 datoshi]
    /// 38 : OpCode.PUSHDATA1 73756D6D6572	[8 datoshi]
    /// 40 : OpCode.RET	[0 datoshi]
    /// 41 : OpCode.LDLOC0	[2 datoshi]
    /// 42 : OpCode.PUSH9	[1 datoshi]
    /// 43 : OpCode.EQUAL	[32 datoshi]
    /// 44 : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 46 : OpCode.PUSHT	[1 datoshi]
    /// 47 : OpCode.JMP 05	[2 datoshi]
    /// 49 : OpCode.LDLOC0	[2 datoshi]
    /// 4A : OpCode.PUSH10	[1 datoshi]
    /// 4B : OpCode.EQUAL	[32 datoshi]
    /// 4C : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 4E : OpCode.PUSHT	[1 datoshi]
    /// 4F : OpCode.JMP 05	[2 datoshi]
    /// 51 : OpCode.LDLOC0	[2 datoshi]
    /// 52 : OpCode.PUSH11	[1 datoshi]
    /// 53 : OpCode.EQUAL	[32 datoshi]
    /// 54 : OpCode.JMPIFNOT 0B	[2 datoshi]
    /// 56 : OpCode.PUSHDATA1 617574756D6E	[8 datoshi]
    /// 5E : OpCode.RET	[0 datoshi]
    /// 5F : OpCode.LDLOC0	[2 datoshi]
    /// 60 : OpCode.PUSH12	[1 datoshi]
    /// 61 : OpCode.EQUAL	[32 datoshi]
    /// 62 : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 64 : OpCode.PUSHT	[1 datoshi]
    /// 65 : OpCode.JMP 05	[2 datoshi]
    /// 67 : OpCode.LDLOC0	[2 datoshi]
    /// 68 : OpCode.PUSH1	[1 datoshi]
    /// 69 : OpCode.EQUAL	[32 datoshi]
    /// 6A : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 6C : OpCode.PUSHT	[1 datoshi]
    /// 6D : OpCode.JMP 05	[2 datoshi]
    /// 6F : OpCode.LDLOC0	[2 datoshi]
    /// 70 : OpCode.PUSH2	[1 datoshi]
    /// 71 : OpCode.EQUAL	[32 datoshi]
    /// 72 : OpCode.JMPIFNOT 0B	[2 datoshi]
    /// 74 : OpCode.PUSHDATA1 77696E746572	[8 datoshi]
    /// 7C : OpCode.RET	[0 datoshi]
    /// 7D : OpCode.PUSHT	[1 datoshi]
    /// 7E : OpCode.JMPIFNOT 22	[2 datoshi]
    /// 80 : OpCode.PUSHDATA1 556E6578706563746564206D6F6E74683A20	[8 datoshi]
    /// 94 : OpCode.LDARG0	[2 datoshi]
    /// 95 : OpCode.CALLT 0000	[32768 datoshi]
    /// 98 : OpCode.CAT	[2048 datoshi]
    /// 99 : OpCode.PUSHDATA1 2E	[8 datoshi]
    /// 9C : OpCode.CAT	[2048 datoshi]
    /// 9D : OpCode.CONVERT 28	[8192 datoshi]
    /// 9F : OpCode.THROW	[512 datoshi]
    /// A0 : OpCode.LDLOC0	[2 datoshi]
    /// A1 : OpCode.THROW	[512 datoshi]
    /// </remarks>
    [DisplayName("getCalendarSeason")]
    public abstract string? GetCalendarSeason(BigInteger? month);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQADEhlbGxvLCBXb3JsZCFwaHFp2ShpciYIakHP50eWDEhlbGxvLCBXb3JsZCFyanNr2SgmHAxncmVldGluZzIgaXMgc3RyaW5nQc/nR5ZA
    /// 00 : OpCode.INITSLOT 0400	[64 datoshi]
    /// 03 : OpCode.PUSHDATA1 48656C6C6F2C20576F726C6421	[8 datoshi]
    /// 12 : OpCode.STLOC0	[2 datoshi]
    /// 13 : OpCode.LDLOC0	[2 datoshi]
    /// 14 : OpCode.STLOC1	[2 datoshi]
    /// 15 : OpCode.LDLOC1	[2 datoshi]
    /// 16 : OpCode.ISTYPE 28	[2 datoshi]
    /// 18 : OpCode.LDLOC1	[2 datoshi]
    /// 19 : OpCode.STLOC2	[2 datoshi]
    /// 1A : OpCode.JMPIFNOT 08	[2 datoshi]
    /// 1C : OpCode.LDLOC2	[2 datoshi]
    /// 1D : OpCode.SYSCALL CFE74796	[System.Runtime.Log][32768 datoshi]
    /// 22 : OpCode.PUSHDATA1 48656C6C6F2C20576F726C6421	[8 datoshi]
    /// 31 : OpCode.STLOC2	[2 datoshi]
    /// 32 : OpCode.LDLOC2	[2 datoshi]
    /// 33 : OpCode.STLOC3	[2 datoshi]
    /// 34 : OpCode.LDLOC3	[2 datoshi]
    /// 35 : OpCode.ISTYPE 28	[2 datoshi]
    /// 37 : OpCode.JMPIFNOT 1C	[2 datoshi]
    /// 39 : OpCode.PUSHDATA1 6772656574696E673220697320737472696E67	[8 datoshi]
    /// 4E : OpCode.SYSCALL CFE74796	[System.Runtime.Log][32768 datoshi]
    /// 53 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testDeclarationPattern")]
    public abstract void TestDeclarationPattern();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoC5eqQA==
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.STLOC0	[2 datoshi]
    /// 05 : OpCode.LDLOC0	[2 datoshi]
    /// 06 : OpCode.PUSHNULL	[1 datoshi]
    /// 07 : OpCode.EQUAL	[32 datoshi]
    /// 08 : OpCode.NOT	[4 datoshi]
    /// 09 : OpCode.RET	[0 datoshi]
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
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.STLOC0	[2 datoshi]
    /// 05 : OpCode.LDLOC0	[2 datoshi]
    /// 06 : OpCode.ISTYPE 30	[2 datoshi]
    /// 08 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 0A : OpCode.RET	[0 datoshi]
    /// 0B : OpCode.LDLOC0	[2 datoshi]
    /// 0C : OpCode.ISTYPE 28	[2 datoshi]
    /// 0E : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 10 : OpCode.RET	[0 datoshi]
    /// 11 : OpCode.LDLOC0	[2 datoshi]
    /// 12 : OpCode.ISTYPE 20	[2 datoshi]
    /// 14 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 16 : OpCode.RET	[0 datoshi]
    /// 17 : OpCode.RET	[0 datoshi]
    /// 18 : OpCode.RET	[0 datoshi]
    /// 19 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testTypePattern")]
    public abstract void TestTypePattern(object? o1 = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2TAmBQgiBWjZKCYFCCIHaNkoCJcmBBBAaNkgJgQRQGjZIQiXJgQSQAgmBBVAaDo=
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.STLOC0	[2 datoshi]
    /// 05 : OpCode.LDLOC0	[2 datoshi]
    /// 06 : OpCode.ISTYPE 30	[2 datoshi]
    /// 08 : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 0A : OpCode.PUSHT	[1 datoshi]
    /// 0B : OpCode.JMP 05	[2 datoshi]
    /// 0D : OpCode.LDLOC0	[2 datoshi]
    /// 0E : OpCode.ISTYPE 28	[2 datoshi]
    /// 10 : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 12 : OpCode.PUSHT	[1 datoshi]
    /// 13 : OpCode.JMP 07	[2 datoshi]
    /// 15 : OpCode.LDLOC0	[2 datoshi]
    /// 16 : OpCode.ISTYPE 28	[2 datoshi]
    /// 18 : OpCode.PUSHT	[1 datoshi]
    /// 19 : OpCode.EQUAL	[32 datoshi]
    /// 1A : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 1C : OpCode.PUSH0	[1 datoshi]
    /// 1D : OpCode.RET	[0 datoshi]
    /// 1E : OpCode.LDLOC0	[2 datoshi]
    /// 1F : OpCode.ISTYPE 20	[2 datoshi]
    /// 21 : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 23 : OpCode.PUSH1	[1 datoshi]
    /// 24 : OpCode.RET	[0 datoshi]
    /// 25 : OpCode.LDLOC0	[2 datoshi]
    /// 26 : OpCode.ISTYPE 21	[2 datoshi]
    /// 28 : OpCode.PUSHT	[1 datoshi]
    /// 29 : OpCode.EQUAL	[32 datoshi]
    /// 2A : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 2C : OpCode.PUSH2	[1 datoshi]
    /// 2D : OpCode.RET	[0 datoshi]
    /// 2E : OpCode.PUSHT	[1 datoshi]
    /// 2F : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 31 : OpCode.PUSH5	[1 datoshi]
    /// 32 : OpCode.RET	[0 datoshi]
    /// 33 : OpCode.LDLOC0	[2 datoshi]
    /// 34 : OpCode.THROW	[512 datoshi]
    /// </remarks>
    [DisplayName("testTypePattern2")]
    public abstract BigInteger? TestTypePattern2(object? t = null);

    #endregion
}
