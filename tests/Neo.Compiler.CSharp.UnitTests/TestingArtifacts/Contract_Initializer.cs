using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Initializer(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Initializer"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""sum1"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":77,""safe"":false},{""name"":""sum2"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":150,""safe"":false},{""name"":""anonymousObjectCreation"",""parameters"":[],""returntype"":""Void"",""offset"":229,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0eAVcBABAQEsBKNDlwaBDOaBHOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwABeBAR0HgREtBAVwECEBASwEo07EoQeNBKEXnQcGgQzmgRzp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcBAhAQEsBKNKNweEpoEFHQRXlKaBFR0EVoEM5oEc6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAgAMBUhlbGxvAGwSwHBoNChBz+dHlhEMBWdyYXBlEsAUDAVhcHBsZRLAEsBxaRDONAdBz+dHlkDyixZl").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADAVIZWxsbwBsEsBwaDQoQc/nR5YRDAVncmFwZRLAFAwFYXBwbGUSwBLAcWkQzjQHQc/nR5ZA
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHDATA1 48656C6C6F 'Hello' [8 datoshi]
    /// 0A : PUSHINT8 6C [1 datoshi]
    /// 0C : PUSH2 [1 datoshi]
    /// 0D : PACK [2048 datoshi]
    /// 0E : STLOC0 [2 datoshi]
    /// 0F : LDLOC0 [2 datoshi]
    /// 10 : CALL 28 [512 datoshi]
    /// 12 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 17 : PUSH1 [1 datoshi]
    /// 18 : PUSHDATA1 6772617065 'grape' [8 datoshi]
    /// 1F : PUSH2 [1 datoshi]
    /// 20 : PACK [2048 datoshi]
    /// 21 : PUSH4 [1 datoshi]
    /// 22 : PUSHDATA1 6170706C65 'apple' [8 datoshi]
    /// 29 : PUSH2 [1 datoshi]
    /// 2A : PACK [2048 datoshi]
    /// 2B : PUSH2 [1 datoshi]
    /// 2C : PACK [2048 datoshi]
    /// 2D : STLOC1 [2 datoshi]
    /// 2E : LDLOC1 [2 datoshi]
    /// 2F : PUSH0 [1 datoshi]
    /// 30 : PICKITEM [64 datoshi]
    /// 31 : CALL 07 [512 datoshi]
    /// 33 : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 38 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("anonymousObjectCreation")]
    public abstract void AnonymousObjectCreation();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEBASwEo0OXBoEM5oEc6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : PACK [2048 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : CALL 39 [512 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : LDLOC0 [2 datoshi]
    /// 0C : PUSH0 [1 datoshi]
    /// 0D : PICKITEM [64 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : PUSH1 [1 datoshi]
    /// 10 : PICKITEM [64 datoshi]
    /// 11 : ADD [8 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : PUSHINT32 00000080 [1 datoshi]
    /// 18 : JMPGE 04 [2 datoshi]
    /// 1A : JMP 0A [2 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 22 : JMPLE 1E [2 datoshi]
    /// 24 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2D : AND [8 datoshi]
    /// 2E : DUP [2 datoshi]
    /// 2F : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 34 : JMPLE 0C [2 datoshi]
    /// 36 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 3F : SUB [8 datoshi]
    /// 40 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEBASwEo07EoQeNBKEXnQcGgQzmgRzp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// 00 : INITSLOT 0102 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : PACK [2048 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : CALL EC [512 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : LDARG0 [2 datoshi]
    /// 0D : SETITEM [8192 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSH1 [1 datoshi]
    /// 10 : LDARG1 [2 datoshi]
    /// 11 : SETITEM [8192 datoshi]
    /// 12 : STLOC0 [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : PUSH0 [1 datoshi]
    /// 15 : PICKITEM [64 datoshi]
    /// 16 : LDLOC0 [2 datoshi]
    /// 17 : PUSH1 [1 datoshi]
    /// 18 : PICKITEM [64 datoshi]
    /// 19 : ADD [8 datoshi]
    /// 1A : DUP [2 datoshi]
    /// 1B : PUSHINT32 00000080 [1 datoshi]
    /// 20 : JMPGE 04 [2 datoshi]
    /// 22 : JMP 0A [2 datoshi]
    /// 24 : DUP [2 datoshi]
    /// 25 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2A : JMPLE 1E [2 datoshi]
    /// 2C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 35 : AND [8 datoshi]
    /// 36 : DUP [2 datoshi]
    /// 37 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3C : JMPLE 0C [2 datoshi]
    /// 3E : PUSHINT64 0000000001000000 [1 datoshi]
    /// 47 : SUB [8 datoshi]
    /// 48 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum1")]
    public abstract BigInteger? Sum1(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEBASwEo0o3B4SmgQUdBFeUpoEVHQRWgQzmgRzp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// 00 : INITSLOT 0102 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : PACK [2048 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : CALL A3 [512 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : LDLOC0 [2 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : ROT [2 datoshi]
    /// 10 : SETITEM [8192 datoshi]
    /// 11 : DROP [2 datoshi]
    /// 12 : LDARG1 [2 datoshi]
    /// 13 : DUP [2 datoshi]
    /// 14 : LDLOC0 [2 datoshi]
    /// 15 : PUSH1 [1 datoshi]
    /// 16 : ROT [2 datoshi]
    /// 17 : SETITEM [8192 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : LDLOC0 [2 datoshi]
    /// 1A : PUSH0 [1 datoshi]
    /// 1B : PICKITEM [64 datoshi]
    /// 1C : LDLOC0 [2 datoshi]
    /// 1D : PUSH1 [1 datoshi]
    /// 1E : PICKITEM [64 datoshi]
    /// 1F : ADD [8 datoshi]
    /// 20 : DUP [2 datoshi]
    /// 21 : PUSHINT32 00000080 [1 datoshi]
    /// 26 : JMPGE 04 [2 datoshi]
    /// 28 : JMP 0A [2 datoshi]
    /// 2A : DUP [2 datoshi]
    /// 2B : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 30 : JMPLE 1E [2 datoshi]
    /// 32 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 3B : AND [8 datoshi]
    /// 3C : DUP [2 datoshi]
    /// 3D : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 42 : JMPLE 0C [2 datoshi]
    /// 44 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 4D : SUB [8 datoshi]
    /// 4E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum2")]
    public abstract BigInteger? Sum2(BigInteger? a, BigInteger? b);

    #endregion
}
