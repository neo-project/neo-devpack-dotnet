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
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHT
    /// 0006 : OpCode.ASSERT
    /// 0007 : OpCode.PUSH1
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.PUSHF
    /// 000A : OpCode.ASSERT
    /// 000B : OpCode.PUSHINT8 64
    /// 000D : OpCode.STLOC0
    /// 000E : OpCode.LDLOC0
    /// 000F : OpCode.RET
    /// </remarks>
    [DisplayName("testAssertFalse")]
    public abstract BigInteger? TestAssertFalse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHA7ERcRcAxleGNlcHRpb246cTS2cD0FEnA/aEA=
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.TRY 1117
    /// 0008 : OpCode.PUSH1
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.PUSHDATA1 657863657074696F6E
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.STLOC1
    /// 0017 : OpCode.CALL B6
    /// 0019 : OpCode.STLOC0
    /// 001A : OpCode.ENDTRY 05
    /// 001C : OpCode.PUSH2
    /// 001D : OpCode.STLOC0
    /// 001E : OpCode.ENDFINALLY
    /// 001F : OpCode.LDLOC0
    /// 0020 : OpCode.RET
    /// </remarks>
    [DisplayName("testAssertInCatch")]
    public abstract BigInteger? TestAssertInCatch();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHA7BwwRcD0AcRJwPQA1m////w==
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.TRY 070C
    /// 0008 : OpCode.PUSH1
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.ENDTRY 00
    /// 000C : OpCode.STLOC1
    /// 000D : OpCode.PUSH2
    /// 000E : OpCode.STLOC0
    /// 000F : OpCode.ENDTRY 00
    /// 0011 : OpCode.CALL_L 9BFFFFFF
    /// </remarks>
    [DisplayName("testAssertInFinally")]
    public abstract BigInteger? TestAssertInFinally();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHA063ARcGhA
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.CALL EB
    /// 0007 : OpCode.STLOC0
    /// 0008 : OpCode.PUSH1
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.LDLOC0
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("testAssertInFunction")]
    public abstract BigInteger? TestAssertInFunction();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHA7CA003HA9CnERcD0FEnA/aEA=
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.TRY 080D
    /// 0008 : OpCode.CALL DC
    /// 000A : OpCode.STLOC0
    /// 000B : OpCode.ENDTRY 0A
    /// 000D : OpCode.STLOC1
    /// 000E : OpCode.PUSH1
    /// 000F : OpCode.STLOC0
    /// 0010 : OpCode.ENDTRY 05
    /// 0012 : OpCode.PUSH2
    /// 0013 : OpCode.STLOC0
    /// 0014 : OpCode.ENDFINALLY
    /// 0015 : OpCode.LDLOC0
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("testAssertInTry")]
    public abstract BigInteger? TestAssertInTry();

    #endregion

}
