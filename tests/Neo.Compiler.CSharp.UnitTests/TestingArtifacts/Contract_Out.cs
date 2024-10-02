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
    [DisplayName("testExistingVar")]
    public abstract BigInteger? TestExistingVar();
    // 0000 : INITSLOT
    // 0003 : LDLOC0
    // 0004 : STSFLD0
    // 0005 : LDSFLD0
    // 0006 : CALL
    // 0008 : LDSFLD0
    // 0009 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testMultipleOut")]
    public abstract string? TestMultipleOut();
    // 0000 : PUSH0
    // 0001 : STSFLD3
    // 0002 : PUSHNULL
    // 0003 : STSFLD2
    // 0004 : PUSH0
    // 0005 : STSFLD1
    // 0006 : LDSFLD3
    // 0007 : LDSFLD2
    // 0008 : LDSFLD1
    // 0009 : CALL
    // 000B : LDSFLD1
    // 000C : CALLT
    // 000F : PUSHDATA1
    // 0013 : CAT
    // 0014 : LDSFLD2
    // 0015 : CAT
    // 0016 : PUSHDATA1
    // 001A : CAT
    // 001B : LDSFLD3
    // 001C : JMPIFNOT
    // 001E : PUSHDATA1
    // 0024 : JMP
    // 0026 : PUSHDATA1
    // 002D : CAT
    // 002E : CONVERT
    // 0030 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testNestedOut")]
    public abstract IList<object>? TestNestedOut();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STSFLD4
    // 0005 : LDSFLD4
    // 0006 : CALL
    // 0008 : STLOC0
    // 0009 : NEWSTRUCT0
    // 000A : DUP
    // 000B : LDLOC0
    // 000C : APPEND
    // 000D : DUP
    // 000E : LDSFLD4
    // 000F : APPEND
    // 0010 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testOutConditional")]
    public abstract string? TestOutConditional(bool? flag);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : JMPIFNOT
    // 0006 : PUSH0
    // 0007 : STSFLD0
    // 0008 : LDSFLD0
    // 0009 : CALL_L
    // 000E : LDSFLD0
    // 000F : CALLT
    // 0012 : RET
    // 0013 : PUSH0
    // 0014 : STSFLD3
    // 0015 : PUSHNULL
    // 0016 : STSFLD2
    // 0017 : PUSH0
    // 0018 : STSFLD1
    // 0019 : LDSFLD3
    // 001A : LDSFLD2
    // 001B : LDSFLD1
    // 001C : CALL_L
    // 0021 : LDSFLD2
    // 0022 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testOutDiscard")]
    public abstract void TestOutDiscard();
    // 0000 : PUSH0
    // 0001 : STSFLD3
    // 0002 : PUSHNULL
    // 0003 : STSFLD2
    // 0004 : PUSH0
    // 0005 : STSFLD1
    // 0006 : LDSFLD3
    // 0007 : LDSFLD2
    // 0008 : LDSFLD1
    // 0009 : CALL
    // 000B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testOutInLoop")]
    public abstract BigInteger? TestOutInLoop();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : PUSH0
    // 0006 : STLOC1
    // 0007 : JMP
    // 0009 : PUSH0
    // 000A : STSFLD0
    // 000B : LDSFLD0
    // 000C : CALL
    // 000E : LDLOC0
    // 000F : LDSFLD0
    // 0010 : ADD
    // 0011 : DUP
    // 0012 : PUSHINT32
    // 0017 : JMPGE
    // 0019 : JMP
    // 001B : DUP
    // 001C : PUSHINT32
    // 0021 : JMPLE
    // 0023 : PUSHINT64
    // 002C : AND
    // 002D : DUP
    // 002E : PUSHINT32
    // 0033 : JMPLE
    // 0035 : PUSHINT64
    // 003E : SUB
    // 003F : STLOC0
    // 0040 : LDLOC1
    // 0041 : DUP
    // 0042 : INC
    // 0043 : DUP
    // 0044 : PUSHINT32
    // 0049 : JMPGE
    // 004B : JMP
    // 004D : DUP
    // 004E : PUSHINT32
    // 0053 : JMPLE
    // 0055 : PUSHINT64
    // 005E : AND
    // 005F : DUP
    // 0060 : PUSHINT32
    // 0065 : JMPLE
    // 0067 : PUSHINT64
    // 0070 : SUB
    // 0071 : STLOC1
    // 0072 : DROP
    // 0073 : LDLOC1
    // 0074 : PUSH5
    // 0075 : LT
    // 0076 : JMPIF
    // 0078 : LDLOC0
    // 0079 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testOutSwitch")]
    public abstract BigInteger? TestOutSwitch(BigInteger? option);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSH1
    // 0007 : EQUAL
    // 0008 : JMPIF
    // 000A : LDLOC0
    // 000B : PUSH2
    // 000C : EQUAL
    // 000D : JMPIF
    // 000F : JMP
    // 0011 : PUSH0
    // 0012 : STSFLD0
    // 0013 : LDSFLD0
    // 0014 : CALL_L
    // 0019 : LDSFLD0
    // 001A : RET
    // 001B : PUSH0
    // 001C : STSFLD3
    // 001D : PUSHNULL
    // 001E : STSFLD2
    // 001F : PUSH0
    // 0020 : STSFLD1
    // 0021 : LDSFLD3
    // 0022 : LDSFLD2
    // 0023 : LDSFLD1
    // 0024 : CALL_L
    // 0029 : LDSFLD1
    // 002A : RET
    // 002B : PUSHM1
    // 002C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testOutVar")]
    public abstract BigInteger? TestOutVar();
    // 0000 : PUSH0
    // 0001 : STSFLD0
    // 0002 : LDSFLD0
    // 0003 : CALL
    // 0005 : LDSFLD0
    // 0006 : RET

    #endregion

}
