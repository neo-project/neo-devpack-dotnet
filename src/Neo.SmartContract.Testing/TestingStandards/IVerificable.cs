using System.ComponentModel;

namespace Neo.SmartContract.Testing.TestingStandards;

public interface IVerificable
{
    /// <summary>
    /// Safe property
    /// </summary>
    public bool? Verify { [DisplayName("verify")] get; }
}
