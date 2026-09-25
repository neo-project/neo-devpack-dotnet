// Copyright (C) 2015-2026 The Neo Project.
//
// GasSnapshot.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Neo.SmartContract.Testing.TestingStandards;

/// <summary>
/// Stores named gas measurements in a deterministic, JSON-compatible snapshot.
/// </summary>
public sealed class GasSnapshot
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly SortedDictionary<string, long> _measurements = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the measurements in ordinal name order.
    /// </summary>
    public IReadOnlyDictionary<string, long> Measurements => _measurements;

    /// <summary>
    /// Records a gas measurement.
    /// </summary>
    /// <param name="name">Stable name of the measured operation.</param>
    /// <param name="gasConsumed">Gas consumed by the operation.</param>
    /// <returns>This snapshot.</returns>
    public GasSnapshot Record(string name, long gasConsumed)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegative(gasConsumed);
        if (!_measurements.TryAdd(name, gasConsumed))
            throw new ArgumentException($"A gas measurement named '{name}' already exists.", nameof(name));
        return this;
    }

    /// <summary>
    /// Creates a snapshot from named measurements.
    /// </summary>
    public static GasSnapshot Capture(IEnumerable<(string Name, long GasConsumed)> measurements)
    {
        ArgumentNullException.ThrowIfNull(measurements);
        var snapshot = new GasSnapshot();
        foreach (var (name, gasConsumed) in measurements)
            snapshot.Record(name, gasConsumed);
        return snapshot;
    }

    /// <summary>
    /// Compares this snapshot with a baseline.
    /// </summary>
    /// <param name="baseline">Expected measurements.</param>
    /// <param name="tolerance">Allowed absolute difference in datoshi.</param>
    /// <returns>Only missing, added, or out-of-tolerance measurements.</returns>
    public IReadOnlyList<GasSnapshotDifference> Compare(GasSnapshot baseline, long tolerance = 0)
    {
        ArgumentNullException.ThrowIfNull(baseline);
        ArgumentOutOfRangeException.ThrowIfNegative(tolerance);

        return baseline._measurements.Keys
            .Union(_measurements.Keys, StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .Select(name =>
            {
                baseline._measurements.TryGetValue(name, out var expected);
                _measurements.TryGetValue(name, out var actual);
                var hasExpected = baseline._measurements.ContainsKey(name);
                var hasActual = _measurements.ContainsKey(name);
                var differs = !hasExpected || !hasActual
                    || Math.Abs((decimal)actual - expected) > tolerance;
                return differs ? new GasSnapshotDifference(name, hasExpected ? expected : null, hasActual ? actual : null) : null;
            })
            .Where(difference => difference is not null)
            .Select(difference => difference!)
            .ToArray();
    }

    /// <summary>
    /// Serializes the snapshot with stable property ordering.
    /// </summary>
    public string ToJson() => System.Text.Json.JsonSerializer.Serialize(_measurements, JsonOptions);

    /// <summary>
    /// Writes the snapshot to a UTF-8 JSON file.
    /// </summary>
    public void Save(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        File.WriteAllText(path, ToJson());
    }

    /// <summary>
    /// Parses a snapshot from JSON.
    /// </summary>
    public static GasSnapshot FromJson(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        var values = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, long>>(json)
            ?? throw new FormatException("Gas snapshot JSON must contain an object.");
        return Capture(values.Select(pair => (pair.Key, pair.Value)));
    }

    /// <summary>
    /// Loads a snapshot from a UTF-8 JSON file.
    /// </summary>
    public static GasSnapshot Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return FromJson(File.ReadAllText(path));
    }
}

/// <summary>
/// Describes one gas snapshot difference.
/// </summary>
public sealed record GasSnapshotDifference(string Name, long? Expected, long? Actual);
