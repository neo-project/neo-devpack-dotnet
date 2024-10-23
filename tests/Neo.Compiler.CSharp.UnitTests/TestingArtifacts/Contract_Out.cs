using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Out(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Out"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testOutVar"",""parameters"":[],""returntype"":""Integer"",""offset"":23,""safe"":false},{""name"":""testExistingVar"",""parameters"":[],""returntype"":""Integer"",""offset"":30,""safe"":false},{""name"":""testMultipleOut"",""parameters"":[],""returntype"":""String"",""offset"":40,""safe"":false},{""name"":""testOutDiscard"",""parameters"":[],""returntype"":""Void"",""offset"":89,""safe"":false},{""name"":""testOutInLoop"",""parameters"":[],""returntype"":""Integer"",""offset"":101,""safe"":false},{""name"":""testOutConditional"",""parameters"":[{""name"":""flag"",""type"":""Boolean""}],""returntype"":""String"",""offset"":223,""safe"":false},{""name"":""testOutSwitch"",""parameters"":[{""name"":""option"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":258,""safe"":false},{""name"":""testNestedOut"",""parameters"":[],""returntype"":""Array"",""offset"":303,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":380,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/X8BVwABACpgQFcAAxphDAVIZWxsb2IIY0AQYFg05lhAVwEAaGBYNNxYQBBjC2IQYVtaWTTWWTcAAAwCLCCLWosMAiwgi1smCgwEVHJ1ZSIJDAVGYWxzZYvbKEAQYwtiEGFbWlk0pUBXAgAQcBBxImwQYFg0j2hYnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9waUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaRW1JJNoQFcAAXgmDxBgWDUY////WDcAAEAQYwtiEGFbWlk1DP///1pAVwEBeHBoEZckCWgSlyQOIhwQYFg16v7//1hAEGMLYhBhW1pZNeH+//9ZQA9AVwEAEGRcNAhwXGgSv0BXAAFcZFg1vf7//1hkXBKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BWBUBHGrIq"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAaGBYNNxYQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.LDLOC0
    /// 04 : OpCode.STSFLD0
    /// 05 : OpCode.LDSFLD0
    /// 06 : OpCode.CALL DC
    /// 08 : OpCode.LDSFLD0
    /// 09 : OpCode.RET
    /// </remarks>
    [DisplayName("testExistingVar")]
    public abstract BigInteger? TestExistingVar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGMLYhBhW1pZNNZZNwAADCwgi1qLDCwgi1smCgxUcnVlIgkMRmFsc2WL2yhA
    /// 00 : OpCode.PUSH0
    /// 01 : OpCode.STSFLD3
    /// 02 : OpCode.PUSHNULL
    /// 03 : OpCode.STSFLD2
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.STSFLD1
    /// 06 : OpCode.LDSFLD3
    /// 07 : OpCode.LDSFLD2
    /// 08 : OpCode.LDSFLD1
    /// 09 : OpCode.CALL D6
    /// 0B : OpCode.LDSFLD1
    /// 0C : OpCode.CALLT 0000
    /// 0F : OpCode.PUSHDATA1 2C20
    /// 13 : OpCode.CAT
    /// 14 : OpCode.LDSFLD2
    /// 15 : OpCode.CAT
    /// 16 : OpCode.PUSHDATA1 2C20
    /// 1A : OpCode.CAT
    /// 1B : OpCode.LDSFLD3
    /// 1C : OpCode.JMPIFNOT 0A
    /// 1E : OpCode.PUSHDATA1 54727565
    /// 24 : OpCode.JMP 09
    /// 26 : OpCode.PUSHDATA1 46616C7365
    /// 2D : OpCode.CAT
    /// 2E : OpCode.CONVERT 28
    /// 30 : OpCode.RET
    /// </remarks>
    [DisplayName("testMultipleOut")]
    public abstract string? TestMultipleOut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEGRcNAhwXGgSv0A=
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STSFLD4
    /// 05 : OpCode.LDSFLD4
    /// 06 : OpCode.CALL 08
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDSFLD4
    /// 0A : OpCode.LDLOC0
    /// 0B : OpCode.PUSH2
    /// 0C : OpCode.PACKSTRUCT
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("testNestedOut")]
    public abstract IList<object>? TestNestedOut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCYPEGBYNRj///9YNwAAQBBjC2IQYVtaWTUM////WkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.JMPIFNOT 0F
    /// 06 : OpCode.PUSH0
    /// 07 : OpCode.STSFLD0
    /// 08 : OpCode.LDSFLD0
    /// 09 : OpCode.CALL_L 18FFFFFF
    /// 0E : OpCode.LDSFLD0
    /// 0F : OpCode.CALLT 0000
    /// 12 : OpCode.RET
    /// 13 : OpCode.PUSH0
    /// 14 : OpCode.STSFLD3
    /// 15 : OpCode.PUSHNULL
    /// 16 : OpCode.STSFLD2
    /// 17 : OpCode.PUSH0
    /// 18 : OpCode.STSFLD1
    /// 19 : OpCode.LDSFLD3
    /// 1A : OpCode.LDSFLD2
    /// 1B : OpCode.LDSFLD1
    /// 1C : OpCode.CALL_L 0CFFFFFF
    /// 21 : OpCode.LDSFLD2
    /// 22 : OpCode.RET
    /// </remarks>
    [DisplayName("testOutConditional")]
    public abstract string? TestOutConditional(bool? flag);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGMLYhBhW1pZNKVA
    /// 00 : OpCode.PUSH0
    /// 01 : OpCode.STSFLD3
    /// 02 : OpCode.PUSHNULL
    /// 03 : OpCode.STSFLD2
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.STSFLD1
    /// 06 : OpCode.LDSFLD3
    /// 07 : OpCode.LDSFLD2
    /// 08 : OpCode.LDSFLD1
    /// 09 : OpCode.CALL A5
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("testOutDiscard")]
    public abstract void TestOutDiscard();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHAQcSJsEGBYNI9oWJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWkVtSSTaEA=
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.JMP 6C
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.STSFLD0
    /// 0B : OpCode.LDSFLD0
    /// 0C : OpCode.CALL 8F
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.LDSFLD0
    /// 10 : OpCode.ADD
    /// 11 : OpCode.DUP
    /// 12 : OpCode.PUSHINT32 00000080
    /// 17 : OpCode.JMPGE 04
    /// 19 : OpCode.JMP 0A
    /// 1B : OpCode.DUP
    /// 1C : OpCode.PUSHINT32 FFFFFF7F
    /// 21 : OpCode.JMPLE 1E
    /// 23 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2C : OpCode.AND
    /// 2D : OpCode.DUP
    /// 2E : OpCode.PUSHINT32 FFFFFF7F
    /// 33 : OpCode.JMPLE 0C
    /// 35 : OpCode.PUSHINT64 0000000001000000
    /// 3E : OpCode.SUB
    /// 3F : OpCode.STLOC0
    /// 40 : OpCode.LDLOC1
    /// 41 : OpCode.DUP
    /// 42 : OpCode.INC
    /// 43 : OpCode.DUP
    /// 44 : OpCode.PUSHINT32 00000080
    /// 49 : OpCode.JMPGE 04
    /// 4B : OpCode.JMP 0A
    /// 4D : OpCode.DUP
    /// 4E : OpCode.PUSHINT32 FFFFFF7F
    /// 53 : OpCode.JMPLE 1E
    /// 55 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 5E : OpCode.AND
    /// 5F : OpCode.DUP
    /// 60 : OpCode.PUSHINT32 FFFFFF7F
    /// 65 : OpCode.JMPLE 0C
    /// 67 : OpCode.PUSHINT64 0000000001000000
    /// 70 : OpCode.SUB
    /// 71 : OpCode.STLOC1
    /// 72 : OpCode.DROP
    /// 73 : OpCode.LDLOC1
    /// 74 : OpCode.PUSH5
    /// 75 : OpCode.LT
    /// 76 : OpCode.JMPIF 93
    /// 78 : OpCode.LDLOC0
    /// 79 : OpCode.RET
    /// </remarks>
    [DisplayName("testOutInLoop")]
    public abstract BigInteger? TestOutInLoop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoEZckCWgSlyQOIhwQYFg16v7//1hAEGMLYhBhW1pZNeH+//9ZQA9A
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSH1
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.JMPIF 09
    /// 0A : OpCode.LDLOC0
    /// 0B : OpCode.PUSH2
    /// 0C : OpCode.EQUAL
    /// 0D : OpCode.JMPIF 0E
    /// 0F : OpCode.JMP 1C
    /// 11 : OpCode.PUSH0
    /// 12 : OpCode.STSFLD0
    /// 13 : OpCode.LDSFLD0
    /// 14 : OpCode.CALL_L EAFEFFFF
    /// 19 : OpCode.LDSFLD0
    /// 1A : OpCode.RET
    /// 1B : OpCode.PUSH0
    /// 1C : OpCode.STSFLD3
    /// 1D : OpCode.PUSHNULL
    /// 1E : OpCode.STSFLD2
    /// 1F : OpCode.PUSH0
    /// 20 : OpCode.STSFLD1
    /// 21 : OpCode.LDSFLD3
    /// 22 : OpCode.LDSFLD2
    /// 23 : OpCode.LDSFLD1
    /// 24 : OpCode.CALL_L E1FEFFFF
    /// 29 : OpCode.LDSFLD1
    /// 2A : OpCode.RET
    /// 2B : OpCode.PUSHM1
    /// 2C : OpCode.RET
    /// </remarks>
    [DisplayName("testOutSwitch")]
    public abstract BigInteger? TestOutSwitch(BigInteger? option);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGBYNOZYQA==
    /// 00 : OpCode.PUSH0
    /// 01 : OpCode.STSFLD0
    /// 02 : OpCode.LDSFLD0
    /// 03 : OpCode.CALL E6
    /// 05 : OpCode.LDSFLD0
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testOutVar")]
    public abstract BigInteger? TestOutVar();

    #endregion
}
