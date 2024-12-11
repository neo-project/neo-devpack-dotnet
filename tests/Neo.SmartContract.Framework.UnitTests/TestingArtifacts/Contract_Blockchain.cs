using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Blockchain(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Blockchain"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getHeight"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""getTransactionHeight"",""parameters"":[{""name"":""hash"",""type"":""Hash256""}],""returntype"":""Integer"",""offset"":4,""safe"":false},{""name"":""getBlockByHash"",""parameters"":[{""name"":""hash"",""type"":""Hash256""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":12,""safe"":false},{""name"":""getBlockByIndex"",""parameters"":[{""name"":""index"",""type"":""Integer""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":222,""safe"":false},{""name"":""getTxByHash"",""parameters"":[{""name"":""hash"",""type"":""Hash256""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":238,""safe"":false},{""name"":""getTxByBlockHash"",""parameters"":[{""name"":""blockHash"",""type"":""Hash256""},{""name"":""txIndex"",""type"":""Integer""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":481,""safe"":false},{""name"":""getTxByBlockIndex"",""parameters"":[{""name"":""blockIndex"",""type"":""Integer""},{""name"":""txIndex"",""type"":""Integer""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":498,""safe"":false},{""name"":""getContract"",""parameters"":[{""name"":""hash"",""type"":""Hash160""},{""name"":""whatReturn"",""type"":""String""}],""returntype"":""Any"",""offset"":515,""safe"":false},{""name"":""getTxVMState"",""parameters"":[{""name"":""hash"",""type"":""Hash256""}],""returntype"":""Integer"",""offset"":655,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""currentIndex"",""getBlock"",""getTransaction"",""getTransactionFromBlock"",""getTransactionHeight"",""getTransactionSigners"",""getTransactionVMState""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAi+8gQxQDYqd8FQmcfmTBL3ALZl2gxjdXJyZW50SW5kZXgAAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoUZ2V0VHJhbnNhY3Rpb25IZWlnaHQBAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoIZ2V0QmxvY2sBAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoOZ2V0VHJhbnNhY3Rpb24BAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoVZ2V0VHJhbnNhY3Rpb25TaWduZXJzAQABD77yBDFANip3wVCZx+ZMEvcAtmXaF2dldFRyYW5zYWN0aW9uRnJvbUJsb2NrAgABD/2j+kNG6lMqJY/El92t22Q3yf3/C2dldENvbnRyYWN0AQABD77yBDFANip3wVCZx+ZMEvcAtmXaFWdldFRyYW5zYWN0aW9uVk1TdGF0ZQEAAQ8AAP2XAjcAAEBXAAF4NwEAQFcBAng3AgBweWg0A0BXAAJ42CYVDApOVUxMIEJsb2NrQc/nR5YLQHkMBEhhc2iXJgZ4EM5AeQwFSW5kZXiXJgZ4Fs5AeQwKTWVya2xlUm9vdJcmBngTzkB5DA1OZXh0Q29uc2Vuc3VzlyYGeBjOQHkMCFByZXZIYXNolyYGeBLOQHkMCVRpbWVzdGFtcJcmBngUzkB5DBFUcmFuc2FjdGlvbnNDb3VudJcmBngZzkB5DAdWZXJzaW9ulyYGeBHOQAwPVWtub3duIHByb3BlcnR5OlcBAng3AgBweWg1Mf///0BXAQJ4NwMAcHloNANAVwACeNgmEgwHTlVMTCBUeEHP50eWC0B5DARIYXNolyYGeBDOQHkMCk5ldHdvcmtGZWWXJgZ4Fc5AeQwFTm9uY2WXJgZ4Es5AeQwGU2NyaXB0lyYGeBfOQHkMBlNlbmRlcpcmBngTzkB5DAlTeXN0ZW1GZWWXJgZ4FM5AeQwPVmFsaWRVbnRpbEJsb2NrlyYGeBbOQHkMB1ZlcnNpb26XJgZ4Ec5AeQwHU2lnbmVyc5cmCXgQzjcEAEB5DApGaXJzdFNjb3BllyYNeBDONwQAEM4RzkAMD1Vrbm93biBwcm9wZXJ0eTpXAQN5eDcFAHB6aDUP////QFcBA3l4NwUAcHpoNf7+//9AVwECeDcGAHB5aDQDQFcAAnjYJhgMDU5VTEwgY29udHJhY3RBz+dHlgtAeQwCSWSXJgZ4EM5AeQwNVXBkYXRlQ291bnRlcpcmBngRzkB5DARIYXNolyYGeBLOQHkMCE1hbmlmZXN0lyYGeBTOQHkMA05lZpcmBngTzkAMD1Vrbm93biBwcm9wZXJ0eTpXAAF4NwcAQKsrR1s="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECeDcCAHB5aDQDQA==
    /// INITSLOT 0102 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0200 [32768 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getBlockByHash")]
    public abstract object? GetBlockByHash(UInt256? hash, string? whatReturn);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECeDcCAHB5aDUx////QA==
    /// INITSLOT 0102 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0200 [32768 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL_L 31FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getBlockByIndex")]
    public abstract object? GetBlockByIndex(BigInteger? index, string? whatReturn);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECeDcGAHB5aDQDQA==
    /// INITSLOT 0102 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0600 [32768 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getContract")]
    public abstract object? GetContract(UInt160? hash, string? whatReturn);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NwAAQA==
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getHeight")]
    public abstract BigInteger? GetHeight();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcBAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTransactionHeight")]
    public abstract BigInteger? GetTransactionHeight(UInt256? hash);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEDeXg3BQBwemg1D////0A=
    /// INITSLOT 0103 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0500 [32768 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL_L 0FFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTxByBlockHash")]
    public abstract object? GetTxByBlockHash(UInt256? blockHash, BigInteger? txIndex, string? whatReturn);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEDeXg3BQBwemg1/v7//0A=
    /// INITSLOT 0103 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0500 [32768 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL_L FEFEFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTxByBlockIndex")]
    public abstract object? GetTxByBlockIndex(BigInteger? blockIndex, BigInteger? txIndex, string? whatReturn);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECeDcDAHB5aDQDQA==
    /// INITSLOT 0102 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0300 [32768 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTxByHash")]
    public abstract object? GetTxByHash(UInt256? hash, string? whatReturn);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcHAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0700 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTxVMState")]
    public abstract BigInteger? GetTxVMState(UInt256? hash);

    #endregion
}
