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
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSHNULL
    /// 0007 : OpCode.EQUAL
    /// 0008 : OpCode.NOT
    /// 0009 : OpCode.JMPIF 04
    /// 000B : OpCode.PUSHF
    /// 000C : OpCode.RET
    /// 000D : OpCode.LDARG0
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.PICKITEM
    /// 0010 : OpCode.STLOC0
    /// 0011 : OpCode.LDLOC0
    /// 0012 : OpCode.PUSHNULL
    /// 0013 : OpCode.EQUAL
    /// 0014 : OpCode.NOT
    /// 0015 : OpCode.RET
    /// </remarks>
    [DisplayName("isValid")]
    public abstract bool? IsValid(object? person = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSHNULL
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.PUSHNULL
    /// 0006 : OpCode.PUSH3
    /// 0007 : OpCode.PACK
    /// 0008 : OpCode.PUSHDATA1 4A6F686E
    /// 000E : OpCode.OVER
    /// 000F : OpCode.CALL_L 87000000
    /// 0014 : OpCode.STLOC0
    /// 0015 : OpCode.LDLOC0
    /// 0016 : OpCode.CALL_L 96000000
    /// 001B : OpCode.JMPIFNOT 79
    /// 001D : OpCode.LDLOC0
    /// 001E : OpCode.DUP
    /// 001F : OpCode.PUSH1
    /// 0020 : OpCode.PICKITEM
    /// 0021 : OpCode.TUCK
    /// 0022 : OpCode.INC
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.PUSHINT32 00000080
    /// 0029 : OpCode.JMPGE 04
    /// 002B : OpCode.JMP 0A
    /// 002D : OpCode.DUP
    /// 002E : OpCode.PUSHINT32 FFFFFF7F
    /// 0033 : OpCode.JMPLE 1E
    /// 0035 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 003E : OpCode.AND
    /// 003F : OpCode.DUP
    /// 0040 : OpCode.PUSHINT32 FFFFFF7F
    /// 0045 : OpCode.JMPLE 0C
    /// 0047 : OpCode.PUSHINT64 0000000001000000
    /// 0050 : OpCode.SUB
    /// 0051 : OpCode.PUSH1
    /// 0052 : OpCode.SWAP
    /// 0053 : OpCode.SETITEM
    /// 0054 : OpCode.DROP
    /// 0055 : OpCode.LDLOC0
    /// 0056 : OpCode.PUSH2
    /// 0057 : OpCode.PICKITEM
    /// 0058 : OpCode.PUSH1
    /// 0059 : OpCode.OVER
    /// 005A : OpCode.OVER
    /// 005B : OpCode.PICKITEM
    /// 005C : OpCode.DUP
    /// 005D : OpCode.REVERSE4
    /// 005E : OpCode.REVERSE3
    /// 005F : OpCode.INC
    /// 0060 : OpCode.DUP
    /// 0061 : OpCode.PUSHINT32 00000080
    /// 0066 : OpCode.JMPGE 04
    /// 0068 : OpCode.JMP 0A
    /// 006A : OpCode.DUP
    /// 006B : OpCode.PUSHINT32 FFFFFF7F
    /// 0070 : OpCode.JMPLE 1E
    /// 0072 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 007B : OpCode.AND
    /// 007C : OpCode.DUP
    /// 007D : OpCode.PUSHINT32 FFFFFF7F
    /// 0082 : OpCode.JMPLE 0C
    /// 0084 : OpCode.PUSHINT64 0000000001000000
    /// 008D : OpCode.SUB
    /// 008E : OpCode.SETITEM
    /// 008F : OpCode.DROP
    /// 0090 : OpCode.LDLOC0
    /// 0091 : OpCode.PUSH0
    /// 0092 : OpCode.PICKITEM
    /// 0093 : OpCode.RET
    /// 0094 : OpCode.PUSHNULL
    /// 0095 : OpCode.RET
    /// </remarks>
    [DisplayName("test")]
    public abstract string? Test();

    #endregion

}
