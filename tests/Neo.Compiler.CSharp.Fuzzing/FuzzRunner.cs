// Copyright (C) 2015-2026 The Neo Project.
//
// FuzzRunner.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Neo.Compiler.Fuzzing;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM.Types;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Orchestrates the fuzzing loop: generation, mutation, compilation, and differential
/// analysis.
/// </summary>
public sealed class FuzzRunner : IDisposable
{
    private const int ReferenceCollectionInterval = 500;

    private readonly List<FuzzFinding> _findings = [];

    /// <summary>
    /// The reported finding for every signature seen so far, so that a repeat of a known
    /// signature counts towards that finding instead of being reported again.
    /// </summary>
    private readonly Dictionary<string, FuzzFinding> _seenSignatures = new(StringComparer.Ordinal);

    /// <summary>
    /// The maximum number of entries retained in the corpus.
    /// </summary>
    /// <remarks>
    /// At capacity the least instructive entry yields to a strictly more instructive
    /// input, so a long campaign keeps absorbing new coverage instead of freezing at
    /// the cap.
    /// </remarks>
    internal const int MaxCorpusEntries = 8192;

    /// <summary>
    /// The corpus of inputs that are worth mutating further.
    /// </summary>
    /// <remarks>
    /// An entry is retained when it reached instructions the fuzzer had not seen
    /// before, which is the standard coverage-guided retention rule. Entries that add
    /// nothing new are dropped so that the corpus converges on inputs that actually
    /// exercise new compiler output. Retention is bounded: at capacity the least
    /// instructive entry is evicted in favor of strictly more instructive inputs.
    /// </remarks>
    private readonly List<CorpusEntry> _corpus = [];

    /// <summary>
    /// Opcode/operand and branch-outcome identities the fuzzer has already observed.
    /// </summary>
    private readonly HashSet<string> _seenCoverage = new(StringComparer.Ordinal);

    private readonly CompilerHarness _compiler;
    private readonly CompilerHarness _compilerNoOpt;
    private readonly BehaviorDifferentialAnalyzer _behavior;
    private readonly CrossLanguageOracle _crossLanguage;
    private readonly SourceGenerator _generator;
    private readonly SourceMutator _mutator;

    /// <summary>
    /// Whether the corpus is filtered by the coverage signal.
    /// </summary>
    private readonly bool _coverageGuided;

    /// <summary>
    /// Persistent corpus storage, when a directory was provided.
    /// </summary>
    private readonly CorpusStore? _corpusStore;

    /// <summary>
    /// Coverage identities restored from a previous campaign.
    /// </summary>
    private readonly HashSet<string> _restoredCoverage = new(StringComparer.Ordinal);

    /// <summary>
    /// The number of findings that matched an already-reported defect.
    /// </summary>
    private int _suppressedKnownDefects;

    /// <summary>
    /// The number of single-sided gas exhaustions that disappeared under a larger budget.
    /// </summary>
    private int _gasDivergencesDismissed;

    /// <summary>
    /// Creates a runner with the given seed.
    /// </summary>
    /// <param name="seed">Seed controlling generation and mutation.</param>
    /// <param name="coverageGuided">
    /// When <c>false</c>, the corpus retains every input that compiled and the coverage
    /// signal is not tracked. This exists so that the benefit of coverage guidance can
    /// be measured rather than assumed.
    /// </param>
    /// <param name="corpusDirectory">
    /// When set, the corpus is loaded from this directory before the campaign and saved
    /// back to it afterwards, so coverage accumulates across runs.
    /// </param>
    /// <param name="iterationBudget">End-to-end budget for one iteration; defaults to 30 seconds.</param>
    public FuzzRunner(
        int seed,
        bool coverageGuided = true,
        string? corpusDirectory = null,
        TimeSpan? iterationBudget = null)
    {
        if (iterationBudget <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(iterationBudget));
        }

        _iterationBudget = iterationBudget ?? TimeSpan.FromSeconds(30);
        _coverageGuided = coverageGuided;
        _corpusStore = string.IsNullOrWhiteSpace(corpusDirectory) ? null : new CorpusStore(corpusDirectory);
        _generator = new SourceGenerator(seed);
        _mutator = new SourceMutator(seed * 7919 + 13);

        CompilationOptions baseline = CompilerHarness.CreateDefaultOptions();
        _compiler = new CompilerHarness(baseline);

        CompilationOptions noOpt = CompilerHarness.CreateDefaultOptions();
        noOpt.Optimize = CompilationOptions.OptimizationType.None;
        _compilerNoOpt = new CompilerHarness(noOpt);

        _behavior = new BehaviorDifferentialAnalyzer();
        _crossLanguage = new CrossLanguageOracle();

        LoadCorpus();
    }

    /// <summary>Findings discovered so far.</summary>
    public IReadOnlyList<FuzzFinding> Findings => _findings;

    /// <summary>
    /// The number of distinct opcode/operand and branch-outcome identities observed.
    /// </summary>
    /// <remarks>
    /// This is the coverage signal. A rising value means the fuzzer is still reaching
    /// new compiler output; a flat value means the search has saturated.
    /// </remarks>
    public int CoveredInstructions => _seenCoverage.Count;

    /// <summary>The number of entries retained in the corpus.</summary>
    public int CorpusSize => _corpus.Count;

    /// <summary>Number of fuzz iterations executed.</summary>
    public long Iterations { get; private set; }

    /// <summary>Number of inputs that compiled successfully.</summary>
    public long Compiled { get; private set; }

    /// <summary>Number of inputs rejected with normal diagnostics.</summary>
    public long Rejected { get; private set; }

    /// <summary>
    /// Runs the fuzzing loop.
    /// </summary>
    /// <param name="iterations">Number of iterations to run.</param>
    /// <param name="timeBudget">Optional wall-clock budget.</param>
    /// <param name="cancellation">Token used to stop the loop early.</param>
    /// <param name="onProgress">Optional progress callback invoked every N iterations.</param>
    public void Run(
        int iterations,
        TimeSpan? timeBudget = null,
        CancellationToken cancellation = default,
        Action<FuzzRunner>? onProgress = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var features = (FeatureFamily[])Enum.GetValues(typeof(FeatureFamily));

        if (iterations == 0 && _corpus.Count > 0)
        {
            ReplayCorpus(stopwatch, timeBudget, cancellation);
            onProgress?.Invoke(this);
            return;
        }

        // Seed the corpus with one source per feature family so the loop starts from a
        // diverse, already-valid baseline.
        foreach (FeatureFamily feature in features)
        {
            (string contractSource, string referenceSource) = _generator.GeneratePair(feature);
            Process(contractSource, referenceSource);
        }

        for (int i = 0; i < iterations; i++)
        {
            if (cancellation.IsCancellationRequested)
            {
                break;
            }

            if (timeBudget is { } budget && stopwatch.Elapsed > budget)
            {
                break;
            }

            (string source, string? reference) = NextInput(features);
            var iterationStopwatch = Stopwatch.StartNew();

            if (reference is null && IsMultiContract(source))
            {
                MultiContractIterations++;
                ProcessMultiContract(source);
            }
            else
            {
                Process(source, reference);
            }

            Iterations++;
            CheckIterationBudget(source, iterationStopwatch.Elapsed);

            if (StoppedOnSlowExecution)
            {
                // A single pathological input must not end the campaign: a corpus that
                // carries an inherently slow entry would stop every run at the same
                // point and starve the farm. Keep going until several iterations in a
                // row exceed the budget, which points at a systematic slowdown rather
                // than isolated pathological mutants.
                _slowIterationCount++;
                if (_slowIterationCount >= MaxSlowIterationsPerCampaign)
                {
                    break;
                }
            }
            else
            {
                _slowIterationCount = 0;
            }

            if (Iterations % ReferenceCollectionInterval == 0)
            {
                _crossLanguage.CollectUnloadedAssemblies();
                ReferenceCollectionCount++;
            }

            if (onProgress is not null && Iterations % 50 == 0)
            {
                onProgress(this);
            }
        }

        if (onProgress is not null)
        {
            onProgress(this);
        }
    }

    /// <summary>
    /// Determines whether a source declares more than one contract.
    /// </summary>
    /// <param name="source">The source to inspect.</param>
    /// <returns><c>true</c> when the source declares both ContractA and ContractB.</returns>
    private static bool IsMultiContract(string source) =>
        source.Contains("class ContractA", StringComparison.Ordinal) &&
        source.Contains("class ContractB", StringComparison.Ordinal);

    /// <summary>
    /// Produces the next fuzzing input, mixing fresh generation with corpus mutation.
    /// </summary>
    /// <remarks>
    /// When the input comes from the generator it comes with a matching plain C#
    /// reference, which enables the cross-language oracle. Mutated inputs have no
    /// trustworthy reference, so they only take part in the compiler-internal oracles.
    /// </remarks>
    private (string Source, string? Reference) NextInput(FeatureFamily[] features)
    {
        // 40% random expressions, 30% corpus mutation, 20% other families,
        // 10% multi-contract source.
        // Random expressions dominate because they are the only source of structurally
        // new programs; the template families mostly vary constants.
        int roll = _randomNext(10);

        if (roll < 3)
        {
            return _generator.GeneratePair(FeatureFamily.RandomExpressions);
        }

        if (roll < 5 || _corpus.Count == 0)
        {
            FeatureFamily feature = features[_randomNext(features.Length)];
            return _generator.GeneratePair(feature);
        }

        if (roll < 8)
        {
            // Mutate a corpus entry. The corpus only holds entries that reached new
            // instructions, so this is where the search actually deepens.
            CorpusEntry entry = _corpus[_randomNext(_corpus.Count)];
            return (_mutator.Mutate(entry.Source), null);
        }

        if (roll < 9)
        {
            return (_generator.GenerateMultiContract(), null);
        }

        return _generator.GeneratePair(FeatureFamily.RandomExpressions);
    }

    private int _randomCounter = 0;

    private int _randomNext(int bound)
    {
        // Deterministic, seed-independent jitter derived from the iteration counter so
        // that a failing seed reproduces exactly.
        _randomCounter++;
        uint x = (uint)(_randomCounter * 2654435761u);
        x ^= x >> 15;
        x *= 2246822519u;
        x ^= x >> 13;
        return (int)(x % (uint)Math.Max(1, bound));
    }

    /// <summary>
    /// The name of the contract a cross-contract call targets.
    /// </summary>
    /// <remarks>
    /// The multi-contract family always names its callee <c>ContractB</c>, so a Hash160
    /// parameter is resolved to that contract's deployed hash.
    /// </remarks>
    public const string MultiContractCallee = "ContractB";

    /// <summary>
    /// The wall-clock budget for a single iteration.
    /// </summary>
    /// <remarks>
    /// The VM executes synchronously, so a pathological input cannot be interrupted
    /// safely without corrupting the engine. Instead, an iteration that exceeds this
    /// budget is reported with a reproducer and ends the campaign: a fuzzer that stalls
    /// on one input cannot make progress, and silently burning the whole time budget on
    /// it would hide that.
    /// </remarks>
    private readonly TimeSpan _iterationBudget;

    /// <summary>
    /// The number of consecutive over-budget iterations tolerated before the campaign
    /// stops on a slow execution.
    /// </summary>
    private const int MaxSlowIterationsPerCampaign = 3;

    /// <summary>Running count of consecutive over-budget iterations.</summary>
    private int _slowIterationCount;

    /// <summary>
    /// Whether the campaign was stopped because an iteration exceeded the budget.
    /// </summary>
    public bool StoppedOnSlowExecution { get; private set; }

    /// <summary>
    /// Compiles a source, classifies the result, and runs differential analysis.
    /// </summary>
    /// <param name="source">The contract source.</param>
    /// <param name="referenceSource">The matching plain C# reference, when available.</param>
    private void Process(string source, string? referenceSource)
    {
        if (SyntaxAwareMutator.HasExcessiveLoop(source))
        {
            Rejected++;
            return;
        }

        (FuzzOutcome outcome, CompilationContext? context, FuzzFinding? finding) = _compiler.Compile(source);

        if (finding is not null)
        {
            Record(finding);
            return;
        }

        if (outcome == FuzzOutcome.Rejected)
        {
            Rejected++;
            return;
        }

        Compiled++;

        if (context is null)
        {
            return;
        }

        // Only inputs that compile cleanly are useful for the differential phase.
        try
        {
            RunDifferential(source, context);

            if (referenceSource is not null)
            {
                (NefFile nef, ContractManifest manifest, _) = context.CreateResults();
                RunCrossLanguageDifferential(source, referenceSource, new ArtifactSet(nef, manifest));
            }
        }
        catch (Exception ex)
        {
            Record(new FuzzFinding(
                FuzzOutcome.DifferentialMismatch,
                source,
                $"Differential analysis threw: {ex.GetType().FullName}: {ex.Message}",
                $"{ex.GetType().FullName}: {ex.Message}",
                ex.ToString()));
        }
    }

    /// <summary>
    /// Applies one wall-clock budget to compilation and every differential execution.
    /// </summary>
    internal void CheckIterationBudget(string source, TimeSpan elapsed)
    {
        if (elapsed <= _iterationBudget)
        {
            return;
        }

        StoppedOnSlowExecution = true;
        Record(new FuzzFinding(
            FuzzOutcome.SlowExecution,
            source,
            $"The iteration took {elapsed.TotalSeconds:F1}s, over the " +
            $"{_iterationBudget.TotalSeconds:F3}s per-iteration budget."));
    }

    /// <summary>
    /// Compares the behaviour of a contract against the equivalent plain C# code.
    /// </summary>
    /// <remarks>
    /// The contract is executed on the Neo VM; the reference is executed on the CLR.
    /// The two must agree on every input. Because the reference is the C# language
    /// itself, a divergence is a compiler defect rather than a disagreement between two
    /// implementations.
    /// </remarks>
    private void RunCrossLanguageDifferential(string contractSource, string referenceSource, ArtifactSet artifacts)
    {
        if (string.IsNullOrWhiteSpace(referenceSource))
        {
            // Stateful families have no CLR equivalent, so there is nothing to compare
            // against. The optimizer-equivalence oracle still covers them.
            return;
        }

        Assembly? referenceAssembly = _crossLanguage.TryCompileReference(referenceSource);
        if (referenceAssembly is null)
        {
            // The reference did not compile; the Neo compiler may still accept the
            // contract. That asymmetry is worth reporting but is not a behaviour bug,
            // so it is skipped here.
            return;
        }

        // Build the call list first so the contract is deployed only once.
        var calls = new List<(string AbiName, string ClrName, object?[] Args, object?[] ClrArgs)>();
        foreach (ContractMethodAbi method in EnumerateCallableMethods(artifacts.Manifest))
        {
            // The CLR method keeps its original PascalCase name; the ABI name is the
            // lower-cased form the VM dispatches on.
            string clrName = UpperFirst(method.Name);

            foreach (object?[] args in ArgumentSetsFor(method))
            {
                calls.Add((method.Name, clrName, args, args.Select(ToClrArgument).ToArray()));
            }
        }

        if (calls.Count == 0)
        {
            return;
        }

        (List<(bool Ok, string? Error, StackItem? Value)> vmResults, _, _, string? deploymentError) =
            _behavior.ExecuteBatchWithCoverage(artifacts, calls.Select(c => (c.AbiName, c.Args)).ToList());

        // The contract never deployed, so there is no VM behaviour to hold against the
        // reference; reporting every call as a divergence would misstate one defect as many.
        if (deploymentError is not null)
        {
            Record(new FuzzFinding(
                FuzzOutcome.DeploymentFailure,
                contractSource,
                $"The optimized build failed to deploy: {deploymentError}"));
            return;
        }

        for (int i = 0; i < calls.Count; i++)
        {
            (string abiName, string clrName, object?[] args, object?[] clrArgs) = calls[i];

            (bool okRef, object? refValue, string? refError) =
                _crossLanguage.TryInvoke(referenceAssembly, "FuzzReference", clrName, clrArgs);
            if (CrossLanguageOracle.IsHarnessError(refError))
            {
                // An ABI integer has no CLR width. If it cannot be converted to the
                // generated method's parameter type, there is no C# invocation to compare.
                continue;
            }

            var vm = vmResults[i];
            string? mismatch = CompareCrossLanguageOutcome(
                clrName, args, okRef, refValue, refError, vm.Ok, vm.Error, vm.Value);
            if (mismatch is not null)
            {
                Record(new FuzzFinding(FuzzOutcome.DifferentialMismatch, contractSource, mismatch));
            }
        }
    }

    /// <summary>
    /// Compares CLR and VM outcomes in both directions, including success/failure mismatches.
    /// </summary>
    internal static string? CompareCrossLanguageOutcome(
        string methodName,
        object?[] args,
        bool okRef,
        object? refValue,
        string? refError,
        bool okVm,
        string? vmError,
        StackItem? vmValue)
    {
        if (CrossLanguageOracle.IsHarnessError(refError))
        {
            return null;
        }

        string renderedArgs = string.Join(", ", args.Select(a => a?.ToString() ?? "null"));
        if (okRef && !okVm)
        {
            return $"Method '{methodName}' with args [{renderedArgs}] returned {RenderClrValue(refValue)} in C# " +
                   $"but faulted in the Neo VM ({vmError}).";
        }

        if (!okRef && okVm)
        {
            return $"Method '{methodName}' with args [{renderedArgs}] faulted in C# ({refError}) " +
                   $"but returned {BehaviorDifferentialAnalyzer.Render(vmValue!)} in the Neo VM.";
        }

        if (!okRef)
        {
            return null;
        }

        string renderedRef = RenderClrValue(refValue);
        string renderedVm = BehaviorDifferentialAnalyzer.Render(vmValue!);
        return string.Equals(renderedRef, renderedVm, StringComparison.Ordinal)
            ? null
            : $"Method '{methodName}' with args [{renderedArgs}] returned {renderedRef} in C# " +
              $"but {renderedVm} in the Neo VM.";
    }

    /// <summary>
    /// Converts a VM argument into the CLR equivalent.
    /// </summary>
    private static object? ToClrArgument(object? arg) => arg switch
    {
        null => null,
        bool b => b,
        System.Numerics.BigInteger i => i,
        string s => s,
        byte[] bytes => bytes,
        _ => arg,
    };

    /// <summary>
    /// Renders a CLR value into the same canonical form used for VM values.
    /// </summary>
    /// <remarks>
    /// The Neo VM has no distinct string type: a string is a byte string holding its
    /// UTF-8 bytes. A CLR <see cref="string"/> must therefore be rendered as the hex of
    /// its UTF-8 bytes, otherwise the same value would compare as different.
    /// </remarks>
    private static string RenderClrValue(object? value) => value switch
    {
        null => "<null>",
        bool b => b ? "true" : "false",
        System.Numerics.BigInteger i => i.ToString(System.Globalization.CultureInfo.InvariantCulture),
        int i => ((System.Numerics.BigInteger)i).ToString(System.Globalization.CultureInfo.InvariantCulture),
        long l => ((System.Numerics.BigInteger)l).ToString(System.Globalization.CultureInfo.InvariantCulture),
        string s => "0x" + Convert.ToHexString(System.Text.Encoding.UTF8.GetBytes(s)),
        byte[] bytes => "0x" + Convert.ToHexString(bytes),
        _ => value.ToString() ?? "<unknown>",
    };

    /// <summary>
    /// Upper-cases the first character of a name.
    /// </summary>
    private static string UpperFirst(string name)
    {
        if (name.Length == 0)
        {
            return name;
        }
        return char.ToUpperInvariant(name[0]) + name[1..];
    }

    /// <summary>
    /// Runs the differential analyses over a successfully compiled source.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The artifact sets are produced in two different ways, and the distinction matters:
    /// </para>
    /// <list type="bullet">
    /// <item><description><b>Optimized</b> uses <c>CreateResults()</c>, which applies the
    /// experimental optimizer (16 strategies: jump compression, peephole rewrites,
    /// reachability pruning). This is where most defects live.</description></item>
    /// <item><description><b>Unoptimized</b> uses <c>CreateExecutable()</c>, which is the
    /// raw script. Note that <c>Optimize=None</c> is <i>not</i> the same thing: the
    /// compiler unconditionally runs <c>BasicOptimizer.CompressJumps</c>, so a separate
    /// compilation with <c>Optimize=None</c> still differs from the raw script.</description></item>
    /// </list>
    /// </remarks>
    private void RunDifferential(string source, CompilationContext context)
    {
        // Artifact set A: the fully optimized build. CreateResults() applies the
        // experimental optimizer (16 strategies: jump compression, peephole rewrites,
        // reachability pruning). This is where most defects live.
        (NefFile optimizedNef, ContractManifest optimizedManifest, _) = context.CreateResults();
        ArtifactSet optimized = new(optimizedNef, optimizedManifest);

        // Artifact set B: the same source compiled with Optimize=None. The experimental
        // optimizer is skipped, but the compiler still runs BasicOptimizer.CompressJumps
        // unconditionally, so this is a genuinely different code path.
        (FuzzOutcome outcomeB, CompilationContext? contextB, FuzzFinding? findingB) = _compilerNoOpt.Compile(source);
        if (outcomeB != FuzzOutcome.Clean || contextB is null)
        {
            if (findingB is not null && findingB.Outcome is FuzzOutcome.CompilerCrash or FuzzOutcome.InternalError)
            {
                Record(findingB);
            }
            return;
        }

        (NefFile rawNef, ContractManifest rawManifest, _) = contextB.CreateResults();
        ArtifactSet unoptimized = new(rawNef, rawManifest);

        // Oracle 1: determinism. Compiling the identical source twice must agree.
        (FuzzOutcome outcomeC, CompilationContext? contextC, _) = _compiler.Compile(source);
        if (outcomeC == FuzzOutcome.Clean && contextC is not null)
        {
            (NefFile repeatNef, ContractManifest repeatManifest, _) = contextC.CreateResults();
            ArtifactSet repeat = new(repeatNef, repeatManifest);
            foreach (DifferentialDivergence divergence in DifferentialAnalyzers.CheckDeterminism(optimized, repeat))
            {
                Record(new FuzzFinding(FuzzOutcome.NonDeterministic, source, divergence.Detail));
            }
        }

        // Oracle 2: ABI equivalence across optimization levels.
        foreach (DifferentialDivergence divergence in DifferentialAnalyzers.Compare("optimized", optimized, "unoptimized", unoptimized))
        {
            if (divergence.Kind is "AbiMethodSet" or "AbiParameterCount" or "AbiParameterTypes" or "AbiReturnType")
            {
                Record(new FuzzFinding(FuzzOutcome.DifferentialMismatch, source, divergence.Detail));
            }
        }

        // Oracle 3: behaviour equivalence. The optimizer must not change observable
        // behaviour. This is the strongest oracle because it needs no reference
        // implementation, so it cannot produce false positives.
        List<(string Method, object?[] Args)> calls = [];
        foreach (ContractMethodAbi method in EnumerateCallableMethods(optimized.Manifest))
        {
            foreach (object?[] args in ArgumentSetsFor(method))
            {
                calls.Add((method.Name, args));
            }
        }

        if (calls.Count > 0)
        {
            (List<(bool Ok, string? Error, StackItem? Value)> resultsA, IReadOnlyCollection<string> coverage,
             IReadOnlyDictionary<string, string> storageA, List<IReadOnlyList<string>> notificationsA,
             string? deploymentA) =
                _behavior.ExecuteBatchWithNotifications(optimized, calls);
            (List<(bool Ok, string? Error, StackItem? Value)> resultsB, _,
             IReadOnlyDictionary<string, string> storageB, List<IReadOnlyList<string>> notificationsB,
             string? deploymentB) =
                _behavior.ExecuteBatchWithNotifications(unoptimized, calls);

            // A build that did not deploy has no per-call behaviour to compare. Report the
            // deployment itself and stop here: comparing the mirrored failures against the
            // other build would turn the one defect into a mismatch per call.
            if (deploymentA is not null || deploymentB is not null)
            {
                RecordDeploymentFailure(source, deploymentA, deploymentB);
                return;
            }

            CheckNotifications(source, calls, resultsA, notificationsA, "optimized");
            CheckNotifications(source, calls, resultsB, notificationsB, "unoptimized");

            // Storage state differential. The return value alone is not enough: an
            // optimizer rewrite that stores the wrong value while returning the same
            // thing would slip through unnoticed.
            CompareStorage(source, storageA, storageB);

            // Coverage-guided retention: keep the input only when it reached
            // instructions the fuzzer had not seen before. With guidance disabled the
            // corpus keeps every input that compiled, which is the baseline to compare
            // against.
            bool reachedNewInstructions = false;
            foreach (string coverageKey in coverage)
            {
                if (_seenCoverage.Add(coverageKey))
                {
                    reachedNewInstructions = true;
                }
            }

            if (reachedNewInstructions)
            {
                // Coverage-feedback loop: mutation kinds that produced new instructions
                // are rewarded, raising their selection weight for future inputs.
                _mutator.RewardKinds(_ => true);
            }

            if (!_coverageGuided || reachedNewInstructions)
            {
                TryAddCorpusEntry(source, coverage.Count, CanonicalCoverage(coverage));
            }

            for (int i = 0; i < calls.Count; i++)
            {
                (string method, object?[] args) = calls[i];
                var a = resultsA[i];
                var b = resultsB[i];

                if (!a.Ok && !b.Ok)
                {
                    if (!BehaviorDifferentialAnalyzer.FailuresEquivalent(a.Error, b.Error))
                    {
                        Record(new FuzzFinding(
                            FuzzOutcome.DifferentialMismatch,
                            source,
                            $"Method '{method}' failed in both variants but with different errors: " +
                            $"optimized='{a.Error}', unoptimized='{b.Error}'."));
                    }
                    continue;
                }
                if (!a.Ok || !b.Ok)
                {
                    // One build ran out of gas where the other finished. The unoptimized
                    // build spends more gas for the same work, so first check whether the
                    // two agree once the budget stops being the limiting factor.
                    if (IsGasExhaustion(a.Ok ? b.Error : a.Error)
                        && _behavior.AgreeUnderLargerBudget(optimized, unoptimized, method, args))
                    {
                        _gasDivergencesDismissed++;
                        continue;
                    }

                    Record(new FuzzFinding(
                        FuzzOutcome.DifferentialMismatch,
                        source,
                        $"Method '{method}' with args [{string.Join(", ", args.Select(RenderArg))}] " +
                        (a.Ok
                            ? $"succeeded in optimized (returned {BehaviorDifferentialAnalyzer.Render(a.Value!)}) but faulted in unoptimized ({b.Error})."
                            : $"faulted in optimized ({a.Error}) but succeeded in unoptimized (returned {BehaviorDifferentialAnalyzer.Render(b.Value!)}).")));
                    continue;
                }

                string renderedA = BehaviorDifferentialAnalyzer.Render(a.Value!);
                string renderedB = BehaviorDifferentialAnalyzer.Render(b.Value!);

                if (!string.Equals(renderedA, renderedB, StringComparison.Ordinal))
                {
                    Record(new FuzzFinding(
                        FuzzOutcome.DifferentialMismatch,
                        source,
                        $"Method '{method}' with args [{string.Join(", ", args.Select(RenderArg))}] " +
                        $"returned {renderedA} in optimized but {renderedB} in unoptimized."));
                }
            }

            // The loop above can only see differences between the two builds. Report the
            // cases where both builds agree on failing for a reason that points at the
            // harness rather than the contract.
            foreach (var group in calls
                .Select((call, index) => (call.Method, Results: resultsA[index]))
                .GroupBy(x => x.Method, StringComparer.Ordinal))
            {
                TriageFailures(
                    source,
                    group.Key,
                    group.Select(x => (x.Results.Ok, x.Results.Error)).ToList(),
                    "optimizer-equivalence");
            }
        }
    }

    /// <summary>
    /// Compiles a multi-contract source and compares the behaviour of the two builds.
    /// </summary>
    /// <remarks>
    /// A cross-contract call crosses compilation units: the caller's method token table
    /// records the callee's hash and method signature, and the callee is a separate
    /// artifact with its own entry point. Only the VM can execute that pair, so there is
    /// no plain C# reference to compare against; the optimizer-equivalence and storage
    /// oracles carry the comparison instead.
    /// </remarks>
    /// <param name="source">The multi-contract source.</param>
    private void ProcessMultiContract(string source)
    {
        (FuzzOutcome outcome, List<(string Name, CompilationContext Context)> contracts, FuzzFinding? finding) =
            _compiler.CompileAll(source);

        if (finding is not null)
        {
            Record(finding);
            return;
        }

        if (outcome != FuzzOutcome.Clean || contracts.Count < 2)
        {
            Rejected++;
            return;
        }

        Compiled++;

        var optimized = new Dictionary<string, ArtifactSet>(StringComparer.Ordinal);
        foreach ((string name, CompilationContext context) in contracts)
        {
            (NefFile nef, ContractManifest manifest, _) = context.CreateResults();
            optimized[name] = new ArtifactSet(nef, manifest);
        }

        (FuzzOutcome plainOutcome, List<(string Name, CompilationContext Context)> plainContracts, FuzzFinding? plainFinding) =
            _compilerNoOpt.CompileAll(source);

        if (plainFinding is not null && plainFinding.Outcome is FuzzOutcome.CompilerCrash or FuzzOutcome.InternalError)
        {
            Record(plainFinding);
            return;
        }

        if (plainOutcome != FuzzOutcome.Clean || plainContracts.Count < 2)
        {
            return;
        }

        var unoptimized = new Dictionary<string, ArtifactSet>(StringComparer.Ordinal);
        foreach ((string name, CompilationContext context) in plainContracts)
        {
            (NefFile nef, ContractManifest manifest, _) = context.CreateResults();
            unoptimized[name] = new ArtifactSet(nef, manifest);
        }

        // The caller is the contract whose ABI references the callee by hash.
        string? entry = optimized.Keys.FirstOrDefault(k =>
            optimized[k].Manifest.Abi.Methods.Any(m =>
                m.Parameters.Any(p => p.Type == Neo.SmartContract.ContractParameterType.Hash160)));

        if (entry is null || !unoptimized.ContainsKey(entry))
        {
            return;
        }

        var calls = new List<(string Method, object?[] Args)>();
        foreach (ContractMethodAbi method in EnumerateCallableMethods(optimized[entry].Manifest))
        {
            foreach (object?[] args in ArgumentSetsFor(method))
            {
                calls.Add((method.Name, args));
            }
        }

        if (calls.Count == 0)
        {
            return;
        }

        var optimizedBundle = new BehaviorDifferentialAnalyzer.ArtifactBundle(optimized);
        var unoptimizedBundle = new BehaviorDifferentialAnalyzer.ArtifactBundle(unoptimized);

        (List<(bool Ok, string? Error, StackItem? Value)> resultsA, IReadOnlyCollection<string> coverage,
         IReadOnlyDictionary<string, string> storageA, string? deploymentA) =
            _behavior.ExecuteBundleWithCoverage(optimizedBundle, entry, calls);
        (List<(bool Ok, string? Error, StackItem? Value)> resultsB, _,
         IReadOnlyDictionary<string, string> storageB, string? deploymentB) =
            _behavior.ExecuteBundleWithCoverage(unoptimizedBundle, entry, calls);

        // Same as for a single contract: a bundle that did not deploy has nothing to compare.
        if (deploymentA is not null || deploymentB is not null)
        {
            RecordDeploymentFailure(source, deploymentA, deploymentB);
            return;
        }

        // Coverage retention works the same way as for a single contract.
        bool reachedNewInstructions = false;
        foreach (string coverageKey in coverage)
        {
            if (_seenCoverage.Add(coverageKey))
            {
                reachedNewInstructions = true;
            }
        }

        if (reachedNewInstructions)
        {
            _mutator.RewardKinds(_ => true);
        }

        if (reachedNewInstructions)
        {
            TryAddCorpusEntry(source, coverage.Count, CanonicalCoverage(coverage));
        }

        CompareStorage(source, storageA, storageB);

        for (int i = 0; i < calls.Count; i++)
        {
            (string method, object?[] args) = calls[i];
            var a = resultsA[i];
            var b = resultsB[i];

            if (!a.Ok && !b.Ok)
            {
                if (!BehaviorDifferentialAnalyzer.FailuresEquivalent(a.Error, b.Error))
                {
                    Record(new FuzzFinding(
                        FuzzOutcome.DifferentialMismatch,
                        source,
                        $"Cross-contract method '{method}' on '{entry}' failed in both builds but differently: " +
                        $"optimized='{a.Error}', unoptimized='{b.Error}'."));
                }
                continue;
            }

            if (!a.Ok || !b.Ok)
            {
                Record(new FuzzFinding(
                    FuzzOutcome.DifferentialMismatch,
                    source,
                    $"Cross-contract method '{method}' on '{entry}' " +
                    (a.Ok
                        ? $"succeeded in optimized (returned {BehaviorDifferentialAnalyzer.Render(a.Value!)}) but faulted in unoptimized ({b.Error})."
                        : $"faulted in optimized ({a.Error}) but succeeded in unoptimized (returned {BehaviorDifferentialAnalyzer.Render(b.Value!)}).")));
                continue;
            }

            string renderedA = BehaviorDifferentialAnalyzer.Render(a.Value!);
            string renderedB = BehaviorDifferentialAnalyzer.Render(b.Value!);

            if (!string.Equals(renderedA, renderedB, StringComparison.Ordinal))
            {
                Record(new FuzzFinding(
                    FuzzOutcome.DifferentialMismatch,
                    source,
                    $"Cross-contract method '{method}' on '{entry}' returned {renderedA} in optimized " +
                    $"but {renderedB} in unoptimized."));
            }
        }

        // A cross-contract call is the case most likely to fail identically on both sides
        // for a harness reason (a missing permission, a wrong method name), so the triage
        // matters most here.
        foreach (var group in calls
            .Select((call, index) => (call.Method, Results: resultsA[index]))
            .GroupBy(x => x.Method, StringComparer.Ordinal))
        {
            TriageFailures(
                source,
                $"{entry}.{group.Key}",
                group.Select(x => (x.Results.Ok, x.Results.Error)).ToList(),
                "cross-contract");
        }
    }

    /// <summary>
    /// Runs the failure triage over one method's results and reports anything suspicious.
    /// </summary>
    /// <remarks>
    /// The differential only compares the two builds with each other, so it cannot notice
    /// a call that fails identically on both sides. This triage supplies the missing
    /// signal: a method that never succeeds, or an error that describes a malformed call,
    /// means the harness may be measuring nothing at all.
    /// </remarks>
    /// <param name="source">The contract source, used for the report.</param>
    /// <param name="method">The method that was called.</param>
    /// <param name="results">The per-input results for that method.</param>
    /// <param name="context">A label describing which oracle produced the results.</param>
    private void TriageFailures(
        string source,
        string method,
        IReadOnlyList<(bool Ok, string? Error)> results,
        string context)
    {
        string? suspicion = FailureTriage.Inspect(method, results);
        if (suspicion is not null)
        {
            Record(new FuzzFinding(FuzzOutcome.SuspiciousFailure, source, $"{context}: {suspicion}"));
        }
    }

    /// <summary>
    /// Compares the storage state produced by two artifact sets.
    /// </summary>
    /// <remarks>
    /// A method that writes to storage and returns a constant produces the same return
    /// value no matter what the optimizer does to the stored value. Comparing the storage
    /// state as well is what makes the optimizer-equivalence oracle complete for
    /// stateful contracts.
    /// </remarks>
    /// <summary>
    /// Checks the notification a method emitted against the value it returned.
    /// </summary>
    /// <remarks>
    /// The generator raises an event with the return value as both arguments, so the payload has
    /// to contain exactly that value twice. The check needs no reference implementation: it
    /// compares two outputs of the same execution, which makes it a safe oracle.
    /// </remarks>
    private void CheckNotifications(string source, IReadOnlyList<(string Method, object?[] Args)> calls,
        IReadOnlyList<(bool Ok, string? Error, StackItem? Value)> results,
        IReadOnlyList<IReadOnlyList<string>> notifications, string label)
    {
        // The statement-deletion mutation may remove the Changed(...) invocation from a
        // single method; a method that no longer emits the event is behaviorally
        // self-consistent, so its oracle expectation only applies when the invocation
        // survived mutation *in that method's body*.
        var invokesEvent = MethodsInvokingEvent(source, "echoEvent", "Changed");

        for (int i = 0; i < calls.Count && i < notifications.Count; i++)
        {
            if (!calls[i].Method.StartsWith("echoEvent", StringComparison.OrdinalIgnoreCase))
                continue;
            if (!invokesEvent(calls[i].Method))
                continue;
            if (!results[i].Ok)
                continue;

            string value = BehaviorDifferentialAnalyzer.Render(results[i].Value!);
            string expected = $"Changed:[{value},{value}]";
            string actual = string.Join("|", notifications[i]);

            if (!string.Equals(actual, expected, StringComparison.Ordinal))
            {
                Record(new FuzzFinding(
                    FuzzOutcome.DifferentialMismatch,
                    source,
                    $"Method '{calls[i].Method}' returned {value} but emitted '{actual}' in the {label} build, " +
                    $"expected '{expected}'."));
            }
        }
    }

    /// <summary>
    /// Determines, per event-method, whether its body still invokes the given event.
    /// </summary>
    /// <param name="source">The contract source.</param>
    /// <param name="abiPrefix">The ABI (lower-cased) name prefix of the methods to inspect.</param>
    /// <param name="eventName">The event identifier the body must invoke.</param>
    /// <returns>A predicate over the ABI method name.</returns>
    /// <remarks>
    /// The statement-deletion mutation can remove the invocation from one method while
    /// leaving sibling methods intact, so the check has to be per method body, not per
    /// contract.
    /// </remarks>
    private static Func<string, bool> MethodsInvokingEvent(string source, string abiPrefix, string eventName)
    {
        var invoking = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source,
                new Microsoft.CodeAnalysis.CSharp.CSharpParseOptions(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Latest));
            foreach (var method in tree.GetRoot().DescendantNodes()
                .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax>()
                .Where(m => m.Identifier.ValueText.StartsWith(abiPrefix, StringComparison.OrdinalIgnoreCase)))
            {
                bool calls = method.DescendantNodes()
                    .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax>()
                    .Any(id => id.Identifier.ValueText == eventName);
                if (calls)
                    invoking.Add(method.Identifier.ValueText);
            }
        }
        catch (Exception)
        {
            // If the source does not parse, treat every method as invoking so the oracle
            // keeps its previous (conservative) behavior.
        }
        return name => invoking.Contains(name);
    }

    /// <param name="source">The contract source, used for the report.</param>
    /// <param name="optimized">The storage produced by the optimized build.</param>
    /// <param name="unoptimized">The storage produced by the unoptimized build.</param>
    private void CompareStorage(
        string source,
        IReadOnlyDictionary<string, string> optimized,
        IReadOnlyDictionary<string, string> unoptimized)
    {
        foreach (DifferentialDivergence divergence in CompareStorageState(optimized, unoptimized))
        {
            Record(new FuzzFinding(FuzzOutcome.DifferentialMismatch, source, divergence.Detail));
        }
    }

    /// <summary>
    /// Compares two storage snapshots and reports every differing entry.
    /// </summary>
    /// <param name="optimized">The storage produced by the optimized build.</param>
    /// <param name="unoptimized">The storage produced by the unoptimized build.</param>
    /// <returns>The divergences; empty when the two snapshots agree.</returns>
    public static List<DifferentialDivergence> CompareStorageState(
        IReadOnlyDictionary<string, string> optimized,
        IReadOnlyDictionary<string, string> unoptimized)
    {
        var result = new List<DifferentialDivergence>();

        foreach (string key in optimized.Keys.Union(unoptimized.Keys, StringComparer.Ordinal).OrderBy(k => k, StringComparer.Ordinal))
        {
            optimized.TryGetValue(key, out string? valueA);
            unoptimized.TryGetValue(key, out string? valueB);

            if (!string.Equals(valueA, valueB, StringComparison.Ordinal))
            {
                result.Add(new DifferentialDivergence(
                    "StorageMismatch",
                    $"The contract's storage differs after execution: key 0x{Convert.ToHexString(Convert.FromBase64String(key))} " +
                    $"holds {DescribeStorageValue(valueA)} in optimized but {DescribeStorageValue(valueB)} in unoptimized."));
            }
        }

        return result;
    }

    /// <summary>
    /// Renders a storage value for a finding message.
    /// </summary>
    private static string DescribeStorageValue(string? base64Value)
    {
        if (base64Value is null)
        {
            return "<absent>";
        }

        byte[] bytes = Convert.FromBase64String(base64Value);
        return $"0x{Convert.ToHexString(bytes)}";
    }

    /// <summary>
    /// Renders an argument for a finding message.
    /// </summary>
    private static string RenderArg(object? arg) => arg switch
    {
        null => "null",
        bool b => b ? "true" : "false",
        BigInteger i => i.ToString(System.Globalization.CultureInfo.InvariantCulture),
        string s => "\"" + s + "\"",
        _ => arg.ToString() ?? "null",
    };

    /// <summary>
    /// Enumerates the publicly callable methods of a compiled contract.
    /// </summary>
    /// <remarks>
    /// The constructor is excluded: it is invoked by the VM at deploy time, not through
    /// the public call path, and its presence legitimately differs between optimization
    /// levels (see <see cref="DifferentialAnalyzers"/>).
    /// </remarks>
    private static IEnumerable<ContractMethodAbi> EnumerateCallableMethods(Neo.SmartContract.Manifest.ContractManifest manifest)
    {
        foreach (Neo.SmartContract.Manifest.ContractMethodDescriptor method in manifest.Abi.Methods)
        {
            if (string.Equals(method.Name, "_initialize", StringComparison.Ordinal))
            {
                continue;
            }

            // The compiler lower-cases the first character of public method names, so the
            // ABI name is the one the VM dispatches on.
            yield return new ContractMethodAbi(
                method.Name,
                method.Parameters.Length,
                method.Parameters.Select(p => p.Type).ToArray(),
                method.ReturnType);
        }
    }

    /// <summary>
    /// Produces the argument sets used when invoking a method.
    /// </summary>
    /// <remarks>
    /// The argument count always matches the ABI signature exactly: passing the wrong
    /// number of arguments makes the VM fail with a stack-underflow that says nothing
    /// about the compiler. Values are chosen to sit on boundaries (zero, one, negative,
    /// and the extremes of each integer width) because that is where optimisation bugs
    /// concentrate.
    /// </remarks>
    private static IEnumerable<object?[]> ArgumentSetsFor(ContractMethodAbi method)
    {
        object[] baseValues = [0, 1, -1, 2, 7, -7, 255, 65535, int.MaxValue, int.MinValue, long.MaxValue, long.MinValue];

        if (method.ParameterCount == 0)
        {
            yield return [];
            yield break;
        }

        for (int i = 0; i < baseValues.Length; i++)
        {
            var args = new object?[method.ParameterCount];
            for (int p = 0; p < args.Length; p++)
            {
                args[p] = ValueForType(method.ParameterTypes[p], baseValues[(i + p) % baseValues.Length]);
            }
            yield return args;
        }
    }

    /// <summary>
    /// Coerces a boundary value into a value the given ABI parameter type accepts.
    /// </summary>
    private static object? ValueForType(Neo.SmartContract.ContractParameterType type, object candidate)
    {
        BigInteger value = candidate switch
        {
            int i => i,
            long l => l,
            BigInteger b => b,
            _ => 0,
        };

        return type switch
        {
            Neo.SmartContract.ContractParameterType.Boolean => value != 0,
            Neo.SmartContract.ContractParameterType.Integer => value,
            Neo.SmartContract.ContractParameterType.String => value.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Neo.SmartContract.ContractParameterType.ByteArray => System.Text.Encoding.UTF8.GetBytes(value.ToString(System.Globalization.CultureInfo.InvariantCulture)),

            // An Array parameter receives a struct or an array. Passing an Integer here, as the
            // default branch used to, made every call with such a parameter fail during argument
            // conversion, which silently disabled the families that declare one.
            Neo.SmartContract.ContractParameterType.Array => new object?[] { value, value + 1 },

            // Any/object parameters need both runtime shapes to cover type patterns.
            Neo.SmartContract.ContractParameterType.Any => value.IsEven
                ? value
                : value.ToString(System.Globalization.CultureInfo.InvariantCulture),

            // A Hash160 parameter is a cross-contract call target. The hash only exists
            // after the callee is deployed, so a placeholder is passed here and resolved
            // at deployment time.
            Neo.SmartContract.ContractParameterType.Hash160 => new BehaviorDifferentialAnalyzer.ContractReference(MultiContractCallee),

            _ => value,
        };
    }

    /// <summary>
    /// Shares the canonical string instance already retained by the global coverage set.
    /// Corpus entries contain many repeated identities, so retaining each deserialized
    /// copy needlessly grows the working set during long campaigns and full replays.
    /// </summary>
    private string[] CanonicalCoverage(IEnumerable<string> coverage) => coverage
        .Select(key => _seenCoverage.TryGetValue(key, out string? canonical) ? canonical : key)
        .ToArray();

    /// <summary>
    /// Retains an input in the corpus, evicting the least instructive entry when the
    /// corpus is at capacity.
    /// </summary>
    /// <remarks>
    /// A plain growth stop freezes the corpus once the cap is reached even while the
    /// campaign still reaches new instructions. Evicting the entry with the smallest
    /// instruction count keeps the corpus bounded without throttling new coverage: an
    /// input is only replaced when something that reached strictly more instructions
    /// takes its place.
    /// </remarks>
    /// <param name="source">The input source.</param>
    /// <param name="instructionCount">The number of instructions the input reached.</param>
    /// <param name="coverage">The canonical coverage identities of the input.</param>
    /// <returns><c>true</c> when the corpus changed.</returns>
    internal bool TryAddCorpusEntry(string source, int instructionCount, string[] coverage)
    {
        if (_corpus.Count < MaxCorpusEntries)
        {
            _corpus.Add(new CorpusEntry(source, instructionCount, coverage));
            return true;
        }

        int weakest = 0;
        for (int i = 1; i < _corpus.Count; i++)
        {
            if (_corpus[i].InstructionCount < _corpus[weakest].InstructionCount)
            {
                weakest = i;
            }
        }

        // Only evict for a strictly more instructive input; an equal or weaker input
        // would churn the corpus without improving it.
        if (instructionCount <= _corpus[weakest].InstructionCount)
        {
            return false;
        }

        _corpus[weakest] = new CorpusEntry(source, instructionCount, coverage);
        return true;
    }

    /// <summary>
    /// Loads the persisted corpus and restores the coverage it already reached.
    /// </summary>
    /// <remarks>
    /// Restoring coverage identities is what makes a new campaign continue from the
    /// previous one: without it every persisted entry would look like it adds nothing and
    /// the corpus would never be mutated again.
    /// </remarks>
    private void LoadCorpus()
    {
        if (_corpusStore is null)
        {
            return;
        }

        foreach (PersistedEntry entry in _corpusStore.Load())
        {
            if (_corpus.Count >= MaxCorpusEntries)
            {
                break;
            }

            string[] coverage = entry.Coverage ?? [];
            foreach (string coverageKey in coverage)
            {
                _seenCoverage.Add(coverageKey);
                _restoredCoverage.Add(coverageKey);
            }

            if (SyntaxAwareMutator.HasExcessiveLoop(entry.Source))
            {
                SkippedCorpusSize++;
                continue;
            }
            _corpus.Add(new CorpusEntry(entry.Source, coverage.Length, CanonicalCoverage(coverage)));
        }

        RestoredCorpusSize = _corpus.Count;
    }

    /// <summary>
    /// Saves the corpus entries that reached new coverage during this campaign.
    /// </summary>
    /// <returns>The number of newly written entries.</returns>
    public int SaveCorpus()
    {
        if (_corpusStore is null)
        {
            return 0;
        }

        var entries = _corpus
            .Where(e => e.Coverage.Length > 0)
            .Select(e => new PersistedEntry(e.Source, e.InstructionCount, [])
            {
                Coverage = e.Coverage,
            });
        return _corpusStore.SaveAll(entries);
    }

    /// <summary>The number of corpus entries loaded from a previous campaign.</summary>
    public int RestoredCorpusSize { get; private set; }

    /// <summary>The number of persisted sources skipped because they contain excessive loops.</summary>
    public int SkippedCorpusSize { get; private set; }

    /// <summary>The number of persisted sources executed by a zero-iteration replay.</summary>
    public int ReplayedCorpusSize { get; private set; }

    /// <summary>
    /// Executes persisted reproducers instead of merely restoring their coverage metadata.
    /// </summary>
    private void ReplayCorpus(
        Stopwatch stopwatch,
        TimeSpan? timeBudget,
        CancellationToken cancellation)
    {
        foreach (CorpusEntry entry in _corpus.ToArray())
        {
            if (cancellation.IsCancellationRequested ||
                timeBudget is { } budget && stopwatch.Elapsed > budget ||
                StoppedOnSlowExecution)
            {
                break;
            }

            var iterationStopwatch = Stopwatch.StartNew();
            if (IsMultiContract(entry.Source))
            {
                MultiContractIterations++;
                ProcessMultiContract(entry.Source);
            }
            else
            {
                Process(entry.Source, null);
            }

            ReplayedCorpusSize++;
            if (ReplayedCorpusSize % ReferenceCollectionInterval == 0)
            {
                _crossLanguage.CollectUnloadedAssemblies();
                ReferenceCollectionCount++;
            }
            CheckIterationBudget(entry.Source, iterationStopwatch.Elapsed);
        }
    }

    /// <summary>
    /// The number of iterations that took the multi-contract path.
    /// </summary>
    /// <remarks>
    /// Reported so that a silently broken multi-contract path is visible: a count of
    /// zero means the path never ran, which would otherwise look like a clean result.
    /// </remarks>
    public int MultiContractIterations { get; private set; }

    /// <summary>The number of coverage identities restored from a previous campaign.</summary>
    public int RestoredCoverage => _restoredCoverage.Count;

    /// <summary>The configured end-to-end budget for one iteration.</summary>
    public TimeSpan IterationBudget => _iterationBudget;

    /// <summary>The number of CLR reference assemblies currently cached.</summary>
    public int CachedReferenceCount => _crossLanguage.CachedAssemblyCount;

    /// <summary>The number of CLR reference load contexts evicted and unloaded.</summary>
    public long EvictedReferenceCount => _crossLanguage.EvictedAssemblyCount;

    /// <summary>The number of periodic collections used to reclaim unloaded references.</summary>
    public long ReferenceCollectionCount { get; private set; }

    private void Record(FuzzFinding finding)
    {
        if (IsKnownDefect(finding))
        {
            _suppressedKnownDefects++;
            return;
        }

        // A repeat of a signature is the same defect found through another input. Count
        // it on the first finding, which keeps its source as the reproducer, instead of
        // reporting it again.
        if (_seenSignatures.TryGetValue(finding.Signature, out FuzzFinding? existing))
        {
            existing.Occurrences++;
            return;
        }

        _seenSignatures.Add(finding.Signature, finding);
        _findings.Add(finding);
    }

    /// <summary>
    /// Reports a build that <c>ContractManagement.Deploy</c> rejected, once per source.
    /// </summary>
    /// <remarks>
    /// Which builds failed matters for the triage: a failure of exactly one build is a
    /// divergence the optimizer introduced, whereas a failure of both points at the source
    /// or the compiler's artifact generation as a whole.
    /// </remarks>
    /// <param name="source">The source whose builds were deployed.</param>
    /// <param name="optimizedError">The deployment error of the optimized build, if it failed.</param>
    /// <param name="unoptimizedError">The deployment error of the unoptimized build, if it failed.</param>
    private void RecordDeploymentFailure(string source, string? optimizedError, string? unoptimizedError)
    {
        string detail = (optimizedError, unoptimizedError) switch
        {
            (not null, not null) when string.Equals(optimizedError, unoptimizedError, StringComparison.Ordinal) =>
                $"Neither build deployed: {optimizedError}",
            (not null, not null) =>
                $"Neither build deployed, and for different reasons: optimized='{optimizedError}', unoptimized='{unoptimizedError}'.",
            (not null, null) =>
                $"The optimized build failed to deploy ({optimizedError}) while the unoptimized build deployed.",
            _ =>
                $"The unoptimized build failed to deploy ({unoptimizedError}) while the optimized build deployed.",
        };

        Record(new FuzzFinding(FuzzOutcome.DeploymentFailure, source, detail));
    }

    /// <summary>
    /// Tells whether a call error is the engine running out of gas rather than the contract
    /// faulting.
    /// </summary>
    private static bool IsGasExhaustion(string? error) =>
        error is not null && error.Contains("Insufficient GAS", StringComparison.Ordinal);

    /// <summary>
    /// Identifies already-reported defects so they do not drown out new findings.
    /// </summary>
    /// <remarks>
    /// A struct assignment shares storage instead of copying it, and a struct nested inside
    /// another struct faults on the first field access. Both are reported in
    /// <c>docs/testing/fuzz-findings/struct-value-semantics.md</c> and are still covered by
    /// <c>--struct-probe</c>, which is the regression check for them. Value-producing void
    /// lambda bodies are also known to leave extra items on the evaluation stack.
    /// User-defined comparison operators are emitted as raw VM opcodes instead of calls to
    /// the operator methods, and the user-defined implicit conversion faults on execution;
    /// both are reported in <c>docs/testing/fuzz-findings/user-defined-operators.md</c> and
    /// are covered by <c>--operator-probe</c>. Matches require both the generated method
    /// name and the specific failure, so a different failure in the same family is still
    /// reported.
    /// </remarks>
    /// <param name="finding">The finding to inspect.</param>
    /// <returns><c>true</c> when the finding is a known defect.</returns>
    private static bool IsKnownDefect(FuzzFinding finding)
    {
        if (finding.Detail.Contains("structNested", StringComparison.OrdinalIgnoreCase) &&
            (finding.Detail.Contains("SETITEM", StringComparison.Ordinal) ||
             finding.Detail.Contains("PICKITEM", StringComparison.Ordinal)))
        {
            return true;
        }

        if (finding.Detail.Contains("Return value count mismatch", StringComparison.Ordinal) &&
            (finding.Detail.Contains("structClosure", StringComparison.OrdinalIgnoreCase) ||
             finding.Detail.Contains("voidCompound", StringComparison.OrdinalIgnoreCase) ||
             finding.Detail.Contains("voidIncrement", StringComparison.OrdinalIgnoreCase) ||
             finding.Detail.Contains("voidNestedAssignment", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if ((finding.Detail.Contains("structCopyIsIndependent", StringComparison.OrdinalIgnoreCase) ||
             finding.Detail.Contains("conditionalStructCopy", StringComparison.OrdinalIgnoreCase)) &&
            finding.Detail.Contains("in C# but", StringComparison.Ordinal))
        {
            return true;
        }

        // User-defined operators (see docs/testing/fuzz-findings/user-defined-operators.md):
        // ==/!= compare structurally instead of calling op_Equality, < faults with a cast
        // error, and the implicit conversion faults with the PrimitiveType range error.
        if (finding.Detail.Contains("vecEqPartial", StringComparison.OrdinalIgnoreCase) &&
            finding.Detail.Contains("in C# but", StringComparison.Ordinal))
        {
            return true;
        }

        if ((finding.Detail.Contains("vecRelational", StringComparison.OrdinalIgnoreCase) ||
             finding.Detail.Contains("vecMixed", StringComparison.OrdinalIgnoreCase)) &&
            finding.Detail.Contains("Specified cast is not valid", StringComparison.Ordinal))
        {
            return true;
        }

        return finding.Detail.Contains("vecImplicit", StringComparison.OrdinalIgnoreCase) &&
               (finding.Detail.Contains("PrimitiveType is out of range", StringComparison.Ordinal) ||
                finding.Detail.Contains("in C# but", StringComparison.Ordinal));
    }

    /// <summary>The number of known defects that were suppressed.</summary>
    public int SuppressedKnownDefects => _suppressedKnownDefects;

    /// <summary>
    /// The number of single-sided gas exhaustions that were dismissed because both builds
    /// agreed under a larger budget.
    /// </summary>
    public int GasDivergencesDismissed => _gasDivergencesDismissed;

    /// <summary>
    /// Releases the harnesses held by this runner.
    /// </summary>
    public void Dispose()
    {
        _compiler.Dispose();
        _compilerNoOpt.Dispose();
        _behavior.Dispose();
        _crossLanguage.Dispose();
    }
}

/// <summary>
/// An entry of the coverage-guided corpus.
/// </summary>
/// <param name="Source">The contract source that produced the coverage.</param>
/// <param name="InstructionCount">The number of distinct instructions the source reached.</param>
/// <param name="Coverage">The opcode/operand and branch-outcome identities the source reached.</param>
public sealed record CorpusEntry(string Source, int InstructionCount, string[] Coverage);

/// <summary>
/// Minimal view of a callable contract method.
/// </summary>
/// <param name="Name">The method name as recorded in the ABI.</param>
/// <param name="ParameterCount">The number of parameters.</param>
/// <param name="ParameterTypes">The ABI types of the parameters.</param>
/// <param name="ReturnType">The ABI return type.</param>
public sealed record ContractMethodAbi(
    string Name,
    int ParameterCount,
    Neo.SmartContract.ContractParameterType[] ParameterTypes,
    Neo.SmartContract.ContractParameterType ReturnType);
