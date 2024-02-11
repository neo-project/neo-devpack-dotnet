using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class PolicyContract : Neo.SmartContract.Testing.SmartContract
{
    #region Properties
    public abstract BigInteger ExecFeeFactor { [DisplayName("getExecFeeFactor")] get; [DisplayName("setExecFeeFactor")] set; }
    public abstract BigInteger FeePerByte { [DisplayName("getFeePerByte")] get; [DisplayName("setFeePerByte")] set; }
    public abstract BigInteger StoragePrice { [DisplayName("getStoragePrice")] get; [DisplayName("setStoragePrice")] set; }
    #endregion
    #region Safe methods
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger getAttributeFee(BigInteger attributeType);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract bool isBlocked(UInt160 account);
    #endregion
    #region Unsafe methods
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract bool blockAccount(UInt160 account);
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract void setAttributeFee(BigInteger attributeType, BigInteger value);
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract bool unblockAccount(UInt160 account);
    #endregion
    #region Constructor for internal use only
    protected PolicyContract(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) {}
    #endregion
}