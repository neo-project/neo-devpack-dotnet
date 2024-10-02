using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Assert(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Assert"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAssertFalse"",""parameters"":[],""returntype"":""Integer"",""offset"":121,""safe"":false},{""name"":""testAssertInFunction"",""parameters"":[],""returntype"":""Integer"",""offset"":133,""safe"":false},{""name"":""testAssertInTry"",""parameters"":[],""returntype"":""Integer"",""offset"":145,""safe"":false},{""name"":""testAssertInCatch"",""parameters"":[],""returntype"":""Integer"",""offset"":157,""safe"":false},{""name"":""testAssertInFinally"",""parameters"":[],""returntype"":""Integer"",""offset"":166,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALBXAQEQcAg5EXAJOQBkcGhAVwABeDQDQFcAAUBXAQEQcHg033ARcGhAVwIBEHA7CQ54NM9wPQpxEXA9BRJwP2hAVwIBEHA7ERgRcAwJZXhjZXB0aW9uOnF4NKhwPQUScD9oQFcCARBwOwcMEXA9AHEScD0AeDWM////wko1lf///yOA////wko1if///yOP////wko1ff///yOQ////wko1cf///yKcwko1aP///yK1QF5bPYA="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertFalse")]
    public abstract BigInteger? TestAssertFalse();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : PUSHT
    // 0006 : ASSERT
    // 0007 : PUSH1
    // 0008 : STLOC0
    // 0009 : PUSHF
    // 000A : ASSERT
    // 000B : PUSHINT8
    // 000D : STLOC0
    // 000E : LDLOC0
    // 000F : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertInCatch")]
    public abstract BigInteger? TestAssertInCatch();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : TRY
    // 0008 : PUSH1
    // 0009 : STLOC0
    // 000A : PUSHDATA1
    // 0015 : THROW
    // 0016 : STLOC1
    // 0017 : LDARG0
    // 0018 : CALL
    // 001A : STLOC0
    // 001B : ENDTRY
    // 001D : PUSH2
    // 001E : STLOC0
    // 001F : ENDFINALLY
    // 0020 : LDLOC0
    // 0021 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertInFinally")]
    public abstract BigInteger? TestAssertInFinally();
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
    // 0011 : LDARG0
    // 0012 : CALL_L

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertInFunction")]
    public abstract BigInteger? TestAssertInFunction();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : LDARG0
    // 0006 : CALL
    // 0008 : STLOC0
    // 0009 : PUSH1
    // 000A : STLOC0
    // 000B : LDLOC0
    // 000C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertInTry")]
    public abstract BigInteger? TestAssertInTry();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : TRY
    // 0008 : LDARG0
    // 0009 : CALL
    // 000B : STLOC0
    // 000C : ENDTRY
    // 000E : STLOC1
    // 000F : PUSH1
    // 0010 : STLOC0
    // 0011 : ENDTRY
    // 0013 : PUSH2
    // 0014 : STLOC0
    // 0015 : ENDFINALLY
    // 0016 : LDLOC0
    // 0017 : RET

    #endregion

}
