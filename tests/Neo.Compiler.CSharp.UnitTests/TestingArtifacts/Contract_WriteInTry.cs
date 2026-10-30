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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_WriteInTry"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""baseTry"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""tryWrite"",""parameters"":[],""returntype"":""Void"",""offset"":111,""safe"":false},{""name"":""tryWriteWithVulnerability"",""parameters"":[],""returntype"":""Void"",""offset"":185,""safe"":false},{""name"":""recursiveTry"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Void"",""offset"":208,""safe"":false},{""name"":""mutualRecursiveTry"",""parameters"":[{""name"":""i"",""type"":""Integer""}],""returntype"":""Void"",""offset"":299,""safe"":false},{""name"":""safeTryWithCatchWithThrowInFinally"",""parameters"":[],""returntype"":""Void"",""offset"":364,""safe"":false},{""name"":""unsafeNestedTryWrite"",""parameters"":[{""name"":""recursive"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":447,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3vAVcDAENxOwc/NFQ9UXBDaTIFRSL7OwAHNE89KTsdAAwXdGhyb3cgaW4gbmVzdGVkIGZpbmFsbHk6ckNpMgVFIvtqOj0ZOw0ADAEAQXVU9ZQ9C3BDaTIFRSL7OD9AEAwBAEE5DOMKQAwBAEF1VPWUQFcCAENxOx0ANOUMFXRocm93IGluIFRyeVdyaXRlIHRyeTpwQ2kyBUUi+zsAHzTKDBd0aHJvdyBpbiBUcnlXcml0ZSBjYXRjaDo/VwIAQ3E7BwA0pT0McENpMgVFIvs9AkBXAAE7ADE0hngQtyYneJ1KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ800D0oeJ1KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ80BD9AVwABOwA8eBC3Jip4nUrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnzV3////NVv///81A////z0DP0BXAwBDcTslTTsKFDXl/v//PRhwQ2kyBUUi+z0ODAlleGNlcHRpb246PStwQ2kyBUUi+zsKFDW7/v//PRhyQ2kyBUUi+z0ODAlleGNlcHRpb246OFcCAUNxOyAAOwAKNZL+//89CXgmBQk06j8MCWV4Y2VwdGlvbjpwQ2kyBUUi+z0CQN+UOGg=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAQ3E7Bz80VD1RcENpMgVFIvs7AAc0Tz0pOx0ADBd0aHJvdyBpbiBuZXN0ZWQgZmluYWxseTpyQ2kyBUUi+2o6PRk7DQAMAQBBdVT1lD0LcENpMgVFIvs4P0A=
    /// INITSLOT 0300 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 073F [4 datoshi]
    /// CALL 54 [512 datoshi]
    /// ENDTRY 51 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// TRY 0007 [4 datoshi]
    /// CALL 4F [512 datoshi]
    /// ENDTRY 29 [4 datoshi]
    /// TRY 1D00 [4 datoshi]
    /// PUSHDATA1 7468726F7720696E206E65737465642066696E616C6C79 [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC2 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// THROW [512 datoshi]
    /// ENDTRY 19 [4 datoshi]
    /// TRY 0D00 [4 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// SYSCALL 7554F594 'System.Storage.Local.Delete' [32768 datoshi]
    /// ENDTRY 0B [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
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
