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
    /// <remarks>
    /// Script: NwQAQA==
    /// CALLT 0400 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("destroy")]
    public abstract void Destroy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QZXaOoFA
    /// SYSCALL 95DA3A81 'System.Contract.GetCallFlags' [1024 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getCallFlags")]
    public abstract BigInteger? GetCallFlags();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcBAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getContractById")]
    public abstract object? GetContractById(BigInteger? id);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEANwIAcGhBnAjtnEVoQfNUvx0RzkA=
    /// INITSLOT 0100 [64 datoshi]
    /// CALLT 0200 [32768 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 9C08ED9C 'System.Iterator.Next' [32768 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getContractHashes")]
    public abstract object? GetContractHashes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QTlTbjw3AAAUzhDOQA==
    /// SYSCALL 39536E3C 'System.Runtime.GetCallingScriptHash' [16 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("oldContract")]
    public abstract string? OldContract();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACC3l42yg3AwBA
    /// INITSLOT 0002 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0300 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nef, string? manifest);

    #endregion
}
