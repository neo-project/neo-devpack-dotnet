using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_PostfixUnary(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_PostfixUnary"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""isValid"",""parameters"":[{""name"":""person"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":176,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMNXAQALEAsTwAwESm9obks1hwAAAHBoNZoAAAAmeWhKEc5OnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8RUNBFaBLOEUtLzkpUU5xKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACf0EVoEM5AC0BXAAJ4ERDQeBIAUAA8AFATwNB5SngQUdBFQFcBAXhwaNgmBAlAeBDOcGjYqkDxZMoR").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2CYECUB4EM5waNiqQA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : ISNULL [2 datoshi]
    /// 07 : JMPIFNOT 04 [2 datoshi]
    /// 09 : PUSHF [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : PUSH0 [1 datoshi]
    /// 0D : PICKITEM [64 datoshi]
    /// 0E : STLOC0 [2 datoshi]
    /// 0F : LDLOC0 [2 datoshi]
    /// 10 : ISNULL [2 datoshi]
    /// 11 : NOT [4 datoshi]
    /// 12 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isValid")]
    public abstract bool? IsValid(object? person = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxALE8AMBEpvaG5LNYcAAABwaDWaAAAAJnloShHOTpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfEVDQRWgSzhFLS85KVFOcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn9BFaBDOQAtA
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : PUSHNULL [1 datoshi]
    /// 06 : PUSH3 [1 datoshi]
    /// 07 : PACK [2048 datoshi]
    /// 08 : PUSHDATA1 4A6F686E 'John' [8 datoshi]
    /// 0E : OVER [2 datoshi]
    /// 0F : CALL_L 87000000 [512 datoshi]
    /// 14 : STLOC0 [2 datoshi]
    /// 15 : LDLOC0 [2 datoshi]
    /// 16 : CALL_L 9A000000 [512 datoshi]
    /// 1B : JMPIFNOT 79 [2 datoshi]
    /// 1D : LDLOC0 [2 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : PUSH1 [1 datoshi]
    /// 20 : PICKITEM [64 datoshi]
    /// 21 : TUCK [2 datoshi]
    /// 22 : INC [4 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : PUSHINT32 00000080 [1 datoshi]
    /// 29 : JMPGE 04 [2 datoshi]
    /// 2B : JMP 0A [2 datoshi]
    /// 2D : DUP [2 datoshi]
    /// 2E : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 33 : JMPLE 1E [2 datoshi]
    /// 35 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 3E : AND [8 datoshi]
    /// 3F : DUP [2 datoshi]
    /// 40 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 45 : JMPLE 0C [2 datoshi]
    /// 47 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 50 : SUB [8 datoshi]
    /// 51 : PUSH1 [1 datoshi]
    /// 52 : SWAP [2 datoshi]
    /// 53 : SETITEM [8192 datoshi]
    /// 54 : DROP [2 datoshi]
    /// 55 : LDLOC0 [2 datoshi]
    /// 56 : PUSH2 [1 datoshi]
    /// 57 : PICKITEM [64 datoshi]
    /// 58 : PUSH1 [1 datoshi]
    /// 59 : OVER [2 datoshi]
    /// 5A : OVER [2 datoshi]
    /// 5B : PICKITEM [64 datoshi]
    /// 5C : DUP [2 datoshi]
    /// 5D : REVERSE4 [2 datoshi]
    /// 5E : REVERSE3 [2 datoshi]
    /// 5F : INC [4 datoshi]
    /// 60 : DUP [2 datoshi]
    /// 61 : PUSHINT32 00000080 [1 datoshi]
    /// 66 : JMPGE 04 [2 datoshi]
    /// 68 : JMP 0A [2 datoshi]
    /// 6A : DUP [2 datoshi]
    /// 6B : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 70 : JMPLE 1E [2 datoshi]
    /// 72 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 7B : AND [8 datoshi]
    /// 7C : DUP [2 datoshi]
    /// 7D : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 82 : JMPLE 0C [2 datoshi]
    /// 84 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 8D : SUB [8 datoshi]
    /// 8E : SETITEM [8192 datoshi]
    /// 8F : DROP [2 datoshi]
    /// 90 : LDLOC0 [2 datoshi]
    /// 91 : PUSH0 [1 datoshi]
    /// 92 : PICKITEM [64 datoshi]
    /// 93 : RET [0 datoshi]
    /// 94 : PUSHNULL [1 datoshi]
    /// 95 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract string? Test();

    #endregion
}
