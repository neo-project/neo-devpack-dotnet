using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Native;
using Neo.VM;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native;

public abstract class Ledger : SmartContract
{
    #region Compiled data

    public static Manifest.ContractManifest Manifest { get; } =
        NativeContract.Ledger.GetContractState(ProtocolSettings.Default, uint.MaxValue).Manifest;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract UInt256 CurrentHash { [DisplayName("currentHash")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract uint CurrentIndex { [DisplayName("currentIndex")] get; }

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
    public Models.Transaction? GetTransactionFromBlock(uint blockIndex, int txIndex)
        => GetTransactionFromBlock(new BigInteger(blockIndex).ToByteArray(), txIndex);

    /// <summary>
    /// Safe helper method
    /// </summary>
    public Models.Transaction? GetTransactionFromBlock(UInt256 blockHash, int txIndex)
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
    public abstract Models.Transaction? GetTransaction(UInt256 hash);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getTransactionFromBlock")]
    public abstract Models.Transaction? GetTransactionFromBlock(byte[] blockIndexOrHash, int txIndex);

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
    public abstract VMState GetTransactionVMState(UInt256 hash);

    #endregion

    #region Constructor for internal use only

    protected Ledger(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
