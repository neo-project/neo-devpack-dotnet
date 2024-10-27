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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9XAQALEAsTwAwESm9obks1hwAAAHBoNZYAAAAmeWhKEc5OnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8RUNBFaBLOEUtLzkpUU5xKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACf0EVoEM5AC0BXAAJ4EgBQADwAUBPA0HlKeBBR0EVAVwEBeHBo2CYECUB4EM5waNiqQAQGYzE="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2CYECUB4EM5waNiqQA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.ISNULL
    /// 07 : OpCode.JMPIFNOT 04
    /// 09 : OpCode.PUSHF
    /// 0A : OpCode.RET
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.PUSH0
    /// 0D : OpCode.PICKITEM
    /// 0E : OpCode.STLOC0
    /// 0F : OpCode.LDLOC0
    /// 10 : OpCode.ISNULL
    /// 11 : OpCode.NOT
    /// 12 : OpCode.RET
    /// </remarks>
    [DisplayName("isValid")]
    public abstract bool? IsValid(object? person = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxALE8AMSm9obks1hwAAAHBoNZYAAAAmeWhKEc5OnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8RUNBFaBLOEUtLzkpUU5xKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACf0EVoEM5AC0A=
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSHNULL
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.PUSHNULL
    /// 06 : OpCode.PUSH3
    /// 07 : OpCode.PACK
    /// 08 : OpCode.PUSHDATA1 4A6F686E
    /// 0E : OpCode.OVER
    /// 0F : OpCode.CALL_L 87000000
    /// 14 : OpCode.STLOC0
    /// 15 : OpCode.LDLOC0
    /// 16 : OpCode.CALL_L 96000000
    /// 1B : OpCode.JMPIFNOT 79
    /// 1D : OpCode.LDLOC0
    /// 1E : OpCode.DUP
    /// 1F : OpCode.PUSH1
    /// 20 : OpCode.PICKITEM
    /// 21 : OpCode.TUCK
    /// 22 : OpCode.INC
    /// 23 : OpCode.DUP
    /// 24 : OpCode.PUSHINT32 00000080
    /// 29 : OpCode.JMPGE 04
    /// 2B : OpCode.JMP 0A
    /// 2D : OpCode.DUP
    /// 2E : OpCode.PUSHINT32 FFFFFF7F
    /// 33 : OpCode.JMPLE 1E
    /// 35 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 3E : OpCode.AND
    /// 3F : OpCode.DUP
    /// 40 : OpCode.PUSHINT32 FFFFFF7F
    /// 45 : OpCode.JMPLE 0C
    /// 47 : OpCode.PUSHINT64 0000000001000000
    /// 50 : OpCode.SUB
    /// 51 : OpCode.PUSH1
    /// 52 : OpCode.SWAP
    /// 53 : OpCode.SETITEM
    /// 54 : OpCode.DROP
    /// 55 : OpCode.LDLOC0
    /// 56 : OpCode.PUSH2
    /// 57 : OpCode.PICKITEM
    /// 58 : OpCode.PUSH1
    /// 59 : OpCode.OVER
    /// 5A : OpCode.OVER
    /// 5B : OpCode.PICKITEM
    /// 5C : OpCode.DUP
    /// 5D : OpCode.REVERSE4
    /// 5E : OpCode.REVERSE3
    /// 5F : OpCode.INC
    /// 60 : OpCode.DUP
    /// 61 : OpCode.PUSHINT32 00000080
    /// 66 : OpCode.JMPGE 04
    /// 68 : OpCode.JMP 0A
    /// 6A : OpCode.DUP
    /// 6B : OpCode.PUSHINT32 FFFFFF7F
    /// 70 : OpCode.JMPLE 1E
    /// 72 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 7B : OpCode.AND
    /// 7C : OpCode.DUP
    /// 7D : OpCode.PUSHINT32 FFFFFF7F
    /// 82 : OpCode.JMPLE 0C
    /// 84 : OpCode.PUSHINT64 0000000001000000
    /// 8D : OpCode.SUB
    /// 8E : OpCode.SETITEM
    /// 8F : OpCode.DROP
    /// 90 : OpCode.LDLOC0
    /// 91 : OpCode.PUSH0
    /// 92 : OpCode.PICKITEM
    /// 93 : OpCode.RET
    /// 94 : OpCode.PUSHNULL
    /// 95 : OpCode.RET
    /// </remarks>
    [DisplayName("test")]
    public abstract string? Test();

    #endregion
}
