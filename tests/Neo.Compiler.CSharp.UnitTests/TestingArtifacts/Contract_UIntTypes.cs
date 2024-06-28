using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_UIntTypes : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_UIntTypes"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""checkOwner"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""checkZeroStatic"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":9,""safe"":false},{""name"":""constructUInt160"",""parameters"":[{""name"":""bytes"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":39,""safe"":false},{""name"":""validateAddress"",""parameters"":[{""name"":""address"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":59,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":82,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAABsVwABeFiXIgJAVwABeAwUAAAAAAAAAAAAAAAAAAAAAAAAAACXIgJAVwABeNsoStgkCUrKABQoAzoiAkBXAAF4StkoUMoAFLOrJAUJIgZ4ELOqQFYBDBT2ZENJjTh40yuZTk4Sg8aTRCHa/mBAdk8owQ=="));

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

    #region Constructor for internal use only

    protected Contract_UIntTypes(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
