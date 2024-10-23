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
    /// Script: VwEBeHBoC5eqJAQJQHgQznBoC5eqQA==
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.STLOC0	[2 datoshi]
    /// 05 : OpCode.LDLOC0	[2 datoshi]
    /// 06 : OpCode.PUSHNULL	[1 datoshi]
    /// 07 : OpCode.EQUAL	[32 datoshi]
    /// 08 : OpCode.NOT	[4 datoshi]
    /// 09 : OpCode.JMPIF 04	[2 datoshi]
    /// 0B : OpCode.PUSHF	[1 datoshi]
    /// 0C : OpCode.RET	[0 datoshi]
    /// 0D : OpCode.LDARG0	[2 datoshi]
    /// 0E : OpCode.PUSH0	[1 datoshi]
    /// 0F : OpCode.PICKITEM	[64 datoshi]
    /// 10 : OpCode.STLOC0	[2 datoshi]
    /// 11 : OpCode.LDLOC0	[2 datoshi]
    /// 12 : OpCode.PUSHNULL	[1 datoshi]
    /// 13 : OpCode.EQUAL	[32 datoshi]
    /// 14 : OpCode.NOT	[4 datoshi]
    /// 15 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("isValid")]
    public abstract bool? IsValid(object? person = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxALE8AMSm9obks1hwAAAHBoNZYAAAAmeWhKEc5OnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8RUNBFaBLOEUtLzkpUU5xKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACf0EVoEM5AC0A=
    /// 00 : OpCode.INITSLOT 0100	[64 datoshi]
    /// 03 : OpCode.PUSHNULL	[1 datoshi]
    /// 04 : OpCode.PUSH0	[1 datoshi]
    /// 05 : OpCode.PUSHNULL	[1 datoshi]
    /// 06 : OpCode.PUSH3	[1 datoshi]
    /// 07 : OpCode.PACK	[2048 datoshi]
    /// 08 : OpCode.PUSHDATA1 4A6F686E	[8 datoshi]
    /// 0E : OpCode.OVER	[2 datoshi]
    /// 0F : OpCode.CALL_L 87000000	[512 datoshi]
    /// 14 : OpCode.STLOC0	[2 datoshi]
    /// 15 : OpCode.LDLOC0	[2 datoshi]
    /// 16 : OpCode.CALL_L 96000000	[512 datoshi]
    /// 1B : OpCode.JMPIFNOT 79	[2 datoshi]
    /// 1D : OpCode.LDLOC0	[2 datoshi]
    /// 1E : OpCode.DUP	[2 datoshi]
    /// 1F : OpCode.PUSH1	[1 datoshi]
    /// 20 : OpCode.PICKITEM	[64 datoshi]
    /// 21 : OpCode.TUCK	[2 datoshi]
    /// 22 : OpCode.INC	[4 datoshi]
    /// 23 : OpCode.DUP	[2 datoshi]
    /// 24 : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 29 : OpCode.JMPGE 04	[2 datoshi]
    /// 2B : OpCode.JMP 0A	[2 datoshi]
    /// 2D : OpCode.DUP	[2 datoshi]
    /// 2E : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 33 : OpCode.JMPLE 1E	[2 datoshi]
    /// 35 : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 3E : OpCode.AND	[8 datoshi]
    /// 3F : OpCode.DUP	[2 datoshi]
    /// 40 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 45 : OpCode.JMPLE 0C	[2 datoshi]
    /// 47 : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 50 : OpCode.SUB	[8 datoshi]
    /// 51 : OpCode.PUSH1	[1 datoshi]
    /// 52 : OpCode.SWAP	[2 datoshi]
    /// 53 : OpCode.SETITEM	[8192 datoshi]
    /// 54 : OpCode.DROP	[2 datoshi]
    /// 55 : OpCode.LDLOC0	[2 datoshi]
    /// 56 : OpCode.PUSH2	[1 datoshi]
    /// 57 : OpCode.PICKITEM	[64 datoshi]
    /// 58 : OpCode.PUSH1	[1 datoshi]
    /// 59 : OpCode.OVER	[2 datoshi]
    /// 5A : OpCode.OVER	[2 datoshi]
    /// 5B : OpCode.PICKITEM	[64 datoshi]
    /// 5C : OpCode.DUP	[2 datoshi]
    /// 5D : OpCode.REVERSE4	[2 datoshi]
    /// 5E : OpCode.REVERSE3	[2 datoshi]
    /// 5F : OpCode.INC	[4 datoshi]
    /// 60 : OpCode.DUP	[2 datoshi]
    /// 61 : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 66 : OpCode.JMPGE 04	[2 datoshi]
    /// 68 : OpCode.JMP 0A	[2 datoshi]
    /// 6A : OpCode.DUP	[2 datoshi]
    /// 6B : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 70 : OpCode.JMPLE 1E	[2 datoshi]
    /// 72 : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 7B : OpCode.AND	[8 datoshi]
    /// 7C : OpCode.DUP	[2 datoshi]
    /// 7D : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 82 : OpCode.JMPLE 0C	[2 datoshi]
    /// 84 : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 8D : OpCode.SUB	[8 datoshi]
    /// 8E : OpCode.SETITEM	[8192 datoshi]
    /// 8F : OpCode.DROP	[2 datoshi]
    /// 90 : OpCode.LDLOC0	[2 datoshi]
    /// 91 : OpCode.PUSH0	[1 datoshi]
    /// 92 : OpCode.PICKITEM	[64 datoshi]
    /// 93 : OpCode.RET	[0 datoshi]
    /// 94 : OpCode.PUSHNULL	[1 datoshi]
    /// 95 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract string? Test();

    #endregion
}
