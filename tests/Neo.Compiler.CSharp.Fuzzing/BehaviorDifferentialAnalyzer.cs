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
        ContractState state = _engine.Native.ContractManagement.Deploy(nefBytes, manifest);
        return state.Hash;
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
            if (!string.Equals(errorA, errorB, StringComparison.Ordinal))
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
    /// <returns>The results, the coverage reached, and the storage of every contract.</returns>
    public (List<(bool Ok, string? Error, StackItem? Value)> Results, IReadOnlyCollection<int> Coverage,
             IReadOnlyDictionary<string, string> Storage)
        ExecuteBundleWithCoverage(
            ArtifactBundle bundle,
            string entryContract,
            IReadOnlyList<(string Method, object?[] Args)> calls)
    {
        var results = new List<(bool Ok, string? Error, StackItem? Value)>(calls.Count);
        IReadOnlyCollection<int> coverage = [];
        IReadOnlyDictionary<string, string> storage = new Dictionary<string, string>(StringComparer.Ordinal);

        if (calls.Count == 0 || !bundle.Contracts.ContainsKey(entryContract))
        {
            return (results, coverage, storage);
        }

        EngineCheckpoint checkpoint = _engine.Checkpoint();
        try
        {
            var hashes = new Dictionary<string, UInt160>(StringComparer.Ordinal);
            foreach ((string name, ArtifactSet artifacts) in bundle.Contracts)
            {
                hashes[name] = Deploy(artifacts);
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
            string error = $"{ex.GetType().Name}: {ex.Message}";
            for (int i = 0; i < calls.Count; i++)
            {
                results.Add((false, error, null));
            }
        }
        finally
        {
            _engine.Restore(checkpoint);
        }

        return (results, coverage, storage);
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
    /// Returns the instruction pointers that were executed for a deployed contract.
    /// </summary>
    /// <remarks>
    /// The coverage data is what turns the fuzzer from a random search into a
    /// coverage-guided one: the set of executed instructions tells the harness which
    /// parts of the compiler's output are actually being exercised, so corpus entries
    /// that reach new instructions can be preferred and retained.
    /// </remarks>
    /// <param name="contractHash">The hash of the deployed contract.</param>
    /// <returns>The executed instruction pointers, or an empty set when no coverage was captured.</returns>
    public IReadOnlyCollection<int> GetExecutedInstructions(UInt160 contractHash)
    {
        if (!_engine.Coverage.TryGetValue(contractHash, out CoveredContract? covered))
        {
            return [];
        }

        return covered.Lines
            .Where(l => l.Hits > 0)
            .Select(l => l.Offset)
            .ToHashSet();
    }

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
    /// <returns>The call results, the executed instruction pointers, and the storage state.</returns>
    public (List<(bool Ok, string? Error, StackItem? Value)> Results, IReadOnlyCollection<int> Coverage,
             IReadOnlyDictionary<string, string> Storage)
        ExecuteBatchWithCoverage(
            ArtifactSet artifacts,
            IReadOnlyList<(string Method, object?[] Args)> calls)
    {
        var (results, coverage, storage, _) = ExecuteBatchWithNotifications(artifacts, calls);
        return (results, coverage, storage);
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
    /// <returns>The call results, coverage, storage, and the rendered notifications per call.</returns>
    public (List<(bool Ok, string? Error, StackItem? Value)> Results, IReadOnlyCollection<int> Coverage,
             IReadOnlyDictionary<string, string> Storage, List<IReadOnlyList<string>> Notifications)
        ExecuteBatchWithNotifications(
            ArtifactSet artifacts,
            IReadOnlyList<(string Method, object?[] Args)> calls)
    {
        var results = new List<(bool Ok, string? Error, StackItem? Value)>(calls.Count);
        var notifications = new List<IReadOnlyList<string>>(calls.Count);
        IReadOnlyCollection<int> coverage = [];
        IReadOnlyDictionary<string, string> storage = new Dictionary<string, string>(StringComparer.Ordinal);

        if (calls.Count == 0)
        {
            return (results, coverage, storage, notifications);
        }

        EngineCheckpoint checkpoint = _engine.Checkpoint();
        using var watcher = new NotificationWatcher(_engine);
        try
        {
            UInt160 hash = Deploy(artifacts);

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
            // The deployment itself failed; every call in the batch fails the same way.
            string error = $"{ex.GetType().Name}: {ex.Message}";
            for (int i = 0; i < calls.Count; i++)
            {
                results.Add((false, error, null));
                notifications.Add([]);
            }
        }
        finally
        {
            _engine.Restore(checkpoint);
        }

        return (results, coverage, storage, notifications);
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
