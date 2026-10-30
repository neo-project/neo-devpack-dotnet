using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Assert(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Assert"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAssertFalse"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testAssertInFunction"",""parameters"":[],""returntype"":""Integer"",""offset"":16,""safe"":false},{""name"":""testAssertInTry"",""parameters"":[],""returntype"":""Integer"",""offset"":28,""safe"":false},{""name"":""testAssertInCatch"",""parameters"":[],""returntype"":""Integer"",""offset"":60,""safe"":false},{""name"":""testAssertInFinally"",""parameters"":[],""returntype"":""Integer"",""offset"":102,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIZXAQAQcAg5EXAJOQBkcGhAVwEAEHA063ARcGhAVwMAQ3IQcDsIFDTacD0RcUNqMgVFIvsRcD0FEnA/aEBXAwBDchBwOxEeEXAMCWV4Y2VwdGlvbjpxQ2oyBUUi+zSkcD0FEnA/aEBXAwBDchBwOwcTEXA9EnFDajIFRSL7EnA9BjSAcD9oQFiowiQ=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHAIORFwCTkAZHBoQA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssertFalse")]
    public abstract BigInteger? TestAssertFalse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAQ3IQcDsRHhFwDAlleGNlcHRpb246cUNqMgVFIvs0pHA9BRJwP2hA
    /// INITSLOT 0300 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 111E [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// CALL A4 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 05 [4 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssertInCatch")]
    public abstract BigInteger? TestAssertInCatch();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAQ3IQcDsHExFwPRJxQ2oyBUUi+xJwPQY0gHA/aEA=
    /// INITSLOT 0300 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 0713 [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 12 [4 datoshi]
    /// STLOC1 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 06 [4 datoshi]
    /// CALL 80 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssertInFinally")]
    public abstract BigInteger? TestAssertInFinally();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHA063ARcGhA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// CALL EB [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssertInFunction")]
    public abstract BigInteger? TestAssertInFunction();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAQ3IQcDsIFDTacD0RcUNqMgVFIvsRcD0FEnA/aEA=
    /// INITSLOT 0300 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 0814 [4 datoshi]
    /// CALL DA [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 11 [4 datoshi]
    /// STLOC1 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 05 [4 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssertInTry")]
    public abstract BigInteger? TestAssertInTry();

    #endregion
}
