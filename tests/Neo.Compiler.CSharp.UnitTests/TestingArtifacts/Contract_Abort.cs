using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHZXAQAQcDhXAQAQcAwJQUJPUlQgTVNH4FcBARBweCYENOc031cCARBwOwoPeCYENNg00HERcD0FEnA/aEBXAgEQcDsRGRFwDAlleGNlcHRpb246cXgmBDSwNKgScD9XAgEQcDsHDBFwPQBxEnA9AHgmBDSVNI1AxCfDrA==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHA4
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : ABORT [0 datoshi]
    /// </remarks>
    [DisplayName("testAbort")]
    public abstract BigInteger? TestAbort();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHA7ERkRcAwJZXhjZXB0aW9uOnF4JgQ0sDSoEnA/
    /// 00 : INITSLOT 0201 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : TRY 1119 [4 datoshi]
    /// 08 : PUSH1 [1 datoshi]
    /// 09 : STLOC0 [2 datoshi]
    /// 0A : PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 15 : THROW [512 datoshi]
    /// 16 : STLOC1 [2 datoshi]
    /// 17 : LDARG0 [2 datoshi]
    /// 18 : JMPIFNOT 04 [2 datoshi]
    /// 1A : CALL B0 [512 datoshi]
    /// 1C : CALL A8 [512 datoshi]
    /// 1E : PUSH2 [1 datoshi]
    /// 1F : STLOC0 [2 datoshi]
    /// 20 : ENDFINALLY [4 datoshi]
    /// </remarks>
    [DisplayName("testAbortInCatch")]
    public abstract BigInteger? TestAbortInCatch(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHA7BwwRcD0AcRJwPQB4JgQ0lTSN
    /// 00 : INITSLOT 0201 [64 datoshi]
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
    /// 11 : LDARG0 [2 datoshi]
    /// 12 : JMPIFNOT 04 [2 datoshi]
    /// 14 : CALL 95 [512 datoshi]
    /// 16 : CALL 8D [512 datoshi]
    /// </remarks>
    [DisplayName("testAbortInFinally")]
    public abstract BigInteger? TestAbortInFinally(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEHB4JgQ05zTf
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDARG0 [2 datoshi]
    /// 06 : JMPIFNOT 04 [2 datoshi]
    /// 08 : CALL E7 [512 datoshi]
    /// 0A : CALL DF [512 datoshi]
    /// </remarks>
    [DisplayName("testAbortInFunction")]
    public abstract BigInteger? TestAbortInFunction(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHA7Cg94JgQ02DTQcRFwPQUScD9oQA==
    /// 00 : INITSLOT 0201 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : TRY 0A0F [4 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : JMPIFNOT 04 [2 datoshi]
    /// 0B : CALL D8 [512 datoshi]
    /// 0D : CALL D0 [512 datoshi]
    /// 0F : STLOC1 [2 datoshi]
    /// 10 : PUSH1 [1 datoshi]
    /// 11 : STLOC0 [2 datoshi]
    /// 12 : ENDTRY 05 [4 datoshi]
    /// 14 : PUSH2 [1 datoshi]
    /// 15 : STLOC0 [2 datoshi]
    /// 16 : ENDFINALLY [4 datoshi]
    /// 17 : LDLOC0 [2 datoshi]
    /// 18 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAbortInTry")]
    public abstract BigInteger? TestAbortInTry(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHAMCUFCT1JUIE1TR+A=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHDATA1 41424F5254204D5347 [8 datoshi]
    /// 10 : ABORTMSG [0 datoshi]
    /// </remarks>
    [DisplayName("testAbortMsg")]
    public abstract BigInteger? TestAbortMsg();

    #endregion
}
