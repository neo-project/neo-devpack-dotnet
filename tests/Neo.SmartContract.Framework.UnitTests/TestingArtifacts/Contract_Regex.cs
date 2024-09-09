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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrAxtZW1vcnlTZWFyY2gCAAEPAAD9oAEMBUhlbGxvDAtIZWxsbyBXb3JsZDQDQFcAAnl4NwAAEJdADAFvDAtIZWxsbyBXb3JsZDQDQFcAAnl4NwAAQAwFV29ybGQMC0hlbGxvIFdvcmxkNANAVwACeXg3AAB5yp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfeMqXQAwCbGwMC0hlbGxvIFdvcmxkNANAVwACeXg3AAAPmEAMCjAxMjM0NTY3ODk0A0BXBQF4SnDKcRByIhxoas5za3RsADC1JgUIIgZsADm3JgQJQGqccmppMOQIQAw0QUJDREVGR0hJSktMTU5PUFFSU1RVVldYWVphYmNkZWZnaGlqa2xtbm9wcXJzdHV2d3h5ejQDQFcEAXhKcMpxEHIiLWhqznNrAEG4JAUJIgZrAFq2JgUIIg9rAGG4JAUJIgZrAHq2qiYECUBqnHJqaTDTCEAMGmFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6NKdADBpBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWjSIQNFUrHQ="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAlphabetOnly")]
    public abstract bool? TestAlphabetOnly();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testContains")]
    public abstract bool? TestContains();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEndWith")]
    public abstract bool? TestEndWith();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIndexOf")]
    public abstract BigInteger? TestIndexOf();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testLowerAlphabetOnly")]
    public abstract bool? TestLowerAlphabetOnly();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testNumberOnly")]
    public abstract bool? TestNumberOnly();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStartWith")]
    public abstract bool? TestStartWith();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testUpperAlphabetOnly")]
    public abstract bool? TestUpperAlphabetOnly();

    #endregion

}
