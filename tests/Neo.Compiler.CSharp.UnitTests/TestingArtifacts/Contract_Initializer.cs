using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Initializer(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Initializer"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""sum1"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":62,""safe"":false},{""name"":""sum2"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":132,""safe"":false},{""name"":""anonymousObjectCreation"",""parameters"":[],""returntype"":""Void"",""offset"":208,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0BAVcBABIREsBwaBDOaBHOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwECEhESwEoQeNBKEXnQcGgQzmgRzp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcBAhIREsBweEpoEFHQRXlKaBFR0EVoEM5oEc6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAgAMBUhlbGxvAGwSwHBBz+dHlhEMBWdyYXBlEsAUDAVhcHBsZRLAEsBxQc/nR5ZA9Z8EmQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADAVIZWxsbwBsEsBwQc/nR5YRDAVncmFwZRLAFAwFYXBwbGUSwBLAcUHP50eWQA==
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHDATA1 48656C6C6F 'Hello' [8 datoshi]
    /// 0A : PUSHINT8 6C [1 datoshi]
    /// 0C : PUSH2 [1 datoshi]
    /// 0D : PACK [2048 datoshi]
    /// 0E : STLOC0 [2 datoshi]
    /// 0F : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 14 : PUSH1 [1 datoshi]
    /// 15 : PUSHDATA1 6772617065 'grape' [8 datoshi]
    /// 1C : PUSH2 [1 datoshi]
    /// 1D : PACK [2048 datoshi]
    /// 1E : PUSH4 [1 datoshi]
    /// 1F : PUSHDATA1 6170706C65 'apple' [8 datoshi]
    /// 26 : PUSH2 [1 datoshi]
    /// 27 : PACK [2048 datoshi]
    /// 28 : PUSH2 [1 datoshi]
    /// 29 : PACK [2048 datoshi]
    /// 2A : STLOC1 [2 datoshi]
    /// 2B : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 30 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("anonymousObjectCreation")]
    public abstract void AnonymousObjectCreation();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEhESwHBoEM5oEc6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH2 [1 datoshi]
    /// 04 : PUSH1 [1 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : PACK [2048 datoshi]
    /// 07 : STLOC0 [2 datoshi]
    /// 08 : LDLOC0 [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : PICKITEM [64 datoshi]
    /// 0B : LDLOC0 [2 datoshi]
    /// 0C : PUSH1 [1 datoshi]
    /// 0D : PICKITEM [64 datoshi]
    /// 0E : ADD [8 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : PUSHINT32 00000080 [1 datoshi]
    /// 15 : JMPGE 04 [2 datoshi]
    /// 17 : JMP 0A [2 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1F : JMPLE 1E [2 datoshi]
    /// 21 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2A : AND [8 datoshi]
    /// 2B : DUP [2 datoshi]
    /// 2C : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 31 : JMPLE 0C [2 datoshi]
    /// 33 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 3C : SUB [8 datoshi]
    /// 3D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEhESwEoQeNBKEXnQcGgQzmgRzp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// 00 : INITSLOT 0102 [64 datoshi]
    /// 03 : PUSH2 [1 datoshi]
    /// 04 : PUSH1 [1 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : PACK [2048 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : PUSH0 [1 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : SETITEM [8192 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : PUSH1 [1 datoshi]
    /// 0D : LDARG1 [2 datoshi]
    /// 0E : SETITEM [8192 datoshi]
    /// 0F : STLOC0 [2 datoshi]
    /// 10 : LDLOC0 [2 datoshi]
    /// 11 : PUSH0 [1 datoshi]
    /// 12 : PICKITEM [64 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : PUSH1 [1 datoshi]
    /// 15 : PICKITEM [64 datoshi]
    /// 16 : ADD [8 datoshi]
    /// 17 : DUP [2 datoshi]
    /// 18 : PUSHINT32 00000080 [1 datoshi]
    /// 1D : JMPGE 04 [2 datoshi]
    /// 1F : JMP 0A [2 datoshi]
    /// 21 : DUP [2 datoshi]
    /// 22 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 27 : JMPLE 1E [2 datoshi]
    /// 29 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 32 : AND [8 datoshi]
    /// 33 : DUP [2 datoshi]
    /// 34 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 39 : JMPLE 0C [2 datoshi]
    /// 3B : PUSHINT64 0000000001000000 [1 datoshi]
    /// 44 : SUB [8 datoshi]
    /// 45 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum1")]
    public abstract BigInteger? Sum1(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEhESwHB4SmgQUdBFeUpoEVHQRWgQzmgRzp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// 00 : INITSLOT 0102 [64 datoshi]
    /// 03 : PUSH2 [1 datoshi]
    /// 04 : PUSH1 [1 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : PACK [2048 datoshi]
    /// 07 : STLOC0 [2 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : LDLOC0 [2 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : ROT [2 datoshi]
    /// 0D : SETITEM [8192 datoshi]
    /// 0E : DROP [2 datoshi]
    /// 0F : LDARG1 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : LDLOC0 [2 datoshi]
    /// 12 : PUSH1 [1 datoshi]
    /// 13 : ROT [2 datoshi]
    /// 14 : SETITEM [8192 datoshi]
    /// 15 : DROP [2 datoshi]
    /// 16 : LDLOC0 [2 datoshi]
    /// 17 : PUSH0 [1 datoshi]
    /// 18 : PICKITEM [64 datoshi]
    /// 19 : LDLOC0 [2 datoshi]
    /// 1A : PUSH1 [1 datoshi]
    /// 1B : PICKITEM [64 datoshi]
    /// 1C : ADD [8 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : PUSHINT32 00000080 [1 datoshi]
    /// 23 : JMPGE 04 [2 datoshi]
    /// 25 : JMP 0A [2 datoshi]
    /// 27 : DUP [2 datoshi]
    /// 28 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2D : JMPLE 1E [2 datoshi]
    /// 2F : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 38 : AND [8 datoshi]
    /// 39 : DUP [2 datoshi]
    /// 3A : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3F : JMPLE 0C [2 datoshi]
    /// 41 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 4A : SUB [8 datoshi]
    /// 4B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum2")]
    public abstract BigInteger? Sum2(BigInteger? a, BigInteger? b);

    #endregion
}
