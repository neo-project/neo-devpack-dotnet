using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.VM;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native;

public abstract class LedgerContract : SmartContract
{
    #region Compiled data

    public static readonly Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""LedgerContract"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""currentHash"",""parameters"":[],""returntype"":""Hash256"",""offset"":0,""safe"":true},{""name"":""currentIndex"",""parameters"":[],""returntype"":""Integer"",""offset"":7,""safe"":true},{""name"":""getBlock"",""parameters"":[{""name"":""indexOrHash"",""type"":""ByteArray""}],""returntype"":""Array"",""offset"":14,""safe"":true},{""name"":""getTransaction"",""parameters"":[{""name"":""hash"",""type"":""Hash256""}],""returntype"":""Array"",""offset"":21,""safe"":true},{""name"":""getTransactionFromBlock"",""parameters"":[{""name"":""blockIndexOrHash"",""type"":""ByteArray""},{""name"":""txIndex"",""type"":""Integer""}],""returntype"":""Array"",""offset"":28,""safe"":true},{""name"":""getTransactionHeight"",""parameters"":[{""name"":""hash"",""type"":""Hash256""}],""returntype"":""Integer"",""offset"":35,""safe"":true},{""name"":""getTransactionSigners"",""parameters"":[{""name"":""hash"",""type"":""Hash256""}],""returntype"":""Array"",""offset"":42,""safe"":true},{""name"":""getTransactionVMState"",""parameters"":[{""name"":""hash"",""type"":""Hash256""}],""returntype"":""Integer"",""offset"":49,""safe"":true}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":null}");

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract UInt256? CurrentHash { [DisplayName("currentHash")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? CurrentIndex { [DisplayName("currentIndex")] get; }

    #endregion

    #region Safe methods

    #region Helpers

    /// <summary>
    /// Safe helper method
    /// </summary>
    public Models.Block? GetBlock(UInt256 hash)
        => GetBlock(hash.ToArray());

    /// <summary>
    /// Safe helper method
    /// </summary>
    public Models.Block? GetBlock(uint index)
        => GetBlock(new BigInteger(index).ToByteArray());

    /// <summary>
    /// Safe helper method
    /// </summary>
    public Models.Transaction? GetTransactionFromBlock(uint blockIndex, uint txIndex)
        => GetTransactionFromBlock(new BigInteger(blockIndex).ToByteArray(), txIndex);

    /// <summary>
    /// Safe helper method
    /// </summary>
    public Models.Transaction? GetTransactionFromBlock(UInt256 blockHash, uint txIndex)
        => GetTransactionFromBlock(blockHash.ToArray(), txIndex);

    #endregion

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getBlock")]
    public abstract Models.Block? GetBlock(byte[]? indexOrHash);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getTransaction")]
    public abstract Models.Transaction? GetTransaction(UInt256? hash);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getTransactionFromBlock")]
    public abstract Models.Transaction? GetTransactionFromBlock(byte[]? blockIndexOrHash, BigInteger? txIndex);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getTransactionHeight")]
    public abstract BigInteger? GetTransactionHeight(UInt256? hash);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getTransactionSigners")]
    public abstract Signer[]? GetTransactionSigners(UInt256? hash);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getTransactionVMState")]
    public abstract VMState? GetTransactionVMState(UInt256? hash);

    #endregion

    #region Constructor for internal use only

    protected LedgerContract(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
