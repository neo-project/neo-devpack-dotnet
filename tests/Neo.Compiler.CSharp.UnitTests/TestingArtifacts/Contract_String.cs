using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_String(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_String"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testMain"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""testEqual"",""parameters"":[],""returntype"":""Void"",""offset"":82,""safe"":false},{""name"":""testSubstring"",""parameters"":[],""returntype"":""Void"",""offset"":127,""safe"":false},{""name"":""testEmpty"",""parameters"":[],""returntype"":""String"",""offset"":163,""safe"":false},{""name"":""testIsNullOrEmpty"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Boolean"",""offset"":166,""safe"":false},{""name"":""testEndWith"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Boolean"",""offset"":181,""safe"":false},{""name"":""testContains"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Boolean"",""offset"":220,""safe"":false},{""name"":""testIndexOf"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Integer"",""offset"":237,""safe"":false},{""name"":""testInterpolatedStringHandler"",""parameters"":[],""returntype"":""String"",""offset"":252,""safe"":false},{""name"":""testTrim"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""String"",""offset"":579,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa"",""memorySearch""]},{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""currentHash"",""getBlock""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAS+8gQxQDYqd8FQmcfmTBL3ALZl2ghnZXRCbG9jawEAAQ++8gQxQDYqd8FQmcfmTBL3ALZl2gtjdXJyZW50SGFzaAAAAQ/A7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABD8DvOc7g5OklxsKgannhRA3Yb86sDG1lbW9yeVNlYXJjaAIAAQ8AAP2GAlcDAAwETWFya3AMAHE3AQA3AAAUznIMB0hlbGxvLCBoiwwBIItpiwwXISBDdXJyZW50IHRpbWVzdGFtcCBpcyCLajcCAIsMAS6L2yhBz+dHlkBXAgAMBWhlbGxvcAwFaGVsbG9xaGmXJAsMBUZhbHNlIggMBFRydWVBz+dHlkBXAQAMCDAxMjM0NTY3cGgRS8pLn4xBz+dHlmgRFIxBz+dHlkAMAEBXAAF4StgkBsoQs0BFCEBXAAEMBXdvcmxkeErKUUrKShNSUJ9KECwIRUVFRQlAE1JTjNsol0BXAAEMBXdvcmxkeDcDABC4QFcAAQwFd29ybGR4NwMAQFcEAAQAAKDexa3JNTYAAAAAAAAAcAwiTlhWN1poSGl5TTFhSFh3cFZzUlpDNkJ3TkZQMmpnaFhBcXEMAwECA9swcgwHU0J5dGU6IADWNwIAiwwILCBCeXRlOiCLACo3AgCLDAosIFVTaG9ydDogiwHoAzcCAIsMAiwgi9soDAZVSW50OiACQEIPADcCAIsMCSwgVUxvbmc6IIsDABCl1OgAAAA3AgCLDAIsIIvbKIvbKAwMQmlnSW50ZWdlcjogaDcCAIsMCCwgQ2hhcjogiwBB2yiLDAosIFN0cmluZzogiwwFSGVsbG+LDAIsIIvbKIvbKAwJRUNQb2ludDogaYsMDiwgQnl0ZVN0cmluZzogiwwNU3lzdGVtLkJ5dGVbXYsMCCwgQm9vbDogiwgmCgwEVHJ1ZSIJDAVGYWxzZYvbKIvbKHNrQFcDAXjKcBBxaJ1yaWi1JhV4ac5KGR67UAAgl6wmB2mccSLqamm3JhV4as5KGR67UAAgl6wmB2qdciLqeGlqaZ+cjEAbrJCF"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDHdvcmxkeDcDABC4QA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHDATA1 776F726C64
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.CALLT 0300
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.GE
    /// 0010 : OpCode.RET
    /// </remarks>
    [DisplayName("testContains")]
    public abstract bool? TestContains(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DEA=
    /// 0000 : OpCode.PUSHDATA1
    /// 0002 : OpCode.RET
    /// </remarks>
    [DisplayName("testEmpty")]
    public abstract string? TestEmpty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDHdvcmxkeErKUUrKShNSUJ9KECwIRUVFRQlAE1JTjNsol0A=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHDATA1 776F726C64
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.DUP
    /// 000C : OpCode.SIZE
    /// 000D : OpCode.ROT
    /// 000E : OpCode.DUP
    /// 000F : OpCode.SIZE
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSH3
    /// 0012 : OpCode.ROLL
    /// 0013 : OpCode.SWAP
    /// 0014 : OpCode.SUB
    /// 0015 : OpCode.DUP
    /// 0016 : OpCode.PUSH0
    /// 0017 : OpCode.JMPGT 08
    /// 0019 : OpCode.DROP
    /// 001A : OpCode.DROP
    /// 001B : OpCode.DROP
    /// 001C : OpCode.DROP
    /// 001D : OpCode.PUSHF
    /// 001E : OpCode.RET
    /// 001F : OpCode.PUSH3
    /// 0020 : OpCode.ROLL
    /// 0021 : OpCode.REVERSE3
    /// 0022 : OpCode.SUBSTR
    /// 0023 : OpCode.CONVERT 28
    /// 0025 : OpCode.EQUAL
    /// 0026 : OpCode.RET
    /// </remarks>
    [DisplayName("testEndWith")]
    public abstract bool? TestEndWith(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADGhlbGxvcAxoZWxsb3FoaZckCwxGYWxzZSIIDFRydWVBz+dHlkA=
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSHDATA1 68656C6C6F
    /// 000A : OpCode.STLOC0
    /// 000B : OpCode.PUSHDATA1 68656C6C6F
    /// 0012 : OpCode.STLOC1
    /// 0013 : OpCode.LDLOC0
    /// 0014 : OpCode.LDLOC1
    /// 0015 : OpCode.EQUAL
    /// 0016 : OpCode.JMPIF 0B
    /// 0018 : OpCode.PUSHDATA1 46616C7365
    /// 001F : OpCode.JMP 08
    /// 0021 : OpCode.PUSHDATA1 54727565
    /// 0027 : OpCode.SYSCALL CFE74796
    /// 002C : OpCode.RET
    /// </remarks>
    [DisplayName("testEqual")]
    public abstract void TestEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDHdvcmxkeDcDAEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHDATA1 776F726C64
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.CALLT 0300
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("testIndexOf")]
    public abstract BigInteger? TestIndexOf(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQABAAAoN7Frck1NgAAAAAAAABwDE5YVjdaaEhpeU0xYUhYd3BWc1JaQzZCd05GUDJqZ2hYQXFxDAECA9swcgxTQnl0ZTogANY3AgCLDCwgQnl0ZTogiwAqNwIAiwwsIFVTaG9ydDogiwHoAzcCAIsMLCCL2ygMVUludDogAkBCDwA3AgCLDCwgVUxvbmc6IIsDABCl1OgAAAA3AgCLDCwgi9soi9soDEJpZ0ludGVnZXI6IGg3AgCLDCwgQ2hhcjogiwBB2yiLDCwgU3RyaW5nOiCLDEhlbGxviwwsIIvbKIvbKAxFQ1BvaW50OiBpiwwsIEJ5dGVTdHJpbmc6IIsMU3lzdGVtLkJ5dGVbXYsMLCBCb29sOiCLCCYKDFRydWUiCQxGYWxzZYvbKIvbKHNrQA==
    /// 0000 : OpCode.INITSLOT 0400
    /// 0003 : OpCode.PUSHINT128 0000A0DEC5ADC9353600000000000000
    /// 0014 : OpCode.STLOC0
    /// 0015 : OpCode.PUSHDATA1 4E5856375A684869794D3161485877705673525A433642774E4650326A6768584171
    /// 0039 : OpCode.STLOC1
    /// 003A : OpCode.PUSHDATA1 010203
    /// 003F : OpCode.CONVERT 30
    /// 0041 : OpCode.STLOC2
    /// 0042 : OpCode.PUSHDATA1 53427974653A20
    /// 004B : OpCode.PUSHINT8 D6
    /// 004D : OpCode.CALLT 0200
    /// 0050 : OpCode.CAT
    /// 0051 : OpCode.PUSHDATA1 2C20427974653A20
    /// 005B : OpCode.CAT
    /// 005C : OpCode.PUSHINT8 2A
    /// 005E : OpCode.CALLT 0200
    /// 0061 : OpCode.CAT
    /// 0062 : OpCode.PUSHDATA1 2C205553686F72743A20
    /// 006E : OpCode.CAT
    /// 006F : OpCode.PUSHINT16 E803
    /// 0072 : OpCode.CALLT 0200
    /// 0075 : OpCode.CAT
    /// 0076 : OpCode.PUSHDATA1 2C20
    /// 007A : OpCode.CAT
    /// 007B : OpCode.CONVERT 28
    /// 007D : OpCode.PUSHDATA1 55496E743A20
    /// 0085 : OpCode.PUSHINT32 40420F00
    /// 008A : OpCode.CALLT 0200
    /// 008D : OpCode.CAT
    /// 008E : OpCode.PUSHDATA1 2C20554C6F6E673A20
    /// 0099 : OpCode.CAT
    /// 009A : OpCode.PUSHINT64 0010A5D4E8000000
    /// 00A3 : OpCode.CALLT 0200
    /// 00A6 : OpCode.CAT
    /// 00A7 : OpCode.PUSHDATA1 2C20
    /// 00AB : OpCode.CAT
    /// 00AC : OpCode.CONVERT 28
    /// 00AE : OpCode.CAT
    /// 00AF : OpCode.CONVERT 28
    /// 00B1 : OpCode.PUSHDATA1 426967496E74656765723A20
    /// 00BF : OpCode.LDLOC0
    /// 00C0 : OpCode.CALLT 0200
    /// 00C3 : OpCode.CAT
    /// 00C4 : OpCode.PUSHDATA1 2C20436861723A20
    /// 00CE : OpCode.CAT
    /// 00CF : OpCode.PUSHINT8 41
    /// 00D1 : OpCode.CONVERT 28
    /// 00D3 : OpCode.CAT
    /// 00D4 : OpCode.PUSHDATA1 2C20537472696E673A20
    /// 00E0 : OpCode.CAT
    /// 00E1 : OpCode.PUSHDATA1 48656C6C6F
    /// 00E8 : OpCode.CAT
    /// 00E9 : OpCode.PUSHDATA1 2C20
    /// 00ED : OpCode.CAT
    /// 00EE : OpCode.CONVERT 28
    /// 00F0 : OpCode.CAT
    /// 00F1 : OpCode.CONVERT 28
    /// 00F3 : OpCode.PUSHDATA1 4543506F696E743A20
    /// 00FE : OpCode.LDLOC1
    /// 00FF : OpCode.CAT
    /// 0100 : OpCode.PUSHDATA1 2C2042797465537472696E673A20
    /// 0110 : OpCode.CAT
    /// 0111 : OpCode.PUSHDATA1 53797374656D2E427974655B5D
    /// 0120 : OpCode.CAT
    /// 0121 : OpCode.PUSHDATA1 2C20426F6F6C3A20
    /// 012B : OpCode.CAT
    /// 012C : OpCode.PUSHT
    /// 012D : OpCode.JMPIFNOT 0A
    /// 012F : OpCode.PUSHDATA1 54727565
    /// 0135 : OpCode.JMP 09
    /// 0137 : OpCode.PUSHDATA1 46616C7365
    /// 013E : OpCode.CAT
    /// 013F : OpCode.CONVERT 28
    /// 0141 : OpCode.CAT
    /// 0142 : OpCode.CONVERT 28
    /// 0144 : OpCode.STLOC3
    /// 0145 : OpCode.LDLOC3
    /// 0146 : OpCode.RET
    /// </remarks>
    [DisplayName("testInterpolatedStringHandler")]
    public abstract string? TestInterpolatedStringHandler();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAbKELNARQhA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 06
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.NUMEQUAL
    /// 000B : OpCode.RET
    /// 000C : OpCode.DROP
    /// 000D : OpCode.PUSHT
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("testIsNullOrEmpty")]
    public abstract bool? TestIsNullOrEmpty(string? str);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMADE1hcmtwDHE3AQA3AAAUznIMSGVsbG8sIGiLDCCLaYsMISBDdXJyZW50IHRpbWVzdGFtcCBpcyCLajcCAIsMLovbKEHP50eWQA==
    /// 0000 : OpCode.INITSLOT 0300
    /// 0003 : OpCode.PUSHDATA1 4D61726B
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.PUSHDATA1
    /// 000C : OpCode.STLOC1
    /// 000D : OpCode.CALLT 0100
    /// 0010 : OpCode.CALLT 0000
    /// 0013 : OpCode.PUSH4
    /// 0014 : OpCode.PICKITEM
    /// 0015 : OpCode.STLOC2
    /// 0016 : OpCode.PUSHDATA1 48656C6C6F2C20
    /// 001F : OpCode.LDLOC0
    /// 0020 : OpCode.CAT
    /// 0021 : OpCode.PUSHDATA1 20
    /// 0024 : OpCode.CAT
    /// 0025 : OpCode.LDLOC1
    /// 0026 : OpCode.CAT
    /// 0027 : OpCode.PUSHDATA1 212043757272656E742074696D657374616D7020697320
    /// 0040 : OpCode.CAT
    /// 0041 : OpCode.LDLOC2
    /// 0042 : OpCode.CALLT 0200
    /// 0045 : OpCode.CAT
    /// 0046 : OpCode.PUSHDATA1 2E
    /// 0049 : OpCode.CAT
    /// 004A : OpCode.CONVERT 28
    /// 004C : OpCode.SYSCALL CFE74796
    /// 0051 : OpCode.RET
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADDAxMjM0NTY3cGgRS8pLn4xBz+dHlmgRFIxBz+dHlkA=
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSHDATA1 3031323334353637
    /// 000D : OpCode.STLOC0
    /// 000E : OpCode.LDLOC0
    /// 000F : OpCode.PUSH1
    /// 0010 : OpCode.OVER
    /// 0011 : OpCode.SIZE
    /// 0012 : OpCode.OVER
    /// 0013 : OpCode.SUB
    /// 0014 : OpCode.SUBSTR
    /// 0015 : OpCode.SYSCALL CFE74796
    /// 001A : OpCode.LDLOC0
    /// 001B : OpCode.PUSH1
    /// 001C : OpCode.PUSH4
    /// 001D : OpCode.SUBSTR
    /// 001E : OpCode.SYSCALL CFE74796
    /// 0023 : OpCode.RET
    /// </remarks>
    [DisplayName("testSubstring")]
    public abstract void TestSubstring();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeMpwEHFonXJpaLUmFXhpzkoZHrtQACCXrCYHaZxxIupqabcmFXhqzkoZHrtQACCXrCYHap1yIup4aWppn5yMQA==
    /// 0000 : OpCode.INITSLOT 0301
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.SIZE
    /// 0005 : OpCode.STLOC0
    /// 0006 : OpCode.PUSH0
    /// 0007 : OpCode.STLOC1
    /// 0008 : OpCode.LDLOC0
    /// 0009 : OpCode.DEC
    /// 000A : OpCode.STLOC2
    /// 000B : OpCode.LDLOC1
    /// 000C : OpCode.LDLOC0
    /// 000D : OpCode.LT
    /// 000E : OpCode.JMPIFNOT 15
    /// 0010 : OpCode.LDARG0
    /// 0011 : OpCode.LDLOC1
    /// 0012 : OpCode.PICKITEM
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.PUSH9
    /// 0015 : OpCode.PUSH14
    /// 0016 : OpCode.WITHIN
    /// 0017 : OpCode.SWAP
    /// 0018 : OpCode.PUSHINT8 20
    /// 001A : OpCode.EQUAL
    /// 001B : OpCode.BOOLOR
    /// 001C : OpCode.JMPIFNOT 07
    /// 001E : OpCode.LDLOC1
    /// 001F : OpCode.INC
    /// 0020 : OpCode.STLOC1
    /// 0021 : OpCode.JMP EA
    /// 0023 : OpCode.LDLOC2
    /// 0024 : OpCode.LDLOC1
    /// 0025 : OpCode.GT
    /// 0026 : OpCode.JMPIFNOT 15
    /// 0028 : OpCode.LDARG0
    /// 0029 : OpCode.LDLOC2
    /// 002A : OpCode.PICKITEM
    /// 002B : OpCode.DUP
    /// 002C : OpCode.PUSH9
    /// 002D : OpCode.PUSH14
    /// 002E : OpCode.WITHIN
    /// 002F : OpCode.SWAP
    /// 0030 : OpCode.PUSHINT8 20
    /// 0032 : OpCode.EQUAL
    /// 0033 : OpCode.BOOLOR
    /// 0034 : OpCode.JMPIFNOT 07
    /// 0036 : OpCode.LDLOC2
    /// 0037 : OpCode.DEC
    /// 0038 : OpCode.STLOC2
    /// 0039 : OpCode.JMP EA
    /// 003B : OpCode.LDARG0
    /// 003C : OpCode.LDLOC1
    /// 003D : OpCode.LDLOC2
    /// 003E : OpCode.LDLOC1
    /// 003F : OpCode.SUB
    /// 0040 : OpCode.INC
    /// 0041 : OpCode.SUBSTR
    /// 0042 : OpCode.RET
    /// </remarks>
    [DisplayName("testTrim")]
    public abstract string? TestTrim(string? str);

    #endregion

}
