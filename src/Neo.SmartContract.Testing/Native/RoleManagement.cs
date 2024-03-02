using Neo.Cryptography.ECC;
using Neo.SmartContract.Native;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native;

public abstract class RoleManagement : SmartContract
{
    #region Compiled data

    public static readonly Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""RoleManagement"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""designateAsRole"",""parameters"":[{""name"":""role"",""type"":""Integer""},{""name"":""nodes"",""type"":""Array""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""getDesignatedByRole"",""parameters"":[{""name"":""role"",""type"":""Integer""},{""name"":""index"",""type"":""Integer""}],""returntype"":""Array"",""offset"":7,""safe"":true}],""events"":[{""name"":""Designation"",""parameters"":[{""name"":""Role"",""type"":""Integer""},{""name"":""BlockIndex"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":null}");

    #endregion

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
    public abstract ECPoint[] GetDesignatedByRole(Role role, uint index);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("designateAsRole")]
    public abstract void DesignateAsRole(Role role, ECPoint[] nodes);

    #endregion

    #region Constructor for internal use only

    protected RoleManagement(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
