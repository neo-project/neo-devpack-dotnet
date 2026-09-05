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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Regex"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testStartWith"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testIndexOf"",""parameters"":[],""returntype"":""Integer"",""offset"":35,""safe"":false},{""name"":""testEndWith"",""parameters"":[],""returntype"":""Boolean"",""offset"":64,""safe"":false},{""name"":""testContains"",""parameters"":[],""returntype"":""Boolean"",""offset"":154,""safe"":false},{""name"":""testNumberOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":186,""safe"":false},{""name"":""testAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":277,""safe"":false},{""name"":""testLowerAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":426,""safe"":false},{""name"":""testUpperAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":533,""safe"":false},{""name"":""testNumberRejectsNonDigit"",""parameters"":[],""returntype"":""Boolean"",""offset"":640,""safe"":false},{""name"":""testAlphabetRejectsNumber"",""parameters"":[],""returntype"":""Boolean"",""offset"":659,""safe"":false},{""name"":""testLowerAlphabetRejectsUpper"",""parameters"":[],""returntype"":""Boolean"",""offset"":674,""safe"":false},{""name"":""testUpperAlphabetRejectsLower"",""parameters"":[],""returntype"":""Boolean"",""offset"":686,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""memorySearch""]}],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrAxtZW1vcnlTZWFyY2gCAAEFwO85zuDk6SXGwqBqeeFEDdhvzqwMbWVtb3J5U2VhcmNoAwABBQAA/bcCDAtIZWxsbyBXb3JsZAwFSGVsbG9QNANAVwACeXg3AACxqkAMC0hlbGxvIFdvcmxkDAFvUDQDQFcAAnl4NwAAQAwLSGVsbG8gV29ybGQMBVdvcmxkUDQDQFcBAnnKEJcmBAhAeMp5yp9KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9waBC1JgQJQGh5eDcBAGizQAwLSGVsbG8gV29ybGQMAmxsUDQDQFcAAnl4NwAAD5hADAowMTIzNDU2Nzg5NANAVwMBEHAiP3hoznFpcmoAMLUmBQgiBmoAObcmBAlAaEqcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoeMq1JL8IQAw0QUJDREVGR0hJSktMTU5PUFFSU1RVVldYWVphYmNkZWZnaGlqa2xtbm9wcXJzdHV2d3h5ejQDQFcCARBwIk94aM5xaQBBuCQFCSIGaQBatiYFCCIPaQBhuCQFCSIGaQB6tiQECUBoSpxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWh4yrUkrwhADBphYmNkZWZnaGlqa2xtbm9wcXJzdHV2d3h5ejQDQFcDARBwIj94aM5xaXJqAGG1JgUIIgZqAHq3JgQJQGhKnErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaHjKtSS/CEAMGkFCQ0RFRkdISUpLTE1OT1BRUlNUVVZXWFlaNANAVwMBEHAiP3hoznFpcmoAQbUmBQgiBmoAWrcmBAlAaEqcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoeMq1JL8IQAwLMDEyMzQ1Njc4OUE1PP7//0AMB0FCQ3h5ejE1sv7//0AMBGFiY1o1If///0AMBEFCQ3o0gEDQHs3K").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

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
    /// Script: DAdBQkN4eXoxNbL+//9A
    /// PUSHDATA1 41424378797A31 'ABCxyz1' [8 datoshi]
    /// CALL_L B2FEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAlphabetRejectsNumber")]
    public abstract bool? TestAlphabetRejectsNumber();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAtIZWxsbyBXb3JsZAwCbGxQNANA
    /// PUSHDATA1 48656C6C6F20576F726C64 [8 datoshi]
    /// PUSHDATA1 6C6C 'll' [8 datoshi]
    /// SWAP [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testContains")]
    public abstract bool? TestContains();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAtIZWxsbyBXb3JsZAwFV29ybGRQNANA
    /// PUSHDATA1 48656C6C6F20576F726C64 [8 datoshi]
    /// PUSHDATA1 576F726C64 'World' [8 datoshi]
    /// SWAP [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEndWith")]
    public abstract bool? TestEndWith();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAtIZWxsbyBXb3JsZAwBb1A0A0A=
    /// PUSHDATA1 48656C6C6F20576F726C64 [8 datoshi]
    /// PUSHDATA1 6F 'o' [8 datoshi]
    /// SWAP [2 datoshi]
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
    /// Script: DARhYmNaNSH///9A
    /// PUSHDATA1 6162635A 'abcZ' [8 datoshi]
    /// CALL_L 21FFFFFF [512 datoshi]
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
    /// Script: DAswMTIzNDU2Nzg5QTU8/v//QA==
    /// PUSHDATA1 3031323334353637383941 '0123456789A' [8 datoshi]
    /// CALL_L 3CFEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNumberRejectsNonDigit")]
    public abstract bool? TestNumberRejectsNonDigit();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAtIZWxsbyBXb3JsZAwFSGVsbG9QNANA
    /// PUSHDATA1 48656C6C6F20576F726C64 [8 datoshi]
    /// PUSHDATA1 48656C6C6F 'Hello' [8 datoshi]
    /// SWAP [2 datoshi]
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
    /// Script: DARBQkN6NIBA
    /// PUSHDATA1 4142437A 'ABCz' [8 datoshi]
    /// CALL 80 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUpperAlphabetRejectsLower")]
    public abstract bool? TestUpperAlphabetRejectsLower();

    #endregion
}
