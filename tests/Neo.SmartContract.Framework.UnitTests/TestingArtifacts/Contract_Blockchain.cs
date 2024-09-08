using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Blockchain(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Blockchain"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getHeight"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""getTransactionHeight"",""parameters"":[{""name"":""hash"",""type"":""Hash256""}],""returntype"":""Integer"",""offset"":4,""safe"":false},{""name"":""getBlockByHash"",""parameters"":[{""name"":""hash"",""type"":""Hash256""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":12,""safe"":false},{""name"":""getBlockByIndex"",""parameters"":[{""name"":""index"",""type"":""Integer""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":223,""safe"":false},{""name"":""getTxByHash"",""parameters"":[{""name"":""hash"",""type"":""Hash256""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":239,""safe"":false},{""name"":""getTxByBlockHash"",""parameters"":[{""name"":""blockHash"",""type"":""Hash256""},{""name"":""txIndex"",""type"":""Integer""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":483,""safe"":false},{""name"":""getTxByBlockIndex"",""parameters"":[{""name"":""blockIndex"",""type"":""Integer""},{""name"":""txIndex"",""type"":""Integer""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":500,""safe"":false},{""name"":""getContract"",""parameters"":[{""name"":""hash"",""type"":""Hash160""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":517,""safe"":false},{""name"":""getTxVMState"",""parameters"":[{""name"":""hash"",""type"":""Hash256""}],""returntype"":""Integer"",""offset"":658,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""currentIndex"",""getBlock"",""getTransaction"",""getTransactionFromBlock"",""getTransactionHeight"",""getTransactionSigners"",""getTransactionVMState""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAi+8gQxQDYqd8FQmcfmTBL3ALZl2gxjdXJyZW50SW5kZXgAAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoUZ2V0VHJhbnNhY3Rpb25IZWlnaHQBAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoIZ2V0QmxvY2sBAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoOZ2V0VHJhbnNhY3Rpb24BAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoVZ2V0VHJhbnNhY3Rpb25TaWduZXJzAQABD77yBDFANip3wVCZx+ZMEvcAtmXaF2dldFRyYW5zYWN0aW9uRnJvbUJsb2NrAgABD/2j+kNG6lMqJY/El92t22Q3yf3/C2dldENvbnRyYWN0AQABD77yBDFANip3wVCZx+ZMEvcAtmXaFWdldFRyYW5zYWN0aW9uVk1TdGF0ZQEAAQ8AAP2aAjcAAEBXAAF4NwEAQFcBAng3AgBweWg0A0BXAAJ4C5cmFQwKTlVMTCBCbG9ja0HP50eWC0B5DARIYXNolyYGeBDOQHkMBUluZGV4lyYGeBbOQHkMCk1lcmtsZVJvb3SXJgZ4E85AeQwNTmV4dENvbnNlbnN1c5cmBngYzkB5DAhQcmV2SGFzaJcmBngSzkB5DAlUaW1lc3RhbXCXJgZ4FM5AeQwRVHJhbnNhY3Rpb25zQ291bnSXJgZ4Gc5AeQwHVmVyc2lvbpcmBngRzkAMD1Vrbm93biBwcm9wZXJ0eTpXAQJ4NwIAcHloNTD///9AVwECeDcDAHB5aDQDQFcAAngLlyYSDAdOVUxMIFR4Qc/nR5YLQHkMBEhhc2iXJgZ4EM5AeQwKTmV0d29ya0ZlZZcmBngVzkB5DAVOb25jZZcmBngSzkB5DAZTY3JpcHSXJgZ4F85AeQwGU2VuZGVylyYGeBPOQHkMCVN5c3RlbUZlZZcmBngUzkB5DA9WYWxpZFVudGlsQmxvY2uXJgZ4Fs5AeQwHVmVyc2lvbpcmBngRzkB5DAdTaWduZXJzlyYJeBDONwQAQHkMCkZpcnN0U2NvcGWXJg14EM43BAAQzhHOQAwPVWtub3duIHByb3BlcnR5OlcBA3l4NwUAcHpoNQ7///9AVwEDeXg3BQBwemg1/f7//0BXAQJ4NwYAcHloNANAVwACeAuXJhgMDU5VTEwgY29udHJhY3RBz+dHlgtAeQwCSWSXJgZ4EM5AeQwNVXBkYXRlQ291bnRlcpcmBngRzkB5DARIYXNolyYGeBLOQHkMCE1hbmlmZXN0lyYGeBTOQHkMA05lZpcmBngTzkAMD1Vrbm93biBwcm9wZXJ0eTpXAAF4NwcAQP1Rmag="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getBlockByHash")]
    public abstract object? GetBlockByHash(UInt256? hash, string? whatReturn);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getBlockByIndex")]
    public abstract object? GetBlockByIndex(BigInteger? index, string? whatReturn);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getContract")]
    public abstract object? GetContract(UInt160? hash, string? whatReturn);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getHeight")]
    public abstract BigInteger? GetHeight();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTransactionHeight")]
    public abstract BigInteger? GetTransactionHeight(UInt256? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTxByBlockHash")]
    public abstract object? GetTxByBlockHash(UInt256? blockHash, BigInteger? txIndex, string? whatReturn);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTxByBlockIndex")]
    public abstract object? GetTxByBlockIndex(BigInteger? blockIndex, BigInteger? txIndex, string? whatReturn);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTxByHash")]
    public abstract object? GetTxByHash(UInt256? hash, string? whatReturn);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTxVMState")]
    public abstract BigInteger? GetTxVMState(UInt256? hash);

    #endregion

}
