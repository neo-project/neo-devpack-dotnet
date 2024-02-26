using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.TestingStandards;

public interface INep17Standard
{
    #region Events

    public delegate void delTransfer(UInt160? from, UInt160? to, BigInteger? amount);

    [DisplayName("Transfer")]
    public event delTransfer? OnTransfer;

    #endregion

    #region Properties

    /// <summary>
    /// Safe method
    /// </summary>
    public string? Symbol { [DisplayName("symbol")] get; }

    /// <summary>
    /// Safe method
    /// </summary>
    public BigInteger? Decimals { [DisplayName("decimals")] get; }

    /// <summary>
    /// Safe method
    /// </summary>
    public BigInteger? TotalSupply { [DisplayName("totalSupply")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("balanceOf")]
    public BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    #endregion
}
