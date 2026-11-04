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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Regex"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testStartWith"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testIndexOf"",""parameters"":[],""returntype"":""Integer"",""offset"":34,""safe"":false},{""name"":""testEndWith"",""parameters"":[],""returntype"":""Boolean"",""offset"":62,""safe"":false},{""name"":""testEndsWith"",""parameters"":[{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":150,""safe"":false},{""name"":""testContains"",""parameters"":[],""returntype"":""Boolean"",""offset"":164,""safe"":false},{""name"":""testNumberOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":195,""safe"":false},{""name"":""testAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":243,""safe"":false},{""name"":""testLowerAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":336,""safe"":false},{""name"":""testUpperAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":400,""safe"":false},{""name"":""testNumberRejectsNonDigit"",""parameters"":[],""returntype"":""Boolean"",""offset"":464,""safe"":false},{""name"":""testAlphabetRejectsNumber"",""parameters"":[],""returntype"":""Boolean"",""offset"":483,""safe"":false},{""name"":""testAlphabetInput"",""parameters"":[{""name"":""input"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":498,""safe"":false},{""name"":""testLowerAlphabetRejectsUpper"",""parameters"":[],""returntype"":""Boolean"",""offset"":508,""safe"":false},{""name"":""testUpperAlphabetRejectsLower"",""parameters"":[],""returntype"":""Boolean"",""offset"":520,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""memorySearch""]}],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrAxtZW1vcnlTZWFyY2gCAAEFwO85zuDk6SXGwqBqeeFEDdhvzqwMbWVtb3J5U2VhcmNoAwABBQAA/RECDAVIZWxsbwwLSGVsbG8gV29ybGQ0A0BXAAJ5eDcAALGqQAwBbwwLSGVsbG8gV29ybGQ0A0BXAAJ5eDcAAEAMBVdvcmxkDAtIZWxsbyBXb3JsZDQDQFcCAnnKcHjKaJ9KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaRC1JgQJQGgkBAhAaXl4NwEAabNAVwABDAVXb3JsZHg0tEAMAmxsDAtIZWxsbyBXb3JsZDQDQFcAAnl4NwAAD7RADAowMTIzNDU2Nzg5NANAVwIBEHAiFXhoznFpADAAOrskBAlAaEqccEVoeMow6ghADDRBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWmFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6NANAVwIBEHAiGHhoznFpACCSAGEAe7skBAlAaEqccEVoeMow5whADBphYmNkZWZnaGlqa2xtbm9wcXJzdHV2d3h5ejQDQFcCARBwIhV4aM5xaQBhAHu7JAQJQGhKnHBFaHjKMOoIQAwaQUJDREVGR0hJSktMTU5PUFFSU1RVVldYWVo0A0BXAgEQcCIVeGjOcWkAQQBbuyQECUBoSpxwRWh4yjDqCEAMCzAxMjM0NTY3ODlBNfX+//9ADAdBQkN4eXoxNUD///9AVwABeDU2////QAwEYWJjWjVt////QAwEQUJDejShQMG+Rg8=").AsSerializable<Neo.SmartContract.NefFile>();

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
    /// Script: VwABDAVXb3JsZHg0tEA=
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 576F726C64 'World' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL B4 [512 datoshi]
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
