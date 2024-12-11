using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_PostfixUnary(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_PostfixUnary"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""isValid"",""parameters"":[{""name"":""person"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":168,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALtXAQAAUAA8AFATwBALE8AMBEpvaG5LNYcAAABwaDWLAAAAJnloShHOTpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfEVDQRWgSzhFLS85KVFOcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn9BFaBDOQAtAVwACeUp4EFHQRUBXAQF4cGjYJgQJQHgQznBo2KpAad6x8A=="));

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
    /// Script: VwEAAFAAPABQE8AQCxPADARKb2huSzWHAAAAcGg1iwAAACZ5aEoRzk6cSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnxFQ0EVoEs4RS0vOSlRTnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ/QRWgQzkALQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHINT8 50 [1 datoshi]
    /// 05 : PUSHINT8 3C [1 datoshi]
    /// 07 : PUSHINT8 50 [1 datoshi]
    /// 09 : PUSH3 [1 datoshi]
    /// 0A : PACK [2048 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : PUSHNULL [1 datoshi]
    /// 0D : PUSH3 [1 datoshi]
    /// 0E : PACK [2048 datoshi]
    /// 0F : PUSHDATA1 4A6F686E 'John' [8 datoshi]
    /// 15 : OVER [2 datoshi]
    /// 16 : CALL_L 87000000 [512 datoshi]
    /// 1B : STLOC0 [2 datoshi]
    /// 1C : LDLOC0 [2 datoshi]
    /// 1D : CALL_L 8B000000 [512 datoshi]
    /// 22 : JMPIFNOT 79 [2 datoshi]
    /// 24 : LDLOC0 [2 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : PUSH1 [1 datoshi]
    /// 27 : PICKITEM [64 datoshi]
    /// 28 : TUCK [2 datoshi]
    /// 29 : INC [4 datoshi]
    /// 2A : DUP [2 datoshi]
    /// 2B : PUSHINT32 00000080 [1 datoshi]
    /// 30 : JMPGE 04 [2 datoshi]
    /// 32 : JMP 0A [2 datoshi]
    /// 34 : DUP [2 datoshi]
    /// 35 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3A : JMPLE 1E [2 datoshi]
    /// 3C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 45 : AND [8 datoshi]
    /// 46 : DUP [2 datoshi]
    /// 47 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 4C : JMPLE 0C [2 datoshi]
    /// 4E : PUSHINT64 0000000001000000 [1 datoshi]
    /// 57 : SUB [8 datoshi]
    /// 58 : PUSH1 [1 datoshi]
    /// 59 : SWAP [2 datoshi]
    /// 5A : SETITEM [8192 datoshi]
    /// 5B : DROP [2 datoshi]
    /// 5C : LDLOC0 [2 datoshi]
    /// 5D : PUSH2 [1 datoshi]
    /// 5E : PICKITEM [64 datoshi]
    /// 5F : PUSH1 [1 datoshi]
    /// 60 : OVER [2 datoshi]
    /// 61 : OVER [2 datoshi]
    /// 62 : PICKITEM [64 datoshi]
    /// 63 : DUP [2 datoshi]
    /// 64 : REVERSE4 [2 datoshi]
    /// 65 : REVERSE3 [2 datoshi]
    /// 66 : INC [4 datoshi]
    /// 67 : DUP [2 datoshi]
    /// 68 : PUSHINT32 00000080 [1 datoshi]
    /// 6D : JMPGE 04 [2 datoshi]
    /// 6F : JMP 0A [2 datoshi]
    /// 71 : DUP [2 datoshi]
    /// 72 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 77 : JMPLE 1E [2 datoshi]
    /// 79 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 82 : AND [8 datoshi]
    /// 83 : DUP [2 datoshi]
    /// 84 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 89 : JMPLE 0C [2 datoshi]
    /// 8B : PUSHINT64 0000000001000000 [1 datoshi]
    /// 94 : SUB [8 datoshi]
    /// 95 : SETITEM [8192 datoshi]
    /// 96 : DROP [2 datoshi]
    /// 97 : LDLOC0 [2 datoshi]
    /// 98 : PUSH0 [1 datoshi]
    /// 99 : PICKITEM [64 datoshi]
    /// 9A : RET [0 datoshi]
    /// 9B : PUSHNULL [1 datoshi]
    /// 9C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract string? Test();

    #endregion
}
