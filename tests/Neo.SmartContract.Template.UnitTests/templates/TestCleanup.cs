using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract.Template.UnitTests.templates.neocontractnep17;
using Neo.SmartContract.Template.UnitTests.templates.neocontractowner;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Coverage.Formats;

namespace Neo.SmartContract.Template.UnitTests.templates
{
    [TestClass]
    public class TestCleanup
    {
        /// <summary>
        /// Required coverage to be success
        /// </summary>
        public static decimal RequiredCoverage { get; set; } = 1M;

        [TestMethod]
        public void EnsureArtifactsUpToDate()
        {
            string templatePath = Path.GetFullPath("../../../../../src/Neo.SmartContract.Template/templates");
            string artifactsPath = Path.GetFullPath("../../../templates");

            // Compile

            var result = new CompilationEngine(new Options()
            {
                Debug = true,
                Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable,
                GenerateArtifacts = Options.GenerateArtifactsKind.None
            })
            .CompileSources(
                Path.Combine(templatePath, "neocontractnep17/Nep17Contract.cs"),
                Path.Combine(templatePath, "neocontractoracle/OracleRequest.cs"),
                Path.Combine(templatePath, "neocontractowner/Ownable.cs")
                );

            Assert.IsTrue(result.Count() == 3 && result.All(u => u.Success), "Error compiling templates");

            // Ensure Nep17

            var content = File.ReadAllText(Path.Combine(artifactsPath, "neocontractnep17/TestingArtifacts/Nep17Contract.artifacts.cs"));
            // TODO: Compile source code and compare articats

            // Ensure Ownable

            content = File.ReadAllText(Path.Combine(artifactsPath, "neocontractowner/TestingArtifacts/Ownable.artifacts.cs"));
            // TODO: Compile source code and compare articats

            // Ensure Oracle

            content = File.ReadAllText(Path.Combine(artifactsPath, "neocontractoracle/TestingArtifacts/OracleRequest.artifacts.cs"));
            // TODO: Compile source code and compare articats
        }

        [AssemblyCleanup]
        public static void EnsureCoverage()
        {
            // Join here all of your coverage sources

            var coverageNep17 = Nep17ContractTests.Coverage;
            coverageNep17?.Join(OwnerContractTests.Coverage);
            var coverageOwnable = OwnableContractTests.Coverage;
            var coverageOracle = OracleRequestTests.Coverage;

            // Dump coverage to console

            Assert.IsNotNull(coverageNep17, "NEP17 coverage can't be null");
            Assert.IsNotNull(coverageOwnable, "Ownable coverage can't be null");
            Assert.IsNotNull(coverageOracle, "Oracle coverage can't be null");

            var coverage = coverageNep17 + coverageOwnable + coverageOracle;

            Assert.IsNotNull(coverage, "Coverage can't be null");

            // Dump current coverage

            Console.WriteLine(coverage.Dump(DumpFormat.Console));
            File.WriteAllText("coverage.instruction.html", coverage.Dump(DumpFormat.Html));

            // Write the cobertura format

            File.WriteAllText("coverage.cobertura.xml", new CoberturaFormat(
                (coverageNep17, Nep17TemplateContract.DebugInfo),
                (coverageOwnable, Ownable.DebugInfo),
                (coverageOracle, OracleRequest.DebugInfo)
                ).Dump());

            // Write the report to the specific path

            CoverageReporting.CreateReport("coverage.cobertura.xml", "./coverageReport/");

            // Ensure that the coverage is more than X% at the end of the tests

            Assert.IsTrue(coverage.CoveredLinesPercentage >= RequiredCoverage, $"Coverage is less than {RequiredCoverage:P2}");
        }
    }
}
