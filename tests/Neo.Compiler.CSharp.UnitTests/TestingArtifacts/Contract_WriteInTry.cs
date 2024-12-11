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
    /// INITSLOT 0200 [64 datoshi]
    /// TRY 072F [4 datoshi]
    /// CALL 3A [512 datoshi]
    /// ENDTRY 37 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 0007 [4 datoshi]
    /// CALL 49 [512 datoshi]
    /// ENDTRY 00 [4 datoshi]
    /// TRY 1D00 [4 datoshi]
    /// PUSHDATA1 7468726F7720696E206E65737465642066696E616C6C79 [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// THROW [512 datoshi]
    /// TRY 0A00 [4 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// CALL 25 [512 datoshi]
    /// ENDTRY 04 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// ABORT [0 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("baseTry")]
    public abstract void BaseTry();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABOwBKeBC3Jjh4EZ9KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfNUr///81N////zXc/v//PQM/QA==
    /// INITSLOT 0001 [64 datoshi]
    /// TRY 004A [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIFNOT 38 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// CALL_L 4AFFFFFF [512 datoshi]
    /// CALL_L 37FFFFFF [512 datoshi]
    /// CALL_L DCFEFFFF [512 datoshi]
    /// ENDTRY 03 [4 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("mutualRecursiveTry")]
    public abstract void MutualRecursiveTry(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABOwBCNX////94ELcmNXgRn0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ80vz02eBGfSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnzQEP0A=
    /// INITSLOT 0001 [64 datoshi]
    /// TRY 0042 [4 datoshi]
    /// CALL_L 7FFFFFFF [512 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIFNOT 35 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// CALL BF [512 datoshi]
    /// ENDTRY 36 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// CALL 04 [512 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("recursiveTry")]
    public abstract void RecursiveTry(BigInteger? i);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAOx0ANM4MFXRocm93IGluIFRyeVdyaXRlIHRyeTpwOwAfNMcMF3Rocm93IGluIFRyeVdyaXRlIGNhdGNoOj8=
    /// INITSLOT 0100 [64 datoshi]
    /// TRY 1D00 [4 datoshi]
    /// CALL CE [512 datoshi]
    /// PUSHDATA1 7468726F7720696E20547279577269746520747279 [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 001F [4 datoshi]
    /// CALL C7 [512 datoshi]
    /// PUSHDATA1 7468726F7720696E205472795772697465206361746368 [8 datoshi]
    /// THROW [512 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// </remarks>
    [DisplayName("tryWrite")]
    public abstract void TryWrite();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAOwcANKQ9BXA9AkA=
    /// INITSLOT 0100 [64 datoshi]
    /// TRY 0700 [4 datoshi]
    /// CALL A4 [512 datoshi]
    /// ENDTRY 05 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryWriteWithVulnerability")]
    public abstract void TryWriteWithVulnerability();

    #endregion
}
