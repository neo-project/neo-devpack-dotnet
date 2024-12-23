using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ContractCall(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ContractCall"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testContractCall"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""testContractCallVoid"",""parameters"":[],""returntype"":""Void"",""offset"":5,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0e26a6a9b6f37a54d5666aaa2efb71dc75abfdfa"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL6/at13HH7LqpqZtVUevO2qaYmDgl0ZXN0QXJnczEBAAEP+v2rddxx+y6qambVVHrztqmmJg4IdGVzdFZvaWQAAAAPAAAJFDcAAEA3AQBAPzwuew=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FDcAAEA=
    /// PUSH4 [1 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testContractCall")]
    public abstract byte[]? TestContractCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NwEAQA==
    /// CALLT 0100 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testContractCallVoid")]
    public abstract void TestContractCallVoid();

    #endregion
}
