using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Blockchain : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Blockchain"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getHeight"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""getTransactionHeight"",""parameters"":[{""name"":""hash"",""type"":""Hash256""}],""returntype"":""Integer"",""offset"":6,""safe"":false},{""name"":""getBlockByHash"",""parameters"":[{""name"":""hash"",""type"":""Hash256""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":16,""safe"":false},{""name"":""getBlockByIndex"",""parameters"":[{""name"":""index"",""type"":""Integer""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":251,""safe"":false},{""name"":""getTxByHash"",""parameters"":[{""name"":""hash"",""type"":""Hash256""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":269,""safe"":false},{""name"":""getTxByBlockHash"",""parameters"":[{""name"":""blockHash"",""type"":""Hash256""},{""name"":""txIndex"",""type"":""Integer""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":545,""safe"":false},{""name"":""getTxByBlockIndex"",""parameters"":[{""name"":""blockIndex"",""type"":""Integer""},{""name"":""txIndex"",""type"":""Integer""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":564,""safe"":false},{""name"":""getContract"",""parameters"":[{""name"":""hash"",""type"":""Hash160""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":583,""safe"":false},{""name"":""getTxVMState"",""parameters"":[{""name"":""hash"",""type"":""Hash256""}],""returntype"":""Integer"",""offset"":733,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""currentIndex"",""getBlock"",""getTransaction"",""getTransactionFromBlock"",""getTransactionHeight"",""getTransactionSigners"",""getTransactionVMState""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM3Rlc3Rob3N0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAi+8gQxQDYqd8FQmcfmTBL3ALZl2gxjdXJyZW50SW5kZXgAAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoUZ2V0VHJhbnNhY3Rpb25IZWlnaHQBAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoIZ2V0QmxvY2sBAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoOZ2V0VHJhbnNhY3Rpb24BAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoVZ2V0VHJhbnNhY3Rpb25TaWduZXJzAQABD77yBDFANip3wVCZx+ZMEvcAtmXaF2dldFRyYW5zYWN0aW9uRnJvbUJsb2NrAgABD/2j+kNG6lMqJY/El92t22Q3yf3/C2dldENvbnRyYWN0AQABD77yBDFANip3wVCZx+ZMEvcAtmXaFWdldFRyYW5zYWN0aW9uVk1TdGF0ZQEAAQ8AAP3nAjcAACICQFcAAXg3AQAiAkBXAQJ4NwIAcHloNAUiAkBXAAJ4C5cmGQwKTlVMTCBCbG9ja0HP50eWCyPBAAAAeQwESGFzaJcmCngQziOvAAAAeQwFSW5kZXiXJgp4Fs4jnAAAAHkMCk1lcmtsZVJvb3SXJgp4E84jhAAAAHkMDU5leHRDb25zZW5zdXOXJgd4GM4iaXkMCFByZXZIYXNolyYHeBLOIlZ5DAlUaW1lc3RhbXCXJgd4FM4iQnkMEVRyYW5zYWN0aW9uc0NvdW50lyYHeBnOIiZ5DAdWZXJzaW9ulyYHeBHOIhQMD1Vrbm93biBwcm9wZXJ0eTpAVwECeDcCAHB5aDUa////IgJAVwECeDcDAHB5aDQFIgJAVwACeAuXJhYMB05VTEwgVHhBz+dHlgsj7QAAAHkMBEhhc2iXJgp4EM4j2wAAAHkMCk5ldHdvcmtGZWWXJgp4Fc4jwwAAAHkMBU5vbmNllyYKeBLOI7AAAAB5DAZTY3JpcHSXJgp4F84jnAAAAHkMBlNlbmRlcpcmCngTziOIAAAAeQwJU3lzdGVtRmVllyYHeBTOInF5DA9WYWxpZFVudGlsQmxvY2uXJgd4Fs4iV3kMB1ZlcnNpb26XJgd4Ec4iRXkMB1NpZ25lcnOXJgp4EM43BAAiMHkMCkZpcnN0U2NvcGWXJg54EM43BAAQzhHOIhQMD1Vrbm93biBwcm9wZXJ0eTpAVwEDeXg3BQBwemg18P7//yICQFcBA3l4NwUAcHpoNd3+//8iAkBXAQJ4NwYAcHloNAUiAkBXAAJ4C5cmGQwNTlVMTCBjb250cmFjdEHP50eWCyJpeQwCSWSXJgd4EM4iXHkMDVVwZGF0ZUNvdW50ZXKXJgd4Ec4iRHkMBEhhc2iXJgd4Es4iNXkMCE1hbmlmZXN0lyYHeBTOIiJ5DANOZWaXJgd4E84iFAwPVWtub3duIHByb3BlcnR5OkBXAAF4NwcAIgJApF/bZw=="));

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

    #region Constructor for internal use only

    protected Contract_Blockchain(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
