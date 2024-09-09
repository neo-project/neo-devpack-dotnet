using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_UIntTypes(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_UIntTypes"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""checkOwner"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""checkZeroStatic"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":7,""safe"":false},{""name"":""constructUInt160"",""parameters"":[{""name"":""bytes"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":35,""safe"":false},{""name"":""validateAddress"",""parameters"":[{""name"":""address"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":53,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":75,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGVXAAF4WJdAVwABeAwUAAAAAAAAAAAAAAAAAAAAAAAAAACXQFcAAXjbKErYJAlKygAUKAM6QFcAAXhK2ShQygAUs6skBAlAeBCzqkBWAQwU9mRDSY04eNMrmU5OEoPGk0Qh2v5gQDC/e2Q="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkOwner")]
    public abstract bool? CheckOwner(UInt160? owner);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkZeroStatic")]
    public abstract bool? CheckZeroStatic(UInt160? owner);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("constructUInt160")]
    public abstract UInt160? ConstructUInt160(byte[]? bytes);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("validateAddress")]
    public abstract bool? ValidateAddress(UInt160? address);

    #endregion

}
