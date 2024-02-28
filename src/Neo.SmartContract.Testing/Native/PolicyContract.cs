using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native;

public abstract class PolicyContract : SmartContract
{
    #region Compiled data

    public static readonly Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""PolicyContract"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""blockAccount"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""getAttributeFee"",""parameters"":[{""name"":""attributeType"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":7,""safe"":true},{""name"":""getExecFeeFactor"",""parameters"":[],""returntype"":""Integer"",""offset"":14,""safe"":true},{""name"":""getFeePerByte"",""parameters"":[],""returntype"":""Integer"",""offset"":21,""safe"":true},{""name"":""getStoragePrice"",""parameters"":[],""returntype"":""Integer"",""offset"":28,""safe"":true},{""name"":""isBlocked"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":35,""safe"":true},{""name"":""setAttributeFee"",""parameters"":[{""name"":""attributeType"",""type"":""Integer""},{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":42,""safe"":false},{""name"":""setExecFeeFactor"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":49,""safe"":false},{""name"":""setFeePerByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":56,""safe"":false},{""name"":""setStoragePrice"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":63,""safe"":false},{""name"":""unblockAccount"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":70,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":null}");

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? ExecFeeFactor { [DisplayName("getExecFeeFactor")] get; [DisplayName("setExecFeeFactor")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? FeePerByte { [DisplayName("getFeePerByte")] get; [DisplayName("setFeePerByte")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? StoragePrice { [DisplayName("getStoragePrice")] get; [DisplayName("setStoragePrice")] set; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getAttributeFee")]
    public abstract BigInteger? GetAttributeFee(BigInteger? attributeType);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("isBlocked")]
    public abstract bool? IsBlocked(UInt160? account);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("blockAccount")]
    public abstract bool? BlockAccount(UInt160? account);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("setAttributeFee")]
    public abstract void SetAttributeFee(BigInteger? attributeType, BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unblockAccount")]
    public abstract bool? UnblockAccount(UInt160? account);

    #endregion

    #region Constructor for internal use only

    protected PolicyContract(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
