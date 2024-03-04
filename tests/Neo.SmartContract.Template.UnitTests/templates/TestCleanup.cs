using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract.Template.UnitTests.templates.neocontractnep17;
using Neo.SmartContract.Template.UnitTests.templates.neocontractoracle;
using Neo.SmartContract.Template.UnitTests.templates.neocontractowner;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Coverage.Formats;
using Neo.SmartContract.Testing.Extensions;
using System.Text.RegularExpressions;

namespace Neo.SmartContract.Template.UnitTests.templates
{
    [TestClass]
    public class TestCleanup
    {
        private static readonly Regex WhiteSpaceRegex = new("\\s");

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
            // Define paths

            string frameworkPath = Path.GetFullPath("../../../../../src/Neo.SmartContract.Framework/Neo.SmartContract.Framework.csproj");
            string templatePath = Path.GetFullPath("../../../../../src/Neo.SmartContract.Template/templates");
            string artifactsPath = Path.GetFullPath("../../../templates");

            // Compile

            var result = new CompilationEngine(new CompilationOptions()
            {
                Debug = true,
                CompilerVersion = "TestingEngine",
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable
            })
            .CompileSources(new CompilationSourceReferences() { Projects = new[] { frameworkPath } },
            new[] {
                    Path.Combine(templatePath, "neocontractnep17/Nep17Contract.cs"),
                    Path.Combine(templatePath, "neocontractoracle/OracleRequest.cs"),
                    Path.Combine(templatePath, "neocontractowner/Ownable.cs")
                });

            Assert.IsTrue(result.Count() == 3 && result.All(u => u.Success), "Error compiling templates");

            // Ensure Nep17

            var root = Path.GetPathRoot(templatePath) ?? "";
            (var artifact, DebugInfo_NEP17) = CreateArtifact<Nep17ContractTemplate>(result[0], root,
                Path.Combine(artifactsPath, "neocontractnep17/TestingArtifacts/Nep17ContractTemplate.artifacts.cs"));

            // Ensure Oracle

            (artifact, DebugInfo_Oracle) = CreateArtifact<OracleRequestTemplate>(result[1], root,
                Path.Combine(artifactsPath, "neocontractoracle/TestingArtifacts/OracleRequestTemplate.artifacts.cs"));

            // Ensure Ownable

            (artifact, DebugInfo_Ownable) = CreateArtifact<OwnableTemplate>(result[2], root,
                Path.Combine(artifactsPath, "neocontractowner/TestingArtifacts/OwnableTemplate.artifacts.cs"));
        }

        private static (string artifact, NeoDebugInfo debugInfo) CreateArtifact<T>(CompilationContext context, string rootDebug, string artifactsPath)
        {
            (var nef, var manifest, var debugInfo) = context.CreateResults(rootDebug);
            var debug = NeoDebugInfo.FromDebugInfoJson(debugInfo);
            var artifact = manifest.GetArtifactsSource(typeof(T).Name, nef, generateProperties: true);

            string writtenArtifact = File.Exists(artifactsPath) ? File.ReadAllText(artifactsPath) : "";
            if (string.IsNullOrEmpty(writtenArtifact) || WhiteSpaceRegex.Replace(artifact, "") != WhiteSpaceRegex.Replace(writtenArtifact, ""))
            {
                // Uncomment to overwrite the artifact file
                File.WriteAllText(artifactsPath, artifact);
                Assert.Fail($"{typeof(T).Name} artifact was wrong");
            }

            return (artifact, debug);
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

                // Merge coverlet json

                if (Environment.GetEnvironmentVariable("COVERAGE_MERGE_JOIN") is string mergeWith &&
                    !string.IsNullOrEmpty(mergeWith))
                {
                    new CoverletJsonFormat(
                       (coverageNep17, DebugInfo_NEP17),
                       (coverageOwnable, DebugInfo_Ownable),
                       (coverageOracle, DebugInfo_Oracle)
                       ).
                       Write(Environment.ExpandEnvironmentVariables(mergeWith), true);

                    Console.WriteLine($"Coverage merged with: {mergeWith}");
                }
            }

            // Ensure that the coverage is more than X% at the end of the tests

            Assert.IsTrue(coverage.CoveredLinesPercentage >= RequiredCoverage, $"Coverage is less than {RequiredCoverage:P2}");
        }
    }
}
