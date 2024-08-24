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
    [DisplayName("callingScriptHash")]
    public abstract byte[]? CallingScriptHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("entryScriptHash")]
    public abstract byte[]? EntryScriptHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("executingScriptHash")]
    public abstract byte[]? ExecutingScriptHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("scriptContainer")]
    public abstract object? ScriptContainer();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transaction")]
    public abstract object? Transaction();

    #endregion

}
