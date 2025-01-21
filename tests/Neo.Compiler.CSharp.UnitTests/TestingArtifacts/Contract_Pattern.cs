using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Pattern(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Pattern"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""between"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""between2"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":17,""safe"":false},{""name"":""between3"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":34,""safe"":false},{""name"":""testRecursivePattern"",""parameters"":[],""returntype"":""Boolean"",""offset"":79,""safe"":false},{""name"":""between4"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":138,""safe"":false},{""name"":""testNotPattern"",""parameters"":[{""name"":""x"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":147,""safe"":false},{""name"":""classify"",""parameters"":[{""name"":""measurement"",""type"":""Integer""}],""returntype"":""String"",""offset"":156,""safe"":false},{""name"":""getCalendarSeason"",""parameters"":[{""name"":""month"",""type"":""Integer""}],""returntype"":""String"",""offset"":263,""safe"":false},{""name"":""testDeclarationPattern"",""parameters"":[],""returntype"":""Void"",""offset"":425,""safe"":false},{""name"":""testTypePattern"",""parameters"":[{""name"":""o1"",""type"":""Any""}],""returntype"":""Void"",""offset"":509,""safe"":false},{""name"":""testTypePattern2"",""parameters"":[{""name"":""t"",""type"":""Any""}],""returntype"":""Integer"",""offset"":531,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/UkCVwEBeHBoEbckBAlAaABktUBXAQF4cGgRtyQECUBoAGS1QFcBAXhwaBG3JAUJIgZoADK1JgQIQGgAMrgkBQkiBmgAZLUmBAhACCYECUBoOlcCAAwUAAAAAAAAAAAAAAAAAAAAAAAAAABwaHFpStkoJAZFCSIGygAUswiXsaoJlyYECEAIJgQJQGk6VwEBeHBoELZAVwEBeHBo2KpAVwEBeHBoANi1JgwMB1RvbyBsb3dAaADYuCQFCSIFaBC1JggMA0xvd0BoELgkBQkiBWgatSYPDApBY2NlcHRhYmxlQGgauCQFCSIGaAAUtSYJDARIaWdoQGgAFLgmDQwIVG9vIGhpZ2hAaDpXAQF4cGgTlyYFCCIFaBSXJgUIIgVoFZcmCwwGc3ByaW5nQGgWlyYFCCIFaBeXJgUIIgVoGJcmCwwGc3VtbWVyQGgZlyYFCCIFaBqXJgUIIgVoG5cmCwwGYXV0dW1uQGgclyYFCCIFaBGXJgUIIgVoEpcmCwwGd2ludGVyQAgmIgwSVW5leHBlY3RlZCBtb250aDogeDcAAIsMAS6L2yg6aDpXBAAMDUhlbGxvLCBXb3JsZCFwaHFp2ShpciYIakHP50eWDA1IZWxsbywgV29ybGQhcmpza9koJhwME2dyZWV0aW5nMiBpcyBzdHJpbmdBz+dHlkBXAQF4cGjZMCYDQGjZKCYDQGjZIEVAVwEBeHBo2TAmBQgiBWjZKCYFCCIHaNkoCJcmBBBAaNkgJgQRQGjZIQiXJgQSQAgmBBVAaDpAtOcsgw==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoEbckBAlAaABktUA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : PUSH1 [1 datoshi]
    /// 07 : GT [8 datoshi]
    /// 08 : JMPIF 04 [2 datoshi]
    /// 0A : PUSHF [1 datoshi]
    /// 0B : RET [0 datoshi]
    /// 0C : LDLOC0 [2 datoshi]
    /// 0D : PUSHINT8 64 [1 datoshi]
    /// 0F : LT [8 datoshi]
    /// 10 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("between")]
    public abstract bool? Between(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoEbckBAlAaABktUA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : PUSH1 [1 datoshi]
    /// 07 : GT [8 datoshi]
    /// 08 : JMPIF 04 [2 datoshi]
    /// 0A : PUSHF [1 datoshi]
    /// 0B : RET [0 datoshi]
    /// 0C : LDLOC0 [2 datoshi]
    /// 0D : PUSHINT8 64 [1 datoshi]
    /// 0F : LT [8 datoshi]
    /// 10 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("between2")]
    public abstract bool? Between2(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoEbckBQkiBmgAMrUmBAhAaAAyuCQFCSIGaABktSYECEAIJgQJQGg6
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : PUSH1 [1 datoshi]
    /// 07 : GT [8 datoshi]
    /// 08 : JMPIF 05 [2 datoshi]
    /// 0A : PUSHF [1 datoshi]
    /// 0B : JMP 06 [2 datoshi]
    /// 0D : LDLOC0 [2 datoshi]
    /// 0E : PUSHINT8 32 [1 datoshi]
    /// 10 : LT [8 datoshi]
    /// 11 : JMPIFNOT 04 [2 datoshi]
    /// 13 : PUSHT [1 datoshi]
    /// 14 : RET [0 datoshi]
    /// 15 : LDLOC0 [2 datoshi]
    /// 16 : PUSHINT8 32 [1 datoshi]
    /// 18 : GE [8 datoshi]
    /// 19 : JMPIF 05 [2 datoshi]
    /// 1B : PUSHF [1 datoshi]
    /// 1C : JMP 06 [2 datoshi]
    /// 1E : LDLOC0 [2 datoshi]
    /// 1F : PUSHINT8 64 [1 datoshi]
    /// 21 : LT [8 datoshi]
    /// 22 : JMPIFNOT 04 [2 datoshi]
    /// 24 : PUSHT [1 datoshi]
    /// 25 : RET [0 datoshi]
    /// 26 : PUSHT [1 datoshi]
    /// 27 : JMPIFNOT 04 [2 datoshi]
    /// 29 : PUSHF [1 datoshi]
    /// 2A : RET [0 datoshi]
    /// 2B : LDLOC0 [2 datoshi]
    /// 2C : THROW [512 datoshi]
    /// </remarks>
    [DisplayName("between3")]
    public abstract bool? Between3(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoELZA
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : PUSH0 [1 datoshi]
    /// 07 : LE [8 datoshi]
    /// 08 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("between4")]
    public abstract bool? Between4(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoANi1JgwMB1RvbyBsb3dAaADYuCQFCSIFaBC1JggMA0xvd0BoELgkBQkiBWgatSYPDApBY2NlcHRhYmxlQGgauCQFCSIGaAAUtSYJDARIaWdoQGgAFLgmDQwIVG9vIGhpZ2hAaDo=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : PUSHINT8 D8 [1 datoshi]
    /// 08 : LT [8 datoshi]
    /// 09 : JMPIFNOT 0C [2 datoshi]
    /// 0B : PUSHDATA1 546F6F206C6F77 [8 datoshi]
    /// 14 : RET [0 datoshi]
    /// 15 : LDLOC0 [2 datoshi]
    /// 16 : PUSHINT8 D8 [1 datoshi]
    /// 18 : GE [8 datoshi]
    /// 19 : JMPIF 05 [2 datoshi]
    /// 1B : PUSHF [1 datoshi]
    /// 1C : JMP 05 [2 datoshi]
    /// 1E : LDLOC0 [2 datoshi]
    /// 1F : PUSH0 [1 datoshi]
    /// 20 : LT [8 datoshi]
    /// 21 : JMPIFNOT 08 [2 datoshi]
    /// 23 : PUSHDATA1 4C6F77 'Low' [8 datoshi]
    /// 28 : RET [0 datoshi]
    /// 29 : LDLOC0 [2 datoshi]
    /// 2A : PUSH0 [1 datoshi]
    /// 2B : GE [8 datoshi]
    /// 2C : JMPIF 05 [2 datoshi]
    /// 2E : PUSHF [1 datoshi]
    /// 2F : JMP 05 [2 datoshi]
    /// 31 : LDLOC0 [2 datoshi]
    /// 32 : PUSH10 [1 datoshi]
    /// 33 : LT [8 datoshi]
    /// 34 : JMPIFNOT 0F [2 datoshi]
    /// 36 : PUSHDATA1 41636365707461626C65 'Acceptable' [8 datoshi]
    /// 42 : RET [0 datoshi]
    /// 43 : LDLOC0 [2 datoshi]
    /// 44 : PUSH10 [1 datoshi]
    /// 45 : GE [8 datoshi]
    /// 46 : JMPIF 05 [2 datoshi]
    /// 48 : PUSHF [1 datoshi]
    /// 49 : JMP 06 [2 datoshi]
    /// 4B : LDLOC0 [2 datoshi]
    /// 4C : PUSHINT8 14 [1 datoshi]
    /// 4E : LT [8 datoshi]
    /// 4F : JMPIFNOT 09 [2 datoshi]
    /// 51 : PUSHDATA1 48696768 'High' [8 datoshi]
    /// 57 : RET [0 datoshi]
    /// 58 : LDLOC0 [2 datoshi]
    /// 59 : PUSHINT8 14 [1 datoshi]
    /// 5B : GE [8 datoshi]
    /// 5C : JMPIFNOT 0D [2 datoshi]
    /// 5E : PUSHDATA1 546F6F2068696768 [8 datoshi]
    /// 68 : RET [0 datoshi]
    /// 69 : LDLOC0 [2 datoshi]
    /// 6A : THROW [512 datoshi]
    /// </remarks>
    [DisplayName("classify")]
    public abstract string? Classify(BigInteger? measurement);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoE5cmBQgiBWgUlyYFCCIFaBWXJgsMBnNwcmluZ0BoFpcmBQgiBWgXlyYFCCIFaBiXJgsMBnN1bW1lckBoGZcmBQgiBWgalyYFCCIFaBuXJgsMBmF1dHVtbkBoHJcmBQgiBWgRlyYFCCIFaBKXJgsMBndpbnRlckAIJiIMElVuZXhwZWN0ZWQgbW9udGg6IHg3AACLDAEui9soOmg6
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : PUSH3 [1 datoshi]
    /// 07 : EQUAL [32 datoshi]
    /// 08 : JMPIFNOT 05 [2 datoshi]
    /// 0A : PUSHT [1 datoshi]
    /// 0B : JMP 05 [2 datoshi]
    /// 0D : LDLOC0 [2 datoshi]
    /// 0E : PUSH4 [1 datoshi]
    /// 0F : EQUAL [32 datoshi]
    /// 10 : JMPIFNOT 05 [2 datoshi]
    /// 12 : PUSHT [1 datoshi]
    /// 13 : JMP 05 [2 datoshi]
    /// 15 : LDLOC0 [2 datoshi]
    /// 16 : PUSH5 [1 datoshi]
    /// 17 : EQUAL [32 datoshi]
    /// 18 : JMPIFNOT 0B [2 datoshi]
    /// 1A : PUSHDATA1 737072696E67 'spring' [8 datoshi]
    /// 22 : RET [0 datoshi]
    /// 23 : LDLOC0 [2 datoshi]
    /// 24 : PUSH6 [1 datoshi]
    /// 25 : EQUAL [32 datoshi]
    /// 26 : JMPIFNOT 05 [2 datoshi]
    /// 28 : PUSHT [1 datoshi]
    /// 29 : JMP 05 [2 datoshi]
    /// 2B : LDLOC0 [2 datoshi]
    /// 2C : PUSH7 [1 datoshi]
    /// 2D : EQUAL [32 datoshi]
    /// 2E : JMPIFNOT 05 [2 datoshi]
    /// 30 : PUSHT [1 datoshi]
    /// 31 : JMP 05 [2 datoshi]
    /// 33 : LDLOC0 [2 datoshi]
    /// 34 : PUSH8 [1 datoshi]
    /// 35 : EQUAL [32 datoshi]
    /// 36 : JMPIFNOT 0B [2 datoshi]
    /// 38 : PUSHDATA1 73756D6D6572 'summer' [8 datoshi]
    /// 40 : RET [0 datoshi]
    /// 41 : LDLOC0 [2 datoshi]
    /// 42 : PUSH9 [1 datoshi]
    /// 43 : EQUAL [32 datoshi]
    /// 44 : JMPIFNOT 05 [2 datoshi]
    /// 46 : PUSHT [1 datoshi]
    /// 47 : JMP 05 [2 datoshi]
    /// 49 : LDLOC0 [2 datoshi]
    /// 4A : PUSH10 [1 datoshi]
    /// 4B : EQUAL [32 datoshi]
    /// 4C : JMPIFNOT 05 [2 datoshi]
    /// 4E : PUSHT [1 datoshi]
    /// 4F : JMP 05 [2 datoshi]
    /// 51 : LDLOC0 [2 datoshi]
    /// 52 : PUSH11 [1 datoshi]
    /// 53 : EQUAL [32 datoshi]
    /// 54 : JMPIFNOT 0B [2 datoshi]
    /// 56 : PUSHDATA1 617574756D6E 'autumn' [8 datoshi]
    /// 5E : RET [0 datoshi]
    /// 5F : LDLOC0 [2 datoshi]
    /// 60 : PUSH12 [1 datoshi]
    /// 61 : EQUAL [32 datoshi]
    /// 62 : JMPIFNOT 05 [2 datoshi]
    /// 64 : PUSHT [1 datoshi]
    /// 65 : JMP 05 [2 datoshi]
    /// 67 : LDLOC0 [2 datoshi]
    /// 68 : PUSH1 [1 datoshi]
    /// 69 : EQUAL [32 datoshi]
    /// 6A : JMPIFNOT 05 [2 datoshi]
    /// 6C : PUSHT [1 datoshi]
    /// 6D : JMP 05 [2 datoshi]
    /// 6F : LDLOC0 [2 datoshi]
    /// 70 : PUSH2 [1 datoshi]
    /// 71 : EQUAL [32 datoshi]
    /// 72 : JMPIFNOT 0B [2 datoshi]
    /// 74 : PUSHDATA1 77696E746572 'winter' [8 datoshi]
    /// 7C : RET [0 datoshi]
    /// 7D : PUSHT [1 datoshi]
    /// 7E : JMPIFNOT 22 [2 datoshi]
    /// 80 : PUSHDATA1 556E6578706563746564206D6F6E74683A20 [8 datoshi]
    /// 94 : LDARG0 [2 datoshi]
    /// 95 : CALLT 0000 [32768 datoshi]
    /// 98 : CAT [2048 datoshi]
    /// 99 : PUSHDATA1 2E '.' [8 datoshi]
    /// 9C : CAT [2048 datoshi]
    /// 9D : CONVERT 28 'ByteString' [8192 datoshi]
    /// 9F : THROW [512 datoshi]
    /// A0 : LDLOC0 [2 datoshi]
    /// A1 : THROW [512 datoshi]
    /// </remarks>
    [DisplayName("getCalendarSeason")]
    public abstract string? GetCalendarSeason(BigInteger? month);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQADA1IZWxsbywgV29ybGQhcGhxadkoaXImCGpBz+dHlgwNSGVsbG8sIFdvcmxkIXJqc2vZKCYcDBNncmVldGluZzIgaXMgc3RyaW5nQc/nR5ZA
    /// 00 : INITSLOT 0400 [64 datoshi]
    /// 03 : PUSHDATA1 48656C6C6F2C20576F726C6421 [8 datoshi]
    /// 12 : STLOC0 [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : STLOC1 [2 datoshi]
    /// 15 : LDLOC1 [2 datoshi]
    /// 16 : ISTYPE 28 'ByteString' [2 datoshi]
    /// 18 : LDLOC1 [2 datoshi]
    /// 19 : STLOC2 [2 datoshi]
    /// 1A : JMPIFNOT 08 [2 datoshi]
    /// 1C : LDLOC2 [2 datoshi]
    /// 1D : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 22 : PUSHDATA1 48656C6C6F2C20576F726C6421 [8 datoshi]
    /// 31 : STLOC2 [2 datoshi]
    /// 32 : LDLOC2 [2 datoshi]
    /// 33 : STLOC3 [2 datoshi]
    /// 34 : LDLOC3 [2 datoshi]
    /// 35 : ISTYPE 28 'ByteString' [2 datoshi]
    /// 37 : JMPIFNOT 1C [2 datoshi]
    /// 39 : PUSHDATA1 6772656574696E673220697320737472696E67 [8 datoshi]
    /// 4E : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 53 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDeclarationPattern")]
    public abstract void TestDeclarationPattern();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2KpA
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : ISNULL [2 datoshi]
    /// 07 : NOT [4 datoshi]
    /// 08 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNotPattern")]
    public abstract bool? TestNotPattern(bool? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADBQAAAAAAAAAAAAAAAAAAAAAAAAAAHBocWlK2SgkBkUJIgbKABSzCJexqgmXJgQIQAgmBAlAaTo=
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 19 : STLOC0 [2 datoshi]
    /// 1A : LDLOC0 [2 datoshi]
    /// 1B : STLOC1 [2 datoshi]
    /// 1C : LDLOC1 [2 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : ISTYPE 28 'ByteString' [2 datoshi]
    /// 20 : JMPIF 06 [2 datoshi]
    /// 22 : DROP [2 datoshi]
    /// 23 : PUSHF [1 datoshi]
    /// 24 : JMP 06 [2 datoshi]
    /// 26 : SIZE [4 datoshi]
    /// 27 : PUSHINT8 14 [1 datoshi]
    /// 29 : NUMEQUAL [8 datoshi]
    /// 2A : PUSHT [1 datoshi]
    /// 2B : EQUAL [32 datoshi]
    /// 2C : NZ [4 datoshi]
    /// 2D : NOT [4 datoshi]
    /// 2E : PUSHF [1 datoshi]
    /// 2F : EQUAL [32 datoshi]
    /// 30 : JMPIFNOT 04 [2 datoshi]
    /// 32 : PUSHT [1 datoshi]
    /// 33 : RET [0 datoshi]
    /// 34 : PUSHT [1 datoshi]
    /// 35 : JMPIFNOT 04 [2 datoshi]
    /// 37 : PUSHF [1 datoshi]
    /// 38 : RET [0 datoshi]
    /// 39 : LDLOC1 [2 datoshi]
    /// 3A : THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testRecursivePattern")]
    public abstract bool? TestRecursivePattern();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2TAmA0Bo2SgmA0Bo2SBFQA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : ISTYPE 30 'Buffer' [2 datoshi]
    /// 08 : JMPIFNOT 03 [2 datoshi]
    /// 0A : RET [0 datoshi]
    /// 0B : LDLOC0 [2 datoshi]
    /// 0C : ISTYPE 28 'ByteString' [2 datoshi]
    /// 0E : JMPIFNOT 03 [2 datoshi]
    /// 10 : RET [0 datoshi]
    /// 11 : LDLOC0 [2 datoshi]
    /// 12 : ISTYPE 20 'Boolean' [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testTypePattern")]
    public abstract void TestTypePattern(object? o1 = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2TAmBQgiBWjZKCYFCCIHaNkoCJcmBBBAaNkgJgQRQGjZIQiXJgQSQAgmBBVAaDo=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : ISTYPE 30 'Buffer' [2 datoshi]
    /// 08 : JMPIFNOT 05 [2 datoshi]
    /// 0A : PUSHT [1 datoshi]
    /// 0B : JMP 05 [2 datoshi]
    /// 0D : LDLOC0 [2 datoshi]
    /// 0E : ISTYPE 28 'ByteString' [2 datoshi]
    /// 10 : JMPIFNOT 05 [2 datoshi]
    /// 12 : PUSHT [1 datoshi]
    /// 13 : JMP 07 [2 datoshi]
    /// 15 : LDLOC0 [2 datoshi]
    /// 16 : ISTYPE 28 'ByteString' [2 datoshi]
    /// 18 : PUSHT [1 datoshi]
    /// 19 : EQUAL [32 datoshi]
    /// 1A : JMPIFNOT 04 [2 datoshi]
    /// 1C : PUSH0 [1 datoshi]
    /// 1D : RET [0 datoshi]
    /// 1E : LDLOC0 [2 datoshi]
    /// 1F : ISTYPE 20 'Boolean' [2 datoshi]
    /// 21 : JMPIFNOT 04 [2 datoshi]
    /// 23 : PUSH1 [1 datoshi]
    /// 24 : RET [0 datoshi]
    /// 25 : LDLOC0 [2 datoshi]
    /// 26 : ISTYPE 21 'Integer' [2 datoshi]
    /// 28 : PUSHT [1 datoshi]
    /// 29 : EQUAL [32 datoshi]
    /// 2A : JMPIFNOT 04 [2 datoshi]
    /// 2C : PUSH2 [1 datoshi]
    /// 2D : RET [0 datoshi]
    /// 2E : PUSHT [1 datoshi]
    /// 2F : JMPIFNOT 04 [2 datoshi]
    /// 31 : PUSH5 [1 datoshi]
    /// 32 : RET [0 datoshi]
    /// 33 : LDLOC0 [2 datoshi]
    /// 34 : THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testTypePattern2")]
    public abstract BigInteger? TestTypePattern2(object? t = null);

    #endregion
}
