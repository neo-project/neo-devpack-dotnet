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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAS+8gQxQDYqd8FQmcfmTBL3ALZl2ghnZXRCbG9jawEAAQ++8gQxQDYqd8FQmcfmTBL3ALZl2gtjdXJyZW50SGFzaAAAAQ/A7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABD8DvOc7g5OklxsKgannhRA3Yb86sDG1lbW9yeVNlYXJjaAIAAQ8AAP1UA1cDAAwETWFya3AMAHE3AQA3AAAUznIMB0hlbGxvLCBoiwwBIItpiwwXISBDdXJyZW50IHRpbWVzdGFtcCBpcyCLajcCAIsMAS6L2yhBz+dHlkBXAgAMBWhlbGxvcAwFaGVsbG9xaGmXJAsMBUZhbHNlIggMBFRydWVBz+dHlkBXAQAMCDAxMjM0NTY3cGgRS8pLn4xBz+dHlmgRFIxBz+dHlkAMAEBXAAF4StgkBsqxqkBFCEBXAAEMBXdvcmxkeErKUUrKShNSUJ9KECwIRUVFRQlAE1JTjNsol0BXAAEMBXdvcmxkeDcDABC4QFcAAQwFd29ybGR4NwMAQFcEAAQAAKDexa3JNTYAAAAAAAAAcAwiTlhWN1poSGl5TTFhSFh3cFZzUlpDNkJ3TkZQMmpnaFhBcXEMAwECA9swcgwHU0J5dGU6IADWNwIAiwwILCBCeXRlOiCLACo3AgCLDAosIFVTaG9ydDogiwHoAzcCAIsMAiwgi9soDAZVSW50OiACQEIPADcCAIsMCSwgVUxvbmc6IIsDABCl1OgAAAA3AgCLDAIsIIvbKIvbKAwMQmlnSW50ZWdlcjogaDcCAIsMCCwgQ2hhcjogiwBB2yiLDAosIFN0cmluZzogiwwFSGVsbG+LDAIsIIvbKIvbKAwJRUNQb2ludDogaYsMDiwgQnl0ZVN0cmluZzogiwwNU3lzdGVtLkJ5dGVbXYsMCCwgQm9vbDogiwgmCgwEVHJ1ZSIJDAVGYWxzZYvbKIvbKHNrQFcDAXjKcBBxaJ1yaWi1JhV4ac5KGR67UAAgl6wmB2mccSLqamm3JhV4as5KGR67UAAgl6wmB2qdciLqeGlqaZ+cjEBXAAJ4ec5AVwACeHlLykufjEBXAAJ5eErYJgVFDABQStgmBUUMAItAVwACeXg3AwBAVwABDAAQSnjKtSYiSnhQzkoAQQBbuyQJUVCLUJwi6QBBnwBhnlFQi1CcItxF2yhAVwABDAAQSnjKtSYiSnhQzkoAYQB7uyQJUVCLUJwi6QBhnwBBnlFQi1CcItxF2yhAVwMCeXjKcBBxaJ1yRWlotSYOeGnOebMmB2mccSLxamm3Jg54as55syYHap1yIvF4aWppn5yMQFcAAXjKQEia5Tw="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhK2CYFRQwAUErYJgVFDACLQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// CAT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testConcat")]
    public abstract string? TestConcat(string? s1, string? s2);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAV3b3JsZHg3AwAQuEA=
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 776F726C64 'world' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0300 [32768 datoshi]
    /// PUSH0 [1 datoshi]
    /// GE [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testContains")]
    public abstract bool? TestContains(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DABA
    /// PUSHDATA1 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEmpty")]
    public abstract string? TestEmpty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAV3b3JsZHhKylFKykoTUlCfShAsCEVFRUUJQBNSU4zbKJdA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 776F726C64 'world' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// ROLL [16 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// PUSH3 [1 datoshi]
    /// ROLL [16 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SUBSTR [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// EQUAL [32 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEndWith")]
    public abstract bool? TestEndWith(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADAVoZWxsb3AMBWhlbGxvcWhplyQLDAVGYWxzZSIIDARUcnVlQc/nR5ZA
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 68656C6C6F 'hello' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 68656C6C6F 'hello' [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 0B [2 datoshi]
    /// PUSHDATA1 46616C7365 'False' [8 datoshi]
    /// JMP 08 [2 datoshi]
    /// PUSHDATA1 54727565 'True' [8 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEqual")]
    public abstract void TestEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAV3b3JsZHg3AwBA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 776F726C64 'world' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0300 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIndexOf")]
    public abstract BigInteger? TestIndexOf(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg3AwBA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0300 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIndexOfChar")]
    public abstract BigInteger? TestIndexOfChar(string? s, BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQABAAAoN7Frck1NgAAAAAAAABwDCJOWFY3WmhIaXlNMWFIWHdwVnNSWkM2QndORlAyamdoWEFxcQwDAQID2zByDAdTQnl0ZTogANY3AgCLDAgsIEJ5dGU6IIsAKjcCAIsMCiwgVVNob3J0OiCLAegDNwIAiwwCLCCL2ygMBlVJbnQ6IAJAQg8ANwIAiwwJLCBVTG9uZzogiwMAEKXU6AAAADcCAIsMAiwgi9soi9soDAxCaWdJbnRlZ2VyOiBoNwIAiwwILCBDaGFyOiCLAEHbKIsMCiwgU3RyaW5nOiCLDAVIZWxsb4sMAiwgi9soi9soDAlFQ1BvaW50OiBpiwwOLCBCeXRlU3RyaW5nOiCLDA1TeXN0ZW0uQnl0ZVtdiwwILCBCb29sOiCLCCYKDARUcnVlIgkMBUZhbHNli9soi9soc2tA
    /// INITSLOT 0400 [64 datoshi]
    /// PUSHINT128 0000A0DEC5ADC9353600000000000000 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 4E5856375A684869794D3161485877705673525A433642774E4650326A6768584171 'NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq' [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSHDATA1 010203 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSHDATA1 53427974653A20 [8 datoshi]
    /// PUSHINT8 D6 [1 datoshi]
    /// CALLT 0200 [32768 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 2C20427974653A20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHINT8 2A [1 datoshi]
    /// CALLT 0200 [32768 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 2C205553686F72743A20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHINT16 E803 [1 datoshi]
    /// CALLT 0200 [32768 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 2C20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSHDATA1 55496E743A20 [8 datoshi]
    /// PUSHINT32 40420F00 [1 datoshi]
    /// CALLT 0200 [32768 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 2C20554C6F6E673A20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHINT64 0010A5D4E8000000 [1 datoshi]
    /// CALLT 0200 [32768 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 2C20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSHDATA1 426967496E74656765723A20 [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALLT 0200 [32768 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 2C20436861723A20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 2C20537472696E673A20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 48656C6C6F 'Hello' [8 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 2C20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSHDATA1 4543506F696E743A20 [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 2C2042797465537472696E673A20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 53797374656D2E427974655B5D 'System.Byte[]' [8 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 2C20426F6F6C3A20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHT [1 datoshi]
    /// JMPIFNOT 0A [2 datoshi]
    /// PUSHDATA1 54727565 'True' [8 datoshi]
    /// JMP 09 [2 datoshi]
    /// PUSHDATA1 46616C7365 'False' [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testInterpolatedStringHandler")]
    public abstract string? TestInterpolatedStringHandler();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAbKsapARQhA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// SIZE [4 datoshi]
    /// NZ [4 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIsNullOrEmpty")]
    public abstract bool? TestIsNullOrEmpty(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLength")]
    public abstract BigInteger? TestLength(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMADARNYXJrcAwAcTcBADcAABTOcgwHSGVsbG8sIGiLDAEgi2mLDBchIEN1cnJlbnQgdGltZXN0YW1wIGlzIItqNwIAiwwBLovbKEHP50eWQA==
    /// INITSLOT 0300 [64 datoshi]
    /// PUSHDATA1 4D61726B 'Mark' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSHDATA1 48656C6C6F2C20 [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 212043757272656E742074696D657374616D7020697320 [8 datoshi]
    /// CAT [2048 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CALLT 0200 [32768 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 2E '.' [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHnOQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPickItem")]
    public abstract BigInteger? TestPickItem(string? s, BigInteger? index);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAgwMTIzNDU2N3BoEUvKS5+MQc/nR5ZoERSMQc/nR5ZA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHDATA1 3031323334353637 '01234567' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// OVER [2 datoshi]
    /// SIZE [4 datoshi]
    /// OVER [2 datoshi]
    /// SUB [8 datoshi]
    /// SUBSTR [2048 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// SUBSTR [2048 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSubstring")]
    public abstract void TestSubstring();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHlLykufjEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// OVER [2 datoshi]
    /// SIZE [4 datoshi]
    /// OVER [2 datoshi]
    /// SUB [8 datoshi]
    /// SUBSTR [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSubstringToEnd")]
    public abstract string? TestSubstringToEnd(string? s, BigInteger? startIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAAQSnjKtSYiSnhQzkoAQQBbuyQJUVCLUJwi6QBBnwBhnlFQi1CcItxF2yhA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 22 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// PUSHINT8 5B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 09 [2 datoshi]
    /// ROT [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP E9 [2 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// SUB [8 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// ADD [8 datoshi]
    /// ROT [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP DC [2 datoshi]
    /// DROP [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testToLower")]
    public abstract string? TestToLower(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAAQSnjKtSYiSnhQzkoAYQB7uyQJUVCLUJwi6QBhnwBBnlFQi1CcItxF2yhA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 22 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 09 [2 datoshi]
    /// ROT [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP E9 [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// SUB [8 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// ADD [8 datoshi]
    /// ROT [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP DC [2 datoshi]
    /// DROP [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testToUpper")]
    public abstract string? TestToUpper(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeMpwEHFonXJpaLUmFXhpzkoZHrtQACCXrCYHaZxxIupqabcmFXhqzkoZHrtQACCXrCYHap1yIup4aWppn5yMQA==
    /// INITSLOT 0301 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DEC [4 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 15 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// DUP [2 datoshi]
    /// PUSH9 [1 datoshi]
    /// PUSH14 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// BOOLOR [8 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC1 [2 datoshi]
    /// JMP EA [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// GT [8 datoshi]
    /// JMPIFNOT 15 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// DUP [2 datoshi]
    /// PUSH9 [1 datoshi]
    /// PUSH14 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// BOOLOR [8 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// DEC [4 datoshi]
    /// STLOC2 [2 datoshi]
    /// JMP EA [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SUB [8 datoshi]
    /// INC [4 datoshi]
    /// SUBSTR [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testTrim")]
    public abstract string? TestTrim(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMCeXjKcBBxaJ1yRWlotSYOeGnOebMmB2mccSLxamm3Jg54as55syYHap1yIvF4aWppn5yMQA==
    /// INITSLOT 0302 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DEC [4 datoshi]
    /// STLOC2 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 0E [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC1 [2 datoshi]
    /// JMP F1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// GT [8 datoshi]
    /// JMPIFNOT 0E [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// DEC [4 datoshi]
    /// STLOC2 [2 datoshi]
    /// JMP F1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SUB [8 datoshi]
    /// INC [4 datoshi]
    /// SUBSTR [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testTrimChar")]
    public abstract string? TestTrimChar(string? s, BigInteger? trimChar);

    #endregion
}
