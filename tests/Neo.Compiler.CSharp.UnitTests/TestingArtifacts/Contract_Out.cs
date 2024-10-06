using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Out(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Out"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testOutVar"",""parameters"":[],""returntype"":""Integer"",""offset"":23,""safe"":false},{""name"":""testExistingVar"",""parameters"":[],""returntype"":""Integer"",""offset"":30,""safe"":false},{""name"":""testMultipleOut"",""parameters"":[],""returntype"":""String"",""offset"":40,""safe"":false},{""name"":""testOutDiscard"",""parameters"":[],""returntype"":""Void"",""offset"":89,""safe"":false},{""name"":""testOutInLoop"",""parameters"":[],""returntype"":""Integer"",""offset"":101,""safe"":false},{""name"":""testOutConditional"",""parameters"":[{""name"":""flag"",""type"":""Boolean""}],""returntype"":""String"",""offset"":223,""safe"":false},{""name"":""testOutSwitch"",""parameters"":[{""name"":""option"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":258,""safe"":false},{""name"":""testNestedOut"",""parameters"":[],""returntype"":""Array"",""offset"":303,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":383,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/YIBVwABACpgQFcAAxphDAVIZWxsb2IIY0AQYFg05lhAVwEAaGBYNNxYQBBjC2IQYVtaWTTWWTcAAAwCLCCLWosMAiwgi1smCgwEVHJ1ZSIJDAVGYWxzZYvbKEAQYwtiEGFbWlk0pUBXAgAQcBBxImwQYFg0j2hYnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9waUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaRW1JJNoQFcAAXgmDxBgWDUY////WDcAAEAQYwtiEGFbWlk1DP///1pAVwEBeHBoEZckCWgSlyQOIhwQYFg16v7//1hAEGMLYhBhW1pZNeH+//9ZQA9AVwEAEGRcNAtwxUpoz0pcz0BXAAFcZFg1uv7//1hkXBKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BWBUBuy72M"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAaGBYNNxYQA==
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.LDLOC0
    /// 0004 : OpCode.STSFLD0
    /// 0005 : OpCode.LDSFLD0
    /// 0006 : OpCode.CALL DC
    /// 0008 : OpCode.LDSFLD0
    /// 0009 : OpCode.RET
    /// </remarks>
    [DisplayName("testExistingVar")]
    public abstract BigInteger? TestExistingVar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGMLYhBhW1pZNNZZNwAADCwgi1qLDCwgi1smCgxUcnVlIgkMRmFsc2WL2yhA
    /// 0000 : OpCode.PUSH0
    /// 0001 : OpCode.STSFLD3
    /// 0002 : OpCode.PUSHNULL
    /// 0003 : OpCode.STSFLD2
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.STSFLD1
    /// 0006 : OpCode.LDSFLD3
    /// 0007 : OpCode.LDSFLD2
    /// 0008 : OpCode.LDSFLD1
    /// 0009 : OpCode.CALL D6
    /// 000B : OpCode.LDSFLD1
    /// 000C : OpCode.CALLT 0000
    /// 000F : OpCode.PUSHDATA1 2C20
    /// 0013 : OpCode.CAT
    /// 0014 : OpCode.LDSFLD2
    /// 0015 : OpCode.CAT
    /// 0016 : OpCode.PUSHDATA1 2C20
    /// 001A : OpCode.CAT
    /// 001B : OpCode.LDSFLD3
    /// 001C : OpCode.JMPIFNOT 0A
    /// 001E : OpCode.PUSHDATA1 54727565
    /// 0024 : OpCode.JMP 09
    /// 0026 : OpCode.PUSHDATA1 46616C7365
    /// 002D : OpCode.CAT
    /// 002E : OpCode.CONVERT 28
    /// 0030 : OpCode.RET
    /// </remarks>
    [DisplayName("testMultipleOut")]
    public abstract string? TestMultipleOut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEGRcNAtwxUpoz0pcz0A=
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STSFLD4
    /// 0005 : OpCode.LDSFLD4
    /// 0006 : OpCode.CALL 0B
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.NEWSTRUCT0
    /// 000A : OpCode.DUP
    /// 000B : OpCode.LDLOC0
    /// 000C : OpCode.APPEND
    /// 000D : OpCode.DUP
    /// 000E : OpCode.LDSFLD4
    /// 000F : OpCode.APPEND
    /// 0010 : OpCode.RET
    /// </remarks>
    [DisplayName("testNestedOut")]
    public abstract IList<object>? TestNestedOut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCYPEGBYNRj///9YNwAAQBBjC2IQYVtaWTUM////WkA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.JMPIFNOT 0F
    /// 0006 : OpCode.PUSH0
    /// 0007 : OpCode.STSFLD0
    /// 0008 : OpCode.LDSFLD0
    /// 0009 : OpCode.CALL_L 18FFFFFF
    /// 000E : OpCode.LDSFLD0
    /// 000F : OpCode.CALLT 0000
    /// 0012 : OpCode.RET
    /// 0013 : OpCode.PUSH0
    /// 0014 : OpCode.STSFLD3
    /// 0015 : OpCode.PUSHNULL
    /// 0016 : OpCode.STSFLD2
    /// 0017 : OpCode.PUSH0
    /// 0018 : OpCode.STSFLD1
    /// 0019 : OpCode.LDSFLD3
    /// 001A : OpCode.LDSFLD2
    /// 001B : OpCode.LDSFLD1
    /// 001C : OpCode.CALL_L 0CFFFFFF
    /// 0021 : OpCode.LDSFLD2
    /// 0022 : OpCode.RET
    /// </remarks>
    [DisplayName("testOutConditional")]
    public abstract string? TestOutConditional(bool? flag);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGMLYhBhW1pZNKVA
    /// 0000 : OpCode.PUSH0
    /// 0001 : OpCode.STSFLD3
    /// 0002 : OpCode.PUSHNULL
    /// 0003 : OpCode.STSFLD2
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.STSFLD1
    /// 0006 : OpCode.LDSFLD3
    /// 0007 : OpCode.LDSFLD2
    /// 0008 : OpCode.LDSFLD1
    /// 0009 : OpCode.CALL A5
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("testOutDiscard")]
    public abstract void TestOutDiscard();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHAQcSJsEGBYNI9oWJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWkVtSSTaEA=
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.STLOC1
    /// 0007 : OpCode.JMP 6C
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.STSFLD0
    /// 000B : OpCode.LDSFLD0
    /// 000C : OpCode.CALL 8F
    /// 000E : OpCode.LDLOC0
    /// 000F : OpCode.LDSFLD0
    /// 0010 : OpCode.ADD
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.PUSHINT32 00000080
    /// 0017 : OpCode.JMPGE 04
    /// 0019 : OpCode.JMP 0A
    /// 001B : OpCode.DUP
    /// 001C : OpCode.PUSHINT32 FFFFFF7F
    /// 0021 : OpCode.JMPLE 1E
    /// 0023 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 002C : OpCode.AND
    /// 002D : OpCode.DUP
    /// 002E : OpCode.PUSHINT32 FFFFFF7F
    /// 0033 : OpCode.JMPLE 0C
    /// 0035 : OpCode.PUSHINT64 0000000001000000
    /// 003E : OpCode.SUB
    /// 003F : OpCode.STLOC0
    /// 0040 : OpCode.LDLOC1
    /// 0041 : OpCode.DUP
    /// 0042 : OpCode.INC
    /// 0043 : OpCode.DUP
    /// 0044 : OpCode.PUSHINT32 00000080
    /// 0049 : OpCode.JMPGE 04
    /// 004B : OpCode.JMP 0A
    /// 004D : OpCode.DUP
    /// 004E : OpCode.PUSHINT32 FFFFFF7F
    /// 0053 : OpCode.JMPLE 1E
    /// 0055 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 005E : OpCode.AND
    /// 005F : OpCode.DUP
    /// 0060 : OpCode.PUSHINT32 FFFFFF7F
    /// 0065 : OpCode.JMPLE 0C
    /// 0067 : OpCode.PUSHINT64 0000000001000000
    /// 0070 : OpCode.SUB
    /// 0071 : OpCode.STLOC1
    /// 0072 : OpCode.DROP
    /// 0073 : OpCode.LDLOC1
    /// 0074 : OpCode.PUSH5
    /// 0075 : OpCode.LT
    /// 0076 : OpCode.JMPIF 93
    /// 0078 : OpCode.LDLOC0
    /// 0079 : OpCode.RET
    /// </remarks>
    [DisplayName("testOutInLoop")]
    public abstract BigInteger? TestOutInLoop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoEZckCWgSlyQOIhwQYFg16v7//1hAEGMLYhBhW1pZNeH+//9ZQA9A
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSH1
    /// 0007 : OpCode.EQUAL
    /// 0008 : OpCode.JMPIF 09
    /// 000A : OpCode.LDLOC0
    /// 000B : OpCode.PUSH2
    /// 000C : OpCode.EQUAL
    /// 000D : OpCode.JMPIF 0E
    /// 000F : OpCode.JMP 1C
    /// 0011 : OpCode.PUSH0
    /// 0012 : OpCode.STSFLD0
    /// 0013 : OpCode.LDSFLD0
    /// 0014 : OpCode.CALL_L EAFEFFFF
    /// 0019 : OpCode.LDSFLD0
    /// 001A : OpCode.RET
    /// 001B : OpCode.PUSH0
    /// 001C : OpCode.STSFLD3
    /// 001D : OpCode.PUSHNULL
    /// 001E : OpCode.STSFLD2
    /// 001F : OpCode.PUSH0
    /// 0020 : OpCode.STSFLD1
    /// 0021 : OpCode.LDSFLD3
    /// 0022 : OpCode.LDSFLD2
    /// 0023 : OpCode.LDSFLD1
    /// 0024 : OpCode.CALL_L E1FEFFFF
    /// 0029 : OpCode.LDSFLD1
    /// 002A : OpCode.RET
    /// 002B : OpCode.PUSHM1
    /// 002C : OpCode.RET
    /// </remarks>
    [DisplayName("testOutSwitch")]
    public abstract BigInteger? TestOutSwitch(BigInteger? option);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGBYNOZYQA==
    /// 0000 : OpCode.PUSH0
    /// 0001 : OpCode.STSFLD0
    /// 0002 : OpCode.LDSFLD0
    /// 0003 : OpCode.CALL E6
    /// 0005 : OpCode.LDSFLD0
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("testOutVar")]
    public abstract BigInteger? TestOutVar();

    #endregion

}
