using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using ContractArtifact = Neo.SmartContract.Testing.Contract;

namespace NeoContractSolution.UnitTests
{
    [TestClass]
    public class SmartContractTests : OwnableTests<ContractArtifact>
    {
        public SmartContractTests() : base(ContractArtifact.Nef, ContractArtifact.Manifest) { }

        [TestMethod]
        public void TestMyMethod()
        {
            Assert.AreEqual("World", Contract.MyMethod());
        }

        [TestMethod]
        public void TestUpdate()
        {
            Engine.SetTransactionSigners(Bob);

            Assert.ThrowsException<TestException>(() => Contract.Update(NefFile.ToArray(), Manifest.ToJson().ToString()));

            Engine.SetTransactionSigners(Alice);
            Contract.Update(NefFile.ToArray(), Manifest.ToJson().ToString());

            TestVerify();
        }
    }
}
