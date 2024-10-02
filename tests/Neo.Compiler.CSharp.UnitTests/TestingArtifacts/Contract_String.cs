using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_String(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_String"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testMain"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""testEqual"",""parameters"":[],""returntype"":""Void"",""offset"":82,""safe"":false},{""name"":""testSubstring"",""parameters"":[],""returntype"":""Void"",""offset"":127,""safe"":false},{""name"":""testEmpty"",""parameters"":[],""returntype"":""String"",""offset"":163,""safe"":false},{""name"":""testIsNullOrEmpty"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Boolean"",""offset"":166,""safe"":false},{""name"":""testEndWith"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Boolean"",""offset"":181,""safe"":false},{""name"":""testContains"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Boolean"",""offset"":220,""safe"":false},{""name"":""testIndexOf"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Integer"",""offset"":237,""safe"":false},{""name"":""testInterpolatedStringHandler"",""parameters"":[],""returntype"":""String"",""offset"":252,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa"",""memorySearch""]},{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""currentHash"",""getBlock""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAS+8gQxQDYqd8FQmcfmTBL3ALZl2ghnZXRCbG9jawEAAQ++8gQxQDYqd8FQmcfmTBL3ALZl2gtjdXJyZW50SGFzaAAAAQ/A7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABD8DvOc7g5OklxsKgannhRA3Yb86sDG1lbW9yeVNlYXJjaAIAAQ8AAP1DAlcDAAwETWFya3AMAHE3AQA3AAAUznIMB0hlbGxvLCBoiwwBIItpiwwXISBDdXJyZW50IHRpbWVzdGFtcCBpcyCLajcCAIsMAS6L2yhBz+dHlkBXAgAMBWhlbGxvcAwFaGVsbG9xaGmXJAsMBUZhbHNlIggMBFRydWVBz+dHlkBXAQAMCDAxMjM0NTY3cGgRS8pLn4xBz+dHlmgRFIxBz+dHlkAMAEBXAAF4StgkBsoQs0BFCEBXAAEMBXdvcmxkeErKUUrKShNSUJ9KECwIRUVFRQlAE1JTjNsol0BXAAEMBXdvcmxkeDcDABC4QFcAAQwFd29ybGR4NwMAQFcEAAQAAKDexa3JNTYAAAAAAAAAcAwiTlhWN1poSGl5TTFhSFh3cFZzUlpDNkJ3TkZQMmpnaFhBcXEMAwECA9swcgwHU0J5dGU6IADWNwIAiwwILCBCeXRlOiCLACo3AgCLDAosIFVTaG9ydDogiwHoAzcCAIsMAiwgi9soDAZVSW50OiACQEIPADcCAIsMCSwgVUxvbmc6IIsDABCl1OgAAAA3AgCLDAIsIIvbKIvbKAwMQmlnSW50ZWdlcjogaDcCAIsMCCwgQ2hhcjogiwBB2yiLDAosIFN0cmluZzogiwwFSGVsbG+LDAIsIIvbKIvbKAwJRUNQb2ludDogaYsMDiwgQnl0ZVN0cmluZzogiwwNU3lzdGVtLkJ5dGVbXYsMCCwgQm9vbDogiwgmCgwEVHJ1ZSIJDAVGYWxzZYvbKIvbKHNrQENTLRg="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testContains")]
    public abstract bool? TestContains(string? str);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 000A : LDARG0
    // 000B : CALLT
    // 000E : PUSH0
    // 000F : GE
    // 0010 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEmpty")]
    public abstract string? TestEmpty();
    // 0000 : PUSHDATA1
    // 0002 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEndWith")]
    public abstract bool? TestEndWith(string? str);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 000A : LDARG0
    // 000B : DUP
    // 000C : SIZE
    // 000D : ROT
    // 000E : DUP
    // 000F : SIZE
    // 0010 : DUP
    // 0011 : PUSH3
    // 0012 : ROLL
    // 0013 : SWAP
    // 0014 : SUB
    // 0015 : DUP
    // 0016 : PUSH0
    // 0017 : JMPGT
    // 0019 : DROP
    // 001A : DROP
    // 001B : DROP
    // 001C : DROP
    // 001D : PUSHF
    // 001E : RET
    // 001F : PUSH3
    // 0020 : ROLL
    // 0021 : REVERSE3
    // 0022 : SUBSTR
    // 0023 : CONVERT
    // 0025 : EQUAL
    // 0026 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEqual")]
    public abstract void TestEqual();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 000A : STLOC0
    // 000B : PUSHDATA1
    // 0012 : STLOC1
    // 0013 : LDLOC0
    // 0014 : LDLOC1
    // 0015 : EQUAL
    // 0016 : JMPIF
    // 0018 : PUSHDATA1
    // 001F : JMP
    // 0021 : PUSHDATA1
    // 0027 : SYSCALL
    // 002C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIndexOf")]
    public abstract BigInteger? TestIndexOf(string? str);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 000A : LDARG0
    // 000B : CALLT
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testInterpolatedStringHandler")]
    public abstract string? TestInterpolatedStringHandler();
    // 0000 : INITSLOT
    // 0003 : PUSHINT128
    // 0014 : STLOC0
    // 0015 : PUSHDATA1
    // 0039 : STLOC1
    // 003A : PUSHDATA1
    // 003F : CONVERT
    // 0041 : STLOC2
    // 0042 : PUSHDATA1
    // 004B : PUSHINT8
    // 004D : CALLT
    // 0050 : CAT
    // 0051 : PUSHDATA1
    // 005B : CAT
    // 005C : PUSHINT8
    // 005E : CALLT
    // 0061 : CAT
    // 0062 : PUSHDATA1
    // 006E : CAT
    // 006F : PUSHINT16
    // 0072 : CALLT
    // 0075 : CAT
    // 0076 : PUSHDATA1
    // 007A : CAT
    // 007B : CONVERT
    // 007D : PUSHDATA1
    // 0085 : PUSHINT32
    // 008A : CALLT
    // 008D : CAT
    // 008E : PUSHDATA1
    // 0099 : CAT
    // 009A : PUSHINT64
    // 00A3 : CALLT
    // 00A6 : CAT
    // 00A7 : PUSHDATA1
    // 00AB : CAT
    // 00AC : CONVERT
    // 00AE : CAT
    // 00AF : CONVERT
    // 00B1 : PUSHDATA1
    // 00BF : LDLOC0
    // 00C0 : CALLT
    // 00C3 : CAT
    // 00C4 : PUSHDATA1
    // 00CE : CAT
    // 00CF : PUSHINT8
    // 00D1 : CONVERT
    // 00D3 : CAT
    // 00D4 : PUSHDATA1
    // 00E0 : CAT
    // 00E1 : PUSHDATA1
    // 00E8 : CAT
    // 00E9 : PUSHDATA1
    // 00ED : CAT
    // 00EE : CONVERT
    // 00F0 : CAT
    // 00F1 : CONVERT
    // 00F3 : PUSHDATA1
    // 00FE : LDLOC1
    // 00FF : CAT
    // 0100 : PUSHDATA1
    // 0110 : CAT
    // 0111 : PUSHDATA1
    // 0120 : CAT
    // 0121 : PUSHDATA1
    // 012B : CAT
    // 012C : PUSHT
    // 012D : JMPIFNOT
    // 012F : PUSHDATA1
    // 0135 : JMP
    // 0137 : PUSHDATA1
    // 013E : CAT
    // 013F : CONVERT
    // 0141 : CAT
    // 0142 : CONVERT
    // 0144 : STLOC3
    // 0145 : LDLOC3
    // 0146 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIsNullOrEmpty")]
    public abstract bool? TestIsNullOrEmpty(string? str);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : ISNULL
    // 0006 : JMPIF
    // 0008 : SIZE
    // 0009 : PUSH0
    // 000A : NUMEQUAL
    // 000B : RET
    // 000C : DROP
    // 000D : PUSHT
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testMain")]
    public abstract void TestMain();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0009 : STLOC0
    // 000A : PUSHDATA1
    // 000C : STLOC1
    // 000D : CALLT
    // 0010 : CALLT
    // 0013 : PUSH4
    // 0014 : PICKITEM
    // 0015 : STLOC2
    // 0016 : PUSHDATA1
    // 001F : LDLOC0
    // 0020 : CAT
    // 0021 : PUSHDATA1
    // 0024 : CAT
    // 0025 : LDLOC1
    // 0026 : CAT
    // 0027 : PUSHDATA1
    // 0040 : CAT
    // 0041 : LDLOC2
    // 0042 : CALLT
    // 0045 : CAT
    // 0046 : PUSHDATA1
    // 0049 : CAT
    // 004A : CONVERT
    // 004C : SYSCALL
    // 0051 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSubstring")]
    public abstract void TestSubstring();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 000D : STLOC0
    // 000E : LDLOC0
    // 000F : PUSH1
    // 0010 : OVER
    // 0011 : SIZE
    // 0012 : OVER
    // 0013 : SUB
    // 0014 : SUBSTR
    // 0015 : SYSCALL
    // 001A : LDLOC0
    // 001B : PUSH1
    // 001C : PUSH4
    // 001D : SUBSTR
    // 001E : SYSCALL
    // 0023 : RET

    #endregion

}
