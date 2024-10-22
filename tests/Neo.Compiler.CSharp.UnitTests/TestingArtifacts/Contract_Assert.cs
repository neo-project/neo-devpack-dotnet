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
    /// 00 : OpCode.INITSLOT 0100 	-> 64 datoshi
    /// 03 : OpCode.PUSH0 	-> 1 datoshi
    /// 04 : OpCode.STLOC0 	-> 2 datoshi
    /// 05 : OpCode.PUSHT 	-> 1 datoshi
    /// 06 : OpCode.ASSERT 	-> 1 datoshi
    /// 07 : OpCode.PUSH1 	-> 1 datoshi
    /// 08 : OpCode.STLOC0 	-> 2 datoshi
    /// 09 : OpCode.PUSHF 	-> 1 datoshi
    /// 0A : OpCode.ASSERT 	-> 1 datoshi
    /// 0B : OpCode.PUSHINT8 64 	-> 1 datoshi
    /// 0D : OpCode.STLOC0 	-> 2 datoshi
    /// 0E : OpCode.LDLOC0 	-> 2 datoshi
    /// 0F : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testAssertFalse")]
    public abstract BigInteger? TestAssertFalse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHA7ERcRcAxleGNlcHRpb246cTS2cD0FEnA/aEA=
    /// 00 : OpCode.INITSLOT 0200 	-> 64 datoshi
    /// 03 : OpCode.PUSH0 	-> 1 datoshi
    /// 04 : OpCode.STLOC0 	-> 2 datoshi
    /// 05 : OpCode.TRY 1117 	-> 4 datoshi
    /// 08 : OpCode.PUSH1 	-> 1 datoshi
    /// 09 : OpCode.STLOC0 	-> 2 datoshi
    /// 0A : OpCode.PUSHDATA1 657863657074696F6E 	-> 8 datoshi
    /// 15 : OpCode.THROW 	-> 512 datoshi
    /// 16 : OpCode.STLOC1 	-> 2 datoshi
    /// 17 : OpCode.CALL B6 	-> 512 datoshi
    /// 19 : OpCode.STLOC0 	-> 2 datoshi
    /// 1A : OpCode.ENDTRY 05 	-> 4 datoshi
    /// 1C : OpCode.PUSH2 	-> 1 datoshi
    /// 1D : OpCode.STLOC0 	-> 2 datoshi
    /// 1E : OpCode.ENDFINALLY 	-> 4 datoshi
    /// 1F : OpCode.LDLOC0 	-> 2 datoshi
    /// 20 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testAssertInCatch")]
    public abstract BigInteger? TestAssertInCatch();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHA7BwwRcD0AcRJwPQA1m////w==
    /// 00 : OpCode.INITSLOT 0200 	-> 64 datoshi
    /// 03 : OpCode.PUSH0 	-> 1 datoshi
    /// 04 : OpCode.STLOC0 	-> 2 datoshi
    /// 05 : OpCode.TRY 070C 	-> 4 datoshi
    /// 08 : OpCode.PUSH1 	-> 1 datoshi
    /// 09 : OpCode.STLOC0 	-> 2 datoshi
    /// 0A : OpCode.ENDTRY 00 	-> 4 datoshi
    /// 0C : OpCode.STLOC1 	-> 2 datoshi
    /// 0D : OpCode.PUSH2 	-> 1 datoshi
    /// 0E : OpCode.STLOC0 	-> 2 datoshi
    /// 0F : OpCode.ENDTRY 00 	-> 4 datoshi
    /// 11 : OpCode.CALL_L 9BFFFFFF 	-> 512 datoshi
    /// </remarks>
    [DisplayName("testAssertInFinally")]
    public abstract BigInteger? TestAssertInFinally();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHA063ARcGhA
    /// 00 : OpCode.INITSLOT 0100 	-> 64 datoshi
    /// 03 : OpCode.PUSH0 	-> 1 datoshi
    /// 04 : OpCode.STLOC0 	-> 2 datoshi
    /// 05 : OpCode.CALL EB 	-> 512 datoshi
    /// 07 : OpCode.STLOC0 	-> 2 datoshi
    /// 08 : OpCode.PUSH1 	-> 1 datoshi
    /// 09 : OpCode.STLOC0 	-> 2 datoshi
    /// 0A : OpCode.LDLOC0 	-> 2 datoshi
    /// 0B : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testAssertInFunction")]
    public abstract BigInteger? TestAssertInFunction();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHA7CA003HA9CnERcD0FEnA/aEA=
    /// 00 : OpCode.INITSLOT 0200 	-> 64 datoshi
    /// 03 : OpCode.PUSH0 	-> 1 datoshi
    /// 04 : OpCode.STLOC0 	-> 2 datoshi
    /// 05 : OpCode.TRY 080D 	-> 4 datoshi
    /// 08 : OpCode.CALL DC 	-> 512 datoshi
    /// 0A : OpCode.STLOC0 	-> 2 datoshi
    /// 0B : OpCode.ENDTRY 0A 	-> 4 datoshi
    /// 0D : OpCode.STLOC1 	-> 2 datoshi
    /// 0E : OpCode.PUSH1 	-> 1 datoshi
    /// 0F : OpCode.STLOC0 	-> 2 datoshi
    /// 10 : OpCode.ENDTRY 05 	-> 4 datoshi
    /// 12 : OpCode.PUSH2 	-> 1 datoshi
    /// 13 : OpCode.STLOC0 	-> 2 datoshi
    /// 14 : OpCode.ENDFINALLY 	-> 4 datoshi
    /// 15 : OpCode.LDLOC0 	-> 2 datoshi
    /// 16 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testAssertInTry")]
    public abstract BigInteger? TestAssertInTry();

    #endregion
}
