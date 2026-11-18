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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_UserDefinedCompoundAssignment"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""structLocalTarget"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""structStaticFieldTarget"",""parameters"":[],""returntype"":""Integer"",""offset"":186,""safe"":false},{""name"":""structArrayElementTarget"",""parameters"":[],""returntype"":""Integer"",""offset"":214,""safe"":false},{""name"":""structMemberAccessTarget"",""parameters"":[],""returntype"":""Integer"",""offset"":337,""safe"":false},{""name"":""classLocalTarget"",""parameters"":[],""returntype"":""Integer"",""offset"":388,""safe"":false},{""name"":""classParameterTarget"",""parameters"":[],""returntype"":""Integer"",""offset"":530,""safe"":false},{""name"":""receiverEvaluatedBeforeArgument"",""parameters"":[],""returntype"":""Integer"",""offset"":567,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":611,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP10AlcBABARvxJLNBlwFWg0HRFoNEkSaDR1aBDOSsoUMgM6QFcAAnl4EFHQQFcAAnhKEM55nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn04QUNBFQFcAAnhKEM55n0rKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn04QUNBFQFcAAnhKEM55oErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn04QUNBFQBARvxNLNWL///9gFFg1Y////1gQzkrKFDIDOkBXAQAQEb8aSzVD////EBG/EUs1Of///1ASwHAVaBDONTX///9oEM4QzgBkoErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2gRzhDOnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0rKFDIDOkBXAQALEcAQEb8AFEs1xP7//0s0GHASaBDONcD+//9oEM4QzkrKFDIDOkBXAAJ5eBBR0EBXAQAQEcAAHks0FXAYaDQZE2g0RWg0ckrKFDIDOkBXAAJ5eBBR0EBXAAJ4ShDOeZ5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9OEFDQRUBXAAJ4ShDOeZ9KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9OEFDQRUBXAAF4EM5AVwEAEBHAADJLNIdwEmg0DGg06ErKFDIDOkBXAAJ5eDV6////QBARwAAoSzVl////YVk0ElA1ZP///1k0vkrKFDIDOkAQEcAQSzVH////YRVAVgILYBARwAAoSzU1////YUC3xZaI").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEBHAAB5LNBVwGGg0GRNoNEVoNHJKyhQyAzpA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHINT8 1E [1 datoshi]
    /// OVER [2 datoshi]
    /// CALL 15 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH8 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 19 [512 datoshi]
    /// PUSH3 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 45 [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 72 [512 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("classLocalTarget")]
    public abstract BigInteger? ClassLocalTarget();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEBHAADJLNIdwEmg0DGg06ErKFDIDOkA=
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
    /// CALL 0C [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL E8 [512 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("classParameterTarget")]
    public abstract BigInteger? ClassParameterTarget();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EBHAAChLNWX///9hWTQSUDVk////WTS+SsoUMgM6QA==
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHINT8 28 [1 datoshi]
    /// OVER [2 datoshi]
    /// CALL_L 65FFFFFF [512 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// CALL 12 [512 datoshi]
    /// SWAP [2 datoshi]
    /// CALL_L 64FFFFFF [512 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// CALL BE [512 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("receiverEvaluatedBeforeArgument")]
    public abstract BigInteger? ReceiverEvaluatedBeforeArgument();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEBG/Gks1Q////xARvxFLNTn///9QEsBwFWgQzjU1////aBDOEM4AZKBKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9oEc4Qzp5KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KyhQyAzpA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// PUSH10 [1 datoshi]
    /// OVER [2 datoshi]
    /// CALL_L 43FFFFFF [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// PUSH1 [1 datoshi]
    /// OVER [2 datoshi]
    /// CALL_L 39FFFFFF [512 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH5 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// CALL_L 35FFFFFF [512 datoshi]
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
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("structArrayElementTarget")]
    public abstract BigInteger? StructArrayElementTarget();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEBG/Eks0GXAVaDQdEWg0SRJoNHVoEM5KyhQyAzpA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// PUSH2 [1 datoshi]
    /// OVER [2 datoshi]
    /// CALL 19 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH5 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 1D [512 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 49 [512 datoshi]
    /// PUSH2 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 75 [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("structLocalTarget")]
    public abstract BigInteger? StructLocalTarget();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxHAEBG/ABRLNcT+//9LNBhwEmgQzjXA/v//aBDOEM5KyhQyAzpA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// OVER [2 datoshi]
    /// CALL_L C4FEFFFF [512 datoshi]
    /// OVER [2 datoshi]
    /// CALL 18 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// CALL_L C0FEFFFF [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("structMemberAccessTarget")]
    public abstract BigInteger? StructMemberAccessTarget();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EBG/E0s1Yv///2AUWDVj////WBDOSsoUMgM6QA==
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
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("structStaticFieldTarget")]
    public abstract BigInteger? StructStaticFieldTarget();

    #endregion
}
