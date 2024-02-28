using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ExecutionEngine : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ExecutionEngine"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""callingScriptHash"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""entryScriptHash"",""parameters"":[],""returntype"":""ByteArray"",""offset"":10,""safe"":false},{""name"":""executingScriptHash"",""parameters"":[],""returntype"":""ByteArray"",""offset"":20,""safe"":false},{""name"":""scriptContainer"",""parameters"":[],""returntype"":""Any"",""offset"":30,""safe"":false},{""name"":""transaction"",""parameters"":[],""returntype"":""Any"",""offset"":38,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM3Rlc3Rob3N0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAC5BOVNuPNswIgJAQfm04jjbMCICQEHb/qh02zAiAkBBLVEIMCICQEEtUQgwIgJAajU8hg=="));

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

    #region Constructor for internal use only

    protected Contract_ExecutionEngine(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
