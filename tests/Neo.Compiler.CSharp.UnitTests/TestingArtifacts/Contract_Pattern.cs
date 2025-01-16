using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/UkCVwEBeHBoEbckBAlAaABktUBXAQF4cGgRtyQECUBoAGS1QFcBAXhwaBG3JAUJIgZoADK1JgQIQGgAMrgkBQkiBmgAZLUmBAhACCYECUBoOlcCAAwUAAAAAAAAAAAAAAAAAAAAAAAAAABwaHFpStkoJAZFCSIGygAUswiXsaoJlyYECEAIJgQJQGk6VwEBeHBoELZAVwEBeHBo2KpAVwEBeHBoANi1JgwMB1RvbyBsb3dAaADYuCQFCSIFaBC1JggMA0xvd0BoELgkBQkiBWgatSYPDApBY2NlcHRhYmxlQGgauCQFCSIGaAAUtSYJDARIaWdoQGgAFLgmDQwIVG9vIGhpZ2hAaDpXAQF4cGgTlyYFCCIFaBSXJgUIIgVoFZcmCwwGc3ByaW5nQGgWlyYFCCIFaBeXJgUIIgVoGJcmCwwGc3VtbWVyQGgZlyYFCCIFaBqXJgUIIgVoG5cmCwwGYXV0dW1uQGgclyYFCCIFaBGXJgUIIgVoEpcmCwwGd2ludGVyQAgmIgwSVW5leHBlY3RlZCBtb250aDogeDcAAIsMAS6L2yg6aDpXBAAMDUhlbGxvLCBXb3JsZCFwaHFp2ShpciYIakHP50eWDA1IZWxsbywgV29ybGQhcmpza9koJhwME2dyZWV0aW5nMiBpcyBzdHJpbmdBz+dHlkBXAQF4cGjZMCYDQGjZKCYDQGjZIEVAVwEBeHBo2TAmBQgiBWjZKCYFCCIHaNkoCJcmBBBAaNkgJgQRQGjZIQiXJgQSQAgmBBVAaDpAtOcsgw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoEbckBAlAaABktUA=
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// LT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("between")]
    public abstract bool? Between(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoEbckBAlAaABktUA=
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// LT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("between2")]
    public abstract bool? Between2(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoEbckBQkiBmgAMrUmBAhAaAAyuCQFCSIGaABktSYECEAIJgQJQGg6
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 32 [1 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 32 [1 datoshi]
    /// GE [8 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// THROW [512 datoshi]
    /// </remarks>
    [DisplayName("between3")]
    public abstract bool? Between3(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoELZA
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LE [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("between4")]
    public abstract bool? Between4(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoANi1JgwMB1RvbyBsb3dAaADYuCQFCSIFaBC1JggMA0xvd0BoELgkBQkiBWgatSYPDApBY2NlcHRhYmxlQGgauCQFCSIGaAAUtSYJDARIaWdoQGgAFLgmDQwIVG9vIGhpZ2hAaDo=
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 D8 [1 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 0C [2 datoshi]
    /// PUSHDATA1 546F6F206C6F77 [8 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 D8 [1 datoshi]
    /// GE [8 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// PUSHDATA1 4C6F77 'Low' [8 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// GE [8 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH10 [1 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 0F [2 datoshi]
    /// PUSHDATA1 41636365707461626C65 'Acceptable' [8 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH10 [1 datoshi]
    /// GE [8 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 09 [2 datoshi]
    /// PUSHDATA1 48696768 'High' [8 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// GE [8 datoshi]
    /// JMPIFNOT 0D [2 datoshi]
    /// PUSHDATA1 546F6F2068696768 [8 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// THROW [512 datoshi]
    /// </remarks>
    [DisplayName("classify")]
    public abstract string? Classify(BigInteger? measurement);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoE5cmBQgiBWgUlyYFCCIFaBWXJgsMBnNwcmluZ0BoFpcmBQgiBWgXlyYFCCIFaBiXJgsMBnN1bW1lckBoGZcmBQgiBWgalyYFCCIFaBuXJgsMBmF1dHVtbkBoHJcmBQgiBWgRlyYFCCIFaBKXJgsMBndpbnRlckAIJiIMElVuZXhwZWN0ZWQgbW9udGg6IHg3AACLDAEui9soOmg6
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH5 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0B [2 datoshi]
    /// PUSHDATA1 737072696E67 'spring' [8 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH6 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH7 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH8 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0B [2 datoshi]
    /// PUSHDATA1 73756D6D6572 'summer' [8 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH9 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH10 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH11 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0B [2 datoshi]
    /// PUSHDATA1 617574756D6E 'autumn' [8 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH12 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0B [2 datoshi]
    /// PUSHDATA1 77696E746572 'winter' [8 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// JMPIFNOT 22 [2 datoshi]
    /// PUSHDATA1 556E6578706563746564206D6F6E74683A20 [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 2E '.' [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// THROW [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// THROW [512 datoshi]
    /// </remarks>
    [DisplayName("getCalendarSeason")]
    public abstract string? GetCalendarSeason(BigInteger? month);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQADA1IZWxsbywgV29ybGQhcGhxadkoaXImCGpBz+dHlgwNSGVsbG8sIFdvcmxkIXJqc2vZKCYcDBNncmVldGluZzIgaXMgc3RyaW5nQc/nR5ZA
    /// INITSLOT 0400 [64 datoshi]
    /// PUSHDATA1 48656C6C6F2C20576F726C6421 [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// PUSHDATA1 48656C6C6F2C20576F726C6421 [8 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// JMPIFNOT 1C [2 datoshi]
    /// PUSHDATA1 6772656574696E673220697320737472696E67 [8 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDeclarationPattern")]
    public abstract void TestDeclarationPattern();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2KpA
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNotPattern")]
    public abstract bool? TestNotPattern(bool? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADBQAAAAAAAAAAAAAAAAAAAAAAAAAAHBocWlK2SgkBkUJIgbKABSzCJexqgmXJgQIQAgmBAlAaTo=
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// PUSHT [1 datoshi]
    /// EQUAL [32 datoshi]
    /// NZ [4 datoshi]
    /// NOT [4 datoshi]
    /// PUSHF [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC1 [2 datoshi]
    /// THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testRecursivePattern")]
    public abstract bool? TestRecursivePattern();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2TAmA0Bo2SgmA0Bo2SBFQA==
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISTYPE 30 'Buffer' [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISTYPE 20 'Boolean' [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testTypePattern")]
    public abstract void TestTypePattern(object? o1 = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2TAmBQgiBWjZKCYFCCIHaNkoCJcmBBBAaNkgJgQRQGjZIQiXJgQSQAgmBBVAaDo=
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISTYPE 30 'Buffer' [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 07 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// PUSHT [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISTYPE 20 'Boolean' [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISTYPE 21 'Integer' [2 datoshi]
    /// PUSHT [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH5 [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testTypePattern2")]
    public abstract BigInteger? TestTypePattern2(object? t = null);

    #endregion
}
