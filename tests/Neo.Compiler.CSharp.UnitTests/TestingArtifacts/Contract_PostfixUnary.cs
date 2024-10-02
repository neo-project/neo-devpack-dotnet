using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_PostfixUnary(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_PostfixUnary"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""isValid"",""parameters"":[{""name"":""person"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":172,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMJXAQALEAsTwAwESm9obks1hwAAAHBoNZYAAAAmeWhKEc5OnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8RUNBFaBLOEUtLzkpUU5xKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACf0EVoEM5AC0BXAAJ4EgBQADwAUBPA0HlKeBBR0EVAVwEBeHBoC5eqJAQJQHgQznBoC5eqQFsFvBY="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isValid")]
    public abstract bool? IsValid(object? person = null);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSHNULL
    // 0007 : EQUAL
    // 0008 : NOT
    // 0009 : JMPIF
    // 000B : PUSHF
    // 000C : RET
    // 000D : LDARG0
    // 000E : PUSH0
    // 000F : PICKITEM
    // 0010 : STLOC0
    // 0011 : LDLOC0
    // 0012 : PUSHNULL
    // 0013 : EQUAL
    // 0014 : NOT
    // 0015 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test")]
    public abstract string? Test();
    // 0000 : INITSLOT
    // 0003 : PUSHNULL
    // 0004 : PUSH0
    // 0005 : PUSHNULL
    // 0006 : PUSH3
    // 0007 : PACK
    // 0008 : PUSHDATA1
    // 000E : OVER
    // 000F : CALL_L
    // 0014 : STLOC0
    // 0015 : LDLOC0
    // 0016 : CALL_L
    // 001B : JMPIFNOT
    // 001D : LDLOC0
    // 001E : DUP
    // 001F : PUSH1
    // 0020 : PICKITEM
    // 0021 : TUCK
    // 0022 : INC
    // 0023 : DUP
    // 0024 : PUSHINT32
    // 0029 : JMPGE
    // 002B : JMP
    // 002D : DUP
    // 002E : PUSHINT32
    // 0033 : JMPLE
    // 0035 : PUSHINT64
    // 003E : AND
    // 003F : DUP
    // 0040 : PUSHINT32
    // 0045 : JMPLE
    // 0047 : PUSHINT64
    // 0050 : SUB
    // 0051 : PUSH1
    // 0052 : SWAP
    // 0053 : SETITEM
    // 0054 : DROP
    // 0055 : LDLOC0
    // 0056 : PUSH2
    // 0057 : PICKITEM
    // 0058 : PUSH1
    // 0059 : OVER
    // 005A : OVER
    // 005B : PICKITEM
    // 005C : DUP
    // 005D : REVERSE4
    // 005E : REVERSE3
    // 005F : INC
    // 0060 : DUP
    // 0061 : PUSHINT32
    // 0066 : JMPGE
    // 0068 : JMP
    // 006A : DUP
    // 006B : PUSHINT32
    // 0070 : JMPLE
    // 0072 : PUSHINT64
    // 007B : AND
    // 007C : DUP
    // 007D : PUSHINT32
    // 0082 : JMPLE
    // 0084 : PUSHINT64
    // 008D : SUB
    // 008E : SETITEM
    // 008F : DROP
    // 0090 : LDLOC0
    // 0091 : PUSH0
    // 0092 : PICKITEM
    // 0093 : RET
    // 0094 : PUSHNULL
    // 0095 : RET

    #endregion

}
