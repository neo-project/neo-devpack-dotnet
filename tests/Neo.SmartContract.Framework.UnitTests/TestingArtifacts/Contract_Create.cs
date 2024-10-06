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
    /// 0000 : OpCode.CALLT 0400
    /// 0003 : OpCode.RET
    /// </remarks>
    [DisplayName("destroy")]
    public abstract void Destroy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QZXaOoFA
    /// 0000 : OpCode.SYSCALL 95DA3A81
    /// 0005 : OpCode.RET
    /// </remarks>
    [DisplayName("getCallFlags")]
    public abstract BigInteger? GetCallFlags();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcBAEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.CALLT 0100
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("getContractById")]
    public abstract object? GetContractById(BigInteger? id);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEANwIAcGhBnAjtnEVoQfNUvx0RzkA=
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.CALLT 0200
    /// 0006 : OpCode.STLOC0
    /// 0007 : OpCode.LDLOC0
    /// 0008 : OpCode.SYSCALL 9C08ED9C
    /// 000D : OpCode.DROP
    /// 000E : OpCode.LDLOC0
    /// 000F : OpCode.SYSCALL F354BF1D
    /// 0014 : OpCode.PUSH1
    /// 0015 : OpCode.PICKITEM
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("getContractHashes")]
    public abstract object? GetContractHashes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: QTlTbjw3AAAUzhDOQA==
    /// 0000 : OpCode.SYSCALL 39536E3C
    /// 0005 : OpCode.CALLT 0000
    /// 0008 : OpCode.PUSH4
    /// 0009 : OpCode.PICKITEM
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.PICKITEM
    /// 000C : OpCode.RET
    /// </remarks>
    [DisplayName("oldContract")]
    public abstract string? OldContract();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACC3l42yg3AwBA
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.PUSHNULL
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.CONVERT 28
    /// 0008 : OpCode.CALLT 0300
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nef, string? manifest);

    #endregion

}
