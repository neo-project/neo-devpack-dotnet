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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_WriteInTry"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""baseTry"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""tryWrite"",""parameters"":[],""returntype"":""Void"",""offset"":115,""safe"":false},{""name"":""tryWriteWithVulnerability"",""parameters"":[],""returntype"":""Void"",""offset"":189,""safe"":false},{""name"":""recursiveTry"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Void"",""offset"":212,""safe"":false},{""name"":""mutualRecursiveTry"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Void"",""offset"":303,""safe"":false},{""name"":""safeTryWithCatchWithThrowInFinally"",""parameters"":[],""returntype"":""Void"",""offset"":368,""safe"":false},{""name"":""unsafeNestedTryWrite"",""parameters"":[{""name"":""recursive"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":451,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3zAVcFAENxOwdBNFg9VXBDaTIFRSL7OwAHNFM9K0NyOx0ADBd0aHJvdyBpbiBuZXN0ZWQgZmluYWxseTpzQ2oyBUUi+2s6PRtDdDsNAAwBAEF1VPWUPQtzQ2wyBUUi+zg/QBAMAQBBOQzjCkAMAQBBdVT1lEBXAgBDcTsdADTlDBV0aHJvdyBpbiBUcnlXcml0ZSB0cnk6cENpMgVFIvs7AB80ygwXdGhyb3cgaW4gVHJ5V3JpdGUgY2F0Y2g6P1cCAENxOwcANKU9DHBDaTIFRSL7PQJAVwABOwAxNIZ4ELcmJ3idSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfNNA9KHidSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfNAQ/QFcAATsAPHgQtyYqeJ1KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ81d////zVb////NQP///89Az9AVwMAQ3E7JU07ChQ15f7//z0YcENpMgVFIvs9DgwJZXhjZXB0aW9uOj0rcENpMgVFIvs7ChQ1u/7//z0YckNpMgVFIvs9DgwJZXhjZXB0aW9uOjhXAgFDcTsgADsACjWS/v//PQl4JgUJNOo/DAlleGNlcHRpb246cENpMgVFIvs9AkDitaT8").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUAQ3E7B0E0WD1VcENpMgVFIvs7AAc0Uz0rQ3I7HQAMF3Rocm93IGluIG5lc3RlZCBmaW5hbGx5OnNDajIFRSL7azo9G0N0Ow0ADAEAQXVU9ZQ9C3NDbDIFRSL7OD9A
    /// INITSLOT 0500 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0741 [4 datoshi]
    /// CALL 58 [512 datoshi]
    /// ENDTRY 55 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// TRY 0007 [4 datoshi]
    /// CALL 53 [512 datoshi]
    /// ENDTRY 2B [4 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// TRY 1D00 [4 datoshi]
    /// PUSHDATA1 7468726F7720696E206E65737465642066696E616C6C79 [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC3 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// THROW [512 datoshi]
    /// ENDTRY 1B [4 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC4 [2 datoshi]
    /// TRY 0D00 [4 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// SYSCALL 7554F594 'System.Storage.Local.Delete' [32768 datoshi]
    /// ENDTRY 0B [4 datoshi]
    /// STLOC3 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
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
    /// Script: VwABOwA8eBC3Jip4nUrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnzV3////NVv///81A////z0DP0A=
    /// INITSLOT 0001 [64 datoshi]
    /// TRY 003C [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIFNOT 2A [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DEC [4 datoshi]
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
    /// CALL_L 77FFFFFF [512 datoshi]
    /// CALL_L 5BFFFFFF [512 datoshi]
    /// CALL_L 03FFFFFF [512 datoshi]
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
    /// Script: VwABOwAxNIZ4ELcmJ3idSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfNNA9KHidSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfNAQ/QA==
    /// INITSLOT 0001 [64 datoshi]
    /// TRY 0031 [4 datoshi]
    /// CALL 86 [512 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIFNOT 27 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DEC [4 datoshi]
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
    /// CALL D0 [512 datoshi]
    /// ENDTRY 28 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DEC [4 datoshi]
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
    /// Script: VwMAQ3E7JU07ChQ15f7//z0YcENpMgVFIvs9DgwJZXhjZXB0aW9uOj0rcENpMgVFIvs7ChQ1u/7//z0YckNpMgVFIvs9DgwJZXhjZXB0aW9uOjg=
    /// INITSLOT 0300 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 254D [4 datoshi]
    /// TRY 0A14 [4 datoshi]
    /// CALL_L E5FEFFFF [512 datoshi]
    /// ENDTRY 18 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// ENDTRY 0E [4 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// ENDTRY 2B [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// TRY 0A14 [4 datoshi]
    /// CALL_L BBFEFFFF [512 datoshi]
    /// ENDTRY 18 [4 datoshi]
    /// STLOC2 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// ENDTRY 0E [4 datoshi]
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
    /// Script: VwIAQ3E7HQA05QwVdGhyb3cgaW4gVHJ5V3JpdGUgdHJ5OnBDaTIFRSL7OwAfNMoMF3Rocm93IGluIFRyeVdyaXRlIGNhdGNoOj8=
    /// INITSLOT 0200 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 1D00 [4 datoshi]
    /// CALL E5 [512 datoshi]
    /// PUSHDATA1 7468726F7720696E20547279577269746520747279 [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// TRY 001F [4 datoshi]
    /// CALL CA [512 datoshi]
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
    /// Script: VwIAQ3E7BwA0pT0McENpMgVFIvs9AkA=
    /// INITSLOT 0200 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0700 [4 datoshi]
    /// CALL A5 [512 datoshi]
    /// ENDTRY 0C [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryWriteWithVulnerability")]
    public abstract void TryWriteWithVulnerability();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBQ3E7IAA7AAo1kv7//z0JeCYFCTTqPwwJZXhjZXB0aW9uOnBDaTIFRSL7PQJA
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 2000 [4 datoshi]
    /// TRY 000A [4 datoshi]
    /// CALL_L 92FEFFFF [512 datoshi]
    /// ENDTRY 09 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// CALL EA [512 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unsafeNestedTryWrite")]
    public abstract void UnsafeNestedTryWrite(bool? recursive);

    #endregion
}
