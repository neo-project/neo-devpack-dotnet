using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Abort(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Abort"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAbort"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testAbortMsg"",""parameters"":[],""returntype"":""Integer"",""offset"":6,""safe"":false},{""name"":""testAbortInFunction"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":23,""safe"":false},{""name"":""testAbortInTry"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":35,""safe"":false},{""name"":""testAbortInCatch"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":75,""safe"":false},{""name"":""testAbortInFinally"",""parameters"":[{""name"":""abortMsg"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":117,""safe"":false},{""name"":""isGuardSet"",""parameters"":[],""returntype"":""Boolean"",""offset"":215,""safe"":false},{""name"":""isFinallyMarkerSet"",""parameters"":[],""returntype"":""Boolean"",""offset"":233,""safe"":false},{""name"":""guardedCall"",""parameters"":[{""name"":""abort"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":251,""safe"":false},{""name"":""catchGuardedAbort"",""parameters"":[],""returntype"":""Boolean"",""offset"":282,""safe"":false},{""name"":""catchGuardedThrow"",""parameters"":[],""returntype"":""Boolean"",""offset"":346,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP11AVcBABBwOFcBABBwDAlBQk9SVCBNU0fgVwEBEHB4JgQ05zTfVwMBQ3IQcDsKFngmBDTWNM5xQ2oyBUUi+xFwPQUScD9oSsoUMgM6QFcDAUNyEHA7ESARcAwJZXhjZXB0aW9uOnFDajIFRSL7eCYENJg0kBJwP1cDAUNyEHA7BxMRcD0bcUNqMgVFIvsScD0PeCYHNXT///81af///1cBAAwBENswQdWNXuhwaNgkFAwPYWxyZWFkeSBndWFyZGVkOhEMARDbMEE5DOMKQAwBENswQXVU9ZRAVwEADAEQ2zBB1Y1e6HBo2KpAVwEADAER2zBB1Y1e6HBo2KpAVwABNJ47ABZ4Jg8MCkFCT1JUIENBTEzgCD0FNLY/QFcCAENxOwkACDTYRT0OcENpMgVFIvs0pz0ENKNANWX///87ABAMClRIUk9XIENBTEw6NIARDAER2zBBOQzjCj9XAgBDcTsFADTVcENpMgVFIvs1a////6o9AkD9LUlE").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAQ3E7CQAINNhFPQ5wQ2kyBUUi+zSnPQQ0o0A=
    /// INITSLOT 0200 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0900 [4 datoshi]
    /// PUSHT [1 datoshi]
    /// CALL D8 [512 datoshi]
    /// DROP [2 datoshi]
    /// ENDTRY 0E [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// CALL A7 [512 datoshi]
    /// ENDTRY 04 [4 datoshi]
    /// CALL A3 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("catchGuardedAbort")]
    public abstract bool? CatchGuardedAbort();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAQ3E7BQA01XBDaTIFRSL7NWv///+qPQJA
    /// INITSLOT 0200 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0500 [4 datoshi]
    /// CALL D5 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// CALL_L 6BFFFFFF [512 datoshi]
    /// NOT [4 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("catchGuardedThrow")]
    public abstract bool? CatchGuardedThrow();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABNJ47ABZ4Jg8MCkFCT1JUIENBTEzgCD0FNLY/QA==
    /// INITSLOT 0001 [64 datoshi]
    /// CALL 9E [512 datoshi]
    /// TRY 0016 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 0F [2 datoshi]
    /// PUSHDATA1 41424F52542043414C4C [8 datoshi]
    /// ABORTMSG [0 datoshi]
    /// PUSHT [1 datoshi]
    /// ENDTRY 05 [4 datoshi]
    /// CALL B6 [512 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("guardedCall")]
    public abstract bool? GuardedCall(bool? abort);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAER2zBB1Y1e6HBo2KpA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHDATA1 11 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isFinallyMarkerSet")]
    public abstract bool? IsFinallyMarkerSet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAEQ2zBB1Y1e6HBo2KpA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHDATA1 10 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isGuardSet")]
    public abstract bool? IsGuardSet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHA4
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ABORT [0 datoshi]
    /// </remarks>
    [DisplayName("testAbort")]
    public abstract BigInteger? TestAbort();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBQ3IQcDsRIBFwDAlleGNlcHRpb246cUNqMgVFIvt4JgQ0mDSQEnA/
    /// INITSLOT 0301 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 1120 [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// CALL 98 [512 datoshi]
    /// CALL 90 [512 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// </remarks>
    [DisplayName("testAbortInCatch")]
    public abstract BigInteger? TestAbortInCatch(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBQ3IQcDsHExFwPRtxQ2oyBUUi+xJwPQ94Jgc1dP///zVp////
    /// INITSLOT 0301 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 0713 [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 1B [4 datoshi]
    /// STLOC1 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 0F [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// CALL_L 74FFFFFF [512 datoshi]
    /// CALL_L 69FFFFFF [512 datoshi]
    /// </remarks>
    [DisplayName("testAbortInFinally")]
    public abstract BigInteger? TestAbortInFinally(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEHB4JgQ05zTf
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// CALL E7 [512 datoshi]
    /// CALL DF [512 datoshi]
    /// </remarks>
    [DisplayName("testAbortInFunction")]
    public abstract BigInteger? TestAbortInFunction(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBQ3IQcDsKFngmBDTWNM5xQ2oyBUUi+xFwPQUScD9oSsoUMgM6QA==
    /// INITSLOT 0301 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 0A16 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// CALL D6 [512 datoshi]
    /// CALL CE [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDTRY 05 [4 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// ENDFINALLY [4 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAbortInTry")]
    public abstract BigInteger? TestAbortInTry(bool? abortMsg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHAMCUFCT1JUIE1TR+A=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 41424F5254204D5347 [8 datoshi]
    /// ABORTMSG [0 datoshi]
    /// </remarks>
    [DisplayName("testAbortMsg")]
    public abstract BigInteger? TestAbortMsg();

    #endregion
}
