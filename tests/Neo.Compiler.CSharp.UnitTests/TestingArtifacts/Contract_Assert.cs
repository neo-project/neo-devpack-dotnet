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
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.PUSHT
    /// 0006 : OpCode.ASSERT
    /// 0007 : OpCode.PUSH1
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.PUSHF
    /// 000A : OpCode.ASSERT
    /// 000B : OpCode.PUSHINT8 64
    /// 000D : OpCode.STLOC0
    /// 000E : OpCode.LDLOC0
    /// 000F : OpCode.RET
    /// </remarks>
    [DisplayName("testAssertFalse")]
    public abstract BigInteger? TestAssertFalse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.TRY 1118
    /// 0008 : OpCode.PUSH1
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.PUSHDATA1 657863657074696F6E
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.STLOC1
    /// 0017 : OpCode.LDARG0
    /// 0018 : OpCode.CALL A8
    /// 001A : OpCode.STLOC0
    /// 001B : OpCode.ENDTRY 05
    /// 001D : OpCode.PUSH2
    /// 001E : OpCode.STLOC0
    /// 001F : OpCode.ENDFINALLY
    /// 0020 : OpCode.LDLOC0
    /// 0021 : OpCode.RET
    /// </remarks>
    [DisplayName("testAssertInCatch")]
    public abstract BigInteger? TestAssertInCatch();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
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
    /// 0012 : OpCode.CALL_L 8CFFFFFF
    /// </remarks>
    [DisplayName("testAssertInFinally")]
    public abstract BigInteger? TestAssertInFinally();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.CALL DF
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.PUSH1
    /// 000A : OpCode.STLOC0
    /// 000B : OpCode.LDLOC0
    /// 000C : OpCode.RET
    /// </remarks>
    [DisplayName("testAssertInFunction")]
    public abstract BigInteger? TestAssertInFunction();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.TRY 090E
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.CALL CF
    /// 000B : OpCode.STLOC0
    /// 000C : OpCode.ENDTRY 0A
    /// 000E : OpCode.STLOC1
    /// 000F : OpCode.PUSH1
    /// 0010 : OpCode.STLOC0
    /// 0011 : OpCode.ENDTRY 05
    /// 0013 : OpCode.PUSH2
    /// 0014 : OpCode.STLOC0
    /// 0015 : OpCode.ENDFINALLY
    /// 0016 : OpCode.LDLOC0
    /// 0017 : OpCode.RET
    /// </remarks>
    [DisplayName("testAssertInTry")]
    public abstract BigInteger? TestAssertInTry();

    #endregion

}
