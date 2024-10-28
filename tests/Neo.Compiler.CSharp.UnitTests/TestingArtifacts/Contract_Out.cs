using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Out(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Out"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testOutVar"",""parameters"":[],""returntype"":""Integer"",""offset"":23,""safe"":false},{""name"":""testExistingVar"",""parameters"":[],""returntype"":""Integer"",""offset"":30,""safe"":false},{""name"":""testMultipleOut"",""parameters"":[],""returntype"":""String"",""offset"":40,""safe"":false},{""name"":""testOutDiscard"",""parameters"":[],""returntype"":""Void"",""offset"":89,""safe"":false},{""name"":""testOutInLoop"",""parameters"":[],""returntype"":""Integer"",""offset"":101,""safe"":false},{""name"":""testOutConditional"",""parameters"":[{""name"":""flag"",""type"":""Boolean""}],""returntype"":""String"",""offset"":131,""safe"":false},{""name"":""testOutSwitch"",""parameters"":[{""name"":""option"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":166,""safe"":false},{""name"":""testNestedOut"",""parameters"":[],""returntype"":""Array"",""offset"":211,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":242,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA9VcAAQAqYEBXAAMaYQwFSGVsbG9iCGNAEGBYNOZYQFcBAGhgWDTcWEAQYwtiEGFbWlk01lk3AAAMAiwgi1qLDAIsIItbJgoMBFRydWUiCQwFRmFsc2WL2yhAEGMLYhBhW1pZNKVAVwIAEHAQcSIQEGBYNI9oWJ5waUqccUVpFbUk72hAVwABeCYPEGBYNXT///9YNwAAQBBjC2IQYVtaWTVo////WkBXAQF4cGgRlyQJaBKXJA4iHBBgWDVG////WEAQYwtiEGFbWlk1Pf///1lAD0BXAQAQZFw0CHBcaBK/QFcAAVxkWDUZ////WGRcEqBAVgVAlht2RQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAaGBYNNxYQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.LDLOC0 [2 datoshi]
    /// 04 : OpCode.STSFLD0 [2 datoshi]
    /// 05 : OpCode.LDSFLD0 [2 datoshi]
    /// 06 : OpCode.CALL DC [512 datoshi]
    /// 08 : OpCode.LDSFLD0 [2 datoshi]
    /// 09 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testExistingVar")]
    public abstract BigInteger? TestExistingVar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGMLYhBhW1pZNNZZNwAADCwgi1qLDCwgi1smCgxUcnVlIgkMRmFsc2WL2yhA
    /// 00 : OpCode.PUSH0 [1 datoshi]
    /// 01 : OpCode.STSFLD3 [2 datoshi]
    /// 02 : OpCode.PUSHNULL [1 datoshi]
    /// 03 : OpCode.STSFLD2 [2 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.STSFLD1 [2 datoshi]
    /// 06 : OpCode.LDSFLD3 [2 datoshi]
    /// 07 : OpCode.LDSFLD2 [2 datoshi]
    /// 08 : OpCode.LDSFLD1 [2 datoshi]
    /// 09 : OpCode.CALL D6 [512 datoshi]
    /// 0B : OpCode.LDSFLD1 [2 datoshi]
    /// 0C : OpCode.CALLT 0000 [32768 datoshi]
    /// 0F : OpCode.PUSHDATA1 2C20 [8 datoshi]
    /// 13 : OpCode.CAT [2048 datoshi]
    /// 14 : OpCode.LDSFLD2 [2 datoshi]
    /// 15 : OpCode.CAT [2048 datoshi]
    /// 16 : OpCode.PUSHDATA1 2C20 [8 datoshi]
    /// 1A : OpCode.CAT [2048 datoshi]
    /// 1B : OpCode.LDSFLD3 [2 datoshi]
    /// 1C : OpCode.JMPIFNOT 0A [2 datoshi]
    /// 1E : OpCode.PUSHDATA1 54727565 [8 datoshi]
    /// 24 : OpCode.JMP 09 [2 datoshi]
    /// 26 : OpCode.PUSHDATA1 46616C7365 [8 datoshi]
    /// 2D : OpCode.CAT [2048 datoshi]
    /// 2E : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 30 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMultipleOut")]
    public abstract string? TestMultipleOut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEGRcNAhwXGgSv0A=
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STSFLD4 [2 datoshi]
    /// 05 : OpCode.LDSFLD4 [2 datoshi]
    /// 06 : OpCode.CALL 08 [512 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.LDSFLD4 [2 datoshi]
    /// 0A : OpCode.LDLOC0 [2 datoshi]
    /// 0B : OpCode.PUSH2 [1 datoshi]
    /// 0C : OpCode.PACKSTRUCT [2048 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNestedOut")]
    public abstract IList<object>? TestNestedOut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCYPEGBYNXT///9YNwAAQBBjC2IQYVtaWTVo////WkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.JMPIFNOT 0F [2 datoshi]
    /// 06 : OpCode.PUSH0 [1 datoshi]
    /// 07 : OpCode.STSFLD0 [2 datoshi]
    /// 08 : OpCode.LDSFLD0 [2 datoshi]
    /// 09 : OpCode.CALL_L 74FFFFFF [512 datoshi]
    /// 0E : OpCode.LDSFLD0 [2 datoshi]
    /// 0F : OpCode.CALLT 0000 [32768 datoshi]
    /// 12 : OpCode.RET [0 datoshi]
    /// 13 : OpCode.PUSH0 [1 datoshi]
    /// 14 : OpCode.STSFLD3 [2 datoshi]
    /// 15 : OpCode.PUSHNULL [1 datoshi]
    /// 16 : OpCode.STSFLD2 [2 datoshi]
    /// 17 : OpCode.PUSH0 [1 datoshi]
    /// 18 : OpCode.STSFLD1 [2 datoshi]
    /// 19 : OpCode.LDSFLD3 [2 datoshi]
    /// 1A : OpCode.LDSFLD2 [2 datoshi]
    /// 1B : OpCode.LDSFLD1 [2 datoshi]
    /// 1C : OpCode.CALL_L 68FFFFFF [512 datoshi]
    /// 21 : OpCode.LDSFLD2 [2 datoshi]
    /// 22 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutConditional")]
    public abstract string? TestOutConditional(bool? flag);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGMLYhBhW1pZNKVA
    /// 00 : OpCode.PUSH0 [1 datoshi]
    /// 01 : OpCode.STSFLD3 [2 datoshi]
    /// 02 : OpCode.PUSHNULL [1 datoshi]
    /// 03 : OpCode.STSFLD2 [2 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.STSFLD1 [2 datoshi]
    /// 06 : OpCode.LDSFLD3 [2 datoshi]
    /// 07 : OpCode.LDSFLD2 [2 datoshi]
    /// 08 : OpCode.LDSFLD1 [2 datoshi]
    /// 09 : OpCode.CALL A5 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutDiscard")]
    public abstract void TestOutDiscard();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHAQcSIQEGBYNI9oWJ5waUqccUVpFbUk72hA
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.STLOC1 [2 datoshi]
    /// 07 : OpCode.JMP 10 [2 datoshi]
    /// 09 : OpCode.PUSH0 [1 datoshi]
    /// 0A : OpCode.STSFLD0 [2 datoshi]
    /// 0B : OpCode.LDSFLD0 [2 datoshi]
    /// 0C : OpCode.CALL 8F [512 datoshi]
    /// 0E : OpCode.LDLOC0 [2 datoshi]
    /// 0F : OpCode.LDSFLD0 [2 datoshi]
    /// 10 : OpCode.ADD [8 datoshi]
    /// 11 : OpCode.STLOC0 [2 datoshi]
    /// 12 : OpCode.LDLOC1 [2 datoshi]
    /// 13 : OpCode.DUP [2 datoshi]
    /// 14 : OpCode.INC [4 datoshi]
    /// 15 : OpCode.STLOC1 [2 datoshi]
    /// 16 : OpCode.DROP [2 datoshi]
    /// 17 : OpCode.LDLOC1 [2 datoshi]
    /// 18 : OpCode.PUSH5 [1 datoshi]
    /// 19 : OpCode.LT [8 datoshi]
    /// 1A : OpCode.JMPIF EF [2 datoshi]
    /// 1C : OpCode.LDLOC0 [2 datoshi]
    /// 1D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutInLoop")]
    public abstract BigInteger? TestOutInLoop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoEZckCWgSlyQOIhwQYFg1Rv///1hAEGMLYhBhW1pZNT3///9ZQA9A
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.PUSH1 [1 datoshi]
    /// 07 : OpCode.EQUAL [32 datoshi]
    /// 08 : OpCode.JMPIF 09 [2 datoshi]
    /// 0A : OpCode.LDLOC0 [2 datoshi]
    /// 0B : OpCode.PUSH2 [1 datoshi]
    /// 0C : OpCode.EQUAL [32 datoshi]
    /// 0D : OpCode.JMPIF 0E [2 datoshi]
    /// 0F : OpCode.JMP 1C [2 datoshi]
    /// 11 : OpCode.PUSH0 [1 datoshi]
    /// 12 : OpCode.STSFLD0 [2 datoshi]
    /// 13 : OpCode.LDSFLD0 [2 datoshi]
    /// 14 : OpCode.CALL_L 46FFFFFF [512 datoshi]
    /// 19 : OpCode.LDSFLD0 [2 datoshi]
    /// 1A : OpCode.RET [0 datoshi]
    /// 1B : OpCode.PUSH0 [1 datoshi]
    /// 1C : OpCode.STSFLD3 [2 datoshi]
    /// 1D : OpCode.PUSHNULL [1 datoshi]
    /// 1E : OpCode.STSFLD2 [2 datoshi]
    /// 1F : OpCode.PUSH0 [1 datoshi]
    /// 20 : OpCode.STSFLD1 [2 datoshi]
    /// 21 : OpCode.LDSFLD3 [2 datoshi]
    /// 22 : OpCode.LDSFLD2 [2 datoshi]
    /// 23 : OpCode.LDSFLD1 [2 datoshi]
    /// 24 : OpCode.CALL_L 3DFFFFFF [512 datoshi]
    /// 29 : OpCode.LDSFLD1 [2 datoshi]
    /// 2A : OpCode.RET [0 datoshi]
    /// 2B : OpCode.PUSHM1 [1 datoshi]
    /// 2C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutSwitch")]
    public abstract BigInteger? TestOutSwitch(BigInteger? option);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGBYNOZYQA==
    /// 00 : OpCode.PUSH0 [1 datoshi]
    /// 01 : OpCode.STSFLD0 [2 datoshi]
    /// 02 : OpCode.LDSFLD0 [2 datoshi]
    /// 03 : OpCode.CALL E6 [512 datoshi]
    /// 05 : OpCode.LDSFLD0 [2 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOutVar")]
    public abstract BigInteger? TestOutVar();

    #endregion
}
