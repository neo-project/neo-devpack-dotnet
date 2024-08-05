using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
namespace Neo.Compiler.CSharp.UnitTests;

public class DebugAndTestBase<T> : TestBase<T>
    where T : SmartContract.Testing.SmartContract, IContractInfo
{
    static DebugAndTestBase()
    {
        TestCleanup.TestInitialize(typeof(T));
    }
}
