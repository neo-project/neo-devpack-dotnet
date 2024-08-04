using Akka.Util;
using Akka.Util.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Extensions;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class TestCleanup : TestCleanupBase
    {
        private static readonly Regex WhiteSpaceRegex = new("\\s");
        private static CompilationContext[]? compilationContexts;

        private static readonly string ArtifactsPath = new FileInfo("../../../TestingArtifacts").FullName;
        private static readonly string TestContractsPath = new FileInfo("../../../../Neo.Compiler.CSharp.TestContracts/Neo.Compiler.CSharp.TestContracts.csproj").FullName;
        private static bool _isAssemblyInitialized = false;
        private static CompilationEngine? _compilationEngine = null;

        private static List<INamedTypeSymbol> _sortedClasses;
        private static Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> _classDependencies;
        private static List<INamedTypeSymbol?> _allClassSymbols;
        private static readonly ConcurrentSet<string> UpdatedArtifactNames = [];
        private static int TestCount = 0;

        [AssemblyInitialize]
        public static void TestAssemblyInitialize(TestContext testContext)
        {
            if (_isAssemblyInitialized) return;
            // Compile
            _compilationEngine = new CompilationEngine(new CompilationOptions
            {
                Debug = true,
                CompilerVersion = "TestingEngine",
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = NullableContextOptions.Enable
            });

            (_sortedClasses, _classDependencies, _allClassSymbols) = _compilationEngine.PrepareProjectContracts(TestContractsPath);
            _isAssemblyInitialized = true;
        }

        public static void TestInitialize(Type contract)
        {
            try
            {
                if (!typeof(SmartContract.Testing.SmartContract).IsAssignableFrom(contract))
                {
                    throw new InvalidOperationException(
                        $"The type {contract.Name} does not inherit from SmartContract.Testing.SmartContract");
                }
                TestCount++;
                // try to compile the contract first.
                EnsureArtifactUpToDateInternal(contract.Name);
            }
            catch (Exception e)
            {
                Assert.Fail("Error compiling contract " + contract.Name + ": " + e.Message);
            }
        }

        [AssemblyCleanup]
        public static void EnsureCoverage()
        {
            if (UpdatedArtifactNames.Count > 0)
                Assert.Fail("Some artifacts were updated. Please rerun the tests.");

            if (TestCount > 1)
                EnsureCoverageInternal(Assembly.GetExecutingAssembly(), 0.77M);
        }

        internal static CompilationContext[] EnsureArtifactUpToDateInternal(string singleContractName)
        {
            var root = new FileInfo(TestContractsPath).Directory?.Root.FullName ?? "";

            // Ensure that all was well compiled
            var result = _compilationEngine.CompileProject(TestContractsPath, _sortedClasses, _classDependencies, _allClassSymbols, singleContractName).FirstOrDefault();
            if (result.ContractName != "Contract_DuplicateNames" && !result.Success)
            {
                result.Diagnostics.Where(u => u.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                    .ToList().ForEach(Console.Error.WriteLine);
                Assert.Fail("Error compiling contract " + result.ContractName);
            }

            // Find the corresponding type
            var type = Assembly.GetExecutingAssembly().GetTypes()
                .FirstOrDefault(t => typeof(SmartContract.Testing.SmartContract).IsAssignableFrom(t) && t.Name == result.ContractName);

            // Ensure that it exists
            var (debug, res) = CreateArtifact(result.ContractName!, result, root, Path.Combine(ArtifactsPath, $"{result.ContractName}.cs"));
            if (debug != null)
            {
                DebugInfos[type] = debug;
            }
            else
            {
                UpdatedArtifactNames.TryAdd(res);
                Console.Error.WriteLine($"Artifact {res} was updated. please rerun the tests.");
            }
            return [result];
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
