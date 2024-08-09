using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Blockchain : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Blockchain"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getHeight"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""getTransactionHeight"",""parameters"":[{""name"":""hash"",""type"":""Hash256""}],""returntype"":""Integer"",""offset"":4,""safe"":false},{""name"":""getBlockByHash"",""parameters"":[{""name"":""hash"",""type"":""Hash256""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":12,""safe"":false},{""name"":""getBlockByIndex"",""parameters"":[{""name"":""index"",""type"":""Integer""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":245,""safe"":false},{""name"":""getTxByHash"",""parameters"":[{""name"":""hash"",""type"":""Hash256""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":261,""safe"":false},{""name"":""getTxByBlockHash"",""parameters"":[{""name"":""blockHash"",""type"":""Hash256""},{""name"":""txIndex"",""type"":""Integer""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":535,""safe"":false},{""name"":""getTxByBlockIndex"",""parameters"":[{""name"":""blockIndex"",""type"":""Integer""},{""name"":""txIndex"",""type"":""Integer""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":552,""safe"":false},{""name"":""getContract"",""parameters"":[{""name"":""hash"",""type"":""Hash160""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":569,""safe"":false},{""name"":""getTxVMState"",""parameters"":[{""name"":""hash"",""type"":""Hash256""}],""returntype"":""Integer"",""offset"":717,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""currentIndex"",""getBlock"",""getTransaction"",""getTransactionFromBlock"",""getTransactionHeight"",""getTransactionSigners"",""getTransactionVMState""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAi+8gQxQDYqd8FQmcfmTBL3ALZl2gxjdXJyZW50SW5kZXgAAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoUZ2V0VHJhbnNhY3Rpb25IZWlnaHQBAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoIZ2V0QmxvY2sBAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoOZ2V0VHJhbnNhY3Rpb24BAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoVZ2V0VHJhbnNhY3Rpb25TaWduZXJzAQABD77yBDFANip3wVCZx+ZMEvcAtmXaF2dldFRyYW5zYWN0aW9uRnJvbUJsb2NrAgABD/2j+kNG6lMqJY/El92t22Q3yf3/C2dldENvbnRyYWN0AQABD77yBDFANip3wVCZx+ZMEvcAtmXaFWdldFRyYW5zYWN0aW9uVk1TdGF0ZQEAAQ8AAP3VAjcAAEBXAAF4NwEAQFcBAng3AgBweWg0A0BXAAJ4C5cmGQwKTlVMTCBCbG9ja0HP50eWCyPBAAAAeQwESGFzaJcmCngQziOvAAAAeQwFSW5kZXiXJgp4Fs4jnAAAAHkMCk1lcmtsZVJvb3SXJgp4E84jhAAAAHkMDU5leHRDb25zZW5zdXOXJgd4GM4iaXkMCFByZXZIYXNolyYHeBLOIlZ5DAlUaW1lc3RhbXCXJgd4FM4iQnkMEVRyYW5zYWN0aW9uc0NvdW50lyYHeBnOIiZ5DAdWZXJzaW9ulyYHeBHOIhQMD1Vrbm93biBwcm9wZXJ0eTpAVwECeDcCAHB5aDUa////QFcBAng3AwBweWg0A0BXAAJ4C5cmFgwHTlVMTCBUeEHP50eWCyPtAAAAeQwESGFzaJcmCngQziPbAAAAeQwKTmV0d29ya0ZlZZcmCngVziPDAAAAeQwFTm9uY2WXJgp4Es4jsAAAAHkMBlNjcmlwdJcmCngXziOcAAAAeQwGU2VuZGVylyYKeBPOI4gAAAB5DAlTeXN0ZW1GZWWXJgd4FM4icXkMD1ZhbGlkVW50aWxCbG9ja5cmB3gWziJXeQwHVmVyc2lvbpcmB3gRziJFeQwHU2lnbmVyc5cmCngQzjcEACIweQwKRmlyc3RTY29wZZcmDngQzjcEABDOEc4iFAwPVWtub3duIHByb3BlcnR5OkBXAQN5eDcFAHB6aDXw/v//QFcBA3l4NwUAcHpoNd/+//9AVwECeDcGAHB5aDQDQFcAAngLlyYZDA1OVUxMIGNvbnRyYWN0Qc/nR5YLIml5DAJJZJcmB3gQziJceQwNVXBkYXRlQ291bnRlcpcmB3gRziJEeQwESGFzaJcmB3gSziI1eQwITWFuaWZlc3SXJgd4FM4iInkMA05lZpcmB3gTziIUDA9Va25vd24gcHJvcGVydHk6QFcAAXg3BwBAjOZ9RQ=="));

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
