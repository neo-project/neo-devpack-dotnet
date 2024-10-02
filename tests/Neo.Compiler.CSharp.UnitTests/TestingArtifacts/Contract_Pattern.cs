using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Pattern(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Pattern"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""between"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":596,""safe"":false},{""name"":""between2"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":608,""safe"":false},{""name"":""between3"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":620,""safe"":false},{""name"":""testRecursivePattern"",""parameters"":[],""returntype"":""Boolean"",""offset"":632,""safe"":false},{""name"":""between4"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":644,""safe"":false},{""name"":""testNotPattern"",""parameters"":[{""name"":""x"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":154,""safe"":false},{""name"":""classify"",""parameters"":[{""name"":""measurement"",""type"":""Integer""}],""returntype"":""String"",""offset"":164,""safe"":false},{""name"":""getCalendarSeason"",""parameters"":[{""name"":""month"",""type"":""Integer""}],""returntype"":""String"",""offset"":271,""safe"":false},{""name"":""testDeclarationPattern"",""parameters"":[],""returntype"":""Void"",""offset"":433,""safe"":false},{""name"":""testTypePattern"",""parameters"":[{""name"":""o1"",""type"":""Any""}],""returntype"":""Void"",""offset"":656,""safe"":false},{""name"":""testTypePattern2"",""parameters"":[{""name"":""t"",""type"":""Any""}],""returntype"":""Integer"",""offset"":668,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/akCVwECeXBoEbckBAlAaABktUBXAAF4NANAVwABQFcBAnlwaBG3JAQJQGgAZLVAVwECeXBoEbckBQkiBmgAMrUmBAhAaAAyuCQFCSIGaABktSYECEAIJgQJQGg6VwIBDBQAAAAAAAAAAAAAAAAAAAAAAAAAAHBocWlK2ShQygAUs6sIlxCzCZcmBAhACCYECUBpOlcBAnlwaBC2QFcBAXhwaAuXqkBXAQF4cGgA2LUmDAwHVG9vIGxvd0BoANi4JAUJIgVoELUmCAwDTG93QGgQuCQFCSIFaBq1Jg8MCkFjY2VwdGFibGVAaBq4JAUJIgZoABS1JgkMBEhpZ2hAaAAUuCYNDAhUb28gaGlnaEBoOlcBAXhwaBOXJgUIIgVoFJcmBQgiBWgVlyYLDAZzcHJpbmdAaBaXJgUIIgVoF5cmBQgiBWgYlyYLDAZzdW1tZXJAaBmXJgUIIgVoGpcmBQgiBWgblyYLDAZhdXR1bW5AaByXJgUIIgVoEZcmBQgiBWgSlyYLDAZ3aW50ZXJACCYiDBJVbmV4cGVjdGVkIG1vbnRoOiB4NwAAiwwBLovbKDpoOlcEAAwNSGVsbG8sIFdvcmxkIXBocWnZKGlyJghqQc/nR5YMDUhlbGxvLCBXb3JsZCFyanNr2SgmHAwTZ3JlZXRpbmcyIGlzIHN0cmluZ0HP50eWQFcBAnlwaNkwJgNAaNkoJgNAaNkgJgNAQEBAVwECeXBo2TAmBQgiBWjZKCYFCCIHaNkoCJcmBBBAaNkgJgQRQGjZIQiXJgQSQAgmBBVAaDrCSjW7/f//I6X9///CSjWv/f//I7X9///CSjWj/f//I7r9///CSjWX/f//I9v9///CSjWL/f//Iwb+///CSjV//f//I27////CSjVz/f//I3z///9AAqAueQ=="));

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
    [DisplayName("classify")]
    public abstract string? Classify(BigInteger? measurement);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSHINT8
    // 0008 : LT
    // 0009 : JMPIFNOT
    // 000B : PUSHDATA1
    // 0014 : RET
    // 0015 : LDLOC0
    // 0016 : PUSHINT8
    // 0018 : GE
    // 0019 : JMPIF
    // 001B : PUSHF
    // 001C : JMP
    // 001E : LDLOC0
    // 001F : PUSH0
    // 0020 : LT
    // 0021 : JMPIFNOT
    // 0023 : PUSHDATA1
    // 0028 : RET
    // 0029 : LDLOC0
    // 002A : PUSH0
    // 002B : GE
    // 002C : JMPIF
    // 002E : PUSHF
    // 002F : JMP
    // 0031 : LDLOC0
    // 0032 : PUSH10
    // 0033 : LT
    // 0034 : JMPIFNOT
    // 0036 : PUSHDATA1
    // 0042 : RET
    // 0043 : LDLOC0
    // 0044 : PUSH10
    // 0045 : GE
    // 0046 : JMPIF
    // 0048 : PUSHF
    // 0049 : JMP
    // 004B : LDLOC0
    // 004C : PUSHINT8
    // 004E : LT
    // 004F : JMPIFNOT
    // 0051 : PUSHDATA1
    // 0057 : RET
    // 0058 : LDLOC0
    // 0059 : PUSHINT8
    // 005B : GE
    // 005C : JMPIFNOT
    // 005E : PUSHDATA1
    // 0068 : RET
    // 0069 : LDLOC0
    // 006A : THROW

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getCalendarSeason")]
    public abstract string? GetCalendarSeason(BigInteger? month);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSH3
    // 0007 : EQUAL
    // 0008 : JMPIFNOT
    // 000A : PUSHT
    // 000B : JMP
    // 000D : LDLOC0
    // 000E : PUSH4
    // 000F : EQUAL
    // 0010 : JMPIFNOT
    // 0012 : PUSHT
    // 0013 : JMP
    // 0015 : LDLOC0
    // 0016 : PUSH5
    // 0017 : EQUAL
    // 0018 : JMPIFNOT
    // 001A : PUSHDATA1
    // 0022 : RET
    // 0023 : LDLOC0
    // 0024 : PUSH6
    // 0025 : EQUAL
    // 0026 : JMPIFNOT
    // 0028 : PUSHT
    // 0029 : JMP
    // 002B : LDLOC0
    // 002C : PUSH7
    // 002D : EQUAL
    // 002E : JMPIFNOT
    // 0030 : PUSHT
    // 0031 : JMP
    // 0033 : LDLOC0
    // 0034 : PUSH8
    // 0035 : EQUAL
    // 0036 : JMPIFNOT
    // 0038 : PUSHDATA1
    // 0040 : RET
    // 0041 : LDLOC0
    // 0042 : PUSH9
    // 0043 : EQUAL
    // 0044 : JMPIFNOT
    // 0046 : PUSHT
    // 0047 : JMP
    // 0049 : LDLOC0
    // 004A : PUSH10
    // 004B : EQUAL
    // 004C : JMPIFNOT
    // 004E : PUSHT
    // 004F : JMP
    // 0051 : LDLOC0
    // 0052 : PUSH11
    // 0053 : EQUAL
    // 0054 : JMPIFNOT
    // 0056 : PUSHDATA1
    // 005E : RET
    // 005F : LDLOC0
    // 0060 : PUSH12
    // 0061 : EQUAL
    // 0062 : JMPIFNOT
    // 0064 : PUSHT
    // 0065 : JMP
    // 0067 : LDLOC0
    // 0068 : PUSH1
    // 0069 : EQUAL
    // 006A : JMPIFNOT
    // 006C : PUSHT
    // 006D : JMP
    // 006F : LDLOC0
    // 0070 : PUSH2
    // 0071 : EQUAL
    // 0072 : JMPIFNOT
    // 0074 : PUSHDATA1
    // 007C : RET
    // 007D : PUSHT
    // 007E : JMPIFNOT
    // 0080 : PUSHDATA1
    // 0094 : LDARG0
    // 0095 : CALLT
    // 0098 : CAT
    // 0099 : PUSHDATA1
    // 009C : CAT
    // 009D : CONVERT
    // 009F : THROW
    // 00A0 : LDLOC0
    // 00A1 : THROW

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDeclarationPattern")]
    public abstract void TestDeclarationPattern();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0012 : STLOC0
    // 0013 : LDLOC0
    // 0014 : STLOC1
    // 0015 : LDLOC1
    // 0016 : ISTYPE
    // 0018 : LDLOC1
    // 0019 : STLOC2
    // 001A : JMPIFNOT
    // 001C : LDLOC2
    // 001D : SYSCALL
    // 0022 : PUSHDATA1
    // 0031 : STLOC2
    // 0032 : LDLOC2
    // 0033 : STLOC3
    // 0034 : LDLOC3
    // 0035 : ISTYPE
    // 0037 : JMPIFNOT
    // 0039 : PUSHDATA1
    // 004E : SYSCALL
    // 0053 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testNotPattern")]
    public abstract bool? TestNotPattern(bool? x);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSHNULL
    // 0007 : EQUAL
    // 0008 : NOT
    // 0009 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testRecursivePattern")]
    public abstract bool? TestRecursivePattern();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testTypePattern")]
    public abstract void TestTypePattern(object? o1 = null);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : ISTYPE
    // 0008 : JMPIFNOT
    // 000A : RET
    // 000B : LDLOC0
    // 000C : ISTYPE
    // 000E : JMPIFNOT
    // 0010 : RET
    // 0011 : LDLOC0
    // 0012 : ISTYPE
    // 0014 : JMPIFNOT
    // 0016 : RET
    // 0017 : RET
    // 0018 : RET
    // 0019 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testTypePattern2")]
    public abstract BigInteger? TestTypePattern2(object? t = null);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : ISTYPE
    // 0008 : JMPIFNOT
    // 000A : PUSHT
    // 000B : JMP
    // 000D : LDLOC0
    // 000E : ISTYPE
    // 0010 : JMPIFNOT
    // 0012 : PUSHT
    // 0013 : JMP
    // 0015 : LDLOC0
    // 0016 : ISTYPE
    // 0018 : PUSHT
    // 0019 : EQUAL
    // 001A : JMPIFNOT
    // 001C : PUSH0
    // 001D : RET
    // 001E : LDLOC0
    // 001F : ISTYPE
    // 0021 : JMPIFNOT
    // 0023 : PUSH1
    // 0024 : RET
    // 0025 : LDLOC0
    // 0026 : ISTYPE
    // 0028 : PUSHT
    // 0029 : EQUAL
    // 002A : JMPIFNOT
    // 002C : PUSH2
    // 002D : RET
    // 002E : PUSHT
    // 002F : JMPIFNOT
    // 0031 : PUSH5
    // 0032 : RET
    // 0033 : LDLOC0
    // 0034 : THROW

    #endregion

}
