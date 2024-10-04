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
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSHINT8 D8
    /// 0008 : OpCode.LT
    /// 0009 : OpCode.JMPIFNOT 0C
    /// 000B : OpCode.PUSHDATA1 546F6F206C6F77
    /// 0014 : OpCode.RET
    /// 0015 : OpCode.LDLOC0
    /// 0016 : OpCode.PUSHINT8 D8
    /// 0018 : OpCode.GE
    /// 0019 : OpCode.JMPIF 05
    /// 001B : OpCode.PUSHF
    /// 001C : OpCode.JMP 05
    /// 001E : OpCode.LDLOC0
    /// 001F : OpCode.PUSH0
    /// 0020 : OpCode.LT
    /// 0021 : OpCode.JMPIFNOT 08
    /// 0023 : OpCode.PUSHDATA1 4C6F77
    /// 0028 : OpCode.RET
    /// 0029 : OpCode.LDLOC0
    /// 002A : OpCode.PUSH0
    /// 002B : OpCode.GE
    /// 002C : OpCode.JMPIF 05
    /// 002E : OpCode.PUSHF
    /// 002F : OpCode.JMP 05
    /// 0031 : OpCode.LDLOC0
    /// 0032 : OpCode.PUSH10
    /// 0033 : OpCode.LT
    /// 0034 : OpCode.JMPIFNOT 0F
    /// 0036 : OpCode.PUSHDATA1 41636365707461626C65
    /// 0042 : OpCode.RET
    /// 0043 : OpCode.LDLOC0
    /// 0044 : OpCode.PUSH10
    /// 0045 : OpCode.GE
    /// 0046 : OpCode.JMPIF 05
    /// 0048 : OpCode.PUSHF
    /// 0049 : OpCode.JMP 06
    /// 004B : OpCode.LDLOC0
    /// 004C : OpCode.PUSHINT8 14
    /// 004E : OpCode.LT
    /// 004F : OpCode.JMPIFNOT 09
    /// 0051 : OpCode.PUSHDATA1 48696768
    /// 0057 : OpCode.RET
    /// 0058 : OpCode.LDLOC0
    /// 0059 : OpCode.PUSHINT8 14
    /// 005B : OpCode.GE
    /// 005C : OpCode.JMPIFNOT 0D
    /// 005E : OpCode.PUSHDATA1 546F6F2068696768
    /// 0068 : OpCode.RET
    /// 0069 : OpCode.LDLOC0
    /// 006A : OpCode.THROW
    /// </remarks>
    [DisplayName("classify")]
    public abstract string? Classify(BigInteger? measurement);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSH3
    /// 0007 : OpCode.EQUAL
    /// 0008 : OpCode.JMPIFNOT 05
    /// 000A : OpCode.PUSHT
    /// 000B : OpCode.JMP 05
    /// 000D : OpCode.LDLOC0
    /// 000E : OpCode.PUSH4
    /// 000F : OpCode.EQUAL
    /// 0010 : OpCode.JMPIFNOT 05
    /// 0012 : OpCode.PUSHT
    /// 0013 : OpCode.JMP 05
    /// 0015 : OpCode.LDLOC0
    /// 0016 : OpCode.PUSH5
    /// 0017 : OpCode.EQUAL
    /// 0018 : OpCode.JMPIFNOT 0B
    /// 001A : OpCode.PUSHDATA1 737072696E67
    /// 0022 : OpCode.RET
    /// 0023 : OpCode.LDLOC0
    /// 0024 : OpCode.PUSH6
    /// 0025 : OpCode.EQUAL
    /// 0026 : OpCode.JMPIFNOT 05
    /// 0028 : OpCode.PUSHT
    /// 0029 : OpCode.JMP 05
    /// 002B : OpCode.LDLOC0
    /// 002C : OpCode.PUSH7
    /// 002D : OpCode.EQUAL
    /// 002E : OpCode.JMPIFNOT 05
    /// 0030 : OpCode.PUSHT
    /// 0031 : OpCode.JMP 05
    /// 0033 : OpCode.LDLOC0
    /// 0034 : OpCode.PUSH8
    /// 0035 : OpCode.EQUAL
    /// 0036 : OpCode.JMPIFNOT 0B
    /// 0038 : OpCode.PUSHDATA1 73756D6D6572
    /// 0040 : OpCode.RET
    /// 0041 : OpCode.LDLOC0
    /// 0042 : OpCode.PUSH9
    /// 0043 : OpCode.EQUAL
    /// 0044 : OpCode.JMPIFNOT 05
    /// 0046 : OpCode.PUSHT
    /// 0047 : OpCode.JMP 05
    /// 0049 : OpCode.LDLOC0
    /// 004A : OpCode.PUSH10
    /// 004B : OpCode.EQUAL
    /// 004C : OpCode.JMPIFNOT 05
    /// 004E : OpCode.PUSHT
    /// 004F : OpCode.JMP 05
    /// 0051 : OpCode.LDLOC0
    /// 0052 : OpCode.PUSH11
    /// 0053 : OpCode.EQUAL
    /// 0054 : OpCode.JMPIFNOT 0B
    /// 0056 : OpCode.PUSHDATA1 617574756D6E
    /// 005E : OpCode.RET
    /// 005F : OpCode.LDLOC0
    /// 0060 : OpCode.PUSH12
    /// 0061 : OpCode.EQUAL
    /// 0062 : OpCode.JMPIFNOT 05
    /// 0064 : OpCode.PUSHT
    /// 0065 : OpCode.JMP 05
    /// 0067 : OpCode.LDLOC0
    /// 0068 : OpCode.PUSH1
    /// 0069 : OpCode.EQUAL
    /// 006A : OpCode.JMPIFNOT 05
    /// 006C : OpCode.PUSHT
    /// 006D : OpCode.JMP 05
    /// 006F : OpCode.LDLOC0
    /// 0070 : OpCode.PUSH2
    /// 0071 : OpCode.EQUAL
    /// 0072 : OpCode.JMPIFNOT 0B
    /// 0074 : OpCode.PUSHDATA1 77696E746572
    /// 007C : OpCode.RET
    /// 007D : OpCode.PUSHT
    /// 007E : OpCode.JMPIFNOT 22
    /// 0080 : OpCode.PUSHDATA1 556E6578706563746564206D6F6E74683A20
    /// 0094 : OpCode.LDARG0
    /// 0095 : OpCode.CALLT 0000
    /// 0098 : OpCode.CAT
    /// 0099 : OpCode.PUSHDATA1 2E
    /// 009C : OpCode.CAT
    /// 009D : OpCode.CONVERT 28
    /// 009F : OpCode.THROW
    /// 00A0 : OpCode.LDLOC0
    /// 00A1 : OpCode.THROW
    /// </remarks>
    [DisplayName("getCalendarSeason")]
    public abstract string? GetCalendarSeason(BigInteger? month);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0400
    /// 0003 : OpCode.PUSHDATA1 48656C6C6F2C20576F726C6421
    /// 0012 : OpCode.STLOC0
    /// 0013 : OpCode.LDLOC0
    /// 0014 : OpCode.STLOC1
    /// 0015 : OpCode.LDLOC1
    /// 0016 : OpCode.ISTYPE 28
    /// 0018 : OpCode.LDLOC1
    /// 0019 : OpCode.STLOC2
    /// 001A : OpCode.JMPIFNOT 08
    /// 001C : OpCode.LDLOC2
    /// 001D : OpCode.SYSCALL CFE74796
    /// 0022 : OpCode.PUSHDATA1 48656C6C6F2C20576F726C6421
    /// 0031 : OpCode.STLOC2
    /// 0032 : OpCode.LDLOC2
    /// 0033 : OpCode.STLOC3
    /// 0034 : OpCode.LDLOC3
    /// 0035 : OpCode.ISTYPE 28
    /// 0037 : OpCode.JMPIFNOT 1C
    /// 0039 : OpCode.PUSHDATA1 6772656574696E673220697320737472696E67
    /// 004E : OpCode.SYSCALL CFE74796
    /// 0053 : OpCode.RET
    /// </remarks>
    [DisplayName("testDeclarationPattern")]
    public abstract void TestDeclarationPattern();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSHNULL
    /// 0007 : OpCode.EQUAL
    /// 0008 : OpCode.NOT
    /// 0009 : OpCode.RET
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
    /// 0000 : OpCode.INITSLOT 0102
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.ISTYPE 30
    /// 0008 : OpCode.JMPIFNOT 03
    /// 000A : OpCode.RET
    /// 000B : OpCode.LDLOC0
    /// 000C : OpCode.ISTYPE 28
    /// 000E : OpCode.JMPIFNOT 03
    /// 0010 : OpCode.RET
    /// 0011 : OpCode.LDLOC0
    /// 0012 : OpCode.ISTYPE 20
    /// 0014 : OpCode.JMPIFNOT 03
    /// 0016 : OpCode.RET
    /// 0017 : OpCode.RET
    /// 0018 : OpCode.RET
    /// 0019 : OpCode.RET
    /// </remarks>
    [DisplayName("testTypePattern")]
    public abstract void TestTypePattern(object? o1 = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0102
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.ISTYPE 30
    /// 0008 : OpCode.JMPIFNOT 05
    /// 000A : OpCode.PUSHT
    /// 000B : OpCode.JMP 05
    /// 000D : OpCode.LDLOC0
    /// 000E : OpCode.ISTYPE 28
    /// 0010 : OpCode.JMPIFNOT 05
    /// 0012 : OpCode.PUSHT
    /// 0013 : OpCode.JMP 07
    /// 0015 : OpCode.LDLOC0
    /// 0016 : OpCode.ISTYPE 28
    /// 0018 : OpCode.PUSHT
    /// 0019 : OpCode.EQUAL
    /// 001A : OpCode.JMPIFNOT 04
    /// 001C : OpCode.PUSH0
    /// 001D : OpCode.RET
    /// 001E : OpCode.LDLOC0
    /// 001F : OpCode.ISTYPE 20
    /// 0021 : OpCode.JMPIFNOT 04
    /// 0023 : OpCode.PUSH1
    /// 0024 : OpCode.RET
    /// 0025 : OpCode.LDLOC0
    /// 0026 : OpCode.ISTYPE 21
    /// 0028 : OpCode.PUSHT
    /// 0029 : OpCode.EQUAL
    /// 002A : OpCode.JMPIFNOT 04
    /// 002C : OpCode.PUSH2
    /// 002D : OpCode.RET
    /// 002E : OpCode.PUSHT
    /// 002F : OpCode.JMPIFNOT 04
    /// 0031 : OpCode.PUSH5
    /// 0032 : OpCode.RET
    /// 0033 : OpCode.LDLOC0
    /// 0034 : OpCode.THROW
    /// </remarks>
    [DisplayName("testTypePattern2")]
    public abstract BigInteger? TestTypePattern2(object? t = null);

    #endregion

}
