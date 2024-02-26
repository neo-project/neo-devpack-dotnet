using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract.Template.UnitTests.templates.neocontractnep17;
using Neo.SmartContract.Template.UnitTests.templates.neocontractowner;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Coverage.Formats;
using Neo.SmartContract.Testing.Extensions;

namespace Neo.SmartContract.Template.UnitTests.templates
{
    [TestClass]
    public class TestCleanup
    {
        private static NeoDebugInfo? DebugInfo_NEP17;
        private static NeoDebugInfo? DebugInfo_Oracle;
        private static NeoDebugInfo? DebugInfo_Ownable;

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

            var result = new CompilationEngine(new CompilationOptions()
            {
                Debug = true,
                Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable
            })
            .CompileSources(
                Path.Combine(templatePath, "neocontractnep17/Nep17Contract.cs"),
                Path.Combine(templatePath, "neocontractoracle/OracleRequest.cs"),
                Path.Combine(templatePath, "neocontractowner/Ownable.cs")
                );

            Assert.IsTrue(result.Count() == 3 && result.All(u => u.Success), "Error compiling templates");

            // Ensure Nep17

            var content = File.ReadAllText(Path.Combine(artifactsPath, "neocontractnep17/TestingArtifacts/Nep17TemplateContract.artifacts.cs"));
            var artifact = CreateArtifact(result[0], "Nep17TemplateContract");
            DebugInfo_NEP17 = NeoDebugInfo.FromDebugInfoJson(result[0].CreateDebugInformation(content));
            Assert.AreEqual(artifact, content, "Nep17TemplateContract artifact was wrong");

            // Ensure Oracle

            content = File.ReadAllText(Path.Combine(artifactsPath, "neocontractoracle/TestingArtifacts/OracleRequest.artifacts.cs"));
            artifact = CreateArtifact(result[1]);
            DebugInfo_Oracle = NeoDebugInfo.FromDebugInfoJson(result[1].CreateDebugInformation(content));
            Assert.AreEqual(artifact, content, "OracleRequest artifact was wrong");

            // Ensure Ownable

            content = File.ReadAllText(Path.Combine(artifactsPath, "neocontractowner/TestingArtifacts/Ownable.artifacts.cs"));
            artifact = CreateArtifact(result[2]);
            DebugInfo_Ownable = NeoDebugInfo.FromDebugInfoJson(result[2].CreateDebugInformation(content));
            Assert.AreEqual(artifact, content, "Ownable artifact was wrong");
        }

        private static string CreateArtifact(CompilationContext context, string? name = null)
        {
            var manifest = context.CreateManifest();
            var nef = context.CreateExecutable();

            return manifest.GetArtifactsSource(name ?? manifest.Name, nef, null, generateProperties: true);
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

            if (DebugInfo_NEP17 is not null &&
                DebugInfo_Ownable is not null &&
                DebugInfo_Oracle is not null)
            {
                // Write the cobertura format

                File.WriteAllText("coverage.cobertura.xml", new CoberturaFormat(
                    (coverageNep17, DebugInfo_NEP17),
                    (coverageOwnable, DebugInfo_Ownable),
                    (coverageOracle, DebugInfo_Oracle)
                    ).Dump());

                // Write the report to the specific path

                CoverageReporting.CreateReport("coverage.cobertura.xml", "./coverageReport/");
            }

            // Ensure that the coverage is more than X% at the end of the tests

            Assert.IsTrue(coverage.CoveredLinesPercentage >= RequiredCoverage, $"Coverage is less than {RequiredCoverage:P2}");
        }
    }
}
