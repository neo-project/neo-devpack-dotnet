using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Neo.DevPack.Fuzz;

internal sealed record RepoLayout(
    string RootDirectory,
    string FuzzDirectory,
    string CompilerProjectPath,
    string FrameworkProjectPath)
{
    public static RepoLayout Discover()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var solutionPath = Path.Combine(directory.FullName, "neo-devpack-dotnet.sln");
            if (File.Exists(solutionPath))
            {
                return new RepoLayout(
                    directory.FullName,
                    Path.Combine(directory.FullName, "fuzz"),
                    Path.Combine(directory.FullName, "src", "Neo.Compiler.CSharp", "Neo.Compiler.CSharp.csproj"),
                    Path.Combine(directory.FullName, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj"));
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate neo-devpack-dotnet from the current executable path.");
    }
}

internal sealed class RunOptions
{
    public required string CorpusDirectory { get; set; }

    public required string ArtifactDirectory { get; set; }

    public required string FingerprintStorePath { get; set; }

    public required string CrashFingerprintStorePath { get; set; }

    public required string SeedDirectory { get; set; }

    public required string DictionaryPath { get; set; }

    public required TimeSpan MaxTotalTime { get; set; }

    public required TimeSpan StatusInterval { get; set; }

    public int Iterations { get; set; }

    public int MaxInputSize { get; set; }

    public int MaxCorpusEntriesInMemory { get; set; }

    public int MaxCorpusFilesOnDisk { get; set; }

    public int? RandomSeed { get; set; }
}

internal interface IFuzzTarget
{
    string Name { get; }

    string Description { get; }

    FuzzCaseResult Execute(byte[] input);
}

internal sealed record FuzzCaseResult(
    string Fingerprint,
    string Summary,
    bool SaveInputToCorpus = true,
    IReadOnlyDictionary<string, string>? TextArtifacts = null,
    IReadOnlyDictionary<string, byte[]>? BinaryArtifacts = null);

internal sealed class FuzzCrashException : Exception
{
    public FuzzCrashException(
        string summary,
        Exception innerException,
        IReadOnlyDictionary<string, string>? textArtifacts = null,
        IReadOnlyDictionary<string, byte[]>? binaryArtifacts = null)
        : base(summary, innerException)
    {
        Summary = summary;
        TextArtifacts = textArtifacts ?? new Dictionary<string, string>(StringComparer.Ordinal);
        BinaryArtifacts = binaryArtifacts ?? new Dictionary<string, byte[]>(StringComparer.Ordinal);
    }

    public string Summary { get; }

    public IReadOnlyDictionary<string, string> TextArtifacts { get; }

    public IReadOnlyDictionary<string, byte[]> BinaryArtifacts { get; }
}

internal sealed record InvocationPlan(string MethodName, object?[] Arguments);

internal sealed record GeneratedSourceProject(
    string ContractName,
    IReadOnlyDictionary<string, string> Files,
    IReadOnlyList<InvocationPlan> InvocationPlans)
{
    public static GeneratedSourceProject SingleFile(string contractName, string fileName, string sourceText)
    {
        return new GeneratedSourceProject(
            contractName,
            new Dictionary<string, string>(StringComparer.Ordinal) { [fileName] = sourceText },
            Array.Empty<InvocationPlan>());
    }
}

internal sealed record CompilationSnapshot(
    string ContractName,
    byte[] NefBytes,
    string ManifestJson,
    string DebugJson,
    string AssemblyText,
    string InterfaceCode,
    string ArtifactSource)
{
    public string Fingerprint => string.Join(
        ":",
        ContractName,
        Deterministic.HashHex(NefBytes),
        Deterministic.HashHex(Encoding.UTF8.GetBytes(ManifestJson)),
        Deterministic.HashHex(Encoding.UTF8.GetBytes(DebugJson)),
        Deterministic.HashHex(Encoding.UTF8.GetBytes(AssemblyText)),
        Deterministic.HashHex(Encoding.UTF8.GetBytes(InterfaceCode)),
        Deterministic.HashHex(Encoding.UTF8.GetBytes(ArtifactSource)));
}

internal sealed class SourceWorkspace
{
    private readonly string _rootDirectory;

    public SourceWorkspace(string name)
    {
        _rootDirectory = Path.Combine(Path.GetTempPath(), "Neo.Compiler", "Fuzz", name, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_rootDirectory);
    }

    public string RootDirectory => _rootDirectory;

    public string[] WriteFiles(IReadOnlyDictionary<string, string> files)
    {
        if (Directory.Exists(_rootDirectory))
        {
            foreach (var entry in Directory.EnumerateFileSystemEntries(_rootDirectory))
            {
                if (Directory.Exists(entry))
                {
                    Directory.Delete(entry, true);
                }
                else
                {
                    File.Delete(entry);
                }
            }
        }

        var paths = new List<string>(files.Count);
        foreach (var (relativePath, content) in files.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            var absolutePath = Path.Combine(_rootDirectory, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
            File.WriteAllText(absolutePath, content);
            paths.Add(absolutePath);
        }

        return paths.ToArray();
    }
}

internal sealed class CorpusStore
{
    private readonly string _directory;
    private long? _fileCount;

    public CorpusStore(string directory)
    {
        _directory = directory;
        Directory.CreateDirectory(_directory);
    }

    public void SeedFrom(params string[] directories)
    {
        if (EnumerateCorpusFiles().Any())
        {
            return;
        }

        foreach (var directory in directories.Where(Directory.Exists))
        {
            foreach (var file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
            {
                TryAdd(File.ReadAllBytes(file));
            }
        }
    }

    public List<byte[]> LoadAll()
    {
        return LoadSample(int.MaxValue, new Random()).Entries.ToList();
    }

    public bool TryAdd(byte[] input)
    {
        return TryAddCore(input, maxFilesOnDisk: null);
    }

    public bool TryAdd(byte[] input, int maxFilesOnDisk)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxFilesOnDisk);
        return TryAddCore(input, maxFilesOnDisk);
    }

    public long FileCount => _fileCount ??= EnumerateCorpusFiles().LongCount();

    public CorpusLoadResult LoadSample(int maxEntries, Random random)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxEntries);

        var samplePaths = new List<string>(Math.Min(maxEntries, 1024));
        long fileCount = 0;
        foreach (var path in EnumerateCorpusFiles())
        {
            fileCount++;
            if (samplePaths.Count < maxEntries)
            {
                samplePaths.Add(path);
                continue;
            }

            if (maxEntries == 0)
            {
                continue;
            }

            var replacementIndex = random.NextInt64(fileCount);
            if (replacementIndex < maxEntries)
            {
                samplePaths[(int)replacementIndex] = path;
            }
        }

        _fileCount = fileCount;

        var entries = samplePaths
            .OrderBy(static path => path, StringComparer.Ordinal)
            .Select(File.ReadAllBytes)
            .ToList();
        return new CorpusLoadResult(entries, fileCount);
    }

    private bool TryAddCore(byte[] input, int? maxFilesOnDisk)
    {
        var fileName = $"{Deterministic.HashHex(input)}.bin";
        var path = Path.Combine(_directory, fileName);
        if (File.Exists(path))
        {
            return false;
        }

        if (maxFilesOnDisk.HasValue)
        {
            _fileCount ??= EnumerateCorpusFiles().LongCount();
            if (_fileCount.Value >= maxFilesOnDisk.Value)
            {
                return false;
            }
        }

        File.WriteAllBytes(path, input);
        if (_fileCount.HasValue)
        {
            _fileCount++;
        }
        return true;
    }

    private IEnumerable<string> EnumerateCorpusFiles() =>
        Directory.EnumerateFiles(_directory, "*", SearchOption.AllDirectories);
}

internal sealed record CorpusLoadResult(IReadOnlyList<byte[]> Entries, long FileCount);

internal sealed class CorpusPool
{
    private readonly List<byte[]> _entries;
    private readonly int _maxEntries;

    public CorpusPool(IEnumerable<byte[]> entries, int maxEntries)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxEntries);

        _maxEntries = maxEntries;
        _entries = entries
            .Take(maxEntries)
            .Select(static entry => entry.ToArray())
            .ToList();
    }

    public IReadOnlyList<byte[]> Entries => _entries;

    public int Count => _entries.Count;

    public void Remember(byte[] input, Random random)
    {
        if (_maxEntries == 0)
        {
            return;
        }

        var copy = input.ToArray();
        if (_entries.Count < _maxEntries)
        {
            _entries.Add(copy);
            return;
        }

        _entries[random.Next(_entries.Count)] = copy;
    }
}

internal sealed class InputMutator
{
    private readonly IReadOnlyList<byte[]> _dictionaryEntries;

    public InputMutator(IReadOnlyList<byte[]> dictionaryEntries)
    {
        _dictionaryEntries = dictionaryEntries;
    }

    public static IReadOnlyList<byte[]> LoadDictionary(string path)
    {
        if (!File.Exists(path))
        {
            return Array.Empty<byte[]>();
        }

        return File.ReadAllLines(path)
            .Select(static line => line.Trim())
            .Where(static line => line.Length > 0 && !line.StartsWith('#'))
            .Select(static line =>
            {
                if (line.Length >= 2 && line[0] == '"' && line[^1] == '"')
                {
                    line = line[1..^1];
                }

                return Encoding.UTF8.GetBytes(line);
            })
            .ToArray();
    }

    public byte[] CreateCandidate(IReadOnlyList<byte[]> corpus, Random random, int maxInputSize)
    {
        byte[] candidate;
        if (corpus.Count == 0 || random.Next(10) == 0)
        {
            candidate = CreateRandomBytes(random, random.Next(1, Math.Max(2, Math.Min(maxInputSize, 128))));
        }
        else
        {
            candidate = corpus[random.Next(corpus.Count)].ToArray();
        }

        var mutationCount = 1 + random.Next(8);
        for (var index = 0; index < mutationCount; index++)
        {
            candidate = MutateOnce(candidate, corpus, random, maxInputSize);
        }

        if (candidate.Length == 0)
        {
            candidate = [0];
        }

        if (candidate.Length > maxInputSize)
        {
            Array.Resize(ref candidate, maxInputSize);
        }

        return candidate;
    }

    private byte[] MutateOnce(byte[] input, IReadOnlyList<byte[]> corpus, Random random, int maxInputSize)
    {
        return random.Next(7) switch
        {
            0 => FlipBit(input, random),
            1 => ChangeByte(input, random),
            2 => DeleteRange(input, random),
            3 => InsertBytes(input, PickInsertion(random), random, maxInputSize),
            4 => DuplicateRange(input, random, maxInputSize),
            5 => SpliceInput(input, corpus, random, maxInputSize),
            _ => ShuffleRange(input, random)
        };
    }

    private byte[] PickInsertion(Random random)
    {
        if (_dictionaryEntries.Count > 0 && random.Next(3) == 0)
        {
            return _dictionaryEntries[random.Next(_dictionaryEntries.Count)];
        }

        return CreateRandomBytes(random, 1 + random.Next(16));
    }

    private static byte[] FlipBit(byte[] input, Random random)
    {
        var copy = input.ToArray();
        if (copy.Length == 0)
        {
            return CreateRandomBytes(random, 1);
        }

        var index = random.Next(copy.Length);
        copy[index] ^= (byte)(1 << random.Next(8));
        return copy;
    }

    private static byte[] ChangeByte(byte[] input, Random random)
    {
        var copy = input.ToArray();
        if (copy.Length == 0)
        {
            return CreateRandomBytes(random, 1);
        }

        copy[random.Next(copy.Length)] = (byte)random.Next(256);
        return copy;
    }

    private static byte[] DeleteRange(byte[] input, Random random)
    {
        if (input.Length <= 1)
        {
            return input.ToArray();
        }

        var start = random.Next(input.Length);
        var length = 1 + random.Next(input.Length - start);
        return input[..start].Concat(input[(start + length)..]).ToArray();
    }

    private static byte[] InsertBytes(byte[] input, byte[] bytes, Random random, int maxInputSize)
    {
        if (bytes.Length == 0)
        {
            return input.ToArray();
        }

        var offset = random.Next(input.Length + 1);
        var result = input[..offset].Concat(bytes).Concat(input[offset..]).ToArray();
        if (result.Length > maxInputSize)
        {
            Array.Resize(ref result, maxInputSize);
        }

        return result;
    }

    private static byte[] DuplicateRange(byte[] input, Random random, int maxInputSize)
    {
        if (input.Length == 0)
        {
            return CreateRandomBytes(random, 1);
        }

        var start = random.Next(input.Length);
        var length = 1 + random.Next(input.Length - start);
        return InsertBytes(input, input[start..(start + length)], random, maxInputSize);
    }

    private static byte[] SpliceInput(byte[] input, IReadOnlyList<byte[]> corpus, Random random, int maxInputSize)
    {
        if (corpus.Count == 0)
        {
            return DuplicateRange(input, random, maxInputSize);
        }

        var other = corpus[random.Next(corpus.Count)];
        if (input.Length == 0)
        {
            return other.Take(maxInputSize).ToArray();
        }

        var left = input[..random.Next(input.Length + 1)];
        var right = other[random.Next(other.Length + 1)..];
        var result = left.Concat(right).ToArray();
        if (result.Length > maxInputSize)
        {
            Array.Resize(ref result, maxInputSize);
        }

        return result;
    }

    private static byte[] ShuffleRange(byte[] input, Random random)
    {
        if (input.Length < 3)
        {
            return ChangeByte(input, random);
        }

        var copy = input.ToArray();
        var start = random.Next(copy.Length - 1);
        var length = 2 + random.Next(copy.Length - start - 1);
        var slice = copy[start..(start + length)];
        random.Shuffle(slice);
        slice.CopyTo(copy, start);
        return copy;
    }

    private static byte[] CreateRandomBytes(Random random, int length)
    {
        var bytes = new byte[length];
        random.NextBytes(bytes);
        return bytes;
    }
}

internal sealed class ArtifactWriter
{
    private readonly string _directory;

    public ArtifactWriter(string directory)
    {
        _directory = directory;
        Directory.CreateDirectory(_directory);
    }

    public string WriteCrash(string targetName, byte[] input, Exception exception, string summary, IReadOnlyDictionary<string, string>? textArtifacts = null, IReadOnlyDictionary<string, byte[]>? binaryArtifacts = null)
    {
        var folderName = $"crash-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Deterministic.HashHex(input)[..12]}";
        var path = Path.Combine(_directory, folderName);
        Directory.CreateDirectory(path);

        File.WriteAllBytes(Path.Combine(path, "input.bin"), input);
        File.WriteAllText(Path.Combine(path, "summary.txt"), BuildSummary(targetName, summary, exception));

        if (textArtifacts is not null)
        {
            foreach (var (name, value) in textArtifacts)
            {
                File.WriteAllText(Path.Combine(path, SanitizeFileName(name)), value);
            }
        }

        if (binaryArtifacts is not null)
        {
            foreach (var (name, value) in binaryArtifacts)
            {
                File.WriteAllBytes(Path.Combine(path, SanitizeFileName(name)), value);
            }
        }

        return path;
    }

    private static string BuildSummary(string targetName, string summary, Exception exception)
    {
        return string.Join(
            Environment.NewLine,
            $"target: {targetName}",
            $"time:   {DateTime.UtcNow:O}",
            string.Empty,
            summary,
            string.Empty,
            exception.ToString());
    }

    private static string SanitizeFileName(string name)
    {
        var builder = new StringBuilder(name.Length);
        foreach (var character in name)
        {
            builder.Append(Array.IndexOf(Path.GetInvalidFileNameChars(), character) >= 0 ? '_' : character);
        }

        return builder.ToString();
    }
}

internal sealed class FingerprintStore
{
    private readonly string _path;
    private readonly HashSet<string> _fingerprints;

    public FingerprintStore(string path)
    {
        _path = path;
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        _fingerprints = File.Exists(_path)
            ? File.ReadLines(_path)
                .Select(static line => line.Trim())
                .Where(static line => line.Length > 0)
                .ToHashSet(StringComparer.Ordinal)
            : new HashSet<string>(StringComparer.Ordinal);
    }

    public int Count => _fingerprints.Count;

    public bool TryAdd(string fingerprint)
    {
        if (!_fingerprints.Add(fingerprint))
        {
            return false;
        }

        File.AppendAllText(_path, fingerprint + Environment.NewLine);
        return true;
    }
}

internal sealed class FuzzRunner
{
    private static readonly Regex GuidPattern = new(
        "[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex LongHexPattern = new(
        "[0-9a-f]{12,}",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly IFuzzTarget _target;
    private readonly RunOptions _options;

    public FuzzRunner(IFuzzTarget target, RunOptions options)
    {
        _target = target;
        _options = options;
    }

    public int Run()
    {
        Directory.CreateDirectory(_options.CorpusDirectory);
        Directory.CreateDirectory(_options.ArtifactDirectory);

        var corpus = new CorpusStore(_options.CorpusDirectory);
        corpus.SeedFrom(
            Path.Combine(_options.SeedDirectory, "shared"),
            Path.Combine(_options.SeedDirectory, _target.Name));

        var random = _options.RandomSeed.HasValue ? new Random(_options.RandomSeed.Value) : new Random();
        var corpusSnapshot = corpus.LoadSample(_options.MaxCorpusEntriesInMemory, random);
        var corpusEntries = new CorpusPool(corpusSnapshot.Entries, _options.MaxCorpusEntriesInMemory);
        if (corpusEntries.Count == 0 && corpusSnapshot.FileCount == 0)
        {
            var seed = new byte[] { 0 };
            corpus.TryAdd(seed, _options.MaxCorpusFilesOnDisk);
            corpusEntries.Remember(seed, random);
        }

        var mutator = new InputMutator(InputMutator.LoadDictionary(_options.DictionaryPath));
        var artifactWriter = new ArtifactWriter(_options.ArtifactDirectory);
        var fingerprintStore = new FingerprintStore(_options.FingerprintStorePath);
        var crashFingerprintStore = new FingerprintStore(_options.CrashFingerprintStorePath);
        var started = DateTimeOffset.UtcNow;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var lastStatus = System.Diagnostics.Stopwatch.StartNew();

        long iterations = 0;
        long discoveries = 0;
        long newCrashes = 0;
        long duplicateCrashes = 0;

        while (!ReachedLimit(stopwatch.Elapsed, iterations))
        {
            var input = mutator.CreateCandidate(corpusEntries.Entries, random, _options.MaxInputSize);
            iterations++;

            try
            {
                var result = _target.Execute(input);

                if (fingerprintStore.TryAdd(result.Fingerprint))
                {
                    discoveries++;

                    if (result.SaveInputToCorpus)
                    {
                        corpus.TryAdd(input, _options.MaxCorpusFilesOnDisk);
                        corpusEntries.Remember(input, random);
                    }
                }
            }
            catch (FuzzCrashException ex)
            {
                if (RecordCrash(crashFingerprintStore, artifactWriter, input, ex.InnerException ?? ex, ex.Summary, ex.TextArtifacts, ex.BinaryArtifacts))
                {
                    newCrashes++;
                }
                else
                {
                    duplicateCrashes++;
                }
            }
            catch (Exception ex)
            {
                if (RecordCrash(crashFingerprintStore, artifactWriter, input, ex, ex.Message))
                {
                    newCrashes++;
                }
                else
                {
                    duplicateCrashes++;
                }
            }

            if (lastStatus.Elapsed >= _options.StatusInterval)
            {
                Console.WriteLine(
                    $"[{DateTimeOffset.UtcNow:u}] {_target.Name}: iterations={iterations} pool={corpusEntries.Count} corpus={corpus.FileCount} unique={fingerprintStore.Count} crashes={crashFingerprintStore.Count} discoveries={discoveries} newCrashes={newCrashes} duplicateCrashes={duplicateCrashes} elapsed={stopwatch.Elapsed:g}");
                lastStatus.Restart();
            }
        }

        Console.WriteLine(
            $"[{DateTimeOffset.UtcNow:u}] {_target.Name}: completed iterations={iterations} pool={corpusEntries.Count} corpus={corpus.FileCount} unique={fingerprintStore.Count} crashes={crashFingerprintStore.Count} discoveries={discoveries} newCrashes={newCrashes} duplicateCrashes={duplicateCrashes} started={started:u} elapsed={stopwatch.Elapsed:g}");
        return 0;
    }

    public int Repro(byte[] input)
    {
        Directory.CreateDirectory(_options.ArtifactDirectory);
        var artifactWriter = new ArtifactWriter(_options.ArtifactDirectory);

        try
        {
            var result = _target.Execute(input);
            Console.WriteLine(result.Summary);
            return 0;
        }
        catch (FuzzCrashException ex)
        {
            var crashPath = artifactWriter.WriteCrash(_target.Name, input, ex.InnerException ?? ex, ex.Summary, ex.TextArtifacts, ex.BinaryArtifacts);
            Console.Error.WriteLine($"Crash saved to {crashPath}");
            return 1;
        }
        catch (Exception ex)
        {
            var crashPath = artifactWriter.WriteCrash(_target.Name, input, ex, ex.Message);
            Console.Error.WriteLine($"Crash saved to {crashPath}");
            return 1;
        }
    }

    private bool ReachedLimit(TimeSpan elapsed, long iterations)
    {
        if (_options.Iterations > 0 && iterations >= _options.Iterations)
        {
            return true;
        }

        return elapsed >= _options.MaxTotalTime;
    }

    private bool RecordCrash(
        FingerprintStore crashFingerprintStore,
        ArtifactWriter artifactWriter,
        byte[] input,
        Exception exception,
        string summary,
        IReadOnlyDictionary<string, string>? textArtifacts = null,
        IReadOnlyDictionary<string, byte[]>? binaryArtifacts = null)
    {
        var crashFingerprint = BuildCrashFingerprint(summary, exception);
        if (!crashFingerprintStore.TryAdd(crashFingerprint))
        {
            return false;
        }

        var crashPath = artifactWriter.WriteCrash(_target.Name, input, exception, summary, textArtifacts, binaryArtifacts);
        Console.Error.WriteLine($"[{DateTimeOffset.UtcNow:u}] {_target.Name}: new crash saved to {crashPath}");
        return true;
    }

    private string BuildCrashFingerprint(string summary, Exception exception)
    {
        var frames = new System.Diagnostics.StackTrace(exception, false)
            .GetFrames()?
            .Select(static frame => frame.GetMethod())
            .Where(static method => method is not null)
            .Select(static method => $"{method!.DeclaringType?.FullName}.{method.Name}")
            .Take(12);

        var frameSignature = frames is null ? string.Empty : string.Join(">", frames);
        return Deterministic.HashHex(string.Join(
            Environment.NewLine,
            _target.Name,
            NormalizeCrashText(summary),
            exception.GetType().FullName,
            NormalizeCrashText(exception.Message),
            frameSignature));
    }

    private static string NormalizeCrashText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        value = GuidPattern.Replace(value, "<guid>");
        return LongHexPattern.Replace(value, "<hex>");
    }
}

internal static class Deterministic
{
    public static Random CreateRandom(ReadOnlySpan<byte> input)
    {
        var hash = SHA256.HashData(input.ToArray());
        return new Random(BitConverter.ToInt32(hash, 0));
    }

    public static string HashHex(byte[] data)
    {
        return Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant();
    }

    public static string HashHex(string text)
    {
        return HashHex(Encoding.UTF8.GetBytes(text));
    }

    public static string JoinFingerprints(IEnumerable<string> values)
    {
        return string.Join("|", values.OrderBy(static value => value, StringComparer.Ordinal));
    }
}
