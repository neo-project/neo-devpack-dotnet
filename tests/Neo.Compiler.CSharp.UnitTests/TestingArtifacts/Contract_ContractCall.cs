using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ContractCall(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ContractCall"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testContractCall"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""testContractCallVoid"",""parameters"":[],""returntype"":""Void"",""offset"":5,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xec0d3d1d7228546998091a9c729d4e6ac693b5a7"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAKntZPGak6dcpwaCZhpVChyHT0N7Al0ZXN0QXJnczEBAAEPp7WTxmpOnXKcGgmYaVQoch09DewIdGVzdFZvaWQAAAAPAAAJFDcAAEA3AQBA8WCflA=="));

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
}
