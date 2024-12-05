using Neo.Cryptography.ECC;
using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAS+8gQxQDYqd8FQmcfmTBL3ALZl2ghnZXRCbG9jawEAAQ++8gQxQDYqd8FQmcfmTBL3ALZl2gtjdXJyZW50SGFzaAAAAQ/A7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABD8DvOc7g5OklxsKgannhRA3Yb86sDG1lbW9yeVNlYXJjaAIAAQ8AAP1UA1cDAAwETWFya3AMAHE3AQA3AAAUznIMB0hlbGxvLCBoiwwBIItpiwwXISBDdXJyZW50IHRpbWVzdGFtcCBpcyCLajcCAIsMAS6L2yhBz+dHlkBXAgAMBWhlbGxvcAwFaGVsbG9xaGmXJAsMBUZhbHNlIggMBFRydWVBz+dHlkBXAQAMCDAxMjM0NTY3cGgRS8pLn4xBz+dHlmgRFIxBz+dHlkAMAEBXAAF4StgkBsqxqkBFCEBXAAEMBXdvcmxkeErKUUrKShNSUJ9KECwIRUVFRQlAE1JTjNsol0BXAAEMBXdvcmxkeDcDABC4QFcAAQwFd29ybGR4NwMAQFcEAAQAAKDexa3JNTYAAAAAAAAAcAwiTlhWN1poSGl5TTFhSFh3cFZzUlpDNkJ3TkZQMmpnaFhBcXEMAwECA9swcgwHU0J5dGU6IADWNwIAiwwILCBCeXRlOiCLACo3AgCLDAosIFVTaG9ydDogiwHoAzcCAIsMAiwgi9soDAZVSW50OiACQEIPADcCAIsMCSwgVUxvbmc6IIsDABCl1OgAAAA3AgCLDAIsIIvbKIvbKAwMQmlnSW50ZWdlcjogaDcCAIsMCCwgQ2hhcjogiwBB2yiLDAosIFN0cmluZzogiwwFSGVsbG+LDAIsIIvbKIvbKAwJRUNQb2ludDogaYsMDiwgQnl0ZVN0cmluZzogiwwNU3lzdGVtLkJ5dGVbXYsMCCwgQm9vbDogiwgmCgwEVHJ1ZSIJDAVGYWxzZYvbKIvbKHNrQFcDAXjKcBBxaJ1yaWi1JhV4ac5KGR67UAAgl6wmB2mccSLqamm3JhV4as5KGR67UAAgl6wmB2qdciLqeGlqaZ+cjEBXAAJ4ec5AVwACeHlLykufjEBXAAJ5eErYJgVFDABQStgmBUUMAItAVwACeXg3AwBAVwABDAAQSnjKtSYiSnhQzkoAQQBbuyQJUVCLUJwi6QBBnwBhnlFQi1CcItxF2yhAVwABDAAQSnjKtSYiSnhQzkoAYQB7uyQJUVCLUJwi6QBhnwBBnlFQi1CcItxF2yhAVwMCeXjKcBBxaJ1yRWlotSYOeGnOebMmB2mccSLxamm3Jg54as55syYHap1yIvF4aWppn5yMQFcAAXjKQEia5Tw=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhK2CYFRQwAUErYJgVFDACLQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : ISNULL [2 datoshi]
    /// 07 : JMPIFNOT 05 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : PUSHDATA1 [8 datoshi]
    /// 0C : SWAP [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIFNOT 05 [2 datoshi]
    /// 11 : DROP [2 datoshi]
    /// 12 : PUSHDATA1 [8 datoshi]
    /// 14 : CAT [2048 datoshi]
    /// 15 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testConcat")]
    public abstract string? TestConcat(string? s1, string? s2);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAV3b3JsZHg3AwAQuEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHDATA1 776F726C64 'world' [8 datoshi]
    /// 0A : LDARG0 [2 datoshi]
    /// 0B : CALLT 0300 [32768 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : GE [8 datoshi]
    /// 10 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testContains")]
    public abstract bool? TestContains(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DABA
    /// 00 : PUSHDATA1 [8 datoshi]
    /// 02 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEmpty")]
    public abstract string? TestEmpty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAV3b3JsZHhKylFKykoTUlCfShAsCEVFRUUJQBNSU4zbKJdA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHDATA1 776F726C64 'world' [8 datoshi]
    /// 0A : LDARG0 [2 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : SIZE [4 datoshi]
    /// 0D : ROT [2 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : SIZE [4 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSH3 [1 datoshi]
    /// 12 : ROLL [16 datoshi]
    /// 13 : SWAP [2 datoshi]
    /// 14 : SUB [8 datoshi]
    /// 15 : DUP [2 datoshi]
    /// 16 : PUSH0 [1 datoshi]
    /// 17 : JMPGT 08 [2 datoshi]
    /// 19 : DROP [2 datoshi]
    /// 1A : DROP [2 datoshi]
    /// 1B : DROP [2 datoshi]
    /// 1C : DROP [2 datoshi]
    /// 1D : PUSHF [1 datoshi]
    /// 1E : RET [0 datoshi]
    /// 1F : PUSH3 [1 datoshi]
    /// 20 : ROLL [16 datoshi]
    /// 21 : REVERSE3 [2 datoshi]
    /// 22 : SUBSTR [2048 datoshi]
    /// 23 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 25 : EQUAL [32 datoshi]
    /// 26 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEndWith")]
    public abstract bool? TestEndWith(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADAVoZWxsb3AMBWhlbGxvcWhplyQLDAVGYWxzZSIIDARUcnVlQc/nR5ZA
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHDATA1 68656C6C6F 'hello' [8 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : PUSHDATA1 68656C6C6F 'hello' [8 datoshi]
    /// 12 : STLOC1 [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : LDLOC1 [2 datoshi]
    /// 15 : EQUAL [32 datoshi]
    /// 16 : JMPIF 0B [2 datoshi]
    /// 18 : PUSHDATA1 46616C7365 'False' [8 datoshi]
    /// 1F : JMP 08 [2 datoshi]
    /// 21 : PUSHDATA1 54727565 'True' [8 datoshi]
    /// 27 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 2C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEqual")]
    public abstract void TestEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAV3b3JsZHg3AwBA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHDATA1 776F726C64 'world' [8 datoshi]
    /// 0A : LDARG0 [2 datoshi]
    /// 0B : CALLT 0300 [32768 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIndexOf")]
    public abstract BigInteger? TestIndexOf(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg3AwBA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : CALLT 0300 [32768 datoshi]
    /// 08 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIndexOfChar")]
    public abstract BigInteger? TestIndexOfChar(string? s, BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQABAAAoN7Frck1NgAAAAAAAABwDCJOWFY3WmhIaXlNMWFIWHdwVnNSWkM2QndORlAyamdoWEFxcQwDAQID2zByDAdTQnl0ZTogANY3AgCLDAgsIEJ5dGU6IIsAKjcCAIsMCiwgVVNob3J0OiCLAegDNwIAiwwCLCCL2ygMBlVJbnQ6IAJAQg8ANwIAiwwJLCBVTG9uZzogiwMAEKXU6AAAADcCAIsMAiwgi9soi9soDAxCaWdJbnRlZ2VyOiBoNwIAiwwILCBDaGFyOiCLAEHbKIsMCiwgU3RyaW5nOiCLDAVIZWxsb4sMAiwgi9soi9soDAlFQ1BvaW50OiBpiwwOLCBCeXRlU3RyaW5nOiCLDA1TeXN0ZW0uQnl0ZVtdiwwILCBCb29sOiCLCCYKDARUcnVlIgkMBUZhbHNli9soi9soc2tA
    /// 0000 : INITSLOT 0400 [64 datoshi]
    /// 0003 : PUSHINT128 0000A0DEC5ADC9353600000000000000 [4 datoshi]
    /// 0014 : STLOC0 [2 datoshi]
    /// 0015 : PUSHDATA1 4E5856375A684869794D3161485877705673525A433642774E4650326A6768584171 'NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq' [8 datoshi]
    /// 0039 : STLOC1 [2 datoshi]
    /// 003A : PUSHDATA1 010203 [8 datoshi]
    /// 003F : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0041 : STLOC2 [2 datoshi]
    /// 0042 : PUSHDATA1 53427974653A20 [8 datoshi]
    /// 004B : PUSHINT8 D6 [1 datoshi]
    /// 004D : CALLT 0200 [32768 datoshi]
    /// 0050 : CAT [2048 datoshi]
    /// 0051 : PUSHDATA1 2C20427974653A20 [8 datoshi]
    /// 005B : CAT [2048 datoshi]
    /// 005C : PUSHINT8 2A [1 datoshi]
    /// 005E : CALLT 0200 [32768 datoshi]
    /// 0061 : CAT [2048 datoshi]
    /// 0062 : PUSHDATA1 2C205553686F72743A20 [8 datoshi]
    /// 006E : CAT [2048 datoshi]
    /// 006F : PUSHINT16 E803 [1 datoshi]
    /// 0072 : CALLT 0200 [32768 datoshi]
    /// 0075 : CAT [2048 datoshi]
    /// 0076 : PUSHDATA1 2C20 [8 datoshi]
    /// 007A : CAT [2048 datoshi]
    /// 007B : CONVERT 28 'ByteString' [8192 datoshi]
    /// 007D : PUSHDATA1 55496E743A20 [8 datoshi]
    /// 0085 : PUSHINT32 40420F00 [1 datoshi]
    /// 008A : CALLT 0200 [32768 datoshi]
    /// 008D : CAT [2048 datoshi]
    /// 008E : PUSHDATA1 2C20554C6F6E673A20 [8 datoshi]
    /// 0099 : CAT [2048 datoshi]
    /// 009A : PUSHINT64 0010A5D4E8000000 [1 datoshi]
    /// 00A3 : CALLT 0200 [32768 datoshi]
    /// 00A6 : CAT [2048 datoshi]
    /// 00A7 : PUSHDATA1 2C20 [8 datoshi]
    /// 00AB : CAT [2048 datoshi]
    /// 00AC : CONVERT 28 'ByteString' [8192 datoshi]
    /// 00AE : CAT [2048 datoshi]
    /// 00AF : CONVERT 28 'ByteString' [8192 datoshi]
    /// 00B1 : PUSHDATA1 426967496E74656765723A20 [8 datoshi]
    /// 00BF : LDLOC0 [2 datoshi]
    /// 00C0 : CALLT 0200 [32768 datoshi]
    /// 00C3 : CAT [2048 datoshi]
    /// 00C4 : PUSHDATA1 2C20436861723A20 [8 datoshi]
    /// 00CE : CAT [2048 datoshi]
    /// 00CF : PUSHINT8 41 [1 datoshi]
    /// 00D1 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 00D3 : CAT [2048 datoshi]
    /// 00D4 : PUSHDATA1 2C20537472696E673A20 [8 datoshi]
    /// 00E0 : CAT [2048 datoshi]
    /// 00E1 : PUSHDATA1 48656C6C6F 'Hello' [8 datoshi]
    /// 00E8 : CAT [2048 datoshi]
    /// 00E9 : PUSHDATA1 2C20 [8 datoshi]
    /// 00ED : CAT [2048 datoshi]
    /// 00EE : CONVERT 28 'ByteString' [8192 datoshi]
    /// 00F0 : CAT [2048 datoshi]
    /// 00F1 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 00F3 : PUSHDATA1 4543506F696E743A20 [8 datoshi]
    /// 00FE : LDLOC1 [2 datoshi]
    /// 00FF : CAT [2048 datoshi]
    /// 0100 : PUSHDATA1 2C2042797465537472696E673A20 [8 datoshi]
    /// 0110 : CAT [2048 datoshi]
    /// 0111 : PUSHDATA1 53797374656D2E427974655B5D 'System.Byte[]' [8 datoshi]
    /// 0120 : CAT [2048 datoshi]
    /// 0121 : PUSHDATA1 2C20426F6F6C3A20 [8 datoshi]
    /// 012B : CAT [2048 datoshi]
    /// 012C : PUSHT [1 datoshi]
    /// 012D : JMPIFNOT 0A [2 datoshi]
    /// 012F : PUSHDATA1 54727565 'True' [8 datoshi]
    /// 0135 : JMP 09 [2 datoshi]
    /// 0137 : PUSHDATA1 46616C7365 'False' [8 datoshi]
    /// 013E : CAT [2048 datoshi]
    /// 013F : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0141 : CAT [2048 datoshi]
    /// 0142 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0144 : STLOC3 [2 datoshi]
    /// 0145 : LDLOC3 [2 datoshi]
    /// 0146 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testInterpolatedStringHandler")]
    public abstract string? TestInterpolatedStringHandler();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAbKsapARQhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 06 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : NZ [4 datoshi]
    /// 0A : NOT [4 datoshi]
    /// 0B : RET [0 datoshi]
    /// 0C : DROP [2 datoshi]
    /// 0D : PUSHT [1 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIsNullOrEmpty")]
    public abstract bool? TestIsNullOrEmpty(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SIZE [4 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLength")]
    public abstract BigInteger? TestLength(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMADARNYXJrcAwAcTcBADcAABTOcgwHSGVsbG8sIGiLDAEgi2mLDBchIEN1cnJlbnQgdGltZXN0YW1wIGlzIItqNwIAiwwBLovbKEHP50eWQA==
    /// 00 : INITSLOT 0300 [64 datoshi]
    /// 03 : PUSHDATA1 4D61726B 'Mark' [8 datoshi]
    /// 09 : STLOC0 [2 datoshi]
    /// 0A : PUSHDATA1 [8 datoshi]
    /// 0C : STLOC1 [2 datoshi]
    /// 0D : CALLT 0100 [32768 datoshi]
    /// 10 : CALLT 0000 [32768 datoshi]
    /// 13 : PUSH4 [1 datoshi]
    /// 14 : PICKITEM [64 datoshi]
    /// 15 : STLOC2 [2 datoshi]
    /// 16 : PUSHDATA1 48656C6C6F2C20 [8 datoshi]
    /// 1F : LDLOC0 [2 datoshi]
    /// 20 : CAT [2048 datoshi]
    /// 21 : PUSHDATA1 20 [8 datoshi]
    /// 24 : CAT [2048 datoshi]
    /// 25 : LDLOC1 [2 datoshi]
    /// 26 : CAT [2048 datoshi]
    /// 27 : PUSHDATA1 212043757272656E742074696D657374616D7020697320 [8 datoshi]
    /// 40 : CAT [2048 datoshi]
    /// 41 : LDLOC2 [2 datoshi]
    /// 42 : CALLT 0200 [32768 datoshi]
    /// 45 : CAT [2048 datoshi]
    /// 46 : PUSHDATA1 2E '.' [8 datoshi]
    /// 49 : CAT [2048 datoshi]
    /// 4A : CONVERT 28 'ByteString' [8192 datoshi]
    /// 4C : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 51 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHnOQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PICKITEM [64 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPickItem")]
    public abstract BigInteger? TestPickItem(string? s, BigInteger? index);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAgwMTIzNDU2N3BoEUvKS5+MQc/nR5ZoERSMQc/nR5ZA
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHDATA1 3031323334353637 '01234567' [8 datoshi]
    /// 0D : STLOC0 [2 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : PUSH1 [1 datoshi]
    /// 10 : OVER [2 datoshi]
    /// 11 : SIZE [4 datoshi]
    /// 12 : OVER [2 datoshi]
    /// 13 : SUB [8 datoshi]
    /// 14 : SUBSTR [2048 datoshi]
    /// 15 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 1A : LDLOC0 [2 datoshi]
    /// 1B : PUSH1 [1 datoshi]
    /// 1C : PUSH4 [1 datoshi]
    /// 1D : SUBSTR [2048 datoshi]
    /// 1E : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 23 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSubstring")]
    public abstract void TestSubstring();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHlLykufjEA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : OVER [2 datoshi]
    /// 06 : SIZE [4 datoshi]
    /// 07 : OVER [2 datoshi]
    /// 08 : SUB [8 datoshi]
    /// 09 : SUBSTR [2048 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSubstringToEnd")]
    public abstract string? TestSubstringToEnd(string? s, BigInteger? startIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAAQSnjKtSYiSnhQzkoAQQBbuyQJUVCLUJwi6QBBnwBhnlFQi1CcItxF2yhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHDATA1 [8 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : LDARG0 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : LT [8 datoshi]
    /// 0A : JMPIFNOT 22 [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : LDARG0 [2 datoshi]
    /// 0E : SWAP [2 datoshi]
    /// 0F : PICKITEM [64 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT8 41 [1 datoshi]
    /// 13 : PUSHINT8 5B [1 datoshi]
    /// 15 : WITHIN [8 datoshi]
    /// 16 : JMPIF 09 [2 datoshi]
    /// 18 : ROT [2 datoshi]
    /// 19 : SWAP [2 datoshi]
    /// 1A : CAT [2048 datoshi]
    /// 1B : SWAP [2 datoshi]
    /// 1C : INC [4 datoshi]
    /// 1D : JMP E9 [2 datoshi]
    /// 1F : PUSHINT8 41 [1 datoshi]
    /// 21 : SUB [8 datoshi]
    /// 22 : PUSHINT8 61 [1 datoshi]
    /// 24 : ADD [8 datoshi]
    /// 25 : ROT [2 datoshi]
    /// 26 : SWAP [2 datoshi]
    /// 27 : CAT [2048 datoshi]
    /// 28 : SWAP [2 datoshi]
    /// 29 : INC [4 datoshi]
    /// 2A : JMP DC [2 datoshi]
    /// 2C : DROP [2 datoshi]
    /// 2D : CONVERT 28 'ByteString' [8192 datoshi]
    /// 2F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testToLower")]
    public abstract string? TestToLower(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAAQSnjKtSYiSnhQzkoAYQB7uyQJUVCLUJwi6QBhnwBBnlFQi1CcItxF2yhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHDATA1 [8 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : LDARG0 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : LT [8 datoshi]
    /// 0A : JMPIFNOT 22 [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : LDARG0 [2 datoshi]
    /// 0E : SWAP [2 datoshi]
    /// 0F : PICKITEM [64 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT8 61 [1 datoshi]
    /// 13 : PUSHINT8 7B [1 datoshi]
    /// 15 : WITHIN [8 datoshi]
    /// 16 : JMPIF 09 [2 datoshi]
    /// 18 : ROT [2 datoshi]
    /// 19 : SWAP [2 datoshi]
    /// 1A : CAT [2048 datoshi]
    /// 1B : SWAP [2 datoshi]
    /// 1C : INC [4 datoshi]
    /// 1D : JMP E9 [2 datoshi]
    /// 1F : PUSHINT8 61 [1 datoshi]
    /// 21 : SUB [8 datoshi]
    /// 22 : PUSHINT8 41 [1 datoshi]
    /// 24 : ADD [8 datoshi]
    /// 25 : ROT [2 datoshi]
    /// 26 : SWAP [2 datoshi]
    /// 27 : CAT [2048 datoshi]
    /// 28 : SWAP [2 datoshi]
    /// 29 : INC [4 datoshi]
    /// 2A : JMP DC [2 datoshi]
    /// 2C : DROP [2 datoshi]
    /// 2D : CONVERT 28 'ByteString' [8192 datoshi]
    /// 2F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testToUpper")]
    public abstract string? TestToUpper(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeMpwEHFonXJpaLUmFXhpzkoZHrtQACCXrCYHaZxxIupqabcmFXhqzkoZHrtQACCXrCYHap1yIup4aWppn5yMQA==
    /// 00 : INITSLOT 0301 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SIZE [4 datoshi]
    /// 05 : STLOC0 [2 datoshi]
    /// 06 : PUSH0 [1 datoshi]
    /// 07 : STLOC1 [2 datoshi]
    /// 08 : LDLOC0 [2 datoshi]
    /// 09 : DEC [4 datoshi]
    /// 0A : STLOC2 [2 datoshi]
    /// 0B : LDLOC1 [2 datoshi]
    /// 0C : LDLOC0 [2 datoshi]
    /// 0D : LT [8 datoshi]
    /// 0E : JMPIFNOT 15 [2 datoshi]
    /// 10 : LDARG0 [2 datoshi]
    /// 11 : LDLOC1 [2 datoshi]
    /// 12 : PICKITEM [64 datoshi]
    /// 13 : DUP [2 datoshi]
    /// 14 : PUSH9 [1 datoshi]
    /// 15 : PUSH14 [1 datoshi]
    /// 16 : WITHIN [8 datoshi]
    /// 17 : SWAP [2 datoshi]
    /// 18 : PUSHINT8 20 [1 datoshi]
    /// 1A : EQUAL [32 datoshi]
    /// 1B : BOOLOR [8 datoshi]
    /// 1C : JMPIFNOT 07 [2 datoshi]
    /// 1E : LDLOC1 [2 datoshi]
    /// 1F : INC [4 datoshi]
    /// 20 : STLOC1 [2 datoshi]
    /// 21 : JMP EA [2 datoshi]
    /// 23 : LDLOC2 [2 datoshi]
    /// 24 : LDLOC1 [2 datoshi]
    /// 25 : GT [8 datoshi]
    /// 26 : JMPIFNOT 15 [2 datoshi]
    /// 28 : LDARG0 [2 datoshi]
    /// 29 : LDLOC2 [2 datoshi]
    /// 2A : PICKITEM [64 datoshi]
    /// 2B : DUP [2 datoshi]
    /// 2C : PUSH9 [1 datoshi]
    /// 2D : PUSH14 [1 datoshi]
    /// 2E : WITHIN [8 datoshi]
    /// 2F : SWAP [2 datoshi]
    /// 30 : PUSHINT8 20 [1 datoshi]
    /// 32 : EQUAL [32 datoshi]
    /// 33 : BOOLOR [8 datoshi]
    /// 34 : JMPIFNOT 07 [2 datoshi]
    /// 36 : LDLOC2 [2 datoshi]
    /// 37 : DEC [4 datoshi]
    /// 38 : STLOC2 [2 datoshi]
    /// 39 : JMP EA [2 datoshi]
    /// 3B : LDARG0 [2 datoshi]
    /// 3C : LDLOC1 [2 datoshi]
    /// 3D : LDLOC2 [2 datoshi]
    /// 3E : LDLOC1 [2 datoshi]
    /// 3F : SUB [8 datoshi]
    /// 40 : INC [4 datoshi]
    /// 41 : SUBSTR [2048 datoshi]
    /// 42 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testTrim")]
    public abstract string? TestTrim(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMCeXjKcBBxaJ1yRWlotSYOeGnOebMmB2mccSLxamm3Jg54as55syYHap1yIvF4aWppn5yMQA==
    /// 00 : INITSLOT 0302 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : SIZE [4 datoshi]
    /// 06 : STLOC0 [2 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : STLOC1 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : DEC [4 datoshi]
    /// 0B : STLOC2 [2 datoshi]
    /// 0C : DROP [2 datoshi]
    /// 0D : LDLOC1 [2 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : LT [8 datoshi]
    /// 10 : JMPIFNOT 0E [2 datoshi]
    /// 12 : LDARG0 [2 datoshi]
    /// 13 : LDLOC1 [2 datoshi]
    /// 14 : PICKITEM [64 datoshi]
    /// 15 : LDARG1 [2 datoshi]
    /// 16 : NUMEQUAL [8 datoshi]
    /// 17 : JMPIFNOT 07 [2 datoshi]
    /// 19 : LDLOC1 [2 datoshi]
    /// 1A : INC [4 datoshi]
    /// 1B : STLOC1 [2 datoshi]
    /// 1C : JMP F1 [2 datoshi]
    /// 1E : LDLOC2 [2 datoshi]
    /// 1F : LDLOC1 [2 datoshi]
    /// 20 : GT [8 datoshi]
    /// 21 : JMPIFNOT 0E [2 datoshi]
    /// 23 : LDARG0 [2 datoshi]
    /// 24 : LDLOC2 [2 datoshi]
    /// 25 : PICKITEM [64 datoshi]
    /// 26 : LDARG1 [2 datoshi]
    /// 27 : NUMEQUAL [8 datoshi]
    /// 28 : JMPIFNOT 07 [2 datoshi]
    /// 2A : LDLOC2 [2 datoshi]
    /// 2B : DEC [4 datoshi]
    /// 2C : STLOC2 [2 datoshi]
    /// 2D : JMP F1 [2 datoshi]
    /// 2F : LDARG0 [2 datoshi]
    /// 30 : LDLOC1 [2 datoshi]
    /// 31 : LDLOC2 [2 datoshi]
    /// 32 : LDLOC1 [2 datoshi]
    /// 33 : SUB [8 datoshi]
    /// 34 : INC [4 datoshi]
    /// 35 : SUBSTR [2048 datoshi]
    /// 36 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testTrimChar")]
    public abstract string? TestTrimChar(string? s, BigInteger? trimChar);

    #endregion
}
