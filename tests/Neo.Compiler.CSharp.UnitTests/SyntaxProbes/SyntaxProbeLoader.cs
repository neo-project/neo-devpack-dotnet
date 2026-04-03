// Copyright (C) 2015-2026 The Neo Project.
//
// SyntaxProbeLoader.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

internal static class SyntaxProbeLoader
{
    private const string DocsFolderName = "docs";
    private const string SyntaxFolderName = "csharp-syntax";

    internal static IReadOnlyList<SyntaxProbeCase> Load()
    {
        var repoRoot = GetRepositoryRoot();
        var syntaxDocsPath = Path.Combine(repoRoot, DocsFolderName, SyntaxFolderName);
        if (!Directory.Exists(syntaxDocsPath))
        {
            throw new DirectoryNotFoundException($"Syntax documentation directory not found at '{syntaxDocsPath}'.");
        }

        var probeCases = new List<SyntaxProbeCase>();
        var files = Directory.EnumerateFiles(syntaxDocsPath, "csharp-*.md", SearchOption.TopDirectoryOnly)
            .Select(path => new
            {
                Path = path,
                Order = ParseVersionOrder(Path.GetFileNameWithoutExtension(path))
            })
            .OrderBy(static entry => entry.Order)
            .ThenBy(static entry => entry.Path, StringComparer.Ordinal);

        foreach (var entry in files)
        {
            var version = Path.GetFileNameWithoutExtension(entry.Path);
            probeCases.AddRange(ParseDocument(version, entry.Path));
        }

        return probeCases;
    }

    private static int ParseVersionOrder(string name)
    {
        if (name.StartsWith("csharp-", StringComparison.OrdinalIgnoreCase) && int.TryParse(name[7..], out var number))
        {
            return number;
        }
        return int.MaxValue;
    }

    private static IEnumerable<SyntaxProbeCase> ParseDocument(string version, string path)
    {
        var lines = File.ReadAllLines(path);
        var results = new List<SyntaxProbeCase>();

        int index = 0;
        while (index < lines.Length)
        {
            var line = lines[index].Trim();
            if (!line.StartsWith("### ", StringComparison.Ordinal))
            {
                index++;
                continue;
            }

            // Expected format: ### id - Title
            var header = line.Substring(4).Trim();
            var separatorIndex = header.IndexOf(" - ", StringComparison.Ordinal);
            if (separatorIndex < 0)
            {
                throw new FormatException($"Invalid syntax entry header '{line}' in '{path}'. Expected '### id - Title'.");
            }

            var id = header[..separatorIndex].Trim();
            var title = header[(separatorIndex + 3)..].Trim();

            string? statusValue = null;
            string? scopeValue = null;
            string? notesValue = null;

            index++;
            while (index < lines.Length)
            {
                var metaLine = lines[index].Trim();
                if (metaLine.Length == 0)
                {
                    index++;
                    continue;
                }

                if (metaLine.StartsWith("Status:", StringComparison.OrdinalIgnoreCase))
                {
                    statusValue = metaLine[7..].Trim();
                    index++;
                    continue;
                }

                if (metaLine.StartsWith("Scope:", StringComparison.OrdinalIgnoreCase))
                {
                    scopeValue = metaLine[6..].Trim();
                    index++;
                    continue;
                }

                if (metaLine.StartsWith("Notes:", StringComparison.OrdinalIgnoreCase))
                {
                    notesValue = metaLine[6..].Trim();
                    index++;
                    continue;
                }

                if (!metaLine.StartsWith("```", StringComparison.Ordinal))
                {
                    throw new FormatException($"Unexpected line '{metaLine}' before code block in '{path}'.");
                }

                var fenceInfo = metaLine[3..].Trim();
                if (!fenceInfo.StartsWith("csharp", StringComparison.OrdinalIgnoreCase))
                {
                    throw new FormatException($"Syntax example for '{id}' in '{path}' must use a csharp code block.");
                }

                index++;
                var snippetBuilder = new StringBuilder();
                while (index < lines.Length && !lines[index].StartsWith("```", StringComparison.Ordinal))
                {
                    snippetBuilder.AppendLine(lines[index]);
                    index++;
                }

                if (index >= lines.Length)
                {
                    throw new FormatException($"Unterminated code block for '{id}' in '{path}'.");
                }

                // Skip closing fence
                index++;

                if (statusValue is null)
                {
                    throw new FormatException($"Entry '{id}' in '{path}' is missing a Status line.");
                }

                if (scopeValue is null)
                {
                    throw new FormatException($"Entry '{id}' in '{path}' is missing a Scope line.");
                }

                var snippet = snippetBuilder.ToString().TrimEnd('\r', '\n');
                var scope = ParseScope(scopeValue, id, path);
                var status = ParseStatus(statusValue, id, path);

                results.Add(new SyntaxProbeCase(
                    id,
                    title,
                    version,
                    scope,
                    status,
                    snippet,
                    notesValue));

                break;
            }
        }

        return results;
    }

    private static SyntaxProbeScope ParseScope(string value, string id, string path) =>
        value.Trim().ToLowerInvariant() switch
        {
            "method" => SyntaxProbeScope.Method,
            "class" => SyntaxProbeScope.Class,
            "file" => SyntaxProbeScope.File,
            _ => throw new FormatException($"Unsupported scope '{value}' for entry '{id}' in '{path}'.")
        };

    private static SyntaxSupportStatus ParseStatus(string value, string id, string path) =>
        value.Trim().ToLowerInvariant() switch
        {
            "supported" => SyntaxSupportStatus.Supported,
            "unsupported" => SyntaxSupportStatus.Unsupported,
            _ => throw new FormatException($"Unsupported status '{value}' for entry '{id}' in '{path}'.")
        };

    internal static string GetRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")) ||
                File.Exists(Path.Combine(directory.FullName, "global.json")) ||
                File.Exists(Path.Combine(directory.FullName, "neo-devpack-dotnet.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate repository root from current directory.");
    }
}
