using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Regex(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Regex"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testStartWith"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testStartsWith"",""parameters"":[{""name"":""value"",""type"":""ByteArray""},{""name"":""toFind"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":45,""safe"":false},{""name"":""testIndexOf"",""parameters"":[],""returntype"":""Integer"",""offset"":53,""safe"":false},{""name"":""testEndWith"",""parameters"":[],""returntype"":""Boolean"",""offset"":81,""safe"":false},{""name"":""testEndsWith"",""parameters"":[{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":126,""safe"":false},{""name"":""testContains"",""parameters"":[],""returntype"":""Boolean"",""offset"":140,""safe"":false},{""name"":""testNumberOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":171,""safe"":false},{""name"":""testAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":219,""safe"":false},{""name"":""testLowerAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":312,""safe"":false},{""name"":""testUpperAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":376,""safe"":false},{""name"":""testNumberRejectsNonDigit"",""parameters"":[],""returntype"":""Boolean"",""offset"":440,""safe"":false},{""name"":""testAlphabetRejectsNumber"",""parameters"":[],""returntype"":""Boolean"",""offset"":459,""safe"":false},{""name"":""testAlphabetInput"",""parameters"":[{""name"":""input"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":474,""safe"":false},{""name"":""testLowerAlphabetRejectsUpper"",""parameters"":[],""returntype"":""Boolean"",""offset"":484,""safe"":false},{""name"":""testUpperAlphabetRejectsLower"",""parameters"":[],""returntype"":""Boolean"",""offset"":496,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""memorySearch""]}],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrAxtZW1vcnlTZWFyY2gCAAEFAAD9+QEMBUhlbGxvDAtIZWxsbyBXb3JsZDQDQFcBAnnKcGh4yrcmBAlAeGiN2yh5l0BXAAJ5eDTlQAwBbwwLSGVsbG8gV29ybGQ0A0BXAAJ5eDcAAEAMBVdvcmxkDAtIZWxsbyBXb3JsZDQDQFcBAnnKcGh4yrcmBAlAeGiO2yh5l0BXAAEMBVdvcmxkeDTfQAwCbGwMC0hlbGxvIFdvcmxkNANAVwACeXg3AAAPtEAMCjAxMjM0NTY3ODk0A0BXAgEQcCIVeGjOcWkAMAA6uyQECUBoSpxwRWh4yjDqCEAMNEFCQ0RFRkdISUpLTE1OT1BRUlNUVVZXWFlaYWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXo0A0BXAgEQcCIYeGjOcWkAIJIAYQB7uyQECUBoSpxwRWh4yjDnCEAMGmFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6NANAVwIBEHAiFXhoznFpAGEAe7skBAlAaEqccEVoeMow6ghADBpBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWjQDQFcCARBwIhV4aM5xaQBBAFu7JAQJQGhKnHBFaHjKMOoIQAwLMDEyMzQ1Njc4OUE19f7//0AMB0FCQ3h5ejE1QP///0BXAAF4NTb///9ADARhYmNaNW3///9ADARBQkN6NKFAIRuBpg==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDU2////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 36FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAlphabetInput")]
    public abstract bool? TestAlphabetInput(byte[]? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DDRBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWmFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6NANA
    /// PUSHDATA1 4142434445464748494A4B4C4D4E4F505152535455565758595A6162636465666768696A6B6C6D6E6F707172737475767778797A 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz' [8 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAlphabetOnly")]
    public abstract bool? TestAlphabetOnly();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAdBQkN4eXoxNUD///9A
    /// PUSHDATA1 41424378797A31 'ABCxyz1' [8 datoshi]
    /// CALL_L 40FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAlphabetRejectsNumber")]
    public abstract bool? TestAlphabetRejectsNumber();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAJsbAwLSGVsbG8gV29ybGQ0A0A=
    /// PUSHDATA1 6C6C 'll' [8 datoshi]
    /// PUSHDATA1 48656C6C6F20576F726C64 [8 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testContains")]
    public abstract bool? TestContains();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAVXb3JsZAwLSGVsbG8gV29ybGQ0A0A=
    /// PUSHDATA1 576F726C64 'World' [8 datoshi]
    /// PUSHDATA1 48656C6C6F20576F726C64 [8 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEndWith")]
    public abstract bool? TestEndWith();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAVXb3JsZHg030A=
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 576F726C64 'World' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL DF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEndsWith")]
    public abstract bool? TestEndsWith(byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAFvDAtIZWxsbyBXb3JsZDQDQA==
    /// PUSHDATA1 6F 'o' [8 datoshi]
    /// PUSHDATA1 48656C6C6F20576F726C64 [8 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIndexOf")]
    public abstract BigInteger? TestIndexOf();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DBphYmNkZWZnaGlqa2xtbm9wcXJzdHV2d3h5ejQDQA==
    /// PUSHDATA1 6162636465666768696A6B6C6D6E6F707172737475767778797A 'abcdefghijklmnopqrstuvwxyz' [8 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLowerAlphabetOnly")]
    public abstract bool? TestLowerAlphabetOnly();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DARhYmNaNW3///9A
    /// PUSHDATA1 6162635A 'abcZ' [8 datoshi]
    /// CALL_L 6DFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLowerAlphabetRejectsUpper")]
    public abstract bool? TestLowerAlphabetRejectsUpper();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAowMTIzNDU2Nzg5NANA
    /// PUSHDATA1 30313233343536373839 '0123456789' [8 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNumberOnly")]
    public abstract bool? TestNumberOnly();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAswMTIzNDU2Nzg5QTX1/v//QA==
    /// PUSHDATA1 3031323334353637383941 '0123456789A' [8 datoshi]
    /// CALL_L F5FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNumberRejectsNonDigit")]
    public abstract bool? TestNumberRejectsNonDigit();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAVIZWxsbwwLSGVsbG8gV29ybGQ0A0A=
    /// PUSHDATA1 48656C6C6F 'Hello' [8 datoshi]
    /// PUSHDATA1 48656C6C6F20576F726C64 [8 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStartWith")]
    public abstract bool? TestStartWith();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg05UA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL E5 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStartsWith")]
    public abstract bool? TestStartsWith(byte[]? value, byte[]? toFind);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DBpBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWjQDQA==
    /// PUSHDATA1 4142434445464748494A4B4C4D4E4F505152535455565758595A 'ABCDEFGHIJKLMNOPQRSTUVWXYZ' [8 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUpperAlphabetOnly")]
    public abstract bool? TestUpperAlphabetOnly();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DARBQkN6NKFA
    /// PUSHDATA1 4142437A 'ABCz' [8 datoshi]
    /// CALL A1 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUpperAlphabetRejectsLower")]
    public abstract bool? TestUpperAlphabetRejectsLower();

    #endregion
}
