// Copyright (C) 2015-2026 The Neo Project.
//
// Program.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Compiler.Fuzzing;
using System.Diagnostics;
using System.Text;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Command line entry point of the compiler fuzzing and differential analysis harness.
/// </summary>
public static class Program
{
    /// <summary>
    /// Runs the harness.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>Process exit code: 0 when no defect was found, 1 otherwise.</returns>
    public static int Main(string[] args)
    {
        var options = FuzzOptions.Parse(args);

        if (options.ShowHelp)
        {
            Console.WriteLine(FuzzOptions.HelpText);
            return 0;
        }

        if (options.AbiProbe)
        {
            using var probeHarness = new CompilerHarness();
            AbiProbe.Run(probeHarness);
            return 0;
        }

        if (options.GenerateArtifact is { } contractName)
        {
            string root = options.RepositoryRoot ?? FindRepositoryRoot();
            return ArtifactGenerator.Generate(root, contractName);
        }

        if (options.StorageMapDisassembly)
        {
            return StorageMapDisassembly.Run();
        }

        if (options.StructProbe)
        {
            return StructProbe.Run();
        }

        if (options.StructMatrixProbe)
        {
            return StructMatrixProbe.Run();
        }

        if (options.TupleShapeProbe)
        {
            return TupleShapeProbe.Run();
        }

        if (options.SemanticMatrixProbe)
        {
            return SemanticMatrixProbe.Run();
        }

        if (options.EventProbe)
        {
            return EventProbe.Run();
        }

        if (options.CrossContractStructProbe)
        {
            return CrossContractStructProbe.Run() == 0 ? 0 : 1;
        }

        if (options.CryptoProbe)
        {
            return CryptoProbe.Run() == 0 ? 0 : 1;
        }

        if (options.OperatorOverloadProbe)
        {
            return OperatorOverloadProbe.Run() == 0 ? 0 : 1;
        }

        if (options.ControlFlowProbe)
        {
            return ControlFlowProbe.Run() == 0 ? 0 : 1;
        }

        if (options.FamilyCoverageProbe)
        {
            return FamilyCoverageProbe.Run() == 0 ? 0 : 1;
        }

        if (options.StorageMapProbe)
        {
            return StorageMapProbe.Run() == 0 ? 0 : 1;
        }

        if (options.MultiContractProbe)
        {
            return MultiContractProbe.Run(options.Seed) == 0 ? 0 : 1;
        }

        if (options.ReferenceCoverage)
        {
            return ReferenceCoverageProbe.Run(options.Seed) == 0 ? 0 : 3;
        }

        if (options.InitializeAbiProbe)
        {
            InitializeAbiProbe.Run();
            return 0;
        }

        if (options.SelfTest)
        {
            using var selfTestHarness = new CompilerHarness();
            return OracleSelfTest.Run(selfTestHarness) == 0 ? 0 : 3;
        }

        if (options.BigIntegerRepro)
        {
            using var reproHarness = new CompilerHarness();
            BigIntegerRepro.Run(reproHarness);
            return 0;
        }

        if (options.RejectionProbe)
        {
            using var probeHarness = new CompilerHarness();
            RejectionProbe.Run(probeHarness, new SourceGenerator(options.Seed));
            return 0;
        }

        Console.WriteLine("Neo compiler fuzzing & differential analysis harness");
        Console.WriteLine($"  seed       : {options.Seed}");
        Console.WriteLine($"  iterations : {options.Iterations}");
        Console.WriteLine($"  timeout    : {(options.Timeout is { } t ? t.ToString() : "none")}");
        Console.WriteLine();

        using var cancellation = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cancellation.Cancel();
        };

        using var runner = new FuzzRunner(
            options.Seed,
            coverageGuided: !options.NoCoverageGuidance,
            corpusDirectory: options.CorpusDirectory);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            runner.Run(
                options.Iterations,
                options.Timeout,
                cancellation.Token,
                progress => ReportProgress(progress, stopwatch));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Harness failure: {ex}");
            return 2;
        }

        stopwatch.Stop();
        Console.WriteLine();
        Console.WriteLine($"Completed {runner.Iterations} iterations in {stopwatch.Elapsed.TotalSeconds:F1}s " +
                          $"(compiled={runner.Compiled}, rejected={runner.Rejected}).");
        Console.WriteLine($"Coverage: {runner.CoveredInstructions} distinct instructions reached, " +
                          $"corpus={runner.CorpusSize} entries. " +
                          "(A flat instruction count means the search has saturated.)");
        Console.WriteLine($"Multi-contract iterations: {runner.MultiContractIterations}. " +
                          "(Zero means the cross-contract path never ran.)");

        if (runner.SuppressedKnownDefects > 0)
        {
            Console.WriteLine(
                $"Suppressed {runner.SuppressedKnownDefects} finding(s) that match an already-reported defect. " +
                "Only new failure signatures are reported.");
        }

        if (runner.RestoredCorpusSize > 0)
        {
            Console.WriteLine($"Corpus: restored {runner.RestoredCorpusSize} entries " +
                              $"and {runner.RestoredCoverage} coverage offsets from the previous campaign.");
        }

        int stored = runner.SaveCorpus();
        if (options.CorpusDirectory is not null)
        {
            Console.WriteLine($"Corpus: wrote {stored} new entr{(stored == 1 ? "y" : "ies")} to {options.CorpusDirectory}.");
        }

        if (runner.StoppedOnSlowExecution)
        {
            Console.WriteLine("The campaign stopped early: an iteration exceeded the per-iteration budget. " +
                              "The offending input is reported as a SlowExecution finding.");
        }

        return WriteFindings(runner.Findings, options.OutputDirectory);
    }

    /// <summary>
    /// Locates the repository root by walking up from the current directory until the
    /// solution file is found.
    /// </summary>
    /// <returns>The repository root.</returns>
    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "neo-devpack-dotnet.sln")))
            {
                return directory.FullName;
            }
            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            "Could not locate the repository root. Pass --repo-root explicitly.");
    }

    private static void ReportProgress(FuzzRunner runner, Stopwatch stopwatch)
    {
        double rate = stopwatch.Elapsed.TotalSeconds > 0 ? runner.Iterations / stopwatch.Elapsed.TotalSeconds : 0;
        Console.WriteLine(
            $"[{runner.Iterations,7} it] {rate,8:F1}/s  compiled={runner.Compiled,6}  rejected={runner.Rejected,6}  " +
            $"corpus={runner.CorpusSize,4}  instr={runner.CoveredInstructions,6}  " +
            $"multi={runner.MultiContractIterations,4}  findings={runner.Findings.Count,3}");
    }

    private static int WriteFindings(IReadOnlyList<FuzzFinding> findings, string? outputDirectory)
    {
        if (findings.Count == 0)
        {
            // Remove the previous campaign's artifacts; a stale findings.json next to a
            // fresh "No defects found" run reads as a live finding on the next triage.
            if (!string.IsNullOrWhiteSpace(outputDirectory) && Directory.Exists(outputDirectory))
            {
                foreach (string stale in Directory.EnumerateFiles(outputDirectory, "findings.json")
                    .Concat(Directory.EnumerateFiles(outputDirectory, "repro-*.cs")))
                {
                    File.Delete(stale);
                }
            }
            Console.WriteLine();
            Console.WriteLine("No defects found.");
            return 0;
        }

        Console.WriteLine();
        Console.WriteLine($"=== {findings.Count} distinct finding(s) ===");

        var grouped = findings
            .GroupBy(f => f.Outcome)
            .OrderByDescending(g => g.Count());

        foreach (var group in grouped)
        {
            Console.WriteLine();
            Console.WriteLine($"[{group.Key}] x{group.Count()}");

            foreach (FuzzFinding finding in group.Take(5))
            {
                Console.WriteLine($"  - {finding.Detail}");
                if (finding.StackTrace is { } trace)
                {
                    foreach (string line in trace.Split('\n').Take(12))
                    {
                        Console.WriteLine($"      {line.TrimEnd()}");
                    }
                }
            }

            if (group.Count() > 5)
            {
                Console.WriteLine($"  ... and {group.Count() - 5} more");
            }
        }

        if (!string.IsNullOrWhiteSpace(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
            string path = Path.Combine(outputDirectory, "findings.json");
            File.WriteAllText(path, BuildJson(findings), new UTF8Encoding(false));
            Console.WriteLine();
            Console.WriteLine($"Findings written to {path}");

            // Write each finding's reproducer as a standalone .cs file so a human can
            // paste it straight into a test.
            for (int i = 0; i < findings.Count; i++)
            {
                string file = Path.Combine(outputDirectory, $"repro-{i:D3}-{findings[i].Outcome}.cs");
                File.WriteAllText(file, findings[i].Source, new UTF8Encoding(false));
            }
        }

        return 1;
    }

    private static string BuildJson(IReadOnlyList<FuzzFinding> findings)
    {
        var sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine("  \"findings\": [");
        for (int i = 0; i < findings.Count; i++)
        {
            FuzzFinding f = findings[i];
            sb.AppendLine("    {");
            sb.AppendLine($"      \"outcome\": {Json(f.Outcome.ToString())},");
            sb.AppendLine($"      \"detail\": {Json(f.Detail)},");
            sb.AppendLine($"      \"exception\": {Json(f.Exception ?? string.Empty)},");
            sb.AppendLine($"      \"source\": {Json(f.Source)}");
            sb.AppendLine(i == findings.Count - 1 ? "    }" : "    },");
        }
        sb.AppendLine("  ]");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string Json(string value)
    {
        var sb = new StringBuilder("\"");
        foreach (char c in value)
        {
            switch (c)
            {
                case '"': sb.Append("\\\""); break;
                case '\\': sb.Append("\\\\"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default:
                    if (c < 0x20)
                    {
                        sb.Append("\\u").Append(((int)c).ToString("x4"));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                    break;
            }
        }
        sb.Append('"');
        return sb.ToString();
    }
}

/// <summary>
/// Parsed command line options.
/// </summary>
public sealed record FuzzOptions
{
    /// <summary>Random seed.</summary>
    public int Seed { get; init; } = 20261021;

    /// <summary>Number of iterations to run.</summary>
    public int Iterations { get; init; } = 500;

    /// <summary>Optional wall-clock budget.</summary>
    public TimeSpan? Timeout { get; init; }

    /// <summary>Directory that receives the findings report and reproducers.</summary>
    public string? OutputDirectory { get; init; }

    /// <summary>Whether the help text was requested.</summary>
    public bool ShowHelp { get; init; }

    /// <summary>Whether to run the ABI probe instead of the fuzz loop.</summary>
    public bool AbiProbe { get; init; }

    /// <summary>Whether to run the rejection probe instead of the fuzz loop.</summary>
    public bool RejectionProbe { get; init; }

    /// <summary>Whether to run the BigInteger reproduction probe.</summary>
    public bool BigIntegerRepro { get; init; }

    /// <summary>Whether to run the oracle self-test.</summary>
    public bool SelfTest { get; init; }

    /// <summary>Whether to run the _initialize ABI probe.</summary>
    public bool InitializeAbiProbe { get; init; }

    /// <summary>Whether to run the reference coverage probe.</summary>
    public bool ReferenceCoverage { get; init; }

    /// <summary>The contract to generate a testing artifact for.</summary>
    public string? GenerateArtifact { get; init; }

    /// <summary>The repository root used by the artifact generator.</summary>
    public string? RepositoryRoot { get; init; }

    /// <summary>Whether to disable coverage-guided corpus selection.</summary>
    public bool NoCoverageGuidance { get; init; }

    /// <summary>Directory used to persist the corpus across runs.</summary>
    public string? CorpusDirectory { get; init; }

    /// <summary>Whether to run the multi-contract probe.</summary>
    public bool MultiContractProbe { get; init; }

    /// <summary>Whether to run the StorageMap probe.</summary>
    public bool StorageMapProbe { get; init; }

    /// <summary>Whether to run the struct probe.</summary>
    public bool StructProbe { get; init; }

    /// <summary>Whether to run the struct matrix probe.</summary>
    public bool StructMatrixProbe { get; init; }

    /// <summary>Whether to run the tuple shape probe.</summary>
    public bool TupleShapeProbe { get; init; }

    /// <summary>Whether to run the semantic matrix probe.</summary>
    public bool SemanticMatrixProbe { get; init; }

    /// <summary>Whether to run the event probe.</summary>
    public bool EventProbe { get; init; }

    /// <summary>Whether to run the cross-contract struct probe.</summary>
    public bool CrossContractStructProbe { get; init; }

    /// <summary>Whether to run the crypto probe.</summary>
    public bool CryptoProbe { get; init; }

    /// <summary>Whether to run the user-defined operator probe.</summary>
    public bool OperatorOverloadProbe { get; init; }

    /// <summary>Whether to run the control-flow interaction probe.</summary>
    public bool ControlFlowProbe { get; init; }

    /// <summary>Whether to run the family coverage probe.</summary>
    public bool FamilyCoverageProbe { get; init; }

    /// <summary>Whether to print the StorageMap disassembly.</summary>
    public bool StorageMapDisassembly { get; init; }

    /// <summary>Usage text.</summary>
    public static string HelpText =>
        """
        neofuzz - Neo compiler fuzzing & differential analysis harness

        Usage:
          neofuzz [options]

        Options:
          --seed <n>          Random seed (default 20261021)
          --iterations <n>    Number of fuzz iterations (default 500)
          --timeout <sec>     Wall-clock budget in seconds
          --output <dir>      Directory for the findings report and reproducers
          --help              Show this help

        Exit codes:
          0  no defect found
          1  at least one defect found
          2  harness failure
        """;

    /// <summary>
    /// Parses command line arguments.
    /// </summary>
    /// <param name="args">The raw arguments.</param>
    /// <returns>The parsed options.</returns>
    public static FuzzOptions Parse(string[] args)
    {
        int seed = 20261021;
        int iterations = 500;
        TimeSpan? timeout = null;
        string? output = null;
        bool help = false;
        bool abiProbe = false;
        bool rejectionProbe = false;
        bool bigIntegerRepro = false;
        bool selfTest = false;
        bool initAbiProbe = false;
        bool referenceCoverage = false;
        string? generateArtifact = null;
        string? repoRoot = null;
        bool noCoverageGuidance = false;
        string? corpusDirectory = null;
        bool multiContractProbe = false;
        bool storageMapProbe = false;
        bool structProbe = false;
        bool structMatrixProbe = false;
        bool tupleShapeProbe = false;
        bool semanticMatrixProbe = false;
        bool eventProbe = false;
        bool crossContractStructProbe = false;
        bool cryptoProbe = false;
        bool operatorOverloadProbe = false;
        bool controlFlowProbe = false;
        bool familyCoverageProbe = false;
        bool storageMapDisassembly = false;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--seed":
                    seed = int.Parse(args[++i]);
                    break;
                case "--iterations":
                    iterations = int.Parse(args[++i]);
                    break;
                case "--timeout":
                    timeout = TimeSpan.FromSeconds(double.Parse(args[++i]));
                    break;
                case "--output":
                    output = args[++i];
                    break;
                case "--abi-probe":
                    abiProbe = true;
                    break;
                case "--rejection-probe":
                    rejectionProbe = true;
                    break;
                case "--biginteger-repro":
                    bigIntegerRepro = true;
                    break;
                case "--self-test":
                    selfTest = true;
                    break;
                case "--init-abi-probe":
                    initAbiProbe = true;
                    break;
                case "--reference-coverage":
                    referenceCoverage = true;
                    break;
                case "--generate-artifact":
                    generateArtifact = args[++i];
                    break;
                case "--repo-root":
                    repoRoot = args[++i];
                    break;
                case "--no-coverage-guidance":
                    noCoverageGuidance = true;
                    break;
                case "--corpus":
                    corpusDirectory = args[++i];
                    break;
                case "--multi-contract-probe":
                    multiContractProbe = true;
                    break;
                case "--storage-map-probe":
                    storageMapProbe = true;
                    break;
                case "--struct-probe":
                    structProbe = true;
                    break;
                case "--struct-matrix":
                    structMatrixProbe = true;
                    break;
                case "--tuple-shape":
                    tupleShapeProbe = true;
                    break;
                case "--semantic-matrix":
                    semanticMatrixProbe = true;
                    break;
                case "--event-probe":
                    eventProbe = true;
                    break;
                case "--cross-struct":
                    crossContractStructProbe = true;
                    break;
                case "--crypto-probe":
                    cryptoProbe = true;
                    break;
                case "--operator-probe":
                    operatorOverloadProbe = true;
                    break;
                case "--controlflow-probe":
                    controlFlowProbe = true;
                    break;
                case "--family-coverage":
                    familyCoverageProbe = true;
                    break;
                case "--storage-map-disasm":
                    storageMapDisassembly = true;
                    break;
                case "--help":
                case "-h":
                    help = true;
                    break;
            }
        }

        return new FuzzOptions
        {
            Seed = seed,
            Iterations = iterations,
            Timeout = timeout,
            OutputDirectory = output,
            ShowHelp = help,
            AbiProbe = abiProbe,
            RejectionProbe = rejectionProbe,
            BigIntegerRepro = bigIntegerRepro,
            SelfTest = selfTest,
            InitializeAbiProbe = initAbiProbe,
            ReferenceCoverage = referenceCoverage,
            GenerateArtifact = generateArtifact,
            RepositoryRoot = repoRoot,
            NoCoverageGuidance = noCoverageGuidance,
            CorpusDirectory = corpusDirectory,
            MultiContractProbe = multiContractProbe,
            StorageMapProbe = storageMapProbe,
            StorageMapDisassembly = storageMapDisassembly,
            StructProbe = structProbe,
            StructMatrixProbe = structMatrixProbe,
            TupleShapeProbe = tupleShapeProbe,
            SemanticMatrixProbe = semanticMatrixProbe,
            EventProbe = eventProbe,
            CrossContractStructProbe = crossContractStructProbe,
            CryptoProbe = cryptoProbe,
            OperatorOverloadProbe = operatorOverloadProbe,
            ControlFlowProbe = controlFlowProbe,
            FamilyCoverageProbe = familyCoverageProbe,
        };
    }
}
