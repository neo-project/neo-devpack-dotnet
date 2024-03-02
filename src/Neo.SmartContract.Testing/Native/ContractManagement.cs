using Neo.SmartContract.Iterators;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native;

public abstract class ContractManagement : SmartContract
{
    #region Compiled data

    public static readonly Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""ContractManagement"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""deploy"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""ByteArray""}],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""deploy"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Array"",""offset"":7,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":14,""safe"":false},{""name"":""getContract"",""parameters"":[{""name"":""hash"",""type"":""Hash160""}],""returntype"":""Array"",""offset"":21,""safe"":true},{""name"":""getContractById"",""parameters"":[{""name"":""id"",""type"":""Integer""}],""returntype"":""Array"",""offset"":28,""safe"":true},{""name"":""getContractHashes"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":35,""safe"":true},{""name"":""getMinimumDeploymentFee"",""parameters"":[],""returntype"":""Integer"",""offset"":42,""safe"":true},{""name"":""hasMethod"",""parameters"":[{""name"":""hash"",""type"":""Hash160""},{""name"":""method"",""type"":""String""},{""name"":""pcount"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":49,""safe"":true},{""name"":""setMinimumDeploymentFee"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":56,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":63,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":70,""safe"":false}],""events"":[{""name"":""Deploy"",""parameters"":[{""name"":""Hash"",""type"":""Hash160""}]},{""name"":""Update"",""parameters"":[{""name"":""Hash"",""type"":""Hash160""}]},{""name"":""Destroy"",""parameters"":[{""name"":""Hash"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":null}");

    #endregion

    #region Events

    public delegate void delDeploy(UInt160? Hash);

    [DisplayName("Deploy")]
    public event delDeploy? OnDeploy;

    public delegate void delDestroy(UInt160? Hash);

    [DisplayName("Destroy")]
    public event delDestroy? OnDestroy;

    public delegate void delUpdate(UInt160? Hash);

    [DisplayName("Update")]
    public event delUpdate? OnUpdate;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract IIterator? ContractHashes { [DisplayName("getContractHashes")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? MinimumDeploymentFee { [DisplayName("getMinimumDeploymentFee")] get; [DisplayName("setMinimumDeploymentFee")] set; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getContract")]
    public abstract ContractState? GetContract(UInt160? hash);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getContractById")]
    public abstract ContractState? GetContractById(BigInteger? id);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("hasMethod")]
    public abstract bool? HasMethod(UInt160? hash, string? method, BigInteger? pcount);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("deploy")]
    public abstract ContractState? Deploy(byte[]? nefFile, byte[]? manifest);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("deploy")]
    public abstract ContractState? Deploy(byte[]? nefFile, byte[]? manifest, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("destroy")]
    public abstract void Destroy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, byte[]? manifest);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, byte[]? manifest, object? data = null);

    #endregion

    #region Constructor for internal use only

    protected ContractManagement(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
