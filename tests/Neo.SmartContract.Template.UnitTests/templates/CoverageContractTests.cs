using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Template.UnitTests.templates.neocontractnep17;
using Neo.SmartContract.Template.UnitTests.templates.neocontractowner;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Coverage.Formats;

namespace Neo.SmartContract.Template.UnitTests.templates
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

            var coverageNep17 = Nep17ContractTests.Coverage;
            coverageNep17?.Join(OwnerContractTests.Coverage);
            var coverageOwnable = OwnableContractTests.Coverage;
            var coverage = (coverageNep17 + coverageOwnable)!;

            // Dump coverage to console

            Assert.IsNotNull(coverageNep17, "NEP17 Coverage can't be null");
            Assert.IsNotNull(coverageOwnable, "Ownable Coverage can't be null");

            // Dump current coverage

            Console.WriteLine(coverage.Dump());
            File.WriteAllText("coverage.instruction.html", coverage.Dump(DumpFormat.Html));

            // Load debug file in order to generage source code coverage

            if (NeoDebugInfo.TryLoad("templates/neocontractnep17/Artifacts/Nep17Contract.nefdbgnfo", out var dbgNep17) &&
                NeoDebugInfo.TryLoad("templates/neocontractowner/Artifacts/Ownable.nefdbgnfo", out var dbgOwnable))
            {
                // Write the cobertura format

                File.WriteAllText("coverage.cobertura.xml", new CoberturaFormat(
                    (coverageNep17, dbgNep17),
                    (coverageOwnable, dbgOwnable)
                    ).Dump());

                // Write the report to the specific path

                CoverageReporting.CreateReport("coverage.cobertura.xml", "./coverageReport/");
            }

            // Ensure that the coverage is more than X% at the end of the tests

            Assert.IsTrue(coverage.CoveredLinesPercentage >= RequiredCoverage, $"Coverage is less than {RequiredCoverage:P2}");
        }
    }
}
