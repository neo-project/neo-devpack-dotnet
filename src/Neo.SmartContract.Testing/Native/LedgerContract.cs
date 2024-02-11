using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class LedgerContract : Neo.SmartContract.Testing.SmartContract
{
    #region Properties
    public abstract UInt256 currentHash { [DisplayName("currentHash")] get; }
    public abstract BigInteger currentIndex { [DisplayName("currentIndex")] get; }
    #endregion
    #region Safe methods
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract List<object> getBlock(byte[] indexOrHash);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract List<object> getTransaction(UInt256 hash);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract List<object> getTransactionFromBlock(byte[] blockIndexOrHash, BigInteger txIndex);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger getTransactionHeight(UInt256 hash);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract List<object> getTransactionSigners(UInt256 hash);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger getTransactionVMState(UInt256 hash);
    #endregion
    #region Constructor for internal use only
    protected LedgerContract(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) {}
    #endregion
}