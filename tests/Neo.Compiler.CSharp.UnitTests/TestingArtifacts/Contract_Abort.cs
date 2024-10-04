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
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.ABORT
    /// </remarks>
    [DisplayName("testAbort")]
    public abstract BigInteger? TestAbort();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0202
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.TRY 1121
    /// 0008 : OpCode.PUSH1
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.PUSHDATA1 657863657074696F6E
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.STLOC1
    /// 0017 : OpCode.LDARG1
    /// 0018 : OpCode.JMPIFNOT 08
    /// 001A : OpCode.LDARG0
    /// 001B : OpCode.CALL_L ABFFFFFF
    /// 0020 : OpCode.LDARG0
    /// 0021 : OpCode.CALL_L 94FFFFFF
    /// 0026 : OpCode.PUSH2
    /// 0027 : OpCode.STLOC0
    /// 0028 : OpCode.ENDFINALLY
    /// </remarks>
    [DisplayName("testAbortInCatch")]
    public abstract BigInteger? TestAbortInCatch(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0202
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
    /// 0011 : OpCode.LDARG1
    /// 0012 : OpCode.JMPIFNOT 08
    /// 0014 : OpCode.LDARG0
    /// 0015 : OpCode.CALL_L 88FFFFFF
    /// 001A : OpCode.LDARG0
    /// 001B : OpCode.CALL_L 71FFFFFF
    /// </remarks>
    [DisplayName("testAbortInFinally")]
    public abstract BigInteger? TestAbortInFinally(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0102
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG1
    /// 0006 : OpCode.JMPIFNOT 05
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.CALL E6
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.CALL D2
    /// </remarks>
    [DisplayName("testAbortInFunction")]
    public abstract BigInteger? TestAbortInFunction(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0202
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.TRY 0C11
    /// 0008 : OpCode.LDARG1
    /// 0009 : OpCode.JMPIFNOT 05
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.CALL D5
    /// 000E : OpCode.LDARG0
    /// 000F : OpCode.CALL C1
    /// 0011 : OpCode.STLOC1
    /// 0012 : OpCode.PUSH1
    /// 0013 : OpCode.STLOC0
    /// 0014 : OpCode.ENDTRY 05
    /// 0016 : OpCode.PUSH2
    /// 0017 : OpCode.STLOC0
    /// 0018 : OpCode.ENDFINALLY
    /// 0019 : OpCode.LDLOC0
    /// 001A : OpCode.RET
    /// </remarks>
    [DisplayName("testAbortInTry")]
    public abstract BigInteger? TestAbortInTry(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHDATA1 41424F5254204D5347
    /// 0010 : OpCode.ABORTMSG
    /// </remarks>
    [DisplayName("testAbortMsg")]
    public abstract BigInteger? TestAbortMsg();

    #endregion

}
