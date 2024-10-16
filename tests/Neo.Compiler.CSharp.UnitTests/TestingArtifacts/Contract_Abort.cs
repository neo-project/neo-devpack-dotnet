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
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.ABORT
    /// </remarks>
    [DisplayName("testAbort")]
    public abstract BigInteger? TestAbort();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHA7ERkRcAxleGNlcHRpb246cXgmBDSwNKgScD8=
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.TRY 1119
    /// 08 : OpCode.PUSH1
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.PUSHDATA1 657863657074696F6E
    /// 15 : OpCode.THROW
    /// 16 : OpCode.STLOC1
    /// 17 : OpCode.LDARG0
    /// 18 : OpCode.JMPIFNOT 04
    /// 1A : OpCode.CALL B0
    /// 1C : OpCode.CALL A8
    /// 1E : OpCode.PUSH2
    /// 1F : OpCode.STLOC0
    /// 20 : OpCode.ENDFINALLY
    /// </remarks>
    [DisplayName("testAbortInCatch")]
    public abstract BigInteger? TestAbortInCatch(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHA7BwwRcD0AcRJwPQB4
    /// 00 : OpCode.INITSLOT 0201
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
    /// 11 : OpCode.LDARG0
    /// </remarks>
    [DisplayName("testAbortInFinally")]
    public abstract BigInteger? TestAbortInFinally(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEHB4
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// </remarks>
    [DisplayName("testAbortInFunction")]
    public abstract BigInteger? TestAbortInFunction(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHA7Cg94JgQ02DTQcRFwPQUScD9oQA==
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.TRY 0A0F
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.JMPIFNOT 04
    /// 0B : OpCode.CALL D8
    /// 0D : OpCode.CALL D0
    /// 0F : OpCode.STLOC1
    /// 10 : OpCode.PUSH1
    /// 11 : OpCode.STLOC0
    /// 12 : OpCode.ENDTRY 05
    /// 14 : OpCode.PUSH2
    /// 15 : OpCode.STLOC0
    /// 16 : OpCode.ENDFINALLY
    /// 17 : OpCode.LDLOC0
    /// 18 : OpCode.RET
    /// </remarks>
    [DisplayName("testAbortInTry")]
    public abstract BigInteger? TestAbortInTry(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHAMQUJPUlQgTVNH4A==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSHDATA1 41424F5254204D5347
    /// 10 : OpCode.ABORTMSG
    /// </remarks>
    [DisplayName("testAbortMsg")]
    public abstract BigInteger? TestAbortMsg();

    #endregion
}
