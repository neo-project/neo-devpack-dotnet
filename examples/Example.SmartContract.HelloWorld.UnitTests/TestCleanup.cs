using Neo.SmartContract;
using Neo.SmartContract.Manifest;

namespace Example.SmartContract.HelloWorld.UnitTests
{
    public class TestCleanup
    {
        internal static (NefFile nef, ContractManifest manifest) EnsureArtifactsUpToDateInternal()
        {
            // Use the pre-built artifact from the SampleHelloWorld class
            return (Neo.SmartContract.Testing.SampleHelloWorld.Nef, Neo.SmartContract.Testing.SampleHelloWorld.Manifest);
        }
    }
}
