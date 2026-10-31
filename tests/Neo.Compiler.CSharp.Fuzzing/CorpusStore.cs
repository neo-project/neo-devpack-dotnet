// Copyright (C) 2015-2026 The Neo Project.
//
// CorpusStore.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Text;
using System.Text.Json;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// A corpus entry persisted on disk.
/// </summary>
/// <param name="Source">The contract source.</param>
/// <param name="InstructionCount">How many distinct instructions the source reached.</param>
/// <param name="CoveredOffsets">The instruction offsets the source reached.</param>
public sealed record PersistedEntry(string Source, int InstructionCount, int[] CoveredOffsets);

/// <summary>
/// Reads and writes the coverage-guided corpus.
/// </summary>
/// <remarks>
/// <para>
/// Without persistence every run starts from nothing: the same programs are rediscovered,
/// the same coverage plateau is reached, and the effort spent in a previous campaign is
/// lost. Persisting the corpus makes coverage accumulate across runs, so a nightly job
/// keeps making progress instead of repeating itself.
/// </para>
/// <para>
/// Entries are stored as JSON under a directory, one file per entry, named after a hash
/// of the source. Using a hash as the file name makes writes idempotent: re-recording an
/// input that is already present is a no-op rather than a duplicate.
/// </para>
/// </remarks>
public sealed class CorpusStore
{
    private readonly string _directory;

    /// <summary>
    /// Creates a store rooted at a directory.
    /// </summary>
    /// <param name="directory">The directory that holds the corpus.</param>
    public CorpusStore(string directory)
    {
        _directory = directory;
    }

    /// <summary>The directory that holds the corpus.</summary>
    public string Directory => _directory;

    /// <summary>
    /// Loads every persisted entry.
    /// </summary>
    /// <returns>The entries, ordered by how much coverage they reached.</returns>
    public IReadOnlyList<PersistedEntry> Load()
    {
        if (!System.IO.Directory.Exists(_directory))
        {
            return [];
        }

        var entries = new List<PersistedEntry>();

        foreach (string path in System.IO.Directory.GetFiles(_directory, "*.json"))
        {
            try
            {
                string json = File.ReadAllText(path, Encoding.UTF8);
                PersistedEntry? entry = JsonSerializer.Deserialize<PersistedEntry>(json);
                if (entry is not null && !string.IsNullOrWhiteSpace(entry.Source))
                {
                    entries.Add(entry);
                }
            }
            catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
            {
                // A corrupt or unreadable entry must not abort the campaign; the next
                // run simply regenerates whatever this entry contributed.
                continue;
            }
        }

        return entries
            .OrderByDescending(e => e.InstructionCount)
            .ThenBy(e => e.Source, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Persists an entry, unless an identical source is already stored.
    /// </summary>
    /// <param name="entry">The entry to store.</param>
    /// <returns><c>true</c> when the entry was written; <c>false</c> when it already existed.</returns>
    public bool Save(PersistedEntry entry)
    {
        System.IO.Directory.CreateDirectory(_directory);

        string path = Path.Combine(_directory, Hash(entry.Source) + ".json");
        if (File.Exists(path))
        {
            return false;
        }

        var options = new JsonSerializerOptions { WriteIndented = false };
        File.WriteAllText(path, JsonSerializer.Serialize(entry, options), new UTF8Encoding(false));
        return true;
    }

    /// <summary>
    /// Saves every entry that is not stored yet.
    /// </summary>
    /// <param name="entries">The entries to save.</param>
    /// <returns>The number of newly written entries.</returns>
    public int SaveAll(IEnumerable<PersistedEntry> entries)
    {
        int written = 0;
        foreach (PersistedEntry entry in entries)
        {
            if (Save(entry))
            {
                written++;
            }
        }
        return written;
    }

    /// <summary>
    /// Computes the file-name hash of a source.
    /// </summary>
    private static string Hash(string source)
    {
        byte[] bytes = System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(source));
        return Convert.ToHexString(bytes)[..32].ToLowerInvariant();
    }
}
