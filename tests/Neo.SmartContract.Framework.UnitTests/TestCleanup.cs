using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Extensions;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Akka.Util;
using Akka.Util.Internal;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class TestCleanup : TestCleanupBase
    {
        private static readonly Regex WhiteSpaceRegex = new("\\s");
        private static readonly object RootSync = new();

        [AssemblyCleanup]
        public static void EnsureCoverage() => EnsureCoverageInternal(Assembly.GetExecutingAssembly());

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
            var updatedArtifactNames = new ConcurrentSet<string>();
            Task.WhenAll(
                Enumerable.Range(0, Assembly.GetExecutingAssembly().GetTypes().Length).Select(i => Task.Run(() =>
                {
                    var type = Assembly.GetExecutingAssembly().GetTypes()[i];
                    if (!typeof(SmartContract.Testing.SmartContract).IsAssignableFrom(type)) return;

                    // Find result
                    CompilationContext? result;
                    lock (RootSync)
                    {
                        result = results.SingleOrDefault(u => u.ContractName == type.Name);
                        if (result == null) return;
                    }

                    // Ensure that it exists
                    var (debug, res) = CreateArtifact(result.ContractName!, result, root, Path.Combine(artifactsPath, $"{result.ContractName}.cs"));
                    if (debug != null)
                    {
                        lock (RootSync)
                        {
                            DebugInfos[type] = debug!;
                            results = results.Where(r => r != result).ToList();
                        }
                    }
                    else
                    {
                        updatedArtifactNames.TryAdd(res!);
                    }
                }))
            ).GetAwaiter().GetResult();

            if (updatedArtifactNames.Count != 0)
            {
                updatedArtifactNames.ForEach(p => Console.WriteLine($"Artifact {p} was updated"));
                Assert.Fail("There are artifacts being updated, please rerun the tests.");
            }
            // Ensure that all match

            if (results.Count > 0)
            {
                foreach (var result in results.Where(u => u.Success))
                {
                    CreateArtifact(result.ContractName!, result, root, Path.Combine(artifactsPath, $"{result.ContractName}.cs"));
                }

                Assert.Fail("Error compiling templates");
            }
        }

        private static (NeoDebugInfo?, string?) CreateArtifact(string typeName, CompilationContext context, string rootDebug, string artifactsPath)
        {
            var (nef, manifest, debugInfo) = context.CreateResults(rootDebug);
            var debug = NeoDebugInfo.FromDebugInfoJson(debugInfo);
            var artifact = manifest.GetArtifactsSource(typeName, nef, generateProperties: true);

            string writtenArtifact = File.Exists(artifactsPath) ? File.ReadAllText(artifactsPath) : "";
            if (string.IsNullOrEmpty(writtenArtifact) || WhiteSpaceRegex.Replace(artifact, "") != WhiteSpaceRegex.Replace(writtenArtifact, ""))
            {
                // Uncomment to overwrite the artifact file
                File.WriteAllText(artifactsPath, artifact);

                Console.Error.WriteLine($"{typeName} artifact was wrong");
                return (null, typeName);
            }

            return (debug, null);
        }
    }
}
