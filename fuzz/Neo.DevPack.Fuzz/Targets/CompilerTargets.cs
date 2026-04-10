using Microsoft.CodeAnalysis;
using Neo;
using Neo.Compiler;
using Neo.Extensions;
using Neo.SmartContract.Testing.Extensions;
using System.Text;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.DevPack.Fuzz.Targets;

internal abstract class CompilationTargetBase : IFuzzTarget
{
    private readonly Dictionary<string, CompilationEngine> _sourceEngines = new(StringComparer.Ordinal);
    private readonly CompilationSourceReferences _references;
    private readonly SourceWorkspace _workspace;

    protected CompilationTargetBase(RepoLayout layout, string workspaceName)
    {
        Layout = layout;
        _workspace = new SourceWorkspace(workspaceName);
        _references = new CompilationSourceReferences
        {
            Projects = [layout.FrameworkProjectPath]
        };
    }

    public abstract string Name { get; }

    public abstract string Description { get; }

    public abstract FuzzCaseResult Execute(byte[] input);

    protected RepoLayout Layout { get; }

    protected CompilationOptions CreateOptions(ReadOnlySpan<byte> input)
    {
        var optimizeSelector = input.Length > 0 ? input[0] % 3 : 1;
        var debugSelector = input.Length > 1 ? input[1] % 3 : 0;
        var nullableSelector = input.Length > 2 ? input[2] % 4 : 2;
        var flags = input.Length > 3 ? input[3] : (byte)0;

        return new CompilationOptions
        {
            Optimize = optimizeSelector switch
            {
                0 => CompilationOptions.OptimizationType.None,
                1 => CompilationOptions.OptimizationType.Basic,
                _ => CompilationOptions.OptimizationType.All
            },
            Debug = debugSelector switch
            {
                0 => CompilationOptions.DebugType.None,
                1 => CompilationOptions.DebugType.Strict,
                _ => CompilationOptions.DebugType.Extended
            },
            Nullable = nullableSelector switch
            {
                0 => NullableContextOptions.Disable,
                1 => NullableContextOptions.Annotations,
                2 => NullableContextOptions.Enable,
                _ => NullableContextOptions.Warnings
            },
            Checked = (flags & 0x01) != 0,
            NoInline = (flags & 0x02) != 0,
            SkipRestoreIfAssetsPresent = true
        };
    }

    protected IReadOnlyList<CompilationContext> CompileSources(GeneratedSourceProject project, CompilationOptions options)
    {
        var sourceFiles = _workspace.WriteFiles(project.Files);
        var engine = GetOrCreateSourceEngine(options);
        return engine.CompileSources(_references, sourceFiles).ToArray();
    }

    protected CompilationEngine CreateProjectEngine(CompilationOptions options)
    {
        return new CompilationEngine(CloneOptions(options));
    }

    protected FuzzCaseResult SummarizeCompilation(string label, GeneratedSourceProject project, IReadOnlyList<CompilationContext> contexts)
    {
        var textArtifacts = new Dictionary<string, string>(project.Files, StringComparer.Ordinal);
        var diagnostics = SerializeDiagnostics(contexts);

        if (contexts.Count == 0)
        {
            return new FuzzCaseResult(
                $"{label}:no-context",
                $"{label}: compiler returned no contract contexts.",
                TextArtifacts: textArtifacts);
        }

        if (contexts.Any(static context => !context.Success))
        {
            var diagnosticIds = contexts
                .SelectMany(static context => context.Diagnostics)
                .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                .Select(static diagnostic => diagnostic.Id)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(static id => id, StringComparer.Ordinal);

            var summary = $"{label}: diagnostics={string.Join(",", diagnosticIds)}";
            textArtifacts["diagnostics.txt"] = diagnostics;

            return new FuzzCaseResult(
                $"{label}:diag:{Deterministic.HashHex(diagnostics)}",
                summary,
                TextArtifacts: textArtifacts);
        }

        var snapshots = CaptureSnapshots(contexts, _workspace.RootDirectory);
        foreach (var snapshot in snapshots)
        {
            textArtifacts[$"{snapshot.ContractName}.manifest.json"] = snapshot.ManifestJson;
            textArtifacts[$"{snapshot.ContractName}.debug.json"] = snapshot.DebugJson;
            textArtifacts[$"{snapshot.ContractName}.abi.cs"] = snapshot.InterfaceCode;
            textArtifacts[$"{snapshot.ContractName}.artifacts.cs"] = snapshot.ArtifactSource;
            textArtifacts[$"{snapshot.ContractName}.nef.txt"] = snapshot.AssemblyText;
        }

        return new FuzzCaseResult(
            $"{label}:ok:{Deterministic.JoinFingerprints(snapshots.Select(static snapshot => snapshot.Fingerprint))}",
            $"{label}: contracts={snapshots.Count} fingerprints={string.Join(", ", snapshots.Select(static snapshot => snapshot.ContractName))}",
            TextArtifacts: textArtifacts,
            BinaryArtifacts: snapshots.ToDictionary(
                static snapshot => $"{snapshot.ContractName}.nef",
                static snapshot => snapshot.NefBytes,
                StringComparer.Ordinal));
    }

    protected FuzzCaseResult SummarizeBenignFailure(string label, Exception exception, GeneratedSourceProject project)
    {
        var textArtifacts = new Dictionary<string, string>(project.Files, StringComparer.Ordinal)
        {
            ["failure.txt"] = exception.ToString()
        };

        return new FuzzCaseResult(
            $"{label}:failure:{Deterministic.HashHex(exception.ToString())}",
            $"{label}: {exception.GetType().Name}: {exception.Message}",
            TextArtifacts: textArtifacts);
    }

    protected IReadOnlyDictionary<string, string> CreateSourceArtifacts(GeneratedSourceProject project)
    {
        return new Dictionary<string, string>(project.Files, StringComparer.Ordinal);
    }

    protected string SerializeDiagnostics(IReadOnlyList<CompilationContext> contexts)
    {
        return string.Join(
            Environment.NewLine,
            contexts.SelectMany(static context => context.Diagnostics)
                .Select(static diagnostic => diagnostic.ToString()));
    }

    protected IReadOnlyList<CompilationSnapshot> CaptureSnapshots(IReadOnlyList<CompilationContext> contexts, string documentRoot)
    {
        var snapshots = new List<CompilationSnapshot>(contexts.Count);
        foreach (var context in contexts)
        {
            var (nef, manifest, debugInfo) = context.CreateResults(documentRoot);
            var manifestJson = manifest.ToJson().ToString(false);
            var debugJson = debugInfo.ToString(false);
            var assemblyText = context.CreateAssembly();
            var contractName = context.ContractName ?? manifest.Name;
            var contractHash = UInt160.Parse(debugInfo["hash"]!.GetString());
            var interfaceCode = ContractInterfaceGenerator.GenerateInterface(contractName, manifest, contractHash);
            var artifactSource = manifest.GetArtifactsSource(contractName, nef, debugInfo: debugInfo);

            snapshots.Add(
                new CompilationSnapshot(
                    contractName,
                    nef.ToArray(),
                    manifestJson,
                    debugJson,
                    assemblyText,
                    interfaceCode,
                    artifactSource));
        }

        return snapshots.OrderBy(static snapshot => snapshot.ContractName, StringComparer.Ordinal).ToArray();
    }

    private CompilationEngine GetOrCreateSourceEngine(CompilationOptions options)
    {
        var key = $"{options.Optimize}|{options.Debug}|{options.Nullable}|{options.Checked}|{options.NoInline}";
        if (_sourceEngines.TryGetValue(key, out var engine))
        {
            return engine;
        }

        engine = new CompilationEngine(CloneOptions(options));
        _sourceEngines[key] = engine;
        return engine;
    }

    private static CompilationOptions CloneOptions(CompilationOptions options)
    {
        return new CompilationOptions
        {
            AddressVersion = options.AddressVersion,
            BaseName = options.BaseName,
            Checked = options.Checked,
            CompilerVersion = options.CompilerVersion,
            Debug = options.Debug,
            NoInline = options.NoInline,
            Nullable = options.Nullable,
            Optimize = options.Optimize,
            SkipRestoreIfAssetsPresent = options.SkipRestoreIfAssetsPresent
        };
    }
}

internal sealed class CompilerTextTarget : CompilationTargetBase
{
    public CompilerTextTarget(RepoLayout layout)
        : base(layout, "compile-text")
    {
    }

    public override string Name => "fuzz_compile";

    public override string Description => "Compile mutated source text and wrapped code fragments.";

    public override FuzzCaseResult Execute(byte[] input)
    {
        var text = Encoding.UTF8.GetString(input);
        var source = LooksLikeFullSource(text) ? text : WrapAsContract(text);
        var project = GeneratedSourceProject.SingleFile("Contract", "Contract.cs", source);

        try
        {
            var contexts = CompileSources(project, CreateOptions(input));
            return SummarizeCompilation(Name, project, contexts);
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException)
        {
            return SummarizeBenignFailure(Name, ex, project);
        }
        catch (Exception ex)
        {
            throw new FuzzCrashException(
                $"{Name}: unexpected compiler crash",
                ex,
                CreateSourceArtifacts(project));
        }
    }

    private static bool LooksLikeFullSource(string text)
    {
        return text.Contains("class ", StringComparison.Ordinal)
            || text.Contains("SmartContract", StringComparison.Ordinal)
            || text.Contains("namespace ", StringComparison.Ordinal);
    }

    private static string WrapAsContract(string snippet)
    {
        return
            """
            using Neo.SmartContract.Framework;
            using Neo.SmartContract.Framework.Attributes;
            using Neo.SmartContract.Framework.Native;
            using Neo.SmartContract.Framework.Services;
            using System;
            using System.ComponentModel;
            using System.Numerics;

            public class Contract : SmartContract
            {
                [DisplayName("probe")]
                public static object? Probe()
                {
            """
            + Environment.NewLine
            + snippet
            + Environment.NewLine
            +
            """
                    return null;
                }
            }
            """;
    }
}
