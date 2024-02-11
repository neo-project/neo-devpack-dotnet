using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class ContractManagement : Neo.SmartContract.Testing.SmartContract
{
    #region Events
    public delegate void delDeploy(UInt160 Hash);
    public event delDeploy? Deploy;
    public delegate void delDestroy(UInt160 Hash);
    public event delDestroy? Destroy;
    public delegate void delUpdate(UInt160 Hash);
    public event delUpdate? Update;
    #endregion
    #region Properties
    public abstract object ContractHashes { [DisplayName("getContractHashes")] get; }
    public abstract BigInteger MinimumDeploymentFee { [DisplayName("getMinimumDeploymentFee")] get; [DisplayName("setMinimumDeploymentFee")] set; }
    #endregion
    #region Safe methods
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract ContractState getContract(UInt160 hash);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract ContractState getContractById(BigInteger id);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract bool hasMethod(UInt160 hash, string method, BigInteger pcount);
    #endregion
    #region Unsafe methods
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract ContractState deploy(byte[] nefFile, byte[] manifest);
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract ContractState deploy(byte[] nefFile, byte[] manifest, object data);
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract void destroy();
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract void update(byte[] nefFile, byte[] manifest);
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract void update(byte[] nefFile, byte[] manifest, object data);
    #endregion
    #region Constructor for internal use only
    protected ContractManagement(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) { }
    #endregion
}
