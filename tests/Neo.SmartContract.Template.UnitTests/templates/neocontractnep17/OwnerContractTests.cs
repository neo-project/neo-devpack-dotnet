using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractnep17
{
    /// <summary>
    /// You need to build the solution to resolve Nep17Contract class.
    /// </summary>
    [TestClass]
    public class OwnerContractTests : OwnableTests<Nep17TemplateContract>
    {
        /// <summary>
        /// Initialize Test
        /// </summary>
        public OwnerContractTests() : base(Nep17TemplateContract.Nef, Nep17TemplateContract.Manifest) { }

        [TestMethod]
        public override void TestSetGetOwner()
        {
            base.TestSetGetOwner();

            // Test throw if was stored an invalid owner
            // Technically not possible, but raise 100% coverage

            Contract.Storage.Put(new byte[] { 0xff }, 123);
            Assert.ThrowsException<VMUnhandledException>(() => Contract.Owner);
        }
    }
}
