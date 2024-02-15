using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractnep17
{
    /// <summary>
    /// You need to build the solution to resolve Nep17Contract class.
    /// </summary>
    [TestClass]
    public class OwnerContractTests : OwnableTests<Nep17Contract>
    {
        /// <summary>
        /// Initialize Test
        /// </summary>
        public OwnerContractTests() :
            base(
                "templates/neocontractnep17/Artifacts/Nep17Contract.nef",
                "templates/neocontractnep17/Artifacts/Nep17Contract.manifest.json"
                )
        { }
    }
}
