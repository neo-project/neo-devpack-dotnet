using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Coverage.Formats;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractnep17
{
    [TestClass]
    public class CoverageContractTests
    {
        /// <summary>
        /// Required coverage to be success
        /// </summary>
        public static decimal RequiredCoverage { get; set; } = 1M;

        [AssemblyCleanup]
        public static void EnsureCoverage()
        {
            // Join here all of your coverage sources

            var coverage = Nep17ContractTests.Coverage;
            coverage?.Join(OwnerContractTests.Coverage);

            // Dump coverate to console

            Assert.IsNotNull(coverage, "Coverage can't be null");
            Console.WriteLine(coverage.Dump());

            // Write basic instruction html coverage

            File.WriteAllText("coverage.instruction.html", coverage.Dump(DumpFormat.Html));

            // Load our debug file

            if (NeoDebugInfo.TryLoad("templates/neocontractnep17/Artifacts/Nep17Contract.nefdbgnfo", out var dbg))
            {
                // Write the cobertura format

                File.WriteAllText("coverage.cobertura.xml", coverage.Dump(new CoberturaFormat((coverage, dbg))));

                // Write the report to the specific path

                CoverageReporting.CreateReport("coverage.cobertura.xml", "./coverageReport/");
            }

            // Ensure that the coverage is more than X% at the end of the tests

            Assert.IsTrue(coverage.CoveredLinesPercentage >= RequiredCoverage, $"Coverage is less than {RequiredCoverage:P2}");
        }
    }
}
