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
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.ABORT [0 datoshi]
    /// </remarks>
    [DisplayName("testAbort")]
    public abstract BigInteger? TestAbort();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHA7ERkRcAwJZXhjZXB0aW9uOnF4JgQ0sDSoEnA/
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 1119 [4 datoshi]
    /// 08 : OpCode.PUSH1 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.PUSHDATA1 657863657074696F6E [8 datoshi]
    /// 15 : OpCode.THROW [512 datoshi]
    /// 16 : OpCode.STLOC1 [2 datoshi]
    /// 17 : OpCode.LDARG0 [2 datoshi]
    /// 18 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 1A : OpCode.CALL B0 [512 datoshi]
    /// 1C : OpCode.CALL A8 [512 datoshi]
    /// 1E : OpCode.PUSH2 [1 datoshi]
    /// 1F : OpCode.STLOC0 [2 datoshi]
    /// 20 : OpCode.ENDFINALLY [4 datoshi]
    /// </remarks>
    [DisplayName("testAbortInCatch")]
    public abstract BigInteger? TestAbortInCatch(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHA7BwwRcD0AcRJwPQB4
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 070C [4 datoshi]
    /// 08 : OpCode.PUSH1 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.ENDTRY 00 [4 datoshi]
    /// 0C : OpCode.STLOC1 [2 datoshi]
    /// 0D : OpCode.PUSH2 [1 datoshi]
    /// 0E : OpCode.STLOC0 [2 datoshi]
    /// 0F : OpCode.ENDTRY 00 [4 datoshi]
    /// 11 : OpCode.LDARG0 [2 datoshi]
    /// </remarks>
    [DisplayName("testAbortInFinally")]
    public abstract BigInteger? TestAbortInFinally(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEHB4
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// </remarks>
    [DisplayName("testAbortInFunction")]
    public abstract BigInteger? TestAbortInFunction(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHA7Cg94JgQ02DTQcRFwPQUScD9oQA==
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 0A0F [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 0B : OpCode.CALL D8 [512 datoshi]
    /// 0D : OpCode.CALL D0 [512 datoshi]
    /// 0F : OpCode.STLOC1 [2 datoshi]
    /// 10 : OpCode.PUSH1 [1 datoshi]
    /// 11 : OpCode.STLOC0 [2 datoshi]
    /// 12 : OpCode.ENDTRY 05 [4 datoshi]
    /// 14 : OpCode.PUSH2 [1 datoshi]
    /// 15 : OpCode.STLOC0 [2 datoshi]
    /// 16 : OpCode.ENDFINALLY [4 datoshi]
    /// 17 : OpCode.LDLOC0 [2 datoshi]
    /// 18 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAbortInTry")]
    public abstract BigInteger? TestAbortInTry(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHAMCUFCT1JUIE1TR+A=
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHDATA1 41424F5254204D5347 [8 datoshi]
    /// 10 : OpCode.ABORTMSG [0 datoshi]
    /// </remarks>
    [DisplayName("testAbortMsg")]
    public abstract BigInteger? TestAbortMsg();

    #endregion
}
