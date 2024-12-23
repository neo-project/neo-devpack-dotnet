using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Regex(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Regex"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testStartWith"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testIndexOf"",""parameters"":[],""returntype"":""Integer"",""offset"":34,""safe"":false},{""name"":""testEndWith"",""parameters"":[],""returntype"":""Boolean"",""offset"":62,""safe"":false},{""name"":""testContains"",""parameters"":[],""returntype"":""Boolean"",""offset"":146,""safe"":false},{""name"":""testNumberOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":177,""safe"":false},{""name"":""testAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":236,""safe"":false},{""name"":""testLowerAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":353,""safe"":false},{""name"":""testUpperAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":384,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""memorySearch""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrAxtZW1vcnlTZWFyY2gCAAEPAAD9nwEMBUhlbGxvDAtIZWxsbyBXb3JsZDQDQFcAAnl4NwAAsapADAFvDAtIZWxsbyBXb3JsZDQDQFcAAnl4NwAAQAwFV29ybGQMC0hlbGxvIFdvcmxkNANAVwACeXg3AAB5yp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfeMqzQAwCbGwMC0hlbGxvIFdvcmxkNANAVwACeXg3AAAPmEAMCjAxMjM0NTY3ODk0A0BXBQF4SnDKcRByIhxoas5za3RsADC1JgUIIgZsADm3JgQJQGqccmppMOQIQAw0QUJDREVGR0hJSktMTU5PUFFSU1RVVldYWVphYmNkZWZnaGlqa2xtbm9wcXJzdHV2d3h5ejQDQFcEAXhKcMpxEHIiLGhqznNrAEG4JAUJIgZrAFq2JgUIIg9rAGG4JAUJIgZrAHq2JAQJQGqccmppMNQIQAwaYWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXo0qEAMGkFCQ0RFRkdISUpLTE1OT1BRUlNUVVZXWFlaNIlAN8QmLg=="));

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
    /// Script: DBphYmNkZWZnaGlqa2xtbm9wcXJzdHV2d3h5ejSoQA==
    /// PUSHDATA1 6162636465666768696A6B6C6D6E6F707172737475767778797A 'abcdefghijklmnopqrstuvwxyz' [8 datoshi]
    /// CALL A8 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLowerAlphabetOnly")]
    public abstract bool? TestLowerAlphabetOnly();

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
    /// Script: DBpBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWjSJQA==
    /// PUSHDATA1 4142434445464748494A4B4C4D4E4F505152535455565758595A 'ABCDEFGHIJKLMNOPQRSTUVWXYZ' [8 datoshi]
    /// CALL 89 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUpperAlphabetOnly")]
    public abstract bool? TestUpperAlphabetOnly();

    #endregion
}
