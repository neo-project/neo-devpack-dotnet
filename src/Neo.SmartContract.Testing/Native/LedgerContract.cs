using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Native;
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

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getBlock")]
    public abstract TrimmedBlock? GetBlock(byte[]? indexOrHash);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getTransaction")]
    public abstract Transaction? GetTransaction(UInt256? hash);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getTransactionFromBlock")]
    public abstract Transaction? GetTransactionFromBlock(byte[]? blockIndexOrHash, BigInteger? txIndex);

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
