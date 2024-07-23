using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Extensions;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class TestCleanup : TestCleanupBase
    {
        private static readonly Regex WhiteSpaceRegex = new("\\s");
        private static CompilationContext[]? compilationContexts;

        [AssemblyCleanup]
        public static void EnsureCoverage() => EnsureCoverageInternal(Assembly.GetExecutingAssembly(), 0.75M);

        [TestMethod]
        public void EnsureArtifactsUpToDate() => EnsureArtifactsUpToDateInternal();

        internal static CompilationContext[] EnsureArtifactsUpToDateInternal()
        {
            if (DebugInfos.Count > 0) return compilationContexts!; // Maybe a UT call it

            // Define paths

            var artifactsPath = new FileInfo("../../../TestingArtifacts").FullName;
            var testContractsPath = new FileInfo("../../../../Neo.Compiler.CSharp.TestContracts/Neo.Compiler.CSharp.TestContracts.csproj").FullName;
            var root = new FileInfo(testContractsPath).Directory?.Root.FullName ?? "";

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

            if (!results.Where(u => u.ContractName != "Contract_DuplicateNames").All(u => u.Success)) // TODO: Omit NotWorking better
            {
                results.SelectMany(u => u.Diagnostics)
                    .Where(u => u.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                    .ToList().ForEach(Console.Error.WriteLine);

                Assert.Fail("Error compiling templates");
            }

            // Get all artifacts loaded in this assembly

            compilationContexts = results.ToArray();
            var b = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in b)
            {
                if (typeof(SmartContract.Testing.SmartContract).IsAssignableFrom(type))
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

            return compilationContexts;
        }

        private static NeoDebugInfo CreateArtifact(string typeName, CompilationContext context, string rootDebug, string artifactsPath, bool failIfWrong)
        {
            (var nef, var manifest, var debugInfo) = context.CreateResults(rootDebug);
            var debug = NeoDebugInfo.FromDebugInfoJson(debugInfo);
            var artifact = manifest.GetArtifactsSource(typeName, nef, generateProperties: true);

            string writtenArtifact = File.Exists(artifactsPath) ? File.ReadAllText(artifactsPath) : "";
            if (string.IsNullOrEmpty(writtenArtifact) || WhiteSpaceRegex.Replace(artifact, "") != WhiteSpaceRegex.Replace(writtenArtifact, ""))
            {
                // Uncomment to overwrite the artifact file
                File.WriteAllText(artifactsPath, artifact);
                if (failIfWrong) Assert.Fail($"{typeName} artifact was wrong");
            }

            return debug;
        }
    }
}
