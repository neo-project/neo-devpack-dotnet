using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.SmartContract.Framework.UnitTests;

public class TestBase2<T> : TestBase<T>
    where T : SmartContract.Testing.SmartContract
{
    public TestBase2(NefFile nef, ContractManifest manifest) : base(nef, manifest)
    {
        TestCleanup.TestInitialize(typeof(T));
    }
}
