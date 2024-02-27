using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract.Template.UnitTests.templates.neocontractnep17;
using Neo.SmartContract.Template.UnitTests.templates.neocontractowner;
using Neo.SmartContract.Testing;
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
        public static decimal RequiredCoverage { get; set; } = 0.85M;

        [TestMethod]
        public void EnsureArtifactsUpToDate()
        {
            string frameworkPath = Path.GetFullPath("../../../../../src/Neo.SmartContract.Framework");
            string templatePath = Path.GetFullPath("../../../../../src/Neo.SmartContract.Template/templates");
            string artifactsPath = Path.GetFullPath("../../../templates");

            // Compile

            var result = new CompilationEngine(new CompilationOptions()
            {
                Debug = true,
                NoOptimize = false,
                Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Disable
            })
            .CompileSources(
                ("Neo.SmartContract.Framework", "3.6.2-CI00520"),
                Path.Combine(templatePath, "neocontractnep17/Nep17Contract.cs"),
                Path.Combine(templatePath, "neocontractoracle/OracleRequest.cs"),
                Path.Combine(templatePath, "neocontractowner/Ownable.cs")
                );

            Assert.IsTrue(result.Count() == 3 && result.All(u => u.Success), "Error compiling templates");

            // Ensure Nep17

            var root = Path.GetPathRoot(templatePath) ?? "";
            var content = File.ReadAllText(Path.Combine(artifactsPath, "neocontractnep17/TestingArtifacts/Nep17ContractTemplate.artifacts.cs"));
            (var artifact, DebugInfo_NEP17) = CreateArtifact<Nep17ContractTemplate>(result[0], root);
            Assert.AreEqual(artifact, content, "Nep17ContractTemplate artifact was wrong");

            // Ensure Oracle

            content = File.ReadAllText(Path.Combine(artifactsPath, "neocontractoracle/TestingArtifacts/OracleRequestTemplate.artifacts.cs"));
            (artifact, DebugInfo_Oracle) = CreateArtifact<OracleRequestTemplate>(result[1], root);
            Assert.AreEqual(artifact, content, "OracleRequestTemplate artifact was wrong");

            // Ensure Ownable

            content = File.ReadAllText(Path.Combine(artifactsPath, "neocontractowner/TestingArtifacts/OwnableTemplate.artifacts.cs"));
            (artifact, DebugInfo_Ownable) = CreateArtifact<OwnableTemplate>(result[2], root);
            Assert.AreEqual(artifact, content, "OwnableTemplate artifact was wrong");
        }

        private static (string, NeoDebugInfo) CreateArtifact<T>(CompilationContext context, string rootDebug)
        {
            (var nef, var manifest, var debugInfo) = context.CreateResults(rootDebug);
            var debug = NeoDebugInfo.FromDebugInfoJson(debugInfo);

            return (manifest.GetArtifactsSource(typeof(T).Name, nef, generateProperties: true), debug);
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

                // Join coverlet json

                new CoverletJsonFormat(
                   (coverageNep17, DebugInfo_NEP17),
                   (coverageOwnable, DebugInfo_Ownable),
                   (coverageOracle, DebugInfo_Oracle)
                   ).
                   Write("coverage.cobertura.json", true);
            }

            // Ensure that the coverage is more than X% at the end of the tests

            Assert.IsTrue(coverage.CoveredLinesPercentage >= RequiredCoverage, $"Coverage is less than {RequiredCoverage:P2}");
        }
    }
}
