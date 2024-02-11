using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class ContractManagement : Neo.SmartContract.Testing.SmartContract
{
    #region Events
    public delegate void delDeploy(UInt160 Hash);
    [DisplayName("Deploy")]
    public event delDeploy? OnDeploy;
    public delegate void delDestroy(UInt160 Hash);
    [DisplayName("Destroy")]
    public event delDestroy? OnDestroy;
    public delegate void delUpdate(UInt160 Hash);
    [DisplayName("Update")]
    public event delUpdate? OnUpdate;
    #endregion
    #region Properties
    public abstract object ContractHashes { [DisplayName("getContractHashes")] get; }
    public abstract BigInteger MinimumDeploymentFee { [DisplayName("getMinimumDeploymentFee")] get; [DisplayName("setMinimumDeploymentFee")] set; }
    #endregion
    #region Safe methods
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getContract")]
    public abstract ContractState GetContract(UInt160 hash);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getContractById")]
    public abstract ContractState GetContractById(BigInteger id);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("hasMethod")]
    public abstract bool HasMethod(UInt160 hash, string method, BigInteger pcount);
    #endregion
    #region Unsafe methods
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("deploy")]
    public abstract ContractState Deploy(byte[] nefFile, byte[] manifest);
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("deploy")]
    public abstract ContractState Deploy(byte[] nefFile, byte[] manifest, object data);
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("destroy")]
    public abstract void Destroy();
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[] nefFile, byte[] manifest);
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[] nefFile, byte[] manifest, object data);
    #endregion
    #region Constructor for internal use only
    protected ContractManagement(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }
    #endregion
}
