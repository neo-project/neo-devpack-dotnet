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

namespace Example.SmartContract.NFT.UnitTests
{
    public class TestCleanup
    {
        private static readonly Regex WhiteSpaceRegex = new("\\s");
        internal static readonly Dictionary<Type, NeoDebugInfo> DebugInfos = new();

        internal static void EnsureArtifactsUpToDateInternal()
        {
            if (DebugInfos.Count > 0) return; // Maybe a UT call it

            // Define paths

            string testContractsPath = Path.GetFullPath("../../../../Example.SmartContract.NFT/Example.SmartContract.NFT.csproj");
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
                if (typeof(Neo.SmartContract.Testing.SmartContract).IsAssignableFrom(type))
                {
                    // Find result

                    var result = results.SingleOrDefault(u => u.ContractName == type.Name);
                    if (result == null) continue;

                    // Ensure that it exists

                    DebugInfos[type] = CreateArtifact(result.ContractName!, result, root, Path.Combine(artifactsPath, $"{result.ContractName}.cs"), false);
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
            if (string.IsNullOrEmpty(writtenArtifact) || WhiteSpaceRegex.Replace(artifact, "") != WhiteSpaceRegex.Replace(writtenArtifact, ""))
            {
                if (!File.Exists(artifactsPath))
                {
                    string? directoryPath = Path.GetDirectoryName(artifactsPath);

                    if (!Directory.Exists(directoryPath))
                    {
                        if (directoryPath == null)
                            throw new ArgumentNullException($"{nameof(directoryPath)} is null");
                        Directory.CreateDirectory(directoryPath);
                    }

                    File.Create(artifactsPath).Close();
                }

                try
                {
                    File.WriteAllText(artifactsPath, artifact);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                if (failIfWrong) Assert.Fail($"{typeName} artifact was wrong");
            }

            return debug;
        }
    }
}
