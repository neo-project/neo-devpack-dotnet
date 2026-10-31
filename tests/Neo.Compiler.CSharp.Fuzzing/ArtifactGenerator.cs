// Copyright (C) 2015-2026 The Neo Project.
//
// ArtifactGenerator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Neo.SmartContract;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Extensions;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Regenerates a testing artifact for a contract in the test-contract project.
/// </summary>
/// <remarks>
/// <para>
/// The unit-test project compiles the generated artifacts in
/// <c>tests/Neo.Compiler.CSharp.UnitTests/TestingArtifacts/</c> rather than the contract
/// sources directly. A contract that is added to the test-contract project without a
/// matching artifact therefore breaks the unit-test build, and the usual regeneration
/// path — running the tests, which rewrites the artifacts — is itself blocked because
/// the project does not compile.
/// </para>
/// <para>
/// This tool breaks that deadlock by performing the same steps as
/// <c>TestCleanup.CreateArtifactAsync</c>: compile the target contract, then write
/// <c>manifest.GetArtifactsSource(...)</c> to the artifact directory.
/// </para>
/// </remarks>
public static class ArtifactGenerator
{
    /// <summary>
    /// Generates the artifact for a contract.
    /// </summary>
    /// <param name="repositoryRoot">The repository root, used to locate the projects.</param>
    /// <param name="contractName">The contract to generate, for example <c>Contract_ArithmeticOverflow</c>.</param>
    /// <returns>Process exit code: zero on success.</returns>
    public static int Generate(string repositoryRoot, string contractName)
    {
        string testContractsDirectory = Path.Combine(
            repositoryRoot, "tests", "Neo.Compiler.CSharp.TestContracts");

        string artifactsDirectory = Path.Combine(
            repositoryRoot, "tests", "Neo.Compiler.CSharp.UnitTests", "TestingArtifacts");

        if (!Directory.Exists(testContractsDirectory))
        {
            Console.Error.WriteLine($"Cannot find the test-contract directory at '{testContractsDirectory}'.");
            return 2;
        }

        var options = new CompilationOptions
        {
            Debug = CompilationOptions.DebugType.Extended,
            CompilerVersion = "TestingEngine",
            Optimize = CompilationOptions.OptimizationType.All,
            Nullable = NullableContextOptions.Enable,
            SkipRestoreIfAssetsPresent = true,
        };

        var engine = new CompilationEngine(options);

        // The project-based compilation path spawns MSBuild, which is slow and depends
        // on the SDK being resolvable from the repository. The contract sources are
        // plain C# files, so they can be compiled through the pure Roslyn path instead.
        string[] sourceFiles = Directory.GetFiles(testContractsDirectory, "*.cs")
            .OrderBy(static p => p, StringComparer.Ordinal)
            .ToArray();

        if (sourceFiles.Length == 0)
        {
            Console.Error.WriteLine($"No contract sources were found in '{testContractsDirectory}'.");
            return 2;
        }

        using var harness = new CompilerHarness(options, excludeCoreAssemblies: true);
        List<CompilationContext> contexts = engine.Compile(sourceFiles, harness.References);

        CompilationContext? context = contexts.FirstOrDefault(
            c => string.Equals(c.ContractName, contractName, StringComparison.OrdinalIgnoreCase));

        if (context is null)
        {
            Console.Error.WriteLine($"No compilation result was produced for '{contractName}'.");
            foreach (Diagnostic diagnostic in contexts.SelectMany(c => c.Diagnostics).Where(d =>
                d.Severity == DiagnosticSeverity.Error &&
                d.Location.SourceTree?.FilePath.Contains(contractName, StringComparison.OrdinalIgnoreCase) == true))
            {
                Console.Error.WriteLine($"  {diagnostic}");
            }
            return 2;
        }

        if (!context.Success)
        {
            Console.Error.WriteLine($"The contract '{contractName}' does not compile:");
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
            {
                Console.Error.WriteLine($"  {diagnostic}");
            }
            return 2;
        }

        string rootDebug = Path.GetPathRoot(testContractsDirectory) ?? string.Empty;

        (NefFile nef, Neo.SmartContract.Manifest.ContractManifest manifest, Neo.Json.JObject debugInfo) =
            context.CreateResults(rootDebug);

        // The project version normally reaches the manifest through the project file
        // (Directory.Build.props -> VersionPrefix). The fast path used here does not go
        // through MSBuild, so the version is read directly and injected into the manifest
        // extra, keeping the artifact byte-identical to the one the test harness writes.
        string? version = ReadVersionPrefix(Path.Combine(repositoryRoot, "src", "Directory.Build.props"));
        if (version is not null)
        {
            manifest.Extra ??= new Neo.Json.JObject();
            var reordered = new Neo.Json.JObject();
            reordered["Version"] = version;
            foreach (KeyValuePair<string, Neo.Json.JToken?> property in manifest.Extra.Properties)
            {
                reordered[property.Key] = property.Value;
            }
            manifest.Extra = reordered;
        }

        NeoDebugInfo? debug = NeoDebugInfo.FromDebugInfoJson(debugInfo);
        string artifact = manifest.GetArtifactsSource(context.ContractName, nef, generateProperties: true, debugInfo);

        Directory.CreateDirectory(artifactsDirectory);
        string artifactPath = Path.Combine(artifactsDirectory, $"{context.ContractName}.cs");
        File.WriteAllText(artifactPath, artifact);

        Console.WriteLine($"Wrote {artifactPath} ({artifact.Length} chars).");
        Console.WriteLine($"  manifest methods : {manifest.Abi.Methods.Length}");
        Console.WriteLine($"  script length    : {nef.Script.Length} bytes");
        Console.WriteLine($"  version          : {version ?? "<none>"}");
        Console.WriteLine($"  debug info       : {(debug is null ? "<none>" : "present")}");

        return 0;
    }

    /// <summary>
    /// Reads the <c>VersionPrefix</c> property from a MSBuild props file.
    /// </summary>
    /// <param name="propsPath">The path of the props file.</param>
    /// <returns>The version, or <c>null</c> when it cannot be determined.</returns>
    private static string? ReadVersionPrefix(string propsPath)
    {
        if (!File.Exists(propsPath))
        {
            return null;
        }

        System.Xml.Linq.XDocument document = System.Xml.Linq.XDocument.Load(propsPath);
        return document.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "VersionPrefix")?
            .Value;
    }
}
