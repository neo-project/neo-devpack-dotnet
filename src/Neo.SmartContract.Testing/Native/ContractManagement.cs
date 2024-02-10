using System.Collections.Generic;
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
    #region Safe methods
    public abstract List<object> getContract(UInt160 hash);
    public abstract List<object> getContractById(BigInteger id);
    public abstract object getContractHashes();
    public abstract BigInteger getMinimumDeploymentFee();
    public abstract bool hasMethod(UInt160 hash, string method, BigInteger pcount);
    #endregion
    #region Unsafe methods
    public abstract List<object> deploy(byte[] nefFile, byte[] manifest);
    public abstract List<object> deploy(byte[] nefFile, byte[] manifest, object data);
    public abstract void destroy();
    public abstract void setMinimumDeploymentFee(BigInteger value);
    public abstract void update(byte[] nefFile, byte[] manifest);
    public abstract void update(byte[] nefFile, byte[] manifest, object data);
    #endregion
    #region Constructor for internal use only
    protected ContractManagement(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) { }
    #endregion
}
