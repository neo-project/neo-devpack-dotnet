using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Coverage.Formats;
using Neo.SmartContract.Testing.Coverage.Reports;

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
            // Join here all of your Coverage sources

            var coverage = Nep17ContractTests.Coverage;
            coverage?.Join(OwnerContractTests.Coverage);

            // Ensure that the coverage is more than X% at the end of the tests

            Assert.IsNotNull(coverage);
            Console.WriteLine(coverage.Dump());

            File.WriteAllText("instruction-coverage.html", coverage.Dump(DumpFormat.Html));

            if (NeoDebugInfo.TryLoad("templates/neocontractnep17/Artifacts/Nep17Contract.nefdbgnfo", out var dbg))
            {
                File.WriteAllText("coverage.cobertura.xml", coverage.Dump(new CoberturaFormat((coverage, dbg))));
                Report.CreateReport("coverage.cobertura.xml", "./coverageReport/");
            }

            Assert.IsTrue(coverage.CoveredLinesPercentage >= RequiredCoverage, $"Coverage is less than {RequiredCoverage:P2}");
        }
    }
}
