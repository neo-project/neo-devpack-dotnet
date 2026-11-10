// Copyright (C) 2015-2026 The Neo Project.
//
// ReferenceWorker.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text.Json;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Runs CLR reference methods outside the fuzzer process. A timed-out method can only
/// be stopped reliably by terminating its process; unloading an assembly is cooperative.
/// </summary>
internal sealed class ReferenceWorker : IDisposable
{
    internal sealed record Argument(string Kind, string? Value);
    internal sealed record Call(string Method, Argument[] Arguments);
    internal sealed record Request(string Source, Call[] Calls);
    internal sealed record Outcome(bool Ok, string? Rendered, string? Error);
    internal sealed record Response(bool Compiled, Outcome[] Outcomes, int CachedAssemblies,
        long EvictedAssemblies, long Collections);

    private Process? _process;
    private long _lastEvicted;
    private long _lastCollections;

    internal int CachedAssemblyCount { get; private set; }
    internal long EvictedAssemblyCount { get; private set; }
    internal long CollectionCount { get; private set; }

    internal static Argument Encode(object? value) => value switch
    {
        null => new("null", null),
        bool b => new("bool", b ? "true" : "false"),
        BigInteger i => new("integer", i.ToString(CultureInfo.InvariantCulture)),
        string s => new("string", s),
        byte[] bytes => new("bytes", Convert.ToBase64String(bytes)),
        object?[] array => new("array", JsonSerializer.Serialize(array.Select(Encode).ToArray())),
        _ => throw new NotSupportedException($"Cannot send CLR reference argument of type {value.GetType().FullName}.")
    };

    private static object? Decode(Argument argument) => argument.Kind switch
    {
        "null" => null,
        "bool" => bool.Parse(argument.Value!),
        "integer" => BigInteger.Parse(argument.Value!, CultureInfo.InvariantCulture),
        "string" => argument.Value,
        "bytes" => Convert.FromBase64String(argument.Value!),
        "array" => JsonSerializer.Deserialize<Argument[]>(argument.Value!)!.Select(Decode).ToArray(),
        _ => throw new InvalidDataException($"Unknown reference argument kind '{argument.Kind}'.")
    };

    /// <summary>Returns all CLR results, or throws after killing a worker that exceeded the deadline.</summary>
    internal Response Execute(Request request, TimeSpan remaining)
    {
        if (remaining <= TimeSpan.Zero)
        {
            throw new TimeoutException("The iteration budget expired before CLR execution.");
        }

        try
        {
            EnsureStarted();
            using var timeout = new CancellationTokenSource(remaining);
            string message = JsonSerializer.Serialize(request);
            _process!.StandardInput.WriteLineAsync(message).WaitAsync(timeout.Token).GetAwaiter().GetResult();
            string? line = _process.StandardOutput.ReadLineAsync(timeout.Token).GetAwaiter().GetResult();
            if (line is null)
            {
                throw new InvalidDataException("The CLR reference worker exited without a response.");
            }

            Response response = JsonSerializer.Deserialize<Response>(line)
                ?? throw new InvalidDataException("The CLR reference worker returned an empty response.");
            CachedAssemblyCount = response.CachedAssemblies;
            EvictedAssemblyCount += response.EvictedAssemblies - _lastEvicted;
            CollectionCount += response.Collections - _lastCollections;
            _lastEvicted = response.EvictedAssemblies;
            _lastCollections = response.Collections;
            return response;
        }
        catch (OperationCanceledException)
        {
            Stop();
            throw new TimeoutException("CLR reference execution exceeded the iteration budget.");
        }
        catch
        {
            Stop();
            throw;
        }
    }

    private void EnsureStarted()
    {
        if (_process is { HasExited: false })
        {
            return;
        }

        string assemblyPath = typeof(ReferenceWorker).Assembly.Location;
        var start = new ProcessStartInfo("dotnet")
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        start.ArgumentList.Add(assemblyPath);
        start.ArgumentList.Add("--reference-worker");
        _process = Process.Start(start) ?? throw new InvalidOperationException("Could not start the CLR reference worker.");
        _lastEvicted = 0;
        _lastCollections = 0;
    }

    private void Stop()
    {
        if (_process is null)
        {
            return;
        }

        try
        {
            if (!_process.HasExited)
            {
                _process.Kill(entireProcessTree: true);
            }
        }
        catch (InvalidOperationException)
        {
            // The worker may exit between HasExited and Kill.
        }
        _process.Dispose();
        _process = null;
        CachedAssemblyCount = 0;
    }

    public void Dispose() => Stop();

    /// <summary>Child process entry point; one JSON request and response per line.</summary>
    internal static int Run()
    {
        TextWriter protocolOutput = Console.Out;
        // Reference code may write to Console.Out; keep that output out of the JSON protocol.
        Console.SetOut(TextWriter.Null);
        using var oracle = new CrossLanguageOracle();
        long collections = 0;
        long requests = 0;
        string? line;
        while ((line = Console.ReadLine()) is not null)
        {
            Request request = JsonSerializer.Deserialize<Request>(line)
                ?? throw new InvalidDataException("Empty reference request.");
            var assembly = oracle.TryCompileReference(request.Source);
            Response response;
            if (assembly is null)
            {
                response = new Response(false, [], 0, 0, 0);
            }
            else
            {
                Outcome[] outcomes = request.Calls.Select(call =>
                {
                    object?[] args = call.Arguments.Select(Decode).ToArray();
                    (bool ok, object? value, string? error) = oracle.TryInvoke(assembly, "FuzzReference", call.Method, args);
                    return new Outcome(ok, ok ? FuzzRunner.RenderClrValue(value) : null, error);
                }).ToArray();
                response = new Response(true, outcomes, 0, 0, 0);
            }

            if (++requests % 500 == 0)
            {
                oracle.CollectUnloadedAssemblies();
                collections++;
            }
            response = response with
            {
                CachedAssemblies = oracle.CachedAssemblyCount,
                EvictedAssemblies = oracle.EvictedAssemblyCount,
                Collections = collections,
            };
            protocolOutput.WriteLine(JsonSerializer.Serialize(response));
            protocolOutput.Flush();
        }
        return 0;
    }
}
