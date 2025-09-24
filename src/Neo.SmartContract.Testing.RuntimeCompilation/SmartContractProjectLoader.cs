// Copyright (C) 2015-2025 The Neo Project.
//
// SmartContractProjectLoader.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Neo;
using Neo.Compiler;
using CompilerOptions = Neo.Compiler.CompilationOptions;
using Neo.Json;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Extensions;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;

namespace Neo.SmartContract.Testing.RuntimeCompilation;

/// <summary>
/// Builds smart contract artifacts on-demand by invoking Neo.Compiler.CSharp and
/// materialises testing proxies without requiring the developer to pre-generate files.
/// </summary>
public static class SmartContractProjectLoader
{
    private static readonly ConcurrentDictionary<string, CacheEntry> Cache = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Compiles the specified project (or solution) and returns the generated artifacts.
    /// Subsequent calls reuse cached assemblies until the source tree fingerprint changes.
    /// </summary>
    /// <param name="projectPath">Path to a .csproj or .sln containing smart contracts.</param>
    /// <param name="options">Optional build configuration.</param>
    /// <returns>A <see cref="ProjectArtifacts"/> snapshot describing all compiled contracts.</returns>
    public static ProjectArtifacts LoadProject(string projectPath, ArtifactBuildOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(projectPath))
        {
            throw new ArgumentException("Project path can't be null or empty.", nameof(projectPath));
        }

        options ??= ArtifactBuildOptions.Default;
        var normalizedPath = Path.GetFullPath(projectPath);

        var fingerprint = options.ForceRebuild ? null : ProjectFingerprint.TryCompute(normalizedPath);

        if (fingerprint is not null &&
            Cache.TryGetValue(normalizedPath, out var cached) &&
            cached.Matches(fingerprint))
        {
            return cached.Artifacts;
        }

        var freshEntry = BuildArtifacts(normalizedPath, options);

        Cache.AddOrUpdate(normalizedPath,
            _ => freshEntry,
            (_, previous) =>
            {
                previous.Dispose();
                return freshEntry;
            });

        return freshEntry.Artifacts;
    }

    /// <summary>
    /// Compiles the project and returns the artifacts for a specific contract. If a single
    /// contract is present the name may be omitted.
    /// </summary>
    /// <param name="projectPath">Path to the project or solution.</param>
    /// <param name="contractName">Optional contract name. Case insensitive.</param>
    /// <param name="options">Optional build configuration.</param>
    public static ContractArtifacts LoadContract(string projectPath, string? contractName = null, ArtifactBuildOptions? options = null)
    {
        return LoadProject(projectPath, options).GetContract(contractName);
    }

    private static CacheEntry BuildArtifacts(string projectPath, ArtifactBuildOptions options)
    {
        var compilationOptions = new CompilerOptions
        {
            Debug = options.Debug,
            Optimize = options.Optimize,
            Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable,
            CompilerVersion = "RuntimeCompilation"
        };

        var engine = new CompilationEngine(compilationOptions);
        var contexts = engine.CompileProject(projectPath);

        var diagnostics = contexts.SelectMany(c => c.Diagnostics).Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
        if (diagnostics.Length > 0)
        {
            var message = new StringBuilder()
                .AppendLine("Failed to compile smart contract project. Diagnostics:")
                .AppendJoin(Environment.NewLine, diagnostics.Select(d => d.ToString()))
                .ToString();
            throw new InvalidOperationException(message);
        }

        var projectDirectory = Path.GetDirectoryName(projectPath) ?? Environment.CurrentDirectory;

        var pending = new List<PendingContract>();
        foreach (var context in contexts)
        {
            if (!context.Success) continue;

            var (nef, manifest, debugJson) = context.CreateResults(projectDirectory);
            var contractName = context.ContractName ?? manifest.Name ?? Path.GetFileNameWithoutExtension(projectPath);

            // Preserve debug information only when requested and available.
            NeoDebugInfo? debugInfo = null;
            if (options.Debug != CompilerOptions.DebugType.None && debugJson is not null)
            {
                debugInfo = NeoDebugInfo.FromDebugInfoJson((JObject)debugJson);
            }

            var artifactSource = manifest.GetArtifactsSource(contractName, nef, options.GenerateProperties, debugInfo: options.Debug != CompilerOptions.DebugType.None ? debugJson : null, traceRemarks: options.TraceRemarks);

            pending.Add(new PendingContract(contractName, manifest, nef, debugInfo, artifactSource));
        }

        if (pending.Count == 0)
        {
            throw new InvalidOperationException($"No smart contracts were discovered when compiling '{projectPath}'.");
        }

        var (assembly, loadContext) = CompileArtifactsAssembly(pending);

        var artifacts = new Dictionary<string, ContractArtifacts>(StringComparer.OrdinalIgnoreCase);
        foreach (var contract in pending)
        {
            var proxyType = assembly.GetType($"Neo.SmartContract.Testing.{contract.ContractName}");
            if (proxyType is null)
            {
                throw new InvalidOperationException($"Unable to locate generated proxy type for contract '{contract.ContractName}'.");
            }

            artifacts[contract.ContractName] = new ContractArtifacts(contract.ContractName, contract.Nef, contract.Manifest, contract.DebugInfo, proxyType);
        }

        var fingerprint = ProjectFingerprint.Compute(projectPath);
        var projectArtifacts = new ProjectArtifacts(artifacts, loadContext);
        return new CacheEntry(fingerprint, projectArtifacts);
    }

    private static (Assembly Assembly, ArtifactAssemblyLoadContext LoadContext) CompileArtifactsAssembly(IEnumerable<PendingContract> contracts)
    {
        var syntaxTrees = contracts
            .Select(contract => CSharpSyntaxTree.ParseText(contract.Source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest)))
            .ToArray();

        var coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location) ?? string.Empty;
        var neoAssembly = typeof(NeoSystem).Assembly;
        var extensionsAssembly = Assembly.Load(neoAssembly
            .GetReferencedAssemblies()
            .First(a => string.Equals(a.Name, "Neo.Extensions", StringComparison.OrdinalIgnoreCase)));

        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")),
            MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Private.CoreLib.dll")),
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(DisplayNameAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Numerics.BigInteger).Assembly.Location),
            MetadataReference.CreateFromFile(neoAssembly.Location),
            MetadataReference.CreateFromFile(typeof(TestEngine).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Neo.IO.ISerializable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ContractManifest).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(NefFile).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Neo.SmartContract.Framework.SmartContract).Assembly.Location),
            MetadataReference.CreateFromFile(extensionsAssembly.Location)
        };

        var assemblyName = $"NeoArtifacts_{Guid.NewGuid():N}";
        var compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees,
            references,
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Debug,
                platform: Platform.AnyCpu,
                nullableContextOptions: Microsoft.CodeAnalysis.NullableContextOptions.Enable,
                deterministic: true));

        using var peStream = new MemoryStream();
        var emit = compilation.Emit(peStream);
        if (!emit.Success)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Failed to materialise contract proxy assembly:");
            foreach (var diagnostic in emit.Diagnostics)
            {
                builder.AppendLine(diagnostic.ToString());
            }
            throw new InvalidOperationException(builder.ToString());
        }

        peStream.Seek(0, SeekOrigin.Begin);
        var context = new ArtifactAssemblyLoadContext();
        var assembly = context.LoadFromStream(peStream);
        return (assembly, context);
    }

    private sealed record PendingContract(string ContractName, ContractManifest Manifest, NefFile Nef, NeoDebugInfo? DebugInfo, string Source);

    private sealed class CacheEntry : IDisposable
    {
        private bool _disposed;

        internal CacheEntry(string fingerprint, ProjectArtifacts artifacts)
        {
            Fingerprint = fingerprint;
            Artifacts = artifacts;
        }

        public string Fingerprint { get; }
        public ProjectArtifacts Artifacts { get; }

        public bool Matches(string fingerprint) => string.Equals(Fingerprint, fingerprint, StringComparison.Ordinal);

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Artifacts.Dispose();
        }
    }

    /// <summary>
    /// Holds the generated proxy assembly and provides access to individual contract artifacts.
    /// </summary>
    public sealed class ProjectArtifacts : IDisposable
    {
        private readonly ArtifactAssemblyLoadContext _loadContext;
        private readonly IReadOnlyDictionary<string, ContractArtifacts> _contracts;
        private bool _disposed;

        internal ProjectArtifacts(IReadOnlyDictionary<string, ContractArtifacts> contracts, ArtifactAssemblyLoadContext loadContext)
        {
            _contracts = contracts;
            _loadContext = loadContext;
        }

        /// <summary>
        /// Enumerates the names of all compiled contracts.
        /// </summary>
        public IReadOnlyCollection<string> ContractNames => _contracts.Keys.ToArray();

        /// <summary>
        /// Retrieves artifacts for the requested contract. If <paramref name="contractName"/> is
        /// omitted the single compiled contract will be returned.
        /// </summary>
        public ContractArtifacts GetContract(string? contractName = null)
        {
            if (_contracts.Count == 0)
            {
                throw new InvalidOperationException("The project contains no compiled contracts.");
            }

            if (contractName is null)
            {
                return _contracts.Count == 1
                    ? _contracts.Values.First()
                    : throw new ArgumentException("Multiple contracts compiled. Specify a contract name to disambiguate.", nameof(contractName));
            }

            if (_contracts.TryGetValue(contractName, out var artifacts))
            {
                return artifacts;
            }

            var match = _contracts.Values.FirstOrDefault(a => string.Equals(a.ContractName, contractName, StringComparison.OrdinalIgnoreCase));
            if (match is null)
            {
                throw new ArgumentException($"Contract '{contractName}' not found. Available contracts: {string.Join(", ", _contracts.Keys)}", nameof(contractName));
            }

            return match;
        }

        internal void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_loadContext.IsCollectible)
            {
                _loadContext.Unload();
            }
        }

        void IDisposable.Dispose() => Dispose();
    }

    internal sealed class ArtifactAssemblyLoadContext : AssemblyLoadContext
    {
        public ArtifactAssemblyLoadContext() : base(isCollectible: false)
        {
            Resolving += ResolveFromDefaultContext;
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null;
        }

        private Assembly? ResolveFromDefaultContext(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            var loaded = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(a =>
                string.Equals(a.GetName().Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase));
            if (loaded is not null)
            {
                return loaded;
            }

            try
            {
                return AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
            }
            catch
            {
                return null;
            }
        }
    }

    private static class ProjectFingerprint
    {
        private static readonly string[] InterestingExtensions =
        {
            ".cs", ".csproj", ".sln", ".json", ".props", ".targets"
        };

        public static string? TryCompute(string projectPath)
        {
            try
            {
                return Compute(projectPath);
            }
            catch
            {
                return null;
            }
        }

        public static string Compute(string projectPath)
        {
            var projectDirectory = Path.GetDirectoryName(projectPath) ?? throw new InvalidOperationException($"Unable to determine directory for '{projectPath}'.");
            var builder = new StringBuilder();

            foreach (var file in EnumerateRelevantFiles(projectDirectory))
            {
                var info = new FileInfo(file);
                builder.Append(info.FullName.ToLowerInvariant());
                builder.Append('|');
                builder.Append(info.Length.ToString(CultureInfo.InvariantCulture));
                builder.Append('|');
                builder.Append(info.LastWriteTimeUtc.Ticks.ToString(CultureInfo.InvariantCulture));
                builder.Append('\n');
            }

            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(builder.ToString())));
        }

        private static IEnumerable<string> EnumerateRelevantFiles(string root)
        {
            foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            {
                if (IsIgnored(file, root))
                    continue;

                var extension = Path.GetExtension(file);
                if (InterestingExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    yield return file;
                }
            }
        }

        private static bool IsIgnored(string filePath, string root)
        {
            var relative = filePath[root.Length..];
            if (relative.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                return true;
            if (relative.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                return true;
            if (relative.Contains($"{Path.DirectorySeparatorChar}out{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
