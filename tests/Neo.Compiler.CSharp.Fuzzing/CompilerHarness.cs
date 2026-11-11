// Copyright (C) 2015-2026 The Neo Project.
//
// CompilerHarness.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Reflection;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Drives <see cref="CompilationEngine"/> over arbitrary C# sources.
/// </summary>
/// <remarks>
/// <para>
/// The harness deliberately uses <see cref="CompilationEngine.Compile(IEnumerable{string}, IEnumerable{MetadataReference})"/>
/// rather than <c>CompileSources</c>: the former is a pure Roslyn path with no MSBuild / restore
/// round-trip, which is what makes high-throughput fuzzing possible.
/// </para>
/// <para>
/// The reference set is built from <c>TRUSTED_PLATFORM_ASSEMBLIES</c> plus the local
/// <c>Neo.SmartContract.Framework</c> assembly, mirroring what the unit-test probes do.
/// </para>
/// </remarks>
public sealed class CompilerHarness : IDisposable
{
    /// <summary>Diagnostic id emitted when the compiler converts an unexpected exception into a diagnostic.</summary>
    public const string UnexpectedCompilerErrorId = "NC5001";

    private readonly CompilationEngine _engine;
    private readonly ImmutableArray<MetadataReference> _references;
    private readonly string _tempDirectory;

    /// <summary>
    /// Creates a harness with the default compiler options.
    /// </summary>
    public CompilerHarness() : this(CreateDefaultOptions())
    {
    }

    /// <summary>
    /// Creates a harness with explicit compiler options.
    /// </summary>
    /// <param name="options">Options applied to every compilation.</param>
    /// <param name="excludeCoreAssemblies">Whether to leave the Neo core assemblies out of the reference set.</param>
    public CompilerHarness(CompilationOptions options, bool excludeCoreAssemblies = false)
    {
        Options = options;
        _engine = new CompilationEngine(options);
        _references = CreateReferences(excludeCoreAssemblies);
        _tempDirectory = Path.Combine(Path.GetTempPath(), "neofuzz-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDirectory);
    }

    /// <summary>Options used for every compilation performed by this harness.</summary>
    public CompilationOptions Options { get; }

    /// <summary>
    /// The metadata reference set used for compilation.
    /// </summary>
    /// <remarks>
    /// Exposed so that other tools can compile additional sources with exactly the same
    /// references this harness uses.
    /// </remarks>
    public ImmutableArray<MetadataReference> References => _references;

    /// <summary>
    /// Builds the default option set: full optimization, extended debug info, nullable enabled.
    /// </summary>
    public static CompilationOptions CreateDefaultOptions() => new()
    {
        Debug = CompilationOptions.DebugType.Extended,
        CompilerVersion = "FuzzHarness",
        Optimize = CompilationOptions.OptimizationType.All,
        Nullable = NullableContextOptions.Enable,
        SkipRestoreIfAssetsPresent = true,
    };

    /// <summary>
    /// Compiles a source that may declare several contracts and returns every result.
    /// </summary>
    /// <remarks>
    /// The compiler produces one <see cref="CompilationContext"/> per contract in a
    /// project. <see cref="Compile"/> only looks at the first one, which is enough for a
    /// single-contract input but loses the other contracts; a multi-contract input needs
    /// all of them so that each can be deployed.
    /// </remarks>
    /// <param name="source">The C# source to compile.</param>
    /// <returns>The outcome and every contract the source declared, in echelon order.</returns>
    public (FuzzOutcome Outcome, List<(string Name, CompilationContext Context)> Contracts, FuzzFinding? Finding)
        CompileAll(string source)
    {
        string path = Path.Combine(_tempDirectory, Guid.NewGuid().ToString("N") + ".cs");
        File.WriteAllText(path, source);

        try
        {
            List<CompilationContext> contexts = _engine.Compile([path], _references);

            if (contexts.Count == 0)
            {
                return (FuzzOutcome.Rejected, [], null);
            }

            Diagnostic? internalError = contexts
                .SelectMany(c => c.Diagnostics)
                .FirstOrDefault(d => d.Id == UnexpectedCompilerErrorId);

            if (internalError is not null)
            {
                return (FuzzOutcome.InternalError, [], new FuzzFinding(
                    FuzzOutcome.InternalError,
                    source,
                    $"{internalError.Id}: {internalError.GetMessage()}",
                    internalError.GetMessage()));
            }

            var contracts = contexts
                .Select(c => (Name: c.ContractName ?? string.Empty, Context: c))
                .Where(c => !string.IsNullOrEmpty(c.Name))
                .ToList();

            if (contracts.Any(c => !c.Context.Success))
            {
                return (FuzzOutcome.Rejected, [], null);
            }

            return (FuzzOutcome.Clean, contracts, null);
        }
        catch (Exception ex)
        {
            if (IsCompilationException(ex))
            {
                return (FuzzOutcome.Rejected, [], null);
            }

            return (FuzzOutcome.CompilerCrash, [], new FuzzFinding(
                FuzzOutcome.CompilerCrash,
                source,
                $"{ex.GetType().FullName}: {ex.Message}",
                $"{ex.GetType().FullName}: {ex.Message}",
                ex.ToString()));
        }
        finally
        {
            try { File.Delete(path); } catch { /* best effort */ }
        }
    }

    /// <summary>
    /// Compiles a single source file and classifies the result.
    /// </summary>
    /// <param name="source">The C# source to compile.</param>
    /// <param name="context">The compilation context, when the compilation reached that stage.</param>
    /// <returns>The outcome plus any defect detail.</returns>
    public (FuzzOutcome Outcome, CompilationContext? Context, FuzzFinding? Finding) Compile(string source)
    {
        string path = Path.Combine(_tempDirectory, Guid.NewGuid().ToString("N") + ".cs");
        File.WriteAllText(path, source);

        try
        {
            List<CompilationContext> contexts = _engine.Compile([path], _references);

            // A source with no SmartContract-derived type yields zero contexts. That is a
            // legitimate rejection, not a defect.
            if (contexts.Count == 0)
            {
                return (FuzzOutcome.Rejected, null, null);
            }

            CompilationContext context = contexts[0];

            // The compiler converts unexpected internal exceptions into NC5001 diagnostics.
            // Those indicate a defect in the compiler's own error handling, even though the
            // compilation "succeeded" from the caller's point of view.
            Diagnostic? internalError = context.Diagnostics
                .FirstOrDefault(d => d.Id == UnexpectedCompilerErrorId);

            if (internalError is not null)
            {
                return (FuzzOutcome.InternalError, context, new FuzzFinding(
                    FuzzOutcome.InternalError,
                    source,
                    $"{internalError.Id}: {internalError.GetMessage()}",
                    internalError.GetMessage()));
            }

            if (!context.Success)
            {
                return (FuzzOutcome.Rejected, context, null);
            }

            return (FuzzOutcome.Clean, context, null);
        }
        catch (Exception ex)
        {
            // Anything that reaches here escaped the compiler's own exception handling.
            // CompilationException is internal to the compiler assembly, so it is matched
            // by name rather than by type.
            if (IsCompilationException(ex))
            {
                return (FuzzOutcome.Rejected, null, null);
            }

            return (FuzzOutcome.CompilerCrash, null, new FuzzFinding(
                FuzzOutcome.CompilerCrash,
                source,
                $"{ex.GetType().FullName}: {ex.Message}",
                $"{ex.GetType().FullName}: {ex.Message}",
                ex.ToString()));
        }
        finally
        {
            try { File.Delete(path); } catch { /* best effort */ }
        }
    }

    /// <summary>
    /// Determines whether an exception is the compiler's own <c>CompilationException</c>.
    /// </summary>
    /// <param name="ex">The exception to inspect.</param>
    /// <returns><c>true</c> when the exception is a <c>CompilationException</c>.</returns>
    private static bool IsCompilationException(Exception ex)
    {
        for (Type? t = ex.GetType(); t is not null; t = t.BaseType)
        {
            if (t.Name == "CompilationException" && t.Namespace == "Neo.Compiler")
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Builds the metadata reference set used for compilation.
    /// </summary>
    /// <param name="excludeCoreAssemblies">
    /// When <c>true</c>, the Neo core assemblies are left out. The test-contract project
    /// does not reference the Neo core package — the smart-contract framework defines its
    /// own <c>UInt160</c> and <c>ByteString</c> — so including the core assemblies would
    /// make <c>using Neo;</c> resolve <c>Runtime</c> and <c>UInt160</c> to the wrong types.
    /// </param>
    public static ImmutableArray<MetadataReference> CreateReferences(bool excludeCoreAssemblies = false)
    {
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string tpa && !string.IsNullOrWhiteSpace(tpa))
        {
            foreach (string path in tpa.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!File.Exists(path))
                {
                    continue;
                }

                if (excludeCoreAssemblies && IsNeoCoreAssembly(path))
                {
                    continue;
                }

                paths.Add(path);
            }
        }

        string? framework = typeof(scfx::Neo.SmartContract.Framework.SmartContract).Assembly.Location;
        if (!string.IsNullOrWhiteSpace(framework) && File.Exists(framework))
        {
            paths.Add(framework);
        }

        return paths
            .OrderBy(static p => p, StringComparer.OrdinalIgnoreCase)
            .Select(static p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToImmutableArray();
    }

    /// <summary>
    /// Determines whether an assembly belongs to the Neo core package.
    /// </summary>
    /// <param name="path">The assembly path.</param>
    /// <returns><c>true</c> when the assembly is part of the Neo core package.</returns>
    private static bool IsNeoCoreAssembly(string path)
    {
        string fileName = Path.GetFileName(path);
        return fileName.StartsWith("Neo.", StringComparison.OrdinalIgnoreCase) &&
               !fileName.StartsWith("Neo.SmartContract.", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Releases the temporary directory backing this harness.
    /// </summary>
    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
        }
        catch
        {
            // best effort
        }
    }
}
