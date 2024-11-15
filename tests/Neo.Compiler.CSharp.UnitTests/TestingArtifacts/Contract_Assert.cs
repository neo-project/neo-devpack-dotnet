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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGhXAQAQcAg5EXAJOQBkcGhAVwEAEHA063ARcGhAVwIAEHA7CA003HA9CnERcD0FEnA/aEBXAgAQcDsRFxFwDAlleGNlcHRpb246cTS2cD0FEnA/aEBXAgAQcDsHDBFwPQBxEnA9ADSbQDbfjGM="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHAIORFwCTkAZHBoQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHT [1 datoshi]
    /// 06 : ASSERT [1 datoshi]
    /// 07 : PUSH1 [1 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : PUSHF [1 datoshi]
    /// 0A : ASSERT [1 datoshi]
    /// 0B : PUSHINT8 64 [1 datoshi]
    /// 0D : STLOC0 [2 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssertFalse")]
    public abstract BigInteger? TestAssertFalse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHA7ERcRcAwJZXhjZXB0aW9uOnE0tnA9BRJwP2hA
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : TRY 1117 [4 datoshi]
    /// 08 : PUSH1 [1 datoshi]
    /// 09 : STLOC0 [2 datoshi]
    /// 0A : PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 15 : THROW [512 datoshi]
    /// 16 : STLOC1 [2 datoshi]
    /// 17 : CALL B6 [512 datoshi]
    /// 19 : STLOC0 [2 datoshi]
    /// 1A : ENDTRY 05 [4 datoshi]
    /// 1C : PUSH2 [1 datoshi]
    /// 1D : STLOC0 [2 datoshi]
    /// 1E : ENDFINALLY [4 datoshi]
    /// 1F : LDLOC0 [2 datoshi]
    /// 20 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssertInCatch")]
    public abstract BigInteger? TestAssertInCatch();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHA7BwwRcD0AcRJwPQA0mw==
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : TRY 070C [4 datoshi]
    /// 08 : PUSH1 [1 datoshi]
    /// 09 : STLOC0 [2 datoshi]
    /// 0A : ENDTRY 00 [4 datoshi]
    /// 0C : STLOC1 [2 datoshi]
    /// 0D : PUSH2 [1 datoshi]
    /// 0E : STLOC0 [2 datoshi]
    /// 0F : ENDTRY 00 [4 datoshi]
    /// 11 : CALL 9B [512 datoshi]
    /// </remarks>
    [DisplayName("testAssertInFinally")]
    public abstract BigInteger? TestAssertInFinally();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHA063ARcGhA
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : CALL EB [512 datoshi]
    /// 07 : STLOC0 [2 datoshi]
    /// 08 : PUSH1 [1 datoshi]
    /// 09 : STLOC0 [2 datoshi]
    /// 0A : LDLOC0 [2 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssertInFunction")]
    public abstract BigInteger? TestAssertInFunction();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHA7CA003HA9CnERcD0FEnA/aEA=
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : TRY 080D [4 datoshi]
    /// 08 : CALL DC [512 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : ENDTRY 0A [4 datoshi]
    /// 0D : STLOC1 [2 datoshi]
    /// 0E : PUSH1 [1 datoshi]
    /// 0F : STLOC0 [2 datoshi]
    /// 10 : ENDTRY 05 [4 datoshi]
    /// 12 : PUSH2 [1 datoshi]
    /// 13 : STLOC0 [2 datoshi]
    /// 14 : ENDFINALLY [4 datoshi]
    /// 15 : LDLOC0 [2 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssertInTry")]
    public abstract BigInteger? TestAssertInTry();

    #endregion
}
