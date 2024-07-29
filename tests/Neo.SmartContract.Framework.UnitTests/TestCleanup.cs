using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Extensions;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Neo.SmartContract.Testing;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class TestCleanup : TestCleanupBase
    {
        private static readonly Regex WhiteSpaceRegex = new("\\s");
        private static bool _debugContract = false;

        [AssemblyCleanup]
        public static void EnsureCoverage()
        {
            if (!_debugContract)
            {
                EnsureCoverageInternal(Assembly.GetExecutingAssembly(), 0.75M);
            }
        }

        [TestMethod]
        public void EnsureArtifactsUpToDate() => EnsureArtifactsUpToDateInternal();

        internal static void EnsureArtifactsUpToDateInternal()
        {
            if (DebugInfos.Count > 0) return; // Maybe a UT call it

            // Define paths

            var testContractsPath = new FileInfo("../../../../Neo.SmartContract.Framework.TestContracts/Neo.SmartContract.Framework.TestContracts.csproj").FullName;
            var artifactsPath = new FileInfo("../../../TestingArtifacts").FullName;
            var root = new FileInfo(testContractsPath).FullName ?? "";

            // Compile

            // Compile
            var compilationEngine = new CompilationEngine(new CompilationOptions()
            {
                Debug = true,
                CompilerVersion = "TestingEngine",
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable
            });

            // If you want to debug the compilation of a contact,
            // call the SetDebugContract to set the contract to be debugged, effect only under debug mode.
            // For a better debugging experience, you can comment out the `EnsureCoverage` method.
            // SetDebugContract(compilationEngine, nameof(Contract_Attribute));

            var results = compilationEngine.CompileProject(testContractsPath);

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
            var (nef, manifest, debugInfo) = context.CreateResults(rootDebug);
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

        private static void SetDebugContract(CompilationEngine engine, string contractName)
        {
            _debugContract = true;
            engine.SetDebugContract(contractName);
        }
    }
}
