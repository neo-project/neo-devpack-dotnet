// Copyright (C) 2015-2026 The Neo Project.
//
// BehaviorDifferentialAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Storage;
using Neo.VM;
using Neo.VM.Types;
using System.Numerics;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Executes two artifact sets in the Neo VM and compares their observable behaviour.
/// </summary>
/// <remarks>
/// <para>
/// This is the highest-value oracle in the system. The optimizer rewrites scripts
/// aggressively (jump compression, peephole rewrites, unreachable-code removal). Any
/// rewrite that changes observable behaviour is a consensus-level defect, because the
/// same source would then behave differently depending on which compiler settings
/// produced it.
/// </para>
/// <para>
/// The comparison is deliberately oracle-free: it does not need a reference
/// implementation of the C# semantics. It only needs the two compiler outputs to
/// agree with each other, which makes it free of false positives.
/// </para>
/// </remarks>
public sealed class BehaviorDifferentialAnalyzer : IDisposable
{
    private readonly TestEngine _engine;

    /// <summary>
    /// Creates an analyzer bound to a fresh in-memory engine.
    /// </summary>
    public BehaviorDifferentialAnalyzer()
    {
        _engine = new TestEngine(true);
    }

    /// <summary>
    /// Invokes a contract method by name through the <c>System.Contract_Call</c> syscall.
    /// </summary>
    /// <param name="contractHash">The hash of the deployed contract.</param>
    /// <param name="method">The method name.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>The raw stack item returned by the VM.</returns>
    public StackItem Invoke(UInt160 contractHash, string method, params object?[] args)
    {
        using var script = new ScriptBuilder();

        // System.Contract_Call expects the arguments packed into a single array on the
        // stack: push the arguments in reverse order, then the count, then PACK.
        // Pushing them individually makes the VM read the first argument as the array
        // itself, which surfaces as "Integer cannot be converted to Array".
        for (int i = args.Length - 1; i >= 0; i--)
        {
            PushArgument(script, args[i]);
        }

        script.EmitPush(args.Length);
        script.Emit(OpCode.PACK);

        script.EmitPush((BigInteger)(byte)CallFlags.All);
        script.EmitPush(method);
        script.EmitPush(contractHash.GetSpan());
        script.EmitSysCall(ApplicationEngine.System_Contract_Call);

        return _engine.Execute(script.ToArray());
    }

    /// <summary>
    /// Pushes a single argument onto the script stack.
    /// </summary>
    private static void PushArgument(ScriptBuilder script, object? arg)
    {
        switch (arg)
        {
            case null:
                // The VM represents null as an empty byte string.
                script.EmitPush(System.Array.Empty<byte>());
                break;
            case bool b:
                script.EmitPush(b);
                break;
            case BigInteger i:
                script.EmitPush(i);
                break;
            case int i:
                script.EmitPush((BigInteger)i);
                break;
            case long l:
                script.EmitPush((BigInteger)l);
                break;
            case string s:
                script.EmitPush(s);
                break;
            case byte[] bytes:
                script.EmitPush(bytes);
                break;
            // Any other sequence is an array argument, which a struct parameter also uses on the
            // ABI. Pushing it as a single value made the call fail before entering the contract,
            // so a method with an array parameter could never run.
            case System.Collections.IEnumerable items:
                {
                    var list = items.Cast<object?>().ToList();
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        PushArgument(script, list[i]);
                    }
                    script.EmitPush(list.Count);
                    script.Emit(OpCode.PACK);
                    break;
                }
            default:
                throw new ArgumentException($"Unsupported argument type '{arg.GetType().FullName}'.", nameof(arg));
        }
    }

    /// <summary>
    /// The gas budget a deployment runs under, in datoshi.
    /// </summary>
    /// <remarks>
    /// <c>ContractManagement.Deploy</c> charges a storage fee of 0.001 GAS per byte of NEF
    /// plus manifest, so a contract of about 20 KB costs as much as the engine's default
    /// 20 GAS budget. The two builds of one source differ in size, and when that threshold
    /// falls between them exactly one of them deploys — a resource limit of the test
    /// engine, not a semantic difference, yet it would surface as one. Deployment therefore
    /// runs under its own budget, large enough for the biggest contract the network accepts
    /// (512 KB of script plus a 64 KB manifest), while every call keeps the default budget
    /// so that a runaway loop is still cut short quickly.
    /// </remarks>
    private const long DeploymentGasBudget = 1_000L * 100_000_000L;

    /// <summary>
    /// Deploys an artifact set and returns its contract hash.
    /// </summary>
    /// <param name="artifacts">The artifacts to deploy.</param>
    /// <returns>The hash of the deployed contract.</returns>
    public UInt160 Deploy(ArtifactSet artifacts)
    {
        // ContractManagement.deploy expects the full serialized NEF file, not just the script.
        byte[] nefBytes;
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true))
            {
                artifacts.Nef.Serialize(writer);
            }
            nefBytes = stream.ToArray();
        }

        byte[] manifest = System.Text.Encoding.UTF8.GetBytes(artifacts.Manifest.ToJson().ToString(false));

        long callBudget = _engine.Fee;
        _engine.Fee = DeploymentGasBudget;
        try
        {
            ContractState state = _engine.Native.ContractManagement.Deploy(nefBytes, manifest);
            return state.Hash;
        }
        finally
        {
            _engine.Fee = callBudget;
        }
    }

    /// <summary>
    /// Compares failures, treating different observed depths at the same VM stack limit
    /// as the same resource-boundary outcome.
    /// </summary>
    public static bool FailuresEquivalent(string? errorA, string? errorB)
    {
        if (string.Equals(errorA, errorB, StringComparison.Ordinal))
            return true;

        string? signatureA = StackLimitSignature(errorA);
        return signatureA is not null &&
               string.Equals(signatureA, StackLimitSignature(errorB), StringComparison.Ordinal);
    }

    private static string? StackLimitSignature(string? error)
    {
        const string marker = "MaxStackSize exceed: ";
        if (error is null)
            return null;

        int markerIndex = error.IndexOf(marker, StringComparison.Ordinal);
        if (markerIndex < 0)
            return null;

        int observedStart = markerIndex + marker.Length;
        int slash = error.IndexOf('/', observedStart);
        if (slash < 0 ||
            !IsDigits(error.AsSpan(observedStart, slash - observedStart)) ||
            !IsDigits(error.AsSpan(slash + 1)))
        {
            return null;
        }

        return string.Concat(error.AsSpan(0, observedStart), error.AsSpan(slash + 1));
    }

    private static bool IsDigits(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
            return false;

        foreach (char c in value)
        {
            if (c is < '0' or > '9')
                return false;
        }
        return true;
    }

    /// <summary>
    /// Compares the behaviour of two artifact sets for a given method and argument set.
    /// </summary>
    /// <param name="labelA">Label of the first artifact set.</param>
    /// <param name="a">The first artifact set.</param>
    /// <param name="labelB">Label of the second artifact set.</param>
    /// <param name="b">The second artifact set.</param>
    /// <param name="method">The method to invoke.</param>
    /// <param name="args">The arguments to pass.</param>
    /// <returns>The divergence list; empty when behaviour matches.</returns>
    public List<DifferentialDivergence> CompareBehavior(
        string labelA, ArtifactSet a,
        string labelB, ArtifactSet b,
        string method,
        object?[] args)
    {
        var result = new List<DifferentialDivergence>();

        // Each artifact set is deployed and executed in its own storage snapshot so that
        // the two executions cannot influence one another. Deploying the same contract
        // twice in one engine would otherwise fail with "Contract Already Exists".
        (bool okA, string? errorA, StackItem? valueA) = ExecuteIsolated(a, method, args);
        (bool okB, string? errorB, StackItem? valueB) = ExecuteIsolated(b, method, args);

        if (!okA && !okB)
        {
            // Both failed the same way: not a divergence. The failure itself is only
            // interesting if the two sides disagree about whether it happened.
            if (!FailuresEquivalent(errorA, errorB))
            {
                result.Add(new DifferentialDivergence(
                    "BehaviorMismatch",
                    $"Method '{method}' failed in both {labelA} and {labelB} but with different errors: " +
                    $"{labelA}='{errorA}', {labelB}='{errorB}'."));
            }
            return result;
        }

        if (!okA)
        {
            result.Add(new DifferentialDivergence(
                "BehaviorMismatch",
                $"Method '{method}' with args [{string.Join(", ", args.Select(RenderArg))}] threw in {labelA} ({errorA}) " +
                $"but succeeded in {labelB} (returned {Render(valueB!)})."));
            return result;
        }

        if (!okB)
        {
            result.Add(new DifferentialDivergence(
                "BehaviorMismatch",
                $"Method '{method}' with args [{string.Join(", ", args.Select(RenderArg))}] threw in {labelB} ({errorB}) " +
                $"but succeeded in {labelA} (returned {Render(valueA!)})."));
            return result;
        }

        string renderedA = Render(valueA!);
        string renderedB = Render(valueB!);

        if (!string.Equals(renderedA, renderedB, StringComparison.Ordinal))
        {
            result.Add(new DifferentialDivergence(
                "BehaviorMismatch",
                $"Method '{method}' with args [{string.Join(", ", args.Select(RenderArg))}] returned {renderedA} in {labelA} but {renderedB} in {labelB}."));
        }

        return result;
    }

    /// <summary>
    /// A set of compiled contracts that call each other.
    /// </summary>
    /// <param name="Contracts">The artifacts, keyed by contract name.</param>
    public sealed record ArtifactBundle(IReadOnlyDictionary<string, ArtifactSet> Contracts);

    /// <summary>
    /// A reference to a deployed contract, used as an argument value.
    /// </summary>
    /// <remarks>
    /// A cross-contract call needs the callee's hash, but the hash only exists after the
    /// callee is deployed. Using a placeholder lets the argument set be built before
    /// deployment and resolved afterwards.
    /// </remarks>
    /// <param name="ContractName">The name of the contract to pass the hash of.</param>
    public sealed record ContractReference(string ContractName);

    /// <summary>
    /// Deploys every contract of a bundle and invokes the calls against one of them.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlike <see cref="ExecuteBatchWithCoverage"/>, this deploys several contracts into
    /// the same snapshot so that a cross-contract call can resolve its target. Deploying
    /// in name order would not be enough on its own — a callee may itself call — so the
    /// deployment order is the order the compiler reported, which places callees first.
    /// </para>
    /// <para>
    /// Arguments of type <see cref="ContractReference"/> are replaced with the deployed
    /// hash of the named contract. This is what makes the call actually reach the callee
    /// rather than fault on a zero hash.
    /// </para>
    /// </remarks>
    /// <param name="bundle">The contracts to deploy.</param>
    /// <param name="entryContract">The contract whose methods are invoked.</param>
    /// <param name="calls">The methods and arguments to invoke.</param>
    /// <returns>
    /// The results, the coverage reached, the storage of every contract, and the deployment
    /// error when one of the contracts could not be deployed.
    /// </returns>
    public (List<(bool Ok, string? Error, StackItem? Value)> Results, IReadOnlyCollection<string> Coverage,
             IReadOnlyDictionary<string, string> Storage, string? DeploymentError)
        ExecuteBundleWithCoverage(
            ArtifactBundle bundle,
            string entryContract,
            IReadOnlyList<(string Method, object?[] Args)> calls)
    {
        var results = new List<(bool Ok, string? Error, StackItem? Value)>(calls.Count);
        IReadOnlyCollection<string> coverage = [];
        IReadOnlyDictionary<string, string> storage = new Dictionary<string, string>(StringComparer.Ordinal);

        if (calls.Count == 0 || !bundle.Contracts.ContainsKey(entryContract))
        {
            return (results, coverage, storage, null);
        }

        EngineCheckpoint checkpoint = _engine.Checkpoint();
        try
        {
            var hashes = new Dictionary<string, UInt160>(StringComparer.Ordinal);
            try
            {
                foreach ((string name, ArtifactSet artifacts) in bundle.Contracts)
                {
                    hashes[name] = Deploy(artifacts);
                }
            }
            catch (Exception ex)
            {
                // Nothing ran, so there is no per-call result to report. The error is
                // returned on its own, and mirrored into every slot so that a caller which
                // indexes the results by call still sees the failure.
                string deploymentError = DescribeException(ex);
                for (int i = 0; i < calls.Count; i++)
                {
                    results.Add((false, deploymentError, null));
                }
                return (results, coverage, storage, deploymentError);
            }

            UInt160 entryHash = hashes[entryContract];

            foreach ((string method, object?[] args) in calls)
            {
                try
                {
                    object?[] resolved = ResolveArguments(args, hashes);
                    StackItem value = Invoke(entryHash, method, resolved);
                    results.Add((true, null, value));
                }
                catch (Exception ex)
                {
                    results.Add((false, DescribeException(ex), null));
                }
            }

            coverage = GetExecutedInstructions(entryHash);

            // Aggregate the storage of every contract: a cross-contract call can write to
            // the callee's storage, and that write must match between the two builds too.
            var combined = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach ((string name, UInt160 hash) in hashes)
            {
                foreach ((string key, string value) in CaptureStorage(hash))
                {
                    combined[$"{name}:{key}"] = value;
                }
            }
            storage = combined;
        }
        catch (Exception ex)
        {
            // Capturing coverage or storage failed after the calls ran. The per-call results
            // are kept; only a call that has no result yet is attributed the failure.
            string error = $"{ex.GetType().Name}: {ex.Message}";
            while (results.Count < calls.Count)
            {
                results.Add((false, error, null));
            }
        }
        finally
        {
            _engine.Restore(checkpoint);
        }

        return (results, coverage, storage, null);
    }

    /// <summary>
    /// Renders an exception with its inner exceptions, so a wrapped reflection failure
    /// does not hide the underlying VM error.
    /// </summary>
    /// <param name="ex">The exception to describe.</param>
    /// <returns>A single-line description including the inner chain.</returns>
    private static string DescribeException(Exception ex)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append(ex.GetType().Name).Append(": ").Append(ex.Message);

        for (Exception? inner = ex.InnerException; inner is not null; inner = inner.InnerException)
        {
            sb.Append(" <- ").Append(inner.GetType().Name).Append(": ").Append(inner.Message);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Replaces contract references in an argument list with the deployed hashes.
    /// </summary>
    private static object?[] ResolveArguments(object?[] args, IReadOnlyDictionary<string, UInt160> hashes)
    {
        var resolved = new object?[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] is ContractReference reference)
            {
                resolved[i] = hashes.TryGetValue(reference.ContractName, out UInt160? hash)
                    ? hash.GetSpan().ToArray()
                    : new byte[20];
            }
            else
            {
                resolved[i] = args[i];
            }
        }
        return resolved;
    }

    /// <summary>
    /// Deploys and invokes an artifact set inside an isolated storage snapshot.
    /// </summary>
    /// <param name="artifacts">The artifacts to deploy and invoke.</param>
    /// <param name="method">The method to invoke.</param>
    /// <param name="args">The arguments to pass.</param>
    /// <returns>Whether the call succeeded, the error text when it did not, and the value.</returns>
    public (bool Ok, string? Error, StackItem? Value) ExecuteIsolated(ArtifactSet artifacts, string method, object?[] args)
    {
        EngineCheckpoint checkpoint = _engine.Checkpoint();
        try
        {
            UInt160 hash = Deploy(artifacts);
            StackItem value = Invoke(hash, method, args);
            return (true, null, value);
        }
        catch (Exception ex)
        {
            return (false, $"{ex.GetType().Name}: {ex.Message}", null);
        }
        finally
        {
            _engine.Restore(checkpoint);
        }
    }

    /// <summary>
    /// Deploys and invokes an artifact set, and returns the runtime notifications it emitted.
    /// </summary>
    /// <param name="artifacts">The artifacts to deploy and invoke.</param>
    /// <param name="method">The method to invoke.</param>
    /// <param name="args">The arguments to pass.</param>
    /// <returns>The call result plus the emitted notifications, rendered in argument order.</returns>
    public (bool Ok, string? Error, StackItem? Value, IReadOnlyList<string> Notifications) ExecuteWithNotifications(
        ArtifactSet artifacts, string method, object?[] args)
    {
        EngineCheckpoint checkpoint = _engine.Checkpoint();
        using var watcher = new NotificationWatcher(_engine);
        try
        {
            UInt160 hash = Deploy(artifacts);
            StackItem value = Invoke(hash, method, args);
            var notifications = watcher.Notifications
                .Select(n => $"{n.EventName}:[{string.Join(",", n.State.Select(Render))}]")
                .ToList();
            return (true, null, value, notifications);
        }
        catch (Exception ex)
        {
            return (false, $"{ex.GetType().Name}: {ex.Message}", null, []);
        }
        finally
        {
            _engine.Restore(checkpoint);
        }
    }

    /// <summary>
    /// Returns semantic coverage identities for instructions executed by a deployed contract.
    /// </summary>
    /// <remarks>
    /// Offsets are local to one script and therefore collide across unrelated contracts.
    /// An opcode and its operand identify the operation itself, while branch identities
    /// retain which side of a conditional executed. This distinguishes ADD from SUB at
    /// the same offset without treating every newly generated script as wholly new coverage.
    /// </remarks>
    /// <param name="contractHash">The hash of the deployed contract.</param>
    /// <returns>The executed coverage identities, or an empty set when no coverage was captured.</returns>
    public IReadOnlyCollection<string> GetExecutedInstructions(UInt160 contractHash)
    {
        if (!_engine.Coverage.TryGetValue(contractHash, out CoveredContract? covered))
        {
            return [];
        }

        var instructions = covered.Lines
            .Where(l => l.Hits > 0)
            .ToDictionary(l => l.Offset);
        var coverage = instructions.Values
            .Select(l => "I:" + CreateCoverageKey(l.Instruction))
            .ToHashSet(StringComparer.Ordinal);

        foreach (CoverageBranch branch in covered.Branches)
        {
            if (!instructions.TryGetValue(branch.Offset, out CoverageHit? line))
            {
                continue;
            }

            string key = CreateCoverageKey(line.Instruction);
            if (branch.HitsPath1 > 0)
            {
                coverage.Add("B:" + key + ":true");
            }
            if (branch.HitsPath2 > 0)
            {
                coverage.Add("B:" + key + ":false");
            }
        }

        return coverage;
    }

    /// <summary>
    /// Builds a script-independent identity for an instruction.
    /// </summary>
    public static string CreateCoverageKey(Instruction instruction) =>
        $"{instruction.OpCode}:{Convert.ToHexString(instruction.Operand.Span)}";

    /// <summary>
    /// Captures the storage state of a deployed contract.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Comparing the return value alone is not enough: an optimizer rewrite that stores
    /// the wrong value while returning the same thing would slip through. Capturing the
    /// contract's storage closes that gap.
    /// </para>
    /// <para>
    /// The state is captured while the storage snapshot is still in place, so it reflects
    /// exactly the execution that just happened. Only the entries belonging to the given
    /// contract are returned, because the store also holds the native contracts' state.
    /// </para>
    /// </remarks>
    /// <param name="contractHash">The hash of the deployed contract.</param>
    /// <returns>The storage entries, keyed by their raw storage key.</returns>
    public IReadOnlyDictionary<string, string> CaptureStorage(UInt160 contractHash)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);

        ContractState? state = _engine.Native.ContractManagement.GetContract(contractHash);
        if (state is null)
        {
            return result;
        }

        foreach ((StorageKey key, StorageItem value) in _engine.Storage.Snapshot.Seek([], Neo.Persistence.SeekDirection.Forward))
        {
            if (key.Id != state.Id)
            {
                continue;
            }

            result[Convert.ToBase64String(key.Key.ToArray())] = Convert.ToBase64String(value.Value.ToArray());
        }

        return result;
    }

    /// <summary>
    /// Deploys an artifact set once and invokes many methods against it.
    /// </summary>
    /// <remarks>
    /// Deploying a contract is far more expensive than calling it, so the contract is
    /// deployed once per batch and every call in the batch reuses it. The whole batch
    /// runs inside one storage snapshot, so the two sides of a comparison still cannot
    /// influence each other.
    /// </remarks>
    /// <param name="artifacts">The artifacts to deploy.</param>
    /// <param name="calls">The methods and arguments to invoke.</param>
    /// <returns>One result per call, in the same order.</returns>
    public List<(bool Ok, string? Error, StackItem? Value)> ExecuteBatch(
        ArtifactSet artifacts,
        IReadOnlyList<(string Method, object?[] Args)> calls)
    {
        return ExecuteBatchWithCoverage(artifacts, calls).Results;
    }

    /// <summary>
    /// Deploys an artifact set once and invokes many methods against it.
    /// </summary>
    /// <remarks>
    /// Deploying a contract is far more expensive than calling it, so the contract is
    /// deployed once per batch and every call in the batch reuses it. The whole batch
    /// runs inside one storage snapshot, so the two sides of a comparison still cannot
    /// influence each other.
    /// </remarks>

    /// <summary>
    /// Deploys an artifact set once, invokes many methods against it, and reports the
    /// instruction pointers that were executed together with the resulting storage state.
    /// </summary>
    /// <remarks>
    /// This is the coverage-guided variant of <see cref="ExecuteBatch"/>. The coverage
    /// data is captured before the storage snapshot is restored, so it reflects exactly
    /// the execution that just happened. The same applies to the storage state.
    /// </remarks>
    /// <param name="artifacts">The artifacts to deploy.</param>
    /// <param name="calls">The methods and arguments to invoke.</param>
    /// <returns>
    /// The call results, the executed instruction pointers, the storage state, and the
    /// deployment error when the contract could not be deployed.
    /// </returns>
    public (List<(bool Ok, string? Error, StackItem? Value)> Results, IReadOnlyCollection<string> Coverage,
             IReadOnlyDictionary<string, string> Storage, string? DeploymentError)
        ExecuteBatchWithCoverage(
            ArtifactSet artifacts,
            IReadOnlyList<(string Method, object?[] Args)> calls)
    {
        var (results, coverage, storage, _, deploymentError) = ExecuteBatchWithNotifications(artifacts, calls);
        return (results, coverage, storage, deploymentError);
    }

    /// <summary>
    /// Deploys an artifact set once, invokes many methods, and captures the notifications each
    /// call emitted.
    /// </summary>
    /// <remarks>
    /// The notification arguments are the only output a listener sees, so they are captured per
    /// call: a batch executes many calls against one deployed contract, and slicing the capture
    /// by the count before and after each call attributes the notifications to the right one.
    /// </remarks>
    /// <param name="artifacts">The artifacts to deploy.</param>
    /// <param name="calls">The methods and arguments to invoke.</param>
    /// <returns>
    /// The call results, coverage, storage, the rendered notifications per call, and the
    /// deployment error when the contract could not be deployed.
    /// </returns>
    public (List<(bool Ok, string? Error, StackItem? Value)> Results, IReadOnlyCollection<string> Coverage,
             IReadOnlyDictionary<string, string> Storage, List<IReadOnlyList<string>> Notifications,
             string? DeploymentError)
        ExecuteBatchWithNotifications(
            ArtifactSet artifacts,
            IReadOnlyList<(string Method, object?[] Args)> calls)
    {
        var results = new List<(bool Ok, string? Error, StackItem? Value)>(calls.Count);
        var notifications = new List<IReadOnlyList<string>>(calls.Count);
        IReadOnlyCollection<string> coverage = [];
        IReadOnlyDictionary<string, string> storage = new Dictionary<string, string>(StringComparer.Ordinal);

        if (calls.Count == 0)
        {
            return (results, coverage, storage, notifications, null);
        }

        EngineCheckpoint checkpoint = _engine.Checkpoint();
        using var watcher = new NotificationWatcher(_engine);
        try
        {
            UInt160 hash;
            try
            {
                hash = Deploy(artifacts);
            }
            catch (Exception ex)
            {
                // Nothing ran, so there is no per-call result to report. The error is
                // returned on its own, and mirrored into every slot so that a caller which
                // indexes the results by call still sees the failure.
                string deploymentError = DescribeException(ex);
                for (int i = 0; i < calls.Count; i++)
                {
                    results.Add((false, deploymentError, null));
                    notifications.Add([]);
                }
                return (results, coverage, storage, notifications, deploymentError);
            }

            foreach ((string method, object?[] args) in calls)
            {
                int before = watcher.Notifications.Count;
                try
                {
                    StackItem value = Invoke(hash, method, args);
                    results.Add((true, null, value));
                }
                catch (Exception ex)
                {
                    results.Add((false, $"{ex.GetType().Name}: {ex.Message}", null));
                }

                notifications.Add(watcher.Notifications
                    .Skip(before)
                    .Select(n => $"{n.EventName}:[{string.Join(",", n.State.Select(Render))}]")
                    .ToList());
            }

            coverage = GetExecutedInstructions(hash);
            storage = CaptureStorage(hash);
        }
        catch (Exception ex)
        {
            // Capturing coverage or storage failed after the calls ran. The per-call results
            // are kept; only a call that has no result yet is attributed the failure.
            string error = $"{ex.GetType().Name}: {ex.Message}";
            while (results.Count < calls.Count)
            {
                results.Add((false, error, null));
            }
            while (notifications.Count < calls.Count)
            {
                notifications.Add([]);
            }
        }
        finally
        {
            _engine.Restore(checkpoint);
        }

        return (results, coverage, storage, notifications, null);
    }

    /// <summary>
    /// Re-runs a single call on two builds under a larger gas budget and reports whether
    /// they then agree.
    /// </summary>
    /// <remarks>
    /// A call can exhaust the default budget in one build and not in the other without any
    /// semantic difference, because the unoptimized build spends more gas per instruction
    /// executed. When exactly one side faults on gas, the comparison is repeated with ten
    /// times the budget: if both sides now succeed and return the same value, the
    /// divergence was the budget, not the code. The contract is deployed afresh for each
    /// side so that the batch's state cannot leak into the re-run.
    /// </remarks>
    /// <param name="a">The first build.</param>
    /// <param name="b">The second build.</param>
    /// <param name="method">The method to invoke.</param>
    /// <param name="args">The arguments to pass.</param>
    /// <returns><see langword="true"/> when both builds succeed with the same result under the larger budget.</returns>
    public bool AgreeUnderLargerBudget(ArtifactSet a, ArtifactSet b, string method, object?[] args)
    {
        (bool Ok, string Rendered) resultA = InvokeUnderBudget(a, method, args, _engine.Fee * 10);
        if (!resultA.Ok)
        {
            return false;
        }

        (bool Ok, string Rendered) resultB = InvokeUnderBudget(b, method, args, _engine.Fee * 10);
        return resultB.Ok && string.Equals(resultA.Rendered, resultB.Rendered, StringComparison.Ordinal);
    }

    private (bool Ok, string Rendered) InvokeUnderBudget(ArtifactSet artifacts, string method, object?[] args, long budget)
    {
        EngineCheckpoint checkpoint = _engine.Checkpoint();
        long callBudget = _engine.Fee;
        try
        {
            UInt160 hash = Deploy(artifacts);
            _engine.Fee = budget;
            return (true, Render(Invoke(hash, method, args)));
        }
        catch (Exception)
        {
            return (false, string.Empty);
        }
        finally
        {
            _engine.Fee = callBudget;
            _engine.Restore(checkpoint);
        }
    }

    /// <summary>
    /// Renders a stack item into a canonical, comparable text form.
    /// </summary>
    /// <param name="item">The stack item to render.</param>
    /// <returns>A deterministic textual representation.</returns>
    public static string Render(StackItem item)
    {
        if (item is null || item.IsNull)
        {
            return "<null>";
        }

        switch (item.Type)
        {
            case StackItemType.Boolean:
                return item.GetBoolean() ? "true" : "false";
            case StackItemType.Integer:
                return item.GetInteger().ToString(System.Globalization.CultureInfo.InvariantCulture);
            case StackItemType.ByteString:
            case StackItemType.Buffer:
                return "0x" + Convert.ToHexString(item.GetSpan());
            case StackItemType.Array:
            case StackItemType.Struct:
                return "[" + string.Join(",", AsArray(item).Select(Render)) + "]";
            case StackItemType.Map:
                return "{" + string.Join(",", AsMap(item).Select(kv => Render(kv.Key) + ":" + Render(kv.Value))) + "}";
            default:
                return item.ToString() ?? "<unknown>";
        }
    }

    private static string RenderArg(object? arg) => arg switch
    {
        null => "null",
        bool b => b ? "true" : "false",
        BigInteger i => i.ToString(System.Globalization.CultureInfo.InvariantCulture),
        string s => "\"" + s + "\"",
        _ => arg.ToString() ?? "null",
    };

    /// <summary>
    /// Extracts the elements of an array-like stack item.
    /// </summary>
    private static IEnumerable<StackItem> AsArray(StackItem item)
    {
        return item switch
        {
            Neo.VM.Types.Array a => a.SubItems,
            _ => [],
        };
    }

    /// <summary>
    /// Extracts the key/value pairs of a map-like stack item.
    /// </summary>
    private static IEnumerable<KeyValuePair<StackItem, StackItem>> AsMap(StackItem item)
    {
        if (item is not Neo.VM.Types.Map map)
        {
            yield break;
        }

        foreach (PrimitiveType key in map.Keys)
        {
            yield return new KeyValuePair<StackItem, StackItem>(key, map[key]);
        }
    }

    /// <summary>
    /// Converts a .NET argument into the stack item representation pushed onto the VM stack.
    /// </summary>
    private static StackItem ToStackItem(object? arg) => arg switch
    {
        null => StackItem.Null,
        bool b => b,
        BigInteger i => i,
        int i => (BigInteger)i,
        long l => (BigInteger)l,
        string s => s,
        byte[] bytes => bytes,
        _ => throw new ArgumentException($"Unsupported argument type '{arg.GetType().FullName}'.", nameof(arg)),
    };

    /// <summary>
    /// Releases the engine.
    /// </summary>
    public void Dispose()
    {
        // The engine holds no unmanaged resources of its own; dropping the reference is
        // enough. The method exists so that callers can scope it with using.
    }
}
