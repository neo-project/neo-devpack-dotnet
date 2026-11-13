using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_UserDefinedCompoundAssignment(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_UserDefinedCompoundAssignment"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""structLocalTarget"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""structStaticFieldTarget"",""parameters"":[],""returntype"":""Integer"",""offset"":180,""safe"":false},{""name"":""structArrayElementTarget"",""parameters"":[],""returntype"":""Integer"",""offset"":202,""safe"":false},{""name"":""structMemberAccessTarget"",""parameters"":[],""returntype"":""Integer"",""offset"":319,""safe"":false},{""name"":""classLocalTarget"",""parameters"":[],""returntype"":""Integer"",""offset"":373,""safe"":false},{""name"":""classParameterTarget"",""parameters"":[],""returntype"":""Integer"",""offset"":509,""safe"":false},{""name"":""receiverEvaluatedBeforeArgument"",""parameters"":[],""returntype"":""Integer"",""offset"":537,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":575,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1SAlcBABARvxJLNBNwFWg0FxFoNEMSaDRvaBDOQFcAAnl4EFHQQFcAAnhKEM55nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn04QUNBFQFcAAnhKEM55n0rKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn04QUNBFQFcAAnhKEM55oErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn04QUNBFQBARvxNLNWL///9gFFg1Y////1gQzkBXAQAQEb8aSzVJ////EBG/EUs1P////1ASwHAVaBDONTv///9oEM4QzgBkoErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2gRzhDOnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAQAQEb8RwBARvwAUSzXO/v//SzQScBJoEM41yv7//2gQzhDOQFcAAnkQTRDOEb9GeBBR0EBXAQAQEcAAHks0D3AYaDQTE2g0P2g0bEBXAAJ5eBBR0EBXAAJ4ShDOeZ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9OEFDQRUBXAAJ4ShDOeZ9KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9OEFDQRUBXAAF4EM5AVwEAEBHAADJLNIdwEmg0Bmg06EBXAAJ5eDSAQBARwAAoSzVu////YVk0DFA1bf///1k0x0AQEcAQSzVW////YRVAVgIQEb9gEBHAAChLNUL///9hQK2aSpU=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEBHAAB5LNA9wGGg0ExNoND9oNGxA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHINT8 1E [1 datoshi]
    /// OVER [2 datoshi]
    /// CALL 0F [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH8 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 13 [512 datoshi]
    /// PUSH3 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 3F [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 6C [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("classLocalTarget")]
    public abstract BigInteger? ClassLocalTarget();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEBHAADJLNIdwEmg0Bmg06EA=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHINT8 32 [1 datoshi]
    /// OVER [2 datoshi]
    /// CALL 87 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 06 [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL E8 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("classParameterTarget")]
    public abstract BigInteger? ClassParameterTarget();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EBHAAChLNW7///9hWTQMUDVt////WTTHQA==
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHINT8 28 [1 datoshi]
    /// OVER [2 datoshi]
    /// CALL_L 6EFFFFFF [512 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// CALL 0C [512 datoshi]
    /// SWAP [2 datoshi]
    /// CALL_L 6DFFFFFF [512 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// CALL C7 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("receiverEvaluatedBeforeArgument")]
    public abstract BigInteger? ReceiverEvaluatedBeforeArgument();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEBG/Gks1Sf///xARvxFLNT////9QEsBwFWgQzjU7////aBDOEM4AZKBKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9oEc4Qzp5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9A
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// PUSH10 [1 datoshi]
    /// OVER [2 datoshi]
    /// CALL_L 49FFFFFF [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// PUSH1 [1 datoshi]
    /// OVER [2 datoshi]
    /// CALL_L 3FFFFFFF [512 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH5 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// CALL_L 3BFFFFFF [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("structArrayElementTarget")]
    public abstract BigInteger? StructArrayElementTarget();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEBG/Eks0E3AVaDQXEWg0QxJoNG9oEM5A
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// PUSH2 [1 datoshi]
    /// OVER [2 datoshi]
    /// CALL 13 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH5 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 17 [512 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 43 [512 datoshi]
    /// PUSH2 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 6F [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("structLocalTarget")]
    public abstract BigInteger? StructLocalTarget();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEBG/EcAQEb8AFEs1zv7//0s0EnASaBDONcr+//9oEM4QzkA=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// OVER [2 datoshi]
    /// CALL_L CEFEFFFF [512 datoshi]
    /// OVER [2 datoshi]
    /// CALL 12 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// CALL_L CAFEFFFF [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("structMemberAccessTarget")]
    public abstract BigInteger? StructMemberAccessTarget();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EBG/E0s1Yv///2AUWDVj////WBDOQA==
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// PUSH3 [1 datoshi]
    /// OVER [2 datoshi]
    /// CALL_L 62FFFFFF [512 datoshi]
    /// STSFLD0 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// CALL_L 63FFFFFF [512 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("structStaticFieldTarget")]
    public abstract BigInteger? StructStaticFieldTarget();

    #endregion
}
