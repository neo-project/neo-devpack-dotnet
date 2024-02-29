using System.ComponentModel;
using System.Numerics;
using Neo.Network.P2P.Payloads;
using Neo.VM;

namespace Neo.SmartContract.Testing.Native;

public abstract class LedgerContract : SmartContract
{
    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract UInt256 CurrentHash { [DisplayName("currentHash")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger CurrentIndex { [DisplayName("currentIndex")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getBlock")]
    public abstract Models.Block? GetBlock(byte[]? indexOrHash);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getTransaction")]
    public abstract Models.Block? GetTransaction(UInt256? hash);

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
    public abstract VMState GetTransactionVMState(UInt256? hash);

    #endregion

    #region Constructor for internal use only

    protected LedgerContract(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
