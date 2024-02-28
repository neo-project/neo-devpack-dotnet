using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Binary : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Binary"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""base58CheckEncode"",""parameters"":[{""name"":""input"",""type"":""ByteArray""}],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""base58CheckDecode"",""parameters"":[{""name"":""input"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":10,""safe"":false},{""name"":""base64Decode"",""parameters"":[{""name"":""input"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":22,""safe"":false},{""name"":""base64Encode"",""parameters"":[{""name"":""input"",""type"":""ByteArray""}],""returntype"":""String"",""offset"":34,""safe"":false},{""name"":""base58Decode"",""parameters"":[{""name"":""input"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":46,""safe"":false},{""name"":""base58Encode"",""parameters"":[{""name"":""input"",""type"":""ByteArray""}],""returntype"":""String"",""offset"":58,""safe"":false},{""name"":""atoi"",""parameters"":[{""name"":""value"",""type"":""String""},{""name"":""base"",""type"":""Integer""}],""returntype"":""Any"",""offset"":70,""safe"":false},{""name"":""itoa"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""base"",""type"":""Integer""}],""returntype"":""String"",""offset"":81,""safe"":false},{""name"":""memoryCompare"",""parameters"":[{""name"":""str1"",""type"":""ByteArray""},{""name"":""str2"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":92,""safe"":false},{""name"":""memorySearch1"",""parameters"":[{""name"":""mem"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":103,""safe"":false},{""name"":""memorySearch2"",""parameters"":[{""name"":""mem"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""},{""name"":""start"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":114,""safe"":false},{""name"":""memorySearch3"",""parameters"":[{""name"":""mem"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""},{""name"":""start"",""type"":""Integer""},{""name"":""backward"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":126,""safe"":false},{""name"":""stringSplit1"",""parameters"":[{""name"":""str"",""type"":""String""},{""name"":""separator"",""type"":""String""}],""returntype"":""Array"",""offset"":139,""safe"":false},{""name"":""stringSplit2"",""parameters"":[{""name"":""str"",""type"":""String""},{""name"":""separator"",""type"":""String""},{""name"":""removeEmptyEntries"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":150,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi"",""base58CheckDecode"",""base58CheckEncode"",""base58Decode"",""base58Encode"",""base64Decode"",""base64Encode"",""itoa"",""memoryCompare"",""memorySearch"",""stringSplit""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM3Rlc3Rob3N0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA7A7znO4OTpJcbCoGp54UQN2G/OrBFiYXNlNThDaGVja0VuY29kZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrBFiYXNlNThDaGVja0RlY29kZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAxiYXNlNjREZWNvZGUBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwMYmFzZTY0RW5jb2RlAQABD8DvOc7g5OklxsKgannhRA3Yb86sDGJhc2U1OERlY29kZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAxiYXNlNThFbmNvZGUBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwEYXRvaQIAAQ/A7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAgABD8DvOc7g5OklxsKgannhRA3Yb86sDW1lbW9yeUNvbXBhcmUCAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwMbWVtb3J5U2VhcmNoAgABD8DvOc7g5OklxsKgannhRA3Yb86sDG1lbW9yeVNlYXJjaAMAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAxtZW1vcnlTZWFyY2gEAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwLc3RyaW5nU3BsaXQCAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwLc3RyaW5nU3BsaXQDAAEPAACiVwABeDcAACICQFcAAXg3AQDbMCICQFcAAXg3AgDbMCICQFcAAXjbKDcDACICQFcAAXg3BADbMCICQFcAAXjbKDcFACICQFcAAnl4NwYAIgJAVwACeXg3BwAiAkBXAAJ5eDcIACICQFcAAnl4NwkAIgJAVwADenl4NwoAIgJAVwAEe3p5eDcLACICQFcAAnl4NwwAIgJAVwADenl4Nw0AIgJAqC8B5w=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("atoi")]
    public abstract object? Atoi(string? value, BigInteger? @base);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("base58CheckDecode")]
    public abstract byte[]? Base58CheckDecode(string? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("base58CheckEncode")]
    public abstract string? Base58CheckEncode(byte[]? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("base58Decode")]
    public abstract byte[]? Base58Decode(string? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("base58Encode")]
    public abstract string? Base58Encode(byte[]? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("base64Decode")]
    public abstract byte[]? Base64Decode(string? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("base64Encode")]
    public abstract string? Base64Encode(byte[]? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("itoa")]
    public abstract string? Itoa(BigInteger? value, BigInteger? @base);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("memoryCompare")]
    public abstract BigInteger? MemoryCompare(byte[]? str1, byte[]? str2);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("memorySearch1")]
    public abstract BigInteger? MemorySearch1(byte[]? mem, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("memorySearch2")]
    public abstract BigInteger? MemorySearch2(byte[]? mem, byte[]? value, BigInteger? start);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("memorySearch3")]
    public abstract BigInteger? MemorySearch3(byte[]? mem, byte[]? value, BigInteger? start, bool? backward);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("stringSplit1")]
    public abstract IList<object>? StringSplit1(string? str, string? separator);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("stringSplit2")]
    public abstract IList<object>? StringSplit2(string? str, string? separator, bool? removeEmptyEntries);

    #endregion

    #region Constructor for internal use only

    protected Contract_Binary(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
