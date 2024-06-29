using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Regex : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Regex"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testStartWith"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testIndexOf"",""parameters"":[],""returntype"":""Integer"",""offset"":38,""safe"":false},{""name"":""testEndWith"",""parameters"":[],""returntype"":""Boolean"",""offset"":70,""safe"":false},{""name"":""testContains"",""parameters"":[],""returntype"":""Boolean"",""offset"":158,""safe"":false},{""name"":""testNumberOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":193,""safe"":false},{""name"":""testAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":257,""safe"":false},{""name"":""testLowerAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":380,""safe"":false},{""name"":""testUpperAlphabetOnly"",""parameters"":[],""returntype"":""Boolean"",""offset"":413,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""memorySearch""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrAxtZW1vcnlTZWFyY2gCAAEPAAD9vgEMBUhlbGxvDAtIZWxsbyBXb3JsZDQFIgJAVwACeXg3AAAQlyICQAwBbwwLSGVsbG8gV29ybGQ0BSICQFcAAnl4NwAAIgJADAVXb3JsZAwLSGVsbG8gV29ybGQ0BSICQFcAAnl4NwAAecqeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3jKlyICQAwCbGwMC0hlbGxvIFdvcmxkNAUiAkBXAAJ5eDcAAA+YIgJADAowMTIzNDU2Nzg5NAUiAkBXBQF4SnDKcRByIh1oas5za3RsADC1JgUIIgZsADm3JgUJIgxqnHJqaTDjCCICQAw0QUJDREVGR0hJSktMTU5PUFFSU1RVVldYWVphYmNkZWZnaGlqa2xtbm9wcXJzdHV2d3h5ejQFIgJAVwQBeEpwynEQciIuaGrOc2sAQbgkBQkiBmsAWrYmBQgiD2sAYbgkBQkiBmsAeraqJgUJIgxqnHJqaTDSCCICQAwaYWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXo0pCICQAwaQUJDREVGR0hJSktMTU5PUFFSU1RVVldYWVo0gyICQFoIlOI="));

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

    #region Constructor for internal use only

    protected Contract_Regex(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
