using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Abort(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Abort"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAbort"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testAbortMsg"",""parameters"":[],""returntype"":""Integer"",""offset"":6,""safe"":false},{""name"":""testAbortInFunction"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":23,""safe"":false},{""name"":""testAbortInTry"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":35,""safe"":false},{""name"":""testAbortInCatch"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":60,""safe"":false},{""name"":""testAbortInFinally"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":93,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHZXAQAQcDhXAQAQcAwJQUJPUlQgTVNH4FcBARBweCYENOc031cCARBwOwoPeCYENNg00HERcD0FEnA/aEBXAgEQcDsRGRFwDAlleGNlcHRpb246cXgmBDSwNKgScD9XAgEQcDsHDBFwPQBxEnA9AHgmBDSVNI1AxCfDrA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHA4
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ABORT [0 datoshi]
    /// </remarks>
    [DisplayName("testAbort")]
    public abstract BigInteger? TestAbort();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHA7ERkRcAwJZXhjZXB0aW9uOnF4JgQ0sDSoEnA/
    /// INITSLOT 0201 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 1119 [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// CALL B0 [512 datoshi]
    /// CALL A8 [512 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// </remarks>
    [DisplayName("testAbortInCatch")]
    public abstract BigInteger? TestAbortInCatch(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHA7BwwRcD0AcRJwPQB4JgQ0lTSN
    /// INITSLOT 0201 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 070C [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 00 [4 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 00 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// CALL 95 [512 datoshi]
    /// CALL 8D [512 datoshi]
    /// </remarks>
    [DisplayName("testAbortInFinally")]
    public abstract BigInteger? TestAbortInFinally(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEHB4JgQ05zTf
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// CALL E7 [512 datoshi]
    /// CALL DF [512 datoshi]
    /// </remarks>
    [DisplayName("testAbortInFunction")]
    public abstract BigInteger? TestAbortInFunction(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHA7Cg94JgQ02DTQcRFwPQUScD9oQA==
    /// INITSLOT 0201 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 0A0F [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// CALL D8 [512 datoshi]
    /// CALL D0 [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 05 [4 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAbortInTry")]
    public abstract BigInteger? TestAbortInTry(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHAMCUFCT1JUIE1TR+A=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 41424F5254204D5347 [8 datoshi]
    /// ABORTMSG [0 datoshi]
    /// </remarks>
    [DisplayName("testAbortMsg")]
    public abstract BigInteger? TestAbortMsg();

    #endregion
}
