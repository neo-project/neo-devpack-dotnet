using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class LedgerContract : Neo.SmartContract.Testing.SmartContract
{
    #region Properties
    public abstract UInt256 CurrentHash { [DisplayName("currentHash")] get; }
    public abstract BigInteger CurrentIndex { [DisplayName("currentIndex")] get; }
    #endregion
    #region Safe methods
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getBlock")]
    public abstract List<object> GetBlock(byte[] indexOrHash);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getTransaction")]
    public abstract List<object> GetTransaction(UInt256 hash);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getTransactionFromBlock")]
    public abstract List<object> GetTransactionFromBlock(byte[] blockIndexOrHash, BigInteger txIndex);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getTransactionHeight")]
    public abstract BigInteger GetTransactionHeight(UInt256 hash);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getTransactionSigners")]
    public abstract List<object> GetTransactionSigners(UInt256 hash);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getTransactionVMState")]
    public abstract BigInteger GetTransactionVMState(UInt256 hash);
    #endregion
    #region Constructor for internal use only
    protected LedgerContract(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) {}
    #endregion
}
