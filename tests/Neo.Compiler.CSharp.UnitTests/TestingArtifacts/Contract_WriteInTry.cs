using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_WriteInTry(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_WriteInTry"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""baseTry"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""tryWrite"",""parameters"":[],""returntype"":""Void"",""offset"":108,""safe"":false},{""name"":""tryWriteWithVulnerability"",""parameters"":[],""returntype"":""Void"",""offset"":173,""safe"":false},{""name"":""recursiveTry"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Void"",""offset"":187,""safe"":false},{""name"":""mutualRecursiveTry"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Void"",""offset"":309,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2EAVcCADsHLzQ6PTdwOwAHNEk9ADsdAAwXdGhyb3cgaW4gbmVzdGVkIGZpbmFsbHk6cWk6OwoADAEANCU9BHA4P0AQDAEANANAVwACeXhBm/ZnzkHmPxiEQAwBADQDQFcAAXhBm/ZnzkEvWMXtQFcBADsdADTODBV0aHJvdyBpbiBUcnlXcml0ZSB0cnk6cDsAHzTHDBd0aHJvdyBpbiBUcnlXcml0ZSBjYXRjaDo/VwEAOwcANKQ9BXA9AkBXAAE7AEI1f////3gQtyY1eBGfSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnzS/PTZ4EZ9KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfNAQ/QFcAATsASngQtyY4eBGfSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnzVK////NTf///813P7//z0DP0AG3exP"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAOwcvNDo9N3A7AAc0ST0AOx0ADBd0aHJvdyBpbiBuZXN0ZWQgZmluYWxseTpxaTo7CgAMAQA0JT0EcDg/QA==
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : TRY 072F [4 datoshi]
    /// 06 : CALL 3A [512 datoshi]
    /// 08 : ENDTRY 37 [4 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : TRY 0007 [4 datoshi]
    /// 0E : CALL 49 [512 datoshi]
    /// 10 : ENDTRY 00 [4 datoshi]
    /// 12 : TRY 1D00 [4 datoshi]
    /// 15 : PUSHDATA1 7468726F7720696E206E65737465642066696E616C6C79 [8 datoshi]
    /// 2E : THROW [512 datoshi]
    /// 2F : STLOC1 [2 datoshi]
    /// 30 : LDLOC1 [2 datoshi]
    /// 31 : THROW [512 datoshi]
    /// 32 : TRY 0A00 [4 datoshi]
    /// 35 : PUSHDATA1 00 [8 datoshi]
    /// 38 : CALL 25 [512 datoshi]
    /// 3A : ENDTRY 04 [4 datoshi]
    /// 3C : STLOC0 [2 datoshi]
    /// 3D : ABORT [0 datoshi]
    /// 3E : ENDFINALLY [4 datoshi]
    /// 3F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("baseTry")]
    public abstract void BaseTry();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABOwBKeBC3Jjh4EZ9KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfNUr///81N////zXc/v//PQM/QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : TRY 004A [4 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : GT [8 datoshi]
    /// 09 : JMPIFNOT 38 [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : PUSH1 [1 datoshi]
    /// 0D : SUB [8 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSHINT32 00000080 [1 datoshi]
    /// 14 : JMPGE 04 [2 datoshi]
    /// 16 : JMP 0A [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1E : JMPLE 1E [2 datoshi]
    /// 20 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 29 : AND [8 datoshi]
    /// 2A : DUP [2 datoshi]
    /// 2B : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 30 : JMPLE 0C [2 datoshi]
    /// 32 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 3B : SUB [8 datoshi]
    /// 3C : CALL_L 4AFFFFFF [512 datoshi]
    /// 41 : CALL_L 37FFFFFF [512 datoshi]
    /// 46 : CALL_L DCFEFFFF [512 datoshi]
    /// 4B : ENDTRY 03 [4 datoshi]
    /// 4D : ENDFINALLY [4 datoshi]
    /// 4E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("mutualRecursiveTry")]
    public abstract void MutualRecursiveTry(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABOwBCNX////94ELcmNXgRn0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ80vz02eBGfSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnzQEP0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : TRY 0042 [4 datoshi]
    /// 06 : CALL_L 7FFFFFFF [512 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : PUSH0 [1 datoshi]
    /// 0D : GT [8 datoshi]
    /// 0E : JMPIFNOT 35 [2 datoshi]
    /// 10 : LDARG0 [2 datoshi]
    /// 11 : PUSH1 [1 datoshi]
    /// 12 : SUB [8 datoshi]
    /// 13 : DUP [2 datoshi]
    /// 14 : PUSHINT32 00000080 [1 datoshi]
    /// 19 : JMPGE 04 [2 datoshi]
    /// 1B : JMP 0A [2 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 23 : JMPLE 1E [2 datoshi]
    /// 25 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2E : AND [8 datoshi]
    /// 2F : DUP [2 datoshi]
    /// 30 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 35 : JMPLE 0C [2 datoshi]
    /// 37 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 40 : SUB [8 datoshi]
    /// 41 : CALL BF [512 datoshi]
    /// 43 : ENDTRY 36 [4 datoshi]
    /// 45 : LDARG0 [2 datoshi]
    /// 46 : PUSH1 [1 datoshi]
    /// 47 : SUB [8 datoshi]
    /// 48 : DUP [2 datoshi]
    /// 49 : PUSHINT32 00000080 [1 datoshi]
    /// 4E : JMPGE 04 [2 datoshi]
    /// 50 : JMP 0A [2 datoshi]
    /// 52 : DUP [2 datoshi]
    /// 53 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 58 : JMPLE 1E [2 datoshi]
    /// 5A : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 63 : AND [8 datoshi]
    /// 64 : DUP [2 datoshi]
    /// 65 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 6A : JMPLE 0C [2 datoshi]
    /// 6C : PUSHINT64 0000000001000000 [1 datoshi]
    /// 75 : SUB [8 datoshi]
    /// 76 : CALL 04 [512 datoshi]
    /// 78 : ENDFINALLY [4 datoshi]
    /// 79 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("recursiveTry")]
    public abstract void RecursiveTry(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAOx0ANM4MFXRocm93IGluIFRyeVdyaXRlIHRyeTpwOwAfNMcMF3Rocm93IGluIFRyeVdyaXRlIGNhdGNoOj8=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : TRY 1D00 [4 datoshi]
    /// 06 : CALL CE [512 datoshi]
    /// 08 : PUSHDATA1 7468726F7720696E20547279577269746520747279 [8 datoshi]
    /// 1F : THROW [512 datoshi]
    /// 20 : STLOC0 [2 datoshi]
    /// 21 : TRY 001F [4 datoshi]
    /// 24 : CALL C7 [512 datoshi]
    /// 26 : PUSHDATA1 7468726F7720696E205472795772697465206361746368 [8 datoshi]
    /// 3F : THROW [512 datoshi]
    /// 40 : ENDFINALLY [4 datoshi]
    /// </remarks>
    [DisplayName("tryWrite")]
    public abstract void TryWrite();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAOwcANKQ9BXA9AkA=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : TRY 0700 [4 datoshi]
    /// 06 : CALL A4 [512 datoshi]
    /// 08 : ENDTRY 05 [4 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : ENDTRY 02 [4 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryWriteWithVulnerability")]
    public abstract void TryWriteWithVulnerability();

    #endregion
}
