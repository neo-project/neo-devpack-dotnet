using Neo.SmartContract;
using Neo.SmartContract.Manifest;

namespace Example.SmartContract.Transfer.UnitTests
{
    public class TestCleanup
    {
        internal static (NefFile nef, ContractManifest manifest) EnsureArtifactsUpToDateInternal()
        {
            // Use the pre-built artifact from the SampleTransferContract class
            return (Neo.SmartContract.Testing.SampleTransferContract.Nef, Neo.SmartContract.Testing.SampleTransferContract.Manifest);
        }
    }
}
