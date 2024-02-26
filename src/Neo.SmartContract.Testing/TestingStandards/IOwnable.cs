using System.ComponentModel;

namespace Neo.SmartContract.Testing.TestingStandards;

public interface IOwnable
{
    #region Events

    public delegate void delSetOwner(UInt160? previousOwner, UInt160? newOwner);

    [DisplayName("SetOwner")]
    public event delSetOwner? OnSetOwner;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public UInt160? Owner { [DisplayName("getOwner")] get; [DisplayName("setOwner")] set; }

    #endregion
}
