using Neo.SmartContract.Native;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native;

public abstract class GAS : SmartContract, TestingStandards.INep17Standard
{
    #region Compiled data

    public static Manifest.ContractManifest Manifest { get; } =
        NativeContract.GAS.GetContractState(ProtocolSettings.Default, uint.MaxValue).Manifest;

    #endregion

    #region Events
#pragma warning disable CS0067 // Event is never used
    [DisplayName("Transfer")]
    public event TestingStandards.INep17Standard.delTransfer? OnTransfer;
#pragma warning restore CS0067 // Event is never used
    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? Decimals { [DisplayName("decimals")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? Symbol { [DisplayName("symbol")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? TotalSupply { [DisplayName("totalSupply")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? account);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    #endregion

    #region Constructor for internal use only

    protected GAS(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
