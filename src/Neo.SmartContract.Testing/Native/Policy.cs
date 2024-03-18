using Neo.SmartContract.Native;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native;

public abstract class Policy : SmartContract
{
    #region Compiled data

    public static Manifest.ContractManifest Manifest { get; } =
        NativeContract.Policy.GetContractState(ProtocolSettings.Default, uint.MaxValue).Manifest;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract uint ExecFeeFactor { [DisplayName("getExecFeeFactor")] get; [DisplayName("setExecFeeFactor")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract long FeePerByte { [DisplayName("getFeePerByte")] get; [DisplayName("setFeePerByte")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract uint StoragePrice { [DisplayName("getStoragePrice")] get; [DisplayName("setStoragePrice")] set; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getAttributeFee")]
    public abstract uint GetAttributeFee(byte attributeType);

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
    public abstract void SetAttributeFee(BigInteger attributeType, uint value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unblockAccount")]
    public abstract bool UnblockAccount(UInt160 account);

    #endregion

    #region Constructor for internal use only

    protected Policy(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
