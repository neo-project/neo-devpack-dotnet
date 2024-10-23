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
    /// <remarks>
    /// Script: VwACeXhK2CYFRQxQStgmBUUMi0A=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG1	[2 datoshi]
    /// 04 : OpCode.LDARG0	[2 datoshi]
    /// 05 : OpCode.DUP	[2 datoshi]
    /// 06 : OpCode.ISNULL	[2 datoshi]
    /// 07 : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 09 : OpCode.DROP	[2 datoshi]
    /// 0A : OpCode.PUSHDATA1	[8 datoshi]
    /// 0C : OpCode.SWAP	[2 datoshi]
    /// 0D : OpCode.DUP	[2 datoshi]
    /// 0E : OpCode.ISNULL	[2 datoshi]
    /// 0F : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 11 : OpCode.DROP	[2 datoshi]
    /// 12 : OpCode.PUSHDATA1	[8 datoshi]
    /// 14 : OpCode.CAT	[2048 datoshi]
    /// 15 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testConcat")]
    public abstract string? TestConcat(string? s1, string? s2);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDHdvcmxkeDcDABC4QA==
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.PUSHDATA1 776F726C64	[8 datoshi]
    /// 0A : OpCode.LDARG0	[2 datoshi]
    /// 0B : OpCode.CALLT 0300	[32768 datoshi]
    /// 0E : OpCode.PUSH0	[1 datoshi]
    /// 0F : OpCode.GE	[8 datoshi]
    /// 10 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testContains")]
    public abstract bool? TestContains(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DEA=
    /// 00 : OpCode.PUSHDATA1	[8 datoshi]
    /// 02 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testEmpty")]
    public abstract string? TestEmpty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDHdvcmxkeErKUUrKShNSUJ9KECwIRUVFRQlAE1JTjNsol0A=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.PUSHDATA1 776F726C64	[8 datoshi]
    /// 0A : OpCode.LDARG0	[2 datoshi]
    /// 0B : OpCode.DUP	[2 datoshi]
    /// 0C : OpCode.SIZE	[4 datoshi]
    /// 0D : OpCode.ROT	[2 datoshi]
    /// 0E : OpCode.DUP	[2 datoshi]
    /// 0F : OpCode.SIZE	[4 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSH3	[1 datoshi]
    /// 12 : OpCode.ROLL	[16 datoshi]
    /// 13 : OpCode.SWAP	[2 datoshi]
    /// 14 : OpCode.SUB	[8 datoshi]
    /// 15 : OpCode.DUP	[2 datoshi]
    /// 16 : OpCode.PUSH0	[1 datoshi]
    /// 17 : OpCode.JMPGT 08	[2 datoshi]
    /// 19 : OpCode.DROP	[2 datoshi]
    /// 1A : OpCode.DROP	[2 datoshi]
    /// 1B : OpCode.DROP	[2 datoshi]
    /// 1C : OpCode.DROP	[2 datoshi]
    /// 1D : OpCode.PUSHF	[1 datoshi]
    /// 1E : OpCode.RET	[0 datoshi]
    /// 1F : OpCode.PUSH3	[1 datoshi]
    /// 20 : OpCode.ROLL	[16 datoshi]
    /// 21 : OpCode.REVERSE3	[2 datoshi]
    /// 22 : OpCode.SUBSTR	[2048 datoshi]
    /// 23 : OpCode.CONVERT 28	[8192 datoshi]
    /// 25 : OpCode.EQUAL	[32 datoshi]
    /// 26 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testEndWith")]
    public abstract bool? TestEndWith(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADGhlbGxvcAxoZWxsb3FoaZckCwxGYWxzZSIIDFRydWVBz+dHlkA=
    /// 00 : OpCode.INITSLOT 0200	[64 datoshi]
    /// 03 : OpCode.PUSHDATA1 68656C6C6F	[8 datoshi]
    /// 0A : OpCode.STLOC0	[2 datoshi]
    /// 0B : OpCode.PUSHDATA1 68656C6C6F	[8 datoshi]
    /// 12 : OpCode.STLOC1	[2 datoshi]
    /// 13 : OpCode.LDLOC0	[2 datoshi]
    /// 14 : OpCode.LDLOC1	[2 datoshi]
    /// 15 : OpCode.EQUAL	[32 datoshi]
    /// 16 : OpCode.JMPIF 0B	[2 datoshi]
    /// 18 : OpCode.PUSHDATA1 46616C7365	[8 datoshi]
    /// 1F : OpCode.JMP 08	[2 datoshi]
    /// 21 : OpCode.PUSHDATA1 54727565	[8 datoshi]
    /// 27 : OpCode.SYSCALL CFE74796	[System.Runtime.Log][32768 datoshi]
    /// 2C : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testEqual")]
    public abstract void TestEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDHdvcmxkeDcDAEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.PUSHDATA1 776F726C64	[8 datoshi]
    /// 0A : OpCode.LDARG0	[2 datoshi]
    /// 0B : OpCode.CALLT 0300	[32768 datoshi]
    /// 0E : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testIndexOf")]
    public abstract BigInteger? TestIndexOf(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg3AwBA
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG1	[2 datoshi]
    /// 04 : OpCode.LDARG0	[2 datoshi]
    /// 05 : OpCode.CALLT 0300	[32768 datoshi]
    /// 08 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testIndexOfChar")]
    public abstract BigInteger? TestIndexOfChar(string? s, BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQABAAAoN7Frck1NgAAAAAAAABwDE5YVjdaaEhpeU0xYUhYd3BWc1JaQzZCd05GUDJqZ2hYQXFxDAECA9swcgxTQnl0ZTogANY3AgCLDCwgQnl0ZTogiwAqNwIAiwwsIFVTaG9ydDogiwHoAzcCAIsMLCCL2ygMVUludDogAkBCDwA3AgCLDCwgVUxvbmc6IIsDABCl1OgAAAA3AgCLDCwgi9soi9soDEJpZ0ludGVnZXI6IGg3AgCLDCwgQ2hhcjogiwBB2yiLDCwgU3RyaW5nOiCLDEhlbGxviwwsIIvbKIvbKAxFQ1BvaW50OiBpiwwsIEJ5dGVTdHJpbmc6IIsMU3lzdGVtLkJ5dGVbXYsMLCBCb29sOiCLCCYKDFRydWUiCQxGYWxzZYvbKIvbKHNrQA==
    /// 0000 : OpCode.INITSLOT 0400	[64 datoshi]
    /// 0003 : OpCode.PUSHINT128 0000A0DEC5ADC9353600000000000000	[4 datoshi]
    /// 0014 : OpCode.STLOC0	[2 datoshi]
    /// 0015 : OpCode.PUSHDATA1 4E5856375A684869794D3161485877705673525A433642774E4650326A6768584171	[8 datoshi]
    /// 0039 : OpCode.STLOC1	[2 datoshi]
    /// 003A : OpCode.PUSHDATA1 010203	[8 datoshi]
    /// 003F : OpCode.CONVERT 30	[8192 datoshi]
    /// 0041 : OpCode.STLOC2	[2 datoshi]
    /// 0042 : OpCode.PUSHDATA1 53427974653A20	[8 datoshi]
    /// 004B : OpCode.PUSHINT8 D6	[1 datoshi]
    /// 004D : OpCode.CALLT 0200	[32768 datoshi]
    /// 0050 : OpCode.CAT	[2048 datoshi]
    /// 0051 : OpCode.PUSHDATA1 2C20427974653A20	[8 datoshi]
    /// 005B : OpCode.CAT	[2048 datoshi]
    /// 005C : OpCode.PUSHINT8 2A	[1 datoshi]
    /// 005E : OpCode.CALLT 0200	[32768 datoshi]
    /// 0061 : OpCode.CAT	[2048 datoshi]
    /// 0062 : OpCode.PUSHDATA1 2C205553686F72743A20	[8 datoshi]
    /// 006E : OpCode.CAT	[2048 datoshi]
    /// 006F : OpCode.PUSHINT16 E803	[1 datoshi]
    /// 0072 : OpCode.CALLT 0200	[32768 datoshi]
    /// 0075 : OpCode.CAT	[2048 datoshi]
    /// 0076 : OpCode.PUSHDATA1 2C20	[8 datoshi]
    /// 007A : OpCode.CAT	[2048 datoshi]
    /// 007B : OpCode.CONVERT 28	[8192 datoshi]
    /// 007D : OpCode.PUSHDATA1 55496E743A20	[8 datoshi]
    /// 0085 : OpCode.PUSHINT32 40420F00	[1 datoshi]
    /// 008A : OpCode.CALLT 0200	[32768 datoshi]
    /// 008D : OpCode.CAT	[2048 datoshi]
    /// 008E : OpCode.PUSHDATA1 2C20554C6F6E673A20	[8 datoshi]
    /// 0099 : OpCode.CAT	[2048 datoshi]
    /// 009A : OpCode.PUSHINT64 0010A5D4E8000000	[1 datoshi]
    /// 00A3 : OpCode.CALLT 0200	[32768 datoshi]
    /// 00A6 : OpCode.CAT	[2048 datoshi]
    /// 00A7 : OpCode.PUSHDATA1 2C20	[8 datoshi]
    /// 00AB : OpCode.CAT	[2048 datoshi]
    /// 00AC : OpCode.CONVERT 28	[8192 datoshi]
    /// 00AE : OpCode.CAT	[2048 datoshi]
    /// 00AF : OpCode.CONVERT 28	[8192 datoshi]
    /// 00B1 : OpCode.PUSHDATA1 426967496E74656765723A20	[8 datoshi]
    /// 00BF : OpCode.LDLOC0	[2 datoshi]
    /// 00C0 : OpCode.CALLT 0200	[32768 datoshi]
    /// 00C3 : OpCode.CAT	[2048 datoshi]
    /// 00C4 : OpCode.PUSHDATA1 2C20436861723A20	[8 datoshi]
    /// 00CE : OpCode.CAT	[2048 datoshi]
    /// 00CF : OpCode.PUSHINT8 41	[1 datoshi]
    /// 00D1 : OpCode.CONVERT 28	[8192 datoshi]
    /// 00D3 : OpCode.CAT	[2048 datoshi]
    /// 00D4 : OpCode.PUSHDATA1 2C20537472696E673A20	[8 datoshi]
    /// 00E0 : OpCode.CAT	[2048 datoshi]
    /// 00E1 : OpCode.PUSHDATA1 48656C6C6F	[8 datoshi]
    /// 00E8 : OpCode.CAT	[2048 datoshi]
    /// 00E9 : OpCode.PUSHDATA1 2C20	[8 datoshi]
    /// 00ED : OpCode.CAT	[2048 datoshi]
    /// 00EE : OpCode.CONVERT 28	[8192 datoshi]
    /// 00F0 : OpCode.CAT	[2048 datoshi]
    /// 00F1 : OpCode.CONVERT 28	[8192 datoshi]
    /// 00F3 : OpCode.PUSHDATA1 4543506F696E743A20	[8 datoshi]
    /// 00FE : OpCode.LDLOC1	[2 datoshi]
    /// 00FF : OpCode.CAT	[2048 datoshi]
    /// 0100 : OpCode.PUSHDATA1 2C2042797465537472696E673A20	[8 datoshi]
    /// 0110 : OpCode.CAT	[2048 datoshi]
    /// 0111 : OpCode.PUSHDATA1 53797374656D2E427974655B5D	[8 datoshi]
    /// 0120 : OpCode.CAT	[2048 datoshi]
    /// 0121 : OpCode.PUSHDATA1 2C20426F6F6C3A20	[8 datoshi]
    /// 012B : OpCode.CAT	[2048 datoshi]
    /// 012C : OpCode.PUSHT	[1 datoshi]
    /// 012D : OpCode.JMPIFNOT 0A	[2 datoshi]
    /// 012F : OpCode.PUSHDATA1 54727565	[8 datoshi]
    /// 0135 : OpCode.JMP 09	[2 datoshi]
    /// 0137 : OpCode.PUSHDATA1 46616C7365	[8 datoshi]
    /// 013E : OpCode.CAT	[2048 datoshi]
    /// 013F : OpCode.CONVERT 28	[8192 datoshi]
    /// 0141 : OpCode.CAT	[2048 datoshi]
    /// 0142 : OpCode.CONVERT 28	[8192 datoshi]
    /// 0144 : OpCode.STLOC3	[2 datoshi]
    /// 0145 : OpCode.LDLOC3	[2 datoshi]
    /// 0146 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testInterpolatedStringHandler")]
    public abstract string? TestInterpolatedStringHandler();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAbKELNARQhA
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIF 06	[2 datoshi]
    /// 08 : OpCode.SIZE	[4 datoshi]
    /// 09 : OpCode.PUSH0	[1 datoshi]
    /// 0A : OpCode.NUMEQUAL	[8 datoshi]
    /// 0B : OpCode.RET	[0 datoshi]
    /// 0C : OpCode.DROP	[2 datoshi]
    /// 0D : OpCode.PUSHT	[1 datoshi]
    /// 0E : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testIsNullOrEmpty")]
    public abstract bool? TestIsNullOrEmpty(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.SIZE	[4 datoshi]
    /// 05 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testLength")]
    public abstract BigInteger? TestLength(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMADE1hcmtwDHE3AQA3AAAUznIMSGVsbG8sIGiLDCCLaYsMISBDdXJyZW50IHRpbWVzdGFtcCBpcyCLajcCAIsMLovbKEHP50eWQA==
    /// 00 : OpCode.INITSLOT 0300	[64 datoshi]
    /// 03 : OpCode.PUSHDATA1 4D61726B	[8 datoshi]
    /// 09 : OpCode.STLOC0	[2 datoshi]
    /// 0A : OpCode.PUSHDATA1	[8 datoshi]
    /// 0C : OpCode.STLOC1	[2 datoshi]
    /// 0D : OpCode.CALLT 0100	[32768 datoshi]
    /// 10 : OpCode.CALLT 0000	[32768 datoshi]
    /// 13 : OpCode.PUSH4	[1 datoshi]
    /// 14 : OpCode.PICKITEM	[64 datoshi]
    /// 15 : OpCode.STLOC2	[2 datoshi]
    /// 16 : OpCode.PUSHDATA1 48656C6C6F2C20	[8 datoshi]
    /// 1F : OpCode.LDLOC0	[2 datoshi]
    /// 20 : OpCode.CAT	[2048 datoshi]
    /// 21 : OpCode.PUSHDATA1 20	[8 datoshi]
    /// 24 : OpCode.CAT	[2048 datoshi]
    /// 25 : OpCode.LDLOC1	[2 datoshi]
    /// 26 : OpCode.CAT	[2048 datoshi]
    /// 27 : OpCode.PUSHDATA1 212043757272656E742074696D657374616D7020697320	[8 datoshi]
    /// 40 : OpCode.CAT	[2048 datoshi]
    /// 41 : OpCode.LDLOC2	[2 datoshi]
    /// 42 : OpCode.CALLT 0200	[32768 datoshi]
    /// 45 : OpCode.CAT	[2048 datoshi]
    /// 46 : OpCode.PUSHDATA1 2E	[8 datoshi]
    /// 49 : OpCode.CAT	[2048 datoshi]
    /// 4A : OpCode.CONVERT 28	[8192 datoshi]
    /// 4C : OpCode.SYSCALL CFE74796	[System.Runtime.Log][32768 datoshi]
    /// 51 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHnOQA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.PICKITEM	[64 datoshi]
    /// 06 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testPickItem")]
    public abstract BigInteger? TestPickItem(string? s, BigInteger? index);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADDAxMjM0NTY3cGgRS8pLn4xBz+dHlmgRFIxBz+dHlkA=
    /// 00 : OpCode.INITSLOT 0100	[64 datoshi]
    /// 03 : OpCode.PUSHDATA1 3031323334353637	[8 datoshi]
    /// 0D : OpCode.STLOC0	[2 datoshi]
    /// 0E : OpCode.LDLOC0	[2 datoshi]
    /// 0F : OpCode.PUSH1	[1 datoshi]
    /// 10 : OpCode.OVER	[2 datoshi]
    /// 11 : OpCode.SIZE	[4 datoshi]
    /// 12 : OpCode.OVER	[2 datoshi]
    /// 13 : OpCode.SUB	[8 datoshi]
    /// 14 : OpCode.SUBSTR	[2048 datoshi]
    /// 15 : OpCode.SYSCALL CFE74796	[System.Runtime.Log][32768 datoshi]
    /// 1A : OpCode.LDLOC0	[2 datoshi]
    /// 1B : OpCode.PUSH1	[1 datoshi]
    /// 1C : OpCode.PUSH4	[1 datoshi]
    /// 1D : OpCode.SUBSTR	[2048 datoshi]
    /// 1E : OpCode.SYSCALL CFE74796	[System.Runtime.Log][32768 datoshi]
    /// 23 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testSubstring")]
    public abstract void TestSubstring();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHlLykufjEA=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.OVER	[2 datoshi]
    /// 06 : OpCode.SIZE	[4 datoshi]
    /// 07 : OpCode.OVER	[2 datoshi]
    /// 08 : OpCode.SUB	[8 datoshi]
    /// 09 : OpCode.SUBSTR	[2048 datoshi]
    /// 0A : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testSubstringToEnd")]
    public abstract string? TestSubstringToEnd(string? s, BigInteger? startIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDBBKeMq1JiJKeFDOSgBBAFu7JAlRUItQnCLpAEGfAGGeUVCLUJwi3EXbKEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.PUSHDATA1	[8 datoshi]
    /// 05 : OpCode.PUSH0	[1 datoshi]
    /// 06 : OpCode.DUP	[2 datoshi]
    /// 07 : OpCode.LDARG0	[2 datoshi]
    /// 08 : OpCode.SIZE	[4 datoshi]
    /// 09 : OpCode.LT	[8 datoshi]
    /// 0A : OpCode.JMPIFNOT 22	[2 datoshi]
    /// 0C : OpCode.DUP	[2 datoshi]
    /// 0D : OpCode.LDARG0	[2 datoshi]
    /// 0E : OpCode.SWAP	[2 datoshi]
    /// 0F : OpCode.PICKITEM	[64 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSHINT8 41	[1 datoshi]
    /// 13 : OpCode.PUSHINT8 5B	[1 datoshi]
    /// 15 : OpCode.WITHIN	[8 datoshi]
    /// 16 : OpCode.JMPIF 09	[2 datoshi]
    /// 18 : OpCode.ROT	[2 datoshi]
    /// 19 : OpCode.SWAP	[2 datoshi]
    /// 1A : OpCode.CAT	[2048 datoshi]
    /// 1B : OpCode.SWAP	[2 datoshi]
    /// 1C : OpCode.INC	[4 datoshi]
    /// 1D : OpCode.JMP E9	[2 datoshi]
    /// 1F : OpCode.PUSHINT8 41	[1 datoshi]
    /// 21 : OpCode.SUB	[8 datoshi]
    /// 22 : OpCode.PUSHINT8 61	[1 datoshi]
    /// 24 : OpCode.ADD	[8 datoshi]
    /// 25 : OpCode.ROT	[2 datoshi]
    /// 26 : OpCode.SWAP	[2 datoshi]
    /// 27 : OpCode.CAT	[2048 datoshi]
    /// 28 : OpCode.SWAP	[2 datoshi]
    /// 29 : OpCode.INC	[4 datoshi]
    /// 2A : OpCode.JMP DC	[2 datoshi]
    /// 2C : OpCode.DROP	[2 datoshi]
    /// 2D : OpCode.CONVERT 28	[8192 datoshi]
    /// 2F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testToLower")]
    public abstract string? TestToLower(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDBBKeMq1JiJKeFDOSgBhAHu7JAlRUItQnCLpAGGfAEGeUVCLUJwi3EXbKEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.PUSHDATA1	[8 datoshi]
    /// 05 : OpCode.PUSH0	[1 datoshi]
    /// 06 : OpCode.DUP	[2 datoshi]
    /// 07 : OpCode.LDARG0	[2 datoshi]
    /// 08 : OpCode.SIZE	[4 datoshi]
    /// 09 : OpCode.LT	[8 datoshi]
    /// 0A : OpCode.JMPIFNOT 22	[2 datoshi]
    /// 0C : OpCode.DUP	[2 datoshi]
    /// 0D : OpCode.LDARG0	[2 datoshi]
    /// 0E : OpCode.SWAP	[2 datoshi]
    /// 0F : OpCode.PICKITEM	[64 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSHINT8 61	[1 datoshi]
    /// 13 : OpCode.PUSHINT8 7B	[1 datoshi]
    /// 15 : OpCode.WITHIN	[8 datoshi]
    /// 16 : OpCode.JMPIF 09	[2 datoshi]
    /// 18 : OpCode.ROT	[2 datoshi]
    /// 19 : OpCode.SWAP	[2 datoshi]
    /// 1A : OpCode.CAT	[2048 datoshi]
    /// 1B : OpCode.SWAP	[2 datoshi]
    /// 1C : OpCode.INC	[4 datoshi]
    /// 1D : OpCode.JMP E9	[2 datoshi]
    /// 1F : OpCode.PUSHINT8 61	[1 datoshi]
    /// 21 : OpCode.SUB	[8 datoshi]
    /// 22 : OpCode.PUSHINT8 41	[1 datoshi]
    /// 24 : OpCode.ADD	[8 datoshi]
    /// 25 : OpCode.ROT	[2 datoshi]
    /// 26 : OpCode.SWAP	[2 datoshi]
    /// 27 : OpCode.CAT	[2048 datoshi]
    /// 28 : OpCode.SWAP	[2 datoshi]
    /// 29 : OpCode.INC	[4 datoshi]
    /// 2A : OpCode.JMP DC	[2 datoshi]
    /// 2C : OpCode.DROP	[2 datoshi]
    /// 2D : OpCode.CONVERT 28	[8192 datoshi]
    /// 2F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testToUpper")]
    public abstract string? TestToUpper(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeMpwEHFonXJpaLUmFXhpzkoZHrtQACCXrCYHaZxxIupqabcmFXhqzkoZHrtQACCXrCYHap1yIup4aWppn5yMQA==
    /// 00 : OpCode.INITSLOT 0301	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.SIZE	[4 datoshi]
    /// 05 : OpCode.STLOC0	[2 datoshi]
    /// 06 : OpCode.PUSH0	[1 datoshi]
    /// 07 : OpCode.STLOC1	[2 datoshi]
    /// 08 : OpCode.LDLOC0	[2 datoshi]
    /// 09 : OpCode.DEC	[4 datoshi]
    /// 0A : OpCode.STLOC2	[2 datoshi]
    /// 0B : OpCode.LDLOC1	[2 datoshi]
    /// 0C : OpCode.LDLOC0	[2 datoshi]
    /// 0D : OpCode.LT	[8 datoshi]
    /// 0E : OpCode.JMPIFNOT 15	[2 datoshi]
    /// 10 : OpCode.LDARG0	[2 datoshi]
    /// 11 : OpCode.LDLOC1	[2 datoshi]
    /// 12 : OpCode.PICKITEM	[64 datoshi]
    /// 13 : OpCode.DUP	[2 datoshi]
    /// 14 : OpCode.PUSH9	[1 datoshi]
    /// 15 : OpCode.PUSH14	[1 datoshi]
    /// 16 : OpCode.WITHIN	[8 datoshi]
    /// 17 : OpCode.SWAP	[2 datoshi]
    /// 18 : OpCode.PUSHINT8 20	[1 datoshi]
    /// 1A : OpCode.EQUAL	[32 datoshi]
    /// 1B : OpCode.BOOLOR	[8 datoshi]
    /// 1C : OpCode.JMPIFNOT 07	[2 datoshi]
    /// 1E : OpCode.LDLOC1	[2 datoshi]
    /// 1F : OpCode.INC	[4 datoshi]
    /// 20 : OpCode.STLOC1	[2 datoshi]
    /// 21 : OpCode.JMP EA	[2 datoshi]
    /// 23 : OpCode.LDLOC2	[2 datoshi]
    /// 24 : OpCode.LDLOC1	[2 datoshi]
    /// 25 : OpCode.GT	[8 datoshi]
    /// 26 : OpCode.JMPIFNOT 15	[2 datoshi]
    /// 28 : OpCode.LDARG0	[2 datoshi]
    /// 29 : OpCode.LDLOC2	[2 datoshi]
    /// 2A : OpCode.PICKITEM	[64 datoshi]
    /// 2B : OpCode.DUP	[2 datoshi]
    /// 2C : OpCode.PUSH9	[1 datoshi]
    /// 2D : OpCode.PUSH14	[1 datoshi]
    /// 2E : OpCode.WITHIN	[8 datoshi]
    /// 2F : OpCode.SWAP	[2 datoshi]
    /// 30 : OpCode.PUSHINT8 20	[1 datoshi]
    /// 32 : OpCode.EQUAL	[32 datoshi]
    /// 33 : OpCode.BOOLOR	[8 datoshi]
    /// 34 : OpCode.JMPIFNOT 07	[2 datoshi]
    /// 36 : OpCode.LDLOC2	[2 datoshi]
    /// 37 : OpCode.DEC	[4 datoshi]
    /// 38 : OpCode.STLOC2	[2 datoshi]
    /// 39 : OpCode.JMP EA	[2 datoshi]
    /// 3B : OpCode.LDARG0	[2 datoshi]
    /// 3C : OpCode.LDLOC1	[2 datoshi]
    /// 3D : OpCode.LDLOC2	[2 datoshi]
    /// 3E : OpCode.LDLOC1	[2 datoshi]
    /// 3F : OpCode.SUB	[8 datoshi]
    /// 40 : OpCode.INC	[4 datoshi]
    /// 41 : OpCode.SUBSTR	[2048 datoshi]
    /// 42 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testTrim")]
    public abstract string? TestTrim(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMCeXjKcBBxaJ1yRWlotSYOeGnOebMmB2mccSLxamm3Jg54as55syYHap1yIvF4aWppn5yMQA==
    /// 00 : OpCode.INITSLOT 0302	[64 datoshi]
    /// 03 : OpCode.LDARG1	[2 datoshi]
    /// 04 : OpCode.LDARG0	[2 datoshi]
    /// 05 : OpCode.SIZE	[4 datoshi]
    /// 06 : OpCode.STLOC0	[2 datoshi]
    /// 07 : OpCode.PUSH0	[1 datoshi]
    /// 08 : OpCode.STLOC1	[2 datoshi]
    /// 09 : OpCode.LDLOC0	[2 datoshi]
    /// 0A : OpCode.DEC	[4 datoshi]
    /// 0B : OpCode.STLOC2	[2 datoshi]
    /// 0C : OpCode.DROP	[2 datoshi]
    /// 0D : OpCode.LDLOC1	[2 datoshi]
    /// 0E : OpCode.LDLOC0	[2 datoshi]
    /// 0F : OpCode.LT	[8 datoshi]
    /// 10 : OpCode.JMPIFNOT 0E	[2 datoshi]
    /// 12 : OpCode.LDARG0	[2 datoshi]
    /// 13 : OpCode.LDLOC1	[2 datoshi]
    /// 14 : OpCode.PICKITEM	[64 datoshi]
    /// 15 : OpCode.LDARG1	[2 datoshi]
    /// 16 : OpCode.NUMEQUAL	[8 datoshi]
    /// 17 : OpCode.JMPIFNOT 07	[2 datoshi]
    /// 19 : OpCode.LDLOC1	[2 datoshi]
    /// 1A : OpCode.INC	[4 datoshi]
    /// 1B : OpCode.STLOC1	[2 datoshi]
    /// 1C : OpCode.JMP F1	[2 datoshi]
    /// 1E : OpCode.LDLOC2	[2 datoshi]
    /// 1F : OpCode.LDLOC1	[2 datoshi]
    /// 20 : OpCode.GT	[8 datoshi]
    /// 21 : OpCode.JMPIFNOT 0E	[2 datoshi]
    /// 23 : OpCode.LDARG0	[2 datoshi]
    /// 24 : OpCode.LDLOC2	[2 datoshi]
    /// 25 : OpCode.PICKITEM	[64 datoshi]
    /// 26 : OpCode.LDARG1	[2 datoshi]
    /// 27 : OpCode.NUMEQUAL	[8 datoshi]
    /// 28 : OpCode.JMPIFNOT 07	[2 datoshi]
    /// 2A : OpCode.LDLOC2	[2 datoshi]
    /// 2B : OpCode.DEC	[4 datoshi]
    /// 2C : OpCode.STLOC2	[2 datoshi]
    /// 2D : OpCode.JMP F1	[2 datoshi]
    /// 2F : OpCode.LDARG0	[2 datoshi]
    /// 30 : OpCode.LDLOC1	[2 datoshi]
    /// 31 : OpCode.LDLOC2	[2 datoshi]
    /// 32 : OpCode.LDLOC1	[2 datoshi]
    /// 33 : OpCode.SUB	[8 datoshi]
    /// 34 : OpCode.INC	[4 datoshi]
    /// 35 : OpCode.SUBSTR	[2048 datoshi]
    /// 36 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testTrimChar")]
    public abstract string? TestTrimChar(string? s, BigInteger? trimChar);

    #endregion
}
