using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Abort(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Abort"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAbort"",""parameters"":[],""returntype"":""Integer"",""offset"":148,""safe"":false},{""name"":""testAbortMsg"",""parameters"":[],""returntype"":""Integer"",""offset"":160,""safe"":false},{""name"":""testAbortInFunction"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":172,""safe"":false},{""name"":""testAbortInTry"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":184,""safe"":false},{""name"":""testAbortInCatch"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":196,""safe"":false},{""name"":""testAbortInFinally"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":208,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANpXAQEQcDhXAAF4NANAVwABQFcBARBwDAlBQk9SVCBNU0fgVwECEHB5JgV4NOZ4NNJXAgIQcDsMEXkmBXg01Xg0wXERcD0FEnA/aEBXAgIQcDsRIRFwDAlleGNlcHRpb246cXkmCHg1q////3g1lP///xJwP1cCAhBwOwcMEXA9AHEScD0AeSYIeDWI////eDVx////wko1cP///yNl////wko1ZP///yNq////wko1WP///yNv////wko1TP///yNx////wko1QP///yOA////wko1NP///yKdQCgI690="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAbort")]
    public abstract BigInteger? TestAbort();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : ABORT

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAbortInCatch")]
    public abstract BigInteger? TestAbortInCatch(bool? abortMsg);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : TRY
    // 0008 : PUSH1
    // 0009 : STLOC0
    // 000A : PUSHDATA1
    // 0015 : THROW
    // 0016 : STLOC1
    // 0017 : LDARG1
    // 0018 : JMPIFNOT
    // 001A : LDARG0
    // 001B : CALL_L
    // 0020 : LDARG0
    // 0021 : CALL_L
    // 0026 : PUSH2
    // 0027 : STLOC0
    // 0028 : ENDFINALLY

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAbortInFinally")]
    public abstract BigInteger? TestAbortInFinally(bool? abortMsg);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : TRY
    // 0008 : PUSH1
    // 0009 : STLOC0
    // 000A : ENDTRY
    // 000C : STLOC1
    // 000D : PUSH2
    // 000E : STLOC0
    // 000F : ENDTRY
    // 0011 : LDARG1
    // 0012 : JMPIFNOT
    // 0014 : LDARG0
    // 0015 : CALL_L
    // 001A : LDARG0
    // 001B : CALL_L

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAbortInFunction")]
    public abstract BigInteger? TestAbortInFunction(bool? abortMsg);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : LDARG1
    // 0006 : JMPIFNOT
    // 0008 : LDARG0
    // 0009 : CALL
    // 000B : LDARG0
    // 000C : CALL

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAbortInTry")]
    public abstract BigInteger? TestAbortInTry(bool? abortMsg);
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : TRY
    // 0008 : LDARG1
    // 0009 : JMPIFNOT
    // 000B : LDARG0
    // 000C : CALL
    // 000E : LDARG0
    // 000F : CALL
    // 0011 : STLOC1
    // 0012 : PUSH1
    // 0013 : STLOC0
    // 0014 : ENDTRY
    // 0016 : PUSH2
    // 0017 : STLOC0
    // 0018 : ENDFINALLY
    // 0019 : LDLOC0
    // 001A : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAbortMsg")]
    public abstract BigInteger? TestAbortMsg();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : PUSHDATA1
    // 0010 : ABORTMSG

    #endregion

}
