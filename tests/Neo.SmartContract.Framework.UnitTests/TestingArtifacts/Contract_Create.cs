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
    /// 00 : OpCode.CALLT 0400
    /// 03 : OpCode.RET
    /// </remarks>
    [DisplayName("destroy")]
    public abstract void Destroy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QZXaOoFA
    /// 00 : OpCode.SYSCALL 95DA3A81
    /// 05 : OpCode.RET
    /// </remarks>
    [DisplayName("getCallFlags")]
    public abstract BigInteger? GetCallFlags();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcBAEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALLT 0100
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("getContractById")]
    public abstract object? GetContractById(BigInteger? id);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEANwIAcGhBnAjtnEVoQfNUvx0RzkA=
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.CALLT 0200
    /// 06 : OpCode.STLOC0
    /// 07 : OpCode.LDLOC0
    /// 08 : OpCode.SYSCALL 9C08ED9C
    /// 0D : OpCode.DROP
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.SYSCALL F354BF1D
    /// 14 : OpCode.PUSH1
    /// 15 : OpCode.PICKITEM
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("getContractHashes")]
    public abstract object? GetContractHashes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QTlTbjw3AAAUzhDOQA==
    /// 00 : OpCode.SYSCALL 39536E3C
    /// 05 : OpCode.CALLT 0000
    /// 08 : OpCode.PUSH4
    /// 09 : OpCode.PICKITEM
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.PICKITEM
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("oldContract")]
    public abstract string? OldContract();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACC3l42yg3AwBA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.PUSHNULL
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.CONVERT 28
    /// 08 : OpCode.CALLT 0300
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nef, string? manifest);

    #endregion
}
