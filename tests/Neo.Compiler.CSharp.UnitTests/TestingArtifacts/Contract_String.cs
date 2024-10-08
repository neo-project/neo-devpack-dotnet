using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_String(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_String"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testMain"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""testEqual"",""parameters"":[],""returntype"":""Void"",""offset"":82,""safe"":false},{""name"":""testSubstring"",""parameters"":[],""returntype"":""Void"",""offset"":127,""safe"":false},{""name"":""testEmpty"",""parameters"":[],""returntype"":""String"",""offset"":163,""safe"":false},{""name"":""testIsNullOrEmpty"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Boolean"",""offset"":166,""safe"":false},{""name"":""testEndWith"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Boolean"",""offset"":181,""safe"":false},{""name"":""testContains"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Boolean"",""offset"":220,""safe"":false},{""name"":""testIndexOf"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Integer"",""offset"":237,""safe"":false},{""name"":""testInterpolatedStringHandler"",""parameters"":[],""returntype"":""String"",""offset"":252,""safe"":false},{""name"":""testTrim"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""String"",""offset"":579,""safe"":false},{""name"":""testPickItem"",""parameters"":[{""name"":""s"",""type"":""String""},{""name"":""index"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":646,""safe"":false},{""name"":""testSubstringToEnd"",""parameters"":[{""name"":""s"",""type"":""String""},{""name"":""startIndex"",""type"":""Integer""}],""returntype"":""String"",""offset"":653,""safe"":false},{""name"":""testConcat"",""parameters"":[{""name"":""s1"",""type"":""String""},{""name"":""s2"",""type"":""String""}],""returntype"":""String"",""offset"":664,""safe"":false},{""name"":""testIndexOfChar"",""parameters"":[{""name"":""s"",""type"":""String""},{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":686,""safe"":false},{""name"":""testToLower"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""String"",""offset"":695,""safe"":false},{""name"":""testToUpper"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""String"",""offset"":743,""safe"":false},{""name"":""testTrimChar"",""parameters"":[{""name"":""s"",""type"":""String""},{""name"":""trimChar"",""type"":""Integer""}],""returntype"":""String"",""offset"":791,""safe"":false},{""name"":""testLength"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Integer"",""offset"":846,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa"",""memorySearch""]},{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""currentHash"",""getBlock""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAS+8gQxQDYqd8FQmcfmTBL3ALZl2ghnZXRCbG9jawEAAQ++8gQxQDYqd8FQmcfmTBL3ALZl2gtjdXJyZW50SGFzaAAAAQ/A7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABD8DvOc7g5OklxsKgannhRA3Yb86sDG1lbW9yeVNlYXJjaAIAAQ8AAP1UA1cDAAwETWFya3AMAHE3AQA3AAAUznIMB0hlbGxvLCBoiwwBIItpiwwXISBDdXJyZW50IHRpbWVzdGFtcCBpcyCLajcCAIsMAS6L2yhBz+dHlkBXAgAMBWhlbGxvcAwFaGVsbG9xaGmXJAsMBUZhbHNlIggMBFRydWVBz+dHlkBXAQAMCDAxMjM0NTY3cGgRS8pLn4xBz+dHlmgRFIxBz+dHlkAMAEBXAAF4StgkBsoQs0BFCEBXAAEMBXdvcmxkeErKUUrKShNSUJ9KECwIRUVFRQlAE1JTjNsol0BXAAEMBXdvcmxkeDcDABC4QFcAAQwFd29ybGR4NwMAQFcEAAQAAKDexa3JNTYAAAAAAAAAcAwiTlhWN1poSGl5TTFhSFh3cFZzUlpDNkJ3TkZQMmpnaFhBcXEMAwECA9swcgwHU0J5dGU6IADWNwIAiwwILCBCeXRlOiCLACo3AgCLDAosIFVTaG9ydDogiwHoAzcCAIsMAiwgi9soDAZVSW50OiACQEIPADcCAIsMCSwgVUxvbmc6IIsDABCl1OgAAAA3AgCLDAIsIIvbKIvbKAwMQmlnSW50ZWdlcjogaDcCAIsMCCwgQ2hhcjogiwBB2yiLDAosIFN0cmluZzogiwwFSGVsbG+LDAIsIIvbKIvbKAwJRUNQb2ludDogaYsMDiwgQnl0ZVN0cmluZzogiwwNU3lzdGVtLkJ5dGVbXYsMCCwgQm9vbDogiwgmCgwEVHJ1ZSIJDAVGYWxzZYvbKIvbKHNrQFcDAXjKcBBxaJ1yaWi1JhV4ac5KGR67UAAgl6wmB2mccSLqamm3JhV4as5KGR67UAAgl6wmB2qdciLqeGlqaZ+cjEBXAAJ4ec5AVwACeHlLykufjEBXAAJ5eErYJgVFDABQStgmBUUMAItAVwACeXg3AwBAVwABDAAQSnjKtSYiSnhQzkoAQQBbuyQJUVCLUJwi6QBBnwBhnlFQi1CcItxF2yhAVwABDAAQSnjKtSYiSnhQzkoAYQB7uyQJUVCLUJwi6QBhnwBBnlFQi1CcItxF2yhAVwMCeXjKcBBxaJ1yRWlotSYOeGnOebMmB2mccSLxamm3Jg54as55syYHap1yIvF4aWppn5yMQFcAAXjKQGi4rDg="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testConcat")]
    public abstract string? TestConcat(string? s1, string? s2);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testContains")]
    public abstract bool? TestContains(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEmpty")]
    public abstract string? TestEmpty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEndWith")]
    public abstract bool? TestEndWith(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEqual")]
    public abstract void TestEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIndexOf")]
    public abstract BigInteger? TestIndexOf(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIndexOfChar")]
    public abstract BigInteger? TestIndexOfChar(string? s, BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testInterpolatedStringHandler")]
    public abstract string? TestInterpolatedStringHandler();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIsNullOrEmpty")]
    public abstract bool? TestIsNullOrEmpty(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testLength")]
    public abstract BigInteger? TestLength(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testMain")]
    public abstract void TestMain();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPickItem")]
    public abstract BigInteger? TestPickItem(string? s, BigInteger? index);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSubstring")]
    public abstract void TestSubstring();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSubstringToEnd")]
    public abstract string? TestSubstringToEnd(string? s, BigInteger? startIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testToLower")]
    public abstract string? TestToLower(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testToUpper")]
    public abstract string? TestToUpper(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testTrim")]
    public abstract string? TestTrim(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testTrimChar")]
    public abstract string? TestTrimChar(string? s, BigInteger? trimChar);

    #endregion

}
