using System.ComponentModel;

namespace Neo.SmartContract.Testing.TestingStandards;

public interface IVerifiable
{
    /// <summary>
    /// Safe property
    /// </summary>
    public bool? Verify { [DisplayName("verify")] get; }
}
