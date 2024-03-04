using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Create : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Create"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""oldContract"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""getContractById"",""parameters"":[{""name"":""id"",""type"":""Integer""}],""returntype"":""Any"",""offset"":15,""safe"":false},{""name"":""getContractHashes"",""parameters"":[],""returntype"":""Any"",""offset"":25,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nef"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Void"",""offset"":50,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":62,""safe"":false},{""name"":""getCallFlags"",""parameters"":[],""returntype"":""Integer"",""offset"":66,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAX9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/w9nZXRDb250cmFjdEJ5SWQBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8RZ2V0Q29udHJhY3RIYXNoZXMAAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8GdXBkYXRlAwAAD/2j+kNG6lMqJY/El92t22Q3yf3/B2Rlc3Ryb3kAAAAPAABKQTlTbjw3AAAUzhDOIgJAVwABeDcBACICQFcBADcCAHBoQZwI7ZxFaEHzVL8dEc4iAkBXAAILeXjbKDcDAEA3BABAQZXaOoEiAkDDIHI+"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("destroy")]
    public abstract void Destroy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getCallFlags")]
    public abstract BigInteger? GetCallFlags();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getContractById")]
    public abstract object? GetContractById(BigInteger? id);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getContractHashes")]
    public abstract object? GetContractHashes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("oldContract")]
    public abstract string? OldContract();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[]? nef, string? manifest);

    #endregion

    #region Constructor for internal use only

    protected Contract_Create(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
