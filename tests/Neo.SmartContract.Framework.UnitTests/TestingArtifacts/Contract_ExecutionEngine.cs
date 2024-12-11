using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ExecutionEngine(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ExecutionEngine"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""callingScriptHash"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""entryScriptHash"",""parameters"":[],""returntype"":""ByteArray"",""offset"":8,""safe"":false},{""name"":""executingScriptHash"",""parameters"":[],""returntype"":""ByteArray"",""offset"":16,""safe"":false},{""name"":""scriptContainer"",""parameters"":[],""returntype"":""Any"",""offset"":24,""safe"":false},{""name"":""transaction"",""parameters"":[],""returntype"":""Any"",""offset"":30,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACRBOVNuPNswQEH5tOI42zBAQdv+qHTbMEBBLVEIMEBBLVEIMEAFZvrD"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QTlTbjzbMEA=
    /// SYSCALL 39536E3C 'System.Runtime.GetCallingScriptHash' [16 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("callingScriptHash")]
    public abstract byte[]? CallingScriptHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Qfm04jjbMEA=
    /// SYSCALL F9B4E238 'System.Runtime.GetEntryScriptHash' [16 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("entryScriptHash")]
    public abstract byte[]? EntryScriptHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Qdv+qHTbMEA=
    /// SYSCALL DBFEA874 'System.Runtime.GetExecutingScriptHash' [16 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("executingScriptHash")]
    public abstract byte[]? ExecutingScriptHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QS1RCDBA
    /// SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("scriptContainer")]
    public abstract object? ScriptContainer();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QS1RCDBA
    /// SYSCALL 2D510830 'System.Runtime.GetScriptContainer' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("transaction")]
    public abstract object? Transaction();

    #endregion
}
