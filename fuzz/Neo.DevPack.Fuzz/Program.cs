using Neo.DevPack.Fuzz.Targets;

namespace Neo.DevPack.Fuzz;

internal static class Program
{
    private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

    private static int Main(string[] args)
    {
        try
        {
            var layout = RepoLayout.Discover();
            var targets = CreateTargets(layout);

            if (args.Length == 0 || IsHelp(args[0]))
            {
                PrintUsage(targets);
                return 1;
            }

            return args[0].ToLowerInvariant() switch
            {
                "list" => ListTargets(targets),
                "run" => RunTarget(args, layout, targets),
                "repro" => ReproTarget(args, layout, targets),
                _ => Fail($"Unknown command '{args[0]}'.")
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static IReadOnlyDictionary<string, IFuzzTarget> CreateTargets(RepoLayout layout)
    {
        var targets = new IFuzzTarget[]
        {
            new CompilerTextTarget(layout),
            new StructuredCompileTarget(layout),
            new TemplateProjectTarget(layout),
            new DeterminismTarget(layout),
            new DevpackRuntimeTarget(layout)
        };

        return targets.ToDictionary(target => target.Name, target => target, Comparer);
    }

    private static int ListTargets(IReadOnlyDictionary<string, IFuzzTarget> targets)
    {
        foreach (var target in targets.Values.OrderBy(target => target.Name, Comparer))
        {
            Console.WriteLine($"{target.Name}: {target.Description}");
        }

        return 0;
    }

    private static int RunTarget(string[] args, RepoLayout layout, IReadOnlyDictionary<string, IFuzzTarget> targets)
    {
        if (args.Length < 2)
        {
            return Fail("run requires a target name.");
        }

        if (!targets.TryGetValue(args[1], out var target))
        {
            return Fail($"Unknown target '{args[1]}'.");
        }

        var options = CreateDefaultRunOptions(layout, target.Name);
        ApplyRunOptions(options, args.Skip(2).ToArray());

        return new FuzzRunner(target, options).Run();
    }

    private static int ReproTarget(string[] args, RepoLayout layout, IReadOnlyDictionary<string, IFuzzTarget> targets)
    {
        if (args.Length < 3)
        {
            return Fail("repro requires a target name and an input file.");
        }

        if (!targets.TryGetValue(args[1], out var target))
        {
            return Fail($"Unknown target '{args[1]}'.");
        }

        var inputPath = Path.GetFullPath(args[2]);
        if (!File.Exists(inputPath))
        {
            return Fail($"Input file '{inputPath}' does not exist.");
        }

        var options = CreateDefaultRunOptions(layout, target.Name);
        ApplyRunOptions(options, args.Skip(3).ToArray());

        return new FuzzRunner(target, options).Repro(File.ReadAllBytes(inputPath));
    }

    private static RunOptions CreateDefaultRunOptions(RepoLayout layout, string targetName)
    {
        return new RunOptions
        {
            CorpusDirectory = Path.Combine(layout.FuzzDirectory, "corpus", targetName),
            ArtifactDirectory = Path.Combine(layout.FuzzDirectory, "artifacts", targetName),
            FingerprintStorePath = Path.Combine(layout.FuzzDirectory, "state", "fingerprints", $"{targetName}.txt"),
            CrashFingerprintStorePath = Path.Combine(layout.FuzzDirectory, "state", "crashes", $"{targetName}.txt"),
            SeedDirectory = Path.Combine(layout.FuzzDirectory, "seeds"),
            DictionaryPath = Path.Combine(layout.FuzzDirectory, "dotnet.dict"),
            MaxInputSize = 16 * 1024,
            MaxCorpusEntriesInMemory = 4 * 1024,
            MaxCorpusFilesOnDisk = 20 * 1024,
            MaxTotalTime = TimeSpan.FromHours(24),
            Iterations = 0,
            StatusInterval = TimeSpan.FromSeconds(30)
        };
    }

    private static void ApplyRunOptions(RunOptions options, string[] args)
    {
        for (var index = 0; index < args.Length; index++)
        {
            var option = args[index];
            var value = index + 1 < args.Length ? args[index + 1] : null;

            switch (option)
            {
                case "--corpus-dir":
                    options.CorpusDirectory = RequireValue(option, value);
                    index++;
                    break;
                case "--artifacts-dir":
                    options.ArtifactDirectory = RequireValue(option, value);
                    index++;
                    break;
                case "--seed-dir":
                    options.SeedDirectory = RequireValue(option, value);
                    index++;
                    break;
                case "--dictionary":
                    options.DictionaryPath = RequireValue(option, value);
                    index++;
                    break;
                case "--iterations":
                    options.Iterations = int.Parse(RequireValue(option, value));
                    index++;
                    break;
                case "--max-total-time-seconds":
                    options.MaxTotalTime = TimeSpan.FromSeconds(int.Parse(RequireValue(option, value)));
                    index++;
                    break;
                case "--max-input-size":
                    options.MaxInputSize = int.Parse(RequireValue(option, value));
                    index++;
                    break;
                case "--max-corpus-in-memory":
                    options.MaxCorpusEntriesInMemory = int.Parse(RequireValue(option, value));
                    index++;
                    break;
                case "--max-corpus-files":
                    options.MaxCorpusFilesOnDisk = int.Parse(RequireValue(option, value));
                    index++;
                    break;
                case "--status-interval-seconds":
                    options.StatusInterval = TimeSpan.FromSeconds(int.Parse(RequireValue(option, value)));
                    index++;
                    break;
                case "--random-seed":
                    options.RandomSeed = int.Parse(RequireValue(option, value));
                    index++;
                    break;
                default:
                    throw new ArgumentException($"Unknown option '{option}'.");
            }
        }
    }

    private static string RequireValue(string option, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{option} requires a value.");
        }

        return value;
    }

    private static void PrintUsage(IReadOnlyDictionary<string, IFuzzTarget> targets)
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project fuzz/Neo.DevPack.Fuzz/Neo.DevPack.Fuzz.csproj -- list");
        Console.WriteLine("  dotnet run --project fuzz/Neo.DevPack.Fuzz/Neo.DevPack.Fuzz.csproj -- run <target> [options]");
        Console.WriteLine("  dotnet run --project fuzz/Neo.DevPack.Fuzz/Neo.DevPack.Fuzz.csproj -- repro <target> <input-file> [options]");
        Console.WriteLine();
        Console.WriteLine("Targets:");

        foreach (var target in targets.Values.OrderBy(target => target.Name, Comparer))
        {
            Console.WriteLine($"  {target.Name,-24} {target.Description}");
        }
    }

    private static bool IsHelp(string value) =>
        value.Equals("-h", StringComparison.OrdinalIgnoreCase)
        || value.Equals("--help", StringComparison.OrdinalIgnoreCase)
        || value.Equals("help", StringComparison.OrdinalIgnoreCase);

    private static int Fail(string message)
    {
        Console.Error.WriteLine(message);
        return 1;
    }
}
