using Akka.Util;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class TestCleanup : TestCleanupBase
    {
        private static readonly Regex WhiteSpaceRegex = new("\\s");
        public static readonly ConcurrentDictionary<Type, NeoDebugInfo> DebugInfos = new();
        private static readonly string ArtifactsPath = Path.GetFullPath(Path.Combine("..", "..", "..", "TestingArtifacts"));
        private static readonly string TestContractsPath = Path.GetFullPath(Path.Combine("..", "..", "..", "..", "Neo.Compiler.CSharp.TestContracts", "Neo.Compiler.CSharp.TestContracts.csproj"));
        private static readonly string RootPath = Path.GetPathRoot(TestContractsPath) ?? string.Empty;

        private static readonly Lazy<CompilationEngine> _compilationEngine = new(() => new CompilationEngine(new CompilationOptions
        {
            Debug = true,
            CompilerVersion = "TestingEngine",
            Optimize = CompilationOptions.OptimizationType.All,
            Nullable = NullableContextOptions.Enable
        }));

        private static List<INamedTypeSymbol> _sortedClasses;
        private static Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> _classDependencies;
        private static List<INamedTypeSymbol?> _allClassSymbols;
        private static readonly ConcurrentSet<string> UpdatedArtifactNames = new();

        [AssemblyInitialize]
        public static void TestAssemblyInitializeAsync(TestContext testContext)
        {
            (_sortedClasses, _classDependencies, _allClassSymbols) =
                _compilationEngine.Value.PrepareProjectContracts(TestContractsPath);
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
                if (DebugInfos.ContainsKey(contract)) return;
                EnsureArtifactUpToDateInternalAsync(contract.Name).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Assert.Fail($"Error compiling contract {contract.Name}: {e.Message}");
            }
        }

        [AssemblyCleanup]
        public static void EnsureCoverage()
        {
            if (UpdatedArtifactNames.Count > 0)
                Assert.Fail($"Some artifacts were updated: {string.Join(", ", UpdatedArtifactNames)}. Please rerun the tests.");

            // this is because we still miss tests for:
            //     Contract_Logical
            //     Contract_MemberAccess
            //     Contract_NEP11
            //     Contract_NEP17
            //     Contract_OnDeployment
            //     Contract_OnDeployment2
            // TODO: add tests for them
            if (DebugInfos.Count == _sortedClasses.Count - 7)
                EnsureCoverageInternal(Assembly.GetExecutingAssembly(), DebugInfos, 0.77M);
        }

        internal static async Task<IEnumerable<CompilationContext>> EnsureArtifactUpToDateInternalAsync(string singleContractName)
        {
            var result = _compilationEngine.Value.CompileProject(TestContractsPath, _sortedClasses, _classDependencies, _allClassSymbols, singleContractName).FirstOrDefault() ?? throw new InvalidOperationException($"No compilation result found for {singleContractName}"); ;

            if (result.ContractName != "Contract_DuplicateNames" && !result.Success)
            {
                var errors = string.Join(Environment.NewLine, result.Diagnostics
                    .Where(u => u.Severity == DiagnosticSeverity.Error)
                    .Select(d => d.ToString()));
                Assert.Fail($"Error compiling contract {result.ContractName}: {errors}");
            }

            var type = Assembly.GetExecutingAssembly().GetTypes()
                .FirstOrDefault(t => typeof(SmartContract.Testing.SmartContract).IsAssignableFrom(t) &&
                                     t.Name.Equals(result.ContractName, StringComparison.OrdinalIgnoreCase));
            if (type == null)
            {
                throw new InvalidOperationException($"Could not find type for contract {result.ContractName}");
            }
            var debug = CreateArtifactAsync(result.ContractName!, result, RootPath, Path.Combine(ArtifactsPath, $"{result.ContractName}.cs")).GetAwaiter().GetResult(); ;
            if (debug != null)
            {
                DebugInfos[type] = debug;
            }
            return [result];
        }

        private static async Task<NeoDebugInfo?> CreateArtifactAsync(string typeName, CompilationContext context, string rootDebug, string artifactsPath)
        {
            var (nef, manifest, debugInfo) = context.CreateResults(rootDebug);
            var debug = NeoDebugInfo.FromDebugInfoJson(debugInfo);
            var artifact = manifest.GetArtifactsSource(typeName, nef, generateProperties: true);

            var writtenArtifact = File.Exists(artifactsPath)
                ? await File.ReadAllTextAsync(artifactsPath).ConfigureAwait(false)
                : "";

            if (string.IsNullOrEmpty(writtenArtifact) || WhiteSpaceRegex.Replace(artifact, "") != WhiteSpaceRegex.Replace(writtenArtifact, ""))
            {
                await Task.Run(() =>
                {
                    try
                    {
                        File.WriteAllText(artifactsPath, artifact);
                        Console.WriteLine($"{typeName} artifact was updated");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error writing artifact for {typeName}: {ex.Message}");
                    }
                });
                return null;
            }

            return debug;
        }
    }
}
