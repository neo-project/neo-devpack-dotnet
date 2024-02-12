using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Nep17Contract : Neo.SmartContract.Testing.SmartContract
{
    #region Events
    public delegate void delSetOwner(UInt160 newOwner);
    [DisplayName("SetOwner")]
    public event delSetOwner? OnSetOwner;
    public delegate void delTransfer(UInt160 from, UInt160 to, BigInteger amount);
    [DisplayName("Transfer")]
    public event delTransfer? OnTransfer;
    #endregion
    #region Properties
    public abstract BigInteger Decimals { [DisplayName("decimals")] get; }
    public abstract UInt160 Owner { [DisplayName("getOwner")] get; [DisplayName("setOwner")] set; }
    public abstract string Symbol { [DisplayName("symbol")] get; }
    public abstract BigInteger TotalSupply { [DisplayName("totalSupply")] get; }
    public abstract bool Verify { [DisplayName("verify")] get; }
    #endregion
    #region Safe methods
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("balanceOf")]
    public abstract BigInteger BalanceOf(UInt160 owner);
    #endregion
    #region Unsafe methods
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("burn")]
    public abstract void Burn(UInt160 account, BigInteger amount);
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("mint")]
    public abstract void Mint(UInt160 to, BigInteger amount);
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("myMethod")]
    public abstract string MyMethod();
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("onNEP17Payment")]
    public abstract void OnNEP17Payment(UInt160 from, BigInteger amount, object? data = null);
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object? data = null);
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[] nefFile, string manifest);
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("withdraw")]
    public abstract bool Withdraw(UInt160 token, UInt160 to, BigInteger amount);
    #endregion
    #region Constructor for internal use only
    protected Nep17Contract(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }
    #endregion
}
