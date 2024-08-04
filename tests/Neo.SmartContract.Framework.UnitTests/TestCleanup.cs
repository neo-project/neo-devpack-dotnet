using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Extensions;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Akka.Util;
using Microsoft.CodeAnalysis;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class TestCleanup : TestCleanupBase
    {
        private static readonly Regex WhiteSpaceRegex = new("\\s", RegexOptions.Compiled);
        public static readonly ConcurrentDictionary<Type, NeoDebugInfo> DebugInfos = new();
        private static readonly string TestContractsPath = Path.GetFullPath(Path.Combine("..", "..", "..", "..", "Neo.SmartContract.Framework.TestContracts", "Neo.SmartContract.Framework.TestContracts.csproj"));
        private static readonly string ArtifactsPath = Path.GetFullPath(Path.Combine("..", "..", "..", "TestingArtifacts"));
        private static readonly string RootPath = Path.GetDirectoryName(TestContractsPath) ?? string.Empty;

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
        public static void TestAssemblyInitialize(TestContext testContext)
        {
            (_sortedClasses, _classDependencies, _allClassSymbols) = _compilationEngine.Value.PrepareProjectContracts(TestContractsPath);
        }

        public static void TestInitialize(Type contract)
        {
            if (!typeof(SmartContract.Testing.SmartContract).IsAssignableFrom(contract))
            {
                throw new InvalidOperationException($"The type {contract.Name} does not inherit from SmartContract.Testing.SmartContract");
            }
            if (DebugInfos.ContainsKey(contract)) return;
            EnsureArtifactUpToDateInternalAsync(contract.Name).GetAwaiter().GetResult();
        }

        [AssemblyCleanup]
        public static void EnsureCoverage()
        {
            if (UpdatedArtifactNames.Count > 0)
                Assert.Fail($"Some artifacts were updated: {string.Join(", ", UpdatedArtifactNames)}. Please rerun the tests.");

            // this is because we still miss tests/not tested with Testbase for:
            //     Contract_Create
            //     Contract_ExtraAttribute
            //     Contract_ManifestAttribute
            //     Contract_SupportedStandard11Enum
            //     Contract_SupportedStandard11Payable
            //     Contract_SupportedStandard17Enum
            //     Contract_SupportedStandard17Payable
            //     Contract_SupportedStandards
            //     Contract_Update
            if (DebugInfos.Count == _sortedClasses.Count - 9)
                EnsureCoverageInternal(Assembly.GetExecutingAssembly(), DebugInfos);
        }

        private static async Task EnsureArtifactUpToDateInternalAsync(string singleContractName)
        {
            var result = _compilationEngine.Value.CompileProject(TestContractsPath, _sortedClasses, _classDependencies, _allClassSymbols, singleContractName).FirstOrDefault() ?? throw new InvalidOperationException($"No compilation result found for {singleContractName}"); ;
            if (!result.Success)
            {
                var errors = string.Join(Environment.NewLine, result.Diagnostics
                    .Where(u => u.Severity == DiagnosticSeverity.Error)
                    .Select(d => d.ToString()));
                Assert.Fail($"Error compiling {result.ContractName}: {errors}");
            }

            var type = Assembly.GetExecutingAssembly().GetTypes()
                .FirstOrDefault(t => typeof(SmartContract.Testing.SmartContract).IsAssignableFrom(t) &&
                                     t.Name.Equals(result.ContractName, StringComparison.OrdinalIgnoreCase));

            if (type == null)
            {
                Assert.Fail($"Could not find type for contract {result.ContractName}");
                return;
            }
            var debug = CreateArtifactAsync(result.ContractName!, result, RootPath, Path.Combine(ArtifactsPath, $"{result.ContractName}.cs")).GetAwaiter().GetResult();

            if (debug != null)
            {
                DebugInfos[type] = debug;
            }
        }

        private static async Task<NeoDebugInfo?> CreateArtifactAsync(string typeName, CompilationContext context, string rootDebug, string artifactsPath)
        {
            var (nef, manifest, debugInfo) = context.CreateResults(rootDebug);
            var debug = NeoDebugInfo.FromDebugInfoJson(debugInfo);
            var artifact = manifest.GetArtifactsSource(typeName, nef, generateProperties: true);

            var writtenArtifact = File.Exists(artifactsPath)
                ? await File.ReadAllTextAsync(artifactsPath)
                : "";

            if (string.IsNullOrEmpty(writtenArtifact) || WhiteSpaceRegex.Replace(artifact, "") != WhiteSpaceRegex.Replace(writtenArtifact, ""))
            {
                _ = Task.Run(() =>
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
