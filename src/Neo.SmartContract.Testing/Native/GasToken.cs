using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class GasToken : Neo.SmartContract.Testing.SmartContract
{
    #region Events
    public delegate void delTransfer(UInt160 from, UInt160 to, BigInteger amount);
    public event delTransfer? Transfer;
    #endregion
    #region Safe methods
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger balanceOf(UInt160 account);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger decimals();
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract string symbol();
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger totalSupply();
    #endregion
    #region Unsafe methods
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract bool transfer(UInt160 from, UInt160 to, BigInteger amount, object data);
    #endregion
    #region Constructor for internal use only
    protected GasToken(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) {}
    #endregion
}