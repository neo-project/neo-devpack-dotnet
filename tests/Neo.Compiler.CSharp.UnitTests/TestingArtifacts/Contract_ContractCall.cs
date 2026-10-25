using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_ContractCall(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ContractCall"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testContractCall"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""testContractCallVoid"",""parameters"":[],""returntype"":""Void"",""offset"":5,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x499c4b9ad550f4a1a928bdb69ed63e5e1c2446c5"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALFRiQcXj7Wnra9KKmh9FDVmkucSQl0ZXN0QXJnczEBAAEPxUYkHF4+1p62vSipofRQ1ZpLnEkIdGVzdFZvaWQAAAAPAAAJFDcAAEA3AQBA0cDaYg==").AsSerializable<Neo.SmartContract.NefFile>();

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
