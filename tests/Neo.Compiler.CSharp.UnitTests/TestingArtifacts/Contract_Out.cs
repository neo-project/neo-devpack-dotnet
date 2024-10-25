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
    /// Script: VwABeCYPEGBYNXT///9YNwAAQBBjC2IQYVtaWTVo////WkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.JMPIFNOT 0F
    /// 06 : OpCode.PUSH0
    /// 07 : OpCode.STSFLD0
    /// 08 : OpCode.LDSFLD0
    /// 09 : OpCode.CALL_L 74FFFFFF
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
    /// 1C : OpCode.CALL_L 68FFFFFF
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
    /// Script: VwIAEHAQcSIQEGBYNI9oWJ5waUqccUVpFbUk72hA
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.JMP 10
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.STSFLD0
    /// 0B : OpCode.LDSFLD0
    /// 0C : OpCode.CALL 8F
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.LDSFLD0
    /// 10 : OpCode.ADD
    /// 11 : OpCode.STLOC0
    /// 12 : OpCode.LDLOC1
    /// 13 : OpCode.DUP
    /// 14 : OpCode.INC
    /// 15 : OpCode.STLOC1
    /// 16 : OpCode.DROP
    /// 17 : OpCode.LDLOC1
    /// 18 : OpCode.PUSH5
    /// 19 : OpCode.LT
    /// 1A : OpCode.JMPIF EF
    /// 1C : OpCode.LDLOC0
    /// 1D : OpCode.RET
    /// </remarks>
    [DisplayName("testOutInLoop")]
    public abstract BigInteger? TestOutInLoop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoEZckCWgSlyQOIhwQYFg1Rv///1hAEGMLYhBhW1pZNT3///9ZQA9A
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
    /// 14 : OpCode.CALL_L 46FFFFFF
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
    /// 24 : OpCode.CALL_L 3DFFFFFF
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
