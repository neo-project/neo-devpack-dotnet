using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_WriteInTry(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_WriteInTry"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""baseTry"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""tryWrite"",""parameters"":[],""returntype"":""Void"",""offset"":86,""safe"":false},{""name"":""tryWriteWithVulnerability"",""parameters"":[],""returntype"":""Void"",""offset"":151,""safe"":false},{""name"":""recursiveTry"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Void"",""offset"":165,""safe"":false},{""name"":""mutualRecursiveTry"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Void"",""offset"":282,""safe"":false},{""name"":""safeTryWithCatchWithThrowInFinally"",""parameters"":[],""returntype"":""Void"",""offset"":360,""safe"":false},{""name"":""unsafeNestedTryWrite"",""parameters"":[{""name"":""recursive"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":418,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.9.0"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3JAVcCADsHLzQ9PTpwOwAHND89ADsdAAwXdGhyb3cgaW4gbmVzdGVkIGZpbmFsbHk6cWk6Ow0ADAEAQXVU9ZQ9BHA4P0AQDAEAQTkM4wpADAEAQXVU9ZRAVwEAOx0ANOcMFXRocm93IGluIFRyeVdyaXRlIHRyeTpwOwAfNNMMF3Rocm93IGluIFRyeVdyaXRlIGNhdGNoOj9XAQA7BwA0sD0FcD0CQFcAATsAPjSYeBC3JjR4nUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ80wz01eJ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfNAQ/QFcAATsASXgQtyY3eJ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfNVD///81Pf///zXu/v//PQM/QFcCADscNjsKDTXS/v//PQBwPQAMCWV4Y2VwdGlvbjpwOwoNNbj+//89AHE9AAwJZXhjZXB0aW9uOjhXAQE7IAA7AAo1mP7//z0JeCYFCTTsPwwJZXhjZXB0aW9uOnA9AkCebJgB").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAOwcvND09OnA7AAc0Pz0AOx0ADBd0aHJvdyBpbiBuZXN0ZWQgZmluYWxseTpxaTo7DQAMAQBBdVT1lD0EcDg/QA==
    /// INITSLOT 0200 [64 datoshi]
    /// TRY 072F [4 datoshi]
    /// CALL 3D [512 datoshi]
    /// ENDTRY 3A [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 0007 [4 datoshi]
    /// CALL 3F [512 datoshi]
    /// ENDTRY 00 [4 datoshi]
    /// TRY 1D00 [4 datoshi]
    /// PUSHDATA1 7468726F7720696E206E65737465642066696E616C6C79 [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// THROW [512 datoshi]
    /// TRY 0D00 [4 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// SYSCALL 7554F594 'System.Storage.Local.Delete' [32768 datoshi]
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
    /// Script: VwABOwBJeBC3Jjd4nUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ81UP///zU9////Ne7+//89Az9A
    /// INITSLOT 0001 [64 datoshi]
    /// TRY 0049 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIFNOT 37 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DEC [4 datoshi]
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
    /// CALL_L 50FFFFFF [512 datoshi]
    /// CALL_L 3DFFFFFF [512 datoshi]
    /// CALL_L EEFEFFFF [512 datoshi]
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
    /// Script: VwABOwA+NJh4ELcmNHidSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnzTDPTV4nUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ80BD9A
    /// INITSLOT 0001 [64 datoshi]
    /// TRY 003E [4 datoshi]
    /// CALL 98 [512 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIFNOT 34 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DEC [4 datoshi]
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
    /// CALL C3 [512 datoshi]
    /// ENDTRY 35 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DEC [4 datoshi]
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
    /// Script: VwIAOxw2OwoNNdL+//89AHA9AAwJZXhjZXB0aW9uOnA7Cg01uP7//z0AcT0ADAlleGNlcHRpb246OA==
    /// INITSLOT 0200 [64 datoshi]
    /// TRY 1C36 [4 datoshi]
    /// TRY 0A0D [4 datoshi]
    /// CALL_L D2FEFFFF [512 datoshi]
    /// ENDTRY 00 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 00 [4 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 0A0D [4 datoshi]
    /// CALL_L B8FEFFFF [512 datoshi]
    /// ENDTRY 00 [4 datoshi]
    /// STLOC1 [2 datoshi]
    /// ENDTRY 00 [4 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// ABORT [0 datoshi]
    /// </remarks>
    [DisplayName("safeTryWithCatchWithThrowInFinally")]
    public abstract void SafeTryWithCatchWithThrowInFinally();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAOx0ANOcMFXRocm93IGluIFRyeVdyaXRlIHRyeTpwOwAfNNMMF3Rocm93IGluIFRyeVdyaXRlIGNhdGNoOj8=
    /// INITSLOT 0100 [64 datoshi]
    /// TRY 1D00 [4 datoshi]
    /// CALL E7 [512 datoshi]
    /// PUSHDATA1 7468726F7720696E20547279577269746520747279 [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 001F [4 datoshi]
    /// CALL D3 [512 datoshi]
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
    /// Script: VwEAOwcANLA9BXA9AkA=
    /// INITSLOT 0100 [64 datoshi]
    /// TRY 0700 [4 datoshi]
    /// CALL B0 [512 datoshi]
    /// ENDTRY 05 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryWriteWithVulnerability")]
    public abstract void TryWriteWithVulnerability();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOyAAOwAKNZj+//89CXgmBQk07D8MCWV4Y2VwdGlvbjpwPQJA
    /// INITSLOT 0101 [64 datoshi]
    /// TRY 2000 [4 datoshi]
    /// TRY 000A [4 datoshi]
    /// CALL_L 98FEFFFF [512 datoshi]
    /// ENDTRY 09 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// CALL EC [512 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unsafeNestedTryWrite")]
    public abstract void UnsafeNestedTryWrite(bool? recursive);

    #endregion
}
