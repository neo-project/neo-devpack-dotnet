using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_StdLib(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_StdLib"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""base58CheckEncode"",""parameters"":[{""name"":""input"",""type"":""ByteArray""}],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""base58CheckDecode"",""parameters"":[{""name"":""input"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":8,""safe"":false},{""name"":""base64Decode"",""parameters"":[{""name"":""input"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":18,""safe"":false},{""name"":""base64Encode"",""parameters"":[{""name"":""input"",""type"":""ByteArray""}],""returntype"":""String"",""offset"":28,""safe"":false},{""name"":""base58Decode"",""parameters"":[{""name"":""input"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":38,""safe"":false},{""name"":""base58Encode"",""parameters"":[{""name"":""input"",""type"":""ByteArray""}],""returntype"":""String"",""offset"":48,""safe"":false},{""name"":""atoi"",""parameters"":[{""name"":""value"",""type"":""String""},{""name"":""base"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":58,""safe"":false},{""name"":""itoa"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""base"",""type"":""Integer""}],""returntype"":""String"",""offset"":67,""safe"":false},{""name"":""memoryCompare"",""parameters"":[{""name"":""str1"",""type"":""ByteArray""},{""name"":""str2"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":76,""safe"":false},{""name"":""memorySearch1"",""parameters"":[{""name"":""mem"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":85,""safe"":false},{""name"":""memorySearch2"",""parameters"":[{""name"":""mem"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""},{""name"":""start"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":94,""safe"":false},{""name"":""memorySearch3"",""parameters"":[{""name"":""mem"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""},{""name"":""start"",""type"":""Integer""},{""name"":""backward"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":104,""safe"":false},{""name"":""stringSplit1"",""parameters"":[{""name"":""str"",""type"":""String""},{""name"":""separator"",""type"":""String""}],""returntype"":""Array"",""offset"":115,""safe"":false},{""name"":""stringSplit2"",""parameters"":[{""name"":""str"",""type"":""String""},{""name"":""separator"",""type"":""String""},{""name"":""removeEmptyEntries"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":124,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi"",""base58CheckDecode"",""base58CheckEncode"",""base58Decode"",""base58Encode"",""base64Decode"",""base64Encode"",""itoa"",""memoryCompare"",""memorySearch"",""stringSplit""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA7A7znO4OTpJcbCoGp54UQN2G/OrBFiYXNlNThDaGVja0VuY29kZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrBFiYXNlNThDaGVja0RlY29kZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAxiYXNlNjREZWNvZGUBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwMYmFzZTY0RW5jb2RlAQABD8DvOc7g5OklxsKgannhRA3Yb86sDGJhc2U1OERlY29kZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAxiYXNlNThFbmNvZGUBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwEYXRvaQIAAQ/A7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAgABD8DvOc7g5OklxsKgannhRA3Yb86sDW1lbW9yeUNvbXBhcmUCAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwMbWVtb3J5U2VhcmNoAgABD8DvOc7g5OklxsKgannhRA3Yb86sDG1lbW9yeVNlYXJjaAMAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAxtZW1vcnlTZWFyY2gEAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwLc3RyaW5nU3BsaXQCAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwLc3RyaW5nU3BsaXQDAAEPAACGVwABeDcAAEBXAAF4NwEA2zBAVwABeDcCANswQFcAAXjbKDcDAEBXAAF4NwQA2zBAVwABeNsoNwUAQFcAAnl4NwYAQFcAAnl4NwcAQFcAAnl4NwgAQFcAAnl4NwkAQFcAA3p5eDcKAEBXAAR7enl4NwsAQFcAAnl4NwwAQFcAA3p5eDcNAEANwaHV"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg3BgBA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0600 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("atoi")]
    public abstract BigInteger? Atoi(string? value, BigInteger? @base);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcBANswQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("base58CheckDecode")]
    public abstract byte[]? Base58CheckDecode(string? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("base58CheckEncode")]
    public abstract string? Base58CheckEncode(byte[]? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcEANswQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0400 [32768 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("base58Decode")]
    public abstract byte[]? Base58Decode(string? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNsoNwUAQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0500 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("base58Encode")]
    public abstract string? Base58Encode(byte[]? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcCANswQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0200 [32768 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("base64Decode")]
    public abstract byte[]? Base64Decode(string? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNsoNwMAQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0300 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("base64Encode")]
    public abstract string? Base64Encode(byte[]? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg3BwBA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0700 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("itoa")]
    public abstract string? Itoa(BigInteger? value, BigInteger? @base);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg3CABA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0800 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("memoryCompare")]
    public abstract BigInteger? MemoryCompare(byte[]? str1, byte[]? str2);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg3CQBA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0900 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("memorySearch1")]
    public abstract BigInteger? MemorySearch1(byte[]? mem, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4NwoAQA==
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0A00 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("memorySearch2")]
    public abstract BigInteger? MemorySearch2(byte[]? mem, byte[]? value, BigInteger? start);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwAEe3p5eDcLAEA=
    /// INITSLOT 0004 [64 datoshi]
    /// LDARG3 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0B00 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("memorySearch3")]
    public abstract BigInteger? MemorySearch3(byte[]? mem, byte[]? value, BigInteger? start, bool? backward);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg3DABA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0C00 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("stringSplit1")]
    public abstract IList<object>? StringSplit1(string? str, string? separator);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4Nw0AQA==
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0D00 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("stringSplit2")]
    public abstract IList<object>? StringSplit2(string? str, string? separator, bool? removeEmptyEntries);

    #endregion
}
