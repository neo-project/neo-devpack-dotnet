using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractnep17
{
    [TestClass]
    public class CoverageContractTests
    {
        /// <summary>
        /// Required coverage to be success
        /// </summary>
        public static float RequiredCoverage { get; set; } = 1F;

        [AssemblyCleanup]
        public static void EnsureCoverage()
        {
            // Ennsure that the coverage is more than X% at the end of the tests

            var coverage = Nep17ContractTests.Coverage;
            coverage?.Join(OwnerContractTests.Coverage);

            Assert.IsNotNull(coverage);

            Console.WriteLine(coverage.Dump());

            File.WriteAllText("coverage.html", coverage.Dump(Testing.Coverage.DumpFormat.Html));
            Assert.IsTrue(coverage.CoveredPercentage >= RequiredCoverage, $"Coverage is less than {RequiredCoverage:P2}");
        }
    }
}
