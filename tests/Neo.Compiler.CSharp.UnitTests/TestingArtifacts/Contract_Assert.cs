using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Assert(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Assert"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAssertFalse"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testAssertInFunction"",""parameters"":[],""returntype"":""Integer"",""offset"":16,""safe"":false},{""name"":""testAssertInTry"",""parameters"":[],""returntype"":""Integer"",""offset"":28,""safe"":false},{""name"":""testAssertInCatch"",""parameters"":[],""returntype"":""Integer"",""offset"":51,""safe"":false},{""name"":""testAssertInFinally"",""parameters"":[],""returntype"":""Integer"",""offset"":84,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGtXAQAQcAg5EXAJOQBkcGhAVwEAEHA063ARcGhAVwIAEHA7CA003HA9CnERcD0FEnA/aEBXAgAQcDsRFxFwDAlleGNlcHRpb246cTS2cD0FEnA/aEBXAgAQcDsHDBFwPQBxEnA9ADWb////QJ1a8Js="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHAIORFwCTkAZHBoQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSHT
    /// 06 : OpCode.ASSERT
    /// 07 : OpCode.PUSH1
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.PUSHF
    /// 0A : OpCode.ASSERT
    /// 0B : OpCode.PUSHINT8 64
    /// 0D : OpCode.STLOC0
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.RET
    /// </remarks>
    [DisplayName("testAssertFalse")]
    public abstract BigInteger? TestAssertFalse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHA7ERcRcAxleGNlcHRpb246cTS2cD0FEnA/aEA=
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.TRY 1117
    /// 08 : OpCode.PUSH1
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.PUSHDATA1 657863657074696F6E
    /// 15 : OpCode.THROW
    /// 16 : OpCode.STLOC1
    /// 17 : OpCode.CALL B6
    /// 19 : OpCode.STLOC0
    /// 1A : OpCode.ENDTRY 05
    /// 1C : OpCode.PUSH2
    /// 1D : OpCode.STLOC0
    /// 1E : OpCode.ENDFINALLY
    /// 1F : OpCode.LDLOC0
    /// 20 : OpCode.RET
    /// </remarks>
    [DisplayName("testAssertInCatch")]
    public abstract BigInteger? TestAssertInCatch();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHA7BwwRcD0AcRJwPQA1m////w==
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.TRY 070C
    /// 08 : OpCode.PUSH1
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.ENDTRY 00
    /// 0C : OpCode.STLOC1
    /// 0D : OpCode.PUSH2
    /// 0E : OpCode.STLOC0
    /// 0F : OpCode.ENDTRY 00
    /// 11 : OpCode.CALL_L 9BFFFFFF
    /// </remarks>
    [DisplayName("testAssertInFinally")]
    public abstract BigInteger? TestAssertInFinally();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHA063ARcGhA
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.CALL EB
    /// 07 : OpCode.STLOC0
    /// 08 : OpCode.PUSH1
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.LDLOC0
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("testAssertInFunction")]
    public abstract BigInteger? TestAssertInFunction();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHA7CA003HA9CnERcD0FEnA/aEA=
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.TRY 080D
    /// 08 : OpCode.CALL DC
    /// 0A : OpCode.STLOC0
    /// 0B : OpCode.ENDTRY 0A
    /// 0D : OpCode.STLOC1
    /// 0E : OpCode.PUSH1
    /// 0F : OpCode.STLOC0
    /// 10 : OpCode.ENDTRY 05
    /// 12 : OpCode.PUSH2
    /// 13 : OpCode.STLOC0
    /// 14 : OpCode.ENDFINALLY
    /// 15 : OpCode.LDLOC0
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("testAssertInTry")]
    public abstract BigInteger? TestAssertInTry();

    #endregion
}
