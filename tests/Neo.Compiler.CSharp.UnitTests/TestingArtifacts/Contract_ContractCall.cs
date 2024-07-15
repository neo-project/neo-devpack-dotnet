using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ContractCall : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ContractCall"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testContractCall"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""testContractCallVoid"",""parameters"":[],""returntype"":""Void"",""offset"":7,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x506d98f49afa8d3722039d7be3e7e7844ad4072a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIqB9RKhOfn43udAyI3jfqa9JhtUAl0ZXN0QXJnczEBAAEPKgfUSoTn5+N7nQMiN436mvSYbVAIdGVzdFZvaWQAAAAPAAALFDcAACICQDcBAED24cDi"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testContractCall")]
    public abstract byte[]? TestContractCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testContractCallVoid")]
    public abstract void TestContractCallVoid();

    #endregion

    #region Constructor for internal use only

    protected Contract_ContractCall(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
