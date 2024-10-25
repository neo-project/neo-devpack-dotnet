using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Regex(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Regex"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testStartWith"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testIndexOf"",""parameters"":[],""returntype"":""Integer"",""offset"":34,""safe"":false},{""name"":""testEndWith"",""parameters"":[],""returntype"":""Boolean"",""offset"":62,""safe"":false},{""name"":""testContains"",""parameters"":[],""returntype"":""Boolean"",""offset"":146,""safe"":false},{""name"":""testNumberOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":177,""safe"":false},{""name"":""testAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":236,""safe"":false},{""name"":""testLowerAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":354,""safe"":false},{""name"":""testUpperAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":385,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""memorySearch""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrAxtZW1vcnlTZWFyY2gCAAEPAAD9oAEMBUhlbGxvDAtIZWxsbyBXb3JsZDQDQFcAAnl4NwAAELNADAFvDAtIZWxsbyBXb3JsZDQDQFcAAnl4NwAAQAwFV29ybGQMC0hlbGxvIFdvcmxkNANAVwACeXg3AAB5yp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfeMqzQAwCbGwMC0hlbGxvIFdvcmxkNANAVwACeXg3AAAPmEAMCjAxMjM0NTY3ODk0A0BXBQF4SnDKcRByIhxoas5za3RsADC1JgUIIgZsADm3JgQJQGqccmppMOQIQAw0QUJDREVGR0hJSktMTU5PUFFSU1RVVldYWVphYmNkZWZnaGlqa2xtbm9wcXJzdHV2d3h5ejQDQFcEAXhKcMpxEHIiLWhqznNrAEG4JAUJIgZrAFq2JgUIIg9rAGG4JAUJIgZrAHq2qiYECUBqnHJqaTDTCEAMGmFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6NKdADBpBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWjSIQBivb8k="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DEFCQ0RFRkdISUpLTE1OT1BRUlNUVVZXWFlaYWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXo0A0A=
    /// 00 : OpCode.PUSHDATA1 4142434445464748494A4B4C4D4E4F505152535455565758595A6162636465666768696A6B6C6D6E6F707172737475767778797A
    /// 36 : OpCode.CALL 03
    /// 38 : OpCode.RET
    /// </remarks>
    [DisplayName("testAlphabetOnly")]
    public abstract bool? TestAlphabetOnly();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DGxsDEhlbGxvIFdvcmxkNANA
    /// 00 : OpCode.PUSHDATA1 6C6C
    /// 04 : OpCode.PUSHDATA1 48656C6C6F20576F726C64
    /// 11 : OpCode.CALL 03
    /// 13 : OpCode.RET
    /// </remarks>
    [DisplayName("testContains")]
    public abstract bool? TestContains();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DFdvcmxkDEhlbGxvIFdvcmxkNANA
    /// 00 : OpCode.PUSHDATA1 576F726C64
    /// 07 : OpCode.PUSHDATA1 48656C6C6F20576F726C64
    /// 14 : OpCode.CALL 03
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("testEndWith")]
    public abstract bool? TestEndWith();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DG8MSGVsbG8gV29ybGQ0A0A=
    /// 00 : OpCode.PUSHDATA1 6F
    /// 03 : OpCode.PUSHDATA1 48656C6C6F20576F726C64
    /// 10 : OpCode.CALL 03
    /// 12 : OpCode.RET
    /// </remarks>
    [DisplayName("testIndexOf")]
    public abstract BigInteger? TestIndexOf();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DGFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6NKdA
    /// 00 : OpCode.PUSHDATA1 6162636465666768696A6B6C6D6E6F707172737475767778797A
    /// 1C : OpCode.CALL A7
    /// 1E : OpCode.RET
    /// </remarks>
    [DisplayName("testLowerAlphabetOnly")]
    public abstract bool? TestLowerAlphabetOnly();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DDAxMjM0NTY3ODk0A0A=
    /// 00 : OpCode.PUSHDATA1 30313233343536373839
    /// 0C : OpCode.CALL 03
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("testNumberOnly")]
    public abstract bool? TestNumberOnly();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DEhlbGxvDEhlbGxvIFdvcmxkNANA
    /// 00 : OpCode.PUSHDATA1 48656C6C6F
    /// 07 : OpCode.PUSHDATA1 48656C6C6F20576F726C64
    /// 14 : OpCode.CALL 03
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("testStartWith")]
    public abstract bool? TestStartWith();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DEFCQ0RFRkdISUpLTE1OT1BRUlNUVVZXWFlaNIhA
    /// 00 : OpCode.PUSHDATA1 4142434445464748494A4B4C4D4E4F505152535455565758595A
    /// 1C : OpCode.CALL 88
    /// 1E : OpCode.RET
    /// </remarks>
    [DisplayName("testUpperAlphabetOnly")]
    public abstract bool? TestUpperAlphabetOnly();

    #endregion
}
