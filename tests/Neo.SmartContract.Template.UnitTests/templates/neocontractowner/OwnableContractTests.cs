using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.InvalidTypes;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractowner
{
    /// <summary>
    /// You need to build the solution to resolve Ownable class.
    /// </summary>
    [TestClass]
    public class OwnableContractTests : OwnableTests<Ownable>
    {
        /// <summary>
        /// Initialize Test
        /// </summary>
        public OwnableContractTests() : base(Ownable.Nef, Ownable.Manifest) { }

        [TestMethod]
        public override void TestSetGetOwner()
        {
            base.TestSetGetOwner();

            // Test throw if was stored an invalid owner
            // Technically not possible, but raise 100% coverage

            Contract.Storage.Put(new byte[] { 0xff }, 123);
            Assert.ThrowsException<VMUnhandledException>(() => Contract.Owner);
        }

        [TestMethod]
        public void TestMyMethod()
        {
            Assert.AreEqual("World", Contract.MyMethod());
        }

        [TestMethod]
        public void TestUpdate()
        {
            // Alice is the deployer

            Engine.SetTransactionSigners(Bob);

            Assert.ThrowsException<VMUnhandledException>(() => Contract.Update(NefFile.ToArray(), Manifest.ToJson().ToString()));

            Engine.SetTransactionSigners(Alice);

            // Test Update with the same script

            Contract.Update(NefFile.ToArray(), Manifest.ToJson().ToString());

            // Ensure that it works with the same script

            TestVerify();
        }

        [TestMethod]
        public void TestDeployWithOwner()
        {
            // Alice is the deployer

            Engine.SetTransactionSigners(Bob);

            // Try with invalid owners

            Assert.ThrowsException<Exception>(() => Engine.Deploy<Ownable>(NefFile, Manifest, UInt160.Zero));
            Assert.ThrowsException<Exception>(() => Engine.Deploy<Ownable>(NefFile, Manifest, InvalidUInt160.InvalidLength));
            Assert.ThrowsException<Exception>(() => Engine.Deploy<Ownable>(NefFile, Manifest, InvalidUInt160.InvalidType));

            // Test SetOwner notification

            UInt160? previousOwnerRaised = null;
            UInt160? newOwnerRaised = null;

            var expectedHash = Engine.GetDeployHash(NefFile, Manifest);
            var check = Engine.FromHash<Ownable>(expectedHash, false);
            check.OnSetOwner += (previous, newOwner) =>
            {
                previousOwnerRaised = previous;
                newOwnerRaised = newOwner;
            };

            // Deploy with random owner, we can use the same storage
            // because the contract hash contains the Sender, and now it's random

            var rand = TestEngine.GetNewSigner().Account;
            var nep17 = Engine.Deploy<Ownable>(NefFile, Manifest, rand);
            Assert.AreEqual(check.Hash, nep17.Hash);

            Coverage?.Join(nep17.GetCoverage());

            Assert.AreEqual(rand, nep17.Owner);
            Assert.IsNull(previousOwnerRaised);
            Assert.AreEqual(newOwnerRaised, nep17.Owner);
            Assert.AreEqual(newOwnerRaised, rand);
        }

        [TestMethod]
        public void TestDestroy()
        {
            // Try without being owner

            Engine.SetTransactionSigners(Bob);
            Assert.ThrowsException<VMUnhandledException>(Contract.Destroy);

            // Try with the owner

            var checkpoint = Engine.Checkpoint();

            Engine.SetTransactionSigners(Alice);
            Contract.Destroy();
            Assert.IsNull(Engine.Native.ContractManagement.GetContract(Contract));

            Engine.Restore(checkpoint);
        }
    }
}
