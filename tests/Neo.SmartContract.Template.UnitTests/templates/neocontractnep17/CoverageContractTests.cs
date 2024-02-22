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
            // Join here all of your Coverage sources

            var coverage = Nep17ContractTests.Coverage;
            coverage?.Join(OwnerContractTests.Coverage);

            // Ensure that the coverage is more than X% at the end of the tests

            Assert.IsNotNull(coverage);
            Console.WriteLine(coverage.Dump());

            File.WriteAllText("coverage.html", coverage.Dump(Testing.Coverage.DumpFormat.Html));
            Assert.IsTrue(coverage.CoveredLinesPercentage >= RequiredCoverage, $"Coverage is less than {RequiredCoverage:P2}");
        }
    }
}
