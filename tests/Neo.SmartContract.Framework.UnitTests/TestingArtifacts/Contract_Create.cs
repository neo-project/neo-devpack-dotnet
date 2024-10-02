using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Create(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Create"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""oldContract"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""getContractById"",""parameters"":[{""name"":""id"",""type"":""Integer""}],""returntype"":""Any"",""offset"":13,""safe"":false},{""name"":""getContractHashes"",""parameters"":[],""returntype"":""Any"",""offset"":21,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nef"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Void"",""offset"":44,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":56,""safe"":false},{""name"":""getCallFlags"",""parameters"":[],""returntype"":""Integer"",""offset"":60,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAX9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/w9nZXRDb250cmFjdEJ5SWQBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8RZ2V0Q29udHJhY3RIYXNoZXMAAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8GdXBkYXRlAwAAD/2j+kNG6lMqJY/El92t22Q3yf3/B2Rlc3Ryb3kAAAAPAABCQTlTbjw3AAAUzhDOQFcAAXg3AQBAVwEANwIAcGhBnAjtnEVoQfNUvx0RzkBXAAILeXjbKDcDAEA3BABAQZXaOoFA1tmaYw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("destroy")]
    public abstract void Destroy();
    // 0000 : CALLT
    // 0003 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getCallFlags")]
    public abstract BigInteger? GetCallFlags();
    // 0000 : SYSCALL
    // 0005 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getContractById")]
    public abstract object? GetContractById(BigInteger? id);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : CALLT
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getContractHashes")]
    public abstract object? GetContractHashes();
    // 0000 : INITSLOT
    // 0003 : CALLT
    // 0006 : STLOC0
    // 0007 : LDLOC0
    // 0008 : SYSCALL
    // 000D : DROP
    // 000E : LDLOC0
    // 000F : SYSCALL
    // 0014 : PUSH1
    // 0015 : PICKITEM
    // 0016 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("oldContract")]
    public abstract string? OldContract();
    // 0000 : SYSCALL
    // 0005 : CALLT
    // 0008 : PUSH4
    // 0009 : PICKITEM
    // 000A : PUSH0
    // 000B : PICKITEM
    // 000C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[]? nef, string? manifest);
    // 0000 : INITSLOT
    // 0003 : PUSHNULL
    // 0004 : LDARG1
    // 0005 : LDARG0
    // 0006 : CONVERT
    // 0008 : CALLT
    // 000B : RET

    #endregion

}
