using Neo.SmartContract;
using Neo.SmartContract.Manifest;

namespace Example.SmartContract.FaunFeatures.UnitTests
{
    public class TestCleanup
    {
        internal static (NefFile nef, ContractManifest manifest) EnsureArtifactsUpToDateInternal()
        {
            return (Neo.SmartContract.Testing.SampleFaunFeatures.Nef, Neo.SmartContract.Testing.SampleFaunFeatures.Manifest);
        }
    }
}
