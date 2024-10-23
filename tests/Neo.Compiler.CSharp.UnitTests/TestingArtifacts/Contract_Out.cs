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
    /// 00 : OpCode.INITSLOT 0100	[64 datoshi]
    /// 03 : OpCode.LDLOC0	[2 datoshi]
    /// 04 : OpCode.STSFLD0	[2 datoshi]
    /// 05 : OpCode.LDSFLD0	[2 datoshi]
    /// 06 : OpCode.CALL DC	[512 datoshi]
    /// 08 : OpCode.LDSFLD0	[2 datoshi]
    /// 09 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testExistingVar")]
    public abstract BigInteger? TestExistingVar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGMLYhBhW1pZNNZZNwAADCwgi1qLDCwgi1smCgxUcnVlIgkMRmFsc2WL2yhA
    /// 00 : OpCode.PUSH0	[1 datoshi]
    /// 01 : OpCode.STSFLD3	[2 datoshi]
    /// 02 : OpCode.PUSHNULL	[1 datoshi]
    /// 03 : OpCode.STSFLD2	[2 datoshi]
    /// 04 : OpCode.PUSH0	[1 datoshi]
    /// 05 : OpCode.STSFLD1	[2 datoshi]
    /// 06 : OpCode.LDSFLD3	[2 datoshi]
    /// 07 : OpCode.LDSFLD2	[2 datoshi]
    /// 08 : OpCode.LDSFLD1	[2 datoshi]
    /// 09 : OpCode.CALL D6	[512 datoshi]
    /// 0B : OpCode.LDSFLD1	[2 datoshi]
    /// 0C : OpCode.CALLT 0000	[32768 datoshi]
    /// 0F : OpCode.PUSHDATA1 2C20	[8 datoshi]
    /// 13 : OpCode.CAT	[2048 datoshi]
    /// 14 : OpCode.LDSFLD2	[2 datoshi]
    /// 15 : OpCode.CAT	[2048 datoshi]
    /// 16 : OpCode.PUSHDATA1 2C20	[8 datoshi]
    /// 1A : OpCode.CAT	[2048 datoshi]
    /// 1B : OpCode.LDSFLD3	[2 datoshi]
    /// 1C : OpCode.JMPIFNOT 0A	[2 datoshi]
    /// 1E : OpCode.PUSHDATA1 54727565	[8 datoshi]
    /// 24 : OpCode.JMP 09	[2 datoshi]
    /// 26 : OpCode.PUSHDATA1 46616C7365	[8 datoshi]
    /// 2D : OpCode.CAT	[2048 datoshi]
    /// 2E : OpCode.CONVERT 28	[8192 datoshi]
    /// 30 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testMultipleOut")]
    public abstract string? TestMultipleOut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEGRcNAtwxUpoz0pcz0A=
    /// 00 : OpCode.INITSLOT 0100	[64 datoshi]
    /// 03 : OpCode.PUSH0	[1 datoshi]
    /// 04 : OpCode.STSFLD4	[2 datoshi]
    /// 05 : OpCode.LDSFLD4	[2 datoshi]
    /// 06 : OpCode.CALL 0B	[512 datoshi]
    /// 08 : OpCode.STLOC0	[2 datoshi]
    /// 09 : OpCode.NEWSTRUCT0	[16 datoshi]
    /// 0A : OpCode.DUP	[2 datoshi]
    /// 0B : OpCode.LDLOC0	[2 datoshi]
    /// 0C : OpCode.APPEND	[8192 datoshi]
    /// 0D : OpCode.DUP	[2 datoshi]
    /// 0E : OpCode.LDSFLD4	[2 datoshi]
    /// 0F : OpCode.APPEND	[8192 datoshi]
    /// 10 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testNestedOut")]
    public abstract IList<object>? TestNestedOut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCYPEGBYNRj///9YNwAAQBBjC2IQYVtaWTUM////WkA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.JMPIFNOT 0F	[2 datoshi]
    /// 06 : OpCode.PUSH0	[1 datoshi]
    /// 07 : OpCode.STSFLD0	[2 datoshi]
    /// 08 : OpCode.LDSFLD0	[2 datoshi]
    /// 09 : OpCode.CALL_L 18FFFFFF	[512 datoshi]
    /// 0E : OpCode.LDSFLD0	[2 datoshi]
    /// 0F : OpCode.CALLT 0000	[32768 datoshi]
    /// 12 : OpCode.RET	[0 datoshi]
    /// 13 : OpCode.PUSH0	[1 datoshi]
    /// 14 : OpCode.STSFLD3	[2 datoshi]
    /// 15 : OpCode.PUSHNULL	[1 datoshi]
    /// 16 : OpCode.STSFLD2	[2 datoshi]
    /// 17 : OpCode.PUSH0	[1 datoshi]
    /// 18 : OpCode.STSFLD1	[2 datoshi]
    /// 19 : OpCode.LDSFLD3	[2 datoshi]
    /// 1A : OpCode.LDSFLD2	[2 datoshi]
    /// 1B : OpCode.LDSFLD1	[2 datoshi]
    /// 1C : OpCode.CALL_L 0CFFFFFF	[512 datoshi]
    /// 21 : OpCode.LDSFLD2	[2 datoshi]
    /// 22 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testOutConditional")]
    public abstract string? TestOutConditional(bool? flag);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGMLYhBhW1pZNKVA
    /// 00 : OpCode.PUSH0	[1 datoshi]
    /// 01 : OpCode.STSFLD3	[2 datoshi]
    /// 02 : OpCode.PUSHNULL	[1 datoshi]
    /// 03 : OpCode.STSFLD2	[2 datoshi]
    /// 04 : OpCode.PUSH0	[1 datoshi]
    /// 05 : OpCode.STSFLD1	[2 datoshi]
    /// 06 : OpCode.LDSFLD3	[2 datoshi]
    /// 07 : OpCode.LDSFLD2	[2 datoshi]
    /// 08 : OpCode.LDSFLD1	[2 datoshi]
    /// 09 : OpCode.CALL A5	[512 datoshi]
    /// 0B : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testOutDiscard")]
    public abstract void TestOutDiscard();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHAQcSJsEGBYNI9oWJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWkVtSSTaEA=
    /// 00 : OpCode.INITSLOT 0200	[64 datoshi]
    /// 03 : OpCode.PUSH0	[1 datoshi]
    /// 04 : OpCode.STLOC0	[2 datoshi]
    /// 05 : OpCode.PUSH0	[1 datoshi]
    /// 06 : OpCode.STLOC1	[2 datoshi]
    /// 07 : OpCode.JMP 6C	[2 datoshi]
    /// 09 : OpCode.PUSH0	[1 datoshi]
    /// 0A : OpCode.STSFLD0	[2 datoshi]
    /// 0B : OpCode.LDSFLD0	[2 datoshi]
    /// 0C : OpCode.CALL 8F	[512 datoshi]
    /// 0E : OpCode.LDLOC0	[2 datoshi]
    /// 0F : OpCode.LDSFLD0	[2 datoshi]
    /// 10 : OpCode.ADD	[8 datoshi]
    /// 11 : OpCode.DUP	[2 datoshi]
    /// 12 : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 17 : OpCode.JMPGE 04	[2 datoshi]
    /// 19 : OpCode.JMP 0A	[2 datoshi]
    /// 1B : OpCode.DUP	[2 datoshi]
    /// 1C : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 21 : OpCode.JMPLE 1E	[2 datoshi]
    /// 23 : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 2C : OpCode.AND	[8 datoshi]
    /// 2D : OpCode.DUP	[2 datoshi]
    /// 2E : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 33 : OpCode.JMPLE 0C	[2 datoshi]
    /// 35 : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 3E : OpCode.SUB	[8 datoshi]
    /// 3F : OpCode.STLOC0	[2 datoshi]
    /// 40 : OpCode.LDLOC1	[2 datoshi]
    /// 41 : OpCode.DUP	[2 datoshi]
    /// 42 : OpCode.INC	[4 datoshi]
    /// 43 : OpCode.DUP	[2 datoshi]
    /// 44 : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 49 : OpCode.JMPGE 04	[2 datoshi]
    /// 4B : OpCode.JMP 0A	[2 datoshi]
    /// 4D : OpCode.DUP	[2 datoshi]
    /// 4E : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 53 : OpCode.JMPLE 1E	[2 datoshi]
    /// 55 : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 5E : OpCode.AND	[8 datoshi]
    /// 5F : OpCode.DUP	[2 datoshi]
    /// 60 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 65 : OpCode.JMPLE 0C	[2 datoshi]
    /// 67 : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 70 : OpCode.SUB	[8 datoshi]
    /// 71 : OpCode.STLOC1	[2 datoshi]
    /// 72 : OpCode.DROP	[2 datoshi]
    /// 73 : OpCode.LDLOC1	[2 datoshi]
    /// 74 : OpCode.PUSH5	[1 datoshi]
    /// 75 : OpCode.LT	[8 datoshi]
    /// 76 : OpCode.JMPIF 93	[2 datoshi]
    /// 78 : OpCode.LDLOC0	[2 datoshi]
    /// 79 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testOutInLoop")]
    public abstract BigInteger? TestOutInLoop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoEZckCWgSlyQOIhwQYFg16v7//1hAEGMLYhBhW1pZNeH+//9ZQA9A
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.STLOC0	[2 datoshi]
    /// 05 : OpCode.LDLOC0	[2 datoshi]
    /// 06 : OpCode.PUSH1	[1 datoshi]
    /// 07 : OpCode.EQUAL	[32 datoshi]
    /// 08 : OpCode.JMPIF 09	[2 datoshi]
    /// 0A : OpCode.LDLOC0	[2 datoshi]
    /// 0B : OpCode.PUSH2	[1 datoshi]
    /// 0C : OpCode.EQUAL	[32 datoshi]
    /// 0D : OpCode.JMPIF 0E	[2 datoshi]
    /// 0F : OpCode.JMP 1C	[2 datoshi]
    /// 11 : OpCode.PUSH0	[1 datoshi]
    /// 12 : OpCode.STSFLD0	[2 datoshi]
    /// 13 : OpCode.LDSFLD0	[2 datoshi]
    /// 14 : OpCode.CALL_L EAFEFFFF	[512 datoshi]
    /// 19 : OpCode.LDSFLD0	[2 datoshi]
    /// 1A : OpCode.RET	[0 datoshi]
    /// 1B : OpCode.PUSH0	[1 datoshi]
    /// 1C : OpCode.STSFLD3	[2 datoshi]
    /// 1D : OpCode.PUSHNULL	[1 datoshi]
    /// 1E : OpCode.STSFLD2	[2 datoshi]
    /// 1F : OpCode.PUSH0	[1 datoshi]
    /// 20 : OpCode.STSFLD1	[2 datoshi]
    /// 21 : OpCode.LDSFLD3	[2 datoshi]
    /// 22 : OpCode.LDSFLD2	[2 datoshi]
    /// 23 : OpCode.LDSFLD1	[2 datoshi]
    /// 24 : OpCode.CALL_L E1FEFFFF	[512 datoshi]
    /// 29 : OpCode.LDSFLD1	[2 datoshi]
    /// 2A : OpCode.RET	[0 datoshi]
    /// 2B : OpCode.PUSHM1	[1 datoshi]
    /// 2C : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testOutSwitch")]
    public abstract BigInteger? TestOutSwitch(BigInteger? option);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGBYNOZYQA==
    /// 00 : OpCode.PUSH0	[1 datoshi]
    /// 01 : OpCode.STSFLD0	[2 datoshi]
    /// 02 : OpCode.LDSFLD0	[2 datoshi]
    /// 03 : OpCode.CALL E6	[512 datoshi]
    /// 05 : OpCode.LDSFLD0	[2 datoshi]
    /// 06 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testOutVar")]
    public abstract BigInteger? TestOutVar();

    #endregion
}
