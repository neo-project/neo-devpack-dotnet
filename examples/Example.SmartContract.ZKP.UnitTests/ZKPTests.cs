using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.ZKP.UnitTests;

[TestClass]
public class ZKPTests : TestBase<SampleZKP>
{
    [TestMethod]
    public void VeifyRejectsIncorrectPublicInputCount()
    {
        Assert.ThrowsException<TestException>(() =>
            Contract.Veify([], [], [], []));
    }
}
