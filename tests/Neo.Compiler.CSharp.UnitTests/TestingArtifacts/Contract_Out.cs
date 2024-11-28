using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/X8BVwABACpgQFcAAxphDAVIZWxsb2IIY0AQYFg05lhAVwEAaGBYNNxYQAljC2IQYVtaWTTWWTcAAAwCLCCLWosMAiwgi1smCgwEVHJ1ZSIJDAVGYWxzZYvbKEAJYwtiEGFbWlk0pUBXAgAQcBBxImwQYFg0j2hYnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9waUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaRW1JJNoQFcAAXgmDxBgWDUY////WDcAAEAJYwtiEGFbWlk1DP///1pAVwEBeHBoEZckCWgSlyQOIhwQYFg16v7//1hACWMLYhBhW1pZNeH+//9ZQA9AVwEAEGRcNAhwXGgSv0BXAAFcZFg1vf7//1hkXBKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BWBUAa2QfK").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAaGBYNNxYQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : LDLOC0 [2 datoshi]
    /// 04 : STSFLD0 [2 datoshi]
    /// 05 : LDSFLD0 [2 datoshi]
    /// 06 : CALL DC [512 datoshi]
    /// 08 : LDSFLD0 [2 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testExistingVar")]
    public abstract BigInteger? TestExistingVar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CWMLYhBhW1pZNNZZNwAADAIsIItaiwwCLCCLWyYKDARUcnVlIgkMBUZhbHNli9soQA==
    /// 00 : PUSHF [1 datoshi]
    /// 01 : STSFLD3 [2 datoshi]
    /// 02 : PUSHNULL [1 datoshi]
    /// 03 : STSFLD2 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : STSFLD1 [2 datoshi]
    /// 06 : LDSFLD3 [2 datoshi]
    /// 07 : LDSFLD2 [2 datoshi]
    /// 08 : LDSFLD1 [2 datoshi]
    /// 09 : CALL D6 [512 datoshi]
    /// 0B : LDSFLD1 [2 datoshi]
    /// 0C : CALLT 0000 [32768 datoshi]
    /// 0F : PUSHDATA1 2C20 [8 datoshi]
    /// 13 : CAT [2048 datoshi]
    /// 14 : LDSFLD2 [2 datoshi]
    /// 15 : CAT [2048 datoshi]
    /// 16 : PUSHDATA1 2C20 [8 datoshi]
    /// 1A : CAT [2048 datoshi]
    /// 1B : LDSFLD3 [2 datoshi]
    /// 1C : JMPIFNOT 0A [2 datoshi]
    /// 1E : PUSHDATA1 54727565 'True' [8 datoshi]
    /// 24 : JMP 09 [2 datoshi]
    /// 26 : PUSHDATA1 46616C7365 'False' [8 datoshi]
    /// 2D : CAT [2048 datoshi]
    /// 2E : CONVERT 28 'ByteString' [8192 datoshi]
    /// 30 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMultipleOut")]
    public abstract string? TestMultipleOut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEGRcNAhwXGgSv0A=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STSFLD4 [2 datoshi]
    /// 05 : LDSFLD4 [2 datoshi]
    /// 06 : CALL 08 [512 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDSFLD4 [2 datoshi]
    /// 0A : LDLOC0 [2 datoshi]
    /// 0B : PUSH2 [1 datoshi]
    /// 0C : PACKSTRUCT [2048 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNestedOut")]
    public abstract IList<object>? TestNestedOut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCYPEGBYNRj///9YNwAAQAljC2IQYVtaWTUM////WkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : JMPIFNOT 0F [2 datoshi]
    /// 06 : PUSH0 [1 datoshi]
    /// 07 : STSFLD0 [2 datoshi]
    /// 08 : LDSFLD0 [2 datoshi]
    /// 09 : CALL_L 18FFFFFF [512 datoshi]
    /// 0E : LDSFLD0 [2 datoshi]
    /// 0F : CALLT 0000 [32768 datoshi]
    /// 12 : RET [0 datoshi]
    /// 13 : PUSHF [1 datoshi]
    /// 14 : STSFLD3 [2 datoshi]
    /// 15 : PUSHNULL [1 datoshi]
    /// 16 : STSFLD2 [2 datoshi]
    /// 17 : PUSH0 [1 datoshi]
    /// 18 : STSFLD1 [2 datoshi]
    /// 19 : LDSFLD3 [2 datoshi]
    /// 1A : LDSFLD2 [2 datoshi]
    /// 1B : LDSFLD1 [2 datoshi]
    /// 1C : CALL_L 0CFFFFFF [512 datoshi]
    /// 21 : LDSFLD2 [2 datoshi]
    /// 22 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutConditional")]
    public abstract string? TestOutConditional(bool? flag);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CWMLYhBhW1pZNKVA
    /// 00 : PUSHF [1 datoshi]
    /// 01 : STSFLD3 [2 datoshi]
    /// 02 : PUSHNULL [1 datoshi]
    /// 03 : STSFLD2 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : STSFLD1 [2 datoshi]
    /// 06 : LDSFLD3 [2 datoshi]
    /// 07 : LDSFLD2 [2 datoshi]
    /// 08 : LDSFLD1 [2 datoshi]
    /// 09 : CALL A5 [512 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutDiscard")]
    public abstract void TestOutDiscard();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHAQcSJsEGBYNI9oWJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWkVtSSTaEA=
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : STLOC1 [2 datoshi]
    /// 07 : JMP 6C [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : STSFLD0 [2 datoshi]
    /// 0B : LDSFLD0 [2 datoshi]
    /// 0C : CALL 8F [512 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : LDSFLD0 [2 datoshi]
    /// 10 : ADD [8 datoshi]
    /// 11 : DUP [2 datoshi]
    /// 12 : PUSHINT32 00000080 [1 datoshi]
    /// 17 : JMPGE 04 [2 datoshi]
    /// 19 : JMP 0A [2 datoshi]
    /// 1B : DUP [2 datoshi]
    /// 1C : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 21 : JMPLE 1E [2 datoshi]
    /// 23 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2C : AND [8 datoshi]
    /// 2D : DUP [2 datoshi]
    /// 2E : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 33 : JMPLE 0C [2 datoshi]
    /// 35 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 3E : SUB [8 datoshi]
    /// 3F : STLOC0 [2 datoshi]
    /// 40 : LDLOC1 [2 datoshi]
    /// 41 : DUP [2 datoshi]
    /// 42 : INC [4 datoshi]
    /// 43 : DUP [2 datoshi]
    /// 44 : PUSHINT32 00000080 [1 datoshi]
    /// 49 : JMPGE 04 [2 datoshi]
    /// 4B : JMP 0A [2 datoshi]
    /// 4D : DUP [2 datoshi]
    /// 4E : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 53 : JMPLE 1E [2 datoshi]
    /// 55 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 5E : AND [8 datoshi]
    /// 5F : DUP [2 datoshi]
    /// 60 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 65 : JMPLE 0C [2 datoshi]
    /// 67 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 70 : SUB [8 datoshi]
    /// 71 : STLOC1 [2 datoshi]
    /// 72 : DROP [2 datoshi]
    /// 73 : LDLOC1 [2 datoshi]
    /// 74 : PUSH5 [1 datoshi]
    /// 75 : LT [8 datoshi]
    /// 76 : JMPIF 93 [2 datoshi]
    /// 78 : LDLOC0 [2 datoshi]
    /// 79 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutInLoop")]
    public abstract BigInteger? TestOutInLoop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoEZckCWgSlyQOIhwQYFg16v7//1hACWMLYhBhW1pZNeH+//9ZQA9A
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : PUSH1 [1 datoshi]
    /// 07 : EQUAL [32 datoshi]
    /// 08 : JMPIF 09 [2 datoshi]
    /// 0A : LDLOC0 [2 datoshi]
    /// 0B : PUSH2 [1 datoshi]
    /// 0C : EQUAL [32 datoshi]
    /// 0D : JMPIF 0E [2 datoshi]
    /// 0F : JMP 1C [2 datoshi]
    /// 11 : PUSH0 [1 datoshi]
    /// 12 : STSFLD0 [2 datoshi]
    /// 13 : LDSFLD0 [2 datoshi]
    /// 14 : CALL_L EAFEFFFF [512 datoshi]
    /// 19 : LDSFLD0 [2 datoshi]
    /// 1A : RET [0 datoshi]
    /// 1B : PUSHF [1 datoshi]
    /// 1C : STSFLD3 [2 datoshi]
    /// 1D : PUSHNULL [1 datoshi]
    /// 1E : STSFLD2 [2 datoshi]
    /// 1F : PUSH0 [1 datoshi]
    /// 20 : STSFLD1 [2 datoshi]
    /// 21 : LDSFLD3 [2 datoshi]
    /// 22 : LDSFLD2 [2 datoshi]
    /// 23 : LDSFLD1 [2 datoshi]
    /// 24 : CALL_L E1FEFFFF [512 datoshi]
    /// 29 : LDSFLD1 [2 datoshi]
    /// 2A : RET [0 datoshi]
    /// 2B : PUSHM1 [1 datoshi]
    /// 2C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutSwitch")]
    public abstract BigInteger? TestOutSwitch(BigInteger? option);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGBYNOZYQA==
    /// 00 : PUSH0 [1 datoshi]
    /// 01 : STSFLD0 [2 datoshi]
    /// 02 : LDSFLD0 [2 datoshi]
    /// 03 : CALL E6 [512 datoshi]
    /// 05 : LDSFLD0 [2 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutVar")]
    public abstract BigInteger? TestOutVar();

    #endregion
}
