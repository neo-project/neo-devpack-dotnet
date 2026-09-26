using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.Modifier.UnitTests;

[TestClass]
public class ModifierTests : TestBase<SampleModifier>
{
    [TestMethod]
    public void TestRejectsCallWithoutOwnerWitness()
    {
        Engine.ClearTransactionSigners();

        Assert.ThrowsException<TestException>(() => Contract.Test());
    }
}
