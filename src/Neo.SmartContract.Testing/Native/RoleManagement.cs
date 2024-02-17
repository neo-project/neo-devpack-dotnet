using Neo.Cryptography.ECC;
using Neo.SmartContract.Native;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class RoleManagement : SmartContract
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
    public abstract ECPoint[] GetDesignatedByRole(BigInteger? role, BigInteger? index);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("designateAsRole")]
    public abstract void DesignateAsRole(Role? role, ECPoint[]? nodes);

    #endregion

    #region Constructor for internal use only

    protected RoleManagement(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
