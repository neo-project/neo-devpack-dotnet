using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Out(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Out"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testOutVar"",""parameters"":[],""returntype"":""Integer"",""offset"":23,""safe"":false},{""name"":""testExistingVar"",""parameters"":[],""returntype"":""Integer"",""offset"":32,""safe"":false},{""name"":""testMultipleOut"",""parameters"":[],""returntype"":""String"",""offset"":41,""safe"":false},{""name"":""testOutDiscard"",""parameters"":[],""returntype"":""Void"",""offset"":102,""safe"":false},{""name"":""testOutInLoop"",""parameters"":[],""returntype"":""Integer"",""offset"":118,""safe"":false},{""name"":""testOutConditional"",""parameters"":[{""name"":""flag"",""type"":""Boolean""}],""returntype"":""String"",""offset"":248,""safe"":false},{""name"":""testOutSwitch"",""parameters"":[{""name"":""option"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":293,""safe"":false},{""name"":""testNestedOut"",""parameters"":[],""returntype"":""Array"",""offset"":348,""safe"":false},{""name"":""testOutStaticField"",""parameters"":[],""returntype"":""Integer"",""offset"":434,""safe"":false},{""name"":""testOutNamedArguments"",""parameters"":[],""returntype"":""Array"",""offset"":451,""safe"":false},{""name"":""testOutInstanceField"",""parameters"":[],""returntype"":""Array"",""offset"":486,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":558,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""Version"":""3.9.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/TQCVwABACpgQFcAAxphDAVIZWxsb2IIY0AQSmQ05lhkXEAQSmU03VhlXUAJSmYLSmcHEEpnCDTTWWcIWmcHW2ZfCDcAAAwCLCCLXweLDAIsIIteJgoMBFRydWUiCQwFRmFsc2WL2yhACUpjC0pnCRBKYTSXWmcJQFcCABBwEHEidBBKZwo1ff///1hnCmhfCp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWkVtSSLaEBXAAF4JhQQSmcLNf7+//9YZwtfCzcAAEAJSmMLSmcMEEphNe3+//9aZwxfDEBXAQF4cGgRlyQJaBKXJBMiJhBKZw01xv7//1hnDV8NQAlKYwtKYhBKZw41uP7//1lnDl8OQA9AVwEAEEpnEDQNXw9nEHBfEGgSv0BXAAEQSmcPNYn+//9YZw9fDxKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0AQSmBKZxE1SP7//1hnEV8RQAlKZxILSmcTEEpnFDU4/v//WWcUWmcTW2cSXxJfE18UE79AVwEACQwAEBPAcGhnFQlKY0pnFmhnFwtKYkpnGGhnGRBKYUpnGjX8/f//WV8ZEFHQWl8XEVHQW18VElHQaBLOaBHOaBDOE79AVhsQZxFAYq0bGg==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EEplNN1YZV1A
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD5 [2 datoshi]
    /// CALL DD [512 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// STSFLD5 [2 datoshi]
    /// LDSFLD5 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testExistingVar")]
    public abstract BigInteger? TestExistingVar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CUpmC0pnBxBKZwg001lnCFpnB1tmXwg3AAAMAiwgi18HiwwCLCCLXiYKDARUcnVlIgkMBUZhbHNli9soQA==
    /// PUSHF [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD6 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 07 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// CALL D3 [512 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// STSFLD 07 [2 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// STSFLD6 [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// PUSHDATA1 2C20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// LDSFLD 07 [2 datoshi]
    /// CAT [2048 datoshi]
    /// PUSHDATA1 2C20 [8 datoshi]
    /// CAT [2048 datoshi]
    /// LDSFLD6 [2 datoshi]
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
    /// Script: VwEAEEpnEDQNXw9nEHBfEGgSv0A=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 10 [2 datoshi]
    /// CALL 0D [512 datoshi]
    /// LDSFLD 0F [2 datoshi]
    /// STSFLD 10 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDSFLD 10 [2 datoshi]
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
    /// Script: VwABeCYUEEpnCzX+/v//WGcLXws3AABACUpjC0pnDBBKYTXt/v//WmcMXwxA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 14 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// CALL_L FEFEFFFF [512 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// PUSHF [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD3 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 0C [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD1 [2 datoshi]
    /// CALL_L EDFEFFFF [512 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// STSFLD 0C [2 datoshi]
    /// LDSFLD 0C [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutConditional")]
    public abstract string? TestOutConditional(bool? flag);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CUpjC0pnCRBKYTSXWmcJQA==
    /// PUSHF [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD3 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 09 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD1 [2 datoshi]
    /// CALL 97 [512 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// STSFLD 09 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutDiscard")]
    public abstract void TestOutDiscard();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHAQcSJ0EEpnCjV9////WGcKaF8KnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9waUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaRW1JItoQA==
    /// INITSLOT 0200 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// JMP 74 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 0A [2 datoshi]
    /// CALL_L 7DFFFFFF [512 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// STSFLD 0A [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDSFLD 0A [2 datoshi]
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
    /// JMPIF 8B [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutInLoop")]
    public abstract BigInteger? TestOutInLoop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACQwAEBPAcGhnFQlKY0pnFmhnFwtKYkpnGGhnGRBKYUpnGjX8/f//WV8ZEFHQWl8XEVHQW18VElHQaBLOaBHOaBDOE79A
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHF [1 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STSFLD 15 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD3 [2 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 16 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STSFLD 17 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD2 [2 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 18 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STSFLD 19 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD1 [2 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 1A [2 datoshi]
    /// CALL_L FCFDFFFF [512 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// LDSFLD 19 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// LDSFLD 17 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// LDSFLD 15 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutInstanceField")]
    public abstract IList<object>? TestOutInstanceField();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CUpnEgtKZxMQSmcUNTj+//9ZZxRaZxNbZxJfEl8TXxQTv0A=
    /// PUSHF [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 12 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 13 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 14 [2 datoshi]
    /// CALL_L 38FEFFFF [512 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// STSFLD 14 [2 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// STSFLD 13 [2 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// STSFLD 12 [2 datoshi]
    /// LDSFLD 12 [2 datoshi]
    /// LDSFLD 13 [2 datoshi]
    /// LDSFLD 14 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutNamedArguments")]
    public abstract IList<object>? TestOutNamedArguments();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EEpgSmcRNUj+//9YZxFfEUA=
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD0 [2 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 11 [2 datoshi]
    /// CALL_L 48FEFFFF [512 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// STSFLD 11 [2 datoshi]
    /// LDSFLD 11 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutStaticField")]
    public abstract BigInteger? TestOutStaticField();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoEZckCWgSlyQTIiYQSmcNNcb+//9YZw1fDUAJSmMLSmIQSmcONbj+//9ZZw5fDkAPQA==
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
    /// JMPIF 13 [2 datoshi]
    /// JMP 26 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 0D [2 datoshi]
    /// CALL_L C6FEFFFF [512 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// STSFLD 0D [2 datoshi]
    /// LDSFLD 0D [2 datoshi]
    /// RET [0 datoshi]
    /// PUSHF [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD3 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD 0E [2 datoshi]
    /// CALL_L B8FEFFFF [512 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// STSFLD 0E [2 datoshi]
    /// LDSFLD 0E [2 datoshi]
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
    /// Script: EEpkNOZYZFxA
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD4 [2 datoshi]
    /// CALL E6 [512 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// STSFLD4 [2 datoshi]
    /// LDSFLD4 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutVar")]
    public abstract BigInteger? TestOutVar();

    #endregion
}
