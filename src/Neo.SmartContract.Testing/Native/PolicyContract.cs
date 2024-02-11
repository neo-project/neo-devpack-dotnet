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
    [DisplayName("getAttributeFee")]
    public abstract BigInteger GetAttributeFee(BigInteger attributeType);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("isBlocked")]
    public abstract bool IsBlocked(UInt160 account);
    #endregion
    #region Unsafe methods
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("blockAccount")]
    public abstract bool BlockAccount(UInt160 account);
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("setAttributeFee")]
    public abstract void SetAttributeFee(BigInteger attributeType, BigInteger value);
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unblockAccount")]
    public abstract bool UnblockAccount(UInt160 account);
    #endregion
    #region Constructor for internal use only
    protected PolicyContract(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) {}
    #endregion
}