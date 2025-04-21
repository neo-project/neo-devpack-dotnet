using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ContractCall(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ContractCall"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testContractCall"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""testContractCallVoid"",""parameters"":[],""returntype"":""Void"",""offset"":5,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xb6ae1662a8228ed73e372b0d0ea11716445a4281"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""Version"":""3.7.4"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAKBQlpEFhehDg0rNz7XjiKoYhautgl0ZXN0QXJnczEBAAEPgUJaRBYXoQ4NKzc+144iqGIWrrYIdGVzdFZvaWQAAAAPAAAJFDcAAEA3AQBA3RHAvA==").AsSerializable<Neo.SmartContract.NefFile>();

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
