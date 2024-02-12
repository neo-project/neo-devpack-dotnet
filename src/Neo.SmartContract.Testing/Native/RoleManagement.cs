using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class RoleManagement : Neo.SmartContract.Testing.SmartContract
{
    #region Events
    public delegate void delDesignation(BigInteger Role, BigInteger BlockIndex);
    [DisplayName("Designation")]
    public event delDesignation? OnDesignation;
    #endregion
    #region Safe methods
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getDesignatedByRole")]
    public abstract IList<object> GetDesignatedByRole(BigInteger role, BigInteger index);
    #endregion
    #region Unsafe methods
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("designateAsRole")]
    public abstract void DesignateAsRole(BigInteger role, IList<object> nodes);
    #endregion
    #region Constructor for internal use only
    protected RoleManagement(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }
    #endregion
}
