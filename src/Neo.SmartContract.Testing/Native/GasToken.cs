using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class GasToken : Neo.SmartContract.Testing.SmartContract
{
    #region Events
    public delegate void delTransfer(UInt160 from, UInt160 to, BigInteger amount);
    [DisplayName("Transfer")]
    public event delTransfer? OnTransfer;
    #endregion
    #region Properties
    public abstract BigInteger Decimals { [DisplayName("decimals")] get; }
    public abstract string Symbol { [DisplayName("symbol")] get; }
    public abstract BigInteger TotalSupply { [DisplayName("totalSupply")] get; }
    #endregion
    #region Safe methods
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("balanceOf")]
    public abstract BigInteger BalanceOf(UInt160 account);
    #endregion
    #region Unsafe methods
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data);
    #endregion
    #region Constructor for internal use only
    protected GasToken(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) { }
    #endregion
}
