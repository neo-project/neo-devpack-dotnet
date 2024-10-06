using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Abort(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Abort"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAbort"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testAbortMsg"",""parameters"":[],""returntype"":""Integer"",""offset"":6,""safe"":false},{""name"":""testAbortInFunction"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":23,""safe"":false},{""name"":""testAbortInTry"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":35,""safe"":false},{""name"":""testAbortInCatch"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":60,""safe"":false},{""name"":""testAbortInFinally"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":96,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH9XAQAQcDhXAQAQcAwJQUJPUlQgTVNH4FcBARBweCYENOc031cCARBwOwoPeCYENNg00HERcD0FEnA/aEBXAgEQcDsRHBFwDAlleGNlcHRpb246cXgmBDSwNaj///8ScD9XAgEQcDsHDBFwPQBxEnA9AHgmBzWS////NYf///9Auddq9Q=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHA4
    /// 0000 : OpCode.INITSLOT 0100
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
    /// Script: VwIBEHA7ERwRcAxleGNlcHRpb246cXgmBDSwNaj///8ScD8=
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.TRY 111C
    /// 0008 : OpCode.PUSH1
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.PUSHDATA1 657863657074696F6E
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.STLOC1
    /// 0017 : OpCode.LDARG0
    /// 0018 : OpCode.JMPIFNOT 04
    /// 001A : OpCode.CALL B0
    /// 001C : OpCode.CALL_L A8FFFFFF
    /// 0021 : OpCode.PUSH2
    /// 0022 : OpCode.STLOC0
    /// 0023 : OpCode.ENDFINALLY
    /// </remarks>
    [DisplayName("testAbortInCatch")]
    public abstract BigInteger? TestAbortInCatch(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHA7BwwRcD0AcRJwPQB4Jgc1kv///zWH////
    /// 0000 : OpCode.INITSLOT 0201
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
    /// 0011 : OpCode.LDARG0
    /// 0012 : OpCode.JMPIFNOT 07
    /// 0014 : OpCode.CALL_L 92FFFFFF
    /// 0019 : OpCode.CALL_L 87FFFFFF
    /// </remarks>
    [DisplayName("testAbortInFinally")]
    public abstract BigInteger? TestAbortInFinally(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEHB4JgQ05zTf
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.JMPIFNOT 04
    /// 0008 : OpCode.CALL E7
    /// 000A : OpCode.CALL DF
    /// </remarks>
    [DisplayName("testAbortInFunction")]
    public abstract BigInteger? TestAbortInFunction(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBEHA7Cg94JgQ02DTQcRFwPQUScD9oQA==
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.TRY 0A0F
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.JMPIFNOT 04
    /// 000B : OpCode.CALL D8
    /// 000D : OpCode.CALL D0
    /// 000F : OpCode.STLOC1
    /// 0010 : OpCode.PUSH1
    /// 0011 : OpCode.STLOC0
    /// 0012 : OpCode.ENDTRY 05
    /// 0014 : OpCode.PUSH2
    /// 0015 : OpCode.STLOC0
    /// 0016 : OpCode.ENDFINALLY
    /// 0017 : OpCode.LDLOC0
    /// 0018 : OpCode.RET
    /// </remarks>
    [DisplayName("testAbortInTry")]
    public abstract BigInteger? TestAbortInTry(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHAMQUJPUlQgTVNH4A==
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHDATA1 41424F5254204D5347
    /// 0010 : OpCode.ABORTMSG
    /// </remarks>
    [DisplayName("testAbortMsg")]
    public abstract BigInteger? TestAbortMsg();

    #endregion

}
