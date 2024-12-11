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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/X8BVwABACpgQFcAAxphDAVIZWxsb2IIY0AQYFg05lhAVwEAaGBYNNxYQAljC2IQYVtaWTTWWTcAAAwCLCCLWosMAiwgi1smCgwEVHJ1ZSIJDAVGYWxzZYvbKEAJYwtiEGFbWlk0pUBXAgAQcBBxImwQYFg0j2hYnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9waUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaRW1JJNoQFcAAXgmDxBgWDUY////WDcAAEAJYwtiEGFbWlk1DP///1pAVwEBeHBoEZckCWgSlyQOIhwQYFg16v7//1hACWMLYhBhW1pZNeH+//9ZQA9AVwEAEGRcNAhwXGgSv0BXAAFcZFg1vf7//1hkXBKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BWBUAa2QfK"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAaGBYNNxYQA==
    /// INITSLOT 0100 [64 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// CALL DC [512 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testExistingVar")]
    public abstract BigInteger? TestExistingVar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CWMLYhBhW1pZNNZZNwAADAIsIItaiwwCLCCLWyYKDARUcnVlIgkMBUZhbHNli9soQA==
    /// PUSHF [1 datoshi]
    /// STSFLD3 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STSFLD2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// CALL D6 [512 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// PUSHDATA1 2C20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 2C20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// JMPIFNOT 0A [2 datoshi]
    /// PUSHDATA1 54727565 'True' [8 datoshi]
    /// JMP 09 [2 datoshi]
    /// PUSHDATA1 46616C7365 'False' [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMultipleOut")]
    public abstract string? TestMultipleOut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEGRcNAhwXGgSv0A=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD4 [2 datoshi]
    /// LDSFLD4 [2 datoshi]
    /// CALL 08 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDSFLD4 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNestedOut")]
    public abstract IList<object>? TestNestedOut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCYPEGBYNRj///9YNwAAQAljC2IQYVtaWTUM////WkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 0F [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// CALL_L 18FFFFFF [512 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// PUSHF [1 datoshi]
    /// STSFLD3 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STSFLD2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// CALL_L 0CFFFFFF [512 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutConditional")]
    public abstract string? TestOutConditional(bool? flag);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CWMLYhBhW1pZNKVA
    /// PUSHF [1 datoshi]
    /// STSFLD3 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STSFLD2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// CALL A5 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutDiscard")]
    public abstract void TestOutDiscard();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHAQcSJsEGBYNI9oWJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWkVtSSTaEA=
    /// INITSLOT 0200 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// JMP 6C [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// CALL 8F [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH5 [1 datoshi]
    /// LT [8 datoshi]
    /// JMPIF 93 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutInLoop")]
    public abstract BigInteger? TestOutInLoop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoEZckCWgSlyQOIhwQYFg16v7//1hACWMLYhBhW1pZNeH+//9ZQA9A
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 09 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 0E [2 datoshi]
    /// JMP 1C [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// CALL_L EAFEFFFF [512 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// RET [0 datoshi]
    /// PUSHF [1 datoshi]
    /// STSFLD3 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STSFLD2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// CALL_L E1FEFFFF [512 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// RET [0 datoshi]
    /// PUSHM1 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutSwitch")]
    public abstract BigInteger? TestOutSwitch(BigInteger? option);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGBYNOZYQA==
    /// PUSH0 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// CALL E6 [512 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutVar")]
    public abstract BigInteger? TestOutVar();

    #endregion
}
