using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Coverage.Formats;
using Neo.SmartContract.Testing.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Neo.SmartContract.Template.UnitTests.templates
{
    [TestClass]
    public class TestCleanup
    {
        internal static readonly Dictionary<Type, NeoDebugInfo> DebugInfos = new();

        /// <summary>
        /// Required coverage to be success
        /// </summary>
        public static decimal RequiredCoverage { get; set; } = 0.85M;

        [TestMethod]
        public void EnsureArtifactsUpToDate() => EnsureArtifactsUpToDateInternal();

        internal static void EnsureArtifactsUpToDateInternal()
        {
            if (DebugInfos.Count > 0) return; // Maybe a UT call it

            // Define paths

            string testContractsPath = Path.GetFullPath("../../../../Neo.SmartContract.Framework.TestContracts/Neo.SmartContract.Framework.TestContracts.csproj");
            string artifactsPath = Path.GetFullPath("../../../TestingArtifacts");
            var root = Path.GetPathRoot(testContractsPath) ?? "";

            // Compile

            var results = new CompilationEngine(new CompilationOptions()
            {
                Debug = true,
                CompilerVersion = "TestingEngine",
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable
            })
            .CompileProject(testContractsPath);

            // Ensure that all was well compiled

            if (!results.All(u => u.Success))
            {
                results.SelectMany(u => u.Diagnostics)
                    .Where(u => u.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                    .ToList().ForEach(Console.Error.WriteLine);

                Assert.Fail("Error compiling templates");
            }

            // Get all artifacts loaded in this assembly

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(Testing.SmartContract).IsAssignableFrom(type))
                {
                    // Find result

                    var result = results.Where(u => u.ContractName == type.Name).SingleOrDefault();
                    if (result == null) continue;

                    // Ensure that it exists

                    DebugInfos[type] = CreateArtifact(result.ContractName!, result, root, Path.Combine(artifactsPath, $"{result.ContractName}.cs"), true);
                    results.Remove(result);
                }
            }

            // Ensure that all match

            if (results.Count() > 0)
            {
                foreach (var result in results.Where(u => u.Success))
                {
                    CreateArtifact(result.ContractName!, result, root, Path.Combine(artifactsPath, $"{result.ContractName}.cs"), false);
                }

                Assert.Fail("Error compiling templates");
            }
        }

        private static NeoDebugInfo CreateArtifact(string typeName, CompilationContext context, string rootDebug, string artifactsPath, bool failIfWrong)
        {
            (var nef, var manifest, var debugInfo) = context.CreateResults(rootDebug);
            var debug = NeoDebugInfo.FromDebugInfoJson(debugInfo);
            var artifact = manifest.GetArtifactsSource(typeName, nef, generateProperties: true);

            string writtenArtifact = File.Exists(artifactsPath) ? File.ReadAllText(artifactsPath) : "";
            if (writtenArtifact == "" || WhiteSpaceRegex().Replace(artifact, "") != WhiteSpaceRegex().Replace(writtenArtifact, ""))
            {
                // Uncomment to overwrite the artifact file
                File.WriteAllText(artifactsPath, artifact);
                if (failIfWrong) Assert.Fail($"{typeName} artifact was wrong");
            }

            return debug;
        }

        [AssemblyCleanup]
        public static void EnsureCoverage()
        {
            // Join here all of your coverage sources

            CoverageBase? coverage = null;
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();
            var list = new List<(CoveredContract Contract, NeoDebugInfo DebugInfo)>();

            foreach (var infos in DebugInfos)
            {
                Type type = typeof(Testing.TestingStandards.TestBase<>).MakeGenericType(infos.Key);
                CoveredContract? cov = null;

                foreach (var aType in allTypes)
                {
                    if (type.IsAssignableFrom(aType))
                    {
                        cov = type.GetProperty("Coverage")!.GetValue(null) as CoveredContract;
                        Assert.IsNotNull(cov, $"{infos.Key} coverage can't be null");

                        // It doesn't require join, because we have only one UnitTest class per contract

                        coverage += cov;
                        list.Add((cov, infos.Value));
                        break;
                    }
                }

                if (cov is null)
                {
                    Console.Error.WriteLine($"Coverage not found for {infos.Key}");
                }
            }

            // Ensure we have coverage

            Assert.IsNotNull(coverage, $"Coverage can't be null");

            // Dump current coverage

            Console.WriteLine(coverage.Dump(DumpFormat.Console));
            File.WriteAllText("coverage.instruction.html", coverage.Dump(DumpFormat.Html));

            // Write the cobertura format

            File.WriteAllText("coverage.cobertura.xml", new CoberturaFormat(list.ToArray()).Dump());

            // Write the report to the specific path

            CoverageReporting.CreateReport("coverage.cobertura.xml", "./coverageReport/");

            // Merge coverlet json

            if (Environment.GetEnvironmentVariable("COVERAGE_MERGE_JOIN") is string mergeWith &&
                !string.IsNullOrEmpty(mergeWith))
            {
                new CoverletJsonFormat(list.ToArray()).Write(Environment.ExpandEnvironmentVariables(mergeWith), true);

                Console.WriteLine($"Coverage merged with: {mergeWith}");
            }

            // Ensure that the coverage is more than X% at the end of the tests

            Assert.IsTrue(coverage.CoveredLinesPercentage >= RequiredCoverage, $"Coverage is less than {RequiredCoverage:P2}");
        }

        [GeneratedRegex(@"\s")]
        private static partial Regex WhiteSpaceRegex();
    }
}
